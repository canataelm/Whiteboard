using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using Api.DataAccess.Abstracts;
using System.Collections.Concurrent;
using Api.Models.Entities;

namespace Api.Business.Hubs;

public class DrawingHub : Hub
{
    private static Dictionary<int, HubConnection> roomConnections = new Dictionary<int, HubConnection>();
    private Dictionary<string, string> userRooms = new Dictionary<string, string>();
    private readonly IUnitOfWork _unitOfWork;
    private static readonly ConcurrentDictionary<string, List<byte[]>> RoomInkData = new ConcurrentDictionary<string, List<byte[]>>();


    public DrawingHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    

    public async Task SendInkCanvasData( string roomId ,byte[] inkData)
    {

        

        var drawing = new Drawing
        {
            RoomId = roomId,
            InkData = inkData,
            CreatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Drawing.AddAsync(drawing);
        await _unitOfWork.SaveChangesAsync();

        await Clients.Group(roomId).SendAsync("ReceiveInkCanvasData", inkData);

        

     
    }

    public async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);

        var drawings = await _unitOfWork.Drawing.GetAllAsync();
        var roomDrawings = drawings.Where(d => d.RoomId == group).OrderBy(d => d.CreatedAt);

        foreach (var drawing in roomDrawings)
        {
            await Clients.Caller.SendAsync("ReceiveInkCanvasData", drawing.InkData);
        }

            //if (RoomInkData.TryGetValue(group, out var inkDataList))
            //{
            //    foreach (var inkData in inkDataList)
            //    {
            //        await Clients.Caller.SendAsync("ReceiveInkCanvasData", inkData);
            //    }
            //}
    }


    public async Task SendMessage(string message)
    {

        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public override Task OnConnectedAsync()
    {

        //var httpContext = Context.User.Identities.
        


        UserHandler.ConnectedIds.Add(Context.ConnectionId);

        Debug.WriteLine("CONNECTED IDS");
        foreach (var item in UserHandler.ConnectedIds)
        {
            Debug.WriteLine(item);
        }

       
        return base.OnConnectedAsync();
    }


    public override Task OnDisconnectedAsync(Exception exception)
    {
        UserHandler.ConnectedIds.Remove(Context.ConnectionId);
        Debug.WriteLine("CONNECTED IDS");
        foreach (var item in UserHandler.ConnectedIds)
        {
            Debug.WriteLine(item);
        }
        return base.OnDisconnectedAsync(exception);
    }


    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }


    public class DrawingObject
    {
        public byte[] InkData { get; set; }
        public int RoomId { get; set; }
    }
}
