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
using ArkEcosystem.Crypto.Managers;

namespace ArkEcosystem.Crypto {

public static class Utils {
    private static Dictionary<string, bool> genesisTransactions;
    private static Dictionary<string, bool> whitelistedBlockAndTransactionIds;
    private static byte currentNetwork;

    public static string FormatSatoshi(UInt64 amount) {
        var localString = (amount / Constants.SATOSHI).ToString("0:0.########");
        var symbol = ConfigManager.Get<string>("network.client.symbol");
        return $"{localString} {symbol}";
    }

    public static bool IsException(string blockOrTransactionId) {
        var network = ConfigManager.Get<byte>("network.pubKeyHash");

        if (whitelistedBlockAndTransactionIds == null || currentNetwork != network) {
            currentNetwork = network;

            var blocksAndTransactions = ConfigManager.Get<Dictionary<string, bool>>("exceptions.blocks")
            .Concat(ConfigManager.Get<Dictionary<string, bool>>("exceptions.transactions"))
            .Where((kvp) => kvp.Value);

            whitelistedBlockAndTransactionIds = new Dictionary<string, bool>();
            foreach(var item in blocksAndTransactions) {
                whitelistedBlockAndTransactionIds.Add(item.Key, item.Value);
            }
        }

        return whitelistedBlockAndTransactionIds[blockOrTransactionId];
    }

    public static bool IsGensisTransaction(string id) {
        var network = ConfigManager.Get<byte>("network.pubKeyHash");

        if (genesisTransactions == null || currentNetwork != network) {
            currentNetwork = network;

            var genesis = ConfigManager.Get<Dictionary<string, bool>>("genesisBlock.transactions")
            .Where((kvp) => kvp.Value);

            genesisTransactions = new Dictionary<string, bool>();
            foreach(var item in genesis) {
                genesisTransactions.Add(item.Key, item.Value);
            }
        }

        return genesisTransactions[id];
    }

    public static string NumberToHex<T>(T number) {
        return string.Format("X2", number);
    }

    public static UInt64 MaxVendorFieldLength(UInt32 height) {
        return ConfigManager.GetMilestone(height).Value.vendorFieldLength;
    }

    public static bool IsSupportedTransactionVersion(byte version) {
        bool aip11 = ConfigManager.GetMilestone().Value.aip11;

        if (aip11 && version != 2) {
            return false;
        }

        if (!aip11 && version != 1) {
            return false;
        }

        return true;
    }
}

}
