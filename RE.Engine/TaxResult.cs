namespace RE.Engine;

public class TaxResult
{
    private const double DEFAULT_MAS_RECHISHA_RATE = 10;
    public double taxRate = DEFAULT_MAS_RECHISHA_RATE;
    public string bracketsTaxRate = "";

    public double sumOfBackets = 0;
    public List<BracketsTax> bracketsTax = new List<BracketsTax>();
    public bool resolvedFromRule = false;

    public TaxResult()
    {

    }

    public void addBracketsTax(int id, string till, string percentage, string shouldPay)
    {
        BracketsTax bTax = new BracketsTax();
        bTax.Id = id;
        bTax.Till = till;
        bTax.Percentage = percentage;
        bTax.ShouldPay = shouldPay;
        bracketsTax.Add(bTax);
    }

    public List<BracketsTax> getBracketsTax()
    {
        return bracketsTax;
    }



    public void setBracketsTaxRate(string tr)
    {
        bracketsTaxRate = tr;
        resolvedFromRule = true;
    }

    public string getBracketsTaxRate()
    {
        return bracketsTaxRate;
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
