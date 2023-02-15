using System.ComponentModel;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine;
using RulesEngine.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting tax rate rules");

var jsonWorkflows = File.ReadAllText(Path.Combine("MasRechishaFlows2023.json"));
RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);

string jsonInput = System.IO.File.ReadAllText(@"MasRechishaInput.json");
List<ExpandoObject> inputs = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonInput, new ExpandoObjectConverter());

var taxRateEngine = new RulesEngine.RulesEngine(workflows);
foreach(Object input in inputs) {
    double taxRate = 0;

    
    List<RuleResultTree> taxResponse = await taxRateEngine.ExecuteAllRulesAsync("TaxRate", input);          
    taxRate = printInputRulesResult(taxResponse);
    
    List<RuleResultTree> discountResponse = await taxRateEngine.ExecuteAllRulesAsync("Discount", input);
    taxRate = taxRate - printInputRulesResult(discountResponse);

    List<RuleResultTree> specialFixRateResponse = await taxRateEngine.ExecuteAllRulesAsync("SpecialFixRate", input);
    taxRate = taxRate - printInputRulesResult(specialFixRateResponse);
}


double printInputRulesResult(List<RuleResultTree> response) {
    // TODO: if more then 1
    return response
        .Where(r => r.IsSuccess == true)
        .Select(r => Convert.ToDouble(r.Rule.SuccessEvent))
        .FirstOrDefault();
    /*double taxRate = 0;
    foreach(var ruleResult in response){   
        if(ruleResult.IsSuccess){
            string sucessEvent = ruleResult.Rule.SuccessEvent;
            Console.WriteLine(sucessEvent);
           taxRate =  Convert.ToDouble(sucessEvent);
        }
    }
    return taxRate;*/
}