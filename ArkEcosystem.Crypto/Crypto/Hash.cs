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

using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public static class Hash {
    public static string SignECDSA(byte[] hash, Key key) {
        return Hash.SignECDSA(hash, key);
    }

    public static bool VerifyECDSA(byte[] hash, byte[] signature, byte[] publicKey) {
        return Hash.VerifyECDSA(hash, signature, publicKey);
    }

    public static string SignSchnorr(byte[] hash, Key key) {
        var schnorr = new SchnorrSigner();
        var sig = schnorr.Sign(new uint256(hash), key);
        return sig.ToString();
    }

    public static bool VerifySchnorr(byte[] hash, byte[] signature, byte[] publicKey) {
        return Hash.VerifySchnorr(hash, signature, publicKey);
    }
}

}
