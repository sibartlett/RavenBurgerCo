using System.Dynamic;
using Geo.Geometries;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace RavenBurgerCo.Models
{
    public class Restaurant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public Point Location { get; set; }
        public string DriveThruArea { get; set; }
        public Polygon DeliveryArea { get; set; }
    }

    public class RestaurantResult
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public ExpandoObject Location { get; set; }
        public ExpandoObject DeliveryArea { get; set; }
    }
}