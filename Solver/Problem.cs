using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public abstract class Problem
    {
        public abstract void SelectCandidate(Candidate t);
        public abstract void DiscardCandidate(Candidate t);
    }
}
