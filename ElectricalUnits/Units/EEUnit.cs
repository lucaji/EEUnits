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

using ElectricalUnits;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable enable

namespace EEUnits {

    public enum EEUnitDomainEnum {
        Analog,
        Digital,
        Time,
        Frequency,
        Nonspecified
    }

    public enum EEUnitKindEnum {
        Linear,
        Voltage,
        Current,
        Power,
        Logarithmic,
        Percentage,
        Frequency,
        Time,

    }

   
    /// <summary>
    /// holds the measurement unit for a signal level
    /// the current implementation does not support domain changing nor
    /// value conversion between different units in the same domain
    /// </summary>
    public partial class EEUnit : System.IEquatable<EEUnit?> {

       


        #region DEFAULT UNITS

        public static readonly EEUnit dBV = new EEUnit("dBV", EEUnitKindEnum.Logarithmic, EEUnitDomainEnum.Analog, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit dBm = new EEUnit("dBm", EEUnitKindEnum.Logarithmic, EEUnitDomainEnum.Analog, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit dBu = new EEUnit("dBu", EEUnitKindEnum.Logarithmic, EEUnitDomainEnum.Analog, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit dB = new EEUnit("dB", EEUnitKindEnum.Logarithmic, EEUnitDomainEnum.Nonspecified, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit Volt = new EEUnit("V", EEUnitKindEnum.Voltage, EEUnitDomainEnum.Analog, EERmsConversionsEnum.RmsValue);
        public static readonly EEUnit mVolt = new EEUnit("mV", EEUnitKindEnum.Voltage, EEUnitDomainEnum.Analog, EERmsConversionsEnum.RmsValue);
        public static readonly EEUnit Ampere = new EEUnit("A", EEUnitKindEnum.Current, EEUnitDomainEnum.Analog, EERmsConversionsEnum.RmsValue);
        public static readonly EEUnit mAmpere = new EEUnit("mA", EEUnitKindEnum.Current, EEUnitDomainEnum.Analog, EERmsConversionsEnum.RmsValue);
        public static readonly EEUnit Watt = new EEUnit("W", EEUnitKindEnum.Power, EEUnitDomainEnum.Analog, EERmsConversionsEnum.RmsValue);
        public static readonly EEUnit FSPercentage = new EEUnit("%FS", EEUnitKindEnum.Percentage, EEUnitDomainEnum.Digital, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit dBFS = new EEUnit("dBFS", EEUnitKindEnum.Logarithmic, EEUnitDomainEnum.Digital, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);
        public static readonly EEUnit FS = new EEUnit("FS", EEUnitKindEnum.Linear, EEUnitDomainEnum.Digital, EERmsConversionsEnum.NonConvertible);
        public static readonly EEUnit Seconds = new EEUnit("s", EEUnitKindEnum.Time, EEUnitDomainEnum.Time, EERmsConversionsEnum.NonConvertible);
        public static readonly EEUnit mSeconds = new EEUnit("ms", EEUnitKindEnum.Time, EEUnitDomainEnum.Time, EERmsConversionsEnum.NonConvertible);
        public static readonly EEUnit Hz = new EEUnit("Hz", EEUnitKindEnum.Frequency, EEUnitDomainEnum.Frequency, EERmsConversionsEnum.NonConvertible);
        public static readonly EEUnit Percentage = new EEUnit("%", EEUnitKindEnum.Percentage, EEUnitDomainEnum.Nonspecified, EERmsConversionsEnum.NonConvertible, hasMultipliers: false);

        #endregion


        public EERmsConversionsEnum RmsConversionKind { get; private set; }

        public EEUnitDomainEnum Domain { get; }




        public EEUnitKindEnum UnitKind { get; private set; }


        
        public static readonly EEUnit[] SupportedUnits = {
            FS,
            FSPercentage,
            dBFS,

            Volt,
            mVolt,
            Ampere,
            mAmpere,
            Watt,

            Hz,

            Seconds,
            mSeconds,

            Percentage,

            dB,

            dBV,
            dBu,
            dBm,
        };

        // these are optional POSTFIXES after the recognized measurement unit
        public static readonly string RmsString = "rms";
        public static readonly string PeakString = "p";
        public static readonly string PeakPeakString = "pp";

        public bool HasMultipliers { get; }

        public bool IsUnityMultiplier =>this.Multiplier.Exponent == 0;


        /// <summary>
        /// Returns an instance if a supported measurement unit string is given
        /// </summary>
        /// <param name="unitString">the unitstring cannot contain a numeric value</param>
        /// <returns></returns>
        public static EEUnit? Factory(string unitString) {

         
            if (string.IsNullOrWhiteSpace(unitString)) { return null; }
            unitString = unitString.Trim();

            // if the unit can be rms convertible, the unit string may or may not contain "rms" after the unit symbol
            // check for rms, peak (p) and peak/peak (pp)
            EERmsConversionsEnum rmsType = EERmsConversionsEnum.NonConvertible;
            var isRms = unitString.EndsWith(RmsString);
            if (isRms) { 
                rmsType = EERmsConversionsEnum.RmsValue;
                unitString = unitString.Substring(0, unitString.Length - 3);
            } else {
                var isPeakPeak = unitString.EndsWith(PeakPeakString);
                if (isPeakPeak) {
                    rmsType = EERmsConversionsEnum.PeakPeakValue;
                    unitString = unitString.Substring(0, unitString.Length - 2);
                } else {
                    var isPeak = unitString.EndsWith(PeakString);
                    if (isPeak) {
                        rmsType = EERmsConversionsEnum.PeakValue;
                        unitString = unitString.Substring(0, unitString.Length - 1);
                    }
                }

            }


            var prefix = unitString.First().ToString();
            var um = SIPrefix.PrefixForUnit(prefix);
            if (um is not null) {
                unitString = unitString.Substring(1);
            }

            EEUnit? candidate = SupportedUnits.FirstOrDefault(o => o.BaseUnitSymbol.Equals(unitString));
            if (um is not null) {
                candidate.Multiplier = um;
            }
            if (candidate.RmsConversionKind == EERmsConversionsEnum.RmsValue) {
                candidate.RmsConversionKind = rmsType;
            }
            return candidate;
        }

        private EEUnit(string symbol, EEUnitKindEnum kind, EEUnitDomainEnum domain, EERmsConversionsEnum rmsConversion, bool hasMultipliers = true) {
            // find if there is a multiplier in the symbol string
            var prefix = symbol.First().ToString();
            var um = SIPrefix.PrefixForUnit(prefix);
            if (um is not null) {
                BaseUnitSymbol = symbol.Substring(1);
                Multiplier = um;
            } else {
                BaseUnitSymbol = symbol;
                Multiplier = SIPrefix.Unity;
            }
            

            UnitKind = kind;
            Domain = domain;
            RmsConversionKind = rmsConversion;
            HasMultipliers = hasMultipliers;
        }

       

        /// <summary>
        /// the unit symbol
        /// </summary>
        public string BaseUnitSymbol { get; private set; }

        public string UnitSymbol => Multiplier.Symbol + BaseUnitSymbol;

        /// <summary>
        /// returns true if the unit belongs to the analog domain.
        /// </summary>
        public bool IsAnalog => this.Domain == EEUnitDomainEnum.Analog;

        /// <summary>
        /// returns true if the unit represent a logarithmic scale
        /// </summary>
        public bool IsLog => this.UnitKind == EEUnitKindEnum.Logarithmic;


        /// <summary>
        /// the numeric decimal multiplier or submultiplier constant
        /// </summary>
        public SIPrefix Multiplier { get; set; } 

      

        #region Equality Overrides

        public override string ToString() {
            return UnitSymbol;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as EEUnit);
        }

        public bool Equals(EEUnit? other) {
            return other is not null &&
                   this.BaseUnitSymbol.Equals(other.BaseUnitSymbol);
        }

        public override int GetHashCode() {
            int hashCode = 1334310303;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BaseUnitSymbol);
            hashCode = hashCode * -1521134295 + Multiplier.GetHashCode();
            return hashCode;
        }

        #endregion

    }
    

}


