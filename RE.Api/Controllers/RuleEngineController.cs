using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine;
using RulesEngine.Models;
using RE.Engine;
using Newtonsoft.Json.Linq;

namespace RE.Api.Controllers;


[ApiController]
[Route("ruleengine")]
public class RuleEngineController : ControllerBase
{

    private readonly ILogger<RuleEngineController> _logger;

    public RuleEngineController(ILogger<RuleEngineController> logger)
    {

        _logger = logger;
    }


    [HttpGet(Name = "GetWorkflows")]
    public RulesEngine.Models.Workflow[] Get()
    {
        var content = System.IO.File.ReadAllText(Path.Combine("..", "assets", "MasRechishaFlows2023Rules.json"));

        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(content);
        return workflows;
    }

    [HttpPut(Name = "SetWorkflows")]
    public IActionResult Put([FromBody] RulesEngine.Models.Workflow[] workflows)
    {
        var content = JsonConvert.SerializeObject(workflows);
        System.IO.File.WriteAllText("../assets/MasRechishaFlows2023Rules.json", content);
        return Ok(content);
    }

    [HttpPost(Name = "SetExecuteAllRules")]
    [Route("executeallrules")]
    public IActionResult SetAsyn([FromBody] Object[] jsonInput)
    {
        var inputs = jsonInput
            .Select(ji => JsonConvert.
                DeserializeObject<ExpandoObject>(ji.ToString(), new ExpandoObjectConverter())
            ).ToList();

        MasRechishaEngine engine = new MasRechishaEngine();
        List<Task<double>> tasks = new List<Task<double>>();
        inputs.ForEach(i =>
        {
            tasks.Add(engine.run(i));
        });

        return Ok(tasks);
    }

}
