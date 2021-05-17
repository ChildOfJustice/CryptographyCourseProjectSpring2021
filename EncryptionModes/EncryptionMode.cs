using Task_4;

namespace EncryptionModes
{
    public abstract class EncryptionMode
    {
        protected byte[] IV;
        protected byte[] Key;

        protected ICypherAlgorithm algorithm;

        protected int blockSize;
        
        public abstract byte[][] EncryptAll(byte[][] allBlocks);
        public abstract byte[][] DecryptAll(byte[][] allBlocks);
    }
}