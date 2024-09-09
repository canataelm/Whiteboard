using Common.Models.Response;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ApiConnection.Services;




public class SignalRService
{
    private HubConnection _connection;

    

    public async Task ConnectionAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7121/Whiteboard")
            .Build();

     


    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
        }
        catch (Exception ex)
        {
            // Handle connection start error
        }
    }

    public async Task StopAsync()
    {
        try
        {
            await _connection.StopAsync();
        }
        catch (Exception ex)
        {
            // Handle connection stop error
        }
    }

    public void RegisterReceiveMessageHandler(Action<string> handler)
    {
        _connection.On<string>("ReceiveMessage", handler);
    }
}
