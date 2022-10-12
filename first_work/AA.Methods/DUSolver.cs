using AA.Methods.ValueObjects;

namespace AA.Methods
{
    public class DUSolver : IDUSolver
    {
        private Func<double, double, double> func;
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
                    tow *= 2;
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

            while (Math.Abs(tns.Last() - _options.T) > tow / 2)
            {
                yns.Add(
                    ExplicitAdamsMethod4(yns[3], yns[2], yns[1], yns[0],
                    tns[3], tns[2], tns[1], tns[0]));
                tns.Add(tns.Last() + tow);
                results.Add(new Point(tns.Last(), yns.Last()));

                yns.Remove(yns.First());
                tns.Remove(tns.First());
            }

            return results;
        }

        public Dictionary<int, double> FindTowUsingExplicitAdamsMethod()
        {
            var n = (int)_options.N;
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
                    tow *= 2;
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
