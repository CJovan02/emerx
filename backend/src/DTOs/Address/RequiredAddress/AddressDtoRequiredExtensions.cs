using EntityAddress = EMerx.Entities.Address;

namespace EMerx.DTOs.Address;

public static class AddressDtoRequiredExtensions
{
    public static AddressDto ToRequiredDto(this EntityAddress address)
    {
        return new AddressDto
        {
            Street = address.Street,
            City = address.City,
            HouseNumber = address.HouseNumber,
        };
    }

    public static EntityAddress ToEntity(this AddressRequiredDto dto)
    {
        return new EntityAddress
        {
            HouseNumber = dto.HouseNumber,
            Street = dto.Street,
            City = dto.City,
        };
    }
}