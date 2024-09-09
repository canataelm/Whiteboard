namespace Api.Models.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }

    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }

}
