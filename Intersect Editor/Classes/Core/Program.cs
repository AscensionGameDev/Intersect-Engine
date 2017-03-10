
using System;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Forms;

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
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (StreamWriter writer = new StreamWriter("resources/errors.log", true))
            {
                writer.WriteLine("Message :" + ((Exception)e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception)e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            MessageBox.Show(
                "The Intersect editor has encountered an error and must close. Error information can be found in resources/errors.log");
            Application.Exit();
        }
    }
}
