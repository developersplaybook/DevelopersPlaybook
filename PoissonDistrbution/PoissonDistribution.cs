namespace PoissonDistribution;

public class PoissonDistribution
{
    private readonly double _lambda;
    private readonly double _logLambda;
    private readonly double _expMinusLambda;
    private readonly double _logPiDivTwo;

    public PoissonDistribution(double lambda)
    {
        _lambda = lambda;
        _logLambda = Math.Log(lambda);
        _expMinusLambda = Math.Exp(-lambda);
        _logPiDivTwo = Math.Log(Math.PI) / 2;
    }

    public double ProbabilityMassFunction(long k)
    {
        if (k > 170 || double.IsInfinity(Math.Pow(_lambda, k)))
        {
            double ramanujansApprox = k * _logLambda - _lambda - (k * Math.Log(k) - k + Log6ThTail(k) + _logPiDivTwo);
            return Math.Exp(ramanujansApprox);
        }

        return _expMinusLambda * Math.Pow(_lambda, k) / Factorial(k);
    }

    public double CumulativeDistributionFunction(long k)
    {
        double sum = _expMinusLambda;
        double term = sum;
        bool useRamanujanApproximation = (_expMinusLambda == 0.0);

        if (useRamanujanApproximation)
        {
            for (long i = 1; i <= k; i++)
            {
                double ramanujansApprox = i * _logLambda - (i * Math.Log(i) - i + Log6ThTail(i) + _logPiDivTwo);
                term = Math.Exp(ramanujansApprox - _lambda);
                sum += term;
            }

            return (sum > 1) ? 1.0 : sum;
        }
        else
        {
            for (long i = 1; i <= k; i++)
            {
                if (useRamanujanApproximation)
                {
                    double ramanujansApprox = i * _logLambda - (i * Math.Log(i) - i + Log6ThTail(i) + _logPiDivTwo);
                    term = Math.Exp(ramanujansApprox - _lambda);
                }
                else
                {
                    if (i > 170 || double.IsInfinity(Math.Pow(_lambda, i)))
                    {
                        useRamanujanApproximation = true;
                        double ramanujansApprox = i * _logLambda - (i * Math.Log(i) - i + Log6ThTail(i) + _logPiDivTwo);
                        term = Math.Exp(ramanujansApprox - _lambda);
                    }
                    else
                    {
                        term = term * _lambda / i;
                    }
                }

                sum += term;
            }
            
            return (sum > 1) ? 1.0 : sum;
        }
    }

    private double Log6ThTail(long i)
    {
        return Math.Log(i * (1 + 4 * i * (1 + 2 * i))) / 6;
    }

    private double Factorial(double k)
    {
        double count = Convert.ToInt64(k);
        double factorial = 1;
        while (count >= 1)
        {
            factorial = Math.Round(factorial * count);
            count--;
        }

        return factorial;
    }
}
