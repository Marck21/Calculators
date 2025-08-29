namespace Calculators.Models.BuyOrRent;
public class CalculatedValues
{
    public decimal TotalGastosAlquiler { get; set; }
    public decimal PrecioVentaEstimado { get; set; }
    public decimal CostesTotalesCompra { get; set; }
    public decimal CostesTotalesVenta { get; set; }
    public decimal ImportePrestamo { get; set; }
    public decimal PagoMensualHipoteca { get; set; }
    public decimal RetornoTotalInmobiliaria { get; set; }
    public decimal ValorCarteraBolsa { get; set; }
    public decimal CosteImpuestosPlusvalias { get; set; }
    public decimal RetornoTotalBolsa { get; set; }
    public decimal FlujoCajaMensualBolsa { get; set; }
    public decimal DiferenciaRetornoTotal { get; set; }
    public string DownPaymentWarningMessage { get; set; } = string.Empty;
    public Severity DownPaymentWarningSeverity { get; set; }
    public decimal ValorRestanteEnBolsa { get; set; }
    public decimal CosteImpuestosPlusvaliasRestante { get; set; }
    public decimal CosteImpuestosPlusvaliasInmueble { get; set; }
    public decimal EntradaNeta { get; set; }
    public decimal PorcentajeFinanciacion { get; set; }
    public decimal CosteTotalIntereses { get; set; }
    public decimal CosteTotalVivienda { get; set; }
    public decimal CosteTotalConIntereses { get; set; }
}