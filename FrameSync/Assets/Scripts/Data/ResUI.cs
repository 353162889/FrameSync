using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResUI
	{
		[ResCfgKey]
		public string name { get; private set; }
		public string prefab { get; private set; }
		public ResUI(SecurityElement node)
		{
			name = node.Attribute("name");
			prefab = node.Attribute("prefab");
		}
	}
}