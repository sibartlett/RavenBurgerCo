using System.IO;
using CsvHelper;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
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

            store.Initialize();
            MvcApplication.DocumentStore = store;

            IndexCreation.CreateIndexes(typeof(MvcApplication).Assembly, store);

            var statistics = store.DatabaseCommands.GetStatistics();

            if (statistics.CountOfDocuments < 5)
                using (var bulkInsert = store.BulkInsert())
                    LoadRestaurants(application.Server.MapPath("~/App_Data/Restaurants.csv"), bulkInsert);
        }

        public static void LoadRestaurants(string csvFile, BulkInsertOperation bulkInsert)
        {
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader))
            {
                var restaurants = csv.GetRecords<Restaurant>();
                foreach (var restaurant in restaurants)
                {
                    if (string.IsNullOrEmpty(restaurant.DeliveryArea))
                        restaurant.DeliveryArea = null;

                    if (string.IsNullOrEmpty(restaurant.DriveThruArea))
                        restaurant.DriveThruArea = null;

                    bulkInsert.Store(restaurant);
                }
            }
        }
    }
}