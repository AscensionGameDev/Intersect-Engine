namespace Intersect.Network.LiteNetLib;

public enum EncryptionResult
{
    Success,
    NoHeader,
    InvalidVersion,
    InvalidNonce,
    InvalidTag,
    EmptyInput,
    SizeMismatch,
    Error,
}