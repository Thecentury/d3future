using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

using Dict = System.Collections.Generic.Dictionary<Microsoft.Research.DynamicDataDisplay.Charts.Maps.TileIndex, long>;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers;
using System.IO.Packaging;
using System.Net.Mime;

namespace ZipSample
{
	class Program
	{
		static void Main(string[] args)
		{
			//string tilesPath = @"C:\Tiles 1931\Layer_NewLayer";
			//var server = new ZipFileTileServer();
			//server.CreatePackage(tilesPath, @"C:\tiles1.zip");

			//using (Package package = Package.Open(@"C:\tiles.zip"))
			//{
			//    Uri cacheUri = PackUriHelper.CreatePartUri(new Uri("cache.dat", UriKind.Relative));
			//    package.DeletePart(cacheUri);
			//}
			//return;

			//ZipFileTileServer server = new ZipFileTileServer(@"C:\tiles.zip");

			//return;

			VEPathProvider pr = new VEPathProvider();
			var tiles = pr.GetTiles(@"C:\Tiles 1931\Layer_NewLayer", ".png");

			ReadonlyTileCache cache = new ReadonlyTileCache();
			foreach (var tile in tiles)
			{
				cache.Add(tile.ID, false);
			}
			cache.CalcMinMaxLevels();

			return;
			string cacheName = "cache.dat";
			using (Package package = Package.Open(@"C:\tiles.zip"))
			{
				Uri cacheUri = PackUriHelper.CreatePartUri(new Uri(cacheName, UriKind.Relative));
				PackagePart part = package.CreatePart(cacheUri, MediaTypeNames.Application.Octet, CompressionOption.Fast);

				BinaryFormatter formatter = new BinaryFormatter();
				using (Stream stream = part.GetStream())
				{
					formatter.Serialize(stream, cache);
				}

				package.CreateRelationship(cacheUri, TargetMode.Internal, "http://research.microsoft.com/DynamicDataDisplay/1.0");
			}

			return;

			//Dictionary<TileIndex, long> cache = new Dictionary<TileIndex, long>();
			//cache.Add(new TileIndex(0, 0, 1), 100);
			//cache.Add(new TileIndex(0, 1, 1), 101);
			//cache.Add(new TileIndex(1, 0, 1), 102);
			//cache.Add(new TileIndex(1, 1, 1), 103);
			//cache.Add(new TileIndex(0, 0, 2), 104);
			//cache.Add(new TileIndex(0, 1, 2), 105);
			//cache.Add(new TileIndex(1, 0, 2), 106);
			//cache.Add(new TileIndex(1, 1, 2), 106);

			//ReadonlyTileCache c = new ReadonlyTileCache(cache);

			//object d;
			//using (FileStream fs = new FileStream("1.txt", FileMode.OpenOrCreate))
			//{
			//    BinaryFormatter formatter = new BinaryFormatter();
			//    //formatter.Serialize(fs, c);
			//    d = formatter.Deserialize(fs);
			//    ((ReadonlyTileCache)d).CalcMinMaxLevels();
			//}
		}
	}
}
