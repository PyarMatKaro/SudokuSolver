using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public class Tile
    {
        internal Candidate k;
        internal Tile l, r, u, d;
        internal Requirement c;

        internal bool included = false;

        public Candidate Candidate { get { return k; } }

        void CheckIncluded()
        {
            System.Diagnostics.Debug.Assert(l.r == this);
            System.Diagnostics.Debug.Assert(r.l == this);
            System.Diagnostics.Debug.Assert(u.d == this);
            System.Diagnostics.Debug.Assert(d.u == this);
        }

        public bool Included
        {
            get
            {
                if (included)
                    CheckIncluded();
                return included;
            }
        }

        public void SetIncluded()
        {
            included = true;
        }

        internal void ExcludeH()
        {
            r.l = l;
            l.r = r;
            included = false;
        }

        internal void IncludeH()
        {
            r.l = this;
            l.r = this;
            included = true;
            CheckIncluded();
        }

        internal void ExcludeV()
        {
            d.u = u;
            u.d = d;
            c.s--;
            included = false;
        }

        internal void IncludeV()
        {
            included = true;
            c.s++;
            d.u = this;
            u.d = this;
            CheckIncluded();
        }

        public Tile()
        {
            ClearTile();
        }

        /*
        public static Tile AddToCandidate<C>(Tile x, Requirement c)
        {
            Tile t = new Tile();
            t.ClearTile();
            if (x != null)
            {
                x.r.l = t;
                t.r = x.r;
                x.r = t;
                t.l = x;
            }
            t.c = c;
            c.s++;
            t.d = c.d;
            c.d.u = t;
            c.d = t;
            t.u = c;
            t.included = true;
            return t;
        }
        */

        protected void ClearTile()
        {
            l = r = u = d = this;
            c = null;
            included = false;
        }

        internal int CandidateOptions
        {
            get
            {
                int o = c.s;
                Tile j = this;
                do
                {
                    if (j.c.s < o)
                        o = j.c.s;
                    j = j.r;
                }
                while (j != this);
                return o;
            }
        }

        public string CandidateString()
        {
            return c.CandidateString(this);
        }
    }
}
