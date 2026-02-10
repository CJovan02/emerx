namespace EMerx.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }

    /// <summary>
    /// It's used to connect database user to the firebase auth provider user.
    /// It's nullable because I want to create db user first and then auth user later.
    /// If auth user creation fails, it will leave the user with nullable firebaseUid, marking the User document unusable
    /// until auth user is created by calling the endpoint again.
    /// </summary>
    public string? FirebaseUid { get; set; }

    public Address? Address { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}