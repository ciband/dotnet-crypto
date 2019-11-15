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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArkEcosystem.Crypto.Managers;
using ArkEcosystem.Crypto.Enums;

namespace ArkEcosystem.Crypto.Transactions
{
    public class HtlcClaimTransaction : Transaction
    {
        public static UInt16 TYPE_GROUP = TransactionTypeGroup.CORE;
        public static UInt32 TYPE = TransactionTypes.HTLC_CLAIM;
        public static string KEY = "htlcClaim";

        //public static TransactionSchema GetSchema() {

        //}

        protected const UInt64 DefaultStaticFee = 0;

        public override bool Verify() {
            return ConfigManager.GetMilestone().Value.aip11 && base.Verify();
        }

        public byte[] Serialize(ISerializeOptions options) {
            var data = this.Data;
            var buffer = new List<byte>(32 + 32);
            buffer.AddRange(Encoders.Hex.DecodeData(data.Asset.Claim.LockTransactionId));
            buffer.AddRange(Encoders.Hex.DecodeData(data.Asset.Claim.UnlockSecret));

            return buffer.ToArray();
        }

        public void Deserialize(BinaryReader buf) {
            var data = this.Data;

            data.Asset.Claim.LockTransactionId = Encoders.Hex.EncodeData(buf.ReadBytes(32));
            data.Asset.Claim.UnlockSecret = Encoders.Hex.EncodeData(buf.ReadBytes(32));
        }
    }
}
