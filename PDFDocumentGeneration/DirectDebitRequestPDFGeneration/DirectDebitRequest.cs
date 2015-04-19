using PDFDocumentGeneration.Common;
using PDFDocumentGeneration.Models;
using PDFDocumentGeneration.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PDFDocumentGeneration.DirectDebitRequestPDFGeneration
{
    public class DirectDebitRequest : ItextAgreementBase
    {
       

        PdfPTable DirectDebitServiceAgreementTable;
        PdfPTable LegalAdviceTable;
        PdfPTable DirectDebitServiceAgreementTablePartTwo;
        Chunk chunk;
        PdfPTable DirectDebitRequestTable;// table used to add how much borrow and annual percentage rates



        List<AccountOptionsModelcs> accountList = FakeRepository.accountList();
        AccountOptionsModelcs model;
        List<AccountOptionsModelcs> accountRepaytmentList = FakeRepository.accountRepaymentList();
        public DirectDebitRequest()
        {
            //table = getBaseTable();
            AccountOptionsModelcs manager1 = new AccountOptionsModelcs();
            manager1.loanAgreementType = LoanAgreementType.Basic;
            manager1.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + " Variable rate account with flexible repayment option";
            manager1.money = 150;
            model = manager1;

        } //Class Constructor 


        public void GetPdf()
        {
            generatDirectDebitServiceAgreementTablePartTwo();
            generateDirectDebitServiceAgreementTable();
            generateDirectDebitRequest();

            Lender lender = new Lender() { Name = "Neil", ACN = "12345", Australian_Credit_Licence = "L98765" };
            Manager manager = new Manager() { Name = "Mikal", ACN = "12345", Australian_Credit_Licence = "L98765" };
            Borrower borrower = new Borrower() { Name = "Peter", Address = "101 Miller Street North Sydney" };


            //PdfPTable LenderManageBorrowerTable = CreateDocumentLenderManagerBorrowerTable(lender, borrower, manager);
            //PdfPTable BorrowerPleaseNoteTable = CreateBorrowerPleaseNoteTable();
            //PdfPTable DocumentLimitationtable = CreateDocumentLimitationTable();

            //createHowMuchBorrowingTable(accountList);
            //createAnualPercentageRateTable(accountList);

            //CreateFeesAndChargesTable();
            //CreateOtherInformationTable();
            PdfWriter writer = PdfWriter.GetInstance(debitDocument, new FileStream(("C:\\Peter\\DirectDebit" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"), FileMode.Create));
            writer.PageEvent = new PageEventHelper();
            //CreateRepaymentsFinalTable(LoanAgreementType.Basic);
            //ImportantTable();
            debitDocument.Open();
            //debitDocument.AddHeader()


            //  Header header=new Header(,"hello" );
            debitDocument.Add(DirectDebitRequestTable);
            debitDocument.NewPage();
            debitDocument.Add(DirectDebitServiceAgreementTable);
            debitDocument.Add(DirectDebitServiceAgreementTablePartTwo);
            //debitDocument.Add(BorrowerPleaseNoteTable);

            //debitDocument.Add(LoanAgreementTitle);

            //debitDocument.Add(LenderManageBorrowerTable);

            //debitDocument.Add(LenderAndManagerSection);

            //debitDocument.Add(debitDocumentLimitationtable);

            //debitDocument.Add(SpecialCharacterMeaning);

            //debitDocument.Add(FinancialTableHeadingPartA);
            //debitDocument.Add(FinancialTableHeadingPartB);

            //debitDocument.Add(table);
            //debitDocument.Add(RepaymentsTable);
            //debitDocument.Add(FeesAndChargesTable);
            //debitDocument.Add(otherInformationTable);
            //debitDocument.Add(declarationTable);
            //debitDocument.NewPage();
            //debitDocument.Add(AcceptOffer());
            //debitDocument.Add(MustSignParagraph());
            //debitDocument.Add(WarningParagraph());
            //debitDocument.Add(important);
            //debitDocument.Add(signatureTable);
            //debitDocument.Add(notAbtainLegalAdviceTable);

            //debitDocument.Add(LegalAdviceTable);
            debitDocument.Close();
        }//main function to generate the pdf files


        public void generateDirectDebitRequest()
        {
            DirectDebitRequestTable = new PdfPTable(2);
         
            DirectDebitRequestTable.TotalWidth = intTableWidth;
            DirectDebitRequestTable.LockedWidth = true;
            float[] widths = new float[] { 24f, 76f };

            DirectDebitRequestTable.SetWidths(widths);


            pCell = getDisableBorderCell(1, 1, 1, 1);
            pCell.Colspan = 2;
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            chunk = getChunk("Request and Authority to debit the account named below to pay ", font10);
            paragraph.Add(chunk);
            chunk = getChunk("Well Nigh", font10Bold);
            paragraph.Add(chunk);
            pCell.AddElement(paragraph);
            DirectDebitRequestTable.AddCell(pCell);

            pCell = getCell();
            pCell.PaddingTop = 15;
            chunk = getChunk("Request and Authority to debit", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            pCell.AddElement(paragraph);

            DirectDebitRequestTable.AddCell(pCell);

            pCell = getCell();
            chunk = getChunk("Your Surname or company name:", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            chunk = getChunk("____________________________________\n\n", font10Bold);
            paragraph.Add(chunk);
            chunk = getChunk("Your Given names or ABN/ARBN", font10Bold);
            paragraph.Add(chunk);

            chunk = getChunk("____________________________________\n\n", font10Bold);
            paragraph.Add(chunk);

            chunk = getChunk(@"you request and authorise", font10);
            paragraph.Add(chunk);

            chunk = getChunk(" Well Nigh & userid 438648 ", font10Bold);
            paragraph.Add(chunk);

            chunk = getChunk("to arrange, through its ownfinancial institution, a debit to your nominated account any amount ", font10);
            paragraph.Add(chunk);
            chunk = getChunk("Well Nigh", font10Bold);

            paragraph.Add(chunk);
            chunk = getChunk(@", has deemed payable by you.
This debit or charge will be made through the Bulk Electronic Clearing System (BECS) from your account held at the financial institution you have nominated below and will be subject to the terms and conditions of the Direct Debit Request Service Agreement", font10);
            paragraph.Add(chunk);

            pCell.AddElement(paragraph);
            pCell.PaddingTop = 15;
            DirectDebitRequestTable.AddCell(pCell);



            chunk = getChunk("Insert the name and address of financial institution at which account is held", font10Bold);
            pCell = getCell();
            pCell.AddElement(chunk);
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("Financial institution name", font10Bold));
            paragraph.Add(new Chunk("_______________________________________\n\n",font10Bold));
            paragraph.Add(new Chunk("Address", font10Bold));
            paragraph.Add(new Chunk("_________________________________________________________"+"\n\n"+"_____________________________________________________ ",font10Bold));
            pCell = getCell();
            pCell.PaddingBottom = 10;
            pCell.PaddingTop = 10;
            pCell.AddElement(paragraph);
            DirectDebitRequestTable.AddCell(pCell);





            chunk = getChunk("Insert details of account be debited", font10Bold);
            pCell = getCell();
            pCell.PaddingTop = 15;
            pCell.AddElement(chunk);
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("Name/s on account", font10Bold));
            paragraph.Add(new Chunk("_______________________________________\n\n", font10Bold));
            paragraph.Add(new Chunk("BSB number (Must be 6 Digits) ", font10Bold));
            paragraph.Add(new Chunk("123456\n\n", font10Bold));

            paragraph.Add(new Chunk("Account number            ", font10Bold));

            paragraph.Add(new Chunk("             23423423423423",font10Bold));


            pCell = getCell();
            pCell.PaddingBottom = 10;
            pCell.PaddingTop = 15;
            pCell.AddElement(paragraph);
            DirectDebitRequestTable.AddCell(pCell);




            chunk = getChunk("Acknowledgment", font10Bold);
            pCell = getCell();
            pCell.AddElement(chunk);
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("By signing and/or providing us with a valid instruction in respect to your Direct Debit Request, you have understood and agreed to the terms and conditions governing the debit arrangements between you and", font10));
            paragraph.Add(new Chunk(" Well Nigh ", font10Bold));
            paragraph.Add(new Chunk("as set out in this Request and in yourDirect Debit Request Service Agreement. ", font10));
            pCell = getCell();
            pCell.AddElement(paragraph);
            DirectDebitRequestTable.AddCell(pCell);






            chunk = getChunk("Insert your signature and address", font10Bold);
         
            pCell = getCell();
            pCell.PaddingTop = 15;
            pCell.AddElement(chunk);
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("Signature", font10Bold));
            paragraph.Add(new Chunk("_______________________________________\n", font10Bold));

            paragraph.Add(new Chunk("If signing for a company, sign and print full name and capacity for signing eg. director\n ", font10));
            paragraph.Add(new Chunk("Address", font10Bold));
            paragraph.Add(new Chunk("_____________________________________________________"+"\n\n"+"______________________________________________ "+"\n\n", font10Bold));
            paragraph.Add(new Chunk("Date", font10Bold));
            paragraph.Add(new Chunk("          _____/_____/_____/", font10Bold));
            pCell = getCell();
            pCell.PaddingBottom = 15;
            pCell.AddElement(paragraph);
            pCell.PaddingTop = 15;
            DirectDebitRequestTable.AddCell(pCell);






            chunk = getChunk("Second account signatory (if required", font10Bold);
            pCell = getCell();
            pCell.PaddingTop = 15;
            pCell.AddElement(chunk);
            DirectDebitRequestTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("Signature", font10Bold));
            paragraph.Add(new Chunk("_______________________________________\n\n", font10Bold));
            paragraph.Add(new Chunk("Address", font10Bold));
            paragraph.Add(new Chunk("_________________________________________________________"+"\n\n"+"____________________________________________________ "+"\n\n", font10Bold));
            paragraph.Add(new Chunk("Date", font10Bold));
            paragraph.Add(new Chunk("           _____/______/______/", font10Bold));
            pCell = getCell();
            pCell.PaddingTop = 15;
            pCell.PaddingBottom = 15;
            pCell.AddElement(paragraph);
            DirectDebitRequestTable.AddCell(pCell);




        }
        public void generateDirectDebitServiceAgreementTable()
        {
            DirectDebitServiceAgreementTable = new PdfPTable(2);

         

            DirectDebitServiceAgreementTable.TotalWidth = intTableWidth;
            DirectDebitServiceAgreementTable.LockedWidth = true;
            float[] widths = new float[] { 24f, 76f };
            DirectDebitServiceAgreementTable.SpacingBefore = 30;
            DirectDebitServiceAgreementTable.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("This is your Direct Debit Service Agreement with "+" Well Nigh, User Id 438648 & 46 131 937 632."+" It explains whatyour obligations are when undertaking a Direct Debit arrangement with us. It also details what our obligations are toyou as your Direct Debit provider.",font10));
            pCell = getDisableBorderCell(1, 1, 1, 1);
            pCell.AddElement(paragraph);
            pCell.Colspan = 2;
            paragraph = para();
            paragraph.Add(new Chunk("Please keep this agreement for future reference. It forms part of the terms and conditions of your Direct Debit Request (DDR) and should be read in conjunction with your DDR authorisation.",font10));
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 10;
            DirectDebitServiceAgreementTable.AddCell(pCell);

            pCell = getCell();
            pCell.AddElement(new Paragraph("Definitions", font10Bold));
            DirectDebitServiceAgreementTable.AddCell(pCell);
            pCell = getCell();
            chunk = getChunk("account", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk(" means the account held at your financial institution from which we are authorised to arrange for funds to be debited.",font10));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);
            chunk = getChunk("agreement", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk(" means this Direct Debit Request Service Agreement between you and us."));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);

            chunk = getChunk("banking day", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk("means a day other than a Saturday or a Sunday or a public holiday listed throughout Australia."));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);

            chunk = getChunk("debit day", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk("means the day that payment by you to us is due"));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);


            chunk = getChunk("debit payment", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk("means a particular transaction where a debit is made."));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);



            chunk = getChunk("direct debit request", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);
            paragraph.Add(new Chunk("means the Direct Debit Request between us and you."));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);

            chunk = getChunk("us", font10Bold);
            paragraph = para();
            paragraph.Add(chunk);

            paragraph.Add(" or ");
            paragraph.Add(new Chunk("we",font10Bold));
            paragraph.Add(new Chunk(" means" ,font10));
            paragraph.Add(new Chunk(" Well Nigh ", font10Bold));
            paragraph.Add(new Chunk(", (the Debit User) you have authorised by requesting a Direct Debit Request. ",font10));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);

            paragraph=para();
            paragraph.Add(new Chunk("you",font10Bold));
            paragraph.Add(new Chunk(" means the customer who has signed or authorised by other means the Direct Debit Request.",font10));
            paragraph.SpacingAfter = 5;
            pCell.AddElement(paragraph);
            paragraph=para();
            paragraph.Add(new Chunk("you financial institution ",font10Bold));
            paragraph.Add(new Chunk("means the financial institution nominated by you on the DDR at which the account is maintained.",font10));
            paragraph.SpacingAfter = 5;



            pCell.AddElement(paragraph);
            DirectDebitServiceAgreementTable.AddCell(pCell);

