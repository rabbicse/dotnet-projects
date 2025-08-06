using System.Windows;

namespace AsyncWpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        private uint _counter = 0;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the ViewModel and set it as DataContext`
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Async Event handler for downloading data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadDataEvent(object sender, RoutedEventArgs e)
        {
            // ✅ The following call is Okay
            await viewModel.DownloadDataAsync(++_counter);


            // ❌ Don't do this!
            //viewModel.DownloadDataAsync(++_counter).Wait();


            // ✅ The following call is Okay
            //await viewModel.DownloadDataAsync(++_counter);


            // ❌ Don't do this!
            var counterResult = viewModel.DownloadDataWithCounterAsync(++_counter).Result;

        }

        private async void ParallelForEvent(object sender, RoutedEventArgs e)
        {
            //await viewModel.ParallelForExample();
            await viewModel.ParallelForExampleAsync();
        }
    }
}