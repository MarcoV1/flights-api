using System.ComponentModel;

namespace angular_asp.Dtos
{
    public record FlightSearchParameters(

        [DefaultValue("01/09/2023 10:30:00 AM")]
        DateTime? FromDate,
        [DefaultValue("02/09/2023 10:30:00 AM")]
        DateTime? ToDate,
        [DefaultValue("Los Angeles")]
        string? From,
        [DefaultValue("Berlin")]
        string? Destination,
        [DefaultValue(1)]
        int? NumberOfPassengers    
    );
}
