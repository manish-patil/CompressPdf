using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PDFCompress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel = new MainWindowViewModel();

        string syncFusionKey = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = viewModel;

            // Read the Syncfusion API key from the app.config file. More information in README.md
            syncFusionKey = ConfigurationManager.AppSettings["SYNCFUSION_API_KEY"] ?? string.Empty;

            if (string.IsNullOrEmpty(syncFusionKey)) { 
                MessageBox.Show("Syncfusion API key is missing. Please add it to the app.config file. For more information read README.md", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Select a PDF file to compress";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;

                if (fileName != null)
                {
                    viewModel.FileNameToCompress = fileName;
                    viewModel.FilePathToCompress = filePath;
                    viewModel.FileSelectedToCompress = true;
                    viewModel.IsCompressing = false;
                    viewModel.IsCompressed = null;
                    viewModel.CompressionStatusMessage = string.Empty;
                }
            }
        }

        private void btnCompressFile_Click(object sender, RoutedEventArgs e)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncFusionKey);

            if (viewModel.FilePathToCompress != null)
            {
                try
                {
                    using (PdfLoadedDocument document = new PdfLoadedDocument(viewModel.FilePathToCompress))
                    {
                        PdfCompressionOptions options = new PdfCompressionOptions();

                        // 1. Force the removal of duplicate or unused font data string
                        options.OptimizeFont = true;

                        // 2. Clean structural metadata and merge redundant elements left by Word
                        options.OptimizePageContents = true;
                        options.RemoveMetadata = true;

                        // 3. Apply and Save compressed document
                        document.Compress(options);

                        var path = $"{Directory.GetParent(viewModel.FilePathToCompress)}\\{viewModel.FileNameToCompress?.Replace(".pdf", "")}-Compressed.pdf";

                        document.Save(path);

                        viewModel.IsCompressed = true;
                        viewModel.CompressionStatusMessage = $"File compress at {path}";
                    }
                }
                catch (Exception ex)
                {
                    viewModel.IsCompressed = false;
                    viewModel.CompressionStatusMessage = $"Unable to compress file";
                }
            }
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string? _fileNameToCompress;
        private string? _filePathToCompress;
        private bool _fileSelectedToCompress;
        private bool _isCompressing;
        private bool? _isCompressed;
        private string? _compressionStatusMessage;

        public string? FileNameToCompress
        {
            get { return _fileNameToCompress; }
            set
            {
                _fileNameToCompress = value;
                OnPropertyChanged(nameof(FileNameToCompress));
            }
        }

        public string? FilePathToCompress
        {
            get { return _filePathToCompress; }
            set
            {
                _filePathToCompress = value;
                OnPropertyChanged(nameof(FilePathToCompress));
            }
        }

        public bool FileSelectedToCompress
        {
            get { return _fileSelectedToCompress; }
            set
            {
                _fileSelectedToCompress = value;
                OnPropertyChanged(nameof(FileSelectedToCompress));
            }
        }

        public bool IsCompressing
        {
            get { return _isCompressing; }
            set
            {
                _isCompressing = value;
                OnPropertyChanged(nameof(IsCompressing));
            }
        }

        public bool? IsCompressed
        {
            get { return _isCompressed; }
            set
            {
                _isCompressed = value;
                OnPropertyChanged(nameof(IsCompressed));
            }
        }

        public string? CompressionStatusMessage
        {
            get { return _compressionStatusMessage; }
            set
            {
                _compressionStatusMessage = value;
                OnPropertyChanged(nameof(CompressionStatusMessage));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}