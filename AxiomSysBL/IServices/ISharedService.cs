using AxiomSysDL.Models;

namespace AxiomSysBL.IServices;

public interface ISharedService
{
    ResponseModel HandleException(Exception exception);
}