using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 5000;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    //public void CalculateRewards(User user)
    //{
    //    count++;
    //    List<VisitedLocation> userLocations = user.VisitedLocations;
    //    List<Attraction> attractions = _gpsUtil.GetAttractions();

    //    foreach (var visitedLocation in userLocations)
    //    {   
    //        var userRewardsCopy = user.UserRewards.ToList();

    //        foreach (var attraction in attractions)
    //        {
    //            if (!user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
    //            {
    //                if (NearAttraction(visitedLocation, attraction))
    //                {
    //                    lock (user.UserRewards)
    //                    {
    //                        user.AddUserReward(new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user)));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void CalculateRewards(User user)
    {
        count++;
        // Copie des lieux visités par l'utilisateur et copie de la liste des attractions
        var userLocationsCopy = user.VisitedLocations.ToList();
        var attractionsCopy = _gpsUtil.GetAttractions();

        foreach (var visitedLocation in userLocationsCopy)
        {
            foreach (var attraction in attractionsCopy)
            {
                // Création d'une copie des rewards de l'utilisateur
                var userRewardsCopy = user.UserRewards.ToList();
                if (!userRewardsCopy.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
                {
                    if (NearAttraction(visitedLocation, attraction))
                    {
                        user.AddUserReward(new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user)));
                    }
                }
            }
        }
    }


    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    private int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
