
using System;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Forms;
using Intersect_Library.Logging;

namespace Intersect_Editor.Classes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Globals.LoginForm = new FrmLogin();
            Globals.MainForm = new frmMain();
            Application.Run(Globals.LoginForm);
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception)e.ExceptionObject);
            MessageBox.Show("The Intersect Editor has encountered an error and must close. Error information can be found in resources/logs/errors.log");
            Application.Exit();
        }
    }
}
