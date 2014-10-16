﻿namespace Gu.Units
{
    using System;

    [Serializable]
    public struct LengthUnit : IUnit
    {
        public static readonly LengthUnit Metres = new LengthUnit(1.0, "m");
        public static readonly LengthUnit m = Metres;

        public static readonly LengthUnit Nanometres = new LengthUnit(1E-09, "nm");
        public static readonly LengthUnit nm = Nanometres;

        public static readonly LengthUnit Micrometres = new LengthUnit(1E-06, "µm");
        public static readonly LengthUnit µm = Micrometres;

        public static readonly LengthUnit Millimetres = new LengthUnit(0.001, "mm");
        public static readonly LengthUnit mm = Millimetres;

        public static readonly LengthUnit Centimetres = new LengthUnit(0.01, "cm");
        public static readonly LengthUnit cm = Centimetres;

        public static readonly LengthUnit Decimetres = new LengthUnit(0.1, "dm");
        public static readonly LengthUnit dm = Decimetres;

        public static readonly LengthUnit Kilometres = new LengthUnit(1000, "km");
        public static readonly LengthUnit km = Kilometres;


        private readonly double _conversionFactor;
        private readonly string _symbol;

        public LengthUnit(double conversionFactor, string symbol)
        {
            _conversionFactor = conversionFactor;
            _symbol = symbol;
        }

        public string Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public static Length operator *(double left, LengthUnit right)
        {
            return Length.From(left, right);
        }

        /// <summary>
        /// Converts a value to <see cref="T:Gu.Units.Length "/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The converted value</returns>
        public double ToSiUnit(double value)
        {
            return _conversionFactor * value;
        }

        /// <summary>
        /// Converts a value to <see cref="T:Gu.Units.Metres "/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The converted value</returns>
        public double FromSiUnit(double value)
        {
            return value / _conversionFactor;
        }

        public override string ToString()
        {
            return string.Format("1{0} == {1}{2}", _symbol, this.ToSiUnit(1), Metres.Symbol);
        }
    }
}