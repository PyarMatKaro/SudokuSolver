using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    [Serializable]
    public class HintFlags
    {
        public bool PaintPencilMarks { get; set; }
        public bool PaintHints { get; set; }
        public bool SelectForcedMoveHints { get; set; }
    }
}
