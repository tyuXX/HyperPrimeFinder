using System.Collections;
using System.Diagnostics;
using System.Numerics;

Console.WriteLine( "Enter the limit for finding prime numbers:" );

LogS( 10 );
LogS( 100 );
LogS( 1_000 );
LogS( 10_000 );
LogS( 100_000 );
LogS( 1_000_000 );
LogS( 10_000_000 );
LogS( 100_000_000 );

while (true)
{
    if (BigInteger.TryParse( Console.ReadLine(), out BigInteger limit ) && limit >= 2)
    {
        LogS( limit );
    }
    else
    {
        Console.WriteLine( "Please enter a valid number greater than or equal to 2." );
    }
}
static void LogS( BigInteger limit )
{
    var primes = SegmentedSieve( limit );
    Console.WriteLine( $"Prime numbers up to {limit:N0}:" );
    Console.WriteLine( $"Found: {primes.Item1.Count:N0}" );
    Console.WriteLine( $"Elapsed time: {primes.Item2}\n" );
}
static (HashSet<BigInteger>, TimeSpan) SegmentedSieve( BigInteger limit )
{
    Stopwatch sw = Stopwatch.StartNew();
    BigInteger segmentSize = BigIntegerSqrt(limit); // Segment size
    BigInteger numSegments = (limit + segmentSize - 1) / segmentSize; // Number of segments
    HashSet<BigInteger> primes = []; // Store found primes
    // Simple sieve for small primes up to sqrt(limit)
    HashSet<BigInteger> smallPrimes;
    BigInteger sqrtLimit = BigIntegerSqrt( limit ) + 1;
    if(sqrtLimit >= int.MaxValue) smallPrimes = SieveOfEratosthenes( int.MaxValue );
    else smallPrimes = SieveOfEratosthenes( (int)sqrtLimit );
    // Process each segment in parallel using Parallel.For
    Parallel.For( 0, (int)numSegments, seg =>
    {
        BigInteger low = seg * segmentSize;
        BigInteger high = BigInteger.Min( (seg + 1) * segmentSize - 1, limit );
        BitArray isPrimeSegment = new( (int)(high - low + 1), true );
        // Mark multiples of small primes in the current segment
        foreach (BigInteger p in smallPrimes)
        {
            if (p * p > high) break; // No need to mark multiples for larger primes

            BigInteger start = (low + p - 1) / p * p;
            if (start < p * p) start = p * p;

            for (BigInteger multiple = start; multiple <= high; multiple += p)
            {
                isPrimeSegment[(int)(multiple - low)] = false;
            }
        }
        // Collect primes in the current segment
        lock (primes) // Ensure thread safety when updating the primes HashSet
        {
            for (BigInteger i = low; i <= high; i++)
            {
                if (i > 1 && isPrimeSegment[(int)(i - low)])
                {
                    primes.Add( i );
                }
            }
        }
    } );
    return (primes, sw.Elapsed);
}
static HashSet<BigInteger> SieveOfEratosthenes( int limit )
{
    BitArray isPrime = new(limit + 1);
    for (int i = 2; i <= limit; i++)
    {
        isPrime[i] = true;
    }
    for (int p = 2; p * p <= limit; p++)
    {
        if (isPrime[p])
        {
            for (int multiple = p * p; multiple <= limit; multiple += p)
            {
                isPrime[multiple] = false;
            }
        }
    }
    HashSet<BigInteger> primes = [];
    for (int i = 2; i <= limit; i++)
    {
        if (isPrime[i])
        {
            primes.Add( i );
        }
    }
    return primes;
}
static BigInteger BigIntegerSqrt( BigInteger n )
{
    if (n == 0 || n == 1)
    {
        return n;
    }
    BigInteger low = 0, high = n, mid;
    while (low <= high)
    {
        mid = (low + high) / 2;
        BigInteger midSquared = mid * mid;

        if (midSquared == n)
        {
            return mid;
        }
        if (midSquared < n)
        {
            low = mid + 1;
        }
        else
        {
            high = mid - 1;
        }
    }
    return high;
}
