namespace AxiomSysBL.IServices;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? CompanyCode { get; }
    bool IsAuthenticated { get; }
}
