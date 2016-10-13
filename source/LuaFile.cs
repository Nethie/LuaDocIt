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
    class LuaFile
    {
        public string[] Lines;
        public LuaFunction[] Functions;

        public LuaFile( string path )
        {
            Lines = File.ReadAllLines(path);

            List<LuaFunction> finds = new List<LuaFunction>();
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].StartsWith("function")) // find line that is supposedly a function
                {
                    string name = Lines[i];
                    name = name.Remove(0, 9); // Remove function text + one space

                    string stripArgs = Regex.Match(Lines[i], @"(\(.*)\)").Value;
                    name = name.Remove(name.IndexOf(stripArgs), stripArgs.Length);

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    for( int y = 1; y<10; y++ ) // run through 10 lines up to find params
                    {
                        if( i - y >= 0 && Regex.IsMatch( Lines[i-y], @"@\w*" ) ) // if line has @word in it then process it, otherwise stop ALL search
                        {
                            string key = Regex.Match( Lines[i-y], @"@\w*" ).Value;
                            Lines[i - y] = Lines[i - y].Remove(0, Lines[i - y].IndexOf(key)); // remove whats before param key
                            Lines[i - y] = Lines[i - y].Remove(Lines[i - y].IndexOf(key), key.Length + 1); // remove param key from line + one space
                            Lines[i - y] = Lines[i - y].TrimEnd(';');
                            key = key.TrimStart('@');

                            param.Add(key, Lines[i - y]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    finds.Add( new LuaFunction(name, param) );
                }
            }
            Functions = finds.ToArray();
        }
    }
}
