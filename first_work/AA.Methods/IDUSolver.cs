using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public interface IDUSolver
    {
        void SetEquation(Func<double, double, double> equation);
        void SetDerivative(Func<double, double, double> deriviative);
        Dictionary<int, double> FindTowUsingMethodRunge();
        IEnumerable<Point> MethodRunge();
        Dictionary<int, double> FindTowUsingExplicitAdamsMethod();
        IEnumerable<Point> ExplicitAdamsMethod();
        Dictionary<int, double> FindTowUsingImplicitAdamsMethod();
        IEnumerable<Point> ImplicitAdamsMethod();
        void SetN(int value);
        void ResetTow();
        void ConfigureOptions(Action<DUSolverOptions> configure);
    }
}
