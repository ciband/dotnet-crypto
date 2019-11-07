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

using NBitcoin;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArkEcosystem.Crypto.Managers;

namespace ArkEcosystem.Crypto.Transactions
{
    public class Transaction : ITransaction
    {
        static readonly System.Security.Cryptography.SHA256 Sha256 = System.Security.Cryptography.SHA256.Create();


        public string Id { get; private set; }
        public UInt32 TypeGroup { get; private set; }
        public UInt16 Type { get; private set; }
        public bool Verified { get { return IsVerified; } }
        public string Key { get; private set; }
        public UInt64 StaticFee {
            get {
                var milestones = ConfigManager.
            }
        }

        public Boolean IsVerified { get; set; }

        public ITransactionData Data { get; set; }
        public byte[] Serialized { get; set; }
        public UInt32 Timestamp { get; set; }

        public byte[] Serialize(ISerializeOption? options);
        public void Deserialize(byte[] buffer);

        public bool Verify();
        public ISchemaValidationResult VerifySchema(bool? strict);

        public ITransactionJson ToJson();

        public bool HasVendorField();

        public string GetId()
        {
            return Encoders.Hex.EncodeData(Sha256.ComputeHash(ToBytes(false, false)));
        }

        public string Sign(string passphrase)
        {
            SenderPublicKey = Encoders.Hex.EncodeData(Identities.PublicKey.FromPassphrase(passphrase).ToBytes());

            var signature = Identities.PrivateKey
                .FromPassphrase(passphrase)
                .Sign(new uint256(Sha256.ComputeHash(ToBytes())));

            return Encoders.Hex.EncodeData(signature.ToDER());
        }

        public string SecondSign(string passphrase)
        {
            var signature = Identities.PrivateKey
                .FromPassphrase(passphrase)
                .Sign(new uint256(Sha256.ComputeHash(ToBytes(false))));

            return Encoders.Hex.EncodeData(signature.ToDER());
        }

        public string MultiSign(string passphrase, int index = -1)
        {
            if (Signatures == null) {
                Signatures = new List<string>();
            }
            index = index == -1 ? Signatures.Count : index;

            var schnorrSigner = new NBitcoin.Crypto.SchnorrSigner();
            var hash = new uint256(Sha256.ComputeHash(ToBytes(false)));
            var signature = Encoders.Hex.EncodeData(schnorrSigner.Sign(hash, Identities.PrivateKey.FromPassphrase(passphrase)).ToBytes());
            var indexedSignature = index.ToString("%X") + signature;
            Signatures.Add(indexedSignature);
            return indexedSignature;
        }

        public bool Verify()
        {
            var signature = Encoders.Hex.DecodeData(Signature);
            var transactionBytes = ToBytes();

            if (Version == 2) {
                var signer = new NBitcoin.Crypto.SchnorrSigner();
                signer.Verify(
                    new uint256(Sha256.ComputeHash(transactionBytes)),
                    Identities.PublicKey.FromHex(SenderPublicKey),
                    new NBitcoin.Crypto.SchnorrSignature(signature)
                );
            }
            return Identities.PublicKey
                .FromHex(SenderPublicKey)
                .Verify(new uint256(Sha256.ComputeHash(transactionBytes)), signature);
        }

        public bool SecondVerify(string secondPublicKey)
        {
            var signature = Encoders.Hex.DecodeData(SignSignature);
            var transactionBytes = ToBytes(false);

            return Identities.PublicKey
                .FromHex(secondPublicKey)
                .Verify(new uint256(Sha256.ComputeHash(transactionBytes)), signature);
        }

