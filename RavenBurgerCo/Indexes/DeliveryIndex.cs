using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    public class DeliveryIndex : AbstractIndexCreationTask<Restaurant>
    {
        public DeliveryIndex()
        {
            Map = restaurants => from restaurant in restaurants
                                 where restaurant.DeliveryArea != null
                                 select new
                                            {
                                                restaurant.Name,
                                                _ = SpatialGenerate("delivery", restaurant.DeliveryArea, SpatialSearchStrategy.GeohashPrefixTree, 7)
                                            };
        }
    }
}