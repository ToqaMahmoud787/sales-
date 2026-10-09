namespace AxiomSysBL.Models;

public class ResponseModel
{
    public bool status { get; set; }
    public object? data { get; set; }
    public string messageAr { get; set; } = string.Empty;
    public string messageEn { get; set; } = string.Empty;
}
