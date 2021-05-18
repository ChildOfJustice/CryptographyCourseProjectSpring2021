using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using Task_4;

namespace MagentaAlgorithm
{
    public class MagentaCore : FeistelNet, ICypherAlgorithm
    {
        const byte byteSize = 8;
        public byte[] key;
        
        static byte[] MagentaSbox =
        {
            1, 2, 4, 8, 16, 32, 64, 128, 101,
            202, 241, 135, 107, 214, 201, 247, 139, 115,
            230, 169, 55, 110, 220, 221, 223, 219, 211,
            195, 227, 163, 35, 70, 140, 125, 250, 145,
            71, 142, 121, 242, 129, 103, 206, 249, 151,
            75, 150, 73, 146, 65, 130, 97, 194, 225,
            167, 43, 86, 172, 61, 122, 244, 141, 127,
            254, 153, 87, 174, 57, 114, 228, 173, 63,
            126, 252, 157, 95, 190, 25, 50, 100, 200,
            245, 143, 123, 246, 137, 119, 238, 185, 23,
            46, 92, 184, 21, 42, 84, 168, 53, 106,
            212, 205, 255, 155, 83, 166, 41, 82, 164,
            45, 90, 180, 13, 26, 52, 104, 208, 197,
            239, 187, 19, 38, 76, 152, 85, 170, 49,
            98, 196, 237, 191, 27, 54, 108, 216, 213,
            207, 251, 147, 67, 134, 105, 210, 193, 231,
            171, 51, 102, 204, 253, 159, 91, 182, 9,
            18, 36, 72, 144, 69, 138, 113, 226, 161,
            39, 78, 156, 93, 186, 17, 34, 68, 136,
            117, 234, 177, 7, 14, 28, 56, 112, 224,
            165, 47, 94, 188, 29, 58, 116, 232, 181,
            15, 30, 60, 120, 240, 133, 111, 222, 217,
            215, 203, 243, 131, 99, 198, 233, 183, 11,
            22, 44, 88, 176, 5, 10, 20, 40, 80,
            160, 37, 74, 148, 77, 154, 81, 162, 33,
            66, 132, 109, 218, 209, 199, 235, 179, 3,
            6, 12, 24, 48, 96, 192, 229, 175, 59,
            118, 236, 189, 31, 62, 124, 248, 149, 79,
            158, 89, 178, 0
        };
        

        protected override ulong AbstractFeistelFunction(ulong R, ulong RoundKey)
        {
            // 64  64
            //Each Feistel round is expressed as taking (X(1),X(2)), where each X is half the block, 64 bits in length, and replacing it with (X(2),X(1) xor F(X(2),SK(n))), where n is the round number, and SK(n) the n-th subkey.
            //
            //The F function equals the first eight bytes of S(C(3,(X(2),SK(n)))).
            //(X(2),X(1) xor F(X(2),SK(n)))
            //Console.WriteLine(F(R, RoundKey).ToString("X"));
            return F(R, RoundKey);
        }

