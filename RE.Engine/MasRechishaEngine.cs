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

        RuleResultTree? rule = getRule(taxResponse);
        if (rule != null && rule.Rule.Actions != null && rule.Rule.Actions.OnSuccess != null && rule.Rule.Actions.OnSuccess.Context["Brackets"] != null)
        {
            string s = "";
            var b = rule.Rule.Actions.OnSuccess.Context["Brackets"].ToString().Split(',').ToList();
            var p = rule.Rule.Actions.OnSuccess.Context["Percentage"].ToString().Split(',').ToList();
            var bt = rule.Rule.Actions.OnSuccess.Context["BracketsTax"].ToString().Split(',').ToList();
            var size = b.Count;
            bt[size - 1] = rule.ActionResult.Output.ToString();
            Double l = 0;
            for (int i = 0; i < size; i++)
            {
                tax.addBracketsTax(i, b[i], p[i], bt[i]);
                s = s + "Index: " + i + " Till: " + b[i] + " Percentage: " + p[i] + " Should pay: " + bt[i] + ";";
                l = l + Convert.ToDouble(bt[i]);
            }

            s = s + " Sum: " + l;
            tax.sumOfBackets = l;
            tax.setBracketsTaxRate(s);
        }

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
