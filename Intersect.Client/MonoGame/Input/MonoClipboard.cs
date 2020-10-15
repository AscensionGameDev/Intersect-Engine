using Intersect.Client.Framework.Input;
using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Intersect.Client.MonoGame.Input
{
    public class MonoClipboard : GameClipboard
    {

        private PlatformID? mPlatform = null;

        /// <inheritdoc />
        public override void SetText(string data)
        {
            var platform = GetPlatform();
            data = data.Trim();
            switch (platform)
            {
                case PlatformID.Win32NT:
                    Clipboard.SetText(data);
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

        /// <inheritdoc />
        public override string GetText()
        {
            var platform = GetPlatform();
            switch (platform)
            {
                case PlatformID.Win32NT:
                    return Clipboard.GetText().Trim();
                case PlatformID.Unix:
                    // Are we running a Wayland shell?
                    if (ShellUsesWayland())
                    {
                        return GetShellOutput(UnixPlatforms.Linux, "wl-paste").Trim();
                    }
                    else
                    {
                        return GetShellOutput(UnixPlatforms.Linux, "xclip -o").Trim();
                    }
                case PlatformID.MacOSX:
                    return GetShellOutput(UnixPlatforms.MacOSX, "pbpaste").Trim();
                default:
                    // Send help!
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public override bool ContainsText()
        {
            return !string.IsNullOrWhiteSpace(GetText());
        }

        /// <inheritdoc />
        public override bool CanCopyPaste()
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
        private PlatformID GetPlatform()
        {
            if (mPlatform == null)
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
                mPlatform = platform;
            }

            return (PlatformID)mPlatform;
        }

        #region Linux/Mac Support
        /// <summary>
        /// Checks whether or not the current Linux shell is configured to use the Wayland render server.
        /// </summary>
        /// <returns>Returns whether or not the current render server is Wayland.</returns>
        private bool ShellUsesWayland()
        {
            return GetShellOutput(UnixPlatforms.Linux, "ps aux | grep wayland").Replace("grep wayland", "").Contains("wayland");
        }

        /// <summary>
        /// Run a shell command on the provided platform.
        /// </summary>
        /// <param name="platform">The Platform to run this shell command on.</param>
        /// <param name="command">The command to run.</param>
        /// <param name="waitForExit">Should this application wait for the command to exit?</param>
        private void RunShell(UnixPlatforms platform, string command, bool waitForExit = false)
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
        private string GetShellOutput(UnixPlatforms platform, string command)
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
        private Process GetShellProcess(UnixPlatforms platform, string command, bool readable)
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

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
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
