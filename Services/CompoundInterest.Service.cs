using Calculators.Models.CompoundInterest;
using static Calculators.Helpers.CompoundInterestHelper;

namespace Calculators.Services;

public interface ICompoundInterestService
{
    CompoundInterestCalculations CalculateCompoundInterest( CompoundInterestInputs inputs );
    List<AnnualBreakdownRow> GetAnnualBreakdown( CompoundInterestInputs inputs );
}

public class CompoundInterestService : ICompoundInterestService
{
    public CompoundInterestCalculations CalculateCompoundInterest( CompoundInterestInputs inputs )
    {
        var calculations = new CompoundInterestCalculations();

        var sortedPeriods = inputs.ContributionPeriods.OrderBy( p => p.StartYear ).ToList();

        for ( int i = 1; i < sortedPeriods.Count; i++ )
        {
            if ( sortedPeriods[i].StartYear < sortedPeriods[i - 1].EndYear )
            {
                calculations.ValidationMessage = $"Error: El periodo que empieza en el año {sortedPeriods[i].StartYear} se solapa con el anterior que termina en el año {sortedPeriods[i - 1].EndYear}.";
                return calculations;
            }
        }

        var annualBreakdown = GetAnnualBreakdown( inputs );

        var finalRow = annualBreakdown.LastOrDefault() ?? new AnnualBreakdownRow();
        calculations.FinalBalance = finalRow.FinalBalance;
        calculations.TotalContributions = finalRow.TotalContributions;
        calculations.TotalInterest = finalRow.TotalInterest;
        calculations.InflationAdjustedFinalBalance = finalRow.InflationAdjustedBalance;

        return calculations;
    }

    public List<AnnualBreakdownRow> GetAnnualBreakdown( CompoundInterestInputs inputs )
    {
        var annualBreakdown = new List<AnnualBreakdownRow>();
        decimal currentBalance = inputs.InitialBalance;
        decimal totalContributions = 0;
        decimal annualInterestRate = (decimal)inputs.AnnualInterestRate / 100;
        decimal inflationRate = (decimal)inputs.AnnualInflationRate / 100;

        var sortedPeriods = inputs.ContributionPeriods.OrderBy( p => p.StartYear ).ToList();

        for ( int year = 1; year <= inputs.DurationInYears; year++ )
        {
            decimal balanceAtStartOfYear = currentBalance;
            var period = sortedPeriods.LastOrDefault( p => year >= p.StartYear && year < p.EndYear );
            if ( period == null )
            {
                period = sortedPeriods.FirstOrDefault( p => year >= p.StartYear && year <= p.EndYear );
            }

            decimal monthlyContribution = period?.Amount ?? 0;

            currentBalance = FinancialMath.CalculateFutureValue( balanceAtStartOfYear, monthlyContribution, 12, annualInterestRate );

            decimal annualContribution = monthlyContribution * 12;
            decimal interestThisYear = currentBalance - balanceAtStartOfYear - annualContribution;

            totalContributions += annualContribution;
            decimal totalInterest = currentBalance - inputs.InitialBalance - totalContributions;
            decimal inflationAdjustedBalance = currentBalance / (decimal)Math.Pow( 1 + (double)inflationRate, year );

            annualBreakdown.Add( new AnnualBreakdownRow
            {
                Year = year,
                AnnualContribution = annualContribution,
                TotalContributions = totalContributions,
                AnnualInterest = interestThisYear,
                TotalInterest = totalInterest,
                FinalBalance = currentBalance,
                InflationAdjustedBalance = inflationAdjustedBalance
            } );
        }
        return annualBreakdown;
    }
}