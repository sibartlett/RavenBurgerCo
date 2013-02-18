using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    public class DriveThruIndex : AbstractIndexCreationTask<Restaurant>
    {
        public DriveThruIndex()
        {
            Map = restaurants => from restaurant in restaurants
                                 where restaurant.DriveThruArea != null
                                 select new
                                            {
                                                restaurant.Name,
                                                _ = SpatialGenerate("drivethru", restaurant.DriveThruArea)
                                            };
        }
    }
}