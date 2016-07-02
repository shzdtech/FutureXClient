using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Micro.Future.Utility
{
    public class Config
    {
        private Dictionary<string, Dictionary<string, string>> content = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> Content
        {
            get { return content; }
            set { content = value; }
        }

        //support 2 types of comments as started with # and //
        public Config(string fileName)
        {
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith(@"//"))
                    {
                        //comments to ignore
                    }
                    else
                    {
                        if (line.Contains('|') && line.Contains('='))
                        {
                            int index1 = line.IndexOf('|');
                            string firstKey = line.Substring(0, index1).Trim();
                            if (!this.content.ContainsKey(firstKey))
                                this.content.Add(firstKey, new Dictionary<string, string>());
                            string subLine = line.Substring(index1 + 1);
                            int index2 = subLine.IndexOf('=');
                            this.content[firstKey].Add(subLine.Substring(0, index2).Trim(), subLine.Substring(index2 + 1).Trim());
                        }
                    }
                }
                file.Close();
            }
            catch (Exception e)
            {
                Logger.Error("Error on Parsing Config File: " + fileName + "\n\n" + e);
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(-1);
            }
        }
    }
}
