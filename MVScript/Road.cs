namespace MVScript
{
    public class Road
    {
        public string Name { get; set; }
        public List<Block> Blocks { get; }

        public Road(string name)
        {
            Name = name;
            Blocks = new List<Block>();
        }

        public Block this[string name]
        {
            get
            {
                Block? block = Blocks.FirstOrDefault(b => b.Name == name);
                if (block is null)
                {
                    block = new Block(name);
                    Blocks.Add(block);
                }
                return block;
            }
            set
            {
                Block? block = Blocks.FirstOrDefault(r => r.Name == name);
                if (block is null)
                {
                    block = new Block(name);
                    Blocks.Add(block);
                }
                foreach (var (key, val) in value.Values)
                    block.Values[key] = val;
            }
        }
    }
}
