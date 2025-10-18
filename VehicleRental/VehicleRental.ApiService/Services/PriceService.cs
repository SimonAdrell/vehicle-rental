using VehicleRental.Api.Models;

namespace VehicleRental.Api.Services;

public interface IPriceService
{
    Task<double> CalculateRentalPriceAsync(TypeOfVehicle typeOfVehicle, int rentalDays, double kilometersDriven);
}

public class PriceService : IPriceService
{
    public Task<double> CalculateRentalPriceAsync(TypeOfVehicle typeOfVehicle, int rentalDays, double kilometersDriven)
    {
        double timePrice = typeOfVehicle.PricePerDay * rentalDays * (typeOfVehicle.DayMultiplier ?? 1.0);
        double distancePrice = typeOfVehicle.PricePerKilometer * kilometersDriven * (typeOfVehicle.KilometerMultiplier ?? 1.0);
        return Task.FromResult(timePrice + distancePrice);
    }
}
