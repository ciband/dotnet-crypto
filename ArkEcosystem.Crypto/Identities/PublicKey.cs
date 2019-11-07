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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ArkEcosystem.Crypto.Managers;
using NBitcoin;

namespace ArkEcosystem.Crypto.Identities
{
    public static class PublicKey
    {
        public static string FromPassphrase(string passphrase)
        {
            return Keys.FromPassphrase(passphrase).PublicKey;
        }

        public static string FromWIF(string wif, INetwork network) {
            return Keys.FromWIF(wif, network).PublicKey;
        }

        public static string FromMultiSignatureAsset(IMultiSignatureAsset asset) {
            var regex = new Regex("^[0-9A-Fa-f]{66}$");
            foreach(var pubKey in asset.PublicKeys) {
                if (!regex.IsMatch(pubKey)) {
                    throw new PublicKeyError(pubKey);
                }
            }

            if (asset.Min < 1 || asset.Min > asset.PublicKeys.Count) {
                throw new InvalidMultiSignatureAssetError();
            }

            var minKey = PublicKey.FromPassphrase(Utils.NumberToHex(asset.Min));
            List<string> keys = new List<string>();
            keys.Add(asset.Min.ToString("X2"));
            keys.AddRange(asset.PublicKeys);

            /*
            TODO: implement from TS
            return keys.reduce((previousValue: string, currentValue: string) =>
            secp256k1
                .publicKeyAdd(Buffer.from(previousValue, "hex"), Buffer.from(currentValue, "hex"), true)
                .toString("hex"),
            );
            */

            return null;
        }

        public static bool Validate(string publicKey, byte networkVersion) {
            if (networkVersion == 0) {
                networkVersion = ConfigManager.Get<byte>("network.pubKeyHash");
            }

            try {
                return Address.FromPublicKey(publicKey, networkVersion).Length == 34;
            } catch {
                return false;
            }
        }
    }
}
