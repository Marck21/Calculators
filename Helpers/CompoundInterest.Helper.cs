namespace Calculators.Helpers;

public class CompoundInterestHelper
{
    public static class FinancialMath
    {
        public static decimal CalculateFutureValue( decimal principal, decimal monthlyPayment, int numberOfMonths, decimal annualRate )
        {
            if ( annualRate == 0 ) return principal + (monthlyPayment * numberOfMonths);
            decimal r = annualRate / 12;
            int n = numberOfMonths;
            double r_d = (double)r;
            return principal * (decimal)Math.Pow( 1 + r_d, n ) + monthlyPayment * (decimal)((Math.Pow( 1 + r_d, n ) - 1) / r_d);
        }
    }
}
