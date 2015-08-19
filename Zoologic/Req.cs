using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Zoologic
{
    public class ReqP : Requirement
    {
        public Pt p;

        public ReqP(Pt p)
        {
            this.p = p;
        }

        public override string RequirementString()
        {
            return "place at " + p;
        }
    }

    public class ReqK : Requirement
    {
        public K k;

        public ReqK(K k, int n) : base(n)
        {
            this.k = k;
        }

        public override string RequirementString()
        {
            return "place " + k;
        }
    }

    public class ReqPK : Requirement
    {
        public Pt p;
        public K k;

        public ReqPK(Pt p, K k)
        {
            this.k = k;
        }

        public override string RequirementString()
        {
            return "place " + k + " at " + p;
        }
    }
}
