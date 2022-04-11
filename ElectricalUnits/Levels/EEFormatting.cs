/*
 *

    Licensed under the MIT license:

    http://www.opensource.org/licenses/mit-license.php

    Copyright (c) 2019-2022, Luca Cipressi (lucaji.github.io)

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable


namespace EEUnits {
    public class EEFormatting {

        public EEFormatting(EEValue p) {
            this.ParentValue = p;
        }

        public EEValue ParentValue { get; }

        public uint FormattingDecimalPositions = 2;

        /// <summary>
        /// If the decimal part is missing it will be truncated as integer
        /// and the FormattingDecimalPositions field
        /// will determine the maximum number of decimals
        /// </summary>
        public bool FormattingDecimalPositionOptional = false;


       



        #region Formatters

        public string FormattedValueAndUnitString {
            get {
                return FormatDoubleWithUnit();
            }
        }

        public (string value, string unit) FormattedValueAndUnit {
            get {
                var s = FormatDoubleWithUnit().Split(' ');
                if (s.Length == 2) {
                    return new(s[0], s[1]);
                } else {
                    throw new Exception("EEValue FormatDoubleWithUnit split overflow. should not reach.");
                }
            }
        }

        public string FormatDoubleWithUnit() {
            var v = this.ParentValue.Value;
            var unit = this.ParentValue.TheUnit.ToString();
            int exp = this.ParentValue.TheUnit.Multiplier.Exponent;
            bool canIgnoreFracs = this.FormattingDecimalPositionOptional;

            if (double.IsNaN(v)) {
                return "-- " + this.ParentValue.TheUnit;
            }
            if (double.IsInfinity(v)) {
                return "∞ " + this.ParentValue.TheUnit;
            }

            if (this.ParentValue.TheUnit.HasMultipliers) {
                var k = v - Math.Truncate(v);
                canIgnoreFracs = this.FormattingDecimalPositionOptional && (k == 0);
                if (this.ParentValue.TheUnit.Multiplier.Exponent > 0) {
                    var s = unit.First();
                    exp = SIPrefix.ExpForPrefix(s) * -1;
                    if (exp != 0) {
                        unit = unit.Remove(0, 1);
                        var d = (long)Math.Pow(10.0, exp);
                        if (exp < 0) {
                            v *= d;
                        } else {
                            v /= d;
                        }
                        exp = 0;
                    }
                }
                while (v - Math.Floor(v) > 0) {
                    if (Math.Abs(exp) >= 24) { break; }
                    exp -= 3;
                    v *= 1000.0;
                    v = Math.Round(v, 6);
                }
                while (Math.Floor(v).ToString().Length > 3) {
                    if (Math.Abs(exp) >= 24) { break; }
                    exp += 3;
                    v /= 1000.0;
                    v = Math.Round(v, 6);
                }
            }

            var formatString = "0";
            if (canIgnoreFracs) {
                v = Math.Truncate(v);
            } else {
                if (this.FormattingDecimalPositions > 0) { formatString += "."; }
                for (int i = 0; i < this.FormattingDecimalPositions; ++i) {
                    formatString += "0";
                }
            }

            var result = v.ToString(formatString);
            var p = SIPrefix.PrefixForExp(exp);
            if (p is not null) {
                if (p.Symbol != string.Empty) {
                    unit = p.Symbol.ToString() + unit;
                }
            }
            result += " " + unit;
            return result;
        }

        #endregion

    }
}
