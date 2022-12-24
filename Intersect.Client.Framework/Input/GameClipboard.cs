namespace Intersect.Client.Framework.Input
{
    public abstract partial class GameClipboard
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
        /// If the system clipboard contains any text.
        /// </summary>
        public abstract bool IsEmpty { get; }


        /// <summary>
        /// If clipboard support is enabled on this platform.
        /// </summary>
        public abstract bool IsEnabled { get; }
    }
}
