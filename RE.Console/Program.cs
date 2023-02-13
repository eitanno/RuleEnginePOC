using System.ComponentModel;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine;
using RulesEngine.Models;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var content = File.ReadAllText(Path.Combine("flows.json"));
RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(content);


string inp1 = System.IO.File.ReadAllText(@"input1.json");
dynamic inputs = JsonConvert.DeserializeObject<ExpandoObject>(inp1, new ExpandoObjectConverter());


var rulesEngine = new RulesEngine.RulesEngine(workflows);
List<RuleResultTree> response = await rulesEngine.ExecuteAllRulesAsync("Discount", inputs);



foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(response))
{
    string name = descriptor.Name;
    object value = descriptor.GetValue(response);
    Console.WriteLine("{0}={1}", name, value);
}