using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RE.Engine;


string jsonInput = System.IO.File.ReadAllText(Path.Combine("..", "assets", @"MasRechishaInput.json"));
List<ExpandoObject> inputs = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonInput, new ExpandoObjectConverter());


MainEngine engine = new MainEngine();
if (inputs != null)
{
    inputs.ForEach(async inp => Console.WriteLine(await engine.run(inp)));
}
Console.ReadLine();