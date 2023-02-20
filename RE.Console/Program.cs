using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RE.Engine;

Console.WriteLine("Using a rule engine to resolve Mas-Rechisha for different inputs:");
Console.WriteLine("-----------------------------------------------------------------");

//Reding input file [Data of various purchases]
string jsonInput = System.IO.File.ReadAllText(Path.Combine("..", "assets", @"MasRechishaInput.json"));
List<ExpandoObject> inputs = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonInput, new ExpandoObjectConverter());

//Running Mas-Rechisha rule engine 
MasRechishaEngine engine = new MasRechishaEngine();
if (inputs != null)
{
    inputs.ForEach(async inp => engine.run(inp));
}
Console.ReadLine();