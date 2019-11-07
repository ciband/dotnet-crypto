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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcosystem.Crypto.Tests.Transactions.Builder
{
    [TestClass]
    public class MultiSignatureRegistrationTest
    {
        [TestMethod]
        public void Should_Be_Valid_With_A_Signature()
        {
            var publicKeys = new List<string>() {
                "039180ea4a8a803ee11ecb462bb8f9613fcdb5fe917e292dbcc73409f0e98f8f22",
                "028d3611c4f32feca3e6713992ae9387e18a0e01954046511878fe078703324dc0",
                "021d3932ab673230486d0f956d05b9e88791ee298d9af2d6df7d9ed5bb861c92dd"
            };
            var actual = Crypto.Transactions.Builder.MultiSignatureRegistration.Create(2, publicKeys);
            actual.SenderPublicKey = "039180ea4a8a803ee11ecb462bb8f9613fcdb5fe917e292dbcc73409f0e98f8f22";
            actual.MultiSign("secret 1", 0);
            actual.MultiSign("secret 2", 0);
            actual.MultiSign("secret 3", 0);
            actual.Signature = actual.Sign("secret 1");

            Assert.IsTrue(actual.Verify());
        }
    }
}
