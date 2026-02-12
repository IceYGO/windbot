using System;
using System.Collections.Generic;
using System.IO;

namespace WindBot
{
    public static class Config
    {
        private static string CONFIG_FILE_OPTION = "Config";
        private static char SEPARATOR_CHAR = '=';
        private static char COMMENT_CHAR = '#';

        private static Dictionary<string, string> _fields;
        private static Dictionary<string, int> _integerCache;
        private static Dictionary<string, bool> _booleanCache;

        public static void Load(string[] args)
        {
            _integerCache = new Dictionary<string, int>();
            _booleanCache = new Dictionary<string, bool>();

            _fields = LoadArgs(args);

            string filename = GetString(CONFIG_FILE_OPTION);
            if (filename != null)
            {
                Dictionary<string, string> fileFields = LoadFile(filename);
                foreach (var pair in fileFields)
                {
                    if (!_fields.ContainsKey(pair.Key))
                        _fields.Add(pair.Key, pair.Value);
                }
            }
        }

        private static Dictionary<string, string> LoadArgs(string[] args)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; ++i)
            {
                string option = args[i];

                int position = option.IndexOf(SEPARATOR_CHAR);

                if (position == -1)
                    throw new Exception("Invalid argument '" + option + "': no key/value separator");

                string key = option.Substring(0, position).Trim().ToUpper();
                string value = option.Substring(position + 1).Trim();

                if (fields.ContainsKey(key))
                    throw new Exception("Invalid argument '" + option + "': duplicate key '" + key + "'");

                fields.Add(key, value);
            }
            return fields;
        }

        private static Dictionary<string, string> LoadFile(string filename)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(filename))
            {
                int lineNumber = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    ++lineNumber;

                    // Ignore empty lines and comments
                    if (line.Length == 0 || line[0] == COMMENT_CHAR)
                        continue;

                    int position = line.IndexOf(SEPARATOR_CHAR);

                    if (position == -1)
                        throw new Exception("Invalid configuration file: no key/value separator line " + lineNumber);

                    string key = line.Substring(0, position).Trim().ToUpper();
                    string value = line.Substring(position + 1).Trim();

                    if (fields.ContainsKey(key))
                        throw new Exception("Invalid configuration file: duplicate key '" + key + "' line " + lineNumber);

                    fields.Add(key, value);
                }
            }
            return fields;
        }

        public static string GetString(string key, string defaultValue = null)
        {
            key = key.ToUpper();
            if (_fields.ContainsKey(key))
                return _fields[key];
            return defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            key = key.ToUpper();

            // Use a cache to prevent doing the string to int conversion over and over
            if (_integerCache.ContainsKey(key))
                return _integerCache[key];

            int value = defaultValue;
            if (_fields.ContainsKey(key))
            {
                if (_fields[key].StartsWith("0x"))
                    value = Convert.ToInt32(_fields[key], 16);
                else
                    value = Convert.ToInt32(_fields[key]);
            }
            _integerCache.Add(key, value);
            return value;
        }

        public static uint GetUInt(string key, uint defaultValue = 0)
        {
            return (uint)GetInt(key, (int)defaultValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            key = key.ToUpper();

            // Same here, prevent from redoing the string to bool conversion
            if (_booleanCache.ContainsKey(key))
                return _booleanCache[key];

            bool value = defaultValue;
            if (_fields.ContainsKey(key))
            {
                value = Convert.ToBoolean(_fields[key]);
            }
            _booleanCache.Add(key, value);
            return value;
        }
    }
}
