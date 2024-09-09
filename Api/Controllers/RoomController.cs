using Api.Business.Abstracts;
using Api.Business.Hubs;
using Api.Models.Entities;
using Common.Models.ReturnTypes.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

public class RoomController(IRoomService _roomService) : BaseApiController
{ 

    
  

    [HttpPost("create")]
    public async Task<IActionResult> CreateRoom(RoomDto roomDto)
    {
        var result = await _roomService.CreateRoom(roomDto);

        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);    
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinRoom(RoomDto roomDto)
    {
        var result = await _roomService.JoinRoom(roomDto);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    
    }


    [HttpGet("getRooms")]
    public async Task<IActionResult> GetAllRooms()
    {
        var result = await _roomService.GetActiveRooms();
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);   
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid room ID" });
        }
        var result = await _roomService.DeleteRoom(id);
        //if (result.IsSuccess)
            //return Ok(result);

        return Ok(result);
    }

    [HttpPut("archive")]  
    public async Task<IActionResult> ArchiveRoom(RoomDto roomDto)
    {
        var result = await _roomService.ArchiveRoom(roomDto);
        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }

    [HttpGet("archivedRooms")]
    public async Task<IActionResult> GetArchivedRooms()
    {
        var result = await _roomService.GetArchivedRooms();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpPut("unarchive")]
    public async Task<IActionResult> UnarchiveRoom(RoomDto roomDto)
    {
        var result = await _roomService.UnarchiveRoom(roomDto);
        if (result.IsSuccess)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }

    //[HttpGet("roomstate")]
    //public async Task<IActionResult> GetRoomState(RoomDto roomDto)
    //{
    //    var roomState = await _roomService.GetRoomState(roomDto);
    //    if (roomState.IsSuccess)
    //    {
    //        // Oda durumu başarıyla alındı, geri döndür
    //        return Ok(roomState);
    //    }
    //    else
    //    {
    //        // Oda durumu alınamadı, hata döndür
    //        return BadRequest(roomState);
    //    }
    //}

}
