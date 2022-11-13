using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public class DUSolver : IDUSolver
    {
        private Func<double, double, double> func;
        private Func<double, double, double> derivative;
        private double tow;
        private readonly DUSolverOptions _options;

        public DUSolver(DUSolverOptions options)
        {
            _options = options;
            tow = (options.T - options.T0) / options.N;
        }
        #region MethodRunge
        public Dictionary<int, double> FindTowUsingMethodRunge()
        {
            var n = (int)_options.N;
            ResetTow();
            var result = new Dictionary<int, double>() { { n, tow } };

            var points = MethodRunge();
            var lastPoint = points.Last();
            tow /= 2;
            n *= 2;

            while (true)
            {
                points = MethodRunge();

                if (SatisfiedCondition(lastPoint.Y, points.Last().Y))
                {
                    result.Add(n, tow);
                    break;
                }
                result.Add(n, tow);
                lastPoint = points.Last();
                tow /= 2;
                n *= 2;
            }
            return result;
        }

        public IEnumerable<Point> MethodRunge()
        {
            _options.P = 1;
            var tn = _options.T0;
            var yn = _options.U0;

            List<Point> results = new()
            {
                new Point(tn, yn)
            };
            while (Math.Abs(tn - _options.T) >= tow / 2)
            {
                yn = MethodRunge(yn, tn);
                tn += tow;
                results.Add(new Point(tn, yn));
            }

            return results;
        }

        private double MethodRunge(double yn, double tn)
        {
            return yn + tow * func(tn, yn);
        }
        #endregion

        #region ExplicitAdamsMethod
        public IEnumerable<Point> ExplicitAdamsMethod()
        {
            _options.P = 4;
            List<double> yns = new() { _options.U0 };
            List<double> tns = new() { _options.T0 };

            List<Point> results = new() { new Point(_options.T0, _options.U0) };

            yns.Add(ExplicitAdamsMethod1(yns[0], tns[0]));
            tns.Add(tns.Last() + tow);
            results.Add(new Point(tns.Last(), yns.Last()));

            yns.Add(ExplicitAdamsMethod2(yns[1], yns[0], tns[1], tns[0]));
            tns.Add(tns.Last() + tow);
            results.Add(new Point(tns.Last(), yns.Last()));

            yns.Add(ExplicitAdamsMethod3(yns[2], yns[1], yns[0], tns[2], tns[1], tns[0]));
            tns.Add(tns.Last() + tow);
            results.Add(new Point(tns.Last(), yns.Last()));

            while (Math.Abs(tns.Last() - _options.T) >= tow / 2)
            {
                yns.Add(
                    ExplicitAdamsMethod4(yns[3], yns[2], yns[1], yns[0],
                    tns[3], tns[2], tns[1], tns[0]));
                tns.Add(tns.Last() + tow);
                results.Add(new Point(tns.Last(), yns.Last()));

                yns.RemoveAt(0);
                tns.RemoveAt(0);
            }

            return results;
        }

        public Dictionary<int, double> FindTowUsingExplicitAdamsMethod()
        {
            var n = (int)_options.N;
            ResetTow();
            var result = new Dictionary<int, double>() { { n, tow } };

            var points = ExplicitAdamsMethod();
            var lastPoint = points.Last();
            tow /= 2;
            n *= 2;

            while (true)
            {
                points = ExplicitAdamsMethod();
                if (SatisfiedCondition(lastPoint.Y, points.Last().Y))
                {
                    result.Add(n, tow);
                    break;
                }
                result.Add(n, tow);
                lastPoint = points.Last();
                tow /= 2;
                n *= 2;
            }
            return result;
        }

        private double ExplicitAdamsMethod1(double yn, double tn)
        {
            return yn + tow * func(tn, yn);
        }
        private double ExplicitAdamsMethod2(double yn, double yn1, double tn, double tn1)
        {
            return yn + tow / 2 * (3 * func(tn, yn) - func(tn1, yn1));
        }
        private double ExplicitAdamsMethod3(double yn, double yn1, double yn2, double tn, double tn1, double tn2)
        {
            return yn + tow / 12 * (23 * func(tn, yn) - 16 * func(tn1, yn1) + 5 * func(tn2, yn2));
        }
        private double ExplicitAdamsMethod4(double yn, double yn1, double yn2, double yn3, double tn, double tn1, double tn2, double tn3)
        {
            return yn + tow / 24 * (55 * func(tn, yn) - 59 * func(tn1, yn1) + 37 * func(tn2, yn2) - 9 * func(tn3, yn3));
        }
        #endregion

        #region ImplicitAdamsMethod
        public void SetDerivative(Func<double, double, double> derivative)
        {
            this.derivative = derivative;
        }

        public IEnumerable<Point> ImplicitAdamsMethod()
        {
            _options.P = 2;
            var tn = _options.T0;
            var yn = _options.U0;

            List<Point> results = new()
            {
                new Point(tn, yn)
            };

            while (Math.Abs(tn - _options.T) >= tow / 2)
            {
                yn = ImplicitAdamsMethod0(yn, tn);
                tn += tow;
                results.Add(new Point(tn, yn));
            }

            return results;
        }

        public Dictionary<int, double> FindTowUsingImplicitAdamsMethod()
        {
            var n = (int)_options.N;
            ResetTow();
            var result = new Dictionary<int, double>() { { n, tow } };

            var points = ImplicitAdamsMethod();
            var lastPoint = points.Last();
            tow /= 2;
            n *= 2;

            while (true)
            {
                points = ImplicitAdamsMethod();
                if (SatisfiedCondition(lastPoint.Y, points.Last().Y))
                {
                    result.Add(n, tow);
                    break;
                }
                result.Add(n, tow);
                lastPoint = points.Last();
                tow /= 2;
                n *= 2;
            }
            return result;
        }

        private double ImplicitAdamsMethod0(double yn, double tn)
        {
            var y_curr = yn;
            var y_next = y_curr - (y_curr - yn - tow / 2 * (func(tn + tow, y_curr) + func(tn, yn))) /
                    (1 - tow / 2 * derivative(tn + tow, y_curr));

            if (derivative is null)
            {
                throw new ArgumentNullException(nameof(derivative));
            }

            while (Math.Abs(y_curr - y_next) > 10e-3)
            {
                y_curr = y_next;
                y_next = y_curr - (y_curr - yn - tow / 2 * (func(tn + tow, y_curr) + func(tn, yn))) /
                    (1 - tow / 2 * derivative(tn + tow, y_curr));
            }

            return y_next;
        }
        #endregion

        #region AdditionalFeatures
        private bool SatisfiedCondition(double prevValue, double currentValue)
         => Math.Abs((prevValue - currentValue) / currentValue)
            <= _options.Eps * (Math.Pow(2, _options.P + 1) - 2) / 2;

        public void ResetTow()
            => tow = (_options.T - _options.T0) / _options.N;

        public void ConfigureOptions(Action<DUSolverOptions> configure)
        {
            configure(this._options);
        }

        public void SetEquation(Func<double, double, double> equation)
        {
            func += equation;
        }

        public void SetN(int value)
        {
            tow = (_options.T - _options.T0) / value;
        }
        #endregion
    }
}
