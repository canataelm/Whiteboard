using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Ink;
using Client.ApiConnection;
using Client.ApiConnection.Services;
using Common.Models.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using System.IO;
using Common.Models.Response;
using Client.Views;
using Microsoft.Win32;


namespace Client
{
    
    public partial class MainWindow : Window
    {
        AccountService _accountService;
        private SignalRService signalRService;
        private HubConnection _connection;
        private string savedFilePath;
        private readonly string serverIp;
        private readonly int serverPort;




        public int RoomId { get; set; }

        public MainWindow(RoomDto selectedRoom, string serverIp,int serverPort)
        {
            InitializeComponent();
            Task.Run(() => InitializeSignalRConnection());
            this.serverIp = serverIp;
            this.serverPort = serverPort;


            RoomId = selectedRoom.Id;
          

            inkCanvas.DefaultDrawingAttributes = PenAttributes;
            inkCanvas.DefaultDrawingAttributes = EraserAttributes;

            
            string url = $"http://{serverIp}:{serverPort}";


            HttpConfiguration configuration = new HttpConfiguration(url);
            _accountService = new AccountService(configuration);
            

        }



        private async Task InitializeSignalRConnection() 
        {
            _connection = new HubConnectionBuilder()
                        .WithUrl($"http://{serverIp}:{serverPort}/Whiteboard")
                        .Build();

           /* _connection = new HubConnectionBuilder()
                       .WithUrl("https://localhost:7121/Whiteboard?roomId={roomId}")
                       .Build();*/

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(message);
                });

            });
            
            _connection.On<byte[]>("ReceiveInkCanvasData", (inkData) =>
            {
                Dispatcher.Invoke(() =>
                {
                    
                    inkCanvas.Strokes = DeserializeStrokeCollection(inkData);
                });
            });
            await _connection.StartAsync();
            await JoinGroup(RoomId.ToString());
            
        }




        private async Task JoinGroup(string group)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _connection.InvokeAsync("JoinGroup", group);
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"A error occurd while joining room: {ex.ToString()}");
                }
            }
            else
            {
                MessageBox.Show("Try again.");
            }
        }

        private byte[] SerializeStrokeCollection(StrokeCollection strokes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                strokes.Save(stream);
                return stream.ToArray();
            }
        }

        
        private StrokeCollection DeserializeStrokeCollection(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return new StrokeCollection(stream);
            }
        }
    



        private readonly DrawingAttributes EraserAttributes = new() 
        {
            
        };

        private readonly DrawingAttributes PenAttributes = new()
        {
            Color = Colors.Black,
            Height =2,
            Width =2
        };

        private readonly DrawingAttributes HighlighterAttributes = new()
        {
            Color = Colors.Yellow,
            Height = 10,
            Width = 2,
            IgnorePressure = true,
            IsHighlighter = true,
            StylusTip = StylusTip.Rectangle
        };




       
        private void PenButton_Click(object sender, RoutedEventArgs e)
        {

            SetEditingMode(EditingMode.Pen);
        }
        private void Highlighter_Click(object sender, RoutedEventArgs e)
        {
            SetEditingMode(EditingMode.Highlighter);
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {

            SetEditingMode(EditingMode.Select);
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {

            SetEditingMode(EditingMode.Eraser);
        }

        private void ColorSelecter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SetEditingMode(EditingMode editingMode)
        {
            //PenButton.IsEnabled = false;
            //SelectButton.IsEnabled = false;
            //EraseButton.IsEnabled = false;

            switch (editingMode)
            {
                case EditingMode.Pen:
                    // PenButton.IsEnabled = true;
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    inkCanvas.DefaultDrawingAttributes = PenAttributes;
                    break;
                case EditingMode.Highlighter:
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    inkCanvas.DefaultDrawingAttributes= HighlighterAttributes;
                    break;

                case EditingMode.Select:
                    // SelectButton.IsEnabled = true;
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;

                case EditingMode.Eraser:
                    if (PointEraserRadio.IsChecked == true)
                        inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    else
                        inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;                                      
                    break;

                default:
                    break;
            }
        }

        private void PenColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded)
            {
                PenAttributes.Color = PenColorPicker.SelectedColor ?? Colors.Black;
                HighlighterAttributes.Color = PenColorPicker.SelectedColor ?? Colors.Yellow;
            }
        }

        private void ThicknesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                PenAttributes.Width = ThicknesSlider.Value;
                PenAttributes.Height = ThicknesSlider.Value;
            }
        }

        private void EraserThicknesSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                
                double scaledValue = ScaleValueToRange(EraserThicknesSlider.Value, 1, 50); 

                EraserAttributes.Width = scaledValue;
                EraserAttributes.Height = scaledValue;
            }
        }

        private double ScaleValueToRange(double value, double min, double max)
        {
            return (value - EraserThicknesSlider.Minimum) / (EraserThicknesSlider.Maximum - EraserThicknesSlider.Minimum) * (max - min) + min;
        }

        private void PointEraserRadio_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void FullEraserRadio_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

       

        private async void inkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            

            byte[] inkData = SerializeStrokeCollection(inkCanvas.Strokes);
            

            int roomId = RoomId;

            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("SendInkCanvasData", roomId.ToString(), inkData);
            }
            else
            {
                MessageBox.Show("not connected");
            }
        }


        private void TriangleButton_Click(object sender, RoutedEventArgs e)
        {

            
        }

        private void SquareButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void LineButton_Click(object sender, RoutedEventArgs e)
        {
           // await InitializeSignalRConnection();
        }
        private async void button_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load InkCanvas File";
            dialog.DefaultExt = "sandy";
            dialog.Filter = "InkCanvas Format (.png)|*.png";
            dialog.ShowDialog();
            if (dialog.FileName == "") return;
            FileStream fileStream = File.Open(dialog.FileName, FileMode.Open);
            StrokeCollection strokes = new StrokeCollection(fileStream);
            inkCanvas.Strokes = strokes;
            fileStream.Close();


        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
         

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "InkCanvas Format|*.png";
            saveFileDialog.Title = "Save InkCanvas File";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == "") return;
            FileStream fileStream = File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate);
            inkCanvas.Strokes.Save(fileStream);
            fileStream.Close();
        }
     

        private async void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
            }

            

            RoomView roomView = new RoomView(serverIp, serverPort);
            roomView.Show();
            
            this.Close();

        }
    }

   


    public enum EditingMode 
    {
        Pen, Select, Eraser, Highlighter
    }

    public enum ShapeDrawingMode
    {
        None,
        Triangle,
        Square,
        Line
    }
}