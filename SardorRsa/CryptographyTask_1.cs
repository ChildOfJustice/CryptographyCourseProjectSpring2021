using System.Numerics;

namespace SardorRsa
{
    public class CryptographyTask_1
    {
        public static BigInteger FastPowMod(BigInteger baseNum, BigInteger exponent, BigInteger modulus)
        {
            if (modulus == 1)
                return 0;
            BigInteger curPow = baseNum % modulus;
            BigInteger res = 1;
            while(exponent > 0){
                if (exponent % 2 == 1)
                    res = (res * curPow) % modulus;
                exponent = exponent / 2;
                curPow = (curPow * curPow) % modulus; 
            }
            return res;
        }
        public static BigInteger GreatestCommonDivizor(BigInteger x, BigInteger y)
        {
            BigInteger tmp;

            if (x < y)
            {
                tmp = x;
                x = y;
                y = tmp;
            }

            while (true)
            {
                tmp = x % y;
                x = y;
                y = tmp;

                if (y == 0) break;
            }

            // This will be the GCD
            return x;
        }
        public static BigInteger ExtendedEuclideanAlgorithm(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            
    
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
 
            BigInteger gcd = ExtendedEuclideanAlgorithm(b % a, a, out x, out y);
    
            BigInteger newY = x;
            BigInteger newX = y - (b / a) * x;
    
            x = newX;
            y = newY;
            return gcd;
        }
    }
}