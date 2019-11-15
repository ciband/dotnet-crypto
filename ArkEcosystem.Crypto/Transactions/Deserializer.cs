// Author:
//       Brian Faust <brian@ark.io>
//
// Copyright (c) 2018 Ark Ecosystem <info@ark.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NBitcoin.DataEncoders;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using ArkEcosystem.Crypto.Enums;
using ArkEcosystem.Crypto.Identities;

namespace ArkEcosystem.Crypto.Transactions
{
    public static class Deserializer {
        public static ITransaction Deserialize(string serialized, IDeserializeOptions options) {
            return Deserialize(Encoders.Hex.DecodeData(serialized), options);
        }
        public static ITransaction Deserialize(byte[] serialized, IDeserializeOptions options) {
            ITransactionData data = new TransactionData();

            var buffer = new BinaryReader(new MemoryStream(serialized));
            deserializeCommon(data, buffer);

            var instance = TransactionTypeFactory.Create(data);
            deserializeVendorField(instance, buffer);

            // Deserialize type specific parts
            instance.Deserialize(buffer);

            deserializeSignatures(data, buffer);

            if (options.AcceptLegacyVersion || Utils.IsSupportedTransactionVersion(data.Version)) {
                if (data.Version == 1) {
                    applyV1Compatibility(data);
                }
            } else {
                throw new TransactionVersionError(data.Version);
            }

            instance.serialized = buffer;

            return instance;
        }

        private static void deserializeCommon(ITransactionData transaction, BinaryReader buf) {
            buf.ReadByte();
            transaction.Version = buf.ReadByte();
            transaction.Network = buf.ReadByte();

            if (transaction.Version == 1) {
                transaction.Type = buf.ReadByte();;
                transaction.Timestamp = buf.ReadUInt32();
            } else {
                transaction.TypeGroup = buf.ReadUInt32();
                transaction.Type = buf.ReadUInt16();
                transaction.Nonce = buf.ReadUInt64();
            }

            transaction.SenderPublicKey = Encoders.Hex.EncodeData(buf.ReadBytes(33));
            transaction.Fee = buf.ReadUInt64();
            transaction.Amount = 0;
        }

        private static void deserializeVendorField(ITransaction transaction, BinaryReader buf) {
            var vendorFieldLength = buf.ReadByte();
            if (vendorFieldLength > 0) {
                if (transaction.HasVendorField()) {
                    var vendorFieldLengthBuffer = buf.ReadBytes(vendorFieldLength);
                    transaction.Data.VendorField = Encoders.Hex.EncodeData(vendorFieldLengthBuffer);
                } else {
                    buf.ReadBytes(vendorFieldLength);
                }
            }
        }

        private static void deserializeSignatures(ITransactionData transaction, BinaryReader buf) {
            if (transaction.Version == 1) {
                deserializeECDSA(transaction, buf);
            } else {
                deserializeSchnorrOrECDSA(transaction, buf);
            }
        }

        private static void deserializeSchnorrOrECDSA(ITransactionData transaction, BinaryReader buf) {
            if (detectSchnorr(buf)) {
                deserializeSchnorr(transaction, buf);
            } else {
                deserializeECDSA(transaction, buf);
            }
        }

        private static void deserializeECDSA(ITransactionData transaction, BinaryReader buf) {
            Func<int> currentSignatureLength = () => {
                var reset = buf.BaseStream.Position;

                buf.ReadByte();
                var lengthHex = buf.ReadByte();

                buf.BaseStream.Seek(reset, SeekOrigin.Begin);
                return lengthHex + 2;
            };

            Func<bool> beginningMultiSignature = () => {
                var reset = buf.BaseStream.Position;

                var marker = buf.ReadByte();

                buf.BaseStream.Seek(reset, SeekOrigin.Begin);

                return marker == 255;
            };

            // second signature
            if (buf.BaseStream.CanRead && !beginningMultiSignature()) {
                buf.ReadByte();
                var multiSignature = Encoders.Hex.EncodeData(buf.ReadBytes((int)(buf.BaseStream.Length - buf.BaseStream.Position)));
                transaction.Signatures.Add(multiSignature);
            }

            if (buf.BaseStream.CanRead) {
                throw new InvalidTransactionBytesError("signature buffer not exhausted");
            }
        }

        private static void deserializeSchnorr(ITransactionData transaction, BinaryReader buf) {
            Func<bool> canReadNonMultiSignature = () => {
                var remaining = buf.BaseStream.Length - buf.BaseStream.Position;
                return remaining > 0 && ((remaining % 64) == 0 || (remaining % 65) != 0);
            };

            if (canReadNonMultiSignature()) {
                transaction.Signature = Encoders.Hex.EncodeData(buf.ReadBytes(64));
            }

            var remaining = buf.BaseStream.Length - buf.BaseStream.Position;
            if (remaining > 0) {
                if ((remaining %65) == 0) {
                    transaction.Signatures.Clear();

                    var count = remaining / 65;
                    var publicKeyIndexes = new Dictionary<UInt32, bool>();
                    for (var i = 0; i < count; ++i) {
                        var multiSignaturePart = Encoders.Hex.EncodeData(buf.ReadBytes(65));
                        var publicKeyIndex = UInt32.Parse(multiSignaturePart.Substring(0, 2), NumberStyles.HexNumber);

                        if(!publicKeyIndexes[publicKeyIndex]) {
                            publicKeyIndexes[publicKeyIndex] = true;
                        } else {
                            throw new DuplicateParticipantInMultiSignatureError();
                        }

                        transaction.Signatures.Add(multiSignaturePart);
                    }
                } else {
                    throw new InvalidTransactionBytesError("signature buffer not exhausted");
                }
            }
        }

        private static bool detectSchnorr(BinaryReader buf) {
            var remaining = buf.BaseStream.Length - buf.BaseStream.Position;

            // signature / secondSignature
            if ( remaining == 64 || remaining == 128) {
                return true;
            }

            // signatures of a multi signature transaction (type != 4)
            if ((remaining % 65) == 0) {
                return true;
            }

            // only possibility left is a type 4 transaction with and without a 'secondSignature'.
            if ((((remaining - 64) % 65) == 0) || (((remaining - 128) % 65) == 0)) {
                return true;
            }

            return false;
        }

        public static void applyV1Compatibility(ITransactionData transaction) {
            transaction.SecondSignature = !string.IsNullOrEmpty(transaction.SecondSignature) ? transaction.SecondSignature : transaction.signSignature;
            transaction.TypeGroup = TransactionTypeGroup.CORE;

            if (transaction.Type == TransactionTypes.VOTE) {
                transaction.RecipientId = Address.FromPublicKey(transaction.SenderPublicKey, transaction.Network);
            } else if (transaction.Type == TransactionTypes.MULTI_SIGNATURE) {
                transaction.Asset.MultiSignatureLegacy.Keysgroup = transaction.Asset.MultiSignatureLegacy.Keysgroup.Select(k => {
                    return k.StartsWith("+") ? k : $"+{k}";
                }).ToList();
            }
        }
    }
}
