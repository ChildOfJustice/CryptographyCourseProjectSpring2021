using Task_4;

namespace EncryptionModes
{
    public class Cfb : EncryptionMode
    {
        public Cfb(byte[] iv, ICypherAlgorithm _algorithm)
        {
            IV = iv;
            algorithm = _algorithm;
        }

        private byte[][] EncryptChain(byte[] prevBlock, byte[][] allData, int blockNumber, byte[][] result)
        {
            
            var encryptedIv = algorithm.Encrypt(prevBlock);
            
            result[blockNumber] = encryptedIv;
            for (int i = 0; i < encryptedIv.Length; i++)
            {
                result[blockNumber][i] = (byte)(encryptedIv[i] ^ allData[blockNumber][i]);
            }
            
            
            if(blockNumber == allData.Length-1)
                return result;
            return EncryptChain(result[blockNumber], allData, blockNumber + 1, result);
        }

        public override byte[][] EncryptAll(byte[][] allBlocks)
        {
            byte[][] result = new byte[allBlocks.Length][];
            result = EncryptChain(IV, allBlocks, 0, result);
            return result;
        }





        public byte[] Decrypt(byte[] prevCypherBlock, byte[] currentCypherBlock)
        {
            var data = algorithm.Encrypt(prevCypherBlock);
            byte[] res = new byte[currentCypherBlock.Length];
            for (int i = 0; i < currentCypherBlock.Length; i++)
            {
                res[i] = (byte)(currentCypherBlock[i] ^ data[i]);
            }

            return res;
        }
        
        public override byte[][] DecryptAll(byte[][] allBlocks)
        {
            byte[][] result = new byte[allBlocks.Length][];
            result[0] = Decrypt(IV, allBlocks[0]);

            for (int i = 1; i < allBlocks.Length; i++)
                result[i] = Decrypt(allBlocks[i - 1], allBlocks[i]);
            
            
            return result;
        }
    }
}