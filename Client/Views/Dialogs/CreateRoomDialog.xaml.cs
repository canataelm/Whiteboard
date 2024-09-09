using System.Windows;

namespace Client.Views.Dialogs;

/// <summary>
/// Interaction logic for CreateRoomDialog.xaml
/// </summary>
public partial class CreateRoomDialog : Window
{
    private readonly List<string> _existingRoomNames;

    public string RoomName { get; private set; }

    public CreateRoomDialog(List<string> existingRoomNames)
    {
        InitializeComponent();
        textbox_roomName.Focus();
        _existingRoomNames = existingRoomNames;
    }

    private void Cancel_Clicked(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }

    private void Create_Clicked(object sender, RoutedEventArgs e)
    {      
        RoomName = textbox_roomName.Text.Trim();

        if (string.IsNullOrEmpty(RoomName))
        {
            MessageBox.Show("Please enter a room name.");
            return;
        }

        if (_existingRoomNames.Contains(RoomName))
        {
            MessageBox.Show("Room with this name already exists. Please choose another name.");
            return;
        }

        this.DialogResult = true;
        this.Close();        
    }
}
