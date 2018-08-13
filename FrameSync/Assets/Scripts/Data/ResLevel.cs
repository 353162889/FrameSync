using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResLevel
	{
		[ResCfgKey]
		public int id { get; private set; }
		public int scene_id { get; private set; }
		public int gaming_id { get; private set; }
		public ResLevel(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			scene_id = int.Parse(node.Attribute("scene_id"));
			gaming_id = int.Parse(node.Attribute("gaming_id"));
		}
	}
}