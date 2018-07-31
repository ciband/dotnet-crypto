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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArkEcosystem.Crypto.Tests.Configuration
{
    [TestClass]
    public class TransactionTest
    {
        [TestMethod]
        public void Serialize()
        {
            var fixture = TestHelper.ReadTransactionFixture("transfer", "passphrase");
            var transaction = fixture["data"];
            
            var serializedTransaction = transaction.Serialize();
            Assert.AreEqual(fixture["serialized"], serializedTransaction);
        }
        
        [TestMethod]
        public void Deserialize()
        {
            var fixture = TestHelper.ReadTransactionFixture("transfer", "passphrase");
            var serializedTransaction = fixture["serialized"];
            
            var transaction = Transaction.Deserialize(serializedTransaction);
            Assert.AreEqual(0, transaction.Type);
            Assert.AreEqual(200000000, transaction.Amount);
            Assert.AreEqual(10000000, transaction.Fee);
            Assert.AreEqual("D61mfSggzbvQgTUe6JhYKH2doHaqJ3Dyib", transaction.RecipientId);
            Assert.AreEqual(41268326, transaction.Timestamp);
            Assert.AreEqual("034151a3ec46b5670a682b0a63394f863587d1bc97483b1b6c70eb58e7f0aed192", transaction.SenderPublicKey);
            Assert.AreEqual("3044022002994b30e08b58825c8c16ebf2cc693cfe706fb26571674784ead098accc89d702205b79dedc752a84504ecfe4b9e1292997f22260ee4daa102d2d9a61432d93b286", transaction.Signature);
            Assert.AreEqual("da61c6cba363cc39baa0ca3f9ba2c5db81b9805045bd0b9fc58af07ad4206856", transaction.Id);
        }
        
        [TestMethod]
        public void ToDictionary()
        {
            var fixture = TestHelper.ReadTransactionFixture("transfer", "passphrase");
            var transaction = fixture["data"];
            var dictionary = transaction.ToDictionary();
            
            dictionary["amount"] = transaction.Amount,
            dictionary["asset"] = transaction.Asset,
            dictionary["fee"] = transaction.Fee,
            dictionary["id"] = transaction.Id,
            dictionary["network"] = transaction.Network,
            dictionary["recipientId"] = transaction.RecipientId,
            dictionary["secondSignature"] = transaction.SecondSignature,
            dictionary["senderPublicKey"] = transaction.SenderPublicKey,
            dictionary["signature"] = transaction.Signature,
            dictionary["signatures"] = transaction.Signatures,
            dictionary["signSignature"] = transaction.SignSignature,
            dictionary["timestamp"] = transaction.Timestamp,
            dictionary["type"] = transaction.Type,
            dictionary["vendorField"] = transaction.VendorField,
            dictionary["version"] = transaction.Version,
        }
        
        [TestMethod]
        public void ToJson()
        {
            var expectedJson = "{
              "type": 0,
              "amount": 200000000,
              "fee": 10000000,
              "recipientId": "D61mfSggzbvQgTUe6JhYKH2doHaqJ3Dyib",
              "timestamp": 41268326,
              "asset": {},
              "senderPublicKey": "034151a3ec46b5670a682b0a63394f863587d1bc97483b1b6c70eb58e7f0aed192",
              "signature": "3044022002994b30e08b58825c8c16ebf2cc693cfe706fb26571674784ead098accc89d702205b79dedc752a84504ecfe4b9e1292997f22260ee4daa102d2d9a61432d93b286",
              "id": "da61c6cba363cc39baa0ca3f9ba2c5db81b9805045bd0b9fc58af07ad4206856"
            }";
            
            var fixture = TestHelper.ReadTransactionFixture("transfer", "passphrase");
            var transaction = fixture["data"];
            var json = transaction.ToJson();
            
            Assert.AreEqual(expectedJson, json);
        }
    }
}
