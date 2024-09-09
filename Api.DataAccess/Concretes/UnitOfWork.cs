using Api.DataAccess.Abstracts;
using Api.DataAccess.Data;
using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Concretes;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private IDrawingRepository<Drawing> _drawing;
   

    public UnitOfWork(DataContext dbContext)
    {
        _context = dbContext;
        RoomRepository = new RoomRepository(_context);
    }

    public IRoomRepository RoomRepository { get; }

    public IDrawingRepository<Drawing> Drawing => _drawing ??= new DrawingRepository<Drawing>(_context);

    public Task Complete()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
