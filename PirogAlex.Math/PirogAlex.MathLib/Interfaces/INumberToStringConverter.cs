namespace PirogAlex.MathLib.Interfaces
{
    public interface INumberToStringConverter
    {
        /// <summary>
        /// Converts an input integer to a string of the specified radix
        /// </summary>
        /// <param name="inputValue">input integer value</param>
        /// <param name="expectedBaseFormat">expected radix</param>
        /// <returns></returns>
        string ConvertToAnySizeBaseString(int inputValue, int expectedBaseFormat);

        /// <summary>
        /// Converts an input double to a string of the specified radix
        /// </summary>
        /// <param name="inputValue">input double value</param>
        /// <param name="expectedBaseFormat">expected radix</param>
        /// <param name="precision">decimal part precision after converted in radix. Default value is 10</param>
        /// <returns></returns>
        string ConvertToAnySizeBaseString(double inputValue, int expectedBaseFormat, int? precision);
    }
}