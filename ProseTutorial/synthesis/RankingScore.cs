using System;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using System.Text.RegularExpressions;
using Microsoft.ProgramSynthesis.Features;

namespace ProseTutorial
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score") { }

        [FeatureCalculator(nameof(Semantics.Kth))]
        public static double Kth(double v, double d, double n) => d * n;

        [FeatureCalculator("d", Method = CalculationMethod.FromLiteral)]
        public static double D(string d) => 1;

        [FeatureCalculator("n", Method = CalculationMethod.FromLiteral)]
        public static double N(int n) => 1.0 / Math.Abs(n);
    }
}