using VehicleRental.Api.Models;

namespace VehicleRental.Api.Services;

public interface IPriceService
{
    double CalculateRentalPrice(VehicleTypeDto typeOfVehicle, int rentalDays, double kilometersDriven);
}

public class PriceService : IPriceService
{
    private const double DefaultMultiplier = 1.0;
    private const int MinimumRentalDays = 1;

    public double CalculateRentalPrice(VehicleTypeDto typeOfVehicle, int rentalDays, double kilometersDriven)
    {
        double timePrice = CalculateTimePrice(typeOfVehicle.PricePerDay, rentalDays, typeOfVehicle.DayMultiplier);
        double distancePrice = CalculateDistancePrice(typeOfVehicle, kilometersDriven);

        return timePrice + distancePrice;
    }

    private static double CalculateDistancePrice(VehicleTypeDto typeOfVehicle, double kilometersDriven)
    {
        if (typeOfVehicle.PricePerKilometer is not double pricePerKilometer)
        {
            return 0;
        }

        return pricePerKilometer
            * kilometersDriven
            * (typeOfVehicle.KilometerMultiplier ?? DefaultMultiplier);
    }

    private static double CalculateTimePrice(double pricePerDay, int rentalDays, double? dayMultiplier)
    {
        rentalDays = rentalDays <= 0 ? MinimumRentalDays : rentalDays;
        return pricePerDay * rentalDays * (dayMultiplier ?? DefaultMultiplier);
    }
}
