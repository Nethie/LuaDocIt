using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace LuaDocIt
{
    class LuaFunction
    {
        public string name;
        public Dictionary<string, object> param = new Dictionary<string, object>();
        public LuaFunction( string _name, Dictionary<string, object> _param )
        {
            name = _name;
            param = _param;
        }

        public string GenerateJson()
        {
            var result = new JavaScriptSerializer().Serialize(this);
            return result;
        }
    }
}
