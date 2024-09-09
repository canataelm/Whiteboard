using Common.Models.Dtos;

namespace Common.Models.Response;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public object Content { get; set; }
    public List<string> Messages { get; set; } = new List<string>();
    public List<RoomDto> Data { get; set; }



}
