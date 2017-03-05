using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;
using Core.Contracts;

namespace FlickrAsyncApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchInfo _searchInfo = new SearchInfo();
        private object _lockList = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _searchInfo;
            BindingOperations.EnableCollectionSynchronization(_searchInfo.List, _lockList);
        }

        private IEnumerable<IFlickrRequest> GetSearchRequests()
        {
            return new List<IFlickrRequest>
            {
                new FlickrRequest {SearchTerm = _searchInfo.SearchTerm}
            };
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            _searchInfo.List.Clear();
        }

        private void OnSync(object sender, RoutedEventArgs e)
        {
            foreach (var req in GetSearchRequests())
            {
                var client = new WebClient();
                client.Credentials = req.Credentials;
                string resp = client.DownloadString(req.Url);
                IEnumerable<SearchItemResult> images = req.Parse(resp);
                foreach (var image in images)
                {
                    _searchInfo.List.Add(image);
                }
            }
        }

        private void OnAsync(object sender, RoutedEventArgs e)
        {
            Func<string, ICredentials, string> downloadString = (address, cred) =>
            {
                var client = new WebClient();
                client.Credentials = cred;
                return client.DownloadString(address);
            };

            Action<SearchItemResult> addItem = item => _searchInfo.List.Add(item);
            foreach (var req in GetSearchRequests())
            {
                downloadString.BeginInvoke(req.Url, req.Credentials, ar =>
                {
                    var resp = downloadString.EndInvoke(ar);
                    var images = req.Parse(resp);
                    foreach (var image in images)
                    {
                        Dispatcher.Invoke(addItem, image);
                    }
                }, null);
            }
        }

        private void OnAsyncEvent(object sender, RoutedEventArgs e)
        {
            foreach (var req in GetSearchRequests())
            {
                var client = new WebClient();
                client.Credentials = req.Credentials;
                client.DownloadStringCompleted += (sender1, e1) =>
                {
                    string resp = e1.Result;
                    var images = req.Parse(resp);
                    foreach (var image in images)
                    {
                        _searchInfo.List.Add(image);
                    }
                };
                client.DownloadStringAsync(new Uri(req.Url));
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private async void OnTaskBasedAsync(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            try
            {
                foreach (var req in GetSearchRequests())
                {
                    var clientHandler = new HttpClientHandler
                    {
                        Credentials = req.Credentials
                    };

                    var client = new HttpClient(clientHandler);

                    var response = await client.GetAsync(req.Url, _cts.Token);
                    var resp = await response.Content.ReadAsStringAsync();

                    await Task.Run(() =>
                    {
                        var images = req.Parse(resp);
                        foreach (var image in images)
                        {
                            _cts.Token.ThrowIfCancellationRequested();
                            _searchInfo.List.Add(image);
                        }
                    }, _cts.Token);

                }
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnTaskBasedAsync1(object sender, RoutedEventArgs e)
        {
            foreach (var req in GetSearchRequests())
            {
                var client = new WebClient();
                client.Credentials = req.Credentials;

                string resp = await client.DownloadStringTaskAsync(req.Url);

                var images = req.Parse(resp);
                foreach (var image in images)
                {
                    _searchInfo.List.Add(image);
                }
            }
        }
    }
}
