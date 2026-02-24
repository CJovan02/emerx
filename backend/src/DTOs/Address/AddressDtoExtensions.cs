using EntityAddress = EMerx.Entities.Address;

namespace EMerx.DTOs.Address;

public static class AddressDtoExtensions
{
    public static AddressDto? ToDto(this EntityAddress? address)
    {
        if (address is null)
            return null;
        
        return new AddressDto
        {
            Street = address.Street ?? null,
            City = address.City ?? null,
            HouseNumber = address.HouseNumber?? null,
        };
    }

    public static EntityAddress ToEntity(this AddressDto dto)
    {
        return new EntityAddress
        {
            HouseNumber = dto.HouseNumber,
            Street = dto.Street,
            City = dto.City,
        };
    }
}