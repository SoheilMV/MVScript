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
    new Dictionary<string, object>()
    {
        { "name", "Soheil MV"},
        { "date", 2023},
        { "IsDev", true},
    },
    200,
    true
};

var dictionary = new Dictionary<string, object>()
{
    { "myList", new List<object>()
      {
        "Soheil MV",
        2023,
        true
      }
    },
    { "StatusCode", 200 },
    { "IsOk", true },
};

// Initialize MVScript
Script script = new Script(raw);

//Modify the script
script["road2"]["block1"]["myList"] = Value.Create(list);
script["road3"]["block1"]["myDictionary"] = Value.Create(dictionary);

//Read values from the script
Value value = script["road1"]["block1"]["name"]!;
Value value2 = script["road2"]["block1"]["myList"].ToList()!.FromIndex(0)!.ToDictionary()!.GetValue("name")!;
Value value3 = script["road3"]["block1"]["myDictionary"].ToDictionary()!.GetValue("myList")!.ToList()!.FromIndex(0)!;

// Initialize ScriptHandler
ScriptHandler handler = new ScriptHandler();

// Serialize the modified script to a file and read it back
handler.Write("script.mvs", script);
Console.WriteLine(handler.Read("script.mvs").ToString());

// Console Output
Console.WriteLine();
Console.WriteLine($"road1 > block1 > name > {value.ToString()}");
Console.WriteLine($"road2 > block1 > myList[0] > dictionary[name] > {value2.ToString()}");
Console.WriteLine($"road3 > block1 > myDictionary[myList] > list[0] > {value3.ToString()}");
Console.ReadKey();