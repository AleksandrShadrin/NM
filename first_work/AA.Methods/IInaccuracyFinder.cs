﻿using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public interface IInaccuracyFinder
    {
        Point GetInaccuracyOfMethodRunge(int n);
        Point GetInaccuracyOfExplicitAdamsMethod(int n);
        void SetExactSolution(Func<double, double> equation);
    }
}