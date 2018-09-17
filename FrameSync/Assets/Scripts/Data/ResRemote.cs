using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResRemote
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public string target_type { get; private set; }
		public FP move_speed { get; private set; }
		public FP max_move_distance { get; private set; }
		public string effect_name { get; private set; }
		public string logic_path { get; private set; }
		public ResRemote(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			target_type = node.Attribute("target_type");
			move_speed = FP.FromSourceLong(long.Parse(node.Attribute("move_speed")));
			max_move_distance = FP.FromSourceLong(long.Parse(node.Attribute("max_move_distance")));
			effect_name = node.Attribute("effect_name");
			logic_path = node.Attribute("logic_path");
		}
	}
}