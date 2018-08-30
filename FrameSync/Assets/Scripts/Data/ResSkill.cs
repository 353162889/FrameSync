using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResSkill
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string name { get; private set; }
		public string type { get; private set; }
		public string target_type { get; private set; }
		public FP cd { get; private set; }
		public string logic_path { get; private set; }
		public int template_create_remote_id { get; private set; }
		public ResSkill(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			type = node.Attribute("type");
			target_type = node.Attribute("target_type");
			cd = FP.FromSourceLong(long.Parse(node.Attribute("cd")));
			logic_path = node.Attribute("logic_path");
			template_create_remote_id = int.Parse(node.Attribute("template_create_remote_id"));
		}
	}
}