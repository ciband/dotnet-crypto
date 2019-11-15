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

public interface ITransaction {
    string Id { get; }
    UInt32 TypeGroup { get; }
    UInt16 Type { get; }
    bool Verified { get; }
    string Key { get; }
    UInt64 StaticFee { get; }

    Boolean IsVerified { get; set; }

    ITransactionData Data { get; set; }
    byte[] Serialized { get; set; }
    UInt32 Timestamp { get; set; }

    byte[] Serialize(ISerializeOption? options);
    void Deserialize(byte[] buffer);

    bool Verify();
    ISchemaValidationResult VerifySchema(bool? strict);

    ITransactionJson ToJson();

    bool HasVendorField();
}

public interface ITransactionAsset {
    string SignaturePublicKey { get; set; }
    string DelegateUserName { get; set; }
    string DelegatePublicKey { get; set; }
    List<string> Votes { get; set; }
    IMultiSignatureLegacyAsset MultiSignatureLegacy { get; set; }
    IMultiSignatureAsset MultiSignature { get; set; }
    string Ipfs { get; set; }
    List<IMultiPaymentItem> Payments { get; set; }
    IHtlcLockAsset Lock { get; set; }
    IHtlcClaimAsset Claim { get; set; }
    IHtlcRefundAsset Refund { get; set; }
    Dictionary<string, dynamic> Custom { get; set; }
}

class TransactionAsset : ITransactionAsset {
    public string SignaturePublicKey { get; set; }
    public string DelegateUserName { get; set; }
    public string DelegatePublicKey { get; set; }
    public List<string> Votes { get; set; }
    public IMultiSignatureLegacyAsset MultiSignatureLegacy { get; set; }
    public IMultiSignatureAsset MultiSignature { get; set; }
    public string Ipfs { get; set; }
    public List<IMultiPaymentItem> Payments { get; set; }
    public IHtlcLockAsset Lock { get; set; }
    public IHtlcClaimAsset Claim { get; set; }
    public IHtlcRefundAsset Refund { get; set; }
    public Dictionary<string, dynamic> Custom { get; set; }
}

public interface ITransactionData {
    byte Version { get; set; }
    byte Network { get; set; }

    UInt32 TypeGroup { get; set; }
    UInt16 Type { get; set; }

    UInt32 Timestamp { get; set; }
    UInt64 Nonce { get; set; }
    string SenderPublicKey { get; set; }

    UInt64 Fee { get; set; }
    UInt64 Amount { get; set; }

    UInt32 Expiration { get; set; }
    string RecipientId { get; set; }

    ITransactionAsset Asset { get; set; }
    string VendorField { get; set; }

    string Id { get; set; }
    string Signature { get; set; }
    string SecondSignature { get; set; }
    string signSignature { get; set; }
    List<string> Signatures { get; set; }

    string BlockId { get; set; }
    UInt32 Sequence { get; set; }  //TODO check int size
}

public class TransactionData : ITransactionData {
    public byte Version { get; set; }
    public byte Network { get; set; }

    public UInt32 TypeGroup { get; set; }
    public UInt16 Type { get; set; }

    public UInt32 Timestamp { get; set; }
    public UInt64 Nonce { get; set; }
    public string SenderPublicKey { get; set; }

    public UInt64 Fee { get; set; }
    public UInt64 Amount { get; set; }

    public UInt32 Expiration { get; set; }
    public string RecipientId { get; set; }

    public ITransactionAsset Asset { get; set; }
    public string VendorField { get; set; }

    public string Id { get; set; }
    public string Signature { get; set; }
    public string SecondSignature { get; set; }
    public string signSignature { get; set; }
    public List<string> Signatures { get; set; }

    public string BlockId { get; set; }
    public UInt32 Sequence { get; set; }  //TODO check int size
}

public interface ITransactionJson {
    byte Version { get; set; }
    byte Network { get; set; }

    UInt32 TypeGroup { get; set; }
    UInt16 Type { get; set; }

    UInt32 Timestamp { get; set; }
    UInt64 Nonce { get; set; }
    string SenderPublicKey { get; set; }

    UInt64 Fee { get; set; }
    UInt64 Amount { get; set; }

    UInt32 Expiration { get; set; }
    string RecipientId { get; set; }

    ITransactionAsset Asset { get; set; }
    string VendorField { get; set; }

    string Id { get; set; }
    string Signature { get; set; }
    string SecondSignature { get; set; }
    string signSignature { get; set; }
    List<string> Signatures { get; set; }

    string BlockId { get; set; }
    UInt32 Sequence { get; set; }  //TODO check int size

    string IpfsHash {get; set; }
}

public interface ISchemaValidationResult {
    dynamic Value { get; set; }
    dynamic Error { get; set; }
    List<ErrorObject> Errors { get; set; }
}

public interface IMultiPaymentItem {
    UInt64 Amount { get; set; }
    string RecipientId { get; set; }
}

public class MultiPaymentItem : IMultiPaymentItem {
    public UInt64 Amount { get; set; }
    public string RecipientId { get; set; }
}

public interface IMultiSignatureLegacyAsset {
    byte Min { get; set; }
    UInt32 Lifetime { get; set; }
    List<string> Keysgroup { get; set; }
}

public interface IMultiSignatureAsset {
    byte Min { get; set; }
    List<string> PublicKeys { get; set; }
}

public class MultiSignatureAsset : IMultiSignatureAsset {
    public byte Min { get; set; }
    public List<string> PublicKeys { get; set; }
}

public interface IHtlcLockAsset {
    string SecretHash { get; set; }
    HtlcLockExpirationType ExpirationType { get; set; }
    UInt32 ExpirationValue { get; set; } // TODO check int size
}

public interface IHtlcClaimAsset {
    string LockTransactionId { get; set; }
    string UnlockSecret { get; set; }
}

public interface IHtlcRefundAsset {
    string LockTransactionId { get; set; }
}

public interface IHtlcLock : IHtlcLockAsset {
    UInt64 Amount { get; set; }
    string RecipientId { get; set; }
    UInt64 Timestamp { get; set; }
    string VendorField { get; set; }
}

public class IHtlcLocks : Dictionary<string, IHtlcLock> { }

public interface IHtlcExpiration {
    HtlcLockExpirationType Type { get; set; }
    UInt32 Value { get; set; } // TODO check int size
}

public interface IDeserializeOptions {
    bool AcceptLegacyVersion { get; set; }
}

public class DeserializeOptions : IDeserializeOptions{
    public bool AcceptLegacyVersion { get; set; }
}

public interface ISerializeOptions {
    bool AcceptLegacyVersion { get; set; }
    bool ExcludeSignature { get; set; }
    bool ExcludeSecondSignature { get; set; }
    bool ExcludeMultiSignature { get; set; }
    string AddressError { get; set; }
}

public class SerializeOptions : ISerializeOptions {
    public bool AcceptLegacyVersion { get; set; }
    public bool ExcludeSignature { get; set; }
    public bool ExcludeSecondSignature { get; set; }
    public bool ExcludeMultiSignature { get; set; }
    public string AddressError { get; set; }
}
