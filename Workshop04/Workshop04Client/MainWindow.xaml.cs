using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Workshop04Models;

namespace Workshop04Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        HttpClient client;
        TokenModel token;
        HubConnection conn;
        FileSystemWatcher watcher;
        public ObservableCollection<LoggingModel> Logs { get; set; }
        string basePath;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow(TokenModel token, string path)
        {
            InitializeComponent();

            Logs = new ObservableCollection<LoggingModel>();
            this.DataContext = Logs;
            this.token = token;
            this.basePath = path;
            ConfigureHttpClient();
            ConfigureSignalR();
            ConfigureWatcher(path);
            SetUsername();
        }

        public void ConfigureHttpClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5183");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
              new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
        }

        public void ConfigureSignalR()
        {
            conn = new HubConnectionBuilder().WithUrl("http://localhost:5183/events").Build();
            conn.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await conn.StartAsync();
            };
            conn.On<LoggingModel>("watchMessage", async t =>
            {
                Logs.Add(t);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("logs"));
            });
        }

        public void ConfigureWatcher(string path)
        {
            watcher = new FileSystemWatcher(path);
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
            watcher.Created += FileCreated;
            watcher.Deleted += FileDeleted;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private async void FileDeleted(object sender, FileSystemEventArgs e)
        {
            LoggingModel log = new LoggingModel();
            UserDataModel userData = await GetUserInfo();
            log.Email = userData.Email;
            log.Time = DateTime.Now;
            log.BasePath = this.basePath;
            log.File = e.Name;
            log.OperationType = OperationType.Delete;
            var response = await client.PostAsJsonAsync("/log", log);
        }

        private async void FileCreated(object sender, FileSystemEventArgs e)
        {
            LoggingModel log = new LoggingModel();
            UserDataModel userData = await GetUserInfo();
            log.Email = userData.Email;
            log.Time = DateTime.Now;
            log.BasePath = this.basePath;
            log.File = e.Name;
            log.OperationType = OperationType.Create;
            var response = await client.PostAsJsonAsync("/log", log);
        }

        async Task<UserDataModel> GetUserInfo()
        {
            var response = await client.GetAsync("/auth");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<UserDataModel>();
            }
            throw new Exception("something wrong...");
        }

        async void SetUsername()
        {
            UserDataModel userData = await GetUserInfo();
            tb_username.Content = userData.Email;
        }

    }
}
