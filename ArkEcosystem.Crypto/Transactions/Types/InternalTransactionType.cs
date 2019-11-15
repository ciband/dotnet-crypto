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
using ArkEcosystem.Crypto.Enums;

namespace ArkEcosystem.Crypto.Transactions
{

public class InternalTransactionType {
    public UInt32 Type { get; set; }
    public UInt32 TypeGroup { get; set; }

    public static InternalTransactionType From(UInt32 type, UInt32? typeGroup) {
        if (typeGroup == null) {
            typeGroup = TransactionTypeGroup.CORE;
        }

        var compositType = $"{typeGroup}-{type}";
        if (!Types.ContainsKey(compositType)) {
            Types[compositType] = new InternalTransactionType(type, typeGroup.Value);
        }

        return Types[compositType];
    }

    private static Dictionary<string, InternalTransactionType> Types = new Dictionary<string, InternalTransactionType>();

    private InternalTransactionType(UInt32 type, UInt32 typeGroup) {
        Type = type;
        TypeGroup = typeGroup;
    }

    public override string ToString() {
        if (TypeGroup == TransactionTypeGroup.CORE) {
            return $"Core/{Type}";
        }
        return $"{TypeGroup}/{Type}";
    }
}

}
