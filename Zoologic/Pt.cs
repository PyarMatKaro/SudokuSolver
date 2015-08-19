using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zoologic
{
    public class Pt
    {
        public int X, Y;

        public Pt(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pt))
                return false;
            Pt p = obj as Pt;
            return X == p.X && Y == p.Y;
        }

        public override int GetHashCode()
        {
            return X + 101 * Y;
        }

        public Pt[] Neighbours
        {
            get
            {
                return new Pt[]{
                    new Pt(X-1,Y),
                    new Pt(X,Y-1),
                    new Pt(X+1,Y),
                    new Pt(X,Y+1)
                };
            }
        }
    }

    public class PtKind
    {
        public Pt p;
        public K k;

        public PtKind(Pt p, K k)
        {
            this.p = p;
            this.k = k;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PtKind))
                return false;
            PtKind p2 = obj as PtKind;
            return p == p2.p && k == p2.k;
        }

        public override int GetHashCode()
        {
            return p.GetHashCode() + 10001 * (int)k;
        }
    }
}
