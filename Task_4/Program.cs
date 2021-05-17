using System;
using System.Linq;

namespace Task_4
{
    internal class Program
    {
        
        
        public static void Main(string[] args)
        {
            byte[] st = new byte[8];
            st[0] = 1;
            st.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();
            
            byte[] key = new byte[8];
            key[5] = 3;

            FeistelNet fn = new NotAbstractFeistelNet();
            fn.FeistelRoundQuantity = 16;
            fn.Key = key;
            var result = fn.Encrypt(st);
            result.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();
	        
            var decrypted = fn.Decrypt(result);
            decrypted.ToList().ForEach(i => Console.Write(i.ToString() + " "));
            Console.WriteLine();
        }
    }
}