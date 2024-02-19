using PirogAlex.MathLib;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var numberToStringConverter = new NumberToStringConverter();

            int a = 10,
                baseA = 16,
                baseB = 16;
            Console.WriteLine($"{a}base{baseA} -> {numberToStringConverter.ConvertToAnySizeBaseString(a, baseA)}");

            double b = 17.85456;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, 7)}");

            b = 2.5;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, null)}");
        }
    }
}
