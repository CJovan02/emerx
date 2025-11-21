namespace EMerx.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    /// <summary>
    ///  It's used to connect database user to the firebase auth provider user
    /// </summary>
    public required string FirebaseUid { get; set; }
    public Address? Address { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}