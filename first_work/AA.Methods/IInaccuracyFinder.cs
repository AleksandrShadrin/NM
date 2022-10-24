using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public interface IInaccuracyFinder
    {
        Point GetInaccuracyOfMethodRunge(int n);
        Point GetInaccuracyOfExplicitAdamsMethod(int n);
        Point GetInaccuracyOfImplicitAdamsMethod(int n);
        double FindPForMethodRunge();
        double FindPForExplicitAdamsMethod();
        double FindPForImplicitAdamsMethod();
        void SetExactSolution(Func<double, double> equation);
    }
}
