using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResAudio
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string path { get; private set; }
		public int priority { get; private set; }
		public string type { get; private set; }
		public bool loop { get; private set; }
		public ResAudio(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			path = node.Attribute("path");
			priority = int.Parse(node.Attribute("priority"));
			type = node.Attribute("type");
			loop = bool.Parse(node.Attribute("loop"));
		}
	}
}