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

namespace ArkEcosystem.Crypto.Transactions
{
    public abstract class Transaction : ITransaction
    {
        static readonly System.Security.Cryptography.SHA256 Sha256 = System.Security.Cryptography.SHA256.Create();


        public string Id { get; private set; }
        public UInt32 TypeGroup { get; private set; }
        public UInt16 Type { get; private set; }
        public bool Verified { get { return IsVerified; } }
        public string Key { get; private set; }
        public UInt64 StaticFee {
            get {
                return getStaticFee(Data);
            }
        }

        public static TransactionSchema GetSchema() {
            throw new NotImplementedError();
        }

        public bool IsVerified { get; set; }

        public ITransactionData Data { get; set; }
        public byte[] Serialized { get; set; }
        public UInt32 Timestamp { get; set; }

        public abstract byte[] Serialize(ISerializeOption? options);
        public abstract void Deserialize(byte[] buffer);

        public virtual bool Verify() {
            return Verifier.Verify(Data);
        }

        public bool VerifySecondSignature(string publicKey) {
            return Verifier.VerifySecondSignature(Data, publicKey);
        }

        public ISchemaValidationResult VerifySchema(bool? strict) {
            return Verifier.VerifySchema(Data);
        }

        public ITransactionJson ToJson() {
            ITransactionJson data = null;
            return data;
        }

        public bool HasVendorField() { return false; }

        private UInt64 getStaticFee(ITransactionData data, uint? height = null) {
            var milestones = ConfigManager.GetMilestone(height);
            if (milestones.Value.fees && milestones.Value.fees.staticFees) {
                var fee = milestones.Value.fee.staticFees[Key];
                if (fee) {
                    return fee;
                }
            }

            return defaultStaticFee();
        }

        protected virtual UInt64 defaultStaticFee() { return 0; }
    }
}
