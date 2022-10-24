using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public class InaccuracyFinder : IInaccuracyFinder
    {
        private Func<double, double> exactEquation;
        private readonly IDUSolver solver;

        public InaccuracyFinder(IDUSolver solver)
        {
            this.solver = solver;
        }

        public Point GetInaccuracyOfExplicitAdamsMethod(int n)
            => GetInaccuracyUsingMethod(n, solver.ExplicitAdamsMethod);

        public Point GetInaccuracyOfMethodRunge(int n)
            => GetInaccuracyUsingMethod(n, solver.MethodRunge);

        public Point GetInaccuracyOfImplicitAdamsMethod(int n)
            => GetInaccuracyUsingMethod(n, solver.ImplicitAdamsMethod);

        public void SetExactSolution(Func<double, double> equation)
        {
            exactEquation = equation;
        }

        private Point GetInaccuracyUsingMethod(int n, Func<IEnumerable<Point>> method)
        {
            if (n > 10e8)
                return new(0, 0);

            solver.SetN(n);

            var pointsByMethod = method().ToList();

            solver.ResetTow();

            var accuracy = pointsByMethod.Last();

            return accuracy with
            {
                X = n,
                Y = Math.Abs((accuracy.Y - exactEquation(accuracy.X)) / exactEquation(accuracy.X))
            };
        }
    }
}
