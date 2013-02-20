using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using RavenBurgerCo.Indexes;
using RavenBurgerCo.Models;
using RavenBurgerCo.Util;

namespace RavenBurgerCo.Controllers
{
    public class RestaurantsController : ApiController
    {
        public IEnumerable<object> Get(double latitude, double longitude)
        {
            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
                return session.Query<Restaurant, RestaurantIndex>()
                    .Customize(x =>
                                   {
                                       x.WithinRadiusOf(25, latitude, longitude);
                                       x.SortByDistance();
                                   })
                    .Take(250)
                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Street,
                                        x.City,
                                        x.PostCode,
                                        x.Phone,
                                        x.Latitude,
                                        x.Longitude
                                    })
                    .ToList();
            }
        }

        public IEnumerable<object> Get(double latitude, double longitude, bool delivery)
        {
            if (!delivery)
                return Get(latitude, longitude);

            var point = string.Format(CultureInfo.InvariantCulture, "POINT ({0} {1})", longitude, latitude);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
                return session.Query<Restaurant, RestaurantIndex>()
                    .Customize(x => x.RelatesToShape("delivery", point, SpatialRelation.Intersects))
                    // SpatialRelation.Contains is not supported
                    // SpatialRelation.Intersects is OK because we are using a point as the query parameter
                    .Take(250)
                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Street,
                                        x.City,
                                        x.PostCode,
                                        x.Phone,
                                        x.Latitude,
                                        x.Longitude,
                                        x.DeliveryArea
                                    })
                    .ToList();
            }
        }

        public IEnumerable<object> Get(double north, double east, double west, double south)
        {
            var rectangle = string.Format(CultureInfo.InvariantCulture, "{0:F6} {1:F6} {2:F6} {3:F6}", west, south, east, north);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
                return session.Query<Restaurant, RestaurantIndex>()
                    .Customize(x => x.RelatesToShape(Constants.DefaultSpatialFieldName, rectangle, SpatialRelation.Within))
                    .Take(512)
                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Street,
                                        x.City,
                                        x.PostCode,
                                        x.Phone,
                                        x.Latitude,
                                        x.Longitude
                                    })
                    .ToList();
            }
        }

        public IEnumerable<object> Get(string polyline)
        {
            var lineString = PolylineHelper.ConvertGooglePolylineToWkt(polyline);

            using (var session = MvcApplication.DocumentStore.OpenSession())
            {
                return session.Query<Restaurant, RestaurantIndex>()
                    .Customize(x => x.RelatesToShape("drivethru", lineString, SpatialRelation.Intersects))
                    .Take(512)
                    .Select(x => new
                                    {
                                        x.Name,
                                        x.Street,
                                        x.City,
                                        x.PostCode,
                                        x.Phone,
                                        x.Latitude,
                                        x.Longitude
                                    })
                    .ToList();
            }
        }
    }
}
