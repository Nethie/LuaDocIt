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
        public LuaHook[] Hooks;

        public LuaFile( string path )
        {
            Lines = File.ReadAllLines(path);

            List<LuaFunction> finds = new List<LuaFunction>();
            List<LuaHook> hfinds = new List<LuaHook>();
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].StartsWith("function") || Lines[i].StartsWith("local function")) // find line that is supposedly a function
                {
                    int trimLen = Lines[i].StartsWith("local function") ? 15 : 9;
                    string name = Lines[i];
                    name = name.Remove(0, trimLen); // Remove function text + one space

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
                else
                if(Lines[i].StartsWith("hook.Add"))
                {
                    string name = Lines[i];
                    name = name.Remove(0, 8);

                    string[] args = name.Split(',');
                    name = args[0];
                    name = name.TrimStart('('); // remove ( that starts with each hook.Add, hook.Call
                    name = name.TrimStart();    // remove eventual bonus space
                    name = name.TrimStart('"');
                    name = name.TrimEnd('"');

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    for (int y = 1; y < 10; y++) // run through 10 lines up to find params
                    {
                        if (i - y >= 0 && Regex.IsMatch(Lines[i - y], @"@\w*")) // if line has @word in it then process it, otherwise stop ALL search
                        {
                            string key = Regex.Match(Lines[i - y], @"@\w*").Value;
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

                    hfinds.Add(new LuaHook(name, param));
                }
            }
            Functions = finds.ToArray();
            Hooks = hfinds.ToArray();
        }
    }
}
