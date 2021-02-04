using System;
using Erabikata.Models.Input;

namespace Erabikata.Backend.Extensions
{
    public static class EnumConversionExtensions
    {
        public static AnalyzerMode ToAnalyzerMode(this Analyzer analyzer) =>
            analyzer switch
            {
                Analyzer.SudachiA => AnalyzerMode.SudachiA,
                Analyzer.SudachiB => AnalyzerMode.SudachiB,
                Analyzer.SudachiC => AnalyzerMode.SudachiC,
                _ => throw new ArgumentOutOfRangeException(nameof(analyzer), analyzer, null)
            };
    }
}