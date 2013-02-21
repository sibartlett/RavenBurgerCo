using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    public class RestaurantIndex : AbstractIndexCreationTask<Restaurant>
    {
        public RestaurantIndex()
        {
            Map = restaurants => from restaurant in restaurants
                                 select new
                                {
                                    _ = SpatialGenerate(restaurant.Latitude, restaurant.Longitude),
                                    __ = SpatialGenerate("delivery", restaurant.DeliveryArea, SpatialSearchStrategy.GeohashPrefixTree, 7),
                                    ___ = SpatialGenerate("drivethru", restaurant.DriveThruArea)
                                };
        }
    }
}