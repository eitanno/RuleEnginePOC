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

public class MyObject
{
    public Double Sum { get; set; }
    public List<BracketsTax> BracketsTax { get; set; }
}

[ApiController]
[Route("purchasetax")]
public class MasRechishaController : ControllerBase
{

    private readonly ILogger<MasRechishaController> _logger;

    public MasRechishaController(ILogger<MasRechishaController> logger)
    {

        _logger = logger;
    }


    [HttpGet(Name = "GetWorkflows")]
    public RulesEngine.Models.Workflow[] Get()
    {
        MasRechishaEngine engine = new MasRechishaEngine();
        return engine.getWorkflows();
    }

    [HttpPut(Name = "SetWorkflows")]
    public IActionResult Put([FromBody] RulesEngine.Models.Workflow[] workflows)
    {
        MasRechishaEngine engine = new MasRechishaEngine();
        var content = engine.updateWorkflows(workflows);
        return Ok(content);
    }

    [HttpPost(Name = "SetExecuteAllRules")]
    [Route("executeallrules")]
    public List<MyObject> SetAsyn([FromBody] Object[] jsonInput)
    {
        var inputs = jsonInput
            .Select(ji => JsonConvert.
                DeserializeObject<ExpandoObject>(ji.ToString(), new ExpandoObjectConverter())
            ).ToList();

        MasRechishaEngine engine = new MasRechishaEngine();
        List<MyObject> tasks = new List<MyObject>();
        inputs.ForEach(i =>
        {
            MyObject myObject = new MyObject();
            TaxResult tax = engine.run(i).Result;
            myObject.BracketsTax = tax.getBracketsTax();
            myObject.Sum = tax.sumOfBackets;
            tasks.Add(myObject);
        });

        return tasks;
    }

}
