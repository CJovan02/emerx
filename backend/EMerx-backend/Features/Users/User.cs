using EMerx_backend.Entities;

namespace EMerx_backend.Features.Users;
public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    //temporarily will be hashed in prod
    public required string Password { get; set; }
    public Address? Address { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}