        public Transaction ParseSignatures(string serialized, int startOffset)
        {
            Signature = serialized.Substring(startOffset);

            var multiSignatureOffset = 0;

            if (Signature.Length == 0)
            {
                Signature = null;
            }
            else
            {
                var signatureLength = Convert.ToByte(Signature.Substring(2, 2), 16) + 2;
                Signature = serialized.Substring(startOffset, signatureLength * 2);
                multiSignatureOffset += signatureLength * 2;
                SecondSignature = serialized.Substring(startOffset + signatureLength * 2);

                if (SecondSignature.Length == 0)
                {
                    SecondSignature = null;
                }
                else
                {
                    if (SecondSignature.Substring(0, 2) == "ff")
                    {
                        SecondSignature = null;
                    }
                    else
                    {
                        var secondSignatureLength = Convert.ToByte(SecondSignature.Substring(2, 2), 16) + 2;
                        SecondSignature = SecondSignature.Substring(0, secondSignatureLength * 2);
                        multiSignatureOffset += secondSignatureLength * 2;
                    }
                }

                var signatures = serialized.Substring(startOffset + multiSignatureOffset);

                if (signatures.Length == 0)
                {
                    return this;
                }

                if (signatures.Substring(0, 2) != "ff")
                {
                    return this;
                }

                signatures = signatures.Substring(2);
                List<string> signaturesList = new List<string>();

                while (true)
                {
                    if (signatures == "")
                    {
                        break;
                    }

                    var multiSignatureLength = Convert.ToByte(signatures.Substring(2, 2), 16) + 2;

                    if (multiSignatureLength > 0)
                    {
                        signaturesList.Add(signatures.Substring(0, multiSignatureLength * 2));
                    }

                    signatures = signatures.Substring(multiSignatureLength * 2);
                }

                Signatures = signaturesList;
            }

            return this;
        }

        public byte[] ToBytes(bool skipSignature = true, bool skipSecondSignature = true)
        {
            MemoryStream stream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Type);
                writer.Write(Timestamp);
                writer.Write(Encoders.Hex.DecodeData(SenderPublicKey));

                var skipRecipientId = Type == Enums.TransactionTypes.SECOND_SIGNATURE_REGISTRATION || Type == Enums.TransactionTypes.MULTI_SIGNATURE_REGISTRATION;
                if (RecipientId != null && !skipRecipientId)
                {
                    writer.Write(Encoders.Base58Check.DecodeData(RecipientId));
                }
                else
                {
                    writer.Write(new byte[21]);
                }

                if (VendorField != null)
                {
                    var vendorFieldBytes = Encoding.ASCII.GetBytes(VendorField);

                    if (vendorFieldBytes.Length <= 255)
                    {
                        writer.Write(vendorFieldBytes);
                        if (vendorFieldBytes.Length < 64)
                        {
                            writer.Write(new byte[64 - vendorFieldBytes.Length]);
                        }
                    }
                }
                else
                {
                    writer.Write(new byte[64]);
                }

                writer.Write(Amount);
                writer.Write(Fee);

                if (Type == Enums.TransactionTypes.SECOND_SIGNATURE_REGISTRATION)
                {
                    writer.Write(Encoders.Hex.DecodeData(Asset["signature"]["publicKey"]));
                }

                if (Type == Enums.TransactionTypes.DELEGATE_REGISTRATION)
                {
                    writer.Write(Encoding.ASCII.GetBytes(Asset["delegate"]["username"]));
                }

                if (Type == Enums.TransactionTypes.VOTE)
                {
                    writer.Write(Encoding.ASCII.GetBytes(string.Join("", Asset["votes"])));
                }

                if (Type == Enums.TransactionTypes.MULTI_SIGNATURE_REGISTRATION)
                {
                    writer.Write((byte)Asset["multisignature"]["min"]);
                    writer.Write(Encoding.ASCII.GetBytes(string.Join("", Asset["multisignature"]["keysgroup"])));
                }

                if (!skipSignature && Signature != null)
                {
                    writer.Write(Encoders.Hex.DecodeData(Signature));
                }

                if (!skipSecondSignature && SignSignature != null)
                {
                    writer.Write(Encoders.Hex.DecodeData(SignSignature));
                }

                return stream.ToArray();
            }
        }

        public byte[] Serialize()
        {
            return new Serializer(this).Serialize();
        }

        public static Transaction Deserialize(string serialized)
        {
            return new Deserializer(serialized).Deserialize();
        }

        public Dictionary<string, dynamic> ToDictionary()
        {
            return new Dictionary<string, dynamic>
            {
                ["amount"] = Amount,
                ["asset"] = Asset,
                ["fee"] = Fee,
                ["id"] = Id,
                ["network"] = Network,
                ["recipientId"] = RecipientId,
                ["secondSignature"] = SecondSignature,
                ["senderPublicKey"] = SenderPublicKey,
                ["signature"] = Signature,
                ["signatures"] = Signatures,
                ["signSignature"] = SignSignature,
                ["timestamp"] = Timestamp,
                ["type"] = Type,
                ["vendorField"] = VendorField,
                ["version"] = Version,
            };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }
    }
}
