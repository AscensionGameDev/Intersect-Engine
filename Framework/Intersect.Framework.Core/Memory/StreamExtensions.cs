namespace Intersect.Utilities;


public static partial class StreamExtensions
{

    public static void Pipe(this Stream input, Stream output)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        var buffer = new byte[8192];

        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }

}
