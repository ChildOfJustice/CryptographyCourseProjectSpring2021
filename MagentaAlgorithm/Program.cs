using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Task_4;

namespace MagentaAlgorithm
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            byte[] st = new byte[16];
            st[0] = 115;
            st[1] = 2;
            st[2] = 113;
            st[3] = 46;
            st[4] = 26;
            st[5] = 46;
            st[6] = 186;
            st[7] = 225;
            st[15] = 211;
            st.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();
            
            
            
            byte[] key = new byte[8];
            key[5] = 3;
            key[1] = 3;
            key[2] = 4;
            
            FeistelNet fn = new MagentaCore();
            fn.FeistelRoundQuantity = 6;
            fn.Key = key;
            var result = fn.Encrypt(st);
            result.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();
            
            var decrypted = fn.Decrypt(result);
            decrypted.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();

            
            // MagentaCore magenta2 = new MagentaCore();
            // for (int i = 0; i < 256; i++)
            // {
            //     Console.WriteLine(magenta2.F(i, 100).ToString("X"));
            // }
            
            
            
            
            // MagentaCore magenta3 = new MagentaCore();
            //
            // var t2 = BigInteger.Pow(2, 56) + BigInteger.Pow(2, 120) + BigInteger.Pow(2, 121);
            //
            // Console.WriteLine();
            // var temp2 = magenta3.C(3,t2);
            // Console.WriteLine(temp2.ToString("X"));
            return;
            
            MagentaCore magenta = new MagentaCore();
            
            // Console.WriteLine(Convert.ToString(magenta.f(12), 2));
            // Console.WriteLine(magenta.f(12));

            // Console.WriteLine(3^2);
            // Console.WriteLine(Convert.ToString(magenta.A(3,1), 2));
            // Console.WriteLine(magenta.A(3,1));
            
            //Console.WriteLine(3^2);
            //Console.WriteLine(Convert.ToString(magenta.A(3,1), 2));
            //Console.WriteLine(Convert.ToString(magenta.A(1,3), 2));
            //Console.WriteLine(magenta.A(1,3));
            //
            Console.WriteLine("PE(3,1) binary = " + Convert.ToString(magenta.PE(3,1), 2));
            //Console.WriteLine(magenta.PE(3,1));
            //Console.WriteLine(magenta.PE(3,1));

            var t = BigInteger.Pow(2, 56) + BigInteger.Pow(2, 120) + BigInteger.Pow(2, 121);
            //Console.Write("Test big int: ");
            // foreach (var VARIABLE in  ToBinaryString(t).ToCharArray().Reverse())
            // {
            //     Console.Write(VARIABLE);
            // }
            Console.WriteLine(t.ToString("X"));

            Console.WriteLine();
            var temp = magenta.S(t);
            Console.WriteLine(temp.ToString("X"));
            //Console.WriteLine(magenta.S(t));
            
            //Console.WriteLine(Convert.ToString((long)magenta.Pi(3), 2));
            //Console.WriteLine(magenta.F(3, 100));

            // byte[] st = new byte[8];
            // st[0] = 115;
            // st[1] = 2;
            // st[2] = 113;
            // st[3] = 46;
            // st.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            // Console.WriteLine();
            //
            // FeistelNet fn = new MagentaCore();
            // fn.FeistelRoundQuantity = 6;
            // fn.Key = 2153432223452212;
            // var result = fn.CipherTemplateMethod(st, true);
            // result.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            // Console.WriteLine();

            // var decrypted = fn.CipherTemplateMethod(result, false);
            // decrypted.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            // Console.WriteLine();
        }
        public static string ToBinaryString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }
    }
}