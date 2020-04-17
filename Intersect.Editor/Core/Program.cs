using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Intersect.Editor.Forms;
using Intersect.Editor.General;
using Intersect.Logging;

// TODO: Move or change the namespace?
// ReSharper disable once CheckNamespace
namespace Intersect.Editor
{

    public static class Program
    {

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Log.Diagnostic("Starting editor...");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Diagnostic("Unpacking libraries...");

            //Place sqlite3.dll where it's needed.
            var dllname = Environment.Is64BitProcess ? "sqlite3x64.dll" : "sqlite3x86.dll";
            using (var resourceStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Intersect.Editor.Resources." + dllname))
            {
                Debug.Assert(resourceStream != null, "resourceStream != null");
                using (var fileStream = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var data = new byte[resourceStream.Length];
                    resourceStream.Read(data, 0, (int) resourceStream.Length);
                    fileStream.Write(data, 0, data.Length);
                }
            }

            Log.Diagnostic("Libraries unpacked.");

            Log.Diagnostic("Creating forms...");
            Globals.UpdateForm = new FrmUpdate();
            Globals.LoginForm = new FrmLogin();
            Globals.MainForm = new FrmMain();
            Log.Diagnostic("Forms created.");

            Log.Diagnostic("Starting application.");
            Application.Run(Globals.UpdateForm);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(e.Exception, true));
        }

        //Really basic error handler for debugging purposes
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            Log.Error((Exception) exception?.ExceptionObject);
            MessageBox.Show(
                @"The Intersect Editor has encountered an error and must close. Error information can be found in logs/errors.log"
            );

            Environment.Exit(1);
        }

    }

}
