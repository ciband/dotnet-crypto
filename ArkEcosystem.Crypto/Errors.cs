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

namespace ArkEcosystem.Crypto {

public class CryptoError : Exception {
    public CryptoError(string message) : base(message) {}
}

public class Bip38CompressionError : CryptoError {
    public Bip38CompressionError(byte expected, byte given) : this(expected.ToString(), given.ToString()) { }
    public Bip38CompressionError(string expected, string given) :
        base($"Expected flag to be {expected}, but got {given}.") {}
}

public class Bip38LengthError : CryptoError {

    public Bip38LengthError(int expected, int given) : this(expected.ToString(), given.ToString()) {}
    public Bip38LengthError(string expected, string given) :
        base($"Expected length to be {expected}, but got {given}.") {}
}

public class Bip38PrefixError : CryptoError {

    public Bip38PrefixError(byte expected, byte given) : this(expected.ToString(), given.ToString()) { }
    public Bip38PrefixError(string expected, string given) :
        base($"Expected prefix to be {expected}, but got {given}.") {}
}

public class Bip38TypeError : CryptoError {

    public Bip38TypeError(byte expected, byte given) : this(expected.ToString(), given.ToString()) { }
    public Bip38TypeError(string expected, string given) :
        base($"Expected type to be {expected}, but got {given}.") {}
}

public class NetworkVersionError : CryptoError {
    public NetworkVersionError(string expected, string given) :
        base($"Expected version to be {expected}, but got {given}.") {}
}

public class NotImplementedError : CryptoError {
    public NotImplementedError(string expected, string given) :
        base("Feature is not available.") {}
}

public class PrivateKeyLengthError : CryptoError {

    public PrivateKeyLengthError(int expected, int given) : this(expected.ToString(), given.ToString()) { }
    public PrivateKeyLengthError(string expected, string given) :
        base($"Expected length to be {expected}, but got {given}.") {}
}

public class PublicKeyError : CryptoError {
    public PublicKeyError(string given) :
        base($"Expected {given} to be a valid public key.") {}
}

public class AddressNetworkError : CryptoError {
    public AddressNetworkError(string what) : base(what) {}
}

public class TransactionTypeError : CryptoError {
    public TransactionTypeError(string given) : base($"Type {given} not supported.") {}
}

public class InvalidTransactionBytesError : CryptoError {
    public InvalidTransactionBytesError(string message) :
        base($"Failed to deserialize transaction, encountered invalid bytes: {message}") {}
}

public class TransactionSchemaError : CryptoError {
    public TransactionSchemaError(string what) : base(what) {}
}

public class TransactionVersionError : CryptoError {
    public TransactionVersionError(string given) :
        base($"Version {given} not supported.") {}
}

public class UnkownTransactionError : CryptoError {
    public UnkownTransactionError(string given) :
        base($"Unknown transaction type: {given}") {}
}

public class TransactionAlreadyRegisteredError : CryptoError {
    public TransactionAlreadyRegisteredError(string name) :
        base($"Transaction type {name} is already registered.") {}
}

public class TransactionKeyAlreadyRegisteredError : CryptoError {
    public TransactionKeyAlreadyRegisteredError(string name) :
        base($"Transaction key {name} is already registered.") {}
}

public class CoreTransactionTypeGroupImmutableError : CryptoError {
    public CoreTransactionTypeGroupImmutableError() :
        base($"The Core transaction type group is immutable.") {}
}

public class MissingMilestoneFeeError : CryptoError {
    public MissingMilestoneFeeError(string name) :
        base($"Missing milestone fee for '{name}'.") {}
}

public class MaximumPaymentCountExceededError : CryptoError {
    public MaximumPaymentCountExceededError(UInt64 limit) :
        base($"Number of payments exceeded the allowed maximum of {limit}.") {}
}

public class MissingTransactionSignatureError : CryptoError {
    public MissingTransactionSignatureError() :
        base($"Expected the transaction to be signed.") {}
}

public class BlockSchemaError : CryptoError {
    public BlockSchemaError(UInt64 height, string what) :
        base($"Height ({height}): {what}") {}
}

public class PreviousBlockIdFormatError : CryptoError {
    public PreviousBlockIdFormatError(UInt64 thisBlockHeight, string previousBlockId) :
        base($"The config denotes that the block at height ${thisBlockHeight - 1}" +
                " must use full SHA256 block id, but the next block (at ${thisBlockHeight})" +
                " contains previous block id '{previousBlockId}'") {}
}

public class InvalidMilestoneConfigurationError : CryptoError {
    public InvalidMilestoneConfigurationError(string message) :
        base(message) {}
}

public class InvalidMultiSignatureAssetError : CryptoError {
    public InvalidMultiSignatureAssetError() :
        base($"The multi signature asset is invalid.") {}
}

public class DuplicateParticipantInMultiSignatureError : CryptoError {
    public DuplicateParticipantInMultiSignatureError() :
        base($"Invalid multi signature, because duplicate participant found.") {}
}

}
