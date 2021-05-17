using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Task_3.ViewModels;
using Task_8.AsyncCypher;

namespace Task_8
{
    public class MainWindowViewModel : BaseViewModel
    {

        public TaskManager MainTaskManager;
        
        public int RijndaelBlockSize = 16;
        public int MagentaKeySize = 16;
        public string EncryptionMode = "ECB";

        public String SymmetricKeyFile = null;
        public string IvFilePath = null;
        public String DataFile;
        public String PublicKeyFile;
        public String PrivateKeyFile;

        public MainWindowViewModel()
        {
            _img = new BitmapImage(new Uri("C:\\Users\\Administrator\\Desktop\\READY\\ReadyRPKS\\Task_8\\resources\\mainMenu.jpg"));
        }

        
        private BitmapImage _img;
        private String chainInString;
        private LinkedList<String> chain = new LinkedList<string>();

        public BitmapImage ImageContent
        {
            get =>
                _img;

            set
            {
                _img = value;
                OnPropertyChanged(nameof(ImageContent));
            }
        }
        public String ChainInString
        {
            get =>
                chainInString;

            set
            {
                chainInString = value;
                OnPropertyChanged(nameof(ChainInString));
            }
        }
        public LinkedList<String> Chain
        {
            get =>
                chain;

            set
            {
                chain = value;
                ChainInString = ChainToString();
            }
        }



        private String ChainToString()
        {
            String res = "";
            for (var element = chain.First; element != null;)
            {
                var next = element.Next;

                res += element.Value;
                
                element = next;
            }

            return res;
        }
    }
}