//            chunk=getChunk("1. Debiting your accounts",font10Bold);
//            pCell=getCell();
//            pCell.AddElement(chunk);
//            DirectDebitServiceAgreementTable.AddCell(pCell);

//            pCell=getCell();



//            pCell.AddElement(new Paragraph(@"
//1.1      By signing a Direct Debit Request or by providing us with a valid instruction, you
//         have authorised us to arrange for funds to be debited from your account. You should
//         refer to the Direct Debit Request and this agreement for the terms of the
//         arrangement between us and you.
//1.2      We will only arrange for funds to be debited from your account as authorised in the
//         Direct Debit Request.
//         or
//         We will only arrange for funds to be debited from your account if we have sent to the
//         address nominated by you in the Direct Debit Request, a billing advice which specifies
//         the amount payable by you to us and when it is due.
//1.3      If the debit day falls on a day that is not a banking day, we may direct your financial
//         institution to debit your account on the following banking day. If you are unsure about
//         which day your account has or will be debited you should ask your financial
//         institution.",font10));

//            DirectDebitServiceAgreementTable.AddCell(pCell);
        }
        public void generatDirectDebitServiceAgreementTablePartTwo()
        {
            DirectDebitServiceAgreementTablePartTwo = new PdfPTable(3);



            DirectDebitServiceAgreementTablePartTwo.TotalWidth = intTableWidth;
            DirectDebitServiceAgreementTablePartTwo.LockedWidth = true;
            float[] widths = new float[] { 24f, 8f,68f };
         
            DirectDebitServiceAgreementTablePartTwo.SetWidths(widths);

            chunk = getChunk("1. Debiting your accounts", font10Bold);
                     pCell=getCell();
                     pCell.Rowspan = 3;
                     pCell.AddElement(chunk);
                   DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);

                   pCell = getCell();
                   pCell.AddElement(new Chunk("1.1", font10));
                   DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
                   pCell = getCell();
            pCell.AddElement(new Paragraph(@"By signing a Direct Debit Request or by providing us with a valid instruction, you have authorised us to arrange for funds to be debited from your account. You should refer to the Direct Debit Request and this agreement for the terms of the arrangement between us and you.",font10     ));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);

            pCell = getCell();
            pCell.AddElement(new Chunk("1.2", font10));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
            pCell = getCell();
            pCell.AddElement(new Paragraph(@"We will only arrange for funds to be debited from your account as authorised in the Direct Debit Request.", font10));
            pCell.AddElement(new Paragraph("or", font10Bold));
            pCell.AddElement(new Paragraph(@"We will only arrange for funds to be debited from your account if we have sent to the address nominated by you in the Direct Debit Request, a billing advice which specifies the amount payable by you to us and when it is due.",font10         ));
           
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);

            pCell = getCell();
            pCell.AddElement(new Paragraph(@"1.3", font10));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
            pCell = getCell();
            pCell.AddElement(new Paragraph(@"If the debit day falls on a day that is not a banking day, we may direct your financial institution to debit your account on the following banking day. If you are unsure about which day your account has or will be debited you should ask your financial institution.", font10));

            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);







            chunk = getChunk("2. Amendments by us", font10Bold);
            pCell = getCell();
            //pCell.Rowspan = 3;
            pCell.AddElement(chunk);
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);

            pCell = getCell();
            pCell.AddElement(new Chunk("2.1", font10));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
            pCell = getCell();
            paragraph = para();
            paragraph.Add(new Chunk("We may vary any details of this agreement or a Direct Debit Request at any time by giving you at least fourteen ", font10));
            paragraph.Add(new Chunk("(14) days ", font10Bold));
            paragraph.Add(new Chunk("written notice.", font10));
            pCell.AddElement(paragraph  );

            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);







            chunk = getChunk("3. Amendments by you", font10Bold);
            pCell = getCell();
            //pCell.Rowspan = 3;
            pCell.AddElement(chunk);
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);

            pCell = getCell();
            pCell.AddElement(new Chunk("", font10));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
            pCell = getCell();
            paragraph = para();
            paragraph.Add(new Chunk("You may change, stop or defer a debit payment, or terminate this agreement by providing us with at least", font10));
            paragraph.Add(new Chunk(" 5 days ", font10Bold));
            paragraph.Add(new Chunk("notification by writing to:", font10));
            paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);

            pCell.AddElement(new Chunk("Po Box 1962, North Sydney NSW 2059", font10Bold));
            pCell.AddElement(new Chunk("or", font10Bold));
            paragraph = para();
            paragraph.Add(new Chunk("by telephoning us on", font10));
            paragraph.Add(new Chunk(" 02 8116 1010 ", font10Bold));
            paragraph.Add(new Chunk("during business hours;",font10));
            pCell.AddElement(paragraph);
            pCell.AddElement(new Chunk("or", font10Bold));
            pCell.AddElement(new Chunk("arranging it through your own financial institution, which is required to act promptly on your instructions.", font10));
            DirectDebitServiceAgreementTablePartTwo.AddCell(pCell);
        }
        #region  Header And Footer Helper Section

        public class PageEventHelper : PdfPageEventHelper
        {
            BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/ARIAL.TTF", BaseFont.CP1252, BaseFont.EMBEDDED);



            Font ffont = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);

            public override void OnStartPage(PdfWriter writer,Document debitDocument)
            {
                base.OnStartPage(writer, debitDocument);
            }
            //
            public override void OnEndPage(PdfWriter writer, Document debitDocument)
            {
                int pageN = writer.PageNumber;
                PdfContentByte cb = writer.DirectContent;
                Font font8Bold = new Font(bf, 8, Font.BOLD);
                Font font8 = new Font(bf, 8);
                Phrase header = new Phrase("Money Home Loan Agreement-" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year, font8Bold);
                Phrase footer = new Phrase("13859586.14  AOC AOC                                                                                                                                                               " + pageN, font8);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                        header,
                        debitDocument.Right - 100,
                        debitDocument.Top + 25, 0);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                        footer,
                        220 + debitDocument.LeftMargin,
                        debitDocument.Bottom - 10, 0);
            }

            public override void OnCloseDocument(PdfWriter writer, Document debitDocument)
            {
                base.OnCloseDocument(writer, debitDocument);
                // important to avoid closing the stream, YES!
                writer.CloseStream = true;
            }
        }
        #endregion
    }
}