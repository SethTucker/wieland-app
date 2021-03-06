﻿using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace wieland_app
{
	public static class SerializeCsv
	{
		const string dataDir = @"D:\zCsharp\New Wieland\";



		public static void SerializeBacklogCsv(string path, DateTime blCreationDate)
		{
			TextReader textReader = File.OpenText(path);
			var csv = new CsvReader(textReader);
			List<CsvBacklogOrder> backlog = csv.GetRecords<CsvBacklogOrder>().ToList();

			string binPath = $"{dataDir}BL-{blCreationDate.Month}-{blCreationDate.Day}-{blCreationDate.Year}.bin";
			CreateBinFile(binPath, backlog);
		}



		public static void SerializeDefaultVinylList()
		{
			List<CsvVinyl> vinylCsv = OpenBinFile<CsvVinyl>("vinyl_csv.bin");
			List<CsvBasePart> basePartCsv = OpenBinFile<CsvBasePart>("base_part_csv.bin");
		    List<CsvStool> stoolCsv = OpenBinFile<CsvStool>("medical_stools_csv.bin");
			List<Vinyl> defaultVinylList = new List<Vinyl>();

			foreach (var vinyl in vinylCsv)
			{
				defaultVinylList.Add(new Vinyl(vinyl.Color, vinyl.CustomerFabricNumber, vinyl.WielandNum));				
			}

			// a bit inefficient, could make serialized Parts list and then just add the list to Vinyl obj with custnum maybe?
			foreach (var vinyl in defaultVinylList)
			{
				foreach (var basePart in basePartCsv)
				{
					float basePartLinYards = 0;
					if (!String.IsNullOrEmpty(basePart.LY))
						basePartLinYards = Convert.ToSingle(basePart.LY);

					vinyl.VinylParts.Add(new Part(basePart.PartNumber + "-" + vinyl.CustomerFabricNumber, basePartLinYards));
				}

				foreach (var stool in stoolCsv)
				{
				    if (stool.Fabric == vinyl.WielandFabricNumber)
				    {
				        vinyl.VinylParts.Add(new Part(stool.PartNum, stool.Yards));
				    }
				}
			}

			string binPath = $"{dataDir}default_vinyl_list.bin";
			CreateBinFile(binPath, defaultVinylList);
		}

		public static void SerializeVinylCsv()
		{
			string path = dataDir + "vinyl_csv.csv";

			TextReader textReader = File.OpenText(path);
			var csv = new CsvReader(textReader);
			List<CsvVinyl> vinylList = csv.GetRecords<CsvVinyl>().ToList();

			string binPath = dataDir + "vinyl_csv.bin";
			CreateBinFile(binPath, vinylList);
		}

		public static void SerializeBasePartCsv()
		{
			string path = dataDir + "base_part_csv.csv";

			TextReader textReader = File.OpenText(path);
			var csv = new CsvReader(textReader);
			List<CsvBasePart> basePartList = csv.GetRecords<CsvBasePart>().ToList();

			string binPath = dataDir + "base_part_csv.bin";
			CreateBinFile(binPath, basePartList);
		}

		public static void SerializeStoolCsv()
		{
			string path = dataDir + "medical_stools_csv.csv";

			TextReader textReader = File.OpenText(path);
			var csv = new CsvReader(textReader);
			List<CsvStool> stoolList = csv.GetRecords<CsvStool>().ToList();

			string binPath = dataDir + "medical_stools_csv.bin";
			CreateBinFile(binPath, stoolList);
		}



		static void CreateBinFile<T>(string path, List<T> list)
		{
			try
			{
				using (Stream stream = File.Open(path, FileMode.Create))
				{
					var bin = new BinaryFormatter();
					bin.Serialize(stream, list);
				}
			}
			catch (IOException)
			{
			}
		}

		public static List<T> OpenBinFile<T>(string fileName)
		{
			try
			{
				string path = dataDir + fileName;
				using (Stream stream = File.Open(path, FileMode.Open))
				{
					var bin = new BinaryFormatter();
					var list = (List<T>)bin.Deserialize(stream);
					return list;
				}
			}
			catch (IOException)
			{
				return new List<T>();
			}
		}
	}
}
