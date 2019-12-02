﻿using System;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using System.Text.RegularExpressions;
using Microsoft.ProgramSynthesis.Features;

/*
 * PROSE documentation on Witness Functions:
 *  https://microsoft.github.io/prose/documentation/prose/tutorial/#ranking
 */

namespace ProseTutorial
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score") { }

        [FeatureCalculator(nameof(Semantics.Substring))]
        public static double Substring(double v, double start, double end) => start * end;

        [FeatureCalculator(nameof(Semantics.AbsPos))]
        public static double AbsPos(double v, double k) => k;

        [FeatureCalculator("k", Method = CalculationMethod.FromLiteral)]
        public static double K(int k) => 1.0 / Math.Abs(k);

        [FeatureCalculator(nameof(Semantics.RelPos))]
        public static double RelPos(double x, double rr) => rr;

        [FeatureCalculator("rr", Method = CalculationMethod.FromLiteral)]
        public static double RR(Tuple<Regex, Regex> tuple) => 1;

    }
}