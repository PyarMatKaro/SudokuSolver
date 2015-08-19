using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    [Serializable]
    public class HintSelections
    {
        public enum Level { None = 0, Easy = 1, Medium = 2, Hard = 3, Diabolical = 4 };

        public bool ForcedMoves { get; set; }
        public bool ImmediateDiscardables { get; set; }
        public bool EventualDiscardables { get; set; }
        public bool Selectables { get; set; }
        public bool EventualSolutions { get; set; }

        public HintSelections()
        {
        }

        public HintSelections(Level level)
        {
            SetLevel(level);
        }

        public void SetLevel(Level level)
        {
            ForcedMoves = (level > Level.None);
            ImmediateDiscardables = (level > Level.Easy);
            EventualDiscardables = (level > Level.Medium);
            Selectables = (level > Level.Hard);
            EventualSolutions = false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HintSelections))
                return false;
            HintSelections hs = obj as HintSelections;
            return ForcedMoves == hs.ForcedMoves &&
                ImmediateDiscardables == hs.ImmediateDiscardables &&
                EventualDiscardables == hs.EventualDiscardables &&
                Selectables == hs.Selectables &&
                EventualSolutions == hs.EventualSolutions;
        }

        public bool IsLevel(Level level)
        {
            HintSelections hs = new HintSelections(level);
            return hs.Equals(this);
        }
    }
}
