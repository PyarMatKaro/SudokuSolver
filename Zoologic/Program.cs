using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Zoologic
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Test();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void Test()
        {
            Grid g = new Grid();
        }
    }
}
