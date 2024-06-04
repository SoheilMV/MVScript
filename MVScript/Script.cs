using System.Text;
using System.Text.RegularExpressions;

namespace MVScript
{
    public class Script
    {
        public List<Road> Roads { get; private set; }
        public Road this[string name]
        {
            get
            {
                Road? road = Roads.FirstOrDefault(r => r.Name == name);
                if (road is null)
                {
                    road = new Road(name);
                    Roads.Add(road);
                }
                return road;
            }
            set
            {
                Road? road = Roads.FirstOrDefault(r => r.Name == name);
                if (road is null)
                {
                    road = new Road(name);
                    Roads.Add(road);
                }
                road.Blocks.AddRange(value.Blocks);
            }
        }

        public Script()
        {
            Roads = new List<Road>();
        }

        public Script(string raw)
        {
            Roads = ParseScript(raw);
        }

        public void Format()
        {
            Roads.ForEach(r => r.Blocks.RemoveAll(b => b.Values.Count == 0));
            Roads.RemoveAll(r => r.Blocks.Count == 0);
        }

        public override string ToString()
        {
            Format();
            StringBuilder sb = new StringBuilder();
            foreach (var road in Roads)
            {
                sb.AppendLine($"BEGIN ROAD {road.Name}");
                foreach (var block in road.Blocks)
                {
                    sb.AppendLine($"\tBEGIN BLOCK {block.Name}");

                    foreach (var (key, value) in block.Values)
                    {
                        string formattedValue = FormatValue(value);
                        sb.AppendLine($"\t\t{key}: {formattedValue}");
                    }

                    sb.AppendLine($"\tEND BLOCK");
                }
                sb.AppendLine($"END ROAD");
            }
            return sb.ToString();
        }

        private List<Road> ParseScript(string raw)
        {
            List<Road> roads = new List<Road>();
            string[] lines = raw.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            Road? currentRoad = null;
            Block? currentBlock = null;

            foreach (string line in lines)
            {
                switch (line)
                {
                    case var roadLine when roadLine.StartsWith("BEGIN ROAD"):
                        currentRoad = new Road(GetRoadName(roadLine));
                        roads.Add(currentRoad);
                        break;

                    case var blockLine when blockLine.StartsWith("BEGIN BLOCK"):
                        currentBlock = new Block(GetBlockName(blockLine));
                        currentRoad?.Blocks.Add(currentBlock);
                        break;

                    case var keyValuePair when keyValuePair.Contains(":"):
                        int colonIndex = keyValuePair.IndexOf(':');
                        string key = keyValuePair.Substring(0, colonIndex).Trim();
                        string value = keyValuePair.Substring(colonIndex + 1).Trim();
                        currentBlock?.Values.Add(key, new Value(value));
                        break;
                }
            }

            return roads;
        }

        private string GetRoadName(string line)
        {
            return Regex.Match(line, @"BEGIN ROAD (\w+)").Groups[1].Value;
        }

        private string GetBlockName(string line)
        {
            return Regex.Match(line, @"BEGIN BLOCK (\w+)").Groups[1].Value;
        }

        private string FormatValue(Value value)
        {
            return value switch
            {
                { Object: List<Value> list } => $"[{string.Join(", ", list.Select(FormatValue))}]",
                { Object: Dictionary<string, Value> dictionary } => $"{{{string.Join("; ", dictionary.Select(kv => $"{kv.Key}:{FormatValue(kv.Value)}"))}}}",
                _ => value.Object.ToString() ?? string.Empty,
            };
        }
    }
}
