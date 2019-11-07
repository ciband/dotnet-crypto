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
using System.Security.Cryptography;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public static class BIP38 {
    private static byte[] getPublicKey(byte[] buffer, bool compressed) {
        return Encoders.Hex.DecodeData(Keys.FromPrivateKey(buffer, compressed).PublicKey);
    }

    private static string getAddressPrivate(byte[] privateKey, bool compressed) {
        var publicKey = getPublicKey(privateKey, compressed);
        var buffer = HashAlgorithms.Hash160(publicKey);
        var payload = new byte[21];

        payload[0] = 0x00;
        buffer.CopyTo(payload, 1);

        return Base58.EncodeCheck(payload);
    }

    public static bool Verify(string bip38) {
        byte[] decoded = null;
        try {
            decoded = Base58.DecodeCheck(bip38);
        } catch {
            return false;
        }

        if (decoded == null ||
            decoded.Length != 39 ||
            decoded[0] != 0x01
        ) {
            return false;
        }

        var type = decoded[1];
        var flag = decoded[2];

        // encrypted WIF
        if (type == 0x42) {
            if (flag != 0xc0 && flag != 0xe0) {
                return false;
            }
        // EC mult
        } else if (type == 0x43) {
            if ((flag & ~0x24) != 0) {
                return false;
            }
        } else {
            return false;
        }

        return true;
    }

    private static byte[] encryptRaw(byte[] buffer, bool compressed, string passphrase) {
        if(buffer.Length != 32) {
            throw new PrivateKeyLengthError(32, buffer.Length);
        }

        var address = getAddressPrivate(buffer, compressed);

        var secret = Encoders.Hex.DecodeData(passphrase);
        var salt = HashAlgorithms.Hash256(address).Take(4).ToArray();

        var scryptBuf = SCrypt.BitcoinComputeDerivedKey(secret, salt, 64);
        var derivedHalf1 = scryptBuf.Take(32).ToArray();
        var derivedHalf2 = new byte[32];
        scryptBuf.CopyTo(derivedHalf2, 32);

        var xorBuf = new byte[buffer.Length];
        for (var i = 0; i < derivedHalf1.Length; ++i) {
            xorBuf[i] = (byte)(buffer[i] ^ derivedHalf1[i]);
        }

        var aes = new AesManaged() {
            Mode = CipherMode.ECB,
            Key = derivedHalf2,
            Padding = PaddingMode.None
        };

        aes.
    }

}

}
