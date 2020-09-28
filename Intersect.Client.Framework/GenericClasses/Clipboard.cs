using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Intersect.Client.Framework.GenericClasses
{
    public static class Clipboard
    {
        /// <summary>
        /// Set the contents of the clipboard.
        /// </summary>
        /// <param name="data">The data to place on the clipboard.</param>
        public static void SetText(string data)
        {
            var platform = GetPlatform();
            switch (platform)
            {
                case PlatformID.Win32NT:
                    System.Windows.Forms.Clipboard.SetText(data);
                    break;
                case PlatformID.Unix:
                    // Are we running a Wayland shell?
                    if (ShellUsesWayland())
                    {
                        RunShell(UnixPlatforms.Linux, $"wl-copy {data}");
                    }
                    else
                    {
                        RunShell(UnixPlatforms.Linux, $"echo {data} | xclip -i");
                    }
                    break;
                case PlatformID.MacOSX:
                    RunShell(UnixPlatforms.MacOSX, $"echo {data} | pbcopy");
                    break;
                default:
                    // Send help!
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the current content of the clipboard.
        /// </summary>
        /// <returns>Returns a string with the current contents of the clipboard.</returns>
        public static string GetText()
        {
            var platform = GetPlatform();
            switch (platform)
            {
                case PlatformID.Win32NT:
                    return System.Windows.Forms.Clipboard.GetText();
                case PlatformID.Unix:
                    // Are we running a Wayland shell?
                    if (ShellUsesWayland())
                    {
                        return GetShellOutput(UnixPlatforms.Linux, "wl-paste");
                    }
                    else
                    {
                        return GetShellOutput(UnixPlatforms.Linux, "xclip -o");
                    }
                case PlatformID.MacOSX:
                    return GetShellOutput(UnixPlatforms.MacOSX, "pbpaste");
                default:
                    // Send help!
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Checks whether the system clipboard contains any text at all.
        /// </summary>
        /// <returns></returns>
        public static bool ContainsText()
        {
            return !string.IsNullOrWhiteSpace(GetText());
        }

        /// <summary>
        /// Checks whether or not the underlying operating system has the capability to copy/paste and the required libraries installed.
        /// </summary>
        /// <returns>Returns whether or not we can copy/paste data to the clipboard.</returns>
        public static bool CanCopyPaste()
        {
            var platform = GetPlatform();
            switch (platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.MacOSX:
                    return true;
                case PlatformID.Unix:
                    // Are we running a Wayland shell?
                    if (ShellUsesWayland())
                    {
                        return !GetShellOutput(UnixPlatforms.Linux, "wl-paste").Contains("command not found");
                    }
                    else
                    {
                        return !GetShellOutput(UnixPlatforms.Linux, "xclip -o").Contains("command not found");
                    }
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the platform we are currently running on.
        /// </summary>
        /// <returns>Returns the <see cref="PlatformID"/> of the platform we are running on.</returns>
        private static PlatformID GetPlatform()
        {
            var platform = Environment.OSVersion.Platform;
            // If Unix, are we on Mac or Linux?
            if (platform == PlatformID.Unix)
            {
                if (GetShellOutput(UnixPlatforms.MacOSX, "uname").Contains("Darwin"))
                {
                    platform = PlatformID.MacOSX;
                }
            }
            return platform;
        }

        #region Linux/Mac Support
        /// <summary>
        /// Checks whether or not the current Linux shell is configured to use the Wayland render server.
        /// </summary>
        /// <returns>Returns whether or not the current render server is Wayland.</returns>
        private static bool ShellUsesWayland()
        {
            return GetShellOutput(UnixPlatforms.Linux, "ps aux | grep wayland").Replace("grep wayland", "").Contains("wayland");
        }

        /// <summary>
        /// Run a shell command on the provided platform.
        /// </summary>
        /// <param name="platform">The Platform to run this shell command on.</param>
        /// <param name="command">The command to run.</param>
        /// <param name="waitForExit">Should this application wait for the command to exit?</param>
        private static void RunShell(UnixPlatforms platform, string command, bool waitForExit = false)
        {
            // Set up our process to execute.
            var process = GetShellProcess(platform, $"-c \"{command}\"", false);

            // Stat the process and begin reading our data and error!
            process.Start();
        }

        /// <summary>
        /// Run a shell command on the provided platform and retrieve the data returned by it.
        /// </summary>
        /// <param name="platform">The platform to run this shell command on.</param>
        /// <param name="command">The command to run.</param>
        /// <returns>Returns a string containing the output of the command.</returns>
        private static string GetShellOutput(UnixPlatforms platform, string command)
        {
            try
            {
                // Set up our process to execute.
                var process = GetShellProcess(platform, $"-c \"{command}\"", true);

                // Stat the process and begin reading our data and error!
                process.Start();
                process.WaitForExit(100);

                // Did we succeed?
                if (process.ExitCode == 0)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    return output;
                }
                else
                {
                    var error = process.StandardError.ReadToEnd();
                    return error;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        /// <summary>
        /// Create a new <see cref="Process"/> containing the shell command provided.
        /// </summary>
        /// <param name="platform">The platform to run this shell command on.</param>
        /// <param name="command">The command to run.</param>
        /// <param name="readable">Should the output of the command be readable?</param>
        /// <returns>Returns a process containing the shell command provided for execution.</returns>
        private static Process GetShellProcess(UnixPlatforms platform, string command, bool readable)
        {
            var execFile = string.Empty;
            switch (platform)
            {
                case UnixPlatforms.Linux:
                    execFile = "bash";
                    break;
                case UnixPlatforms.MacOSX:
                    execFile = "zsh";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new Process() {
                StartInfo = new ProcessStartInfo() {
                    FileName = execFile,
                    Arguments = command,
                    RedirectStandardOutput = readable,
                    RedirectStandardError = readable,
                    UseShellExecute = false
                }
            };
        }

        /// <summary>
        /// Defines the Unix-like platforms we're supporting here.
        /// </summary>
        enum UnixPlatforms
        {
            Linux,

            MacOSX
        }
        #endregion
    }
}