using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoitxTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmMain myForm1 = new frmMain();
            Application.Run();      //Specify myForm1 in the Run() to show the form on app start
        }
    }
}
