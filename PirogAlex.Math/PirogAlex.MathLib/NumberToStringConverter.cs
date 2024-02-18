using System;
using PirogAlex.MathLib.Interfaces;

namespace PirogAlex.MathLib
{
    public class NumberToStringConverter : INumberToStringConverter
    {
        public readonly int MaxSupportedBaseFormat = 16;

        private readonly bool _onlyPositiveNumbers;

        public NumberToStringConverter()
        {
            
        }

        public NumberToStringConverter(bool onlyPositiveNumbers)
        {
            _onlyPositiveNumbers = onlyPositiveNumbers;
        }

        public string ConvertToAnySizeBaseString(int inputValue, int expectedBaseFormat)
        {
            if (expectedBaseFormat > MaxSupportedBaseFormat)
                throw new ArgumentException($"Parameter {nameof(expectedBaseFormat)} must be less or equals {MaxSupportedBaseFormat}. Actual value: {expectedBaseFormat}");
            if (_onlyPositiveNumbers && inputValue < 0)
                throw new ArgumentException($"Parameter {nameof(inputValue)} must be positive. Actual value: {inputValue}");

            var result = string.Empty;

            int currentValue = inputValue;
            bool needNegotiateSymbol = false;
            if (currentValue < 0)
            {
                needNegotiateSymbol = true;
                currentValue *= -1;
            }

            while (currentValue > 0)
            {
                var valueRight = currentValue % expectedBaseFormat;
                var valueRightStr = expectedBaseFormat > 10
                    ? ConvertDigitToChar(valueRight)
                    : valueRight.ToString();

                result = $"{valueRightStr}{result}";
                var valueLeft = currentValue / expectedBaseFormat;
                currentValue = valueLeft;
            }

            if (needNegotiateSymbol)
            {
                result = $"-{result}";
            }

            return result;
        }

        private string ConvertDigitToChar(int valueRight)
        {
            if (valueRight <= 9)
                return valueRight.ToString();

            return valueRight switch
            {
                10 => "A",
                11 => "B",
                12 => "C",
                13 => "D",
                14 => "E",
                15 => "F",
                _ => throw new ArgumentException($"Unable to cast in char: {valueRight}!")
            };
        }
    }
}
