using Api.Models.Entities;
using Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Business.Tools;

public static class Mapper
{
    internal static List<RoomDto> Map(IEnumerable<Room> activeRooms)
    {
       List<RoomDto> roomDtos = new List<RoomDto>();

        foreach (var room in activeRooms)
        {
            roomDtos.Add(new RoomDto 
            {
                Name = room.Name, 
                Id = room.Id,
                IsArchived = room.IsArchived,
            });
        }
      
        return roomDtos;
    }

    internal static Room Map(RoomDto roomDto)
    {
        return new Room()
        {
            Name = roomDto.Name,
            Capacity = 10,
            IsActive = true
        };
    }
}
