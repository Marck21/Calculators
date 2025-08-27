namespace Calculators.Models.CompoundInterest;

public class CompoundInterestCalculations
{
    public decimal FinalBalance { get; set; }
    public decimal TotalContributions { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal InflationAdjustedFinalBalance { get; set; }
    public string? ValidationMessage { get; set; }
}