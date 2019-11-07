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
using NBitcoin.DataEncoders;
using NBitcoin.Crypto;
using System.IO;
using System;
using System.Linq;

namespace ArkEcosystem.Crypto {

public static class Base58 {
    public static string EncodeCheck(byte[] buffer) {
        var checksum = HashAlgorithms.hash256(buffer);
        var newBuffer = new byte[buffer.Length + 4];
        Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);
        Buffer.BlockCopy(checksum, 0, newBuffer, buffer.Length, 4);
        return Encoders.Base58Check.EncodeData(newBuffer);
    }

    public static byte[] DecodeCheck(string address) {
        var buffer = Encoders.Base58Check.DecodeData(address);
        var payload = buffer.Take(buffer.Length - 4).ToArray();

        var checksum = HashAlgorithms.hash256(payload);

        var c = BitConverter.ToUInt32(checksum, 0);
        var b = BitConverter.ToUInt32(buffer, buffer.Length - 4);
        if (!BitConverter.IsLittleEndian) {
            // Platform is big endian, byte swap
        }
        if (c != b) {
            throw new Exception("Invalid checksum");
        }

        return payload;
    }
}


}

}
