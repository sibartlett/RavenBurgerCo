using System.Linq;
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
									restaurant.Location,
									restaurant.DeliveryArea,
									restaurant.DriveThruArea
                                };

			Spatial(x => x.Location, x => x.Geography.Default());
			Spatial(x => x.DeliveryArea, x => x.Geography.GeohashPrefixTreeIndex(7));
			Spatial(x => x.DriveThruArea, x => x.Geography.Default());
        }
    }
}