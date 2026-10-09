using System.Text.Json;
namespace AxiomSysPortal.Models;

public class DeliveryTerm
{
    public int Code { get; set; }
    public string SName { get; set; } = "";
    public string BName { get; set; } = "";
    public int? Days { get; set; }
    public int? ActiveFlag { get; set; } = 1;
}

public class ApiResponse
{
    public bool Status { get; set; }
    public JsonElement Data { get; set; }
    public string MessageEn { get; set; } = "";
    public string MessageAr { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public DateTimeOffset ExpiresUtc { get; set; }
    public string Username { get; set; } = "";
    public string CompanyCode { get; set; } = "";
}
