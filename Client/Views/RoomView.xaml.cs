
using Client.Views.Dialogs;
using Common.Models.Dtos;
using Common.Models.Response;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
    
    public partial class RoomView : Window
    {
        private ObservableCollection<RoomDto> _Observablerooms;
        private List<RoomDto> _rooms = new List<RoomDto>();
        private RoomDto selectedRoom;
        private HubConnection _connection;
        private readonly string serverIp;
        private readonly int serverPort;

        private readonly HttpClient _httpClient = new HttpClient();
        //private const string _baseUrl = "https://localhost:7121/api/Room";
        //private const string _baseUrl = "https://192.168.1.255/api/Room";
        private  string _baseUrl;
        public RoomView(string serverIp, int serverPort)
        {
            InitializeComponent();
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            _baseUrl = $"http://{serverIp}:{serverPort}/api/Room";
            Refreash_Click(null, null);
            HttpClient client = new HttpClient(); 
            _Observablerooms = new ObservableCollection<RoomDto>();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = $"Whiteboard - {version}";
        }

        private async void CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomDialog createRoomDialog = new (_rooms.Select(r => r.Name).ToList());
            var dr = createRoomDialog.ShowDialog();

            if (!dr.Value)
                return;

            string roomName = createRoomDialog.RoomName;


            var jsonContent = new StringContent(JsonConvert.SerializeObject(new { Name = roomName }), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_baseUrl + "/create", jsonContent);

            //HttpResponseMessage responseMessage = await _httpClient.PostAsync(_baseUrl + "/create", new StringContent
            //  ($"\"{roomName}\""));

            if (responseMessage.IsSuccessStatusCode)
            {
                RoomListbox.Items.Add(roomName);
                MessageBox.Show("Room created: " + roomName);
                Refreash_Click(null, null);

            }
            else
            {
                MessageBox.Show("Room could not be created: " + responseMessage.StatusCode);
            }

        }

        private async void JoinRoom_Click(object sender, RoutedEventArgs e)
        {

            if (RoomListbox.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to join.");
                return;
            }

           
          
            RoomDto selectedRoomDto = (RoomDto)RoomListbox.SelectedItem;
            
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_baseUrl + "/join",
                new StringContent(JsonConvert.SerializeObject(selectedRoomDto), Encoding.UTF8, "application/json"));

            if (responseMessage.IsSuccessStatusCode)
            {
                selectedRoom = selectedRoomDto;

                MessageBox.Show("Joined room: " + selectedRoom.Name);

                
                MainWindow mainWindow = new MainWindow(selectedRoom, serverIp, serverPort);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to join room: " + responseMessage.StatusCode);
            }

            //await _connection.InvokeAsync("JoinGroup", selectedRoom);

        }

        private async void Refreash_Click(object sender, RoutedEventArgs e)
        {



            HttpResponseMessage responseMessage = await _httpClient.GetAsync(_baseUrl + "/getRooms");

            if (responseMessage.IsSuccessStatusCode)
            {
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                if (responseObject != null && responseObject.Data != null)
                {
                    RoomListbox.Items.Clear();
                    foreach (var room in responseObject.Data)
                    {
                        if (!room.IsArchived)  
                        {
                            RoomListbox.Items.Add(room);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No rooms found.");
                }
            }
            else
            {
                MessageBox.Show("Failed to fetch rooms: " + responseMessage.StatusCode);
            }


        }

        private void RoomListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
        }

        private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {


            if (RoomListbox.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            RoomDto selectedRoomDto = (RoomDto)RoomListbox.SelectedItem;

            
            string url = $"{_baseUrl}/delete/{selectedRoomDto.Id}";



            HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                MessageBox.Show("Room deleted successfully.");
                Refreash_Click(null, null);
            }
            else
            {
                MessageBox.Show($"Failed to delete room: {responseMessage.StatusCode}. URL: {url}");
            }
        }

        private async void ArchiveRoom_Click(object sender, RoutedEventArgs e)
        {


            if (RoomListbox.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to archive.");
                return;
            }

            RoomDto selectedRoomDto = (RoomDto)RoomListbox.SelectedItem;

            var jsonContent = new StringContent(JsonConvert.SerializeObject(selectedRoomDto), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await _httpClient.PutAsync($"{_baseUrl}/archive", jsonContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                MessageBox.Show("Room archived successfully.");
                _Observablerooms.Remove(selectedRoomDto);
                //RoomListbox.Items.Remove(selectedRoomDto);
               Refreash_Click(null, null); 
               
            }
            else
            {
                MessageBox.Show($"Failed to archive room: {responseMessage.StatusCode}. URL: {_baseUrl}/archive");
            }
        }

        private void UnarchiveRoom_Click(object sender, RoutedEventArgs e)
        {
            
            ArchiveRoom archiveRoom = new ArchiveRoom(_httpClient, _baseUrl);
            archiveRoom.ShowDialog();
            Refreash_Click(null, null); 
        }


    }
}
