using System.Collections.Generic;
using System.Threading.Tasks;
using AxiomSysBL.Models;
using AxiomSysBL.Models.Sales;

namespace AxiomSysBL.IServices.ISalesService;

public interface ISaDeliveryTermService
{
    Task<List<SaDeliveryTermModel>> GetSaDeliveryTermsAsync();

    Task<SaDeliveryTermModel?> GetSaDeliveryTermAsync(int code);

    Task<ResponseModel> AddSaDeliveryTermAsync(
        SaDeliveryTermModel model,
        long userId);

    Task<ResponseModel> EditSaDeliveryTermAsync(
        SaDeliveryTermModel model,
        long userId);

    Task<ResponseModel> DeleteSaDeliveryTermAsync(int code);
}
