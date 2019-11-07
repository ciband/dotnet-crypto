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

namespace ArkEcosystem.Crypto.Enums
{
    public static class TransactionTypes
    {
        public const byte TRANSFER = 0;
        public const byte SECOND_SIGNATURE = 1;
        public const byte DELEGATE_REGISTRATION = 2;
        public const byte VOTE = 3;
        public const byte MULTI_SIGNATURE = 4;
        public const byte IPFS = 5;
        public const byte MULTI_PAYMENT = 6;
        public const byte DELEGATE_RESIGNATION = 7;
        public const byte HTLC_LOCK = 8;
        public const byte HTLC_CLAIM = 9;
        public const byte HTLC_REFUND = 10;
    }

    public static class TransactionTypeGroup {
        public const UInt16 TEST = 0;
        public const UInt16 CORE = 1;

        // Everything above is available to anyone
        public const UInt16 RESERVED = 1000;
    }

    public static class HtlcLockExpirationType {
        public const byte EPOCH_TIMESTAMP = 1;
        public const byte BLOCK_HEIGHT = 2;
    }
}
