namespace Calculators.Models.BuyOrRent;

public class MortgageCalculations
{
    public decimal CostesTotalesCompra { get; set; }
    public decimal CosteTotalVivienda { get; set; }
    public decimal EntradaNeta { get; set; }
    public decimal ImportePrestamo { get; set; }
    public decimal PorcentajeFinanciacion { get; set; }
    public decimal PagoMensualHipoteca { get; set; }
    public decimal CosteTotalIntereses { get; set; }
    public decimal CosteTotalConIntereses { get; set; }
    public string AlertMessage { get; set; } = string.Empty;
    public Severity AlertSeverity { get; set; }
    public decimal PorcentajeSueldoDedicadoHipoteca { get; set; }
    public decimal PagoMensualMaximoRecomendado { get; set; }
}
