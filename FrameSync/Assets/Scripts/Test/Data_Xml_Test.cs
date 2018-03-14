using System;
using System.Security;
using Framework;
using System.Collections.Generic;

namespace GameData
{
    public class Data_Xml_Test
    {
        public int tt { get; private set; }
        public FP aa { get; private set; }
        public string cc { get; private set; }

        public List<int> bb { get; private set; }
        public Data_Xml_Test(SecurityElement element)
        {
            SecurityElement config = element.Children[0] as SecurityElement;
            foreach (SecurityElement node in config.Children)
            {
                tt = int.Parse(node.Attribute("tt"));
                aa = FP.FromSourceLong(long.Parse(node.Attribute("aa")));
                cc = node.Attribute("cc");
                bb = new List<int>();
                string[] bbArr = node.Attribute("bb").Split(',');
                if (bbArr != null || bbArr.Length > 0)
                {
                    for (int i = 0; i < bbArr.Length; i++)
                    {
                        bb.Add(int.Parse(bbArr[i]));
                    }
                }
            }
        }
    }
}
