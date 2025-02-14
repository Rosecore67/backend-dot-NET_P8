using GpsUtil.Location;
using TripPricer;

namespace TourGuide.Users;

public class User
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTime LatestLocationTimestamp { get; set; }
    public List<VisitedLocation> VisitedLocations { get; } = new List<VisitedLocation>();
    public List<UserReward> UserRewards { get; } = new List<UserReward>();
    public UserPreferences UserPreferences { get; set; } = new UserPreferences();
    public List<Provider> TripDeals { get; set; } = new List<Provider>();
    private readonly object _visitedLocationsLock = new object();
    private readonly object _userRewardsLock = new object();

    public User(Guid userId, string userName, string phoneNumber, string emailAddress)
    {
        UserId = userId;
        UserName = userName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        lock (_visitedLocationsLock) // Verrouille les accès à VisitedLocations pour garantir la sécurité des threads
        {
            VisitedLocations.Add(visitedLocation);
        }
    }

    public void ClearVisitedLocations()
    {
        lock (_visitedLocationsLock) // Verrouille les accès à VisitedLocations pour garantir la sécurité des threads
        {
            VisitedLocations.Clear();
        }
    }

    public void AddUserReward(UserReward userReward)
    {
        lock (_userRewardsLock) // Verrouille les accès à UserRewards pour garantir la sécurité des threads
        {
            if (!UserRewards.Any(r => r.Attraction.AttractionName == userReward.Attraction.AttractionName))
            {
                UserRewards.Add(userReward);
            }
        }
    }

    public VisitedLocation GetLastVisitedLocation()
    {
        lock (_visitedLocationsLock) // Verrouille les accès à VisitedLocations pour garantir la sécurité des threads
        {
            return VisitedLocations[^1];
        }
    }
}
