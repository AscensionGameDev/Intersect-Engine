namespace Intersect.Network.LiteNetLib;

public interface ISymmetricAlgorithm : IDisposable
{
    bool SetKey(ReadOnlySpan<byte> key);
    EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, out ReadOnlySpan<byte> plaindata);
    EncryptionResult TryDecrypt(ReadOnlySpan<byte> cipherdata, int offset, int length, out ReadOnlySpan<byte> plaindata);
    EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, out ReadOnlySpan<byte> cipherdata);
#if DEBUG
    EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, ReadOnlySpan<byte> nonce, out ReadOnlySpan<byte> cipherdata);
#endif
    EncryptionResult TryEncrypt(ReadOnlySpan<byte> plaindata, int offset, int length, out ReadOnlySpan<byte> cipherdata);
}