using Api.DataAccess.Abstracts;
using Api.DataAccess.Data;
using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Concretes
{
    internal class RoomRepository : IRoomRepository
    {
        private readonly DataContext _context;

        public RoomRepository(DataContext context)
        {
            _context = context;
        }

        public async Task ArchiveRoom(Room room)
        {
            room.IsActive = true; 
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoom(Room room)
        {
             _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Room>> GetActiveRooms()
        {
            return await _context.Rooms.Where(r => r.IsActive).ToListAsync();
        }

        public async Task<List<Room>> GetArchivedRooms()
        {
            return await _context.Rooms.Where(r => r.IsArchived).ToListAsync();
                                
        }

        public async Task<Room> GetRoomById(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public void UpdateRoom(Room room)
        {
            _context.Rooms.Update(room);
        }
    }
}
