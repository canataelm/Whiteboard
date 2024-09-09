using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dtos;

public record RoomDto
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public bool IsArchived { get; set; }

    //public string Creator { get; set; }



}
