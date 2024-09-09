using Client.Views;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client.Views;

public partial class BroadcastSearchView : Window
{
    private const int BroadcastPort = 11000;
    private UdpClient udpClient;

    public BroadcastSearchView()
    {
        InitializeComponent();
        udpClient = new UdpClient(BroadcastPort);
        StartListeningAsync();
    }

    private async Task StartListeningAsync()
    {
        try
        {
            await Task.Delay(5000);
            var receiveTask = udpClient.ReceiveAsync();
            var delayTask = Task.Delay(TimeSpan.FromMinutes(1)); 

            var completedTask = await Task.WhenAny(receiveTask, delayTask);

            if (completedTask == receiveTask)
            {
                UdpReceiveResult receivedResult = await receiveTask;
                string receivedMessage = Encoding.ASCII.GetString(receivedResult.Buffer);
                Console.WriteLine($"Received broadcast message: {receivedMessage}");

                OpenRoomView(receivedMessage);
            }
            else
            {                
                MessageBox.Show("Sunucu bulunamadı. Uygulama kapanacak.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while receiving broadcast: " + ex.Message);
        }
        finally
        {
            udpClient?.Close();
        }
    }

    private void OpenRoomView(string serverMessage)
    {
        var parts = serverMessage.Split(':');
        if (parts.Length == 3 && parts[0] == "API")
        {
            string ip = parts[1];
            int port = int.Parse(parts[2]);

            RoomView roomView = new RoomView(ip, port);
            roomView.Show();
            this.Close();
        }
        else
        {
            MessageBox.Show("Sunucu mesajı geçersiz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}