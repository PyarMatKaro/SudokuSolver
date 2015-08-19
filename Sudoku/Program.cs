using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Sudoku
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

            /*
            ApplicationContext ac = new ApplicationContext();
            SudokuGrid grid = new SudokuGrid(false);

            Form f1 = new GridForm(grid);
            Form f2 = new ControlForm(grid);
            Rectangle r = Screen.PrimaryScreen.WorkingArea;
            f1.Location = new Point((r.Width - f1.Width - f2.Width) / 2, (r.Height - f1.Height) / 2);
            f2.Location = new Point((r.Width + f1.Width - f2.Width) / 2, (r.Height - f2.Height) / 2);
            f1.Show();
            f2.Show();

            f2.AddOwnedForm(f1);

            Application.Run(ac);
             */

            SudokuForm form = new SudokuForm(new SudokuGrid(false));
            Application.Run(form);
        }
    }
}
