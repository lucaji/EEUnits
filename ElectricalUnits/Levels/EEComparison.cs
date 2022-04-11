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

namespace EEUnits {
    public class EEComparison {

        public enum CriteriaComparisonEnum {
            LessThan,
            LessOrEqualsThan,
            Equals,
            GreaterThan,
            GreaterOrEqualsThan,
            PlusMinus,
        }

        public EEComparison(EEValue p) {
            this.ParentValue = p;
        }

        public EEValue ParentValue { get; }



        public static string CriteriaComparisonEnumString(CriteriaComparisonEnum e) {
            switch (e) {
                case CriteriaComparisonEnum.LessThan:
                    return "<";
                case CriteriaComparisonEnum.LessOrEqualsThan:
                    return "≤";
                case CriteriaComparisonEnum.Equals:
                    return "=";
                case CriteriaComparisonEnum.GreaterThan:
                    return ">";
                case CriteriaComparisonEnum.GreaterOrEqualsThan:
                    return "≥";
                case CriteriaComparisonEnum.PlusMinus:
                    return "±";
            }
            return string.Empty;
        }

        //public CriteriaComparisonEnum CompareTo(EEValue other) {

        //}
    }
}
