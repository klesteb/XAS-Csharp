
namespace XAS.Core.Utilities {

    // taken from: https://stackoverflow.com/questions/3261451/using-a-bitmask-in-c-sharp
    // with modification.

    /// <summary>
    /// A class to perform bit operations.
    /// </summary>
    /// 
    public static class Bitops {

        /// <summary>
        /// Check if a bit is set.
        /// </summary>
        /// <param name="flags">An integer value.</param>
        /// <param name="flag">The bit to check.</param>
        /// <returns>true if the bit is set.</returns>
        /// 
        public static bool IsSet<T>(T flags, T flag) where T : struct {

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;

        }

        /// <summary>
        /// Set a bit.
        /// </summary>
        /// <param name="flags">A integer value.</param>
        /// <param name="flag">The bit to set.</param>
        /// 
        public static void Set<T>(ref T flags, T flag) where T : struct {

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);

        }

        /// <summary>
        /// Unset a bit.
        /// </summary>
        /// <param name="flags">An integer value.</param>
        /// <param name="flag">The bit to unset.</param>
        /// 
        public static void Unset<T>(ref T flags, T flag) where T : struct {

            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));

        }

    }

}
