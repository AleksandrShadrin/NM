using AA.Methods.ValueObjects;
using DS.SeriesAnalysis.EquationSolvers;

namespace AA.Methods
{
    public class InaccuracyFinder : IInaccuracyFinder
    {
        private Func<double, double> exactEquation;
        private readonly IDUSolver solver;
        private readonly PolynomialSolver equationSolver = new(degree: 1);
        private readonly DUSolverOptions duOptions;

        public InaccuracyFinder(IDUSolver solver, DUSolverOptions duOptions)
        {
            this.solver = solver;
            this.duOptions = duOptions;
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
        public double FindPForMethodRunge()
            => GetPForMethod(GetInaccuracyOfMethodRunge);

        public double FindPForExplicitAdamsMethod()
            => GetPForMethod(GetInaccuracyOfExplicitAdamsMethod);

        public double FindPForImplicitAdamsMethod()
            => GetPForMethod(GetInaccuracyOfImplicitAdamsMethod);

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

        private double GetPForMethod(Func<int, Point> inaccuracyMethod)
        {
            var points =
                Enumerable.Range(2, 14)
                .Select(v => Math.Pow(2, v))
                .Select(v => inaccuracyMethod(Convert.ToInt32(v)))
                .Select(p => p with { X = Math.Log((duOptions.T - duOptions.T0) / p.X), Y = Math.Log(p.Y) });

            var serie = new Serie(
                x: points.Select(p => p.X),
                y: points.Select(p => p.Y));

            equationSolver.FindCoeffs(serie);
            return equationSolver.GetCoeffs().Last();
        }
    }
}
