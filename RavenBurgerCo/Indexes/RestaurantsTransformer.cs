using System.Linq;
using Raven.Client.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Indexes
{
    public class RestaurantsTransformer : AbstractTransformerCreationTask<Restaurant>
    {
        public RestaurantsTransformer()
        {
            TransformResults = restaurants =>    from restaurant in restaurants
                                                 let alias = LoadDocument<Restaurant>(restaurant.Id)
                                                 select new
                                                            {
                                                                alias.Name,
                                                                alias.Street,
                                                                alias.City,
                                                                alias.PostCode,
                                                                alias.Phone,
                                                                alias.Location,
                                                                alias.DeliveryArea
                                                            };
        }
    }
}