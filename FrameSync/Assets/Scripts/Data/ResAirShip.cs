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
		public List<int> skills { get; private set; }
		public int ai { get; private set; }
		public ResAirShip(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			prefab = node.Attribute("prefab");
			skills = new List<int>();
			string[] skillsArr = node.Attribute("skills").Split(',');
			if (skillsArr != null || skillsArr.Length > 0)
			{
				for (int i = 0; i < skillsArr.Length; i++)
				{
					skills.Add(int.Parse(skillsArr[i]));
				}
			}
			ai = int.Parse(node.Attribute("ai"));
		}
	}
}