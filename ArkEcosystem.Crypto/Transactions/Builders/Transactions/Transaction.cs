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
using System.Linq;
using System.Security.Cryptography;
using ArkEcosystem.Crypto.Enums;
using ArkEcosystem.Crypto.Managers;
using ArkEcosystem.Crypto.Transactions;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public abstract class TransactionBuilder<TBuilder> {
    public ITransactionData Data { get; set; }

    protected bool signWithSenderAsRecipient = false;

    protected TransactionBuilder() {
        Data = new TransactionData {
            Id = null,
            Timestamp = Slots.GetTime(),
            TypeGroup = TransactionTypeGroup.TEST,
            Nonce = 0,
            Version = (byte)ConfigManager.GetMilestone().Value.aip11 ? 0x02 : 0x01
        };
    }

    public ITransaction Build(ITransactionData data) {
        return TransactionFactory.FromData(data, false);
    }

    public TBuilder Version(byte version) {
        Data.Version = version;

        return Instance();
    }

    public TBuilder TypeGroup(uint typeGroup) {
        Data.TypeGroup = typeGroup;

        return Instance();
    }

    public TBuilder Nonce(string nonce) {
        Data.Nonce = nonce;

        return Instance();
    }

    public TBuilder Network(byte network) {
        Data.Network = network;

        return Instance();
    }

    public TBuilder Fee(string fee) {
        if (!string.IsNullOrEmpty(fee)) {
            Data.Fee = ulong.Parse(fee);
        }
        return Instance();
    }

    public TBuilder Amount(string amount) {
        if (!string.IsNullOrEmpty(amount)) {
            Data.Amount = ulong.Parse(amount);
        }
        return Instance();
    }

    public TBuilder RecipientId(string recipientId) {
        Data.RecipientId = recipientId;

        return Instance();
    }

    public TBuilder SenderPublicKey(string publicKey) {
        Data.SenderPublicKey = publicKey;

        return Instance();
    }

    public TBuilder VenderField(string vendorField) {
        if (!string.IsNullOrEmpty(vendorField) && vendorField.Length <= maxVendorFieldLength()) {
            Data.VendorField = vendorField;
        }

        return Instance();
    }

    public TBuilder Sign(string passphrase) {
        var keys = Keys.FromPassphrase(passphrase);
        return SignWithKeyPair(keys);
    }

    public TBuilder SignWithWif(string wif, byte? networkWif) {
        var keys = getKeysFromNetworkWifOrDefault(wif, networkWif);
        return SignWithKeyPair(keys);
    }

    public TBuilder SecondSign(string secondPassphrase) {
        return SecondSignWithKeyPair(Keys.FromPassphrase(secondPassphrase));
    }

    public TBuilder SecondSignWithWif(string wif, byte? networkWif) {
        var keys = getKeysFromNetworkWifOrDefault(wif, networkWif);
        return SecondSignWithKeyPair(keys);
    }

    public TBuilder MultiSign(string passphrase, uint index) {
        var keys = Keys.FromPassphrase(passphrase);
        return MultiSignWithKeyPair(index, keys);
    }

    public TBuilder MultiSignWithWif(uint index, string wif, byte? networkWif) {
        var keys = getKeysFromNetworkWifOrDefault(wif, networkWif);
        return MultiSignWithKeyPair(index, keys);
    }

    public bool Verify() {
        return Verifier.VerifyHash(Data);
    }

    public virtual ITransactionData GetStruct() {
        if (string.IsNullOrEmpty(Data.SenderPublicKey) || (string.IsNullOrEmpty(Data.Signature) && Data.Signatures.Count <= 0)) {
            throw new MissingTransactionSignatureError();
        }

        ITransactionData struct = new TransactionData {
            Id = Utils.GetId(Data).ToString(),
            Signature = Data.Signature,
            SecondSignature = Data.SecondSignature,
            Version = Data.Version,
            Type = Data.Type,
            Fee = Data.Fee,
            SenderPublicKey = Data.SenderPublicKey,
            Network = Data.Network
        };

        if (Data.Version == 1) {
            struct.Timestamp = Data.Timestamp;
        } else {
            struct.TypeGroup = Data.TypeGroup;
            struct.Nonce = Data.Nonce;
        }

        // TODO:  From TS:  if (Array.isArray(this.data.signatures)) { ????
        struct.Signatures = Data.Signatures;

        return struct;
    }

    protected abstract TBuilder Instance();

    private IKeyPair getKeysFromNetworkWifOrDefault(string wif, byte? networkWif) {
        return Keys.FromWIF(wif, Networks.GetNetworkFromWif((networkWif != null ? networkWif.Value : ConfigManager.Get<byte>("network.wif"))));
    }
}

}
