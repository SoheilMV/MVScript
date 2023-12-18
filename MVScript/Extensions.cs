namespace MVScript
{
    public static class Extensions
    {
        public static Value? FromIndex(this List<Value> list, int index)
        {
            try
            {
                return list[index];
            }
            catch
            {
                return null;
            }
        }

        public static Value? GetValue(this Dictionary<string, Value> dictionary, string key)
        {
            try
            {
                dictionary.TryGetValue(key, out Value? value);
                return value;
            }
            catch
            {
                return null;
            }
        }
    }
}
