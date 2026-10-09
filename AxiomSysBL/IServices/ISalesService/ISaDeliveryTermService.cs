using AxiomSysDL.Models;
using AxiomSysDL.Models.SupplyChain;

namespace AxiomSysBL.IServices.ISalesService;

public interface ISaDeliveryTermService
{
    Task<List<SaDeliveryTerm>> GetSaDeliveryTermsAsync();

    Task<SaDeliveryTerm?> GetSaDeliveryTermAsync(int code);

    Task<ResponseModel> AddSaDeliveryTermAsync(
        SaDeliveryTerm model, long userId);

    Task<ResponseModel> EditSaDeliveryTermAsync(
        SaDeliveryTerm model, long userId);

    Task<ResponseModel> DeleteSaDeliveryTermAsync(int code);
}