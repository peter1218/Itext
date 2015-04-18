using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PDFDocumentGeneration.Models
{
    public enum LoanAgreementType
    {
        Basic,
        Standard,
        [Description("Stand Plus")]
        StandardPlus,
        [Description("Prestige Access")]
        PrestigeAccess
    }
}