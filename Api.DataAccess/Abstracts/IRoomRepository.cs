using Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Abstracts;

public interface IRoomRepository
{
    public Task ArchiveRoom(Room room);
    public Task CreateRoom(Room room);
    public Task DeleteRoom(Room room);
    Task<List<Room>> GetActiveRooms();
    Task<List<Room>> GetArchivedRooms();
    Task<Room> GetRoomById(int ıd);
    void UpdateRoom(Room room);
}
