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

public class DecryptResult : IDecryptResult {
    public byte[] PrivateKey { get; set; }
    public bool Compressed { get; set; }
}

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

        var scryptBuf = SCrypt.ComputeDerivedKey(secret, salt, 16384, 8, 8, 1, 64);
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

        var cipher = aes.CreateEncryptor();

        var cipherText = cipher.TransformFinalBlock(xorBuf, 0, xorBuf.Length);

        // 0x01 | 0x42 | flagByte | salt (4) | cipherText (32)
        var result = new byte[7 + 32];
        result[0] = 0x01;
        result[1] = 0x42;
        result[2] = (byte)(compressed ? 0xe0 : 0xc0);
        salt.CopyTo(result, 3);
        cipherText.CopyTo(result, 7);

        return result;
    }

    private static IDecryptResult decryptECMulti(byte[] buffer, string passphrase) {
        var flag = buffer[2];
        var compressed = (flag & 0x20) != 0;
        var hasLotSeq = (flag & 0x04) != 0;

        // assert((flag & 0x24), flag, "Invalid private key")

        var addressHash = buffer.Skip(2).Take(4).ToArray();
        var ownerEntropy = buffer.Skip(6).Take(8).ToArray();

        byte[] ownerSalt = null;
        // 4 bytes ownerSalt if 4 bytes lot/squence
        if (hasLotSeq) {
            ownerSalt = ownerEntropy.Take(4).ToArray();

            // else 8 bytes ownerSalt
        } else {
            ownerSalt = ownerEntropy;
        }

        var encryptedPart1 = buffer.Skip(14).Take(8).ToArray();
        var encryptedPart2 = buffer.Skip(22).Take(16).ToArray();

        var preFactor = SCrypt.ComputeDerivedKey(Encoders.Hex.DecodeData(passphrase), ownerSalt, 16384, 8, 8, 1, 32);

        byte[] passFactor = null;

        if(hasLotSeq) {
            var hashTarget = preFactor.Concat(ownerEntropy).ToArray();
            passFactor = HashAlgorithms.Hash256(hashTarget);
        } else {
            passFactor = preFactor;
        }

        var publicKey = getPublicKey(passFactor, true);

        var seedBPass = SCrypt.ComputeDerivedKey(publicKey, addressHash.Concat(ownerEntropy).ToArray(), 1024, 1, 1, 1, 64);
        var derivedHalf1 = seedBPass.Take(32).ToArray();
        var derivedHalf2 = seedBPass.Skip(32).Take(32).ToArray();

        var aes = new AesManaged() {
            Mode = CipherMode.ECB,
            Key = derivedHalf2,
            Padding = PaddingMode.None
        };

        var decipher = aes.CreateDecryptor();

        var decryptedPart2 = decipher.TransformFinalBlock(encryptedPart2, 0, encryptedPart2.Length);

        var tmp = new byte[decryptedPart2.Length];
        var tmp2 = derivedHalf1.Skip(16).Take(16).ToArray();
        for (var i = 0; i < tmp2.Length; ++i) {
            tmp[i] = (byte)(decryptedPart2[i] ^ tmp2[i]);
        }

        var seedBPart2 = tmp.Skip(8).Take(8).ToArray();

        var aes2 = new AesManaged() {
            Mode = CipherMode.ECB,
            Key = derivedHalf2,
            Padding = PaddingMode.None
        };

        var decipher2 = aes2.CreateDecryptor();
        var tmp3 = new byte[8];
        decipher2.TransformBlock(encryptedPart1, 0, 8, tmp3, 0);
        var tmp4 = tmp3.Concat(decipher2.TransformFinalBlock(tmp.Take(8).ToArray(), 0, 8)).ToArray();
        var tmp5 = derivedHalf1.Take(16).ToArray();
        var seedBPart1 = new byte[16];
        for (var i = 0; i < 16; ++i) {
            seedBPart1[i] = (byte)(tmp4[i] ^ tmp5[i]);
        }

        var seedB = seedBPart1.Concat(seedBPart2).Take(24).ToArray();
        //TODO: var privateKey = secp256k1.privateKeyTweakMul(HashAlgorithms.hash256(seedB), passFactor);
        byte[] privateKey = null;

        return new DecryptResult { PrivateKey = privateKey, Compressed = compressed };
    }

    private static IDecryptResult decryptRaw(byte[] buffer, string passphrase) {
        // 39 bytes: 2 bytes prefix, 37 bytes payload
        if (buffer.Length != 39) {
            throw new Bip38LengthError(39, buffer.Length);
        }

        if (buffer[0] != 0x01) {
            throw new Bip38PrefixError(0x01, buffer[0]);
        }

        // check if BIP38 EC multiply
        var type = buffer[1];
        if (type == 0x43) {
            return decryptECMulti(buffer, passphrase);
        }
        if (type != 0x42) {
            throw new Bip38TypeError(0x42, type);
        }

        var flagByte = buffer[2];
        var compressed = flagByte == 0xe0;
        if (!compressed && flagByte != 0xc0) {
            throw new Bip38CompressionError(0xc0, flagByte);
        }
        
    }
}

}
