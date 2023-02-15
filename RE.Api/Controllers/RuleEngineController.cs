using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine;
using RulesEngine.Models;

namespace RE.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class RuleEngineController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<RuleEngineController> _logger;

    public RuleEngineController(ILogger<RuleEngineController> logger)
    {

        _logger = logger;
    }


    [HttpGet(Name = "GetWorkflows")]
    public RulesEngine.Models.Workflow[] Get()
    {
        var content = System.IO.File.ReadAllText(Path.Combine("flows.json"));

        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(content);
        return workflows;
    }

    [HttpPut(Name = "SetWorkflows")]
    public IActionResult Put([FromBody] RulesEngine.Models.Workflow[] workflows)
    {
        var content = JsonConvert.SerializeObject(workflows);
        System.IO.File.WriteAllText("flows.json", content);
        return Ok(content);
    }

    [HttpPost(Name = "SetExecuteAllRules")]
    public async Task<List<RuleResultTree>> SetAsync(Object input)
    {
        var content = System.IO.File.ReadAllText(Path.Combine("flows.json"));
        RulesEngine.Models.Workflow[] workflows = JsonConvert.DeserializeObject<RulesEngine.Models.Workflow[]>(content);
        var rulesEngine = new RulesEngine.RulesEngine(workflows);
        List<RuleResultTree> response = await rulesEngine.ExecuteAllRulesAsync("Discount", input);
        return response;
    }





}
