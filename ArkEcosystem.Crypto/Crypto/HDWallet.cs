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
using ArkEcosystem.Crypto.Managers;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public static class HDWallet {
    public const byte slip44 = 111;

    public static ExtKey FromMnemonic(string mnemonic, string passphrase) {
        var m = new Mnemonic(mnemonic);
        var seed = m.DeriveSeed(passphrase);
        return new ExtKey(seed);
    }

    public static ExtKey FromKeys(IKeyPair keys, byte[] chainCode) {
        if (!keys.Compressed) {
            throw new Exception("BIP32 only allows compressed keys.");
        }
        //TODO:  Do we need to know ConfigManager.Get<byte>("network")
        return new ExtKey(new Key(Encoders.Hex.DecodeData(keys.PrivateKey)), chainCode);
    }

    public static IKeyPair GetKeys(ExtKey node) {
        return new KeyPair {
            PublicKey = node.GetPublicKey().ToString(),
            PrivateKey = node.PrivateKey.ToString(),
            Compressed = true
        };
    }

    public static ExtKey DeriveSlip44(ExtKey root, bool hardened = true) {
        var path = $"m/44'/{slip44}{(hardened ? "'" : "")}";
        return root.Derive(new KeyPath(path));
    }

    public static ExtKey DeriveNetwork(ExtKey root) {
        var index = ConfigManager.Get<int>("network.aip20");
        if (index == 0) { index = 1; }
        return DeriveSlip44(root).Derive(index, true);
    }
}

}
