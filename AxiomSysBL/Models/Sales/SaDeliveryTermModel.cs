using System;

namespace AxiomSysBL.Models.Sales;

public class SaDeliveryTermModel
{
    public int Code { get; set; }

    public string SName { get; set; } = string.Empty;

    public string BName { get; set; } = string.Empty;

    public int? Days { get; set; }

    public int? ActiveFlag { get; set; }

    public long? EntryUser { get; set; }

    public DateTime? EntryDate { get; set; }

    public long? ChangeUser { get; set; }

    public DateTime? ChangeDate { get; set; }
}
