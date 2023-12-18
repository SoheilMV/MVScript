using System.Text;
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
        public bool IsNullOrEmpty => string.IsNullOrEmpty(ToString());

        #endregion

        #region Methods (public)

        public Value(object? value)
        {
            Object = Parse(value?.ToString() ?? string.Empty);
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
            value = value.Trim('[', ']');

            // Split the values based on commas
            string[] valueArray = value.Split(',');

            foreach (var item in valueArray)
            {
                // Trim each item to remove extra spaces
                string trimmedItem = item.Trim();
                list.Add(new Value(trimmedItem));
            }

            return list;
        }


        private Dictionary<string, Value> ParseDictionary(string value)
        {
            var dictionary = new Dictionary<string, Value>();

            // Remove brackets from the beginning and end
            value = value.Trim('{', '}');

            // Split the key-value pairs based on commas
            string[] keyValuePairs = value.Split(';');

            foreach (var keyValuePair in keyValuePairs)
            {
                // Split each key-value pair based on colon
                string[] parts = keyValuePair.Split(':');

                if (parts.Length == 2)
                {
                    // Trim key and value to remove extra spaces
                    string key = parts[0].Trim();
                    string valueString = parts[1].Trim();

                    // Determine the type of the value and parse accordingly
                    dictionary.Add(key, new Value(valueString));
                }
                else
                {
                    // Handle the situation when the key-value pair is not in the expected format
                }
            }

            return dictionary;
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
            return new Value(value);
        }

        public static Value Create(object[] values)
        {
            string formattedValues = "[" + string.Join(", ", values.Select(FormatValue)) + "]";
            return new Value(formattedValues);
        }

        public static Value Create(List<object> values)
        {
            string formattedValues = "[" + string.Join(", ", values.Select(FormatValue)) + "]";
            return new Value(formattedValues);
        }

        public static Value Create(Dictionary<string, object> values)
        {
            string formattedValues = "{" + string.Join("; ", values.Select(kv => $"{kv.Key}:{FormatValue(kv.Value)}")) + "}";
            return new Value(formattedValues);
        }

        private static string FormatValue(object value)
        {
            if (value is List<object> list)
            {
                string formattedList = "[" + string.Join(", ", list.Select(FormatValue)) + "]";
                return formattedList;
            }
            else if (value is Dictionary<string, object> dictionary)
            {
                string formattedDictionary = "{" + string.Join("; ", dictionary.Select(kv => $"{kv.Key}:{FormatValue(kv.Value)}")) + "}";
                return formattedDictionary;
            }
            else
            {
                return new Value(value).Object.ToString() ?? string.Empty;
            }
        }

        #endregion
    }

}
