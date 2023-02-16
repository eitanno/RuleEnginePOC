using System.Dynamic;
using RulesEngine.Models;

namespace RE.Engine;

public class MasRechishaEngine : GenericEngine
{

    public MasRechishaEngine() : base()
    {

    }

    async public Task<double> run(ExpandoObject input)
    {
        double taxRate = 0;

        List<RuleResultTree> taxResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("TaxRate", input);
        taxRate = getSuccessRule(taxResponse);

        List<RuleResultTree> discountResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("Discount", input);
        taxRate = taxRate - getSuccessRule(discountResponse);

        List<RuleResultTree> specialFixRateResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("SpecialFixRate", input);
        if (hasSuccesRule(specialFixRateResponse))
        {
            taxRate = getSuccessRule(specialFixRateResponse);
        }
        return taxRate;
    }

    // The workflow file name
    protected override string getFlowsFileName()
    {
        return "MasRechishaFlows2023.json";
    }
}
