#region

using LoESoft.Core.models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace LoESoft.Core
{
    public class AutoAssign : IDisposable
    {
        private static ILog Logger = LogManager.GetLogger(nameof(AutoAssign));

        private Dictionary<string, string> values { get; set; }
        private string id { get; set; }
        private string cfgFile { get; set; }

        public AutoAssign(string id)
        {
            values = new Dictionary<string, string>();
            this.id = id;
            cfgFile = Path.Combine(Environment.CurrentDirectory, id + ".cfg");
            if (File.Exists(cfgFile))
                using (var rdr = new StreamReader(File.OpenRead(cfgFile)))
                {
                    string line;
                    int lineNum = 1;
                    while ((line = rdr.ReadLine()) != null)
                    {
                        if (line.StartsWith(";"))
                            continue;
                        int i = line.IndexOf(":");
                        if (i == -1)
                        {
                            Log.Info($"Invalid settings at line {lineNum}.");
                            throw new ArgumentException("Invalid settings.");
                        }
                        string val = line.Substring(i + 1);

                        values.Add(line.Substring(0, i),
                            val.Equals("null", StringComparison.InvariantCultureIgnoreCase) ? null : val);
                        lineNum++;
                    }
                }
        }

        public void Dispose()
        {
            try
            {
                using (var writer = new StreamWriter(File.OpenWrite(cfgFile)))
                    foreach (var i in values)
                        writer.WriteLine($"{i.Key}:{(i.Value == null ? "null" : i.Value)}");
            }
            catch (Exception)
            {
                return;
            }
        }

        public T GetValue<T>(string key, string ifNull = null)
        {
            if (!values.TryGetValue(key, out string ret))
            {
                if (ifNull == null)
                {
                    Log.Error($"Attempt to access nonexistant settings \"{key}\".");
                    throw new ArgumentException($"\"{key}\" does not exist in settings.");
                }
                ret = values[key] = ifNull;
            }
            return (T)Convert.ChangeType(ret, typeof(T));
        }

        public void SetValue(string key, string val)
        {
            values[key] = val;
        }
    }
}