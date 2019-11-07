// Copyright (c) 2019 Ark Ecosystem <info@ark.io>
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

using System;
using System.Linq;
using System.Text;
using ArkEcosystem.Crypto.Managers;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public class KeyPair : IKeyPair {
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public bool Compressed { get; set; }
}

public static class Keys {
    public static IKeyPair FromPassphrase(string passphrase, bool compressed = true) {
        return Keys.FromPrivateKey(HashAlgorithms.Sha256(Encoding.UTF8.GetBytes(passphrase)), compressed);
    }

    public static IKeyPair FromPrivateKey(byte[] privateKey, bool compressed = true) {
        return new KeyPair {
            PublicKey = Keys.FromPrivateKey(privateKey, compressed).PublicKey,
            PrivateKey = Encoders.Hex.EncodeData(privateKey),
            Compressed = compressed
        };
    }

    public static IKeyPair FromWIF(string wifKey, INetwork network) {
        if (network == null) {
            network = ConfigManager.Get<INetwork>("network");
        }
        var wif = wif_decode(Encoders.Hex.DecodeData(wifKey), network.Wif);

        if (wif.version != network.Wif) {
            throw new NetworkVersionError(network.Wif.ToString(), wif.version.ToString());
        }
        return new KeyPair {
            PublicKey = Keys.FromPrivateKey(wif.privateKey, wif.compressed).PublicKey,
            PrivateKey = Encoders.Hex.EncodeData(wif.privateKey),
            Compressed = wif.compressed
        };
    }

    private static (UInt64 version, bool compressed, byte[] privateKey) wif_decode(byte[] buffer, UInt64 version) {
        if (buffer[0] != version) {
            throw new Exception("Invalid network version");
        }

        if (buffer.Length == 33) {
            return (version: buffer[0], compressed: false, privateKey: buffer.Skip(1).ToArray());
        }

        if (buffer.Length != 34) { throw new Exception("Invalid WIF length"); }
        if (buffer[33] != 0x01) { throw new Exception("Invalid compression flag"); }

        return (version: buffer[0], compressed: true, privateKey: buffer.Skip(1).ToArray());
    }
}

}
