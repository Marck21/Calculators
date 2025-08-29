using Calculators.Models.BuyOrRent;
using Calculators.Models.Mortage;
using static Calculators.Helpers.BuyOrRentHelper;

namespace Calculators.Services;

public interface IBuyOrRentService
{
    CalculatedValues Calculate( InvestmentInputs inputs, ref List<ChartSeries> RealEstateChartSeries, ref List<ChartSeries> StockChartSeries, ref string[] XAxisLabels, ChartOptions RealEstateChartOptions, ChartOptions StockChartOptions );
}

public class BuyOrRentService : IBuyOrRentService
{
    public CalculatedValues Calculate( InvestmentInputs inputs, ref List<ChartSeries> RealEstateChartSeries, ref List<ChartSeries> StockChartSeries, ref string[] XAxisLabels, ChartOptions RealEstateChartOptions, ChartOptions StockChartOptions )
    {
        var calculated = new CalculatedValues();

        calculated.CostesTotalesCompra = (inputs.PrecioVivienda * ((decimal)inputs.CostesCompra / 100)) + inputs.GastosFijosCompra;
        calculated.CosteTotalVivienda = inputs.PrecioVivienda + calculated.CostesTotalesCompra;
        calculated.EntradaNeta = inputs.Entrada - calculated.CostesTotalesCompra;

        if ( inputs.PrecioVivienda > 0 )
        {
            var netDownPaymentPercentage = calculated.EntradaNeta / inputs.PrecioVivienda;
            string baseMessage = $"Tu entrada neta equivale al <strong>{netDownPaymentPercentage:P1}</strong> del precio. ";

            if ( calculated.EntradaNeta < 0 )
            {
                calculated.DownPaymentWarningMessage = "¡Error! La aportación inicial no cubre los costes de compra.";
                calculated.DownPaymentWarningSeverity = Severity.Error;
            }
            else if ( netDownPaymentPercentage < 0.1m )
            {
                calculated.DownPaymentWarningMessage = baseMessage + "Atención: Aportas menos del 10% del coste total, lo que puede ser arriesgado.";
                calculated.DownPaymentWarningSeverity = Severity.Warning;
            }
            else if ( netDownPaymentPercentage < 0.2m )
            {
                calculated.DownPaymentWarningMessage = baseMessage + "Nota: Estás en un rango razonable, pero por debajo del 20% recomendado.";
                calculated.DownPaymentWarningSeverity = Severity.Info;
            }
            else
            {
                calculated.DownPaymentWarningMessage = baseMessage + "¡Excelente! Aportas más del 20% del coste total, una posición muy sólida.";
                calculated.DownPaymentWarningSeverity = Severity.Success;
            }
        }
        else { calculated.DownPaymentWarningMessage = string.Empty; }

        var realEstateNetValueData = new List<double>();
        var realEstateLeftoverStockNetData = new List<double>();
        var realEstateTaxesPaidData = new List<double>();
        var realEstateTaxesExemptData = new List<double>();
        var realEstateLeftoverTaxesData = new List<double>();
        var stockMarketNetValueData = new List<double>();
        var stockMarketTaxesData = new List<double>();
        var yearsForChart = new List<string>();

        calculated.ImportePrestamo = inputs.PrecioVivienda - Math.Max( 0, calculated.EntradaNeta );
        calculated.PorcentajeFinanciacion = (inputs.PrecioVivienda > 0) ? calculated.ImportePrestamo / inputs.PrecioVivienda : 0;
        calculated.PagoMensualHipoteca = FinancialMath.CalculateMonthlyMortgagePayment( calculated.ImportePrestamo, (decimal)inputs.TipoInteresAnual / 100, inputs.AnosHipoteca );

        decimal inversionRestante = Math.Max( 0, inputs.InversionInicial - inputs.Entrada );

        for ( int year = 1; year <= inputs.AnosInversion; year++ )
        {
            int edadAlVenderAnual = inputs.EdadActual + year;
            bool esExento = edadAlVenderAnual >= 65 && inputs.EsViviendaHabitual;

            decimal precioVentaEstimadoAnual = inputs.PrecioVivienda * (decimal)Math.Pow( 1 + (double)inputs.CrecimientoAnualVivienda / 100, year );
            decimal plusvaliaInmueble = precioVentaEstimadoAnual - inputs.PrecioVivienda;
            decimal impuestosPlusvaliaInmueble = FinancialMath.CalculateProgressiveCapitalGainsTax( plusvaliaInmueble );

            decimal costesVentaAnuales = precioVentaEstimadoAnual * ((decimal)inputs.CostesVenta / 100);
            decimal pagosHipotecaAcumulados = calculated.PagoMensualHipoteca * Math.Min( year, inputs.AnosHipoteca ) * 12;
            decimal principalPagado = FinancialMath.CalculatePrincipalPaid( calculated.ImportePrestamo, (decimal)inputs.TipoInteresAnual / 100, inputs.AnosHipoteca, year );
            decimal interesesPagadosAnuales = pagosHipotecaAcumulados - principalPagado;

            decimal patrimonioBrutoVivienda = precioVentaEstimadoAnual - interesesPagadosAnuales - calculated.CostesTotalesCompra - costesVentaAnuales;
            decimal patrimonioNetoVivienda = patrimonioBrutoVivienda - (esExento ? 0 : impuestosPlusvaliaInmueble);

            decimal valorRestanteEnBolsaAnual = inversionRestante * (decimal)Math.Pow( 1 + (double)inputs.CrecimientoAnualBolsa / 100, year );
            decimal plusvaliaRestante = valorRestanteEnBolsaAnual - inversionRestante;
            decimal impuestosRestanteAnual = FinancialMath.CalculateProgressiveCapitalGainsTax( plusvaliaRestante );
            decimal valorNetoRestanteEnBolsa = valorRestanteEnBolsaAnual - impuestosRestanteAnual;

            decimal retornoTotalInmobiliariaAnual = patrimonioNetoVivienda + valorNetoRestanteEnBolsa;

            decimal valorCarteraCompletaAnual = inputs.InversionInicial * (decimal)Math.Pow( 1 + (double)inputs.CrecimientoAnualBolsa / 100, year );
            decimal plusvaliaCarteraCompleta = valorCarteraCompletaAnual - inputs.InversionInicial;
            decimal impuestosCarteraCompleta = FinancialMath.CalculateProgressiveCapitalGainsTax( plusvaliaCarteraCompleta );
            decimal retornoTotalBolsaAnual = valorCarteraCompletaAnual - impuestosCarteraCompleta;

            int step = Math.Max( 1, inputs.AnosInversion / 10 );
            if ( year == 1 || year % step == 0 || year == inputs.AnosInversion )
            {
                realEstateNetValueData.Add( Math.Round( (double)patrimonioNetoVivienda ) );
                realEstateLeftoverStockNetData.Add( Math.Round( (double)valorNetoRestanteEnBolsa ) );

                realEstateTaxesPaidData.Add( esExento ? 0 : Math.Round( (double)impuestosPlusvaliaInmueble ) );
                realEstateTaxesExemptData.Add( esExento ? Math.Round( (double)impuestosPlusvaliaInmueble ) : 0 );
                realEstateLeftoverTaxesData.Add( Math.Round( (double)impuestosRestanteAnual ) );

                stockMarketNetValueData.Add( Math.Round( (double)retornoTotalBolsaAnual ) );
                stockMarketTaxesData.Add( Math.Round( (double)impuestosCarteraCompleta ) );
                yearsForChart.Add( year.ToString() );
            }

            if ( year == inputs.AnosInversion )
            {
                calculated.TotalGastosAlquiler = FinancialMath.CalculateSimulatedRentExpense( inputs.AlquilerMensualAlternativo, (decimal)inputs.CrecimientoAnualAlquiler / 100, year );
                calculated.PrecioVentaEstimado = precioVentaEstimadoAnual;
                calculated.CostesTotalesVenta = costesVentaAnuales;
                calculated.CosteTotalIntereses = interesesPagadosAnuales;
                calculated.CosteImpuestosPlusvaliasInmueble = impuestosPlusvaliaInmueble;
                calculated.ValorRestanteEnBolsa = valorRestanteEnBolsaAnual;
                calculated.CosteImpuestosPlusvaliasRestante = impuestosRestanteAnual;
                calculated.RetornoTotalInmobiliaria = retornoTotalInmobiliariaAnual;

                calculated.ValorCarteraBolsa = valorCarteraCompletaAnual;
                calculated.CosteImpuestosPlusvalias = impuestosCarteraCompleta;
                calculated.RetornoTotalBolsa = retornoTotalBolsaAnual;
                calculated.FlujoCajaMensualBolsa = (retornoTotalBolsaAnual - inputs.InversionInicial) / (inputs.AnosInversion > 0 ? inputs.AnosInversion * 12 : 1);

                calculated.DiferenciaRetornoTotal = calculated.RetornoTotalBolsa - calculated.RetornoTotalInmobiliaria;
                calculated.CosteTotalConIntereses = calculated.CosteTotalVivienda + calculated.CosteTotalIntereses;
            }
        }

        double maxValue = 0;
        for ( int i = 0; i < realEstateNetValueData.Count; i++ ) { maxValue = Math.Max( maxValue, realEstateNetValueData[i] + realEstateLeftoverStockNetData[i] + realEstateTaxesPaidData[i] + realEstateTaxesExemptData[i] + realEstateLeftoverTaxesData[i] ); }
        for ( int i = 0; i < stockMarketNetValueData.Count; i++ ) { maxValue = Math.Max( maxValue, stockMarketNetValueData[i] + stockMarketTaxesData[i] ); }
        if ( maxValue > 0 )
        {
            double interval = maxValue / 8.0;
            double magnitude = Math.Pow( 10, Math.Floor( Math.Log10( interval ) ) );
            int syncedTicks = (int)(Math.Ceiling( interval / magnitude ) * magnitude);
            RealEstateChartOptions.YAxisTicks = syncedTicks;
            StockChartOptions.YAxisTicks = syncedTicks;
        }

        XAxisLabels = [.. yearsForChart];
        RealEstateChartSeries =
        [
            new() { Name = "Patrimonio Inmobiliario (Neto)", Data = [.. realEstateNetValueData] },
            new() { Name = "Resto en Bolsa (Neto)", Data = [.. realEstateLeftoverStockNetData] },
            new() { Name = "Impuestos Inmueble (Pagados)", Data = [.. realEstateTaxesPaidData] },
            new() { Name = "Impuestos Inmueble (Exentos)", Data = [.. realEstateTaxesExemptData] },
            new() { Name = "Impuestos Bolsa (Restante)", Data = [.. realEstateLeftoverTaxesData] },
        ];
        StockChartSeries =
        [
            new() { Name = "Bolsa (Neto)", Data = [.. stockMarketNetValueData] },
            new() { Name = "Impuestos Pagados", Data = [.. stockMarketTaxesData] },
        ];

        return calculated;
    }
}