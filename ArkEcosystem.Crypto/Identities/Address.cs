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
using NBitcoin;
using SshNet.Security.Cryptography;
using System.IO;
using ArkEcosystem.Crypto.Managers;

namespace ArkEcosystem.Crypto.Identities
{
    public static class Address
    {
        static readonly RIPEMD160 Ripemd160 = new RIPEMD160();

        public static string FromPassphrase(string passphrase, byte networkVersion = 0)
        {
        }

        public static string FromPublicKey(string publicKey, byte networkVersion = 0)
        {
        }

        public static string FromWIF(string wif, byte networkVersion = 0) {

        }

        public static string FromMultiSignatureAsset(IMultiSignatureAsset asset, byte networkVersion = 0) {

        }

        public static string FromPrivateKey(Key privateKey, byte networkVersion = 0)
        {

        }

        public static string FromBuffer(byte[] buffer) {
            return Base58.EncodeCheck(buffer);
        }

        public static (byte[] addressBuffer, string addressError) ToBuffer(string address) {
            var buffer = Base58.DecodeCheck(address);
            var networkVersion = ConfigManager.Get<byte>("network.pubKeyHash");
            var addressBuffer = buffer;
            string addressError = null;

            if (buffer[0] != networkVersion) {
                addressError = $"Expected address network byte {networkVersion}, but got {buffer[0]}.";
            }

            return (addressBuffer, addressError);
        }

        public static bool Validate(string address, byte networkVersion = 0)
        {
            if (networkVersion == 0) {
                networkVersion = ConfigManager.Get<byte>("network.pubKeyHash");
            }

            try {
                return Base58.DecodeCheck(address)[0] == networkVersion;
            } catch {
                return false;
            }
        }
    }
}
