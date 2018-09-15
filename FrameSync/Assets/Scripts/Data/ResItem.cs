using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResItem
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public string prefab { get; private set; }
		public FP radius { get; private set; }
		public FP move_speed { get; private set; }
		public string ai_path { get; private set; }
		public FP hp { get; private set; }
		public int airship { get; private set; }
		public int skill1 { get; private set; }
		public int skill2 { get; private set; }
		public ResItem(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			prefab = node.Attribute("prefab");
			radius = FP.FromSourceLong(long.Parse(node.Attribute("radius")));
			move_speed = FP.FromSourceLong(long.Parse(node.Attribute("move_speed")));
			ai_path = node.Attribute("ai_path");
			hp = FP.FromSourceLong(long.Parse(node.Attribute("hp")));
			airship = int.Parse(node.Attribute("airship"));
			skill1 = int.Parse(node.Attribute("skill1"));
			skill2 = int.Parse(node.Attribute("skill2"));
		}
	}
}