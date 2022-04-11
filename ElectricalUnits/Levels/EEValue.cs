/*
 * 
    Licensed under the MIT license:

    http://www.opensource.org/licenses/mit-license.php

    Copyright(c) 2013 - 2022, Luca Cipressi(lucaji.github.io) lucaji()mail.ru


    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*
*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

#nullable enable

namespace EEUnits {

    public partial class EEValue : IEquatable<EEValue> {

        public EEFormatting Formatter { get; } 

        public EEComparison Comparer { get; }

        public bool AllowNegatives = true;

        private double _TheValue = 0;


        /// <summary>
        /// the numeric value as double
        /// setting the value depends on the unit
        /// </summary>
        public double Value {
            get { return _TheValue * TheUnit.Multiplier.Decimal; }
            set {
                _TheValue = CappedValueFor(value, this.TheUnit);
            }
        }

        public bool AbsoluteValuesOnly { get; set; } = false;

        public bool IsCappedToZeroIfNegative { get; set; } = false;

        /// <summary>
        /// an instance of EEUnit class which manages
        /// analog or digital units as well as
        /// linear and logarithmic units
        /// </summary>
        public EEUnit TheUnit { get; private set; }

        /// <summary>
        /// Gets and sets the level and unit as a compound string
        /// when setting the value, the same unit domain must be
        /// retain (no domain change between digital/analog) and
        /// the operation will fail, leaving the object unchanged.
        /// </summary>
        public string LevelAndUnitString {
            get {
                return Value.ToString() + " " + TheUnit.ToString();
            }
            set {
                var newValue = Factory(value);
                if (newValue is null) { return; }
                _TheValue = newValue.Value;
                TheUnit = newValue.TheUnit;
            }
        }



        public bool IsNegativeInfiniteLogLevel { get { return this.IsLog && _TheValue == Double.NegativeInfinity; } }


        /// <summary>
        /// returns true if the unit expresses values in the analog domain
        /// otherwise it is a digital unit if false.
        /// </summary>
        public bool IsAnalogUnit { get { return TheUnit.IsAnalog; } }

        public bool IsLog { get { return TheUnit.IsLog; } }


        public static EEValue? Factory(string? valueString, EEUnit theUnit, bool allowNegatives = true) {
            if (string.IsNullOrWhiteSpace(valueString)) { return null; }
            bool gotValue;
            double theValue;
            if (allowNegatives) {
                gotValue = double.TryParse(valueString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out theValue);
            } else {
                gotValue = double.TryParse(valueString, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out theValue);
            }
            if (!gotValue) {
                var t = Factory(valueString);
                if (t is null) { return null; }
                if (t.TheUnit != theUnit) {
                    Debug.WriteLine("unit mismatch for " + valueString + " and given " + theUnit);
                }
                return t;
            }
            return new EEValue(theValue, theUnit);

        }

        /// <summary>
        /// A friendly way to deal with numbers and units
        /// 
        /// adimensional numbers being supported (1k, 1dB)
        /// </summary>
        /// <param name="theValueWithUnitString"></param>
        /// <param name="expectedUnit"></param>
        /// <returns></returns>
        public static EEValue? Factory(string? theValueWithUnitString, string? expectedUnit = null) {
            if (string.IsNullOrWhiteSpace(theValueWithUnitString)) { return null; }

            // TODO: allow a unit suggestion and optionally the unit (and its subunits) could be present in the first string


            // trim leading and trailing whitespaces first
            theValueWithUnitString = theValueWithUnitString!.Trim();
            EEUnit? supportedUnit = expectedUnit is null ? null : EEUnit.Factory(expectedUnit);

            string? valuePart;
            string? unitPart;
            EEUnit? theUnit;
            var gotValue = double.TryParse(theValueWithUnitString, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out double theValue);
            if (gotValue) {
                // the string contained only a number then we must have the suggestedUnit
                if (supportedUnit is null) {
                    Debug.WriteLine("Failed parsing " + theValueWithUnitString + " with provided unit <" + (expectedUnit ?? "(null)") + ">.");
                    return null;
                }
                return new EEValue(theValue, supportedUnit);
            }
            // look for whitespace between value and unit ie. "10 mV" it is not required, but we handle it here,
            // to reduce problems in handling the subunits.
            // maybe requiring the space is the best solution, failing to parse 10mV then makes sense.
            // note: some units and multipliers and exceptions (V for Vrms, U..etc) might be a nightmare to fully cover.
            // along with logarithmic attributes, domain, at al.
            var originalLength = theValueWithUnitString.Length;
            var numberOfBlanks = originalLength - (theValueWithUnitString.Replace(" ", "").Length);
            if (numberOfBlanks == 1) {
                var parts = theValueWithUnitString.Split(' ');
                valuePart = parts[0];
                gotValue = double.TryParse(valuePart, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out theValue);
                if (!gotValue) { return null; } // not a value here, then fail
                unitPart = parts[1];
                theUnit = EEUnit.Factory(unitPart);
                if (theUnit is not null) {
                    return new EEValue(theValue, theUnit);
                }
            } else {
                // need testing
                theValueWithUnitString = theValueWithUnitString.Replace(expectedUnit, "");
            }

            theUnit = EEUnit.Factory(expectedUnit!);
            if (theUnit is null) { return null; }
            gotValue = double.TryParse(theValueWithUnitString, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out theValue);
            if (gotValue) {
                return new EEValue(theValue, theUnit);
            }
            return null;
        }

        public static EEValue? Factory(double theValue, string theUnitString) {
            var theUnit = EEUnit.Factory(theUnitString);
            if (theUnit is null) { return null; }
            return new EEValue(theValue, theUnit);
        }

        public static EEValue? Factory(double theValue, EEUnit theUnit) {
            if (theUnit is null) { return null; }
            return new EEValue(theValue, theUnit);
        }

        private EEValue(double theValue, EEUnit theUnit) {
            this.Formatter = new(this);
            this.Comparer = new(this);
            TheUnit = theUnit;
            if (!TheUnit.IsUnityMultiplier) {
                theValue *= TheUnit.Multiplier.Decimal;
                TheUnit.Multiplier = SIPrefix.Unity;
            }
            var cappedValue = CappedValueFor(theValue, theUnit);
            Value = cappedValue;
        }

        private double CappedValueFor(double value, EEUnit unit) {
            // FS
            if (unit.Domain == EEUnitDomainEnum.Digital && unit.UnitKind == EEUnitKindEnum.Linear) {
                if (value > 1) { value = 1; } 
                else if (value < 0) { value = 0; }
            } else
            // dBFS
            if (unit.Domain == EEUnitDomainEnum.Digital && unit.UnitKind == EEUnitKindEnum.Logarithmic) {
                if (value > 0) { value = 0; }
            } else
            // %FS
            if (unit.UnitKind == EEUnitKindEnum.Percentage) {
                if (value > 100) { value = 100; }
                if (value < 0) { value = 0; }
            } else
            if (IsCappedToZeroIfNegative) {
                if (value < 0) { value = 0; }
            }
            return value;
        }




        #region equality and operator overrides

        public override bool Equals(object? obj) {
            return Equals(obj as EEValue);
        }

        public bool Equals(EEValue? other) {
            return other is not null &&
                Value == other.Value &&
                   this.TheUnit.Equals(other.TheUnit);
        }

        public override int GetHashCode() {
            int hashCode = 300764700;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<EEUnit>.Default.GetHashCode(TheUnit);
            return hashCode;
        }

        public static bool operator ==(EEValue? left, EEValue? right) {
            if (left is null || right is null) { return false; }
            return EqualityComparer<EEValue>.Default.Equals(left, right);
        }



        public static bool operator !=(EEValue? left, EEValue? right) {
            return !(left == right);
        }

        public static EEValue? operator +(EEValue? left, EEValue? right) {
            if (left is null || right is null) { return null; }
            if (left.TheUnit != right.TheUnit) { return null; }
            var level = left.Value + right.Value;
            return new EEValue(level, left.TheUnit);
        }

        public static EEValue? operator -(EEValue? left, EEValue? right) {
            if (left is null || right is null) { return null; }
            if (left.TheUnit != right.TheUnit) { return null; }
            var level = left.Value - right.Value;
            return new EEValue(level, left.TheUnit);
        }

        public static EEValue? operator * (EEValue? left, double right) {
            if (left is null) { return null; }
            var level = left.Value * right;
            return new EEValue(level, left.TheUnit);
        }

        public static bool operator < (EEValue? left, EEValue? right) {
            if (left is null || right is null) { return false; }
            if (left.TheUnit.BaseUnitSymbol != right.TheUnit.BaseUnitSymbol) { return false; }
            return left.Value < right.Value;
        }
        public static bool operator >(EEValue? left, EEValue? right) {
            if (left is null || right is null) { return false; }
            if (left.TheUnit.BaseUnitSymbol != right.TheUnit.BaseUnitSymbol) { return false; }
            return left.Value > right.Value;
        }

        public static bool operator <= (EEValue? left, EEValue? right) {
            if (left is null || right is null) { return false; }
            if (left.TheUnit.BaseUnitSymbol != right.TheUnit.BaseUnitSymbol) { return false; }
            return left.Value <= right.Value;
        }

        public static bool operator >= (EEValue? left, EEValue? right) {
            if (left is null || right is null) { return false; }
            if (left.TheUnit.BaseUnitSymbol != right.TheUnit.BaseUnitSymbol) { return false; }
            return left.Value >= right.Value;
        }

        public override string ToString() {
            return this.Formatter.FormattedValueAndUnitString;
        }

        #endregion equality and operator overrides


    }
}