using System.Dynamic;
using Newtonsoft.Json;
using RulesEngine.Actions;
using RulesEngine.Models;

namespace RE.Engine;
public abstract class GenericEngine
{
    protected RulesEngine.Models.Workflow[] workflows;
    protected RulesEngine.RulesEngine microsoftRuleEngine;
    public GenericEngine()
    {
        // TODO - For efficiency rules should be written nested when possible.
        // (The above is true if we use first success. If we iterate all rules - it does not matter)
        // Reading Mas-Rechisha rules file
        string jsonWorkflows = File.ReadAllText(Path.Combine("..", "assets", getFlowsFileName()));
        workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);
        double mrTaxRate = 0;

        //Registaring action to the engine for rule's post execute ation
        var reSettings = new ReSettings
        {
            CustomActions = new Dictionary<string, Func<ActionBase>>{
            {getActionName(), () => getActionObject() }
      }
        };
        microsoftRuleEngine = new RulesEngine.RulesEngine(workflows, reSettings);
    }
    
    public Workflow[] getWorkflows()
    {
        var content = System.IO.File.ReadAllText(Path.Combine("..", "assets", getFlowsFileName()));

        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(content);
        return workflows;
    }

    public string updateWorkflows(Workflow[] workflows)
    {
        var content = JsonConvert.SerializeObject(workflows);
        System.IO.File.WriteAllText("../assets/" + getFlowsFileName(), content);
        return content;
    }




    protected abstract string getFlowsFileName();

    protected abstract string getActionName();

    protected abstract ActionBase getActionObject();


    async public Task<double> run(ExpandoObject input)
    {
        return 0;
    }


    protected bool hasSuccesRule(List<RuleResultTree> response)
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
    protected void resolveTaxFromRules(List<RuleResultTree> response, TaxResult tax)
    {

        var successResponses = response.Where(r => r.IsSuccess == true);

        var successEventValues = successResponses.Select(r => Convert.ToDouble(r.Rule.SuccessEvent));
        if (successEventValues.Any())
        {
            //TODO - what if more than one success rule [here bellow we use FirstOrDefault()]
            var firstSuccessEventValue = successEventValues.FirstOrDefault();
            tax.setTaxRate(firstSuccessEventValue);
        }
    }
}
