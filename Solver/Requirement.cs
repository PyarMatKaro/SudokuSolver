using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Solver
{
    public abstract class Requirement : Tile
    {
        public int s, n;

        public Requirement()
            : base()
        {
        }

        public Requirement(int n)
            : base()
        {
            this.n = n;
        }

        public void ClearRequirement(int n)
        {
            ClearTile();
            s = 0;
            this.n = n;
        }

        public Requirement Right
        {
            get
            {
                return (Requirement)r;
            }
        }

        public void AddRequirement(Requirement c)
        {
            r.l = c;
            c.r = r;
            r = c;
            c.l = this;
            included = true;
        }

        public void Cover()
        {
            ExcludeH();
            for (Tile i = d; i != this; i = i.d)
                for (Tile j = i.r; j != i; j = j.r)
                    j.ExcludeV();
        }

        public void Uncover()
        {
            for (Tile i = u; i != this; i = i.u)
                for (Tile j = i.l; j != i; j = j.l)
                    j.IncludeV();
            IncludeH();
        }

        public abstract string RequirementString();

        public string RequirementString(int s)
        {
            string ret;
            if (s == 0)
                ret = "no way to ";
            else if (s == 1)
                ret = "only one way to ";
            else
                ret = s + " ways to ";

            return ret + RequirementString();
        }

        public override string ToString()
        {
            return RequirementString(s);
        }

        public Candidate[] UnselectedCandidates
        {
            get
            {
                Candidate[] ret = new Candidate[s];
                int j = 0;
                for (Tile i = u; i != this; i = i.u)
                    ret[j++] = i.Candidate;
                return ret;
            }
        }

        public virtual void PaintForeground(HintPainter context, Hint.Kind v) { }
        public virtual void PaintBackground(HintPainter context, Hint.Kind v) { }

        public virtual string CandidateString(Tile t)
        {
            return t.ToString();
        }
    }

    public class HeadRequirement : Requirement
    {
        public override string RequirementString()
        {
            return "Header";
        }
    }
}
