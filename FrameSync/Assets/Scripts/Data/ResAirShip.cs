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
		public string name { get; private set; }
		public string prefab { get; private set; }
		public FP hp { get; private set; }
		public FP attack { get; private set; }
		public FP move_speed { get; private set; }
		public List<int> skills { get; private set; }
		public string ai_path { get; private set; }
		public string die_effect { get; private set; }
		public ResAirShip(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			prefab = node.Attribute("prefab");
			hp = FP.FromSourceLong(long.Parse(node.Attribute("hp")));
			attack = FP.FromSourceLong(long.Parse(node.Attribute("attack")));
			move_speed = FP.FromSourceLong(long.Parse(node.Attribute("move_speed")));
			skills = new List<int>();
			string[] skillsArr = node.Attribute("skills").Split(',');
			if (skillsArr != null || skillsArr.Length > 0)
			{
				for (int i = 0; i < skillsArr.Length; i++)
				{
					skills.Add(int.Parse(skillsArr[i]));
				}
			}
			ai_path = node.Attribute("ai_path");
			die_effect = node.Attribute("die_effect");
		}
	}
}