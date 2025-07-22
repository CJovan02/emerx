namespace EMerx_backend.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public Address? Address { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}