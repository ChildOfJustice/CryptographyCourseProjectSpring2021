using System;
using System.Numerics;

namespace SardorRsa
{
    public class CryptographyTask_2
    {
        public static BigInteger FindPrime(int bitlength)
        {
            //Filling bytes with pseudorandom.
            byte[] randomBytes = new byte[(bitlength / 8)+1];
            Random rand = new Random(Environment.TickCount);
            rand.NextBytes(randomBytes);
            //Making the extra byte 0x0 so the BigInts are unsigned (little endian).
            randomBytes[randomBytes.Length - 1] = 0x0;

            //Setting the bottom bit and top two bits of the number.
            //This ensures the number is odd, and ensures the high bit of N is set when generating keys.
            SetBitInByte(0, ref randomBytes[0]);
            SetBitInByte(7, ref randomBytes[randomBytes.Length - 2]);
            SetBitInByte(6, ref randomBytes[randomBytes.Length - 2]);

            while (true)
            {
                //Performing a Rabin-Miller primality test.
                bool isPrime = RabinMillerTest(new BigInteger(randomBytes), 40);
                if (isPrime)
                {
                    break;
                } else
                {
                    IncrementByteArrayLE(ref randomBytes, 2);
                    var upper_limit = new byte[randomBytes.Length];

                    //Clearing upper bit for unsigned, creating upper and lower bounds.
                    upper_limit[randomBytes.Length - 1] = 0x0;
                    BigInteger upper_limit_bi = new BigInteger(upper_limit);
                    BigInteger lower_limit = upper_limit_bi - 20;
                    BigInteger current = new BigInteger(randomBytes);

                    if (lower_limit<current && current<upper_limit_bi)
                    {
                        //Failed to find a prime, returning -1.
                        //Reached limit with no solutions.
                        return new BigInteger(-1);
                    }
                }
            }

            //Returning working BigInt.
            return new BigInteger(randomBytes);
        }
        
        public static bool RabinMillerTest(BigInteger source, int certainty)
        {
            //Filter out basic primes.
            if (source == 2 || source == 3)
            {
                return true;
            }
            //Below 2, and % 0? Not prime.
            if (source < 2 || source % 2 == 0)
            {
                return false;
            }

            //Finding even integer below number.
            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            //Getting a random BigInt using bytes.
            Random rng = new Random(Environment.TickCount);
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            //Looping to check random factors.
            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    //Generating new random bytes to check as a factor.
                    rng.NextBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                //Checking for x=1 or x=s-1.
                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                {
                    continue;
                }

                //Iterating to check for prime.
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                    {
                        return false;
                    }
                    else if (x == source - 1)
                    {
                        break;
                    }
                }

                if (x != source - 1)
                {
                    return false;
                }
            }

            //All tests have failed to prove composite, so return prime.
            return true;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static void SetBitInByte(int bitNumFromRight, ref byte toSet)
        {
            byte mask = (byte)(1 << bitNumFromRight);
            toSet |= mask;
        }

        /// <summary>
        /// Increments the byte array as a whole, by a given amount. Assumes little endian.
        /// Assumes unsigned randomBytes.
        /// </summary>
        public static void IncrementByteArrayLE(ref byte[] randomBytes, int amt)
        {
            BigInteger n = new BigInteger(randomBytes);
            n += amt;
            randomBytes = n.ToByteArray();
        }
    }
}