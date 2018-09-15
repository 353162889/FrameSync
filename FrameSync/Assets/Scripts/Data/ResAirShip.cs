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
		public List<int> active_skills { get; private set; }
		public List<int> passive_skills { get; private set; }
		public string ai_path { get; private set; }
		public FP ai_attack_time { get; private set; }
		public string die_effect { get; private set; }
		public int die_audio { get; private set; }
		public List<int> die_drop_ids { get; private set; }
		public List<FP> die_drop_weights { get; private set; }
		public FP die_drop_rate { get; private set; }
		public ResAirShip(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			name = node.Attribute("name");
			prefab = node.Attribute("prefab");
			hp = FP.FromSourceLong(long.Parse(node.Attribute("hp")));
			attack = FP.FromSourceLong(long.Parse(node.Attribute("attack")));
			move_speed = FP.FromSourceLong(long.Parse(node.Attribute("move_speed")));
			active_skills = new List<int>();
			string str_active_skills = node.Attribute("active_skills");
			if(!string.IsNullOrEmpty(str_active_skills))
			{
				string[] active_skillsArr = str_active_skills.Split(',');
				if (active_skillsArr != null || active_skillsArr.Length > 0)
				{
					for (int i = 0; i < active_skillsArr.Length; i++)
					{
						active_skills.Add(int.Parse(active_skillsArr[i]));
					}
				}
			}
			passive_skills = new List<int>();
			string str_passive_skills = node.Attribute("passive_skills");
			if(!string.IsNullOrEmpty(str_passive_skills))
			{
				string[] passive_skillsArr = str_passive_skills.Split(',');
				if (passive_skillsArr != null || passive_skillsArr.Length > 0)
				{
					for (int i = 0; i < passive_skillsArr.Length; i++)
					{
						passive_skills.Add(int.Parse(passive_skillsArr[i]));
					}
				}
			}
			ai_path = node.Attribute("ai_path");
			ai_attack_time = FP.FromSourceLong(long.Parse(node.Attribute("ai_attack_time")));
			die_effect = node.Attribute("die_effect");
			die_audio = int.Parse(node.Attribute("die_audio"));
			die_drop_ids = new List<int>();
			string str_die_drop_ids = node.Attribute("die_drop_ids");
			if(!string.IsNullOrEmpty(str_die_drop_ids))
			{
				string[] die_drop_idsArr = str_die_drop_ids.Split(',');
				if (die_drop_idsArr != null || die_drop_idsArr.Length > 0)
				{
					for (int i = 0; i < die_drop_idsArr.Length; i++)
					{
						die_drop_ids.Add(int.Parse(die_drop_idsArr[i]));
					}
				}
			}
			die_drop_weights = new List<FP>();
			string str_die_drop_weights = node.Attribute("die_drop_weights");
			if(!string.IsNullOrEmpty(str_die_drop_weights))
			{
				string[] die_drop_weightsArr = str_die_drop_weights.Split(',');
				if (die_drop_weightsArr != null || die_drop_weightsArr.Length > 0)
				{
					for (int i = 0; i < die_drop_weightsArr.Length; i++)
					{
						die_drop_weights.Add(FP.FromSourceLong(long.Parse(die_drop_weightsArr[i])));
					}
				}
			}
			die_drop_rate = FP.FromSourceLong(long.Parse(node.Attribute("die_drop_rate")));
		}
	}
}