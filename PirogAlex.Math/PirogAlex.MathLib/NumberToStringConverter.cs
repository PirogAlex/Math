﻿using System;
using System.Globalization;
using PirogAlex.MathLib.Interfaces;

namespace PirogAlex.MathLib
{
    public class NumberToStringConverter : INumberToStringConverter
    {
        public readonly int MaxSupportedBaseFormat = 16;

        private readonly bool _onlyPositiveNumbers;
        private readonly bool _needRoundConvertedValue = true;

        public NumberToStringConverter() { }

        public NumberToStringConverter(bool onlyPositiveNumbers, bool needRoundConvertedValue)
            : this()
        {
            _onlyPositiveNumbers = onlyPositiveNumbers;
            _needRoundConvertedValue = needRoundConvertedValue;
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

        public string ConvertToAnySizeBaseString(double inputValue, int expectedBaseFormat, int? precision)
        {
            if (expectedBaseFormat > MaxSupportedBaseFormat)
                throw new ArgumentException($"Parameter {nameof(expectedBaseFormat)} must be less or equals {MaxSupportedBaseFormat}. Actual value: {expectedBaseFormat}");
            if (_onlyPositiveNumbers && inputValue < 0)
                throw new ArgumentException($"Parameter {nameof(inputValue)} must be positive. Actual value: {inputValue}");
            if (precision.HasValue && precision < 0)
                throw new ArgumentException($"Parameter {nameof(precision)} must be positive. Actual value: {precision}");
            if (!precision.HasValue)
                precision = 10;

            var result = string.Empty;

            double currentValue = inputValue;
            bool needNegotiateSymbol = false;
            if (currentValue < 0)
            {
                needNegotiateSymbol = true;
                currentValue *= -1;
            }

            //Step1
            //Делим исходное число на основание искомого числа и записываем остаток до тех пор, пока неполное частное не будет равно нулю. Полученные остатки записываем в обратном порядке.
            currentValue = Math.Truncate(currentValue);
            while (currentValue > 0)
            {
                var valueRight = currentValue % expectedBaseFormat;
                var valueRightStr = expectedBaseFormat > 10
                    ? ConvertDigitToChar(valueRight)
                    : valueRight.ToString(CultureInfo.InvariantCulture);

                result = $"{valueRightStr}{result}";
                var valueLeft = Math.Truncate(currentValue / expectedBaseFormat);
                currentValue = valueLeft;
            }

            //Step2
            //Умножаем дробную часть на основание искомого числа и записываем полученную в результате умножения целую часть.
            //Повторяем операцию с дробной частью полученного числа до тех пор, пока не получится целое число, либо до необходимого количества знаков после запятой.
            if (Math.Abs(inputValue - Math.Truncate(inputValue)) > 0 && precision > 0)
            {
                result = $"{result}{NumberFormatInfo.InvariantInfo.NumberDecimalSeparator}";

                var currentPrecision = 1;
                currentValue = GetFractionalPart(inputValue);

                var precisionInner = _needRoundConvertedValue ? precision + 1 : precision;

                while (currentValue > 0 && currentPrecision < precisionInner)
                {
                    var valueLeft = Math.Truncate(currentValue * expectedBaseFormat);
                    var valueLeftStr = expectedBaseFormat > 10
                        ? ConvertDigitToChar(valueLeft)
                        : valueLeft.ToString(CultureInfo.InvariantCulture);

                    result = $"{result}{valueLeftStr}";
                    var valueRight = GetFractionalPart(currentValue * expectedBaseFormat);
                    currentValue = valueRight;
                    currentPrecision++;

                    //Округление по правилам математики с учётом ограничения точности числа
                    if (_needRoundConvertedValue && currentPrecision >= precisionInner)
                    {
                        result = ConvertNumberUpIfNeeded(result, expectedBaseFormat, Math.Truncate(currentValue * expectedBaseFormat), GetFractionalPart(currentValue * expectedBaseFormat));
                    }
                }
            }

            if (needNegotiateSymbol)
                result = $"-{result}";

            return result;
        }

        /// <summary>
        /// Округляет сконвертированное значение по правилам математики
        /// </summary>
        /// <param name="inputValue">Значение рассчитанное с желаемой точностью</param>
        /// <param name="expectedBaseFormat">Желаемая база числа</param>
        /// <param name="valueInBaseFormatRange">Значение в пределах указанной базы</param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        private string ConvertNumberUpIfNeeded(string inputValue, int expectedBaseFormat, double valueInBaseFormatRange, double currentValue)
        {
            var valueInSizeOfBase = (int)valueInBaseFormatRange;

            if (CanConvertToUp(expectedBaseFormat, valueInSizeOfBase, currentValue))
                inputValue = ConvertNumberUp(inputValue, expectedBaseFormat);

            return inputValue;
        }

        private bool CanConvertToUp(int expectedBaseFormat, int valueInSizeOfBase, double currentValue)
        {
            var result = false;

            if (expectedBaseFormat % 2 > 0)
            {
                if (valueInSizeOfBase > expectedBaseFormat / 2)
                {
                    result = true;
                }
                else if (valueInSizeOfBase > expectedBaseFormat / 2 - 1)
                {
                    while (currentValue > 0)
                    {
                        var valueLeft = Math.Truncate(currentValue * expectedBaseFormat);

                        // ReSharper disable once PossibleLossOfFraction
                        if (valueLeft > expectedBaseFormat / 2)
                        {
                            result = true;
                            break;
                        }

                        // ReSharper disable once PossibleLossOfFraction
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (valueLeft > expectedBaseFormat / 2 - 1)
                        {
                            var valueRight = GetFractionalPart(currentValue * expectedBaseFormat);
                            currentValue = valueRight;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (valueInSizeOfBase >= expectedBaseFormat / 2)
                {
                    result = true;
                }
                else if (valueInSizeOfBase == expectedBaseFormat / 2 - 1)
                {
                    while (currentValue > 0)
                    {
                        var valueLeft = Math.Truncate(currentValue * expectedBaseFormat);

                        // ReSharper disable once PossibleLossOfFraction
                        if (valueLeft >= expectedBaseFormat / 2)
                        {
                            result = true;
                            break;
                        }

                        // ReSharper disable once PossibleLossOfFraction
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (valueLeft == expectedBaseFormat / 2 - 1)
                        {
                            var valueRight = GetFractionalPart(currentValue * expectedBaseFormat);
                            currentValue = valueRight;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private double GetFractionalPart(double inputValue)
        {
            //Потому что иначе возвращается не точное число, а примерно точное
            //  например: для числа 17.85456 (inputValue - System.Math.Floor(inputValue)) выдаст не 0.85456 , а что-то вида 0.854599999999

            //Данный же вариант вернёт точную часть после запятой
            if (Math.Abs(inputValue - Math.Truncate(inputValue)) > 0)
                return double.Parse($"0{NumberFormatInfo.CurrentInfo.NumberDecimalSeparator}{inputValue.ToString(CultureInfo.InvariantCulture).Split(NumberFormatInfo.InvariantInfo.NumberDecimalSeparator)[1]}");

            return 0;
        }

        private string ConvertDigitToChar(double valueRight)
        {
            if (valueRight <= 9)
                return valueRight.ToString(CultureInfo.InvariantCulture);

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

        private string ConvertNumberUp(string inputValue, int expectedBaseFormat)
        {
            for (int i = inputValue.Length - 1; i >= 0; i--)
            {
                var currentLetter = inputValue[i].ToString();

                if (currentLetter == NumberFormatInfo.InvariantInfo.NumberDecimalSeparator)
                    continue;

                var newValue = currentLetter switch
                {
                    "0" => 1,
                    "1" => 2,
                    "2" => 3,
                    "3" => 4,
                    "4" => 5,
                    "5" => 6,
                    "6" => 7,
                    "7" => 8,
                    "8" => 9,
                    "9" => 10,
                    "A" => 11,
                    "B" => 12,
                    "C" => 13,
                    "D" => 14,
                    "E" => 15,
                    "F" => 16,
                    _ => throw new ArgumentException($"Unable to recognize char: {currentLetter}. This char from unsupported size base system!")
                };

                if (newValue == expectedBaseFormat)
                {
                    newValue = 0;
                }

                var newValueStr = expectedBaseFormat > 10
                    ? ConvertDigitToChar(newValue)
                    : newValue.ToString(CultureInfo.InvariantCulture);
                inputValue = inputValue.Remove(i, 1).Insert(i, newValueStr);

                if (newValue != 0)
                    break;
            }

            return inputValue;
        }
    }
}
