using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CourseProjectCryptography2021.Spinner;
using SardorRsa;
using Task_3.WPF_Control_Elements.SpinnerDialog;
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
        
        private string serverLocation = "C:\\Users\\Administrator\\Desktop\\READY\\Cryptography\\CryptographyCourseProjectSpring2021\\Server\\bin\\Debug\\resources\\";
        
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
        private void OnSendFileButtonClick(object sender, RoutedEventArgs e)
        {
            var outputFileName = OutPutFilePathHolder.Text;

            var pubKeyFileName = _mainWindowViewModel.PublicKeyFile;


            Task.Run(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        setContent("Searching for the key and IV...");
                    });
                    if (_mainWindowViewModel.SymmetricKeyFile == null)
                    {
                        //generate symmetric key for MAGENTA
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] magentaKey = new byte[_mainWindowViewModel.MagentaKeySize];
                        rng.GetBytes(magentaKey);
                        for (int i = 0; i < magentaKey.Length; i++)
                        {
                            if (magentaKey[i] < 2)
                                magentaKey[i] = 2;
                        }

                        _mainWindowViewModel.SymmetricKeyFile = "./resources/key";
                        //export IV and this key and encrypt them with the RSA pub key
                        using (var outputStream = File.Open(_mainWindowViewModel.SymmetricKeyFile, FileMode.Create))
                            outputStream.Write(magentaKey, 0, magentaKey.Length);
                    }

                    if (_mainWindowViewModel.IvFilePath == null &&  _mainWindowViewModel.EncryptionMode != "ECB")
                    {
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] IV = new byte[16];
                        rng.GetBytes(IV);
                        for (int i = 0; i < IV.Length; i++)
                        {
                            if (IV[i] < 2)
                                IV[i] = 2;
                        }
                        
                        _mainWindowViewModel.IvFilePath = "./resources/IV";
                        //export IV and this key and encrypt them with the RSA pub key
                        using (var outputStream = File.Open(_mainWindowViewModel.IvFilePath, FileMode.Create))
                            outputStream.Write(IV, 0, IV.Length);
                    }

                    rsaCore = new RsaCore(generateKeys:false);
                    rsaCore.ImportPubKey(pubKeyFileName);
                    
                    CypherMethods.EncryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile, _mainWindowViewModel.SymmetricKeyFile+"Encrypted");
                    // File.Replace(getFileNameOnly(_mainWindowViewModel.SymmetricKeyFile+"Encrypted"), getFileNameOnly(serverLocation+"EncryptedSymmetricKey"), null);
                    MoveFile(_mainWindowViewModel.SymmetricKeyFile + "Encrypted", "EncryptedSymmetricKey");
                    
                    // try
                    // {
                    //     File.Move(_mainWindowViewModel.SymmetricKeyFile+"Encrypted", serverLocation+getFileNameOnly("EncryptedSymmetricKey"));
                    // }
                    // catch
                    // {
                    //     File.Replace(_mainWindowViewModel.SymmetricKeyFile+"Encrypted", serverLocation+getFileNameOnly("EncryptedSymmetricKey"), null);    
                    // }

                    if (_mainWindowViewModel.EncryptionMode != "ECB")
                    {
                        CypherMethods.EncryptKey(rsaCore, _mainWindowViewModel.IvFilePath, _mainWindowViewModel.IvFilePath+"Encrypted");
                        //File.Replace(getFileNameOnly(_mainWindowViewModel.IvFilePath+"Encrypted"), getFileNameOnly(serverLocation+"EncryptedIV"), null);
                        MoveFile(_mainWindowViewModel.IvFilePath+"Encrypted", "EncryptedIV");
                        
                        // try
                        // {
                        //     File.Move((_mainWindowViewModel.IvFilePath+"Encrypted"), serverLocation+getFileNameOnly("EncryptedIV"));
                        // }
                        // catch
                        // {
                        //     File.Replace((_mainWindowViewModel.IvFilePath+"Encrypted"), serverLocation+getFileNameOnly("EncryptedIV"), null);    
                        // }
                    }
                    

                    // rsaCore.ImportPubKey(_mainWindowViewModel.PrivateKeyFile);
                    // CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile+"Encrypted2", _mainWindowViewModel.SymmetricKeyFile+"!!!DEcrypted");
                    // CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.IvFilePath+"Encrypted2", _mainWindowViewModel.IvFilePath+"DEcrypted");
                    
                    
                    
                    
                    
                    
                    _mainWindowViewModel.MainTaskManager = new TaskManager(_mainWindowViewModel.SymmetricKeyFile,_mainWindowViewModel.MagentaKeySize, _mainWindowViewModel.EncryptionMode);
                    
                    if (_mainWindowViewModel.EncryptionMode != "ECB")
                        _mainWindowViewModel.MainTaskManager.ivFilePath = _mainWindowViewModel.IvFilePath;
                    _mainWindowViewModel.MainTaskManager.inputFilePath = _mainWindowViewModel.DataFile;
                    _mainWindowViewModel.MainTaskManager.outputFilePath = outputFileName;
                    
                    _mainWindowViewModel.MainTaskManager.RunEncryptionProcess();
                    // File.Replace(getFileNameOnly(outputFileName), getFileNameOnly(serverLocation+outputFileName), null);
                    MoveFile(outputFileName, outputFileName);
                    // try
                    // {
                    //     File.Move((outputFileName), serverLocation+getFileNameOnly(outputFileName));
                    // }
                    // catch
                    // {
                    //     File.Replace((outputFileName), serverLocation+getFileNameOnly(outputFileName), null);    
                    // }
                    
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
        private void OnChoosePublicKeyFileButtonClick(object sender, RoutedEventArgs e)
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
                _mainWindowViewModel.PublicKeyFile = filename;
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
            
        }

        private void setContent(string message)
        {
            var pb = new CircularProgressBar();
            //pb.Message = message;
            ProgressBar.Content = pb;
        }

        // private void changeMessage(string message)
        // {
        //     SpinnerDialogElement dialog = (SpinnerDialogElement) ProgressBar.Content;
        //     dialog.Message = message;
        // }
        
        private void removeContent()
        {
            var img = this.FindResource("LandingImage") as Image;
            ProgressBar.Content = img;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        private void MoveFile(string sourceFilePath, string destFilePath)
        {
            if (File.Exists(serverLocation + getFileNameOnly(destFilePath)))
                File.Replace(sourceFilePath, serverLocation + getFileNameOnly(destFilePath), null);
            else
                File.Move(sourceFilePath, serverLocation + getFileNameOnly(destFilePath));
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