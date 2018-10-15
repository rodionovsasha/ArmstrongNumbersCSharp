using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using static System.TimeSpan;

namespace ArmstrongNumbersCSharp
{
    internal static class ArmstrongNumbers
    {
        private const int AmountOfSimpleDigits = 10;

        private const long MaxNumber = long.MaxValue;

        //private const long MaxNumber = 1000;
        
        private static readonly long[,] ArrayOfPowers = new long[AmountOfSimpleDigits, GetDigitsAmount(MaxNumber) + 1];
        private static int _counter = 1;

        private static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            InitArrayOfPowers();

            var result = GetNumbers();

            foreach (var armstrongNumber in result)
            {
                Console.WriteLine($"{_counter++}. {armstrongNumber}");
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Execution time: {FromMilliseconds(elapsedMs).Seconds}sec");
        }

        private static IEnumerable<long> GetNumbers()
        {
            var armstrongNumbers = new SortedSet<long>();

            //Main loop
            for (long i = 1; i < MaxNumber; i = GetNextNumber(i))
            {
                if (i < 0)
                {
                    break; // the maximum value is reached
                }

                var sumOfPowers = GetSumOfPowers(i);
                if (sumOfPowers <= MaxNumber && IsArmstrongNumber(sumOfPowers))
                {
                    armstrongNumbers.Add(sumOfPowers);
                }
            }

            return armstrongNumbers;
        }

        private static long GetNextNumber(long number)
        {
            var copyOfNumber = number;
            if (IsGrowingNumber(copyOfNumber))
            {
                // here we have numbers where each digit not less than previous one and not more than next one: 12, 1557, 333 and so on.
                return ++copyOfNumber;
            }

            // here we have numbers which end in zero: 10, 20, ..., 100, 110, 5000, 1000000 and so on.
            long lastNumber = 1; //can be: 1,2,3..., 10,20,30,...,100,200,300,...

            while (copyOfNumber % 10 == 0)
            {
                // 5000 -> 500 -> 50: try to get the last non-zero digit
                copyOfNumber = copyOfNumber / 10;
                lastNumber = lastNumber * 10;
            }

            var lastNonZeroDigit = copyOfNumber % 10;

            return number + lastNonZeroDigit * lastNumber / 10; //e.g. number=100, lastNumber=10, lastNonZeroDigit=1
        }
        
        //135 returns true:  1 < 3 < 5
        //153 returns false: 1 < 5 > 3
        private static bool IsGrowingNumber(long number)
        {
            return (number + 1) % 10 != 1;
        }

        private static long GetSumOfPowers(long number)
        {
            var currentNumber = number;
            var power = GetDigitsAmount(currentNumber);
            long currentSum = 0;
            while (currentNumber > 0)
            {
                currentSum = currentSum + ArrayOfPowers[currentNumber % 10, power]; // get powers from array by indexes and then the sum.
                currentNumber /= 10;
            }

            return currentSum;
        }

        private static bool IsArmstrongNumber(long number)
        {
            return number == GetSumOfPowers(number);
        }

        private static void InitArrayOfPowers()
        {
            for (var i = 0; i < AmountOfSimpleDigits; i++)
            {
                for (var j = 0; j < GetDigitsAmount(MaxNumber) + 1; j++)
                {
                    ArrayOfPowers[i, j] = (long) BigInteger.Pow(i, j);
                }
            }

            Debug.Assert(ArrayOfPowers[0, 0] == 1);
            Debug.Assert(ArrayOfPowers[2, 2] == 4);
            Debug.Assert(ArrayOfPowers[9, 4] == 6561);
        }

        private static int GetDigitsAmount(long number)
        {
            return (int) (Math.Log10(number) + 1);
        }
    }
}