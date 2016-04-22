using Google.ProtocolBuffers;
using Google.ProtocolBuffers.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message.PBMessageHandler
{
    public class PBUtility
    {

        public static IDictionary<string,string> ParseProperties(IDictionary<FieldDescriptor, object> fields, string codename)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (var fd in fields)
            {
                if (fd.Key.GetType().ToString() == "Google.ProtocolBuffers.ByteString")
                {
                    string decodedFieldValue = ((ByteString)fd.Value).ToString(Encoding.GetEncoding(codename));
                    ret[fd.Key.Name] = decodedFieldValue;
                }
                else
                {
                    ret[fd.Key.Name] = fd.Value.ToString();
                }
            }

            return ret;

        }
    }
}
