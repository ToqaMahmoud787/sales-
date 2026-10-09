using AxiomSysBL.IServices;
using AxiomSysBL.IServices.ISalesService;
using AxiomSysDL.Models;
using AxiomSysDL.Models.SupplyChain;
using AxiomSysSL.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxiomSysSL.Controllers.Sales;

[Authorize]
[AuthorizeCompany]
[ApiController]
[Route("api/Sales/[controller]")]
[ApiExplorerSettings(GroupName = "Sales")]
public sealed class SaDeliveryTermController(
    ISaDeliveryTermService service,
    ISharedService sharedService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("GetSaDeliveryTerms")]
    public async Task<IActionResult> List()
    {
        if (currentUser.UserId == null) return Unauthorized();
        try
        {
            return Ok(new ResponseModel
            {
                status = true,
                data = await service.GetSaDeliveryTermsAsync()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, sharedService.HandleException(ex));
        }
    }

    [HttpGet("GetSaDeliveryTerm")]
    public async Task<IActionResult> Get(int code)
    {
        if (currentUser.UserId == null) return Unauthorized();
        try
        {
            var result = await service.GetSaDeliveryTermAsync(code);
            if (result == null)
                return NotFound(new ResponseModel
                {
                    status = false,
                    messageEn = "Delivery term not found.",
                    messageAr = "لم يتم العثور على شروط التسليم."
                });
            return Ok(new ResponseModel { status = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, sharedService.HandleException(ex));
        }
    }

    [HttpPost("AddSaDeliveryTerm")]
    public async Task<IActionResult> Add(SaDeliveryTerm model)
    {
        if (currentUser.UserId is not long userId) return Unauthorized();
        return Ok(await service.AddSaDeliveryTermAsync(model, userId));
    }

    [HttpPost("EditSaDeliveryTerm")]
    public async Task<IActionResult> Edit(SaDeliveryTerm model)
    {
        if (currentUser.UserId is not long userId) return Unauthorized();
        return Ok(await service.EditSaDeliveryTermAsync(model, userId));
    }

    public sealed record DeleteRequest(int Code);

    [HttpPost("DeleteSaDeliveryTerm")]
    public async Task<IActionResult> Delete(DeleteRequest request)
    {
        if (currentUser.UserId == null) return Unauthorized();
        return Ok(await service.DeleteSaDeliveryTermAsync(request.Code));
    }
}
