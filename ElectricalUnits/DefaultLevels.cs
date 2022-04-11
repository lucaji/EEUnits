using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EEUnits {
    public partial class EEValue {

        // SOME STANDARD FREQUENCIES
        /// <summary>
        /// Refers to a 1 kHz frequency
        /// </summary>
        public static EEValue DefaultFrequency1Khz {
            get {
                return new EEValue(1.0, EEUnit.Hz);
            }
        }



        // SOME STANDARD LEVELS



        /// <summary>
        /// Returns a 1 Vrms analog linear level
        /// </summary>
        public static EEValue Default1Vrms {
            get {
                return new EEValue(1.0, EEUnit.Volt);
            }
        }

        /// <summary>
        /// Returns a 2,83Vrms reference level (1W @ 8R)
        /// </summary>
        public static EEValue Default2_83Vrms {
            get {
                return new EEValue(2.83, EEUnit.Volt);
            }
        }

        /// <summary>
        /// Returns a 0dBFS level instance
        /// </summary>
        public static EEValue Default0dBFS {
            get {
                return new EEValue(0.0, EEUnit.dBFS);
            }
        }

        /// <summary>
        /// Returns a -10dBu level instance
        /// </summary>
        public static EEValue Default_Minus10dBu {
            get {
                return new EEValue(-10.0, EEUnit.dBu);
            }
        }

        /// <summary>
        /// Returns a -40dBu level instance
        /// </summary>
        public static EEValue Default_Minus40dBu {
            get {
                return new EEValue(-40.0, EEUnit.dBu);
            }
        }

        /// <summary>
        /// Returns a 0dBu analog log level
        /// </summary>
        public static EEValue Default0dBu {
            get {
                return new EEValue(0.0, EEUnit.dBu);
            }
        }

        /// <summary>
        /// Returns a -12dBFS level instance
        /// </summary>
        public static EEValue DefaultMinus12dBFS {
            get {
                return new EEValue(-12.0, EEUnit.dBFS);
            }
        }

        /// <summary>
        /// Returns a 0Vrms signal level
        /// </summary>
        public static EEValue Default0Vrms {
            get {
                return new EEValue(0.0, EEUnit.Volt);
            }
        }

        /// <summary>
        /// Returns a 0 FS digital signal
        /// </summary>
        public static EEValue Default0FS {
            get {
                return new EEValue(0.0, EEUnit.FS);
            }
        }

        /// <summary>
        /// Returns a 0dB ratio
        /// </summary>
        public static EEValue Default0dB {
            get {
                return new EEValue(0.0, EEUnit.dB);
            }
        }

    }
}
