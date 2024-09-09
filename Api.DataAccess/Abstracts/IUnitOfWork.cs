using Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Abstracts;

public interface IUnitOfWork
{
    public IRoomRepository RoomRepository { get; }
    public IDrawingRepository<Drawing> Drawing {  get; }
    Task Complete();
    Task<int> SaveChangesAsync();
}
