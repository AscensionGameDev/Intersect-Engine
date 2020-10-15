using System;
using System.Threading;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Platform
{

    /// <summary>
    ///     Platform-agnostic utility functions.
    /// </summary>
    public static class Neutral
    {

        private static DateTime sFirstTime = DateTime.Now;

        /// <summary>
        ///     Changes the mouse cursor.
        /// </summary>
        /// <param name="cursor">Cursor type.</param>
        public static void SetCursor(Cursor cursor)
        {
            Cursor.Current = cursor;
        }

        /// <summary>
        ///     Gets text from clipboard.
        /// </summary>
        /// <returns>Clipboard text.</returns>
        public static string GetClipboardText()
        {
            // code from http://forums.getpaint.net/index.php?/topic/13712-trouble-accessing-the-clipboard/page__view__findpost__p__226140
            var ret = String.Empty;
            var staThread = new Thread(
                () =>
                {
                    try
                    {
                        if (GameClipboard.Instance == null || !GameClipboard.Instance.ContainsText())
                        {
                            return;
                        }

                        ret = GameClipboard.Instance.GetText();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            );

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            // at this point either you have clipboard data or an exception
            return ret;
        }

        /// <summary>
        ///     Sets the clipboard text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        /// <returns>True if succeeded.</returns>
        public static bool SetClipboardText(string text)
        {
            var ret = false;
            var staThread = new Thread(
                () =>
                {
                    try
                    {
                        if (GameClipboard.Instance == null)
                        {
                            return;
                        }
                        GameClipboard.Instance.SetText(text);
                        ret = true;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            );

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            // at this point either you have clipboard data or an exception
            return ret;
        }

        /// <summary>
        ///     Gets elapsed time since this class was initalized.
        /// </summary>
        /// <returns>Time interval in seconds.</returns>
        public static float GetTimeInSeconds()
        {
            //[halfofastaple] Note:
            //  After 3.8 months, the difference in value will be greater than a second,
            //  which isn't a problem for most people (who will run this that long?), but
            //  if it is, we can convert this (and all timestamps that rely on this) to a double, 
            //  which will grow stale (time difference > 1s) after ~3,168,888 years 
            //  (that's gotta be good enough, right?)
            //P.S. someone fix those numbers if I'm wrong.
            return (float) (DateTime.Now - sFirstTime).TotalSeconds;
        }

        /// <summary>
        ///     Displays an open file dialog.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="startPath">Initial path.</param>
        /// <param name="extension">File extension filter.</param>
        /// <param name="callback">Callback that is executed after the dialog completes.</param>
        /// <returns>True if succeeded.</returns>
        public static bool FileOpen(string title, string startPath, string extension, Action<string> callback)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = startPath,
                DefaultExt = @"*.*",
                Filter = extension,
                CheckPathExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.Ok)
            {
                if (callback != null)
                {
                    callback(dialog.FileName);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(String.Empty);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Displays a save file dialog.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="startPath">Initial path.</param>
        /// <param name="extension">File extension filter.</param>
        /// <param name="callback">Callback that is executed after the dialog completes.</param>
        /// <returns>True if succeeded.</returns>
        public static bool FileSave(string title, string startPath, string extension, Action<string> callback)
        {
            var dialog = new SaveFileDialog
            {
                Title = title,
                InitialDirectory = startPath,
                DefaultExt = @"*.*",
                Filter = extension,
                CheckPathExists = true,
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() == DialogResult.Ok)
            {
                if (callback != null)
                {
                    callback(dialog.FileName);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(String.Empty);
                }

                return false;
            }

            return true;
        }

    }

}
