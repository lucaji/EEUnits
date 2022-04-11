/*
 *

    Licensed under the MIT license:

    http://www.opensource.org/licenses/mit-license.php

    Copyright (c) 2019-2021, Luca Cipressi (lucaji.github.io)

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

using EEUnits;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace ElectricalUnits {

    public enum EERmsConversionsEnum {
        NonConvertible,
        RmsValue,
        PeakValue,
        PeakPeakValue,
    }


    public static class EEConversions {

        #region CONVERSION RATIOS

        private static readonly double dBuRefVrms = 0.7746;
        private static readonly double dBVRefVrms = 1.0;
        private static readonly double dBmRefVrms = 0.8361;


        /* CONVERSION RATIOS
         * 
         * 0dBu = -2,2184 dBV = 0,7745 Vrms = 2,190 Vpp
         * 0dBV =  2,2184 dBu = 1Vrms = 2,8284 Vpp
         * 1Vrms = 2,2184 dBu = 0dBV
         * 
         * 
         * Formulas
         * 
         * L (dB) = 20 log10 (V / Vo) => 20 log10 (dBvalue)
         * V = v0 * 10 ^ (L (dB) / 20)
         * 
         * Electric Power P and Electric Power Level Lp
         * P = p0 * 10 ^  (Lp / 10) W
         * Lp = 10 log10 (P / p0) dB => 10 log10 (dBvalue)
         * Reference electric power p0 = 1W = 0dB
         * 
         * dBm references p0 = 1mW = 0dB
         * 
         * 
         * 
         * Vp = 1,414 * Vrms
         * Vrms = 0,7071 * Vp
         * 
         * Vpp = 2,828 * Vrms
         * Vrms = 0,3535 * Vpp
         * 
         * Vpp = 2 * Vp
         * Vp = Vp / 2
         * 
         * 
         * 
         */


        #endregion


        public static EEValue? ConvertTodBuValue(EEValue val) {
            if (val.TheUnit.Equals(EEUnit.dBu)) { return val; }
            if (val.TheUnit.Domain == EEUnitDomainEnum.Digital) { return null; }
            var dbu = 20.0 * Math.Log10(val.Value / 0.7745);
            return EEValue.Factory(dbu, EEUnit.dBu);
        }

        public static EEValue? ConvertTodBVValue(EEValue val) {
            if (val.TheUnit.Equals(EEUnit.dBV)) { return val; }
            if (val.TheUnit.Domain == EEUnitDomainEnum.Digital) { return null; }
            var dbv = 20.0 * Math.Log10(val.Value);
            return EEValue.Factory(dbv, EEUnit.dBV);
        }

        public static EEValue? ConvertToVrmsValue(EEValue val) {
            if (val.TheUnit.Equals(EEUnit.Volt)) { return val; }
            if (val.TheUnit.Domain == EEUnitDomainEnum.Digital) { return null; }
            double convertedValue;
            if (val.TheUnit.Equals(EEUnit.dBu)) {
                convertedValue = dBuRefVrms * Math.Pow(10.0, val.Value / 20);
            } else if (val.TheUnit.Equals(EEUnit.dBV)) {
                convertedValue = dBVRefVrms * Math.Pow(10.0, val.Value / 20);
            } else { return null; }
            return EEValue.Factory(convertedValue, EEUnit.Volt);
        }

        public static EEValue? ConvertToWattValue(EEValue val, double z) {
            if (z <= 0) { return null; }
            if (val.TheUnit.Equals(EEUnit.Watt)) { return val; }
            if (val.TheUnit.Domain == EEUnitDomainEnum.Digital) { return null; }

            var v = ConvertToVrmsValue(val);
            if (v is null) { return null; }

            return EEValue.Factory(v.Value * v.Value / z, EEUnit.Volt);
        }


        public static EEValue? RmsTranslateTo(EEValue val, EERmsConversionsEnum rce) {
            if (val.TheUnit.Domain == EEUnitDomainEnum.Digital) { return null; }

            string postfix = string.Empty;
            switch (rce) {
                case EERmsConversionsEnum.NonConvertible:
                    break;
                case EERmsConversionsEnum.RmsValue:
                    postfix = EEUnit.RmsString;
                    break;
                case EERmsConversionsEnum.PeakValue:
                    postfix = EEUnit.PeakString;
                    break;
                case EERmsConversionsEnum.PeakPeakValue:
                    postfix = EEUnit.PeakPeakString;
                    break;
            }
            double ratio = 1.0;
            switch (val.TheUnit.RmsConversionKind) {
                case EERmsConversionsEnum.RmsValue:
                    switch (rce) {
                        case EERmsConversionsEnum.PeakValue:
                            ratio = Math.Sqrt(2);
                            break;
                        case EERmsConversionsEnum.PeakPeakValue:
                            ratio = 2.0 * Math.Sqrt(2);
                            break;
                        default: break;
                    }
                    break;
                case EERmsConversionsEnum.PeakValue:
                    switch (rce) {
                        case EERmsConversionsEnum.NonConvertible:
                            break;
                        case EERmsConversionsEnum.RmsValue:
                            ratio = 1.0 / Math.Sqrt(2);
                            break;
                        case EERmsConversionsEnum.PeakValue:
                            break;
                        case EERmsConversionsEnum.PeakPeakValue:
                            ratio = 2.0;
                            break;
                    }
                    break;
                case EERmsConversionsEnum.PeakPeakValue:
                    switch (rce) {
                        case EERmsConversionsEnum.NonConvertible:
                            break;
                        case EERmsConversionsEnum.RmsValue:
                            ratio = 1.0 / (2.0 * Math.Sqrt(2));
                            break;
                        case EERmsConversionsEnum.PeakValue:
                            ratio = 0.5;
                            break;
                        case EERmsConversionsEnum.PeakPeakValue:
                            break;
                    }
                    break;
                default: break;
            }
            return EEValue.Factory(val.Value * ratio, val.TheUnit.BaseUnitSymbol + postfix);
        }
    }
}
