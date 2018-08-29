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
		public string logic_path { get; private set; }
		public ResLevel(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			scene_id = int.Parse(node.Attribute("scene_id"));
			logic_path = node.Attribute("logic_path");
		}
	}
}