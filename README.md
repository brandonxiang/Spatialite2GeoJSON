Spatialite 转换 GeoJSON 用C#
-----------------------------
由于C#在安装GDAL上存在问题，而又不想使用NetTopologySuite如此大型的库，希望在此写一个例子实现spatialite到GeoJSON的转换。

此例子运用了以下第三方包：

- System.Data.SQLite
- GeoJSON.Net
- SpatialiteSharp

#####使用：

```
var sg = new Spatialite2GeoJson("YourDatabase.db");
Console.WriteLine(sg.GetGeoJson("YourTableName"));
```

Spatialite Convert to GeoJSON by C#
-----------------------------

Because it comes a problem in the installation of GDAL on .net platform, and the heavy package like NetTopologySuite is not ready to use in the small project, a small sample is presented as a simple convertor from spatialite to GeoJSON. 

This sample is powered by some packages:

- System.Data.SQLite
- GeoJSON.Net
- SpatialiteSharp

#####example：

```
var sg = new Spatialite2GeoJson("YourDatabase.db");
Console.WriteLine(sg.GetGeoJson("YourTableName"));
```
