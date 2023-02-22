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
        MasRechishaTaxBrackets taxBrackets = new MasRechishaTaxBrackets();
        List<RuleResultTree> loadedBrackets = await microsoftRuleEngine.ExecuteAllRulesAsync("GLOBAL_BRCAKETS", taxBrackets);
    
        TaxResult tax = new TaxResult();
        RuleResultTree? result;
        List<RuleResultTree> taxResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("TaxRate", input);
        result = resolveTaxFromRules(taxResponse, tax);
        

        List<RuleResultTree> discountResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("DiscountAdditionaAndHighExclusive", input);
        if (hasSuccesRule(discountResponse))
        {
            result = resolveTaxFromRules(discountResponse, tax);
        }

        List<RuleResultTree> specialFixRateResponse = await microsoftRuleEngine.ExecuteAllRulesAsync("DiscountLowExclusive", input);
        if (hasSuccesRule(specialFixRateResponse))
        {
            result = resolveTaxFromRules(specialFixRateResponse, tax);
        }

        string summary = presentTaxTable(tax, result);    
        Console.WriteLine(input.FirstOrDefault(x => x.Key == "name").Value + "\r\n" + summary + "\r\n");
        return tax;
    }

    private string? presentTaxTable(TaxResult tax, RuleResultTree? result)
    {
        if (result != null && result.Rule.Actions != null && result.Rule.Actions.OnSuccess != null && result.Rule.Actions.OnSuccess.Context["Brackets"] != null)
        {
            
            string s = "";
            var purchaseBrackets = result.Rule.Actions.OnSuccess.Context["Brackets"].ToString().Split(',').ToList();
            var percentageBrackets = result.Rule.Actions.OnSuccess.Context["Percentage"].ToString().Split(',').ToList();
            var taxSumBrackets = result.Rule.Actions.OnSuccess.Context["BracketsTax"].ToString().Split(',').ToList();
            var size = purchaseBrackets.Count;
            taxSumBrackets[size - 1] = result.ActionResult.Output.ToString();
            Double taxToPay = 0;
            for (int i = 0; i < size; i++)
            {
                tax.addBracketsTax(i, purchaseBrackets[i], percentageBrackets[i], taxSumBrackets[i]);
                s = s + "Index: " + i + " Till: " + purchaseBrackets[i] + " Percentage: " + percentageBrackets[i] + " Should pay: " + taxSumBrackets[i] + ";"+ "\r\n";
                taxToPay = taxToPay + Convert.ToDouble(taxSumBrackets[i]);
            }

            s = s + "****SUM TAX TO PAY: " + taxToPay;
            tax.sumOfBackets = taxToPay;
            tax.setBracketsTaxRate(s);
            return s;
        }
        return null;
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
