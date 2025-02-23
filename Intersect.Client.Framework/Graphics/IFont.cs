namespace Intersect.Client.Framework.Graphics;

public interface IFont
{
    string Name { get; }

    ICollection<int> SupportedSizes { get; }

    int PickBestMatchFor(int size);

    int GetNextFontSize(int startSize, int direction, int limit = 0);
}