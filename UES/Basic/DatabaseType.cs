using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES
{
    public abstract class DatabaseType
    {
        public abstract XmlDocument CastObjectToXml();
        public abstract string CastObjectToJson();
    }
}
