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
    Console.WriteLine( $"Prime numbers up to {limit}:" );
    Console.WriteLine( $"Found: {primes.Item1.Count}" );
    Console.WriteLine( $"Elapsed time: {primes.Item2}\n" );
}

static (HashSet<BigInteger>, TimeSpan) SegmentedSieve( BigInteger limit )
{
    Stopwatch sw = Stopwatch.StartNew();
    BigInteger segmentSize = (BigInteger)int.MaxValue; // Segment size is within the int array limit
    BigInteger numSegments = (limit + segmentSize - 1) / segmentSize; // Number of segments needed
    HashSet<BigInteger> primes = [];
    // Simple sieve for small primes up to sqrt(limit)
    BigInteger sqrtLimit = BigIntegerSqrt( limit ) + 1;
    HashSet<BigInteger> smallPrimes = SieveOfEratosthenes( sqrtLimit );
    // Process each segment
    for (BigInteger seg = 0; seg < numSegments; seg++)
    {
        BigInteger low = seg * segmentSize;
        BigInteger high = BigInteger.Min( (seg + 1) * segmentSize - 1, limit );
        bool[] isPrimeSegment = new bool[(int)(high - low + 1)];
        for (int i = 0; i < isPrimeSegment.Length; i++)
        {
            isPrimeSegment[i] = true;
        }
        // Mark non-primes in the current segment using small primes
        foreach (BigInteger p in smallPrimes)
        {
            BigInteger start = BigInteger.Max( p * p, (low + p - 1) / p * p );

            for (BigInteger multiple = start; multiple <= high; multiple += p)
            {
                isPrimeSegment[(int)(multiple - low)] = false;
            }
        }
        // Collect primes in the current segment
        for (BigInteger i = low; i <= high; i++)
        {
            if (i > 1 && isPrimeSegment[(int)(i - low)])
            {
                primes.Add( i );
            }
        }
    }
    return (primes, sw.Elapsed);
}
static HashSet<BigInteger> SieveOfEratosthenes( BigInteger limit )
{
    bool[] isPrime = new bool[(int)limit + 1];
    for (BigInteger i = 2; i <= limit; i++)
    {
        isPrime[(int)i] = true;
    }
    for (BigInteger p = 2; p * p <= limit; p++)
    {
        if (isPrime[(int)p])
        {
            for (BigInteger multiple = p * p; multiple <= limit; multiple += p)
            {
                isPrime[(int)multiple] = false;
            }
        }
    }
    HashSet<BigInteger> primes = [];
    for (BigInteger i = 2; i <= limit; i++)
    {
        if (isPrime[(int)i])
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
            return mid; // Perfect square
        }
        else if (midSquared < n)
        {
            low = mid + 1;
        }
        else
        {
            high = mid - 1;
        }
    }

    // When the loop exits, high is the closest approximation
    return high;
}
