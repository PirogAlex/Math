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
    }
}