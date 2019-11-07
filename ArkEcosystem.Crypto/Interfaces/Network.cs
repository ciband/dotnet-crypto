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

public interface INetworkConfig {
    IExceptions Exceptions { get; set; }
    IBlockJson GenesisBlock { get; set; }
    Dictionary<string, dynamic> Milestones{ get; set; }
    INetwork Network { get; set; }
}

public interface INetwork
{
    string Name { get; set; }
    string MessagePrefix { get; set; }
    UInt64 Bip32Public { get; set; }
    UInt64 Bip32Private { get; set; }
    UInt64 PubKeyHash { get; set; }
    string NetHash { get; set; }
    UInt64 Wif { get; set; }
    UInt64 Slip44 { get; set; }
    UInt64 Aip20 { get; set; }
    string ClientToken { get; set; }
    string ClientSymbol { get; set; }
    string ClientExplorer { get; set; }
}

public interface IExceptions {
    List<string> Blocks { get; set; }
    List<string> Transactions { get; set; }
    Dictionary<string, string> OutlookTable { get; set; }
    Dictionary<string, string> TransactionIdFixTable { get; set; }
}
