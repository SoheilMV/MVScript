using MVScript;

string raw = @"
BEGIN ROAD road1
    BEGIN BLOCK block1
        name: Soheil MV
        date: 2023
        isDev: true
    END BLOCK

    BEGIN BLOCK block2
        myList: [Soheil MV, 2023, true]
    END BLOCK

    BEGIN BLOCK block3
        myDictionary: {name:Soheil MV; date:2023; isDev:true}
    END BLOCK
END ROAD
";

var list = new List<object>
{
    new Dictionary<object, object>()
    {
        { "name", "Soheil MV" },
        { "date", 2023 },
        { "IsDev", true },
    },
    new List<int>() { 1, 2, 3, 4, 5, 6, 7 },
    new List<double>() { 1.1, 2.2, 3.3 },
    200,
    true,
};

var dictionary = new Dictionary<object, object>()
{
    { "myList", new List<object>() { "Soheil MV", 2023, true } },
    { "Dic", new Dictionary<object, object>
        {
            { "name", "Soheil MV" },
            { "date", 2023 },
            { "IsDev", true },
        } 
    },
    { "StatusCode", 200 },
    { "IsOk", true },
};


byte[] bytes = new byte[10];
new Random().NextBytes(bytes);

long[] longs = new long[3] { 100, 200, 300 };

double[] doubles = new double[3] { 1.1, 2.2, 3.3, };


// Initialize MVScript
Script script = new Script(raw);

// Modify the script
script["road2"]["block1"]["myList"] = Value.Create(list);
script["road3"]["block1"]["myDictionary"] = Value.Create(dictionary);
script["road4"]["block1"]["bytes"] = Value.Create(bytes);
script["road4"]["block1"]["longs"] = Value.Create(longs);
script["road4"]["block1"]["doubles"] = Value.Create(doubles);
script["road4"]["block1"]["objects"] = Value.Create(new object[3] { bytes, longs, doubles });

// Initialize ScriptHandler
ScriptHandler handler = new ScriptHandler();

// Serialize the modified script to a file and read it back
handler.Write("script.mvs", script);
Console.WriteLine(handler.Read("script.mvs").ToString());

// Read values from the script
Value value = script["road1"]["block1"]["name"]!;
Value value2 = script["road2"]["block1"]["myList"].ToList()!.FromIndex(0)!.ToDictionary()!.GetValue("name")!;
Value value3 = script["road3"]["block1"]["myDictionary"].ToDictionary()!.GetValue("myList")!.ToList()!.FromIndex(0)!;
Value value4 = script["road3"]["block1"]["myDictionary"]!.ToDictionary()!.GetValue("Dic")!.ToDictionary()!.GetValue("name")!;
Value value5 = script["road4"]["block1"]["objects"]!.ToList()!.FromIndex(1)!.ToList()!.FromIndex(2)!;

// Console Output
Console.WriteLine("================================================================");
Console.WriteLine();
Console.WriteLine($"road1 > block1 > name > {value}");
Console.WriteLine($"road2 > block1 > myList[0] > dictionary[name] > {value2}");
Console.WriteLine($"road3 > block1 > myDictionary[myList] > list[0] > {value3}");
Console.WriteLine($"road3 > block1 > myDictionary[Dic] > Dic[name] > {value4}");
Console.WriteLine($"road4 > block1 > objects[1][2] > {value5}");
Console.ReadKey();