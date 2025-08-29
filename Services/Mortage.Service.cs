using Calculators.Models.Mortage;
using static Calculators.Helpers.MortageHelper;

namespace Calculators.Services;

public interface IMortgageService
{
    MortgageCalculations CalculateMortgage( MortgageInputs inputs );
    decimal CalculateDownPaymentBasedOnPercentage( MortgageInputs inputs, decimal targetTotalPercentage );
}

public class MortgageService : IMortgageService
{
    public MortgageCalculations CalculateMortgage( MortgageInputs inputs )
    {
        var calculated = new MortgageCalculations();

        calculated.CostesTotalesCompra = (inputs.PrecioVivienda * ((decimal)inputs.ImpuestosCompra / 100)) + inputs.GastosFijosCompra;
        calculated.CosteTotalVivienda = inputs.PrecioVivienda + calculated.CostesTotalesCompra;
        calculated.EntradaNeta = inputs.AportacionInicial - calculated.CostesTotalesCompra;

        if ( inputs.PrecioVivienda > 0 )
        {
            var netDownPaymentPercentage = calculated.EntradaNeta / inputs.PrecioVivienda;
            string baseMessage = $"Tu entrada neta equivale al <strong>{netDownPaymentPercentage:P1}</strong> del precio. ";
            if ( calculated.EntradaNeta < 0 ) { calculated.AlertMessage = "¡Error! La aportación inicial no cubre los costes de compra."; calculated.AlertSeverity = Severity.Error; }
            else if ( netDownPaymentPercentage < 0.2m ) { calculated.AlertMessage = baseMessage + "Nota: Estás por debajo del 20% recomendado, una posición de mayor riesgo."; calculated.AlertSeverity = Severity.Warning; }
            else { calculated.AlertMessage = baseMessage + "¡Excelente! Superas el 20% de entrada neta, una posición muy sólida."; calculated.AlertSeverity = Severity.Success; }
        }
        else
        {
            calculated.AlertMessage = string.Empty;
            calculated.AlertSeverity = Severity.Normal;
        }

        calculated.ImportePrestamo = inputs.PrecioVivienda - Math.Max( 0, calculated.EntradaNeta );
        calculated.PorcentajeFinanciacion = (inputs.PrecioVivienda > 0) ? calculated.ImportePrestamo / inputs.PrecioVivienda : 0;
        calculated.PagoMensualHipoteca = FinancialMath.CalculateMonthlyMortgagePayment( calculated.ImportePrestamo, (decimal)inputs.TipoInteresAnual / 100, inputs.AnosHipoteca );

        decimal totalPayments = calculated.PagoMensualHipoteca * inputs.AnosHipoteca * 12;
        calculated.CosteTotalIntereses = (calculated.ImportePrestamo > 0) ? totalPayments - calculated.ImportePrestamo : 0;
        calculated.CosteTotalConIntereses = calculated.CosteTotalVivienda + calculated.CosteTotalIntereses;

        calculated.PorcentajeSueldoDedicadoHipoteca = (inputs.SueldoNetoMensual > 0) ? (calculated.PagoMensualHipoteca / inputs.SueldoNetoMensual) : 0;
        calculated.PagoMensualMaximoRecomendado = inputs.SueldoNetoMensual * ((decimal)inputs.PorcentajeMaximoSueldo / 100);

        return calculated;
    }

    public decimal CalculateDownPaymentBasedOnPercentage( MortgageInputs inputs, decimal targetTotalPercentage )
    {
        if ( inputs.PrecioVivienda <= 0 )
        {
            return inputs.AportacionInicial;
        }

        decimal netPercentage = 0m;
        if ( targetTotalPercentage == 0.30m )
            netPercentage = 0.20m;
        else if ( targetTotalPercentage == 0.20m )
            netPercentage = 0.10m;
        else if ( targetTotalPercentage == 0.10m )
            netPercentage = 0.00m;

        decimal purchaseCosts = (inputs.PrecioVivienda * ((decimal)inputs.ImpuestosCompra / 100)) + inputs.GastosFijosCompra;
        decimal requiredNetDownPayment = inputs.PrecioVivienda * netPercentage;
        decimal totalCashOutlay = purchaseCosts + requiredNetDownPayment;

        return Math.Round( totalCashOutlay );
    }
}