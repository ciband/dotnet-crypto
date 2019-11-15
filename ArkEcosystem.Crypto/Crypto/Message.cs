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

using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using ArkEcosystem.Crypto.Managers;

namespace ArkEcosystem.Crypto
{
    internal class MessageImpl : IMessage {
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public string Message { get; set; }

        internal MessageImpl() { }
    }
    public static class Message
    {
        public static IMessage Sign(string message, string passphrase) {
            var keys = Keys.FromPassphrase(passphrase);

            return new MessageImpl {
                PublicKey = keys.PublicKey,
                Signature = Hash.SignECDSA(createHash(message), keys),
                Message = message
            };
        }

        public static IMessage SignWithWif(string message, string wif, INetwork network) {
            if (network == null) {
                network = ConfigManager.Get<INetwork>("network");
            }

            var keys = Keys.FromWIF(wif, network);

            return new MessageImpl {
                PublicKey = keys.PublicKey,
                Signature = Hash.SignECDSA(createHash(message), keys),
                Message = message
            };
        }

        public static bool Verify(IMessage message) {
            return Hash.VerifyECDSA(
                createHash(message.Message),
                Encoders.Hex.DecodeData(message.Signature),
                Encoders.Hex.DecodeData(message.PublicKey)
            );
        }

        private static byte[] createHash(string message) {
            return HashAlgorithms.Sha256(message);
        }
    }
}
