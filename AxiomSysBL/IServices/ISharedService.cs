using System;
using AxiomSysBL.Models;

namespace AxiomSysBL.IServices;

public interface ISharedService
{
    ResponseModel HandleException(Exception exception);
}
