using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MVScript
{
    public class Value
    {
        #region Properties

        public object Object { get; private set; }
        public bool IsByte => Object is byte;
        public bool IsShort => Object is short;
        public bool IsInt => Object is int;
        public bool IsLong => Object is long;
        public bool IsFloat => Object is float;
        public bool IsDouble => Object is double;
        public bool IsDecimal => Object is decimal;
        public bool IsBoolean => Object is bool;
        public bool IsString => Object is string;
        public bool IsList => Object is List<Value>;
        public bool IsDictionary => Object is Dictionary<string, Value>;
        public bool IsBytes => IsHex(ToString());
        public bool IsNullOrEmpty => string.IsNullOrEmpty(ToString());

        #endregion

        #region Methods (public)

        public Value(object value)
        {
            Object = Parse(value.ToString() ?? string.Empty);
        }

        public byte? ToByte()
        {
            try
            {
                return Convert.ToByte(Object);
            }
            catch
            {
                return null;
            }
        }

        public short? ToShort()
        {
            try
            {
                return Convert.ToInt16(Object);
            }
            catch
            {
                return null;
            }
        }

        public int? ToInt()
        {
            try
            {
                return Convert.ToInt32(Object);
            }
            catch
            {
                return null;
            }
        }

        public long? ToLong()
        {
            try
            {
                return Convert.ToInt64(Object);
            }
            catch
            {
                return null;
            }
        }

        public float? ToFloat()
        {
            try
            {
                return Convert.ToSingle(Object);
            }
            catch
            {
                return null;
            }
        }

        public double? ToDouble()
        {
            try
            {
                return Convert.ToDouble(Object);
            }
            catch
            {
                return null;
            }
        }

        public decimal? ToDecimal()
        {
            try
            {
                return Convert.ToDecimal(Object);
            }
            catch
            {
                return null;
            }
        }

        public bool? ToBoolean()
        {
            try
            {
                return Convert.ToBoolean(Object);
            }
            catch
            {
                return null;
            }
        }

        public List<Value>? ToList()
        {
            try
            {
                return (List<Value>)Object;
            }
            catch
            {
                return null;
            }
        }

        public Dictionary<string, Value>? ToDictionary()
        {
            try
            {
                return (Dictionary<string, Value>)Object;
            }
            catch
            {
                return null;
            }
        }

        public byte[]? ToBytes()
        {
            try
            {
                return Convert.FromHexString(ToString());
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            return RestoreEscapedCharacters(Object.ToString()!) ?? string.Empty;
        }

        #endregion

        #region Methods (private)

        //https://faculty.cs.niu.edu/~hutchins/csci473/types.htm
        private object Parse(string value)
        {
            if (value.TrimStart().StartsWith("[") && value.TrimEnd().EndsWith("]"))
                return ParseList(value);
            else if (value.TrimStart().StartsWith("{") && value.TrimEnd().EndsWith("}"))
                return ParseDictionary(value);
            else if (bool.TryParse(value, out bool boolValue))
                return boolValue;
            else if (byte.TryParse(value, out byte byteValue)) //8-bit integer
                return byteValue;
            else if (short.TryParse(value, out short shortValue)) //16-bit integer
                return shortValue;
            else if (int.TryParse(value, out int intValue)) //32-bit integer
                return intValue;
            else if (long.TryParse(value, out long longValue)) //64-bit integer
                return longValue;
            else if (float.TryParse(value, out float floatValue)) //32-bit floating-point, 7 significant digits
                return floatValue;
            else if (double.TryParse(value, out double doubleValue)) //64-bit floating-point, 15-16 significant digits
                return doubleValue;
            else if (decimal.TryParse(value, out decimal decimalValue)) //128-bit data type, 28-29 significant digits, distinct from floating-point
                return decimalValue;
            else
                return GetStringWithEscapeChars(value);
        }

        private List<Value> ParseList(string value)
        {
            var list = new List<Value>();

            // Remove brackets from the beginning and end
            value = value.Remove(0, 1);
            value = value.Remove(value.Length - 1, 1);

            // Split the values based on commas
            string[] valueArray = value.Split(',');

            bool findList = false;
            StringBuilder newValue = new StringBuilder();
            foreach (var item in valueArray)
            {
                if (item.TrimStart().StartsWith("[") || item.TrimEnd().EndsWith("]") || findList)
                {
                    if (item.TrimStart().StartsWith("["))
                    {
                        findList = true;
                        newValue.Append(item);
                    }
                    else if (item.TrimEnd().EndsWith("]"))
                    {
                        newValue.Append($", {item}");
                        findList = false;
                        list.Add(new Value(newValue.ToString().Trim()));
                        newValue.Clear();
                    }
                    else
                        newValue.Append($", {item}");
                }
                else
                {
                    // Trim each item to remove extra spaces
                    string trimmedItem = item.Trim();
                    list.Add(new Value(trimmedItem));
                }
            }

            return list;
        }

        private Dictionary<string, Value> ParseDictionary(string value)
        {
            var dictionary = new Dictionary<string, Value>();

            // Remove brackets from the beginning and end
            value = value.Remove(0, 1);
            value = value.Remove(value.Length - 1, 1);

            // Split the key-value pairs based on commas
            string[] keyValuePairs = value.Split(';');

            bool findDictionary = false;
            string newKey = string.Empty;
            StringBuilder newValue = new StringBuilder();
            foreach (var keyValuePair in keyValuePairs)
            {
                // Split each key-value pair based on colon
                string[] parts = keyValuePair.Split(':');
                if (parts[1].TrimStart().StartsWith("{") || parts[1].TrimStart().EndsWith("}") || findDictionary)
                {
                    if (parts[1].TrimStart().StartsWith("{"))
                    {
                        findDictionary = true;
                        newKey = parts[0].Trim();
                        newValue.Append($"{parts[1]}:{parts[2]}; ");

                    }
                    else if (parts[1].TrimEnd().EndsWith("}"))
                    {
                        newValue.Append($"{parts[0]}:{parts[1]}");
                        findDictionary = false;
                        dictionary.Add(newKey, new Value(newValue.ToString().Trim()));
                        newValue.Clear();
                    }
                    else
                        newValue.Append($"{parts[0]}:{parts[1]}; ");
                }
                else
                {
                    // Trim key and value to remove extra spaces
                    string key = parts[0].Trim();
                    string valueString = parts[1].Trim();

                    // Determine the type of the value and parse accordingly
                    dictionary.Add(key, new Value(valueString));
                }
            }

            return dictionary;
        }

        private bool IsHex(string text)
        {
            Regex myRegex = new Regex("^[a-fA-F0-9]+$");
            if (!string.IsNullOrEmpty(text) && myRegex.IsMatch(text))
                return true;
            return false;
        }

        private string GetStringWithEscapeChars(string input)
        {
            StringBuilder stringWithEscapeChars = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsControl(c) || c is ';' || c is ',' || c is '[' || c is ']' || c is '{' || c is '}')
                {
                    string escapedChar = string.Format("\\u{0:x4}", (int)c);
                    stringWithEscapeChars.Append(escapedChar);
                }
                else
                    stringWithEscapeChars.Append(c);
            }
            return stringWithEscapeChars.ToString();
        }

        private string RestoreEscapedCharacters(string input)
        {
            Regex escapePattern = new Regex(@"\\u([0-9a-fA-F]{4})");
            string restoredString = escapePattern.Replace(input, match =>
            {
                string hexValue = match.Groups[1].Value;
                int unicodeValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                char restoredChar = (char)unicodeValue;
                return restoredChar.ToString();
            });
            return restoredString;
        }

        #endregion

        #region Methods (static)

        public static Value Create(object value)
        {
            string formatedValue = FormatValue(value);
            return new Value(formatedValue);
        }

        private static string FormatValue(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value is byte[])
            {
                return Convert.ToHexString((value as byte[])!);
            }
            else if (value is IList list)
            {
                var formattedList = "[" + string.Join(", ", list.Cast<object>().Select(FormatValue)) + "]";
                return formattedList;
            }
            else if (value is IDictionary dictionary)
            {
                var formattedDict = "{" + string.Join("; ", dictionary.Cast<object>().Select(entry =>
                {
                    if (entry is DictionaryEntry de)
                    {
                        return $"{de.Key}:{FormatValue(de.Value ?? string.Empty)}";
                    }
                    else if (entry is KeyValuePair<object, object> kvp)
                    {
                        return $"{kvp.Key}:{FormatValue(kvp.Value ?? string.Empty)}";
                    }
                    else
                    {
                        var keyProperty = entry.GetType().GetProperty("Key");
                        var valueProperty = entry.GetType().GetProperty("Value");
                        var key = keyProperty?.GetValue(entry, null) ?? string.Empty;
                        var value = valueProperty?.GetValue(entry, null) ?? string.Empty;
                        return $"{key}:{FormatValue(value)}";
                    }
                })) + "}";
                return formattedDict;
            }
            else
            {
                return value.ToString() ?? string.Empty;
            }
        }

        #endregion
    }
}
