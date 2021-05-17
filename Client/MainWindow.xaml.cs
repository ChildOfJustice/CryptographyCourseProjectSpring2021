using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CourseProjectCryptography2021.Spinner;
using SardorRsa;
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
            // get RSA key size
            int rsaKeySize = 516;
            try
            {
                rsaKeySize = Int32.Parse(RsaKeySizeHolder.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Wrong RSA key size, will be used <516>");
            }


            
            

            Task.Run(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        setContent();
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

                    if (_mainWindowViewModel.IvFilePath == null)
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

                    rsaCore = new RsaCore(rsaKeySize,generateKeys:false);
                    rsaCore.ImportPubKey(pubKeyFileName);
                    
                    CypherMethods.EncryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile, _mainWindowViewModel.SymmetricKeyFile+"Encrypted");
                    //File.Move(_mainWindowViewModel.SymmetricKeyFile+"Encrypted", serverLocation+"EncryptedSymmetricKey");
                    CypherMethods.EncryptKey(rsaCore, _mainWindowViewModel.IvFilePath, _mainWindowViewModel.IvFilePath+"Encrypted");
                    //File.Move(_mainWindowViewModel.IvFilePath+"Encrypted", serverLocation+"EncryptedIV");

                    // rsaCore.ImportPubKey(_mainWindowViewModel.PrivateKeyFile);
                    // CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile+"Encrypted2", _mainWindowViewModel.SymmetricKeyFile+"!!!DEcrypted");
                    // CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.IvFilePath+"Encrypted2", _mainWindowViewModel.IvFilePath+"DEcrypted");
                    
                    
                    
                    
                    
                    
                    _mainWindowViewModel.MainTaskManager = new TaskManager(_mainWindowViewModel.SymmetricKeyFile,_mainWindowViewModel.MagentaKeySize, _mainWindowViewModel.EncryptionMode);
                    
                    _mainWindowViewModel.MainTaskManager.ivFilePath = _mainWindowViewModel.IvFilePath;
                    _mainWindowViewModel.MainTaskManager.inputFilePath = _mainWindowViewModel.DataFile;
                    _mainWindowViewModel.MainTaskManager.outputFilePath = outputFileName;
                    
                    _mainWindowViewModel.MainTaskManager.RunEncryptionProcess();
                    //File.Move(outputFileName, serverLocation+outputFileName);
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

        private void setContent()
        {
            var pb = new CircularProgressBar();
            //pb.Message = "Wait...";
            ProgressBar.Content = pb;
        }
        
        private void removeContent()
        {
            ProgressBar.Content = null;
        }
    }
}