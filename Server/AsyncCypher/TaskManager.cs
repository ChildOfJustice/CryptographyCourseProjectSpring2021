using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EncryptionModes;
using MagentaAlgorithm;

namespace Task_8.AsyncCypher
{
    public class TaskManager
    {
        public string outputFilePath = "result.txt";
        public string inputFilePath = "./resources/text.txt";
        public string keyFilePath = "./resources/key";
        public string ivFilePath = "./resources/IV";
        public string pubKeyFilePath = "keypub";
        public string privateKeyFilePath = "./resources/keyprivate";

        public string EncryptionMode;
        public MagentaCore Algorithm;

        public byte[] TempKey;
        private byte[] IV;
        
        private int blockSize;
        private int keySize;
        private int blocksQuantity;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        public TaskManager(string _keyFilePath, int _keySize, string _encryptionMode = "ECB")
        {
            blockSize = 16;
            keySize = _keySize;
            keyFilePath = _keyFilePath;

            //Algorithm = mc;
            Algorithm = new MagentaCore();
            Algorithm.FeistelRoundQuantity = 6;
            Algorithm.ImportKey(keyFilePath, keySize);
            

            EncryptionMode = _encryptionMode;

            //new ASCIIEncoding().GetBytes(sData);
            //MessageBox.Show("Decrypted data: " + new ASCIIEncoding().GetString(Final));
        }
        
