/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
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
