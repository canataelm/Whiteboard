using Api.Business.Abstracts;
using Api.Business.Hubs;
using Api.Business.Tools;
using Api.DataAccess.Abstracts;
using Api.Models.Entities;
using Common.Models.Dtos;
using Common.Models.ReturnTypes.Abstract;
using Common.Models.ReturnTypes.Concrete;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Api.Business.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<DrawingHub> _hubContext;
    
   

    public RoomService(IUnitOfWork unitOfWork, IHubContext<DrawingHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        
    }


    public async Task<IResult> CreateRoom(RoomDto roomDto)
    {
        Room room = Mapper.Map(roomDto);

        await _unitOfWork.RoomRepository.CreateRoom(room);
        await _unitOfWork.Complete();
      
        string groupName = $"Room_{room.Id}";
        
        await _hubContext.Groups.AddToGroupAsync(roomDto.Id.ToString(), groupName);
              
        return new SuccessResult();
        
    }

    public async Task<IResult> GetActiveRooms()
    {
        List<Room> rooms = await _unitOfWork.RoomRepository.GetActiveRooms();

        List<RoomDto> roomDtos = Mapper.Map(rooms);



        return new SuccessDataResult<List<RoomDto>>(roomDtos);
    }

    public async Task<IResult> JoinRoom(RoomDto roomDto )
    {
        Room room = await _unitOfWork.RoomRepository.GetRoomById(roomDto.Id);
        if (room == null)
        {
            return new ErrorResult("Room not found");
        }
        string groupName = $"Room_{room.Id}";

        

        await _hubContext.Groups.AddToGroupAsync(roomDto.Id.ToString(), groupName);   
        await _hubContext.Clients.Group(groupName).SendAsync("UserJoined", $"User {roomDto.Id} joined the room");
        //roomDto.Id

        Debug.WriteLine("REPORT");
        Debug.WriteLine($"User joined to room: {roomDto.Id} - {roomDto.Name}");
        Debug.WriteLine($"Group name: {groupName}");
        Debug.WriteLine($"Clients all: {_hubContext.Clients}");


        return new SuccessResult();
         
    }

    public async Task<IResult> DeleteRoom(int roomId) 
    {
        Room room = await _unitOfWork.RoomRepository.GetRoomById(roomId);
        if (room == null)
        {
            return new ErrorResult("Room not found");
        }

        
        _unitOfWork.RoomRepository.DeleteRoom(room);
        await _unitOfWork.Complete();

        
        string groupName = $"Room_{room.Id}";
        await _hubContext.Groups.RemoveFromGroupAsync(roomId.ToString(), groupName);

        return new SuccessResult();
    }
    //room.IsActive = true;

    public async Task<IResult> GetArchivedRooms()
    {
        List<Room> archivedRooms = await _unitOfWork.RoomRepository.GetArchivedRooms();

        if (archivedRooms == null || !archivedRooms.Any())
        {
            return new SuccessResult("No archived rooms found.");
            // return new ErrorDataResult<List<RoomDto>>("No archived rooms found.");
        }

        List<RoomDto> archivedRoomDtos = Mapper.Map(archivedRooms);
        return new SuccessDataResult<List<RoomDto>>(archivedRoomDtos);
    }


    public async Task<IResult> ArchiveRoom(RoomDto roomDto)
    {
        Room room = await _unitOfWork.RoomRepository.GetRoomById(roomDto.Id);
        if (room == null)
        {
            return new ErrorResult("Room not found");
        }


        room.IsArchived = true;
        _unitOfWork.RoomRepository.UpdateRoom(room);
        await _unitOfWork.Complete();


        string groupName = $"Room_{room.Id}";
        await _hubContext.Groups.RemoveFromGroupAsync(roomDto.Id.ToString(), groupName);

        return new SuccessResult();
    }

    public async Task<IResult> UnarchiveRoom(RoomDto roomDto)
    {
        Room room = await _unitOfWork.RoomRepository.GetRoomById(roomDto.Id);
        if (room == null)
        {
            return new ErrorResult("Room not found");
        }

        room.IsArchived = false;
        _unitOfWork.RoomRepository.ArchiveRoom(room);
        await _unitOfWork.Complete();

        string groupName = $"Room_{room.Id}";
        await _hubContext.Groups.AddToGroupAsync(roomDto.Id.ToString(), groupName);

        return new SuccessResult();
    }


}
