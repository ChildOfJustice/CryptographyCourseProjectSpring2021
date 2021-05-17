using Task_4;

namespace EncryptionModes
{
    public class Ecb : EncryptionMode
    {
        public Ecb(ICypherAlgorithm _algorithm)
        {
            algorithm = _algorithm;
        }
        
        public override byte[][] EncryptAll(byte[][] allBlocks)
        {
            byte[][] result = new byte[allBlocks.Length][];
            for (int i = 0; i < allBlocks.Length; i++)
            {
                result[i] = algorithm.Encrypt(allBlocks[i]);
            }

            return result;
        }

        public override byte[][] DecryptAll(byte[][] allBlocks)
        {
            byte[][] result = new byte[allBlocks.Length][];
            for (int i = 0; i < allBlocks.Length; i++)
            {
                result[i] = algorithm.Decrypt(allBlocks[i]);
            }

            return result;
        }
    }
}