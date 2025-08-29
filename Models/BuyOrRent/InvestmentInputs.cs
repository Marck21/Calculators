namespace Calculators.Models.BuyOrRent;

public class InvestmentInputs
{
    public decimal InversionInicial { get; set; } = 100000;
    public int AnosInversion { get; set; } = 32;
    public decimal AlquilerMensualAlternativo { get; set; } = 456;
    public double CrecimientoAnualAlquiler { get; set; } = 2.00;
    public bool UsarInversionInicialParaEntrada { get; set; } = false;
    public decimal PrecioVivienda { get; set; } = 258000;
    public decimal Entrada { get; set; } = 53600;
    public double CrecimientoAnualVivienda { get; set; } = 3.50;
    public int AnosHipoteca { get; set; } = 30;
    public double TipoInteresAnual { get; set; } = 2;
    public double CostesCompra { get; set; } = 8.00;
    public decimal GastosFijosCompra { get; set; } = 6000;
    public double CostesVenta { get; set; } = 0;
    public double CrecimientoAnualBolsa { get; set; } = 8.00;
    public int EdadActual { get; set; } = 28;
    public bool EsViviendaHabitual { get; set; } = true;
}