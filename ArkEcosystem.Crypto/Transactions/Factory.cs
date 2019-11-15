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
using ArkEcosystem.Crypto.Transactions;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;

namespace ArkEcosystem.Crypto {

public static class TransactionFactory {
    public static ITransaction FromHex(string hex) {
        return FromSerialized(hex);
    }

    public static ITransaction FromBytes(byte[] buffer, bool strict = true) {
        return FromSerialized(buffer != null ? Encoders.Hex.EncodeData(buffer) : null, strict);
    }

    /**
     * Deserializes a transaction from `buffer` with the given `id`. It is faster
     * than `fromBytes` at the cost of vital safety checks (validation, verification and id calculation).
     *
     * NOTE: Only use this internally when it is safe to assume the buffer has already been
     * verified.
     */
    public static ITransaction FromBytesUnsafe(byte[] buffer, string id = null) {
        try {
            var transaction = Deserializer.Deserialize(buffer, new DeserializeOptions { AcceptLegacyVersion = true });
            transaction.Data.Id = !string.IsNullOrEmpty(id) ? id : Utils.GetId(transaction.Data, new SerializeOptions { AcceptLegacyVersion = true });
            transaction.IsVerified = true;

            return transaction;
        } catch (Exception ex) {
            throw new InvalidTransactionBytesError(ex.Message);
        }
    }

    public static ITransaction FromJson(ITransactionJson json) {
        var data = json as ITransactionData;  //TODO: Probably can't do this, make a common base class/interface
        //data.Amount = data.Amount; // big num?
        //data.Fee = data.Fee;       // big num?
        return FromData(data);
    }

    public static ITransaction FromData(ITransactionData data, bool strict = true) {
        ISchemaValidationResult result = Verifier.verifySchema(data, strict);

        if (result.Error && !isException(result.Value)) {
            throw new TransactionSchemaError(result.Error);
        }

        ITransaction transaction = TransactionTypeFactory.Create(result.Value);

        var version = transaction.Data;
        if (version == 1) {
            Deserializer.applyV1Compatibility(transaction.Data);
        }

        Serializer.serialize(transaction);

        return FromBytes(transaction.Serialized, strict);
    }

    public static ITransaction FromSerialized(string serialized, bool strict = true) {
        try {
            var transaction = Deserializer.Deserialize(serialized);
            transaction.Data.Id = Utils.GetId(transaction.Data);

            ISchemaValidationResult result = Verifier.verifySchema(transaction.Data, strict);

            if (result.Error && !isException(result.Value)) {
                throw new TransactionSchemaError(result.Error);
            }
            return transaction;
        } catch (Exception error) {
            if (error.GetType() == typeof(TransactionVersionError) ||
                error.GetType() == typeof(TransactionSchemaError) ||
                error.GetType() == typeof(DuplicateParticipantInMultiSignatureError))
            {
                throw error;
            }

            throw new InvalidTransactionBytesError(error.Message);
        }
    }
}

}
