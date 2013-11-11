using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Geo.Geometries;
using Geo.IO.Wkt;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Spatial.Geo;
using RavenBurgerCo.Indexes;
using RavenBurgerCo.Models;

namespace RavenBurgerCo
{
    public class RavenConfig
    {
        public static void ConfigureRaven(MvcApplication application)
        {
            var store = new EmbeddableDocumentStore
                                {
                                    DataDirectory = "~/App_Data/Database",
                                    UseEmbeddedHttpServer = true
                                };

            store.Conventions.CustomizeJsonSerializer = x => x.Converters.Add(new GeoJsonConverter());
            store.Initialize();
            MvcApplication.DocumentStore = store;

            store.ExecuteIndex(new RestaurantIndex());
            store.ExecuteTransformer(new RestaurantsTransformer());

            var statistics = store.DatabaseCommands.GetStatistics();

            if (statistics.CountOfDocuments < 5)
                using (var bulkInsert = store.BulkInsert())
                    LoadRestaurants(application.Server.MapPath("~/App_Data/Restaurants.csv"), bulkInsert);
        }

        public static void LoadRestaurants(string csvFile, BulkInsertOperation bulkInsert)
        {
            var wktReader = new WktReader();
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader, new CsvConfiguration {UseInvariantCulture = true}))
            {
                var restaurantCsvRows = csv.GetRecords<RestaurantCsvRow>();
                foreach (var row in restaurantCsvRows)
                {
                    Polygon deliveryArea = null;

                    if (!string.IsNullOrEmpty(row.DeliveryArea))
                        deliveryArea = (Polygon)wktReader.Read(row.DeliveryArea);

                    var restaurant = new Restaurant
                    {
                        Name = row.Name,
                        Street = row.Street,
                        City = row.City,
                        PostCode = row.PostCode,
                        Phone = row.Phone,
                        Location = new Point(row.Latitude, row.Longitude),
                        DeliveryArea = deliveryArea,
                        DriveThruArea = string.IsNullOrEmpty(row.DriveThruArea) ? null : row.DriveThruArea
                    };

                    bulkInsert.Store(restaurant);

                }
            }
        }

        public class RestaurantCsvRow
        {
            public string Name { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public string Phone { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string DriveThruArea { get; set; }
            public string DeliveryArea { get; set; }
        }
    }
}