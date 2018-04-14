using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResConst
	{
		[ResCfgKey]
		public string key { get; private set; }
		public int p_int { get; private set; }
		public string p_string { get; private set; }
		public ResConst(SecurityElement node)
		{
			key = node.Attribute("key");
			p_int = int.Parse(node.Attribute("p_int"));
			p_string = node.Attribute("p_string");
		}
	}
}