        public override byte[] Key
        {
            set
            {
                key = value;

                if (key.Length == 16)
                {
                    //128 bit key
                    FeistelRoundQuantity = 6;
                    
                    int currIndex = 0;
                    byte[] tempForKey = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    var temp = BitConverter.ToUInt64(key, 0);
                    var K1 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K2 = temp;
                    
                    
                    RoundKeys = new ulong[FeistelRoundQuantity];
                    RoundKeys[0] = K1;
                    RoundKeys[1] = K1;
                    RoundKeys[2] = K2;
                    RoundKeys[3] = K2;
                    RoundKeys[4] = K1;
                    RoundKeys[5] = K1;
                }
                else if (key.Length == 24)
                {
                    //192 bit key
                    FeistelRoundQuantity = 6;
                    
                    int currIndex = 0;
                    byte[] tempForKey = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    var temp = BitConverter.ToUInt64(key, 0);
                    var K1 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K2 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K3 = temp;
                    
                    RoundKeys = new ulong[FeistelRoundQuantity];
                    RoundKeys[0] = K1;
                    RoundKeys[1] = K2;
                    RoundKeys[2] = K3;
                    RoundKeys[3] = K3;
                    RoundKeys[4] = K2;
                    RoundKeys[5] = K1;
                }
                else if (key.Length == 32)
                {
                    //256 bit key
                    FeistelRoundQuantity = 8;
                    
                    int currIndex = 0;
                    byte[] tempForKey = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    var temp = BitConverter.ToUInt64(key, 0);
                    var K1 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K2 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K3 = temp;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        tempForKey[i] = key[currIndex];
                        currIndex++;
                    }
                    
                    temp = BitConverter.ToUInt64(key, 0);
                    var K4 = temp;
                    
                    RoundKeys = new ulong[FeistelRoundQuantity];
                    RoundKeys[0] = K1;
                    RoundKeys[1] = K2;
                    RoundKeys[2] = K3;
                    RoundKeys[3] = K4;
                    RoundKeys[4] = K4;
                    RoundKeys[5] = K3;
                    RoundKeys[6] = K2;
                    RoundKeys[7] = K1;
                }
                
                
            }
        }
        public void ExportKey(string outPutFile)
        {
            using (var outputStream = File.Open(outPutFile, FileMode.Create))
                outputStream.Write(key, 0, key.Length);
        }
        public void ImportKey(string inPutFile, int keySize)
        {
            
            var temp = new byte[keySize];
            using (var inputStream = File.OpenRead(inPutFile))
                inputStream.Read(temp, 0, keySize); // 8 bytes = 64 bit key

            Key = temp;
        }

        #region MagentaFunctions

        public byte f(byte x)
        {
            return MagentaSbox[x];
        }

        public byte A(byte x, byte y)
        {
            return f((byte)(x ^ f(y)));
        }

        public int PE(byte x, byte y)
        {
            return (A(x, y) << byteSize) | A(y, x);
        }
        
        public BigInteger Pi(BigInteger digit16bytes)
        {
            BigInteger res = 0;
            for (int i = 0; i < byteSize; i++)
            {
                var pe = PE(GetByte(digit16bytes, i), GetByte(digit16bytes, (i+byteSize)));
                //Console.WriteLine(Convert.ToString((long)pe, 2));
                BigInteger tempPe = new BigInteger(pe);
                res <<= 2*byteSize;
                res |= tempPe;
            }
            return res;
        }
        public byte GetByte(BigInteger value, int i)
        {
            uint MaskByte = ((uint)1 << 8) - 1;
            var res = value;
            for (int j = 15-i; j > 0; j--)
            {
                res >>= byteSize;
            }
            return (byte)(res & MaskByte);
        }


        public BigInteger T(BigInteger digit16bytes)
        {
            return Pi(Pi(Pi(Pi(digit16bytes))));
        }
        
        public BigInteger S(BigInteger digit16bytes)
        {
            uint MaskByte = ((uint)1 << 8) - 1;
            
            BigInteger res = 0;
            for (int i = 15; i > 0; i-=2)
            {
                res <<= byteSize;
                res |= ((digit16bytes >> i*byteSize) & MaskByte);
            }
            for (int i = 14; i > 0; i-=2)
            {
                res <<= byteSize;
                res |= ((digit16bytes >> i*byteSize) & MaskByte);
            }
        
            
            return res;
        }
        
        public BigInteger C(int n, BigInteger w)
        {
            if (n == 1)
                return T(w);
            var res = T(w ^ S(C(n - 1, w)));
            
            return res;
        }
        
        public ulong F(BigInteger rightHalf, ulong subKey)
        {
            BigInteger R = rightHalf;
            R <<= byteSize*8;
            R |= subKey;
            
            var fValue = S(C(3, R));
            
            fValue >>= 8 * byteSize;
            
            return (ulong)fValue;
        }
        #endregion
        
        
        
        
        
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