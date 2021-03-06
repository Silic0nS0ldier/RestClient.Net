﻿using RestClientDotNet;
using RestClientNetSamples;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using restClient = RestClientDotNet.RestClient;

#if __WASM__
using Uno.UI.Wasm;
#endif

namespace RestClient.Net.Samples.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields
        private restClient _BitbucketClient;
        #endregion

        #region Constructror
        public MainPage()
        {
            InitializeComponent();
            AttachEventHandlers();
        }

        #endregion

        #region Private Methods

        private void GetBitBucketClient(string password, bool isGet)
        {
            var url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
#if __WASM__
            var httpClient = new HttpClient(new WasmHttpHandler());
#else
            var httpClient = new HttpClient();
#endif

            _BitbucketClient = new restClient(new NewtonsoftSerializationAdapter(), new Uri(url), new TimeSpan(0, 3, 0), httpClient);

            if (!string.IsNullOrEmpty(password))
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + password));
                _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            }
        }

        private async void OnGetReposClick()
        {
            ToggleBusy(true);

            try
            {
                ReposBox.ItemsSource = null;
                ReposBox.IsEnabled = false;

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword(), true);

                //Download the repository data
                var repos = (await _BitbucketClient.GetAsync<RepositoryList>());

                //Put it in the List Box
                ReposBox.ItemsSource = repos.values;
                ReposBox.SelectedItem = repos.values.FirstOrDefault();
                ReposBox.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await HandleException($"An error occurred while attempting to get repos.");
            }

            ToggleBusy(false);
        }

        private async Task OnSavedClicked()
        {
            ToggleBusy(true);

            try
            {
                var selectedRepo = ReposBox.SelectedItem as Repository;
                if (selectedRepo == null)
                {
                    return;
                }

                //Ensure the client is ready to go
                GetBitBucketClient(GetPassword(), false);

                var requestUri = $"{UsernameBox.Text}/{selectedRepo.full_name.Split('/')[1]}";

                //Put the change
                var retVal = await _BitbucketClient.PutAsync<Repository, Repository>(selectedRepo, requestUri);

                await DisplayAlert("Saved", "Your repo was updated.");
            }
            catch (Exception ex)
            {
                await HandleException($"Save error. Please ensure you entered your credentials.");
            }

            ToggleBusy(false);
        }

        private async Task HandleException(string message)
        {
            await DisplayAlert("Error", message);
        }

        private string GetPassword()
        {

#if __WASM__
            return ThePasswordBox.Text;
#else
            return ThePasswordBox.Password;
#endif
        }

        private async Task DisplayAlert(string title, string message)
        {
            var messageDialog = new MessageDialog(message, title);
            await messageDialog.ShowAsync();
        }

        private void ToggleBusy(bool isBusy)
        {
            ReposActivityIndicator.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AttachEventHandlers()
        {
            SaveButton.Click += SaveButton_Clicked;
            GetReposButton.Click += GetReposButton_Clicked;
            ReposBox.SelectionChanged += ReposBox_ItemSelected;
        }
        #endregion

        #region Event Handlers
        private void ReposBox_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ReposPage.DataContext = ReposBox.SelectedItem as Repository;
        }


        private async void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            await OnSavedClicked();
        }

        private void GetReposButton_Clicked(object sender, RoutedEventArgs e)
        {
            OnGetReposClick();
        }
        #endregion
    }
}
