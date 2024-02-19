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
                baseB = 16,
                baseC = 9;
            Console.WriteLine($"{a}base{baseA} -> {numberToStringConverter.ConvertToAnySizeBaseString(a, baseA)}");

            double b = 17.85456;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, 8)}");//Должен округлить в большую сторону

            b = 17.85455999999928;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, 8)}");//Должен округлить в большую сторону

            b = 17.85455999999926;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, 8)}");//Должен округлить в меньшую сторону

            b = 2.5;
            Console.WriteLine($"{b}base{baseB} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseB, null)}");

            b = 17.85455999076512;
            Console.WriteLine($"{b}base{baseC} -> {numberToStringConverter.ConvertToAnySizeBaseString(b, baseC, 8)}");//Должен округлить в большую сторону
        }
    }
}
