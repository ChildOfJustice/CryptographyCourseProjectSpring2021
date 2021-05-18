using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CourseProjectCryptography2021.Spinner;
using SardorRsa;
using Server.Web;
using Task_8;
using Task_8.AsyncCypher;

namespace CourseProjectCryptography2021
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel _mainWindowViewModel;
        private RsaCore rsaCore;
        
        
        private string clientLocation = "C:\\Users\\Administrator\\Desktop\\READY\\Cryptography\\CryptographyCourseProjectSpring2021\\Client\\bin\\Debug\\resources\\";
        
        public MainWindow()
        {
            InitializeComponent();
            _mainWindowViewModel = new MainWindowViewModel();
            DataContext = _mainWindowViewModel;
        }
        
        
        
        private void ComboBoxMagentaKeySizeSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            TextBlock selectedItem = (TextBlock)comboBox.SelectedItem;
            _mainWindowViewModel.MagentaKeySize = Int32.Parse(selectedItem.Text)/8;
        }
        private void ComboBoxEncryptionModeSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            TextBlock selectedItem = (TextBlock)comboBox.SelectedItem;
            _mainWindowViewModel.EncryptionMode = selectedItem.Text;
        }
        private void OnStartSessionButtonClick(object sender, RoutedEventArgs e)
        {
            //get key path to export them
            var pubKeyFileName = OutPutPubKeyFilePathHolder.Text;
            var privateKeyFileName = OutPutPrivateKeyFilePathHolder.Text;
            // get RSA key size
            int rsaKeySize = 512;
            try
            {
                rsaKeySize = Int32.Parse(RsaKeySizeHolder.Text);
                if (rsaKeySize < 512)
                    throw new ArgumentException("Bad RSA key size");
            }
            catch (Exception exception)
            {
                MessageBox.Show("Wrong RSA key size, will be used <512>");
                rsaKeySize = 512;
            }
            Task.Run(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        setContent();
                    });

                    rsaCore = new RsaCore(rsaKeySize);
                    //export keys
                    rsaCore.ExportPubKey(pubKeyFileName);
                    rsaCore.ExportPrivateKey(privateKeyFileName);
                    MessageBox.Show("Public key is now exported to the Client app");
                    MoveFile(pubKeyFileName, pubKeyFileName);
                    // try
                    // {
                    //     File.Move(pubKeyFileName, clientLocation + getFileNameOnly(pubKeyFileName));
                    // }
                    // catch
                    // {
                    //     File.Replace(pubKeyFileName, clientLocation+getFileNameOnly(pubKeyFileName), null);    
                    // }
                    
                    
                    ///////////////////////////////FOR TESTS:
                    // try {  
                    //     
                    //
                    //     string fileName = "./resources/mainMenu.jpg";
                    //     string requestURL = "http://localhost:8080/storage/files?file="+fileName;
                    //     
                    //     WECBlient wc = new WECBlient();  
                    //     // byte[] bytes = wc.DownloadData(fileName); // You need to do this download if your file is on any other server otherwise you can convert that file directly to bytes  
                    //     byte[] bytes = File.ReadAllBytes(fileName);
                    //     Dictionary < string, object > postParameters = new Dictionary < string, object > ();  
                    //     // Add your parameters here  
                    //     postParameters.Add("fileToUpload", new FormUpload.FileParameter(bytes, Path.GetFileName(fileName), "image/png"));  
                    //     string userAgent = "Someone";  
                    //     HttpWebResponse webResponse = FormUpload.MultipartFormPost(requestURL, userAgent, postParameters, "file", fileName);  
                    //     // Process response  
                    //     StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());  
                    //     var returnResponseText = responseReader.ReadToEnd();  
                    //     webResponse.Close();  
                    // } catch (Exception exp) {}  
                    //
                    
                    
                    
                    // // Create a request using a URL that can receive a post.
                    // WebRequest request = WebRequest.Create("http://localhost:8080/storage/files");
                    // // Set the Method property of the request to POST.
                    // request.Method = "POST";
                    //
                    // // Create POST data and convert it to a byte array.
                    // string postData = "This is a test that posts this string to a Web server.";
                    // byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    //
                    // // Set the ContentType property of the WebRequest.
                    // request.ContentType = "multipart/form-data";
                    // // Set the ContentLength property of the WebRequest.
                    // request.ContentLength = byteArray.Length;
                    //
                    // // Get the request stream.
                    // Stream dataStream = request.GetRequestStream();
                    // // Write the data to the request stream.
                    // dataStream.Write(byteArray, 0, byteArray.Length);
                    // // Close the Stream object.
                    // dataStream.Close();
                    //
                    // // Get the response.
                    // WebResponse response = request.GetResponse();
                    // // Display the status.
                    // Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                    //
                    // // Get the stream containing content returned by the server.
                    // // The using block ensures the stream is automatically closed.
                    // using (dataStream = response.GetResponseStream())
                    // {
                    //     // Open the stream using a StreamReader for easy access.
                    //     StreamReader reader = new StreamReader(dataStream);
                    //     // Read the content.
                    //     string responseFromServer = reader.ReadToEnd();
                    //     // Display the content.
                    //     Console.WriteLine(responseFromServer);
                    // }
                    //
                    // // Close the response.
                    // response.Close();
                    
                    
