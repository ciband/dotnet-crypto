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
using System.Text;
using System.IO;
using System.Security.Cryptography;
using ArkEcosystem.Crypto.Managers;
using System;

namespace ArkEcosystem.Crypto.Identities
{
    public static class WIF
    {
        public static string FromPassphrase(string passphrase, INetwork network)
        {
            var keys = Keys.FromPassphrase(passphrase);

            return FromKeys(keys, network);
        }

        public static string FromKeys(IKeyPair keys, INetwork network) {
            if (network == null) {
                network = ConfigManager.Get<INetwork>("network");
            }

            return wif_encode((byte)network.Wif, Encoders.Hex.DecodeData(keys.PrivateKey), keys.Compressed);
        }

        private static string wif_encode(byte version, byte[] privateKey, bool compressed) {
            if (privateKey.Length != 32) {
                throw new Exception("Invalid privateKey length");
            }

            var buffer = new byte[compressed ? 34 : 33];
            buffer[0] = version;
            privateKey.CopyTo(buffer, 1);

            if (compressed) {
                buffer[33] = 0x01;
            }

            return Base58.EncodeCheck(buffer);
        }
    }
}
