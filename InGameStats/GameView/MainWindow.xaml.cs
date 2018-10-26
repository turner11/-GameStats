using Microsoft.Win32;
using Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace GameView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        event EventHandler CsvTextChanged;
        private string _csvPath;
        readonly ManualResetEvent _resetEvent;
        private readonly GameViewModel _viewModel;

        string CsvPath
        {
            get { return _csvPath; }
            set
            {
                _csvPath = value;
                try
                {

                    var exsists = File.Exists(this._csvPath);
                    if (exsists)
                    {
                        this.rdbTrack.IsEnabled = true;
                        this.rdbTrack.IsChecked = true;

                    }
                    else
                    {
                        this.rdbTrack.IsEnabled = false;
                        this.rdbStopTracking.IsChecked = true;
                        MessageBox.Show($"{this._csvPath} does not exist");
                    }

                }
                catch (Exception ex)
                {

                    this.rdbTrack.IsEnabled = false;
                    MessageBox.Show(ex.Message);
                }
            }
        }

        string _currentText;
        private OpenFileDialog Ofd;

        string CurrentText
        {
            get { return this._currentText; }
            set
            {
                if (this._currentText != value)
                {
                    this._currentText = value;
                    this.CsvTextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        bool _isClosing { get; set; }



        public MainWindow()
        {
            
            this._viewModel = new GameViewModel();
            this.DataContext = this._viewModel;

            this._resetEvent = new ManualResetEvent(false);
            this.CsvTextChanged += Csv_TextChanged;
            this.Ofd = new Microsoft.Win32.OpenFileDialog();

            InitializeComponent();

        }


        private void TrakFile()
        {
            while (!this._isClosing)
            {

                
                this._resetEvent.WaitOne();

                try
                {
                    using (FileStream stream = File.Open(this._csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))

                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var txt = reader.ReadToEnd();//File.ReadAllText(this._csvPath);
                            this.CurrentText = txt;
                            Thread.Sleep(3000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }


        }


        private void Csv_TextChanged(object sender, EventArgs e)
        {
            List<PlayerData.GameSnapshot> snapShots = PlayerData.GetGameSnapShots(this.CurrentText);
            

            Application.Current.Dispatcher.BeginInvoke(
            //DispatcherPriority.Background,
          new Action(() => {
              this._viewModel.SetSnapShots(snapShots);
          }));
            

        }




        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._isClosing = true;
            this._resetEvent.Set();

        }

        private void btnNewCsv_Click(object sender, RoutedEventArgs e)
        {

            var myDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fileName = "GameStats_" +
                DateTime.Now.ToString()
                .Replace("\\", "_")
                .Replace("/", "_")
                .Replace(" ", "_")
                .Replace(":", "_");

            var path = System.IO.Path.Combine(myDoc, fileName);
            path = System.IO.Path.ChangeExtension(path, ".csv");




            try
            {
                PlayerData.CsvHandler.CreateTemplate(path);
                this.CsvPath = path;
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelectCsv_Click(object sender, RoutedEventArgs e)
        {
            bool result = this.Ofd.ShowDialog() ?? false;
            if (result)
            {
                this.CsvPath = this.Ofd.FileName;

            }
        }

        private void rdbTracking_CheckedChanged(object sender, RoutedEventArgs e)
        {
            //var fsw = new FileSystemWatcher(this.CsvPath);
            //fsw.Filter = this.CsvPath;
            //fsw.EnableRaisingEvents = true;
            //fsw.Changed += (sndr, arg) => _resetEvent.Set();
            var rdb = sender as RadioButton;
            if (rdb == null)
                return;
            var startTracking = this.rdbTrack?.IsChecked ?? false;
            if (startTracking)
            {
                this._resetEvent.Set();
            }
            else
            {
                this._resetEvent.Reset();
            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => this.TrakFile()).ConfigureAwait(false);
        }
    }
}