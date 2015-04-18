using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFDocumentGeneration.Models
{
    public class AccountOptionsModelcs
    {
        public string AccountType { get; set; }

        public string AccountDetail { get; set; }
        public double money { get; set; }

        public LoanAgreementType loanAgreementType { get; set; }
    }
}