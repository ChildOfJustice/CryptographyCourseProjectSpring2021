using System;

namespace Task_4
{
    public class NotAbstractFeistelNet: FeistelNet
    {
	    const byte Mask4Bit = (1 << 4) - 1;
	    const byte Mask6Bit = (1 << 6) - 1;
	    const uint Mask28Bit = ((uint)1 << 28) - 1;
	    const ulong Mask56Bit = ((ulong)1 << 56) - 1;
	    
        static byte[] ExpandingPermutation =
        {
            32,  1,  2,  3,  4,  5,  4,  5,  6,  7,  8,  9,
            8,  9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32,  1
        };
        static byte[] FeistelPermutation =
        {
            16,  7, 20, 21, 29, 12, 28, 17,
            1, 15, 23, 26,  5, 18, 31, 10,
            2,  8, 24, 14, 32, 27,  3,  9,
            19, 13, 30,  6, 22, 11,  4, 25
        };
        static byte[, ,] SBoxes =
		{
			{
				{ 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
				{ 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
				{ 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
				{ 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
			},
			{
				{ 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
				{ 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
				{ 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
				{ 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
			},
			{
				{ 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
				{ 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
				{ 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
				{ 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 13 }
			},
			{
				{ 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
				{ 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
				{ 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
				{ 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 6, 11, 12, 7, 2, 14 }
			},
			{
				{ 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
				{ 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
				{ 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
				{ 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
			},
			{
				{ 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
				{ 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
				{ 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
				{ 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
			},
			{
				{ 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
				{ 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
				{ 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
				{ 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
			},
			{
				{ 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
				{ 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
				{ 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
				{ 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
			}
		};
        protected override ulong AbstractFeistelFunction(ulong R, ulong RoundKey)
        {
            ulong XorResult = Task_1.Program.Permute(R, ExpandingPermutation) ^ RoundKey;
            ulong FeistelFunctionResult = 0;
            for (byte I = 0, SixBits, Row, Column; I < 8; I++)
            {
                SixBits = (byte)((XorResult >> ((7 - I) * 6)) & Mask6Bit);
                Row = (byte)(((SixBits >> 5) << 1) | (SixBits & 1));
                Column = (byte)((SixBits >> 1) & Mask4Bit);
                FeistelFunctionResult = (FeistelFunctionResult << 4) | SBoxes[I, Row, Column];
            }
            return (ulong)Task_1.Program.Permute(FeistelFunctionResult, FeistelPermutation);
        }

        
        
        static byte[] KeyPermutation =
        {
	        50, 43, 36, 29, 22,	15,  8,  1, 51, 44, 37, 30, 23, 16,
	        9,  2, 52, 45, 38,	31, 24, 17, 10,  3, 53, 46, 39, 32,
	        56, 49, 42, 35, 28,	21, 14,  7, 55, 48, 41, 34, 27, 20,
	        13,  6, 54, 47, 40,	33, 26, 19, 12,  5, 25, 18, 11,  4
        };

        static byte[] RoundKeyPermutation =
        {
	        14, 17, 11, 24,  1,  5,  3, 28, 15,  6, 21, 10,
	        23, 19, 12,  4, 26,  8, 16,  7, 27, 20, 13,  2,
	        41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
	        44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
        };
        static byte[] ShiftsSequence =
        {
	        1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };
        public override byte[] Key
        {
	        set
	        {
		        var temp = BitConverter.ToUInt64(value, 0);
		        if (temp > Mask56Bit)
		        {
			        throw new ArgumentException("Key");
		        }
		        RoundKeys = new ulong[16];
		        ulong PermutedKey = Task_1.Program.Permute(temp, KeyPermutation);
		        ulong C = (ulong)((PermutedKey >> 28) & Mask28Bit), D = (ulong)(PermutedKey & Mask28Bit);
		        for (byte Round = 0, Shift; Round < 16; Round++)
		        {
			        Shift = ShiftsSequence[Round];
			        C = ((C << Shift) | (C >> (28 - Shift))) & Mask28Bit;
			        D = ((D << Shift) | (D >> (28 - Shift))) & Mask28Bit;
			        RoundKeys[Round] = Task_1.Program.Permute((C << 28) | D, RoundKeyPermutation);
		        }
	        }
        }
    }
}