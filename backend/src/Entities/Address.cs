namespace EMerx.Entities;

// Change to record
public record Address
{
    public string City { get; init; }
    public string Street { get; init; }
    public string HouseNumber { get; init; }
}