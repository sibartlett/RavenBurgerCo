using System;
using System.Collections.Generic;
using System.Linq;

namespace RavenBurgerCo.Util
{
    public class PolylineHelper
    {
        public static string ConvertGooglePolylineToWkt(string polyline)
        {
            return GetWkt(ParseGoogleEncodedPolyline(polyline));
        }

        public static string GetWkt(List<GeoPoint> polyline)
        {
            var points = polyline.Select(x => x.Longitude.ToString(MvcApplication.NFI) + " " + x.Latitude.ToString(MvcApplication.NFI)).ToArray();
            return "LINESTRING (" + string.Join(", ", points) + ")";
        }

        public static List<GeoPoint> ParseGoogleEncodedPolyline(String encoded)
        {

            var poly = new List<GeoPoint>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                var dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                var p = new GeoPoint(lat / 1E5, lng / 1E5);
                poly.Add(p);
            }

            return poly;
        }

        public class GeoPoint
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public GeoPoint(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }
    }
}