using System.Collections;
using System.Diagnostics;

namespace HyperPrimeFinder
{
    internal static class Funcs
    {
        internal static void LogS(int limit)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Lengs:{FastSieveOfErastothenes(limit).Length:N0}");
            Console.WriteLine(sw.Elapsed);
            sw.Stop();
        }
        internal static int[] FastSieveOfErastothenes(int limit)
        {
            int primes = 0;
            BitArray unPrimes = new(limit);
            unPrimes[0] = true;
            unPrimes[1] = true;
            for (int i = 2; i < limit; i++)
            {
                if (!unPrimes[i])
                {
                    primes++;
                    for (int j = i; j < limit; j+=i)
                    {
                        unPrimes[j] = true;
                    }
                }
            }
            int[] result = new int[primes];
            int s = -1;
            for (int i = 0; i < limit; i++)
            {
                if (!unPrimes[i])
                {
                    result[s++] = i;
                }
            }
            return result;
        }
    }
}
