using System;

namespace Task_1
{
    public class Program
    {
        
        static byte[] InitialPermutation =
        {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, // 64 -> 0!
            57, 49, 41, 33, 25, 17,  9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
        };
        static public ulong Permute(ulong value, byte[] permutationRule)
        {
            if (permutationRule == null)
            {
                throw new ArgumentNullException(nameof(permutationRule));
            }
            if (permutationRule.Length > sizeof(ulong)*8)
            {
                throw new ArgumentException("Too big array for permutation rule");
            }
            
            ulong permutedValue = 0;
            for (byte i = 0; i < permutationRule.Length; i++)
            {
                //if (des)
                //{
                    if (permutationRule[permutationRule.Length - i - 1] > sizeof(ulong) * 8)
                    {
                        throw new ArgumentException("Value in permutation rule is wrong");
                    }
                    //N -> get the bit number from permutation rule: (permutationRule[permutationRule.Length - i - 1] - 1)
                    //shift and get the new first bit (& 1):  ((value >> N) & 1)
                    //Shift result to add a new byte (permutedValue << 1) |

                    permutedValue = (permutedValue << 1) |
                                    ((value >> (permutationRule[permutationRule.Length - i - 1] - 1)) & 1);
                //}
                // else
                // {
                //     if (permutationRule[permutationRule.Length - i - 1] >= sizeof(ulong) * 8
                //     ) // || permutationRule[permutationRule.Length - i - 1] < 0
                //     {
                //         throw new ArgumentException("Value in permutation rule is wrong");
                //     }
                //     //N -> get the bit number from permutation rule: (permutationRule[permutationRule.Length - i - 1] - 1)
                //     //shift and get the new first bit (& 1):  ((value >> N) & 1)
                //     //Shift result to add a new byte (permutedValue << 1) |
                //
                //     permutedValue = (permutedValue << 1) |
                //                     ((value >> (permutationRule[permutationRule.Length - i - 1])) & 1);
                // }
            }
            return permutedValue;
        }
        
        
        public static void Main(string[] args)
        {
            ulong a = 0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100;
            //                   1_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000
            try
            {
                Console.WriteLine(Convert.ToString((long)Permute(a, InitialPermutation), 2));
                
            } catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        
        
        //FeistelFunctionResult = (FeistelFunctionResult << 4) | SBoxes[I, Row, Column];
        //get the next 4 bits -> the index in the array
        //get the LEFT OR RIGHt 4 bits from 1 byte value from the array
    }
}