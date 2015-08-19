using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    [Serializable]
    public class HintOptions
    {
        public HintFlags Flags;
        public HintSelections Show, AutoSolve;

        public HintOptions()
        {
            Flags = new HintFlags();
            Show = new HintSelections(HintSelections.Level.Hard);
            AutoSolve = new HintSelections(HintSelections.Level.Easy);
        }
    }
}
