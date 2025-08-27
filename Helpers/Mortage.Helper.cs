namespace Calculators.Helpers;

public class MortageHelper
{
    public static class FinancialMath
    {
        public static decimal CalculateMonthlyMortgagePayment( decimal p, decimal r, int y ) { if ( p <= 0 || y <= 0 ) return 0; if ( r <= 0 ) return p / (y * 12); var mR = r / 12; int nP = y * 12; var mRD = (double)mR; var pD = (double)p; return (decimal)(pD * mRD * Math.Pow( 1 + mRD, nP ) / (Math.Pow( 1 + mRD, nP ) - 1)); }
    }
}
