using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public interface IDUSolver
    {
        void SetEquation(Func<double, double, double> equation);
        Dictionary<int, double> FindTowUsingMethodRunge();
        IEnumerable<Point> MethodRunge();
        Dictionary<int, double> FindTowUsingExplicitAdamsMethod();
        IEnumerable<Point> ExplicitAdamsMethod();
        void ResetTow();
        void ConfigureOptions(Action<DUSolverOptions> configure);
    }
}
