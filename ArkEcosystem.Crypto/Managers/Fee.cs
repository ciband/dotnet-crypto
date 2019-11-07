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

using System;
using System.Collections.Generic;
using ArkEcosystem.Crypto.Enums;

namespace ArkEcosystem.Crypto.Managers
{
    public static class FeeManager
    {
        public static Dictionary<UInt16, UInt64> fees = new Dictionary<UInt16, UInt64>();

        public static UInt64 Get(UInt16 type)
        {
            return fees[type];
        }

        public static void Set(UInt16 type, UInt64 value)
        {
            fees[type] = value;
        }

        public static UInt64 GetForTransaction(ITransactionData transaction) {
            var fee = fees[transaction.Type];

            if (transaction.Type == TransactionTypes.MULTI_SIGNATURE) {
                if (transaction.Version == 2) {
                    return fee * (ulong)(transaction.Asset.MultiSignature.PublicKeys.Count + 1);
                }
                return fee * (ulong)(transaction.Asset.MultiSignatureLegacy.Keysgroup.Count + 1);
            }
            return fee;
        }
    }
}
