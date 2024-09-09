using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models.Entities;

public class Drawing
{
    public int Id { get; set; }
    public string RoomId { get; set; }
    public byte[] InkData { get; set; }
    public DateTime CreatedAt { get; set; }
}
