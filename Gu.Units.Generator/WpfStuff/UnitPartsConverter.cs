﻿namespace Gu.Units.Generator.WpfStuff
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gu.Units.Generator.Annotations;

    public class UnitPartsConverter : TypeConverter
    {
        private static string[] superscripts = { "¹", "²", "³" };
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            int sign = 1;
            var s = (string)value;
            //s = s.Replace("⋅", "*");
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            var symbols = UnitBase.AllUnitsStatic.Select(x => x.Symbol).ToArray();
            var symbolsPattern = string.Join("|", new[] { "1" }.Concat(symbols));
            var superscriptsPattern = string.Join("|", superscripts);
            var pattern = string.Format(
@"(?<Unit>
    (?<Symbol>({0}))
    (?<Power>
        ((?:\^)[\+\-]?\d+)
        |
        (⁻?({1}))
    )?
    |
    (?<Op>[⋅\*\/])
)", symbolsPattern, superscriptsPattern);
            var matches = Regex.Matches(s, pattern, RegexOptions.IgnorePatternWhitespace)
                               .OfType<Match>()
                               .ToArray();
            var parts = new UnitParts();
            bool expectsSymbol = true;
            foreach (Match match in matches)
            {
                if (expectsSymbol)
                {
                    var symbol = match.Groups["Symbol"].Value;
                    if (symbol == "1")
                    {
                        expectsSymbol = false;
                        continue;
                    }
                    var unit = UnitBase.AllUnitsStatic.Single(x => x.Symbol == symbol);
                    int p = ParsePower(match.Groups["Power"].Value);
                    parts.Add(new UnitAndPower(unit, sign * p));
                    expectsSymbol = false;
                }
                else
                {
                    var op = match.Groups["Op"].Value;
                    if (op == "/")
                    {
                        if (sign < 0)
                        {
                            throw new FormatException("/ can't appear twice found at position:" + match.Index);
                        }
                        else
                        {
                            sign = -1;
                        }
                    }
                    expectsSymbol = true;
                }
                //capture["Unit"]
                //parts.Add([""]);
            }
            return parts;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value == null ? null : ((UnitParts)value).Expression;
        }

        private int ParsePower(string power)
        {
            if (power == "")
            {
                return 1;
            }
            if (power[0] == '⁻')
            {
                var indexOf = Array.IndexOf(superscripts, power.Substring(1));
                if (indexOf < 0)
                {
                    throw new FormatException();                    
                }
                return -1*(indexOf + 1);
            }
            int p =  Array.IndexOf(superscripts, power) + 1;
            if (p > 0)
            {
                return p;
            }
            if (power[0] != '^')
            {
                throw new FormatException();
            }
            p = int.Parse(power.TrimStart('^'));
            return p;
        }
    }
}