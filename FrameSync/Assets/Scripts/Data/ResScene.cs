using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResScene
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public FP view_height { get; private set; }
		public FP view_width { get; private set; }
		public ResScene(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			view_height = FP.FromSourceLong(long.Parse(node.Attribute("view_height")));
			view_width = FP.FromSourceLong(long.Parse(node.Attribute("view_width")));
		}
	}
}