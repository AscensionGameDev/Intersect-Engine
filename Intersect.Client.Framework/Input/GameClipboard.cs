
namespace Intersect.Client.Framework.Input
{
    public abstract class GameClipboard
    {
        /// <summary>
        /// The current working instance of our clipboard.
        /// </summary>
        public static GameClipboard Instance;

        /// <summary>
        /// Set the contents of the clipboard.
        /// </summary>
        /// <param name="data">The data to place on the clipboard.</param>
        public abstract void SetText(string data);

        /// <summary>
        /// Get the current content of the clipboard.
        /// </summary>
        /// <returns>Returns a string with the current contents of the clipboard.</returns>
        public abstract string GetText();

        /// <summary>
        /// Checks whether the system clipboard contains any text at all.
        /// </summary>
        /// <returns></returns>
        public abstract bool ContainsText();


        /// <summary>
        /// Checks whether or not the underlying operating system has the capability to copy/paste and the required libraries installed.
        /// </summary>
        /// <returns>Returns whether or not we can copy/paste data to the clipboard.</returns>
        public abstract bool CanCopyPaste();
    }
}
