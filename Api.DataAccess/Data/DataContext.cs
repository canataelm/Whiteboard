using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Api.DataAccess.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }


    public DbSet<Room> Rooms { get; set; }
    public DbSet<Drawing> Drawing { get; set; }
}
