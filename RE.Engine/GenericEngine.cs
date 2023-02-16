using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;

namespace RE.Engine;
public abstract class GenericEngine
{
    protected RulesEngine.Models.Workflow[] workflows;
    protected RulesEngine.RulesEngine microsoftRuleEngine;
    public GenericEngine()
    {
        string jsonWorkflows = File.ReadAllText(Path.Combine("..", "assets", getFlowsFileName()));
        workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(jsonWorkflows);
        microsoftRuleEngine = new RulesEngine.RulesEngine(workflows);
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
    protected double getSuccessRule(List<RuleResultTree> response)
    {
        // TODO what if there is more than one success rule.
        return response
            .Where(r => r.IsSuccess == true)
            .Select(r => Convert.ToDouble(r.Rule.SuccessEvent))
            .FirstOrDefault();
    }
}
