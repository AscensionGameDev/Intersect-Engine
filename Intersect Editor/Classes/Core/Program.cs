using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Intersect.Editor.Forms;
using Intersect.Logging;

namespace Intersect.Editor.Classes
{
    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Diagnostic("Starting editor...");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Diagnostic("Unpacking libraries...");

            //Place sqlite3.dll where it's needed.
            var dllname = Environment.Is64BitProcess ? "sqlite3x64.dll" : "sqlite3x86.dll";
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Intersect.Editor.Resources." + dllname))
            {
                using (var fs = new FileStream("sqlite3.dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);
                    fs.Write(data, 0, data.Length);
                }
            }

            Log.Diagnostic("Libraries unpacked.");

            Log.Diagnostic("Creating forms...");
            Globals.LoginForm = new FrmLogin();
            Globals.MainForm = new frmMain();
            Log.Diagnostic("Forms created.");

            Log.Diagnostic("Starting application.");
            Application.Run(Globals.LoginForm);
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception) e.ExceptionObject);
            MessageBox.Show(
                "The Intersect Editor has encountered an error and must close. Error information can be found in resources/logs/errors.log");
            Application.Exit();
        }
    }
}