        public void RunEncryptionProcess()
        {
            ImportCypherKey();

            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1?view=net-5.0
            CancellationToken ct = tokenSource.Token;
            
            blocksQuantity = DetermineBlocksQuantity(true);

            MessageBox.Show("Split the file into "+blocksQuantity+" blocks");

            if (EncryptionMode != "ECB")
            {
                RunEncryptionProcessInMode();
                return;
            }
                
            
            
            Task<TaskProperties>[] allTasks = new Task<TaskProperties>[blocksQuantity];
            for (int i = 0; i < blocksQuantity; i++)
            {
                TaskCompletionSource<TaskProperties> taskCompletionSource = new TaskCompletionSource<TaskProperties>();
                Task<TaskProperties> task = taskCompletionSource.Task;

                var i1 = i;
                Task.Factory.StartNew(() =>
                {
                    // Were we already canceled?
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        int endPositionToRead = (i1 + 1) * blockSize;
                        string keyFilePathToUse = keyFilePath;

                        // var tempRF = new AesCore(TempKey, generateRoundKeys:false);
                        // tempRF.RoundKeys = CypherFramework.RoundKeys;
                        
                        var temp = new MagentaCore();
                        temp.FeistelRoundQuantity = 6;
                        temp.Key = Algorithm.key;
                        
                        //temp.SetRoundKeys(((MagentaCore)algorithm).GetRoundKeys());
                        
                        if (i1 == blocksQuantity-1)
                        {
                            //encrypt the size and add it
                            byte[] block = new byte[blockSize];
                            var tempArr = new ASCIIEncoding().GetBytes(new FileInfo(inputFilePath).Length.ToString());
                            Array.Copy(tempArr, block, tempArr.Length);
                            
                            taskCompletionSource.SetResult(CypherMethods.encryptBlock(new TaskProperties(i1,
                                    blocksQuantity, temp,
                                    block), TempKey, keySize,
                                keyFilePathToUse));
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                                taskCompletionSource.SetResult(CypherMethods.encryptBlock(new TaskProperties(i1,
                                        blocksQuantity, temp,
                                        ReadDesiredPart(fs, i1 * blockSize, endPositionToRead)), TempKey, keySize,
                                    keyFilePathToUse));
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    
                }, tokenSource.Token);

                allTasks[i] = task;
            }

            if (ct.IsCancellationRequested)
            {
                // Clean up here
                ct.ThrowIfCancellationRequested();
            }
            else
            {
                using (var outputStream = File.Open(outputFilePath, FileMode.Create))
                {
                    foreach (var promise in allTasks)
                    {
                        try
                        {
                            //WILL BE LOCKED ON THIS AND WAIT UNTIL A PROMISE WILL BE RESOLVED
                            outputStream.Write(promise.Result.Data, 0, promise.Result.Data.Length);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    MessageBox.Show("Output file is " + outputFilePath);
                    outputStream.Close();
                }
            }
        }

        private void RunEncryptionProcessInMode()
        {
            importIv();
            
            
            var allBlocks = new byte[blocksQuantity][];
            for (int i = 0; i < blocksQuantity-1; i++)
            {
                int endPositionToRead = (i + 1) * blockSize;
                using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                    allBlocks[i] = ReadDesiredPart(fs, i * blockSize, endPositionToRead);
            }
            //allBlocks[blocksQuantity-1] = new ASCIIEncoding().GetBytes(new FileInfo(inputFilePath).Length.ToString());
            byte[] theLastBlock = new byte[blockSize];
            var tempArr = new ASCIIEncoding().GetBytes(new FileInfo(inputFilePath).Length.ToString());
            Array.Copy(tempArr, theLastBlock, tempArr.Length);
            allBlocks[blocksQuantity - 1] = theLastBlock;
            
            var result = new byte[blocksQuantity][];
            switch (EncryptionMode)
            {
                case "CBC":
                    Cbc mode = new Cbc(IV, Algorithm);
                    
                    result = mode.EncryptAll(allBlocks);
                    break;
                case "CFB":
                    Cfb cfbMode = new Cfb(IV, Algorithm);
                    
                    result = cfbMode.EncryptAll(allBlocks);
                    break;
                case "OFB":
                    Ofb ofbMode = new Ofb(IV, Algorithm);
                    
                    result = ofbMode.EncryptAll(allBlocks);
                    break;
            }
            
            
            //outputFilePath = "./resources/" + outputFilePath;
            using (var outputStream = File.Open(outputFilePath, FileMode.Create))
            {
                foreach (var block in result)
                {
                    try
                    {
                        outputStream.Write(block, 0, block.Length);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                MessageBox.Show("Output file is " + outputFilePath);
                outputStream.Close();
            }
        }
        private void RunEncryptionProcessInModeOptimized()
        {
            importIv();
            
            
            var encryptionModeBlocksForOneSection = 8;
            var needToShrink = false;

            var encryptionModeSectionQuantity = blocksQuantity / encryptionModeBlocksForOneSection;
            // if (encryptionModeSectionQuantity == 0)
            //     encryptionModeSectionQuantity = 1;
            //MessageBox.Show("Will split by " + encryptionModeSectionQuantity);
            if (blocksQuantity % encryptionModeBlocksForOneSection != 0)
                needToShrink = true;
            //MessageBox.Show("Will shrink " + needToShrink);
            var counter = 0;
            var previousBlock = new byte[IV.Length];
            Array.Copy(IV, previousBlock, IV.Length);

            while (counter <= encryptionModeSectionQuantity)
            {
                //MessageBox.Show("Next round: " + counter + " of " + encryptionModeSectionQuantity);
                byte[][] currAllBlocks = null;
                if (counter < encryptionModeSectionQuantity)
                {
                    currAllBlocks = new byte[encryptionModeBlocksForOneSection][];
                    for (int i = 0; i < encryptionModeBlocksForOneSection; i++)
                    {
                        int endPositionToRead = (counter*encryptionModeBlocksForOneSection + (i + 1)) * blockSize;
                        using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                            currAllBlocks[i] = ReadDesiredPart(fs, (counter*encryptionModeBlocksForOneSection + i) * blockSize, endPositionToRead);
                        //MessageBox.Show("read block: " + new ASCIIEncoding().GetString(currAllBlocks[i]));
                    }
                }


                var prevEncryptionModeBlocksForOneSection = encryptionModeBlocksForOneSection;
                if (counter == encryptionModeSectionQuantity && needToShrink)
                {
                    
                    encryptionModeBlocksForOneSection = blocksQuantity % encryptionModeBlocksForOneSection;
                    //MessageBox.Show("Shrinked: Will use in the last read: " + encryptionModeBlocksForOneSection);
                }
                if(counter == encryptionModeSectionQuantity)
                {
                    
                    currAllBlocks = new byte[encryptionModeBlocksForOneSection][];
                    for (int i = 0; i < encryptionModeBlocksForOneSection-1; i++)
                    {
                        int endPositionToRead = (counter*prevEncryptionModeBlocksForOneSection + (i + 1)) * blockSize;
                        using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                            currAllBlocks[i] = ReadDesiredPart(fs, (counter*prevEncryptionModeBlocksForOneSection + i) * blockSize, endPositionToRead);
                        //MessageBox.Show("read block in the last section: " + new ASCIIEncoding().GetString(currAllBlocks[i]));
                    }
                    
                    byte[] theLastBlock = new byte[blockSize];
                    var tempArr = new ASCIIEncoding().GetBytes(new FileInfo(inputFilePath).Length.ToString());
                    Array.Copy(tempArr, theLastBlock, tempArr.Length);
                    
                    currAllBlocks[currAllBlocks.Length-1] = theLastBlock;
                    //MessageBox.Show("final block is: " + new ASCIIEncoding().GetString(currAllBlocks[currAllBlocks.Length-1]));
                
                }


                var result = new byte[currAllBlocks.Length][];
                switch (EncryptionMode)
                {
                    case "CBC":
                        Cbc mode = new Cbc(previousBlock, Algorithm);
                        
                        result = mode.EncryptAll(currAllBlocks);
                        //MessageBox.Show("All blocks in this section is encrypted: " + result.Length);
                        break;
                    case "CFB":
                        Cfb cfbMode = new Cfb(previousBlock, Algorithm);
                        
                        result = cfbMode.EncryptAll(currAllBlocks);
                        break;
                    case "OFB":
                        Ofb ofbMode = new Ofb(previousBlock, Algorithm);
                        
                        result = ofbMode.EncryptAll(currAllBlocks);
                        break;
                }
                
                using (var outputStream = File.Open(outputFilePath, FileMode.Append))
                {
                    foreach (var block in result)
                    {
                        try
                        {
                            //MessageBox.Show("Writing");
                            outputStream.Write(block, 0, block.Length);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    outputStream.Close();
                }
                
                Array.Copy(currAllBlocks[currAllBlocks.Length-1], previousBlock, currAllBlocks[currAllBlocks.Length-1].Length);
                counter++;
                
            }
            
            MessageBox.Show("Output file is " + outputFilePath);
        }
        
        
        

        public void RunDecryptionProcess()
        {
            ImportCypherKey();

            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1?view=net-5.0
            CancellationToken ct = tokenSource.Token;

            blocksQuantity = DetermineBlocksQuantity(false);

            
            if (EncryptionMode != "ECB")
            {
                RunDecryptionProcessInMode();
                return;
            }
            
            Task<TaskProperties>[] allTasks = new Task<TaskProperties>[blocksQuantity];
            for (int i = 0; i < blocksQuantity; i++)
            {
                TaskCompletionSource<TaskProperties> taskCompletionSource = new TaskCompletionSource<TaskProperties>();
                Task<TaskProperties> task = taskCompletionSource.Task;

                var i1 = i;
                Task.Factory.StartNew(() =>
                {
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        int endPositionToRead = (i1 + 1) * blockSize;
                        string keyFilePathToUse = keyFilePath;
                        
                      
                        var temp = new MagentaCore();
                        temp.FeistelRoundQuantity = 6;
                        temp.Key = Algorithm.key;
                        
                        using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                        {
                            var result = CypherMethods.decryptBlock(new TaskProperties(i1, blocksQuantity, temp,
                                ReadDesiredPart(fs, i1 * blockSize, endPositionToRead)), TempKey, keySize, keyFilePathToUse);
                            taskCompletionSource.SetResult(result);
                        }
                    }catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                });
                
                allTasks[i] = task;
            }

            if (ct.IsCancellationRequested)
            {
                // Clean up here
                ct.ThrowIfCancellationRequested();
            }
            else
            {
                int blockNumber = 0;
                int actualFileSize = 0;
                int fullArrSize = 0;
                byte[] plainTextBytes = null;
                using (BinaryWriter binWriter = new BinaryWriter(new MemoryStream()))
                {
                    foreach (var promise in allTasks)
                    {
                       
                        try
                        {
                            if (blockNumber == blocksQuantity - 1)
                            {
                                MessageBox.Show("blocknumber : " +blockNumber+" blocks: " + blocksQuantity + "file SIZE IS: " + new ASCIIEncoding().GetString(promise.Result.Data));
                                actualFileSize = Int32.Parse(new ASCIIEncoding().GetString(promise.Result.Data));
                            }
                            else
                            {
                                //WILL BE LOCKED ON THIS AND WAIT UNTIL A PROMISE WILL BE RESOLVED
                                binWriter.Write(promise.Result.Data, 0, promise.Result.Data.Length);
                                fullArrSize = promise.Result.Data.Length;
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        blockNumber++;
                    }
                    
                    BinaryReader binReader =
                        new BinaryReader(binWriter.BaseStream);
                    // Set Position to the beginning of the stream.
                    binReader.BaseStream.Position = 0;
                    
                    // if (algorithm != CypherAlgorithm.RSA)
                    // {
                    plainTextBytes = binReader.ReadBytes(actualFileSize);
                    //var count = memStream.Read(plainTextBytes, 0, actualFileSize);
                    // }
                    // else
                    // {
                        //plainTextBytes = binReader.ReadBytes(fullArrSize);
                    //}
                    
                }
                
                using (var outputStream = File.Open(outputFilePath, FileMode.Create))
                {
                    MessageBox.Show("Output file is " + outputFilePath);
                    outputStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    outputStream.Close();
                }
            }
            
        }
        private void RunDecryptionProcessInMode()
        {
            importIv();
            
            
            var allBlocks = new byte[blocksQuantity][];
            for (int i = 0; i < blocksQuantity; i++)
            {
                int endPositionToRead = (i + 1) * blockSize;
                using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                    allBlocks[i] = ReadDesiredPart(fs, i * blockSize, endPositionToRead);
            }
            
            var result = new byte[blocksQuantity][];
            switch (EncryptionMode)
            {
                case "CBC":
                    Cbc cbcMode = new Cbc(IV, Algorithm);
                    
                    result = cbcMode.DecryptAll(allBlocks);
                    break;
                case "CFB":
                    Cfb cfbMode = new Cfb(IV, Algorithm);
                    
                    result = cfbMode.DecryptAll(allBlocks);
                    break;
                case "OFB":
                    Ofb ofbMode = new Ofb(IV, Algorithm);
                    
                    result = ofbMode.DecryptAll(allBlocks);
                    break;
            }
            
            
            // MessageBox.Show(new ASCIIEncoding().GetString(result[0]));
            // MessageBox.Show(new ASCIIEncoding().GetString(result[1]));
            
            //outputFilePath = "./resources/" + outputFilePath;
            using (var outputStream = File.Open(outputFilePath, FileMode.Create))
            {
                for (int i = 0; i < result.Length-1; i++)
                {
                    
                    try
                    {
                        if(i == result.Length - 2)
                        {
                            outputStream.Write(result[i], 0, Int32.Parse(new ASCIIEncoding().GetString(result[result.Length-1])) - blockSize*(blocksQuantity-2) );
                            
                        }
                        else
                            outputStream.Write(result[i], 0, result[i].Length);
                    }
                    catch (Exception e)
                    {
                        //TODO : MessageBox.Show(e.Message);
                    }
                }
                
                MessageBox.Show("Output file is " + outputFilePath);
                outputStream.Close();
            }
        }
         private void RunDecryptionProcessInModeOptimized()
        {
            importIv();

            var encryptionModeBlocksForOneSection = 8;
            var needToShrink = false;

            var encryptionModeSectionQuantity = blocksQuantity / encryptionModeBlocksForOneSection;
            // if (encryptionModeSectionQuantity == 0)
            //     encryptionModeSectionQuantity = 1;
            //MessageBox.Show("Will split by " + encryptionModeSectionQuantity);
            if (blocksQuantity % encryptionModeBlocksForOneSection != 0)
                needToShrink = true;
            //MessageBox.Show("Will shrink " + needToShrink);
            var counter = 0;
            var previousBlock = new byte[IV.Length];
            Array.Copy(IV, previousBlock, IV.Length);

            while (counter <= encryptionModeSectionQuantity)
            {
                //MessageBox.Show("Next round: " + counter + " of " + encryptionModeSectionQuantity);
                byte[][] currAllBlocks = null;
                if (counter < encryptionModeSectionQuantity)
                {
                    currAllBlocks = new byte[encryptionModeBlocksForOneSection][];
                    for (int i = 0; i < encryptionModeBlocksForOneSection; i++)
                    {
                        int endPositionToRead = (counter*encryptionModeBlocksForOneSection + (i + 1)) * blockSize;
                        using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                            currAllBlocks[i] = ReadDesiredPart(fs, (counter*encryptionModeBlocksForOneSection + i) * blockSize, endPositionToRead);
                        //MessageBox.Show("read block: " + new ASCIIEncoding().GetString(currAllBlocks[i]));
                    }
                }

                var prevEncryptionModeBlocksForOneSection = encryptionModeBlocksForOneSection;
                if (counter == encryptionModeSectionQuantity && needToShrink)
                {
                    encryptionModeBlocksForOneSection = blocksQuantity % encryptionModeBlocksForOneSection;
                    //MessageBox.Show("Will use in the last read: " + encryptionModeBlocksForOneSection);
                }
                if(counter == encryptionModeSectionQuantity){
                    
                    currAllBlocks = new byte[encryptionModeBlocksForOneSection][];
                    for (int i = 0; i < encryptionModeBlocksForOneSection; i++)
                    {
                        int endPositionToRead = (counter*prevEncryptionModeBlocksForOneSection + (i + 1)) * blockSize;
                        using (FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                            currAllBlocks[i] = ReadDesiredPart(fs,  (counter*prevEncryptionModeBlocksForOneSection + i) * blockSize, endPositionToRead);
                        //MessageBox.Show("read block in the last section: " + new ASCIIEncoding().GetString(currAllBlocks[i]));
                    }
                    
                }


                var result = new byte[currAllBlocks.Length][];
                switch (EncryptionMode)
                {
                    case "CBC":
                        Cbc mode = new Cbc(previousBlock, Algorithm);
                        
                        result = mode.DecryptAll(currAllBlocks);
                        //MessageBox.Show("All blocks in this section are decrypted: " + result.Length);
                        break;
                    case "CFB":
                        Cfb cfbMode = new Cfb(previousBlock, Algorithm);
                        
                        result = cfbMode.DecryptAll(currAllBlocks);
                        break;
                    case "OFB":
                        Ofb ofbMode = new Ofb(previousBlock, Algorithm);
                        
                        result = ofbMode.DecryptAll(currAllBlocks);
                        break;
                }

                // foreach (var block in result)
                // {
                //     MessageBox.Show("Block: " + new ASCIIEncoding().GetString(block));
                // }

                if (counter == encryptionModeSectionQuantity)
                {
                    //the last section
                    using (var outputStream = File.Open(outputFilePath, FileMode.Append))
                    {
                        for (int i = 0; i < result.Length-1; i++)
                        {
                    
                            try
                            {
                                if(i == result.Length - 2)
                                {
                                    //MessageBox.Show("final block is: " + new ASCIIEncoding().GetString(result[i]));
                                    //MessageBox.Show("the size is: " + new ASCIIEncoding().GetString(result[result.Length-1]));
                                    //MessageBox.Show("F: " +(Int32.Parse(new ASCIIEncoding().GetString(result[result.Length-1])) - blockSize*(blocksQuantity-2)) );
                                    
                                    outputStream.Write(result[i], 0, Int32.Parse(new ASCIIEncoding().GetString(result[result.Length-1])) - blockSize*(blocksQuantity-2) );
                                    
                                }
                                else
                                {
                                    //MessageBox.Show("write in the last section: " + new ASCIIEncoding().GetString(result[i]));
                                    outputStream.Write(result[i], 0, result[i].Length);
                                }
                                    
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }
                
                        
                        outputStream.Close();
                    }
                }
                else
                {
                    using (var outputStream = File.Open(outputFilePath, FileMode.Append))
                    {
                        foreach (var block in result)
                        {
                            try
                            {
                                //MessageBox.Show("Writing");
                                //MessageBox.Show("block is: " + new ASCIIEncoding().GetString(block));
                                outputStream.Write(block, 0, block.Length);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }
                        outputStream.Close();
                    }
                }
                
                //previousBlock = currAllBlocks[currAllBlocks.Length-1];
                Array.Copy(currAllBlocks[currAllBlocks.Length-1], previousBlock, currAllBlocks[currAllBlocks.Length-1].Length);
                counter++;
            }
            MessageBox.Show("Output file is " + outputFilePath);
        }
        
        
        
        
        
        private void ImportCypherKey()
        {
            MessageBox.Show("Importing secret key...");
            var temp = new byte[keySize];
            using (var inputStream = File.OpenRead(keyFilePath))
                inputStream.Read(temp, 0, keySize);

            TempKey = temp;
        }
        private void importIv()
        {
            // MessageBox.Show("Importing secret key...");
            var temp = new byte[blockSize];
            using (var inputStream = File.OpenRead(ivFilePath))
                inputStream.Read(temp, 0, blockSize);
            IV = temp;
        }

        private int DetermineBlocksQuantity(bool encrypt)
        {
            
            FileInfo fi = new FileInfo(inputFilePath);
            //MessageBox.Show("SIZE IS " + fi.Length + " blocks will be " + (int) (fi.Length / blockSize + 1));
            int res;
            if (fi.Length % blockSize == 0)
                res = (int) (fi.Length / blockSize);
            else
                res = (int) (fi.Length / blockSize + 1);
            if (encrypt)
                res++; // + 1 more for the file size block
            return res; 
        }

        public static byte[] ReadDesiredPart(FileStream fs, int startPosition, int endPosition) {
            byte[] buffer = new byte[endPosition - startPosition];

            int arrayOffset = 0;

            //lock (fsLock) {
                fs.Seek(startPosition, SeekOrigin.Begin);

                int numBytesRead = fs.Read(buffer, arrayOffset , endPosition - startPosition);

                //  Typically used if you're in a loop, reading blocks at a time
                arrayOffset += numBytesRead;
            //}

            return buffer;
        }


        public void Cancel()
        {
            tokenSource.Cancel();
        }
    }
}