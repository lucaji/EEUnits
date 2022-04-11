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

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable


namespace EEUnits {
    public class SIPrefix : IEquatable<SIPrefix?> {

        public static SIPrefix Y => new("yotta", "Y", 24);
        public static SIPrefix Z => new("zetta", "Z", 21);
        public static SIPrefix E => new("exa", "E", 18);
        public static SIPrefix P => new("peta", "P", 15);
        public static SIPrefix T => new("tera", "T", 12);
        public static SIPrefix G => new("giga", "G", 9);
        public static SIPrefix M => new("mega", "M", 6);
        public static SIPrefix k => new("kilo", "k", 3);
        public static SIPrefix K => new("kilo", "K", 3);

        public static SIPrefix Unity { get => new SIPrefix("", "", 0); }

        public static SIPrefix m => new("milli", "m", -3);
        public static SIPrefix mu => new("micro", "μ", -6);
        public static SIPrefix u => new("micro", "u", -6);
        public static SIPrefix n => new("nano", "n", -9);
        public static SIPrefix p => new("pico", "p", -12);
        public static SIPrefix f => new("femto", "f", -15);
        public static SIPrefix a => new("atto", "a", -18);
        public static SIPrefix z => new("zepto", "z", -21);
        public static SIPrefix y => new("yocto", "y", -24);


        public static List<SIPrefix> Prefixes = new List<SIPrefix>() {
            Y, Z, E, P,
            T, G, M, k, K,

            SIPrefix.Unity,

            m, mu, u, n, p,
            f, a, z, y
        };

        public static int ExpForPrefix(char p) {
            var pp = Prefixes.FirstOrDefault(s => s.Symbol.Equals(p));
            return pp?.Exponent ?? 0;
        }

        public static SIPrefix? PrefixForExp(int exp) {
            var p = Prefixes.FirstOrDefault(s => s.Exponent == exp);
            return p;
        }

        /// <summary>
        /// Returns null if the given string does not posses any multiplier prefix.
        /// </summary>
        /// <param name="u">the unit string to parse</param>
        /// <returns></returns>
        public static SIPrefix? PrefixForUnit(string u) {
            return Prefixes.FirstOrDefault(p => p.Symbol.Equals(u));            
        }

        private SIPrefix(string n, string s, int exp) {
            Name = n;
            Symbol = s;
            Exponent = exp;
        }


        public string Name { get; }
        public string Symbol { get; }
        public int Exponent { get; }
        public double Decimal { 
            get {
                var expo = Math.Abs(this.Exponent);
                var dec = Math.Pow(10.0, expo);
                return this.Exponent >= 0 ? dec : 1/dec;
            }
        }

        public override string ToString() {
            return this.Symbol;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as SIPrefix);
        }

        public bool Equals(SIPrefix? other) {
            return other != null &&
                    Name == other.Name;
        }

        public override int GetHashCode() {
            return -1758840423 + Name.GetHashCode();
        }
    }
    

}



/**
 * 
 * Unit Name            Symbol  Symbol(ASCII)   Quantity                    Quantity Symbol     QS(ASCII)
 * Ampere                   A       A               Electric current        I
 * Volt                     V       V               Voltage                 U, V
 * Volt                     Vrms    Vrms            Voltage                 U, V
 * Volt                     Vp      Vp              Voltage                 U, V
 * Volt                     Vpp     Vpp             Voltage                 U, V
 * Volt                     V       V               Electromotive force     E
 * Volt                     V       V               Potential difference    Δφ
 * Ohm                      Ω       Ohm             Resistance              R
 * Watt                     W       W               Electric Power          P
 * Decibel milliwatt        dBm     dBm             Electric Power          P
 * Decibel watt             dBW     dBW             Electric Power          P
 * Volt Ampere Reactive     var     var             Reactive Power          Q
 * Volt Ampere              VA      VA              Apparent Power          S
 * Farad                    F       F               Capacitance             C
 * Henry                    H       H               Inductance              L
 * siemens                  S       S               Conductance             G
 * mho                      S       S               Conductance             G
 * siemens                  S       S               Admittance              Y
 * mho                      S       S               Admittance              Y
 * Coulomb                  C       C               Electric charge         Q
 * Ampere per hour          Ah      Ah              Electric charge         Q
 * Joule                    J       J               Energy                  E
 * Electron-volt            eV      eV              Energy                  E
 * Ohm per meter            Ω⋅m      Ohm*m          Resistivity             ρ
 * siemens per meter        S/m     S/m             Conductivity            σ
 * Volt per meter           V/m                     Electric field          E
 * Newtons per coulomb      N/C                     Electric field          E
 * Volt per meter           V⋅m     Vm              Electric flux           Φe
 * Tesla                    T                       Magnetic field          B
 * Gauss                    G                       Magnetic field          B
 * Weber                    Wb                      Magnetic flux           Φm
 * Hertz                    Hz                      Frequency               f
 * Seconds                  s                       Time                    t
 * Meter                    m                       Length                  l
 * Square Meter             m2                      Area                    A
 * Decibel                  dB 
 * Parts per million        ppm
 * 
 */