//                     byte[] smth = {1, 2};
//                     HttpWebRequest myPostRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/storage/files");
//                     myPostRequest.Method = "POST";
//                     myPostRequest.ContentType = "multipart/form-data";
//                     myPostRequest.ContentLength = smth.Length;
//                     Stream dataStream = myPostRequest.GetRequestStream();
//                     dataStream.Write(smth, 0, smth.Length);
//                     dataStream.Close();
//                     WebResponse response = myPostRequest.GetResponse();
//                     MessageBox.Show(((HttpWebResponse)response).StatusDescription);
// //Write post data to stream
//                     StreamWriter myWriter = new StreamWriter(myPostRequest.GetRequestStream());
//                     myWriter.Write(myWriter);
//                     myWriter.Close();
                    
         
//                     HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/storage/files");
//                     myRequest.Method = "GET";
//                     HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
//                     //MessageBox.Show(myResponse.StatusCode.ToString());
//                     
//                     // Open the stream using a StreamReader for easy access.
//                     StreamReader myReader = new StreamReader(myResponse.GetResponseStream());
// // Read the content.
//                     string output = myReader.ReadToEnd();
//                     MessageBox.Show(output);
                    // _mainWindowViewModel.SymmetricKeyFile = "./resources/key";
                    // CypherMethods.EncryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile, _mainWindowViewModel.SymmetricKeyFile+"Encrypted");
                    //
                    // rsaCore = new RsaCore(rsaKeySize,generateKeys:false);
                    // rsaCore.ImportPubKey(pubKeyFileName);
                    // rsaCore.ImportPrivateKey(privateKeyFileName);
                    //
                    // CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile+"Encrypted", _mainWindowViewModel.SymmetricKeyFile+"Decrypted");
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        removeContent();
                    });
                }
                catch (Exception exception)
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        removeContent();
                    });
                    
                    MessageBox.Show(exception.Message);
                }
            });
        }
        private void OnReceiveFileButtonClick(object sender, RoutedEventArgs e)
        {
            var outputFileName = OutPutFilePathHolder.Text;
            var privateKeyFileName = _mainWindowViewModel.PrivateKeyFile;

            // get RSA key size
            int rsaKeySize = 512;
            try
            {
                rsaKeySize = Int32.Parse(RsaKeySizeHolder.Text);
                if (rsaKeySize < 512)
                    throw new ArgumentException("Bad RSA key size");
            }
            catch (Exception exception)
            {
                MessageBox.Show("Wrong RSA key size, will be used <512>");
                rsaKeySize = 512;
            }
            
            Task.Run(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        setContent();
                    });

                    //decrypt symmetric key with private RSA key
                    rsaCore = new RsaCore(rsaKeySize, generateKeys:false);
                    rsaCore.ImportPrivateKey(privateKeyFileName);
                    
                    var decryptedKeyFilePath = "./resources/DecryptedSymmetricKey";
                    CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile, decryptedKeyFilePath);
                    
                    var decryptedIvFilePath = "./resources/DecryptedIv";
                    if (_mainWindowViewModel.EncryptionMode != "ECB")
                        CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.IvFilePath, decryptedIvFilePath);


                    // var decryptedKeyFilePath = "./resources/key";
                    // var decryptedIvFilePath = "./resources/IV";
                    //
                    //decrypt file with MAGENTA
                    _mainWindowViewModel.MainTaskManager = new TaskManager(decryptedKeyFilePath,_mainWindowViewModel.MagentaKeySize, _mainWindowViewModel.EncryptionMode);
                    
                    _mainWindowViewModel.MainTaskManager.keyFilePath = decryptedKeyFilePath;
                    if (_mainWindowViewModel.EncryptionMode != "ECB")
                        _mainWindowViewModel.MainTaskManager.ivFilePath = decryptedIvFilePath;
                    _mainWindowViewModel.MainTaskManager.inputFilePath = _mainWindowViewModel.DataFile;
                    _mainWindowViewModel.MainTaskManager.outputFilePath = outputFileName;
                    
                    _mainWindowViewModel.MainTaskManager.RunDecryptionProcess();

                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        removeContent();
                    });
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        removeContent();
                    });
                }
            });
        }
        private void OnChooseSymmetricKeyFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _mainWindowViewModel.SymmetricKeyFile = filename;
            }
        }
        private void OnChoosePrivateKeyFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _mainWindowViewModel.PrivateKeyFile = filename;
            }
        }
        private void OnChooseIvFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _mainWindowViewModel.IvFilePath = filename;
            }
        }
        private void OnChooseDataFileButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _mainWindowViewModel.DataFile = filename;
            }
        }
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.MainTaskManager.Cancel();
            removeContent();
        }

        private void setContent()
        {
            var pb = new CircularProgressBar();
            //pb.Message = "Wait...";
            ProgressBar.Content = pb;
        }
        
        private void removeContent()
        {
            var img = this.FindResource("LandingImage") as Image;
            ProgressBar.Content = img;
        }






        private void MoveFile(string sourceFilePath, string destFilePath)
        {
            if (File.Exists(clientLocation + getFileNameOnly(destFilePath)))
                File.Replace(sourceFilePath, clientLocation + getFileNameOnly(destFilePath), null);
            else
                File.Move(sourceFilePath, clientLocation + getFileNameOnly(destFilePath));
        }
        private string getFileNameOnly(string filePath)
        {
            StringBuilder res = new StringBuilder();
            for (int i = filePath.Length-1; i >= 0; i--)
            {
                if (filePath[i] == '/')
                    break;
                res.Append(filePath[i]);
            }

            Reverse(res);
            return res.ToString();
        }
        public static void Reverse(StringBuilder sb)
        {
            char t;
            int end = sb.Length - 1;
            int start = 0;
    
            while (end - start > 0)
            {
                t = sb[end];
                sb[end] = sb[start];
                sb[start] = t;
                start++;
                end--;
            }
        }
    }
}