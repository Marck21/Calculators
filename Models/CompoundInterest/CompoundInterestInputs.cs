namespace Calculators.Models.CompoundInterest;

public class CompoundInterestInputs
{
    public decimal InitialBalance { get; set; } = 100000;
    public double AnnualInterestRate { get; set; } = 10.0;
    public int DurationInYears { get; set; } = 32;
    public double AnnualInflationRate { get; set; } = 2.5;
    public List<ContributionPeriod> ContributionPeriods { get; set; } = new List<ContributionPeriod>
        {
            new() { StartYear = 1, EndYear = 5, Amount = 1500 }
        };
}