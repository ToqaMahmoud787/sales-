using AxiomSysDL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AxiomSysBL.IServices
{

    public interface ISharedService
    {
        ResponseModel HandleException(Exception exception);
    }

}
