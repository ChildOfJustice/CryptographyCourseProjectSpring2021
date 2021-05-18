using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;

namespace SardorRsa
{
    public class RsaCore
    {
        public const int testConfidence = 20;
        public int NumberSize;
        
        private BigInteger p;
        private BigInteger q;

        
        // Encryption exponent
        public BigInteger e;
        
        public BigInteger n;
        // (p-1)(q-1)
        private BigInteger phi;
        // (1+k*phi)/eC
        public BigInteger d;
        
        
        public int keySize;
        
        
        public RsaCore(int _keySize=516, bool generateKeys=true, bool weak=false)
        {
            p = 0;
            q = 0;
            phi = 0;
            keySize = _keySize;

            if (generateKeys)
            {
                e = 0;
                n = 0;
                GenerateKeys();
            }
        }

        private void GenerateKeys()
        {
            q = CryptographyTask_2.FindPrime(keySize / 2);
            p = CryptographyTask_2.FindPrime(keySize / 2);
            
            n = q * p;
            phi = (p - 1) * (q - 1);

            //Generation encryption exponent:
            byte[] randomBytes = new byte[keySize / 2];
            Random rand = new Random(Environment.TickCount);
            
            BigInteger generatedE;
            
            do
            {
                rand.NextBytes(randomBytes);
                //Making the extra byte 0x0 so the BigInts are unsigned (little endian).
                randomBytes[randomBytes.Length - 1] = 0x0;

                //Setting the bottom bit and top two bits of the number.
                //This ensures the number is odd, and ensures the high bit of N is set when generating keys.
                SetBitInByte(0, ref randomBytes[0]);
                SetBitInByte(7, ref randomBytes[randomBytes.Length - 2]);
                SetBitInByte(6, ref randomBytes[randomBytes.Length - 2]);
                generatedE = new BigInteger(randomBytes);
            }
            while (CryptographyTask_1.GreatestCommonDivizor(phi, generatedE) != 1);

            e = generatedE;
            
            
            
            //Computing D such that ed = 1%x.
            //d = Maths.ModularInverse(Constants.e, x);
            BigInteger x1;
            BigInteger y1;
            CryptographyTask_1.ExtendedEuclideanAlgorithm(e, phi, out x1, out y1);
            d = x1;
            while(d < 0)
                d += phi;


            NumberSize = LogPow2(n, 8) + 1;
        }
        public BigInteger EncryptOneByte(byte mToPow)
        {
            BigInteger exp = e;
            BigInteger modN = n;
            BigInteger modRes = CryptographyTask_1.FastPowMod(mToPow, exp, modN);

            return modRes;
        }
        public byte DecryptOneByte(BigInteger c)
        {
            BigInteger cToPow = c;
            BigInteger exp = d;
            BigInteger modN = n;
            var powed = CryptographyTask_1.FastPowMod(cToPow, exp, modN);
            byte modRes = (byte)(int) powed;
            return modRes;
        }
        
        
        public String GetPublicKeyAsString()
        {
            return e.ToString("X") + "," + n.ToString("X");
        }
        public String GetPrivateKeyAsString()
        {
            return d.ToString("X") + "," + n.ToString("X");
        }
        public void ExportPubKey(string outPutFile)
        {
            var exportedKey = GetPublicKeyAsString();
            var exportedKeyBytes= new ASCIIEncoding().GetBytes(exportedKey);
            using (var outputStream = File.Open(outPutFile, FileMode.Create))
                outputStream.Write(exportedKeyBytes, 0, exportedKeyBytes.Length);
        }
        public void ExportPrivateKey(string outPutFile)
        {
            var exportedKey = GetPrivateKeyAsString();
            var exportedKeyBytes= new ASCIIEncoding().GetBytes(exportedKey);
            using (var outputStream = File.Open(outPutFile, FileMode.Create))
                outputStream.Write(exportedKeyBytes, 0, exportedKeyBytes.Length);
        }
        public void ImportPubKey(string inPutFile)
        {
           
            StreamReader sr = new StreamReader(inPutFile);
            string line = sr.ReadLine();
            StringBuilder sbE = new StringBuilder();
            StringBuilder sbN = new StringBuilder();
            sbE.Append("0");
            sbN.Append("0");// to convert to BigInteger
            
            int i = 0;
            while (line[i] != ',')
            {
                sbE.Append(line[i]);
                i++;
            }
            
            i++;
            
            while (i != line.Length)
            {
                sbN.Append(line[i]);
                i++;
            }
            
            
            e = BigInteger.Parse(sbE.ToString(),NumberStyles.AllowHexSpecifier);
            n = BigInteger.Parse(sbN.ToString(),NumberStyles.AllowHexSpecifier);
            NumberSize = LogPow2(n, 8) + 1;
        }
        public void ImportPrivateKey(string inPutFile)
        {
           
            StreamReader sr = new StreamReader(inPutFile);
            string line = sr.ReadLine();
            StringBuilder sbD = new StringBuilder();
            StringBuilder sbN = new StringBuilder();
            sbD.Append("0");
            sbN.Append("0");// to convert to BigInteger

            int i = 0;
            while (line[i] != ',')
            {
                sbD.Append(line[i]);
                i++;
            }

            i++;
            while (i != line.Length)
            {
                sbN.Append(line[i]);
                i++;
            }

            d = BigInteger.Parse(sbD.ToString(),NumberStyles.AllowHexSpecifier);
            n = BigInteger.Parse(sbN.ToString(),NumberStyles.AllowHexSpecifier);
            NumberSize = LogPow2(n, 8) + 1;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int LogPow2(BigInteger number, int powOfTwo)
        {
            int res = 0;
            
            var temp = number;
            while (temp != 0)
            {
                temp >>= powOfTwo;
                res++;
            }

            return res;
        }
        public static void SetBitInByte(int bitNumFromRight, ref byte toSet)
        {
            byte mask = (byte)(1 << bitNumFromRight);
            toSet |= mask;
        }
    }
}