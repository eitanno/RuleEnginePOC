using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;

namespace RE.Engine;
public class MainEngine
{
    private RulesEngine.Models.Workflow[] workflows;
    private RulesEngine.RulesEngine taxRateEngine;

    public MainEngine(string flowsFileName)
    {
        string jsonWorkflows = File.ReadAllText(Path.Combine("..", "assets", flowsFileName));
        workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);
        taxRateEngine = new RulesEngine.RulesEngine(workflows);
    }

    async public Task<double> run(ExpandoObject input)
    {
        double taxRate = 0;

        List<RuleResultTree> taxResponse = await taxRateEngine.ExecuteAllRulesAsync("TaxRate", input);
        taxRate = getFirstSuccessRule(taxResponse);

        List<RuleResultTree> discountResponse = await taxRateEngine.ExecuteAllRulesAsync("Discount", input);
        taxRate = taxRate - getFirstSuccessRule(discountResponse);

        List<RuleResultTree> specialFixRateResponse = await taxRateEngine.ExecuteAllRulesAsync("SpecialFixRate", input);
        if (hasSuccesRule(specialFixRateResponse))
        {
            taxRate = getFirstSuccessRule(specialFixRateResponse);
        }
        return taxRate;
    }


    private bool hasSuccesRule(List<RuleResultTree> response)
    {
        foreach (RuleResultTree res in response)
        {
            if (res.IsSuccess)
            {
                return true;
            }
        }
        return false;
    }
    private double getFirstSuccessRule(List<RuleResultTree> response)
    {
        return response
            .Where(r => r.IsSuccess == true)
            .Select(r => Convert.ToDouble(r.Rule.SuccessEvent))
            .FirstOrDefault();
    }
}
