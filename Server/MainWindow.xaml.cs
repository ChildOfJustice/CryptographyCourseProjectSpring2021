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
        private void OnStartSessionButtonClick(object sender, RoutedEventArgs e)
        {
            //get key path to export them
            var pubKeyFileName = OutPutPubKeyFilePathHolder.Text;
            var privateKeyFileName = OutPutPrivateKeyFilePathHolder.Text;
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
                    
                    rsaCore = new RsaCore(rsaKeySize);
                    //export keys
                    rsaCore.ExportPubKey(pubKeyFileName);
                    rsaCore.ExportPrivateKey(privateKeyFileName);
                    
                    ///////////////////////////////FOR TESTS:
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

                    //decrypt symmetric key with private RSA key
                    rsaCore = new RsaCore(rsaKeySize, generateKeys:false);
                    rsaCore.ImportPrivateKey(privateKeyFileName);
                    
                    var decryptedKeyFilePath = "./resources/DecryptedSymmetricKey";
                    CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.SymmetricKeyFile, decryptedKeyFilePath);
                    var decryptedIvFilePath = "./resources/DecryptedIv";
                    CypherMethods.DecryptKey(rsaCore, _mainWindowViewModel.IvFilePath, decryptedIvFilePath);

                    
                    // var decryptedKeyFilePath = "./resources/key";
                    // var decryptedIvFilePath = "./resources/IV";
                    //
                    //decrypt file with MAGENTA
                    _mainWindowViewModel.MainTaskManager = new TaskManager(decryptedKeyFilePath,_mainWindowViewModel.MagentaKeySize, _mainWindowViewModel.EncryptionMode);
                    
                    _mainWindowViewModel.MainTaskManager.keyFilePath = decryptedKeyFilePath;
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