using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public interface HintSupport
    {
        HintFlags HintFlags { get; }
        void UpdateHints();
    }
}
