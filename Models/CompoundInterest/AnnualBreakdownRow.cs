namespace Calculators.Models.CompoundInterest;

public class AnnualBreakdownRow
{
    public int Year { get; set; }
    public decimal AnnualContribution { get; set; }
    public decimal TotalContributions { get; set; }
    public decimal AnnualInterest { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal FinalBalance { get; set; }
    public decimal InflationAdjustedBalance { get; set; }
}