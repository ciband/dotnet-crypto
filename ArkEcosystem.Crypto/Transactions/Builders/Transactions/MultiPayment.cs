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
using System.Linq;
using System.Security.Cryptography;
using ArkEcosystem.Crypto.Enums;
using ArkEcosystem.Crypto.Managers;
using ArkEcosystem.Crypto.Transactions;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public class MultiPaymentBuilder : TransactionBuilder<MultiPaymentBuilder> {

    public MultiPaymentBuilder() : base() {
        Data.Type = TransactionTypes.MULTI_PAYMENT;
        Data.Fee = FeeManager.Get(TransactionTypes.MULTI_PAYMENT);
        Data.Payments = null;
        Data.Asset = new TransactionAsset { Payments = new List<IMultiPaymentItem>() };
        Data.Amount = 0;
    }

    public MultiPaymentBuilder AddPayment(string recipientId, UInt64 amount) {
        var limit = ConfigManager.GetMilestone().Value.multiPaymentLimit ?? 500;
        if (Data.Asset.Payments.Count >= limit) {
            throw new MaximumPaymentCountExceededError(Data.Asset.Payments.Count);
        }

        IMultiPaymentItem payment = new MultiPaymentItem {
            Amount = amount,
            RecipientId = recipientId
        };
        Data.Asset.Payments.Add(payment);

        return this;
    }

    public override ITransactionData GetStruct() {
        var struct = base.GetStruct();

        struct.SenderPublicKey = Data.SenderPublicKey;
        struct.VendorFieldHex = Data.VendorFieldHex;
        struct.Amount = Data.Amount;
        struct.Asset = Data.Asset;

        return struct;
    }

    protected override MultiPaymentBuilder Instance() {
        return this;
    }
}

}
