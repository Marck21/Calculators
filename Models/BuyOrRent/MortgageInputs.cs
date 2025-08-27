namespace Calculators.Models.BuyOrRent;

public class MortgageInputs
{
    public decimal PrecioVivienda { get; set; } = 250000;
    public decimal AportacionInicial { get; set; } = 68500;
    public int AnosHipoteca { get; set; } = 30;
    public double TipoInteresAnual { get; set; } = 2.00;
    public double ImpuestosCompra { get; set; } = 10.00;
    public decimal GastosFijosCompra { get; set; } = 6000;
    public decimal SueldoNetoMensual { get; set; } = 2500;
    public double PorcentajeMaximoSueldo { get; set; } = 35.0;
}