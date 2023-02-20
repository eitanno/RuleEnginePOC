using RulesEngine.Actions;
using RulesEngine.Models;

public class MasRechishaAction : ActionBase
{
    public MasRechishaAction()
    {
    }


    public override ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
    {
        var customInput = context.GetContext<string>("Expression");
        //Add your custom logic here
        return new System.Threading.Tasks.ValueTask<object>();
    }
}
