using GpsUtil.Location;
using System.Collections.Concurrent;
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

    public async Task CalculateRewards(User user)
    {
        var userLocations = user.VisitedLocations.ToList();
        var attractions = _gpsUtil.GetAttractions();
        var newUserRewards = new ConcurrentBag<UserReward>(); // Utilisation de ConcurrentBag

        var visitedLocationTasks = new List<Task>();
        foreach (var visitedLocation in userLocations)
        {
            visitedLocationTasks.Add(CalculateLocationRewards(user, newUserRewards, attractions, visitedLocation));
        }

        await Task.WhenAll(visitedLocationTasks);

        // Ajout des récompenses depuis le ConcurrentBag
        foreach (var userReward in newUserRewards)
        {
            user.AddUserReward(userReward);
        }
    }

    private Task CalculateLocationRewards(User user, ConcurrentBag<UserReward> newUserRewards,
        IEnumerable<Attraction> attractions, VisitedLocation visitedLocation)
    {
        return Task.Run(async () =>
        {
            var nearAttractions = attractions.Where(a => NearAttraction(visitedLocation, a));

            // Utilisation de tasks pour paralléliser le calcul des récompenses
            var rewardTasks = nearAttractions.Select(attraction => Task.Run(() =>
            {
                var points = GetRewardPoints(attraction, user);
                newUserRewards.Add(new UserReward(visitedLocation, attraction, points)); // Ajout au ConcurrentBag
            }));

            await Task.WhenAll(rewardTasks);
        });
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        var distance = GetDistance(attraction, visitedLocation.Location);
        Console.WriteLine($"Distance to {attraction.AttractionName}: {distance} (Buffer: {_proximityBuffer})");
        return distance <= _proximityBuffer;
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
