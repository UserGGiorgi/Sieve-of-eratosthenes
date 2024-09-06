namespace NumberTheory;

public static class SieveOfEratosthenes
{
    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersSequentialAlgorithm(int n)
    {
        if (n <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "The upper limit must be greater than 0.");
        }

        var primes = new List<int>();

        for (int number = 2; number <= n; number++)
        {
            if (IsPrime(number))
            {
                primes.Add(number);
            }
        }

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a modified sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersModifiedSequentialAlgorithm(int n)
    {
        if (n < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "The upper limit must be greater than or equal to 2.");
        }

        var isPrime = new bool[n + 1];
        for (int i = 2; i <= n; i++)
        {
            isPrime[i] = true;
        }

        int limit = (int)Math.Sqrt(n);
        for (int i = 2; i <= limit; i++)
        {
            if (isPrime[i])
            {
                for (int j = i * i; j <= n; j += i)
                {
                    isPrime[j] = false;
                }
            }
        }

        var primes = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
            }
        }

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by data decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentDataDecomposition(int n)
    {
        if (n < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "The upper limit must be greater than or equal to 2.");
        }

        var isPrime = new bool[n + 1];
        for (int i = 2; i <= n; i++)
        {
            isPrime[i] = true;
        }

        int limit = (int)Math.Sqrt(n);
        for (int i = 2; i <= limit; i++)
        {
            if (isPrime[i])
            {
                _ = Parallel.For(i * i, n + 1, j =>
                {
                    if (j % i == 0)
                    {
                        isPrime[j] = false;
                    }
                });
            }
        }

        var primes = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
            }
        }

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by "basic" primes decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentBasicPrimesDecomposition(int n)
    {
        if (n < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "The upper limit must be greater than or equal to 2.");
        }

        var isPrimee = new bool[n + 1];
        for (int i = 2; i <= n; i++)
        {
            isPrimee[i] = true;
        }

        int max = (int)Math.Sqrt(n);
        for (int i = 2; i <= max; i++)
        {
            if (isPrimee[i])
            {
                _ = Parallel.For(i * i, n + 1, j =>
                {
                    if (j % i == 0)
                    {
                        isPrimee[j] = false;
                    }
                });
            }
        }

        var primes = new List<int>();
        for (int j = 2; j <= n; j++)
        {
            if (isPrimee[j])
            {
                primes.Add(j);
            }
        }

        return primes;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using thread pool and signaling construct.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentWithThreadPool(int n)
    {
        if (n < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "The upper limit must be greater than or equal to 2.");
        }

        var isPrime = new bool[n + 1];
        for (int i = 2; i <= n; i++)
        {
            isPrime[i] = true;
        }

        int limit = (int)Math.Sqrt(n);
        var smallPrimes = new List<int>();
        for (int i = 2; i <= limit; i++)
        {
            if (isPrime[i])
            {
                smallPrimes.Add(i);
                for (int j = i * i; j <= limit; j += i)
                {
                    isPrime[j] = false;
                }
            }
        }

        using var countdownEvent = new CountdownEvent(smallPrimes.Count);

        foreach (var prime in smallPrimes)
        {
            _ = ThreadPool.QueueUserWorkItem(_ =>
            {
                for (int j = prime * prime; j <= n; j += prime)
                {
                    isPrime[j] = false;
                }

                var v = countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();

        var primes = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
            }
        }

        return primes;
    }

    private static bool IsPrime(int number)
    {
        if (number < 2)
        {
            return false;
        }

        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}
