using System.Linq;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    public class LocationIndex : AbstractIndexCreationTask<Restaurant>
    {
        public LocationIndex()
        {
            Map = restaurants => from restaurant in restaurants
                                 select new
                                            {
                                                restaurant.Name,
                                                _ = SpatialGenerate(restaurant.Latitude, restaurant.Longitude),
                                                __ = SpatialGenerate("location", restaurant.LocationWkt)
                                            };
        }
    }
}