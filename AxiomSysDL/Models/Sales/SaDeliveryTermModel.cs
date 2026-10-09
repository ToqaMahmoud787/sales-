using System;
using System.Collections.Generic;
using System.Text;

namespace AxiomSysDL.Models.Sales
{
    
    public class SaDeliveryTermModel
    {
        public int Code { get; set; }

        public string SName { get; set; } = string.Empty;

        public string BName { get; set; } = string.Empty;

        public int? Days { get; set; }

        public int? ActiveFlag { get; set; }
    }
}

