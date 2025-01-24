using System;
using System.IO;
using Adjustment;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.Model
{
    public class ErrorMessageGenerator
    {
        private LanguageViewModel languageViewModel;

        public ErrorMessageGenerator(LanguageViewModel languageViewModel)
        {
            this.languageViewModel = languageViewModel;
        }

        public string GenerateMessage(Exception ex)
        {
            switch (ex)
            {
                case IOException iOEx:
                    return languageViewModel.IOExceptionMessage;
                case IncorrectGeometryAnalysis igEx:
                    return languageViewModel.GeoAnalysisExceptionMessage;
                case IncorrectAdjustmentResultException iarEx:
                    return languageViewModel.AdjustmentResultExceptionMessage;
                case NetworkNotConnectedException nncEx:
                    return languageViewModel.NetworkNotConnectedMessage;
                case HangingPointException hpEx:
                    return string.Format(languageViewModel.HangingPointMessagePattern, hpEx.pointNumber);
                case DuplicateMeasurementException dpEx:
                    return string.Format(languageViewModel.DuplicateMeasurementMessagePattern, dpEx.fromPointNumber, dpEx.toPointNumber);
                case LoopingMeasurementsException lmEx:
                    return string.Format(languageViewModel.LoopingMeasurementsMessagePattern, lmEx.fromPointNumber, lmEx.toPointNumber);
                default:
                    return languageViewModel.UnknownErrorMessage;
            }
        }
    }
}
