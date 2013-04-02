using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Geo.Geometries;
using Geo.IO.Google;
using System.Web.Http;
using RavenBurgerCo.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo.Controllers
{
    public class RestaurantsController : ApiController
    {
        public IEnumerable<object> Get(double latitude, double longitude)
        {
            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
				return session.Query<Restaurant, RestaurantIndex>()
					.Spatial(x => x.Location, x => x.WithinRadiusOf(25, longitude, latitude))
					.TransformWith<RestaurantsTransformer, RestaurantResult>()
                    .Take(250)
					.ToList();
            }
        }

        public IEnumerable<object> Get(double latitude, double longitude, bool delivery)
        {
            if (!delivery)
                return Get(latitude, longitude);

            var point = new Point(latitude, longitude);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
				return session.Query<Restaurant, RestaurantIndex>()
					.Spatial(x => x.DeliveryArea, x => x.Intersects(point))
					.TransformWith<RestaurantsTransformer, RestaurantResult>()
                    .Take(250)
                    .ToList();
            }
        }

        public IEnumerable<object> Get(double north, double east, double west, double south)
        {
            var rectangle = string.Format(CultureInfo.InvariantCulture, "BOX ({0:F6} {1:F6}, {2:F6} {3:F6})", west, south, east, north);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
				var aaa = session.Query<Restaurant, RestaurantIndex>()
					.Spatial(x => x.Location, x => x.Within(rectangle))
					.TransformWith<RestaurantsTransformer, RestaurantResult>()
					.Take(512)
                    .ToList();
	            return aaa;
            }
        }

        public IEnumerable<object> Get(string polyline)
        {
			var lineString = new GooglePolylineEncoder().Decode(polyline);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
				return session.Query<Restaurant, RestaurantIndex>()
					.Spatial(x => x.DriveThruArea, x => x.Within(lineString))
					.TransformWith<RestaurantsTransformer, RestaurantResult>()
                    .Take(512)
                    .ToList();
            }
        }
    }
}
