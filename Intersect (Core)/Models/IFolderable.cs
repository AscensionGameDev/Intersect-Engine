namespace Intersect.Models
{

    public interface IFolderable
    {

        /// <summary>
        /// Used to group editor items together into folders with the same name
        /// </summary>
        string Folder { get; set; }

    }

}
