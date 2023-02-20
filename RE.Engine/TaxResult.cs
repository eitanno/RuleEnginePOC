
namespace RE.Engine;

public class TaxResult
{
    private const double DEFAULT_MAS_RECHISHA_RATE = 10;
    double taxRate = DEFAULT_MAS_RECHISHA_RATE;
    bool resolvedFromRule = false;

    public TaxResult()
    {

    }

    public void setTaxRate(double tr)
    {
        taxRate = tr;
        resolvedFromRule = true;
    }

    public double getTaxRate()
    {
        return taxRate;
    }

    public bool IsResolvedFromRule()
    {
        return resolvedFromRule;
    }


}
