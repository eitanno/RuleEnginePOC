using System.Dynamic;
using RulesEngine.Actions;
using RulesEngine.Models;

namespace RE.Engine;

public class MasRechishaEngine : GenericEngine
{
    
    public MasRechishaEngine() : base()
    {

    }

    async public Task<TaxResult> run(ExpandoObject input)
    {
        TaxResult tax = new TaxResult();

        List<RuleResultTree> taxResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("TaxRate", input);
        resolveTaxFromRules(taxResponse, tax);

        List<RuleResultTree> discountResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("Discount", input);
        resolveTaxFromRules(discountResponse, tax);

        List<RuleResultTree> specialFixRateResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("SpecialFixRate", input);
        if (hasSuccesRule(specialFixRateResponse))
        {
            resolveTaxFromRules(specialFixRateResponse, tax);
        }
        Console.WriteLine(input.FirstOrDefault(x => x.Key == "name").Value + 
        " taxRate is " + tax.getTaxRate() + ",      from rule? :" + tax.IsResolvedFromRule());
        return tax;
    }

    // The workflow file name
    protected override string getFlowsFileName()
    {
        return "MasRechishaFlows2023.json";
    }

    protected override string getActionName()
    {
        return "MasRechishaAction";
    }

    protected override ActionBase getActionObject()
    {
        return new MasRechishaAction();
    }
}
