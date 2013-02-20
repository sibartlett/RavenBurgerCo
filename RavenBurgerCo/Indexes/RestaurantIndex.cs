using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    // The index should look like this, but there is bug in the current version of RavenDB
    //public class RestaurantIndex : AbstractIndexCreationTask<Restaurant>
    //{
    //    public RestaurantIndex()
    //    {
    //        Map = restaurants => from restaurant in restaurants
    //                             select new
    //                            {
    //                                _ = SpatialGenerate(restaurant.Latitude, restaurant.Longitude),
    //                                __ = SpatialGenerate("delivery", restaurant.DeliveryArea, SpatialSearchStrategy.GeohashPrefixTree, 7),
    //                                ___ = SpatialGenerate("drivethru", restaurant.DriveThruArea)
    //                            };
    //    }
    //}

    public class RestaurantIndex : AbstractIndexCreationTask<Restaurant>
    {
        public RestaurantIndex()
        {
            Map = restaurants => from restaurant in restaurants
                                 select new
                                {
                                    _ = SpatialGenerate(restaurant.Latitude, restaurant.Longitude),

                                    __ = restaurant.DeliveryArea == null ? new object[0] :
                                        SpatialGenerate("delivery", restaurant.DeliveryArea, SpatialSearchStrategy.GeohashPrefixTree, 7),

                                    ___ = restaurant.DriveThruArea == null ? new object[0] :
                                        SpatialGenerate("drivethru", restaurant.DriveThruArea)
                                };
        }
    }
}