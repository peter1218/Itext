using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PDFDocumentGeneration;
using PDFDocumentGeneration.LoanAgreement;
namespace PDFDocumentGeneration
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             // LoanAgreementPDFGenerator loan=new LoanAgreementPDFGenerator()
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
          LoanAgreementPdfGenerator loan=new LoanAgreementPdfGenerator();
          loan.GetPdf();
        }
    }
}