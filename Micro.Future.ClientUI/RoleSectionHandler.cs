using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Micro.Future.Configuration
{
    public class RoleSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            Dictionary<string, IList<string>> ret = new Dictionary<string, IList<string>>();

            foreach (XmlNode role in section.ChildNodes)
            {
                var roleType = role.Attributes["type"].Value;
                var frameList = new List<string>();
                foreach(XmlNode frame in role.ChildNodes)
                {
                    frameList.Add(frame.InnerText.Trim());
                }
                ret[roleType] = frameList;
            }

            return ret;
        }
    }
}
