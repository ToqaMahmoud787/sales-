namespace AxiomSysDL.Models;

public class ResponseModel
{
    public bool status { get; set; }
    public string? messageEn { get; set; }
    public string? messageAr { get; set; }
    public object? data { get; set; }
}