namespace MVScript
{
    public class Block
    {
        public string Name { get; set; }
        public Dictionary<string, Value> Values { get; } = new Dictionary<string, Value>();

        public Value this[string name]
        {
            get => Values.ContainsKey(name) ? Values[name] : new Value(string.Empty);
            set
            {
                Values[name] = value;
            }
        }
    }
}
