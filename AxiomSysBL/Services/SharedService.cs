using AxiomSysBL.IServices;using AxiomSysDL.Models;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace AxiomSysBL.Services;

public class SharedService : ISharedService
{
    private readonly ILogger<SharedService> _logger;

    public SharedService(ILogger<SharedService> logger)
    {
        _logger = logger;
    }

    public ResponseModel HandleException(Exception exception)
    {
        _logger.LogError(
            exception,
            "An error occurred while processing a database operation.");

        // Oracle exceptions may be wrapped by Entity Framework.
        OracleException? oracleException = null;

        for (Exception? current = exception;
             current != null;
             current = current.InnerException)
        {
            if (current is OracleException oracle)
            {
                oracleException = oracle;
                break;
            }
        }

        if (oracleException?.Number == 1)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "A record with this code already exists.",
                messageAr = "يوجد سجل بهذا الكود بالفعل."
            };
        }

        return new ResponseModel
        {
            status = false,
            messageEn = "An unexpected error occurred. Please try again.",
            messageAr = "حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى."
        };
    }
}