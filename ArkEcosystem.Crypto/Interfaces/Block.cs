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
using System.Collections.Generic;


public interface IBlockVerification {
    bool Verified { get; set; }
    List<string> Errors { get; set; }
    bool ContainsMultiSignatures { get; set; }
}

public interface IBlock {
    string Serialized { get; set; }
    IBlockData Data { get; set; }
    List<ITransaction> Transactions { get; set; }
    IBlockVerification Verification { get; set; }

    IBlockData GetHeader();
    bool VerifySignature();
    IBlockVerification Verify();

    string ToString();
    IBlockJson ToJson();
}

public interface IBlockData {
    string Id { get; set; }
    string IdHex { get; set; }

    UInt32 Timestamp { get; set; }
    byte Version { get; set; }
    UInt64 Height { get; set; }
    string PreviousBlockHex { get; set; }
    string PreviousBlock { get; set; }
    UInt32 NumberOfTransactions { get; set; }
    string TotalAmount { get; set; }
    string TotalFee { get; set; }
    string Reward { get; set; }
    UInt32 PayloadLength { get; set; }
    string PayloadHash { get; set; }
    string GeneratorPublicKey { get; set; }

    string BlockSignature { get; set; }
    string Serialized { get; set; }
    List<ITransactionData> Transactions { get; set; }
}

public interface IBlockJson {
    string Id { get; set; }
    string IdHex { get; set; }

    UInt32 Timestamp { get; set; }
    byte Version { get; set; }
    UInt64 Height { get; set; }
    string PreviousBlockHex { get; set; }
    string PreviousBlock { get; set; }
    UInt32 NumberOfTransactions { get; set; }
    string TotalAmount { get; set; }
    string TotalFee { get; set; }
    string Reward { get; set; }
    UInt32 PayloadLength { get; set; }
    string PayloadHash { get; set; }
    string GeneratorPublicKey { get; set; }

    string BlockSignature { get; set; }
    string Serialized { get; set; }
    List<ITransactionJson> Transactions { get; set; }
}
