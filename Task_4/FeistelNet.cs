using System;
using System.Numerics;

namespace Task_4
{
    public abstract class FeistelNet : ICypherAlgorithm
    {
        protected ulong[] RoundKeys;
        //public abstract ulong[] generateRoundKeys();//RoundKeys = new ulong[16];
        
        
        //const ulong Mask32Bit = ((ulong)1 << 32) - 1;

        public int FeistelRoundQuantity;
        
        protected int DataSize;
        protected ulong Data;
        protected ulong R;
        protected ulong L;
	
        private int byteSize = 8;

        public byte[] Encrypt(byte[] dataBytes)
        {
	        ProcessDataBytes(dataBytes);
	        Hook1();//InitialPermutation
	        DoTheJob(true);
	        Hook2();//FinalPermutation


	        var leftByteArray = new byte[DataSize / 2];
	        var rightByteArray = new byte[DataSize / 2];


	        if (DataSize == 8)
	        {
		        leftByteArray = BitConverter.GetBytes((uint)R); 
		        rightByteArray = BitConverter.GetBytes((uint)L); 
	        } else if (DataSize == 16)
	        {
		        leftByteArray = BitConverter.GetBytes(R); 
		        rightByteArray = BitConverter.GetBytes(L);
	        }
	        else
	        {
		        throw new ArgumentException("Wrong data block size in the end of the cipher function");
	        }
	        
	        var resultByteArray = new byte[DataSize];
	        for (int i = 0; i < DataSize/2; i++)
	        {
		        resultByteArray[i] = leftByteArray[i];
	        }
	        for (int i = DataSize/2; i < DataSize; i++)
	        {
		        resultByteArray[i] = rightByteArray[i-DataSize/2];
	        }
	   //      foreach (var VARIABLE in leftByteArray)
	   //      {
				// Console.Write(VARIABLE + " ");
	   //      }
	   //      foreach (var VARIABLE in rightByteArray)
	   //      {
		  //       Console.Write(VARIABLE + " ");
	   //      }
	   //
	   //      Console.WriteLine("||||");
	        
	        
	        //Data = ((ulong) L << (DataSize*byteSize)/2) | R;
	        // foreach (var VARIABLE in BitConverter.GetBytes(Data))
	        // {
		       //  Console.Write(VARIABLE + " ");
	        // }

	        //Console.WriteLine("!!!");
	        return resultByteArray;

	        //return BitConverter.GetBytes(Permute(((ulong)L << 32) | R, FinalPermutation));
	        Data = ((ulong) L << (DataSize*byteSize)/2) | R;
	        //return Data.ToByteArray();
	        //return BigIntToByteArray(Data, DataSize);


	        return BitConverter.GetBytes(Data);
        }
        public byte[] Decrypt(byte[] dataBytes)
        {
	        ProcessDataBytes(dataBytes);
	        Hook1();//InitialPermutation
	        DoTheJob(false);
	        Hook2();//FinalPermutation


	        var leftByteArray = new byte[DataSize / 2];
	        var rightByteArray = new byte[DataSize / 2];


	        if (DataSize == 8)
	        {
		        leftByteArray = BitConverter.GetBytes((uint)R); 
		        rightByteArray = BitConverter.GetBytes((uint)L); 
	        } else if (DataSize == 16)
	        {
		        leftByteArray = BitConverter.GetBytes(R); 
		        rightByteArray = BitConverter.GetBytes(L);
	        }
	        else
	        {
		        throw new ArgumentException("Wrong data block size in the end of the cipher function");
	        }
	        
	        var resultByteArray = new byte[DataSize];
	        for (int i = 0; i < DataSize/2; i++)
	        {
		        resultByteArray[i] = leftByteArray[i];
	        }
	        for (int i = DataSize/2; i < DataSize; i++)
	        {
		        resultByteArray[i] = rightByteArray[i-DataSize/2];
	        }
	        
	        return resultByteArray;
        }

        protected void ProcessDataBytes(byte[] DataBytes)
        {
	        DataSize = DataBytes.Length;
	        
	        if (DataBytes.Length == 8)
	        {
		        
		        
		        R = BitConverter.ToUInt32(DataBytes, 0);
		        L = BitConverter.ToUInt32(DataBytes, DataSize/2);
		        
		        
		        // var unsignedDataBytes = new byte[DataSize + 1];
		        // unsignedDataBytes[DataSize] = 00;
		        // for (int i = 0; i < DataSize; i++)
		        // {
			       //  unsignedDataBytes[i] = DataBytes[i];
		        // }
		        // Data = new BigInteger(unsignedDataBytes);
		        //Data = BitConverter.ToUInt64(DataBytes, 0);
	        }
	        else if(DataSize == 16)
	        {
		        R = BitConverter.ToUInt64(DataBytes, 0);
		        L = BitConverter.ToUInt64(DataBytes, DataSize/2);
		        // Data = BitConverter.ToUInt64(DataBytes, 0);
		        // Data <<= 64;
		        // Data |= BitConverter.ToUInt64(DataBytes, 8);
		        // foreach (var VARIABLE in BigIntToByteArray(Data, DataSize))
		        // {
			       //  Console.Write(" " + VARIABLE);
		        // }
	        }
	        else
	        {
		        throw new ArgumentException("Wrong data block size");
	        }
        }

        protected void DoTheJob(bool encrypt)
        {
	        //ulong mask = ((ulong)1 << (DataSize*byteSize)/2) - 1;
	        //L = (ulong)((Data >> (DataSize*byteSize)/2) & mask);
	        //R = (ulong) (Data & mask);
	        
	        ulong PreviousL = L;
	        ulong PreviousR = R;
	        for (byte Round = 0; Round < FeistelRoundQuantity; Round++)
	        {
		        if (encrypt)
		        {
			        L = PreviousR;
			        R = PreviousL ^ AbstractFeistelFunction(PreviousR, RoundKeys[Round]);
			        PreviousL = L;
			        PreviousR = R;
		        }
		        else
		        {
			        R = PreviousL;
			        L = PreviousR ^ AbstractFeistelFunction(PreviousL, RoundKeys[FeistelRoundQuantity-1 - Round]);
			        PreviousR = R;
			        PreviousL = L;
		        }
	        }
	        //return BitConverter.GetBytes(Permute(((ulong)L << 32) | R, FinalPermutation));
	        //return BitConverter.GetBytes((((ulong) L << (DataSize*byteSize)/2) | R);
	        //Data = ((ulong) L << (DataSize*byteSize)/2) | R;
        }

        protected abstract ulong AbstractFeistelFunction(ulong R, ulong RoundKey);
        
        protected virtual void Hook1() { }

        protected virtual void Hook2() { }
        
        public abstract  byte[] Key
        {
	        set;
        }


        // public byte[] BigIntToByteArray(BigInteger bigInteger, int arraySize)
        // {
	       //  Console.WriteLine("!!!" + arraySize);
	       //  
	       //  var res = new byte[arraySize];
	       //  var dataBytes = bigInteger.ToByteArray();
	       //  Console.WriteLine("!!!" + dataBytes.Length);
	       //  for (int i = 0; i < dataBytes.Length-1; i++)
	       //  {
		      //   res[i] = dataBytes[i];
	       //  }
        //
	       //  return res;
        // }
    }
}