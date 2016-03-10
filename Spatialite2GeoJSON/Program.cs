using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGdal;

namespace Spatialite2GeoJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            var sg = new Spatialite2GeoJson("YourDatabase.db");
            Console.WriteLine(sg.GetGeoJson("YourTableName"));
        }
    }
}
