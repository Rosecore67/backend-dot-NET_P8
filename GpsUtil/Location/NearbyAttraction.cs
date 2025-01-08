namespace GpsUtil.Location;

public class NearbyAttraction
{
    public string AttractionName { get; set; }
    public double AttractionLattitude { get; set; }
    public double AttractionLongitude { get; set; }
    public double UserLattitude { get; set; }
    public double UserLongitude { get; set; }
    public double Distance { get; set; }
    public int Reward { get; set; }

}
