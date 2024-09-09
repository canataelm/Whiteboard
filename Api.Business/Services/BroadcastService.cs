using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Api.Business.Services
{
    public class BroadcastService
    {
        private const int BroadcastPort = 11000;
        private UdpClient udpClient;

        public BroadcastService()
        {
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
        }

        public async Task StartBroadcastAsync()
        {
            try
            {
                string localIP = GetLocalIPAddress();
                string broadcastAddress = GetBroadcastAddress(localIP);

                byte[] sendBytes = Encoding.ASCII.GetBytes($"API:{localIP}:{BroadcastPort}");
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), BroadcastPort);

                while (true)
                {
                    await udpClient.SendAsync(sendBytes, sendBytes.Length, endPoint);
                    Console.WriteLine("Broadcast message sent.");
                    await Task.Delay(5000); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while sending broadcast: " + ex.Message);
            }
        }

        private string GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProperties = ni.GetIPProperties();

                foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        
                        if (ip.Address.ToString().StartsWith("192.168.1"))
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private string GetBroadcastAddress(string localIP)
        {
            string[] ipParts = localIP.Split('.');
            return $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.255";
        }
    }
}
