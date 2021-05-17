using System;
using Task_4;

namespace EncryptionModes
{
    public class Ofb : EncryptionMode
    {
        //volatile
        public byte[] CurrIv;
        
        public Ofb(byte[] iv, ICypherAlgorithm _algorithm)
        {
            IV = iv;
            algorithm = _algorithm;
            //CurrIv = new byte[IV.Length];
        }


        private void EncryptChain(byte[] prevIv, byte[][] allData, int blockNumber, byte[][] result)
        {
            var tempIv = algorithm.Encrypt(prevIv);
            //Array.Copy(tempIv, CurrIv, tempIv.Length);
        
            result[blockNumber] = new byte[allData[blockNumber].Length];
            for (int i = 0; i < result[blockNumber].Length; i++)
            {
                result[blockNumber][i] = (byte)(tempIv[i] ^ allData[blockNumber][i]);
            }
        
            if(blockNumber == allData.Length-1)
                return;
            EncryptChain(tempIv, allData, blockNumber + 1, result);
        }
        
        public override byte[][] EncryptAll(byte[][] allBlocks)
        {
            byte[][] result = new byte[allBlocks.Length][];
            EncryptChain(IV, allBlocks, 0, result);
            return result;
        }
        
        
        // private byte[] EncryptBlock(byte[] data)
        // {
        //     var tempIv = algorithm.Encrypt(CurrIv);
        //
        //     for (int i = 0; i < CurrIv.Length; i++)
        //     {
        //         CurrIv[i] = tempIv[i];
        //     }
        //     
        //     var result = new byte[data.Length];
        //     for (int i = 0; i < result.Length; i++)
        //     {
        //         result[i] = (byte)(data[i] ^ tempIv[i]);
        //     }
        //     
        //     return result;
        // }
        //
        // public byte[][] EncryptAll(byte[][] allBlocks)
        // {
        //     CurrIv = IV;
        //
        //     byte[][] result = new byte[allBlocks.Length][];
        //     for (int i = 0; i < allBlocks.Length; i++)
        //     {
        //         result[i] = EncryptBlock(allBlocks[i]);
        //     }
        //     
        //     return result;
        // }
        
        
        
        
        
        
        
        
        
        
        
        
        
        private byte[] DecryptBlock(byte[] data)
        {
            var tempIv = algorithm.Encrypt(CurrIv);
        
            for (int i = 0; i < CurrIv.Length; i++)
            {
                CurrIv[i] = tempIv[i];
            }
            
            var result = new byte[data.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(data[i] ^ tempIv[i]);
            }
            
            return result;
        }
        
        public override byte[][] DecryptAll(byte[][] allBlocks)
        {
            CurrIv = IV;
        
            byte[][] result = new byte[allBlocks.Length][];
            for (int i = 0; i < allBlocks.Length; i++)
            {
                result[i] = DecryptBlock(allBlocks[i]);
            }
            
            return result;
        }
        // private void DecryptChain(byte[] prevIv, byte[][] allData, int blockNumber, byte[][] result)
        // {
        //     var tempIv = algorithm.Encrypt(prevIv);
        //
        //     result[blockNumber] = new byte[allData[blockNumber].Length];
        //     for (int i = 0; i < result[blockNumber].Length; i++)
        //     {
        //         result[blockNumber][i] = (byte)(tempIv[i] ^ allData[blockNumber][i]);
        //     }
        //
        //     if(blockNumber == allData.Length-1)
        //         return;
        //     DecryptChain(tempIv, allData, blockNumber + 1, result);
        // }
        //
        // public byte[][] DecryptAll(byte[][] allBlocks)
        // {
        //     byte[][] result = new byte[allBlocks.Length][];
        //     DecryptChain(IV, allBlocks, 0, result);
        //     return result;
        // }
       
    }
}