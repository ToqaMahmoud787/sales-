using AxiomSysDL.Models;
using AxiomSysDL.Models.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace AxiomSysBL.IServices.ISalesService
{
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
}
