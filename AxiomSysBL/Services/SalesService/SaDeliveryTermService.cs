using AxiomSysBL.IServices;
using AxiomSysBL.IServices.ISalesService;
using AxiomSysDL.Contexts;
using AxiomSysDL.Models;

using AxiomSysDL.Models.SupplyChain;
using Microsoft.EntityFrameworkCore;

namespace AxiomSysBL.Services.SalesService;

public class SaDeliveryTermService : ISaDeliveryTermService
{
    private readonly SupplyChainContext _context;
    private readonly ISharedService _sharedService;

    public SaDeliveryTermService(
        SupplyChainContext context,
        ISharedService sharedService)
    {
        _context = context;
        _sharedService = sharedService;
    }

    public async Task<List<SaDeliveryTerm>> GetSaDeliveryTermsAsync()
    {
        return await _context.SaDeliveryTerms
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync();
    }

    public async Task<SaDeliveryTerm?> GetSaDeliveryTermAsync(int code)
    {
        return await _context.SaDeliveryTerms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<ResponseModel> AddSaDeliveryTermAsync(
        SaDeliveryTerm model,
        long userId)
    {
        try
        {
            bool exists = await _context.SaDeliveryTerms
                .AnyAsync(x => x.Code == model.Code);

            if (exists)
            {
                return new ResponseModel
                {
                    status = false,
                    messageEn = "Delivery term code already exists.",
                    messageAr = "كود شرط التسليم موجود بالفعل."
                };
            }

            var entity = new SaDeliveryTerm
            {
                Code = model.Code,
                SName = model.SName,
                BName = model.BName,
                Days = model.Days,
                ActiveFlag = model.ActiveFlag,
                EntryUser = userId,
                EntryDate = DateTime.Now
            };

            await _context.SaDeliveryTerms.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                messageEn = "Delivery term added successfully.",
                messageAr = "تمت إضافة شرط التسليم بنجاح.",
                data = entity
            };
        }
        catch (Exception ex)
        {
            return _sharedService.HandleException(ex);
        }
    }

    public async Task<ResponseModel> EditSaDeliveryTermAsync(
        SaDeliveryTerm model,
        long userId)
    {
        try
        {
            var entity = await _context.SaDeliveryTerms
                .FirstOrDefaultAsync(x => x.Code == model.Code);

            if (entity == null)
            {
                return new ResponseModel
                {
                    status = false,
                    messageEn = "Delivery term not found.",
                    messageAr = "شرط التسليم غير موجود."
                };
            }

            entity.SName = model.SName;
            entity.BName = model.BName;
            entity.Days = model.Days;
            entity.ActiveFlag = model.ActiveFlag;

            entity.ChangeUser = userId;
            entity.ChangeDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                messageEn = "Delivery term updated successfully.",
                messageAr = "تم تعديل شرط التسليم بنجاح.",
                data = entity
            };
        }
        catch (Exception ex)
        {
            return _sharedService.HandleException(ex);
        }
    }

    public async Task<ResponseModel> DeleteSaDeliveryTermAsync(int code)
    {
        try
        {
            var entity = await _context.SaDeliveryTerms
                .FirstOrDefaultAsync(x => x.Code == code);

            if (entity == null)
            {
                return new ResponseModel
                {
                    status = false,
                    messageEn = "Delivery term not found.",
                    messageAr = "شرط التسليم غير موجود."
                };
            }

            _context.SaDeliveryTerms.Remove(entity);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                messageEn = "Delivery term deleted successfully.",
                messageAr = "تم حذف شرط التسليم بنجاح."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("========== ADD DELIVERY TERM ERROR ==========");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("=============================================");

            return _sharedService.HandleException(ex);
        }
    }
}