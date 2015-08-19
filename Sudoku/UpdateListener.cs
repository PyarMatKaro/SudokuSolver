using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public interface UpdateListener
    {
        void OnChangeGrid();
    }
}
