namespace Calculators.Helpers;

public class BuyOrRentHelper
{
    public static class FinancialMath
    {
        public static decimal CalculateMonthlyMortgagePayment( decimal p, decimal r, int y ) { if ( p <= 0 || y <= 0 ) return 0; if ( r <= 0 ) return p / (y * 12); var mR = r / 12; int nP = y * 12; var mRD = (double)mR; var pD = (double)p; return (decimal)(pD * mRD * Math.Pow( 1 + mRD, nP ) / (Math.Pow( 1 + mRD, nP ) - 1)); }
        public static decimal CalculateSimulatedRentExpense( decimal i, decimal g, int y ) { if ( i <= 0 ) return 0; decimal t = 0, c = i; for ( int m = 1; m <= y * 12; m++ ) { t += c; if ( m % 12 == 0 && m < y * 12 ) c *= (1 + g); } return t; }
        public static decimal CalculatePrincipalPaid( decimal p, decimal r, int lY, int cY ) { if ( p <= 0 ) return p; if ( cY == 0 ) return 0; var mP = CalculateMonthlyMortgagePayment( p, r, lY ); if ( mP <= 0 ) return 0; decimal remB = p, mR = r / 12; int tM = Math.Min( cY * 12, lY * 12 ); for ( int i = 0; i < tM; i++ ) { decimal intr = remB * mR; remB -= (mP - intr); } return p - Math.Max( 0, remB ); }

        public static decimal CalculateProgressiveCapitalGainsTax( decimal capitalGain )
        {
            if ( capitalGain <= 0 ) return 0;

            decimal tax = 0;
            decimal remainingGain = capitalGain;

            decimal bracket1Amount = Math.Min( remainingGain, 6000m );
            tax += bracket1Amount * 0.19m;
            remainingGain -= bracket1Amount;

            if ( remainingGain > 0 )
            {
                decimal bracket2Amount = Math.Min( remainingGain, 44000m );
                tax += bracket2Amount * 0.21m;
                remainingGain -= bracket2Amount;
            }

            if ( remainingGain > 0 )
            {
                decimal bracket3Amount = Math.Min( remainingGain, 150000m );
                tax += bracket3Amount * 0.23m;
                remainingGain -= bracket3Amount;
            }

            if ( remainingGain > 0 )
            {
                decimal bracket4Amount = Math.Min( remainingGain, 100000m );
                tax += bracket4Amount * 0.27m;
                remainingGain -= bracket4Amount;
            }

            if ( remainingGain > 0 )
            {
                tax += remainingGain * 0.28m;
            }

            return tax;
        }
    }
}
