using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NanoFontGenerator
{
    [JsonObject(MemberSerialization.Fields)]
    public class Data
    {
        public Dictionary<string, string> Macro;
        public Dictionary<string, string> Alphas;
        public string Src;

        public string DeMacro(string src)
        {
            var re = new StringBuilder();
            var mac = new StringBuilder();
            int deep = 0;
            foreach (var a in src)
            {
                if (a == '[')
                {
                    if (deep > 0)
                        mac.Append(a);
                    deep ++;
                }
                else if (a == ']')
                {
                    deep --;
                    if (deep == 0)
                    {
                        var ms = mac.ToString().Split(new char[] {','});
                        for (int i = 1; i < ms.Length; i++)
              
                            ms[i] = DeMacro(ms[i]);
                        var rms = new string[ms.Length - 1];
                        Array.Copy(ms, 1, rms, 0, rms.Length);
                        re.Append(String.Format(Macro[ms[0]], rms));
                        mac.Clear();
                    }
                    else mac.Append(a);
                }
                else
                {
                    if (deep != 0) mac.Append(a);
                    else re.Append(a);
                }
            }
            return re.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string adress = "NanoFont.json";
            string outputdir = "Output";
            Data data;
            using (var reader = new StreamReader(new FileStream(adress, FileMode.Open, FileAccess.Read)))
                data = JsonConvert.DeserializeObject<Data>(reader.ReadToEnd());
            foreach (var mac in data.Macro.Keys.ToArray())
                data.Macro[mac] = data.DeMacro(data.Macro[mac]);
            foreach (var a in data.Alphas)
            {
                using (var wirter = new StreamWriter(new FileStream(Path.Combine(outputdir, a.Key+".svg"), FileMode.Create, FileAccess.Write)))
                    wirter.Write(String.Format(data.Src, data.DeMacro(a.Value)));
            }
        }
    }
}
