﻿using Microsoft.Xna.Framework;
using SimpleSprinkler;
using Storm.ExternalEvent;
using Storm.StardewValley.Event;
using Storm.StardewValley.Wrapper;
using System;
using System.Reflection;

namespace SimpleSprinkler_STORM
{
	[Mod]
	internal class STORMMod : DiskResource
	{
		private SimpleConfig Config
		{
			get
			{
				return SimpleConfig.Instance;
			}
		}

		private SimpleSprinklerMod mod;
		private GameLocation location;

		[Subscribe]
		public void InitializeCallback(InitializeEvent @event)
		{
			SetUpEmbededAssemblyResolving();
			SimpleSprinklerMod.Log("STORM Loaded");
			mod = new SimpleSprinklerMod();
		}

		[Subscribe]
		public void WarpFarmerCallback(WarpFarmerEvent @event)
		{
			location = @event.Location;
			foreach (var obj in location.Objects.Values)
			{
				//Does not work yet, because STORM needs "parentSheetIndex" property
				//Workaround using NAME
				SimpleSprinklerMod.Log("Trying to sprinkler with {0}", obj.Name);
				mod.CalculateSimpleSprinkler(obj.Name, obj.TileLocation, SetWatered);
			}
		}

		public void SetWatered(Vector2 position)
		{
			if (location.TerrainFeatures.ContainsKey(position) && location.TerrainFeatures[position] is HoeDirt)
			{
				(location.TerrainFeatures[position] as HoeDirt).State = 1;
			}
		}

		private void SetUpEmbededAssemblyResolving()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				string resourceName = new AssemblyName(args.Name).Name + ".dll";
				string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
				{
					Byte[] assemblyData = new Byte[stream.Length];
					stream.Read(assemblyData, 0, assemblyData.Length);
					return Assembly.Load(assemblyData);
				}
			};
		}
	}
}