using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RE.Engine;

Console.WriteLine("Start evaluating Mas Rechisha for various inputs - ");
string jsonInput = System.IO.File.ReadAllText(Path.Combine("..", "assets", @"MasRechishaInput.json"));
List<ExpandoObject> inputs = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonInput, new ExpandoObjectConverter());


MasRechishaEngine engine = new MasRechishaEngine();
if (inputs != null)
{
    inputs.ForEach(async inp => Console.WriteLine(inp.FirstOrDefault(x => x.Key == "name").Value + " taxRate is " + await engine.run(inp)));
}
Console.ReadLine();