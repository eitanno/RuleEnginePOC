﻿using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;

namespace RE.Engine;
public class MainEngine
{
    private RulesEngine.RulesEngine taxRateEngine;

    public MainEngine()
    {
        var jsonWorkflows = File.ReadAllText(Path.Combine("..", "assets", "MasRechishaFlows2023.json"));
        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);
        taxRateEngine = new RulesEngine.RulesEngine(workflows);
    }

    async public Task<double> run(ExpandoObject input)
    {
        Console.WriteLine("Starting tax rate rules");
        double taxRate = 0;

        List<RuleResultTree> taxResponse = await runWorkflow("TaxRate", input);
        taxRate = printInputRulesResult(taxResponse);

        List<RuleResultTree> discountResponse = await runWorkflow("Discount", input);
        taxRate = taxRate - printInputRulesResult(discountResponse);

        List<RuleResultTree> specialFixRateResponse = await runWorkflow("SpecialFixRate", input);
        taxRate = taxRate - printInputRulesResult(specialFixRateResponse);

        return taxRate;
    }


    async public Task<List<RuleResultTree>> runWorkflow(string workflowName, ExpandoObject input)
    {
        var jsonWorkflows = File.ReadAllText(Path.Combine("..", "assets", "MasRechishaFlows2023.json"));
        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);

        var taxRateEngine = new RulesEngine.RulesEngine(workflows);

        return await taxRateEngine.ExecuteAllRulesAsync(workflowName, input);
    }
    private double printInputRulesResult(List<RuleResultTree> response)
    {
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
}