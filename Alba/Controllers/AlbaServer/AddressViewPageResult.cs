public class AddressViewPageResult
{
    public object[] error { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public Summary summary { get; set; }
    public Html html { get; set; }
}

public class Summary
{
    public string total { get; set; }
}

public class Html
{
    public string pagination { get; set; }
    public string addresses { get; set; }
}
