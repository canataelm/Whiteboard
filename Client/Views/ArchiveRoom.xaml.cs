using Common.Models.Dtos;
using Common.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for ArchiveRoom.xaml
    /// </summary>
    public partial class ArchiveRoom : Window
    {
        private ObservableCollection<RoomDto> _archivedRooms;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public ArchiveRoom(HttpClient httpClient, string baseUrl)
        {
            InitializeComponent();
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _archivedRooms = new ObservableCollection<RoomDto>();
            ArchivedRoomsListBox.ItemsSource = _archivedRooms;

            LoadArchivedRooms();
        }

        private async void LoadArchivedRooms()
        {
            _archivedRooms.Clear();
            HttpResponseMessage responseMessage = await _httpClient.GetAsync($"{_baseUrl}/archivedRooms");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                if (responseObject != null && responseObject.Data != null)
                {
                    ArchivedRoomsListBox.ItemsSource = responseObject.Data;
                }
                else
                {
                    MessageBox.Show("No archived rooms found.");
                }
            }
            else
            {
                MessageBox.Show("Failed to load archived rooms.");
            }
            //HttpResponseMessage responseMessage = await _httpClient.GetAsync($"{_baseUrl}/archivedRooms");

            //if (responseMessage.IsSuccessStatusCode)
            //{
            //    string responseBody = await responseMessage.Content.ReadAsStringAsync();
            //    var rooms = JsonConvert.DeserializeObject<List<RoomDto>>(responseBody);




            //    ArchivedRoomsListBox.ItemsSource = rooms;
            //}
            //else
            //{
            //    MessageBox.Show("Failed to load archived rooms.");
            //}
        }

        private async void UnarchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = ArchivedRoomsListBox.SelectedItem as RoomDto;
            if (selectedRoom != null)
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(selectedRoom), Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PutAsync($"{_baseUrl}/unarchive", jsonContent);

                if (responseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Room unarchived successfully.");
                    _archivedRooms.Remove(selectedRoom);
                    LoadArchivedRooms();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to unarchive room: {responseMessage.StatusCode}. URL: {_baseUrl}/unarchive");
                }
            }
            else
            {
                MessageBox.Show("Please select a room to unarchive.");
            }
        }
    }
}
