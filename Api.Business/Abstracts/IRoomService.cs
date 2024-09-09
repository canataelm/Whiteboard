using Common.Models.Dtos;
using Common.Models.ReturnTypes.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Business.Abstracts;

public interface IRoomService
{
    Task<IResult> CreateRoom(RoomDto roomDto);
    Task<IResult> GetActiveRooms();
    Task<IResult> JoinRoom(RoomDto roomDto );

    //Task<IResult> DeleteRoom(RoomDto roomDto );
    Task<IResult> ArchiveRoom(RoomDto roomDto);
    Task<IResult> UnarchiveRoom(RoomDto roomDto);

    Task<IResult> GetArchivedRooms();
    Task<IResult> DeleteRoom(int roomId);
}
