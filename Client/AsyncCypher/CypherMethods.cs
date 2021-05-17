using System;
using System.IO;
using System.Numerics;
using System.Windows;
using SardorRsa;

namespace Task_8.AsyncCypher
{
    public class CypherMethods
    {
        public static TaskProperties encryptBlock(TaskProperties props, byte[] key, int keySize, string keyFilePath)
        {
            byte[] encryptedData = null;
            
            
            //MessageBox.Show("Data to be encrypted: " + new ASCIIEncoding().GetString(props.Data));
            encryptedData = props.CypherFramework.Encrypt(props.Data);
            //MessageBox.Show("encrypted data: " + new ASCIIEncoding().GetString(encryptedData));
            //EXPORT THE USED KEY
            // if(props.BlockNumber == props.BlocksQuantity - 1)
            //     rijndaelFramework.ExportKey(keyFilePath);
            return new TaskProperties(props.BlockNumber, props.BlocksQuantity,  props.CypherFramework, encryptedData);
        }
        public static TaskProperties decryptBlock(TaskProperties props, byte[] key, int keySize, string keyFilePath)
        {
            byte[] decryptedData = null;

            //MessageBox.Show("Data to be decrypted: " + new ASCIIEncoding().GetString(props.Data));
            decryptedData = props.CypherFramework.Decrypt(props.Data);
            //MessageBox.Show("decrypted data: " + new ASCIIEncoding().GetString(decryptedData));
            
            return new TaskProperties(props.BlockNumber, props.BlocksQuantity, props.CypherFramework, decryptedData);
         
        }


        public static void EncryptKey(RsaCore rsaCore, string keyFileName, string outputFilePath)
        {
            //MessageBox.Show("! " + rsaCore.NumberSize);
            FileInfo fi = new FileInfo(keyFileName);
            int blocks = (int)fi.Length / 8;
            
            if (fi.Length < 8 || fi.Length % 8 != 0)
                Console.WriteLine("Wrong key file");
            int blockSize = 8;
            
            var outputStream = File.Open(outputFilePath, FileMode.Create);
            FileStream fs = new FileStream(keyFileName, FileMode.Open, FileAccess.Read);
            
            for (int i = 0; i < blocks; i++)
            {
                int endPositionToRead = (i + 1) * blockSize;

                var plainText = TaskManager.ReadDesiredPart(fs, i * blockSize, endPositionToRead);
                
                var encryptedBigIntegers = new BigInteger[plainText.Length];
                var encryptedAllBytes = new byte[plainText.Length][];
                for (int j = 0; j < encryptedBigIntegers.Length; j++)
                {
                    encryptedBigIntegers[j] = rsaCore.EncryptOneByte(plainText[j]);
                    //Console.WriteLine("Encrypted byte: " + plainText[j] + " length is " + encryptedBigIntegers[j].ToByteArray().Length);
                    
                    encryptedAllBytes[j] = encryptedBigIntegers[j].ToByteArray();
                    if (encryptedAllBytes[j].Length != rsaCore.NumberSize)
                    {
                        //MessageBox.Show("Error : " + encryptedAllBytes[j].Length + " != " + rsaCore.NumberSize);
                        //Console.WriteLine("Found wrong data size: " + encryptedAllBytes[j].Length);
                        var temp = encryptedBigIntegers[j].ToByteArray();

                        var fixedWithPadding = new byte[rsaCore.NumberSize];
                        if (temp.Length == 1)
                        {
                            //Console.WriteLine("ONE!!!");
                            for (int k = 0; k < fixedWithPadding.Length-1; k++)
                            {
                                fixedWithPadding[k] = 0;
                            }

                            fixedWithPadding[fixedWithPadding.Length-1] = temp[0];
                            //Console.WriteLine("ready one: " + arrToStr(fixedWithPadding));
                        }
                        else
                        {
                            
                            for (int k = 1; k < temp.Length+1; k++)
                            {
                                fixedWithPadding[k] = temp[k-1];
                            }

                            fixedWithPadding[0] = 0;
                        }
                        
                        // MessageBox.Show("2: " + arrToStr(newTemp));
                        // List<byte> temp2 = new List<byte>();
                        // for (int k = 0; k < newTemp.Length; k++)
                        // {
                        //     if (newTemp[k] != 0)
                        //     {
                        //         temp2.Add(newTemp[k]);
                        //         //MessageBox.Show("ADD "+ newTemp[k])
                        //     }
                        //         
                        // }
                        // MessageBox.Show("3: " + arrToStr(temp2.ToArray()));
                        // MessageBox.Show("Need to be eq: " + new ASCIIEncoding().GetString(temp2.ToArray()) + " | " +
                        //                 new ASCIIEncoding().GetString(temp));
                        encryptedAllBytes[j] = fixedWithPadding;
                        //encrypted[j] = new BigInteger(newTemp);
                        
                    }
                }
                
                
                foreach (var encryptedByte in encryptedAllBytes)
                {
                    try
                    {
                        outputStream.Write(encryptedByte, 0, encryptedByte.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                
            }
            outputStream.Close();
            fs.Close();
        }
        public static void DecryptKey(RsaCore rsaCore, string encryptedKeyFileName, string outputFilePath)
        {
            
            var outputStream = File.Open(outputFilePath, FileMode.Create);
            FileStream fs = new FileStream(encryptedKeyFileName, FileMode.Open, FileAccess.Read);

            var fi = new FileInfo(encryptedKeyFileName);
            var numbersQuantity = fi.Length/rsaCore.NumberSize;
            for (int i = 0; i < numbersQuantity; i++)
            {
                int endPositionToRead = (i + 1) * rsaCore.NumberSize;
                var block = TaskManager.ReadDesiredPart(fs, (i) * rsaCore.NumberSize, endPositionToRead);
                //MessageBox.Show("Dec size: " + block.Length);
                //MessageBox.Show("read: " + arrToStr(block));
                byte decryptedByte;
                if (block[0] == 0)
                {
                    //Console.WriteLine("!HMM!");
                    //Console.WriteLine(arrToStr(block));
                    bool isOne = true;
                    for (int j = 1; j < block.Length-1; j++)
                    {
                        if (block[j] != 0)
                        {
                            isOne = false;
                            break;
                        }
                    }

                    if (isOne)
                    {
                        //Console.WriteLine("ITS ONE");
                        decryptedByte = 1;
                    }
                    else
                    {
                        var temp = new byte[block.Length - 1];
                        for (int j = 0; j < block.Length - 1; j++)
                        {
                            temp[j] = block[j + 1];
                        }
                        decryptedByte = (byte)rsaCore.DecryptOneByte(new BigInteger(temp));
                    }
                    
                }
                else
                {
                    decryptedByte = (byte)rsaCore.DecryptOneByte(new BigInteger(block));
                }
                // List<byte> temp = new List<byte>();
                // for (int j = 0; j < block.Length; j++)
                // {
                //     if(block[j] != 0)
                //         temp.Add(block[j]);
                //     else
                //     {
                //         MessageBox.Show("!!");
                //     }
                // }
                
                
                //MessageBox.Show("dec: " + (decryptedByte-'0'));
                try
                {
                    //var encryptedBytes = new ASCIIEncoding().GetString(wtf);
                    outputStream.WriteByte(decryptedByte);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            outputStream.Close();
            //sr.Close();
            
        }
        
    }
}