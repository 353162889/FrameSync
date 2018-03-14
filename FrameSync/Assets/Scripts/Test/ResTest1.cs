using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResTest1
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string desc { get; private set; }
		public FP point { get; private set; }
		public List<int> test_repeated_int { get; private set; }
		public List<FP> test_repeated_float { get; private set; }
		public List<string> test_repeated_string { get; private set; }
		public ResTest1(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			desc = node.Attribute("desc");
			point = FP.FromSourceLong(long.Parse(node.Attribute("point")));
			test_repeated_int = new List<int>();
			string[] test_repeated_intArr = node.Attribute("test_repeated_int").Split(',');
			if (test_repeated_intArr != null || test_repeated_intArr.Length > 0)
			{
				for (int i = 0; i < test_repeated_intArr.Length; i++)
				{
					test_repeated_int.Add(int.Parse(test_repeated_intArr[i]));
				}
			}
			test_repeated_float = new List<FP>();
			string[] test_repeated_floatArr = node.Attribute("test_repeated_float").Split(',');
			if (test_repeated_floatArr != null || test_repeated_floatArr.Length > 0)
			{
				for (int i = 0; i < test_repeated_floatArr.Length; i++)
				{
					test_repeated_float.Add(FP.FromSourceLong(long.Parse(test_repeated_floatArr[i])));
				}
			}
			test_repeated_string = new List<string>();
			string[] test_repeated_stringArr = node.Attribute("test_repeated_string").Split(',');
			if (test_repeated_stringArr != null || test_repeated_stringArr.Length > 0)
			{
				for (int i = 0; i < test_repeated_stringArr.Length; i++)
				{
					test_repeated_string.Add(test_repeated_stringArr[i]);
				}
			}
		}
	}
}