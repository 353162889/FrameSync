using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResAirShip
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string prefab { get; private set; }
		public ResAirShip(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			prefab = node.Attribute("prefab");
		}
	}
}