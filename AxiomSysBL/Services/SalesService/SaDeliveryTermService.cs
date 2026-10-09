using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxiomSysBL.IServices;
using AxiomSysBL.IServices.ISalesService;
using AxiomSysBL.Models;
using AxiomSysBL.Models.Sales;
using AxiomSysDL.Contexts;
using AxiomSysDL.Models.SupplyChain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiomSysBL.Services.SalesService;

public class SaDeliveryTermService : ISaDeliveryTermService
{
    private readonly SupplyChainContext _context;
    private readonly ISharedService _sharedService;
    private readonly ILogger<SaDeliveryTermService> _logger;

    public SaDeliveryTermService(
        SupplyChainContext context,
        ISharedService sharedService,
        ILogger<SaDeliveryTermService> logger)
    {
        _context = context;
        _sharedService = sharedService;
        _logger = logger;
    }

    public async Task<List<SaDeliveryTermModel>> GetSaDeliveryTermsAsync()
    {
        // Do not catch and swallow database exceptions here into an empty list.
        // If the database fails, the exception must propagate to the caller.
        return await _context.SaDeliveryTerms
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new SaDeliveryTermModel
            {
                Code = x.Code,
                SName = x.SName,
                BName = x.BName,
                Days = x.Days,
                ActiveFlag = x.ActiveFlag,
                EntryUser = x.EntryUser,
                EntryDate = x.EntryDate,
                ChangeUser = x.ChangeUser,
                ChangeDate = x.ChangeDate
            })
            .ToListAsync();
    }

    public async Task<SaDeliveryTermModel?> GetSaDeliveryTermAsync(int code)
    {
        var entity = await _context.SaDeliveryTerms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code);

        if (entity == null)
        {
            return null;
        }

        return new SaDeliveryTermModel
        {
            Code = entity.Code,
            SName = entity.SName,
            BName = entity.BName,
            Days = entity.Days,
            ActiveFlag = entity.ActiveFlag,
            EntryUser = entity.EntryUser,
            EntryDate = entity.EntryDate,
            ChangeUser = entity.ChangeUser,
            ChangeDate = entity.ChangeDate
        };
    }

    public async Task<ResponseModel> AddSaDeliveryTermAsync(SaDeliveryTermModel model, long userId)
    {
        var validationResponse = ValidateModel(model);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        try
        {
            // Pre-check for duplicate code before insertion
            bool exists = await _context.SaDeliveryTerms
                .AnyAsync(x => x.Code == model.Code);

            if (exists)
            {
                return new ResponseModel
                {
                    status = false,
                    messageEn = "A record with this code already exists.",
                    messageAr = "يوجد سجل بهذا الكود بالفعل."
                };
            }

            var entity = new SaDeliveryTerm
            {
                Code = model.Code,
                SName = model.SName.Trim(),
                BName = model.BName.Trim(),
                Days = model.Days,
                ActiveFlag = model.ActiveFlag ?? 1,
                // Server-side audit fields: Identity strictly from authenticated user, consistent UTC timestamp convention
                EntryUser = userId,
                EntryDate = DateTime.UtcNow,
                ChangeUser = null,
                ChangeDate = null
            };

            _context.SaDeliveryTerms.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                data = entity.Code,
                messageEn = "Added successfully.",
                messageAr = "تمت الإضافة بنجاح."
            };
        }
        catch (Exception ex)
        {
            // Handles duplicate key (ORA-00001) in case of concurrent insert race, ORA-12899, etc.
            return _sharedService.HandleException(ex);
        }
    }

    public async Task<ResponseModel> EditSaDeliveryTermAsync(SaDeliveryTermModel model, long userId)
    {
        var validationResponse = ValidateModel(model);
        if (validationResponse != null)
        {
            return validationResponse;
        }

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
                    messageAr = "لم يتم العثور على شرط التسليم."
                };
            }

            entity.SName = model.SName.Trim();
            entity.BName = model.BName.Trim();
            entity.Days = model.Days;
            entity.ActiveFlag = model.ActiveFlag ?? 1;

            // Preserve entry audit fields (ENTRY_USER and ENTRY_DATE) unchanged!
            // Stamp change audit fields from authenticated user and UTC timestamp
            entity.ChangeUser = userId;
            entity.ChangeDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                data = entity.Code,
                messageEn = "Updated successfully.",
                messageAr = "تم التعديل بنجاح."
            };
        }
        catch (Exception ex)
        {
            return _sharedService.HandleException(ex);
        }
    }

    public async Task<ResponseModel> DeleteSaDeliveryTermAsync(int code)
    {
        if (code < 1 || code > 999)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Invalid code. Code must be between 1 and 999.",
                messageAr = "كود غير صالح. يجب أن يكون الكود بين 1 و 999."
            };
        }

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
                    messageAr = "لم يتم العثور على شرط التسليم."
                };
            }

            _context.SaDeliveryTerms.Remove(entity);
            await _context.SaveChangesAsync();

            return new ResponseModel
            {
                status = true,
                data = code,
                messageEn = "Deleted successfully.",
                messageAr = "تم الحذف بنجاح."
            };
        }
        catch (Exception ex)
        {
            return _sharedService.HandleException(ex);
        }
    }

    private static ResponseModel? ValidateModel(SaDeliveryTermModel model)
    {
        if (model == null)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Payload is required.",
                messageAr = "البيانات مطلوبة."
            };
        }

        // Code: 1–999 (NUMBER(3) constraint in Oracle)
        if (model.Code < 1 || model.Code > 999)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Code must be between 1 and 999.",
                messageAr = "يجب أن يكون الكود بين 1 و 999."
            };
        }

        var sName = model.SName?.Trim() ?? string.Empty;
        var bName = model.BName?.Trim() ?? string.Empty;

        // Both names required after trimming
        if (string.IsNullOrWhiteSpace(sName))
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Arabic name is required.",
                messageAr = "الاسم بالعربية مطلوب."
            };
        }

        if (string.IsNullOrWhiteSpace(bName))
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "English name is required.",
                messageAr = "الاسم بالإنجليزية مطلوب."
            };
        }

        // Oracle SA_DELIVERY_TERMS has S_NAME VARCHAR2(60) and B_NAME VARCHAR2(60) with BYTE semantics (NLS_LENGTH_SEMANTICS=BYTE, AL32UTF8).
        // Each Arabic character in UTF-8 occupies 2 bytes. Max 60 bytes allowed.
        int sNameBytes = Encoding.UTF8.GetByteCount(sName);
        if (sNameBytes > 60)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = $"Arabic name exceeds maximum database size of 60 bytes (current: {sNameBytes} bytes in UTF-8).",
                messageAr = $"الاسم بالعربية يتجاوز الحد الأقصى لطول العمود في قاعدة البيانات (60 بايت؛ الحالي: {sNameBytes} بايت)."
            };
        }

        int bNameBytes = Encoding.UTF8.GetByteCount(bName);
        if (bNameBytes > 60)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = $"English name exceeds maximum database size of 60 bytes (current: {bNameBytes} bytes).",
                messageAr = $"الاسم بالإنجليزية يتجاوز الحد الأقصى لطول العمود في قاعدة البيانات (60 بايت؛ الحالي: {bNameBytes} بايت)."
            };
        }

        // Days: optional, otherwise 0–9999 (NUMBER(4) constraint)
        if (model.Days.HasValue && (model.Days.Value < 0 || model.Days.Value > 9999))
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Delivery days must be between 0 and 9999.",
                messageAr = "يجب أن تكون أيام التسليم بين 0 و 9999."
            };
        }

        // ActiveFlag: explicitly 0 or 1 for submitted writes
        if (model.ActiveFlag.HasValue && model.ActiveFlag.Value != 0 && model.ActiveFlag.Value != 1)
        {
            return new ResponseModel
            {
                status = false,
                messageEn = "Active status must be either 1 (Active) or 0 (Inactive).",
                messageAr = "يجب أن تكون حالة التفعيل إما 1 (نشط) أو 0 (غير نشط)."
            };
        }

        return null;
    }
}
