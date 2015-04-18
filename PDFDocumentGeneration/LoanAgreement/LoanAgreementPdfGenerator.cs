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

namespace PDFDocumentGeneration.LoanAgreement
{


    public class LoanAgreementPdfGenerator : ItextAgreementBase
    {
        PdfPTable LegalAdviceTable;
        PdfPTable notAbtainLegalAdviceTable;
        PdfPTable signatureTable;
        PdfPTable otherInformationTable;
        PdfPTable FeesAndChargesTable;
        PdfPTable RepaymentsTable;
        PdfPTable important;
        Chunk Chunk;
        PdfPTable table;// table used to add how much borrow and annual percentage rates
        PdfPTable declarationTable;


        List<AccountOptionsModelcs> accountList = FakeRepository.accountList();
        AccountOptionsModelcs model;
        List<AccountOptionsModelcs> accountRepaytmentList = FakeRepository.accountRepaymentList();
        public LoanAgreementPdfGenerator()
        {
            Rectangle pageSize = new Rectangle(PageSize.A4);
            document = new Document(pageSize, 48, 48, 95, 25);
            table = getBaseTable();
            AccountOptionsModelcs manager1 = new AccountOptionsModelcs();
            manager1.loanAgreementType = LoanAgreementType.Basic;
            manager1.AccountType = "Account A:     " + manager1.loanAgreementType.ToString() + " Variable rate account with flexible repayment option";
            manager1.money = 150;
            model = manager1;

        } //Class Constructor 


        public void GetPdf()
        {

            AbtainLegalAdviceTable();
            notToAbtainLegalAdviceTable();
            AgreementDeclarationsTable();
            SignatureTable();
            Lender lender = new Lender() { Name = "Neil", ACN = "12345", Australian_Credit_Licence = "L98765" };
            Manager manager = new Manager() { Name = "Mikal", ACN = "12345", Australian_Credit_Licence = "L98765" };
            Borrower borrower = new Borrower() { Name = "Peter", Address = "101 Miller Street North Sydney" };
            Paragraph LenderAndManagerSection = CreateDocumentLendAndManagerSection();
            Paragraph SpecialCharacterMeaning = CreateDocumentSpecialCharacterMeanings();
            Paragraph FinancialTableHeadingPartA = CreateDocumentFinancialTableHeadingsPartA(DateTime.Now);
            Paragraph FinancialTableHeadingPartB = CreateDocumentFinancialTableHeadingsPartB(DateTime.Now);
            Paragraph LoanAgreementTitle = CreateDocumentLoanAgreementTitleAndType(LoanAgreementType.Basic);

            PdfPTable LenderManageBorrowerTable = CreateDocumentLenderManagerBorrowerTable(lender, borrower, manager);
            PdfPTable BorrowerPleaseNoteTable = CreateBorrowerPleaseNoteTable();
            PdfPTable DocumentLimitationtable = CreateDocumentLimitationTable();

            createHowMuchBorrowingTable(accountList);
            createAnualPercentageRateTable(accountList);

            CreateFeesAndChargesTable();
            CreateOtherInformationTable();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(("C:\\Peter\\MortgageHouseAgreement" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"), FileMode.Create));
            writer.PageEvent = new PageEventHelper();
            CreateRepaymentsFinalTable(LoanAgreementType.Basic);
            ImportantTable();
            document.Open();
            //document.AddHeader()


            //  Header header=new Header(,"hello" );
            //document.Add(header);
            document.Add(BorrowerPleaseNoteTable);

            document.Add(LoanAgreementTitle);

            document.Add(LenderManageBorrowerTable);

            document.Add(LenderAndManagerSection);

            document.Add(DocumentLimitationtable);

            document.Add(SpecialCharacterMeaning);

            document.Add(FinancialTableHeadingPartA);
            document.Add(FinancialTableHeadingPartB);

            document.Add(table);
            document.Add(RepaymentsTable);
            document.Add(FeesAndChargesTable);
            document.Add(otherInformationTable);
            document.Add(declarationTable);
            document.NewPage();
            document.Add(AcceptOffer());
            document.Add(MustSignParagraph());
            document.Add(WarningParagraph());
            document.Add(important);
            document.Add(signatureTable);
            document.Add(notAbtainLegalAdviceTable);

            document.Add(LegalAdviceTable);
            document.Close();
        }//main function to generate the pdf files

        #region TableCreatingCollections
        public PdfPTable CreateBorrowerPleaseNoteTable()
        {

            PdfPTable table = new PdfPTable(1);

            table.TotalWidth = intTableWidth;
            table.LockedWidth = true;
            paragraph = para();
            paragraph.Add(new Chunk("Borrowers Please Note", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.SpacingAfter = 10;
            pCell = getCell();
            pCell.AddElement(paragraph);

            paragraph = para();
            paragraph.Add(new Chunk("We recommend that you consider obtaining legal and financial advice in relation to this loan.  If you have any questions ask before you sign.", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.SpacingAfter = 10;

            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk("If you repay all or part of a fixed rate loan early, significant fees (called ‘break costs’) may be payable.  If interest rates change, your repayments may change (except if your loan is fixed rate).  ", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.SpacingAfter = 10;

            pCell.AddElement(paragraph);

            paragraph = para();
            paragraph.Add(new Chunk("If you default you may lose your property.  You should insure your property.  You should consider whether you need other insurance such as insurance to assist you to make repayments if you are sick, lose your job, or other contingencies occur ", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.SpacingAfter = 10;

            pCell.AddElement(paragraph);

            pCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(pCell);
           
            table.SpacingAfter = 10;
            return table;
        }
        public PdfPTable CreateDocumentLenderManagerBorrowerTable(Lender lender, Borrower borrower, Manager manager)
        {
            int intCol = 2;
            PdfPTable table = new PdfPTable(2);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.KeepTogether = true;
            table.TotalWidth = intTableWidth;
            table.LockedWidth = true;
            float[] widths = new float[] { 25f, 75f };

            table.DefaultCell.Border = PdfPCell.NO_BORDER;
            //  table.DefaultCell.CellEvent = new RoundedBorder();
            table.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("Lender", font10Bold));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getCell();
            pCell.FixedHeight = 25f;
            pCell.AddElement(paragraph);
            pCell.BorderWidth = 0;
            table.AddCell(pCell);
            ;



            pCell = getCell();
            pCell.FixedHeight = 25f;
            paragraph = para();
            paragraph.Add(new Chunk(@lender.Name + " ACN " + lender.ACN + " (Australian Credit Licence " + lender.Australian_Credit_Licence, font10));
            pCell.AddElement(paragraph);
            pCell.BorderWidth = 0;
            table.AddCell(pCell);




            pCell = getCell();
            paragraph = para();
            pCell.BorderWidth = 0;
            pCell.FixedHeight = 25f;
            paragraph.Add(new Chunk("Manager", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);

            pCell = getCell();
            pCell.FixedHeight = 25f;
            paragraph = para();
            paragraph.Add(new Chunk(@manager.Name + " ACN " + manager.ACN + " (Australian Credit Licence " + manager.Australian_Credit_Licence, font10));
            pCell.BorderWidth = 0;
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);

            pCell = getCell();
            pCell.FixedHeight = 25f;
            paragraph = para();
            pCell.BorderWidth = 0;
            // big stuff
            paragraph.Add(new Chunk("Borrower (‘you’)", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);


            pCell = getCell();
            pCell.FixedHeight = 25f;
            pCell.BorderWidth = 0;
            paragraph = para();
            paragraph.Add(new Chunk(@borrower.Name + " of " + borrower.Address, font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);







            return table;
        }
        public PdfPTable CreateDocumentLimitationTable()
        {

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = intTableWidth;
            table.LockedWidth = true;
            PdfPCell cell = new PdfPCell();
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            Paragraph temp = new Paragraph("This document does not contain all the precontractual information required by law to be given to you. This document must be read together with the Moneyer General Terms & Conditions Version 1 dated February. 2015 (T&Cs) which forms part of this loan agreement.  You must perform all of the terms specified in the T&Cs.  If there is any conflict between the T&Cs and this document, the terms of this document prevail.  If there is any conflict between any provisions of any security or guarantee and this document and the T&Cs, the terms of this document and the T&Cs prevail.", font10);

            cell.AddElement(temp);
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.PaddingTop = -5;
            table.AddCell(cell);
            table.SpacingAfter = 10;
            table.SpacingBefore = 10;
            return table;


        }

        public void createHowMuchBorrowingTable(List<AccountOptionsModelcs> accountOptionModel)
        {

            // var list=  FakeRepository.accountList();


            paragraph = para();
            paragraph.Add(new Chunk("How much are you borrowing?", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getCell();
            pCell.FixedHeight = 17f;
            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            
            table.AddCell(pCell);
            pCell = getCell();
            pCell.FixedHeight = 25f;
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            paragraph.Add(new Chunk(" $", font10Bold));
            pCell.AddElement(paragraph);
            table.AddCell(pCell);
            pCell = getCell();
            pCell.FixedHeight = 25f;

            paragraph = para();
            paragraph.Add(new Chunk("Made up of:", font10));
            pCell.AddElement(paragraph);
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            table.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            pCell.FixedHeight = 25f;
            paragraph = para();
            pCell.AddElement(paragraph);
            table.AddCell(pCell);

            foreach (var item in accountOptionModel)
            {

                pCell = getCell();
                pCell.DisableBorderSide(1);
                pCell.DisableBorderSide(2);
                pCell.FixedHeight = 25f;
                paragraph = para();
                paragraph.Add(new Chunk(item.AccountType, font10));
                pCell.AddElement(paragraph);

                table.AddCell(pCell);

                pCell = getCell();
                pCell.DisableBorderSide(1);
                pCell.DisableBorderSide(2);
                pCell.FixedHeight = 25f;
                paragraph = para();
                paragraph.Add(new Chunk(" $", font10));
                pCell.AddElement(paragraph);

                table.AddCell(pCell);

            }

            pCell = getCell();
            pCell.DisableBorderSide(1);


            paragraph = para();
            paragraph.Add(new Chunk(@"Total Amount of Credit", font10Bold));
            pCell.PaddingBottom = 5;
            pCell.AddElement(paragraph);
            
            table.AddCell(pCell);

            
            pCell = getCell();
            pCell.DisableBorderSide(1);

            paragraph = para();
            paragraph.Add(new Chunk(" $", font10Bold));
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 5;
            table.AddCell(pCell);



            //return table;
        }
        public void notToAbtainLegalAdviceTable()
        {
            notAbtainLegalAdviceTable = new PdfPTable(1);

            notAbtainLegalAdviceTable.TotalWidth = intTableWidth;
            notAbtainLegalAdviceTable.LockedWidth = true;

            paragraph = para();
            paragraph.Add(new Chunk("COMPLETE ONE OF THESE BOXES", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            pCell = getDisableBorderCell(0, 1, 0, 0);
            pCell.AddElement(paragraph);

            notAbtainLegalAdviceTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("TO BE COMPLETED IF YOU CHOOSE ", font10Bold));
            paragraph.Add(new Chunk("NOT ", font10UnderLineBold));
            paragraph.Add(new Chunk("TO OBTAIN LEGAL ADVICE", font10Bold));

            paragraph.Alignment = Element.ALIGN_CENTER;


            pCell = getDisableBorderCell(1, 0, 1, 1);

            pCell.FixedHeight = 80;
            //paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk("IF YOU HAVE ANY DOUBTS OR WANT MORE INFORMATION, CONTACT YOUR GOVERNMENT CONSUMER AGENCY OR GET LEGAL ADVICE ", font10Bold));

            paragraph.Alignment = Element.ALIGN_CENTER;
            //paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            notAbtainLegalAdviceTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 1, 1);
            //pCell.Colspan = 2;


            pCell.AddElement(new Chunk("I/WE CERTIFY THAT:\n", font10));

            notAbtainLegalAdviceTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //pCell.Colspan = 2;
            notAbtainLegalAdviceTable.AddCell(pCell);

            pCell.FixedHeight = 15;


            List list = new List(List.UNORDERED);
            list.SetListSymbol("\u2022");

            list.Add(new ListItem("      I/we have read the loan agreement (including the Moneyer General Terms and Conditions) to which this" + "\n" +
                                  "      certificate is attached (the ‘Document’).", font10));
            list.Add(new ListItem("      I/we are the borrower(s) named in the Document.", font10));
            list.Add(new ListItem("      I/we have been given the opportunity to obtain legal advice on the nature and effect of the Document but\n" +
                                  "      have chosen not to do so.", font10));
            list.Add(new ListItem("      I/we understand the nature and effect of the Document.", font10));
            list.Add(new ListItem("      I/we understand the obligations and risks involved in signing the Document.", font10));
            list.Add(new ListItem("      I/we sign the Document freely, voluntarily and without pressure from any person.", font10));
            //list.IndentationLeft = 20;

            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(list);

            pCell.PaddingBottom = 25;
            notAbtainLegalAdviceTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();

            paragraph.Add(new Chunk(@"DATED:	       the.............................................day of............................................." + "\n", font10));

            pCell.AddElement(paragraph);
            notAbtainLegalAdviceTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();
            paragraph.Add(new Chunk(@"SIGNED:	      ................................................................................(borrower(s) signature)" + "\n", font10));
            pCell.AddElement(paragraph);
            notAbtainLegalAdviceTable.AddCell(pCell);
            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();
            paragraph.Add(new Chunk(@"                     ................................................................................(borrower(s) signature)" + "\n", font10));
            pCell.AddElement(paragraph);
            // paragraph.SpacingAfter = 20;
            // pCell.AddElement(paragraph);
            notAbtainLegalAdviceTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("      "));
            pCell = getDisableBorderCell(0, 1, 1, 1);
            pCell.AddElement(paragraph);
            notAbtainLegalAdviceTable.AddCell(pCell);

        }

        public void AbtainLegalAdviceTable()
        {
            LegalAdviceTable = new PdfPTable(1);

            LegalAdviceTable.TotalWidth = intTableWidth;
            LegalAdviceTable.LockedWidth = true;

            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("OR", font10Bold));
            paragraph.Alignment = Element.ALIGN_CENTER;
            pCell.FixedHeight = 25;
            pCell.AddElement(paragraph);
            LegalAdviceTable.AddCell(pCell);
            pCell = getDisableBorderCell(1, 0, 1, 1);

            pCell.FixedHeight = 60;
            //paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk("TO BE COMPLETED IF YOU CHOOSE TO OBTAIN LEGAL ADVICE", font10Bold));

            paragraph.Alignment = Element.ALIGN_CENTER;
            pCell = getDisableBorderCell(1, 0, 1, 1);
            //paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            LegalAdviceTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 1, 1);
            //pCell.Colspan = 2;


            pCell.AddElement(new Chunk("I/WE CERTIFY THAT:\n", font10));

            LegalAdviceTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //pCell.Colspan = 2;
            LegalAdviceTable.AddCell(pCell);

            pCell.FixedHeight = 15;


            List list = new List(List.UNORDERED);
            list.SetListSymbol("\u2022");

            list.Add(new ListItem("      I/we have obtained legal advice on the nature and effect of the document(s) from the solicitor named below\n", font10));
            list.Add(new ListItem("      I/we understand the nature and effect of the loan agreement (including the Moneyer General Terms and" + "\n" +
                                  "      Conditions) to which this certificate is attached (the ‘Document’).\n", font10));
            list.Add(new ListItem("      I/we understand the obligations and risks involved in signing the Document.\n", font10));
            list.Add(new ListItem("      I/we sign the Document freely, voluntarily and without pressure from any person.\n", font10));

            //list.IndentationLeft = 20;

            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(list);
            pCell.PaddingBottom = 15;
            LegalAdviceTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk(@"NAME OF SOLICITOR:......................................................................................." + "\n", font10));
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(paragraph);
            LegalAdviceTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();

            paragraph.Add(new Chunk(@"DATED:	       the.............................................day of............................................." + "\n", font10));

            pCell.AddElement(paragraph);
            LegalAdviceTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();
            paragraph.Add(new Chunk(@"SIGNED:	      ................................................................................(borrower(s) signature)" + "\n", font10));
            pCell.AddElement(paragraph);
            LegalAdviceTable.AddCell(pCell);
            pCell = getDisableBorderCell(0, 1, 1, 1);
            paragraph = para();
            paragraph.Add(new Chunk(@"                     ................................................................................(borrower(s) signature)" + "\n", font10));
            pCell.AddElement(paragraph);
            // paragraph.SpacingAfter = 20;
            // pCell.AddElement(paragraph);
            LegalAdviceTable.AddCell(pCell);

        }

        public void AgreementDeclarationsTable()
        {
            declarationTable = new PdfPTable(2);


            declarationTable.TotalWidth = intTableWidth;
            declarationTable.LockedWidth = true;
            float[] widths = new float[] { 10f, 90f };

            declarationTable.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("How to proceed", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 0, 0);
            pCell.FixedHeight = 60;
            pCell.PaddingBottom = 15;
            pCell.PaddingTop = 25;
            pCell.Colspan = 2;

            pCell.AddElement(paragraph);
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            declarationTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk sign = new Chunk("By signing this loan agreement, each of you has made the following declarations.", font10);
            pCell.AddElement(sign);

            pCell.Colspan = 2;
            declarationTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk = new Chunk("1.", font10);
            pCell.AddElement(Chunk);
            declarationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("You have carefully read this loan agreement and the T&Cs and understand they establish a legal contract between you and us.  ", font10);
            Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);
            paragraph.Add(Chunk);
            paragraph.Add(c2);
            pCell.AddElement(paragraph);

            declarationTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk = new Chunk("2.", font10);
            pCell.AddElement(Chunk);
            declarationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("All information you have given directly or indirectly to us, our agents, or our lawyers is accurate and not misleading.  You acknowledge that we are relying on that information to enter this transaction.  You acknowledge that you can service the loan without undue hardship. ", font10);
            //    Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);
            paragraph.Add(Chunk);
            //  paragraph.Add(c2);
            pCell.AddElement(paragraph);

            declarationTable.AddCell(pCell);










            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk = new Chunk("3.", font10);
            pCell.AddElement(Chunk);
            declarationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("Other than this loan agreement and the T&Cs, you have not relied on any promise or representation by anybody when deciding to enter this transaction. ", font10);
            //    Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);
            paragraph.Add(Chunk);
            //  paragraph.Add(c2);
            pCell.AddElement(paragraph);

            declarationTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk = new Chunk("4.", font10);
            pCell.AddElement(Chunk);
            declarationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("The loan will be used only for the purpose set out above under ‘Purpose’. ", font10);
            //    Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);
            paragraph.Add(Chunk);
            //  paragraph.Add(c2);
            pCell.AddElement(paragraph);

            declarationTable.AddCell(pCell);









            pCell = getDisableBorderCell(0, 0, 0, 0);
            Chunk = new Chunk("5.", font10);
            pCell.AddElement(Chunk);
            declarationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("You agree to pay the lender all fees and charges applicable (as set out under ‘What fees will you pay before or on settlement of your loan’) even if the loan does not proceed to settlement (including because we withdraw from this offer).", font10);
            //    Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);
            paragraph.Add(Chunk);
            //  paragraph.Add(c2);
            pCell.AddElement(paragraph);

            declarationTable.AddCell(pCell);





            pCell = getDisableBorderCell(0, 0, 0, 0);

            paragraph = para();
            Chunk = new Chunk("We reserve the right to withdraw from this transaction if this offer is not accepted within 30 days from the disclosure date on page 1 of this loan agreement, or if the initial drawdown does not occur within 90 days of that date, or if anything occurs which in our opinion makes settlement undesirable.", font10);
            //    Chunk c2 = new Chunk("If you have any questions, ask before you sign", font10Italic);

            paragraph.Add(Chunk);
            //  paragraph.Add(c2);
            pCell.AddElement(paragraph);
            pCell.Colspan = 2;

            declarationTable.AddCell(pCell);


        }
        public void ImportantTable()
        {
            important = new PdfPTable(4);



            important.TotalWidth = intTableWidth;
            important.LockedWidth = true;
            float[] widths = new float[] { 5f, 35f, 5f, 55f };
            //  pCell.DisableBorderSide
            important.SetWidths(widths);
            pCell = getDisableBorderCell(1, 0, 1, 0);
            pCell.Colspan = 3;
            important.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("IMPORTANT", font10Bold));
            // paragraph.Alignment = Element.ALIGN_CENTER;
            pCell = getDisableBorderCell(1, 0, 0, 1);
            // pCell.HorizontalAlignment = Element.ALIGN_CENTER;


            pCell.AddElement(paragraph);
            // pCell.HorizontalAlignment = Element.ALIGN_CENTER;
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            important.AddCell(pCell);




            pCell = getDisableBorderCell(0, 0, 1, 0);
            pCell.Colspan = 2;
            Chunk sign = new Chunk("BEFORE YOU SIGN", font10Bold);
            paragraph = para();
            paragraph.Add(sign);
            paragraph.Alignment = Element.ALIGN_CENTER;
            pCell.AddElement(paragraph);

            // pCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            important.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 1);
            Chunk = new Chunk("THINGS YOU MUST KNOW", font10Bold);
            pCell.Colspan = 2;
            paragraph = para();
            paragraph.Add(Chunk);
            paragraph.Alignment = Element.ALIGN_CENTER;

            pCell.AddElement(paragraph);

            // pCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 0);
            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("READ THIS CONTRACT DOCUMENT so that you know exactly what contract you are entering into and what you will have to do under the contract.", font10));
            important.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 1);

            pCell.AddElement(new Paragraph("Once you sign this contract document, you will be bound by it.  However, you may end the contract before you obtain credit, or a card or other means is used to obtain goods or services for which credit is to be provided under the contract, by telling the credit provider in writing, but you will still be liable for any fees or charges already incurred.", font10));
            important.AddCell(pCell);
            //pCell = getDisableBorderCell(0, 0, 0, 0);
            //List List2 = new List();
            //List2.Items.Add(new ListItem("Once you sign this contract document, you will be bound by it.  However, you may end the contract before you obtain credit, or a card or other means is used to obtain goods or services for which credit is to be provided under the contract, by telling the credit provider in writing, but you will still be liable for any fees or charges already incurred."));
            ////Chunk = new Chunk("*READ THIS CONTRACT DOCUMENT so that you know exactly what contract you are entering into and what you will have to do under the contract.", font10);
            //pCell.AddElement(List2);


            //importantTables.r



            pCell = getDisableBorderCell(0, 0, 1, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("You should also read the information statement:  ‘THINGS YOU SHOULD KNOW ABOUT YOUR PROPOSED CREDIT CONTRACT’.", font10));
            important.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 1);

            pCell.AddElement(new Paragraph("You do not have to take out consumer credit insurance unless you want to.  However, if this contract document says so, you must take out insurance over any mortgaged property, that is used as security, such as a house or car.", font10));
            important.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 1, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("Fill in or cross out any blank spaces.", font10));
            important.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 1);

            pCell.AddElement(new Paragraph("If you take out insurance, the credit provider cannot insist on any particular insurance company.", font10));
            important.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("Get a copy of this contract document.", font10));
            important.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 0, 1);

            pCell.AddElement(new Paragraph("	If this contract document says so, the credit provider can vary the annual percentage rate (the interest rate), the repayments and the fees and charges and can add new fees and charges without your consent.", font10));
            important.AddCell(pCell);





            pCell = getDisableBorderCell(0, 1, 1, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 1, 0, 0);

            pCell.AddElement(new Paragraph("	Do not sign this contract document if there is anything you do not understand.", font10));
            important.AddCell(pCell);



            pCell = getDisableBorderCell(0, 1, 0, 0);

            pCell.AddElement(new Paragraph("   *", font10));
            important.AddCell(pCell);

            pCell = getDisableBorderCell(0, 1, 0, 1);

            pCell.AddElement(new Paragraph("	If this contract document says so, the credit provider can charge a fee if you pay out your contract early.", font10));
            important.AddCell(pCell);

        }
        #region AnnualPercentageRateTableCreatingCollections

        public PdfPCell CreateAnnualPercentageRateAccountAOptionFirst(AccountOptionsModelcs model)
        {
            paragraph = para();
            pCell = getCell();
            pCell.DisableBorderSide(2);
            paragraph.Add(new Chunk("Account A-" + model.loanAgreementType.ToString() + " Variable rate account", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk("Your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date is : ", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.AddElement(Chunk.NEWLINE);



            paragraph = para();
            paragraph.Add(new Chunk(@"Your discount off the reference rate applies for [the life of your loan / a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time]. ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.AddElement(Chunk.NEWLINE);
            return pCell;
        }

        public PdfPCell CreateAnnualPercentageRateAccountAOptionSecond(AccountOptionsModelcs model)
        {
            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            paragraph.Add(new Chunk(@"Account A-" + model.loanAgreementType.ToString() + " Variable rate account ", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.AddElement(Chunk.NEWLINE);

            paragraph = para();
            paragraph.Add(new Chunk(@"Your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  Under your package, we have arranged an introductory rate for you. Your annual interest rate comprises the reference rate, less a discount of XX% per annum, so that your variable introductory rate at the disclosure date is: ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.AddElement(Chunk.NEWLINE);
            paragraph = para();
            paragraph.Add(new Chunk(@"Your discount off the reference rate applies for a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time. ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.AddElement(Chunk.NEWLINE);
            return pCell;
        }

        public void AnnualPercentageRateAccountDCell(AccountOptionsModelcs model)
        {
            pCell = getCell();
            pCell.DisableBorderSide(2);
            paragraph = para();
            paragraph.Add(new Chunk(@"Account D –" + model.loanAgreementType.ToString() + " Construction loan account", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(2);
            paragraph = para();
            pCell.AddElement(paragraph);
            table.AddCell(pCell);



            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();
            paragraph.Add(new Chunk(@"During the construction period, your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date  is:", font10));
            paragraph.SpacingAfter = 5;

            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk("Your discount off the reference rate is applied a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time.   ", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);

            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();

            paragraph.Add(new Chunk(" xxx % per annum"));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = 20;
            table.AddCell(pCell);




            pCell = getCell();
            pCell.DisableBorderSide(1);

            paragraph = para();
            paragraph.Add(new Chunk(@"After completion of construction, your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date is:", font10));
            paragraph.SpacingAfter = 10;

            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk(@"Your discount off the reference rate applies for [the life of your loan / a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time].  ", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);

            pCell = getCell();
            pCell.DisableBorderSide(1);
            paragraph = para();

            paragraph.Add(new Chunk(" xxx % per annum"));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = 20;
            table.AddCell(pCell);
        }





        public void AnnualPercentageRateAccountBCell(AccountOptionsModelcs model)
        {

            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("Account B-" + model.loanAgreementType.ToString() + " Fixed rate account", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);

            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);


            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            pCell.FixedHeight = 35;
            // big stuff
            paragraph.Add(new Chunk("The initial fixed rate period ends", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            table.AddCell(pCell);


            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("years from the settlement date on", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);


            table.AddCell(pCell);

            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("The fixed rate for this period will be set on or about the settlement date.  The rate at the disclosure date is:\n", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.FixedHeight = 35;
            table.AddCell(pCell);


            pCell = getCell();
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk(@"XXX% per annum (indicative fixed rate))", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            table.AddCell(pCell);

            pCell = getCell();

            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();

            // big stuff
            paragraph.Add(new Chunk(@"At the end of the fixed rate period, your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date is:
           
Your discount off the reference rate applies for [the life of your loan / a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time].", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);
            pCell.DisableBorderSide(1);

            pCell = getCell();
            paragraph = para();

            pCell.DisableBorderSide(1);

            paragraph.Add(new Chunk(@"XXX% per annum ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;







            pCell.AddElement(paragraph);
            pCell.PaddingTop = 30;
            table.AddCell(pCell);


        }


        public void AnnualPercentageRateAccountCCell(AccountOptionsModelcs model)
        {


            pCell = getCell();
            paragraph = para();

            // big stuff
            paragraph.Add(new Chunk(@"Account C -" + model.loanAgreementType.ToString() + " Line of credit account", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);
            pCell = DefaultEmptyCell(1, 1, 1, 1);
            table.AddCell(pCell);


            // keep continue


            pCell = getCell();
            paragraph = para();


            paragraph.Add(new Chunk(@"Your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date  is:       	
Your discount off the reference rate is applied for [the life of your loan / a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time].
", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            table.AddCell(pCell);

            pCell = RatePerAnnumCell(model);
            table.AddCell(pCell);
        }
        public void AnnualPercentageRateAccountECell(AccountOptionsModelcs model)
        {
            pCell = getCell();
            paragraph = para();
            paragraph.Add(new Chunk(@"Account E- Transitional loan account", font10Bold));
            paragraph.SpacingAfter = 7;

            pCell.AddElement(paragraph);


            paragraph = para();
            paragraph.Add(new Chunk(@"Your loan is priced by reference to the [Moneyer standard variable rate / Moneyer Low Doc variable rate], currently XX % per annum.  We are giving you a discount of XX% per annum, so that your variable rate at the disclosure date  is:    ", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;
            paragraph.SpacingAfter = 7;
            pCell.AddElement(paragraph);

            paragraph = para();
            paragraph.Add(new Chunk(@"Your discount off the reference rate applies for [the life of your loan / a period of XX months.  At the end of the discount period, your rate will revert to the annual reference rate at the time]. ", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;

            pCell.AddElement(paragraph);

            table.AddCell(pCell);



            pCell = RatePerAnnumCell(model);
            table.AddCell(pCell);
        }

        public void createAnualPercentageRateTable(List<AccountOptionsModelcs> accountType)
        {




            paragraph = para();
            paragraph.Add(new Chunk("What is the annual percentage rate(s)?", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getCell();
            
            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(pCell);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pCell = getCell();
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            // paragraph.Add(new Chunk("$", font10Bold));
            pCell.AddElement(paragraph);
            table.AddCell(pCell);



            pCell = getCell();
            paragraph = para();
            paragraph.Add(new Chunk("Interest rates (including fixed rates) may change prior to the settlement date.  Interest rates other than fixed rates can vary after the settlement date. Your annual interest rate will change if the reference rate changes.", font10));
            pCell.AddElement(paragraph);
            table.AddCell(pCell);


            pCell = DefaultEmptyCell(1, 1, 1, 1);

            table.AddCell(pCell);

            pCell = CreateAnnualPercentageRateAccountAOptionFirst(model);
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            table.AddCell(pCell);


            pCell = getCell();
            pCell = RatePerAnnumCell(model);
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            table.AddCell(pCell);

            pCell = CreateAnnualPercentageRateAccountAOptionSecond(model);
            table.AddCell(pCell);

            pCell = RatePerAnnumCell(model);
            pCell.DisableBorderSide(1);
            table.AddCell(pCell);


            AnnualPercentageRateAccountBCell(model);

            AnnualPercentageRateAccountCCell(model);
            AnnualPercentageRateAccountDCell(model);

            AnnualPercentageRateAccountECell(model);

        }

        #endregion

        #region repayments table creation


        PdfPCell FlexibleRepaymentOptionCellAForRepaymentTable()
        {
            pCell = getCell();
            pCell.Colspan = 2;
            paragraph = para();
            Chunk c1 = new Chunk("Flexible repayment option\n\n", font10Bold);
            Chunk c2 = new Chunk(@"This section only applies if your repayments are shown above as ‘flexible repayment option’.  

The ordinary principal and interest repayment amount for your loan is:

Each time you make an early repayment, or redraw from your account, your repayment amount may be recalculated so that your account balance is repaid within the loan term.  For example, if you make an early payment of $10,000, your next repayment will be less than the repayment that would have applied if the early repayment had not been made.  Your loan will be repaid faster, and you will pay less interest if you continue making ordinary repayments instead of the reduced repayment.  

Any amount in any offset account is disregarded for the purpose of calculating your new payment unless you are in an interest only period.  You should log into your online account for a display of your flexible repayment progress.  
", font10);


            paragraph.Add(c1);
            paragraph.Add(c2);
            pCell.AddElement(paragraph);
            return pCell;

        }

        PdfPCell FlexibleRepaymentOptionCellBForRepaymentTable(double money)
        {

            pCell = getCell();
            paragraph = para();
            Chunk c1 = new Chunk("\n\n$" + money, font10);
            paragraph.Add(c1);
            pCell.AddElement(paragraph);
            return pCell;
        }

        PdfPCell LineOfCreditFacilityCellAForRepaymentTable()
        {

            pCell = getCell();
            pCell.Colspan = 2;
            paragraph = para();
            Chunk c1 = new Chunk("Line of credit facility\n", font10Bold);

            Chunk c2 = new Chunk(@"You must make monthly interest only repayments on same day each month as the settlement date of an amount equal to interest for the previous monthcalculated on the daily balances of the amount you owe us at the applicable interest rate.

We can convert the loan to a variable rate loan with principal and interest repayments by giving you not less than 30 days written notice.

Your line of credit account will automatically convert to a variable rate loan with principal and interest repayments ten years after the settlement date.

The amount of your repayments at the end of the line of credit period is unascertainable at the disclosure date.  We will tell you how much they are before they become due.  They will be broadly equivalent to the amount required to repay either the loan balance at that time, or the total amount of credit at that time (at our option), together with interest over the balance of the loan term.  These repayments may change from time to time in accordance with interest rate changes and otherwise as specified in this loan agreement.", font10);
            Chunk c3 = new Chunk("\n\n" + "Warning – We may change your line of credit limit at any time without your consent.  ", font10Bold);
            paragraph.Add(c1);
            paragraph.Add(c2);
            paragraph.Add(c3);
            pCell.AddElement(paragraph);
            return pCell;
        }

        PdfPCell ListBoxOfConstructionLoanAccountCellForRepaymentTable()
        {
            paragraph = para();
            Chunk c1 = new Chunk("Construction loan account\n", font10Bold);
            Chunk c2 = new Chunk("Progressively advanced construction accounts are initially interest only.  The account converts to principal and interest repayments on the earlier of:\n\n", font10);
            paragraph.Add(c1);
            paragraph.Add(c2);

            pCell = getDisableBorderCell(1, 0, 1, 1);
            pCell.Colspan = 2;
            List list = new List(List.UNORDERED);
            list.SetListSymbol("\u2022");
            list.Add(new ListItem("   18 months after settlement", font10));
            list.Add(new ListItem("   the date we nominate which  will usually be at the end of the construction\n" + "   period, or", font10));
            list.Add(new ListItem("   when the account is fully drawn.", font10));

            list.IndentationLeft = 20;

            pCell.AddElement(paragraph);
            pCell.AddElement(list);
            return pCell;
        }

        PdfPCell InterestOnlyPeriodCellForConstuctionOfRepaymentTable()
        {
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.Colspan = 2;
            pCell.AddElement(new Chunk("You have selected an interest only period of:", font10));
            pCell.FixedHeight = 40;
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            return pCell;

        }
        PdfPCell PrincipalAndInterestRepaymentsCommenceDate()
        {

            pCell = getDisableBorderCell(0, 0, 1, 1);

            pCell.Colspan = 2;
            pCell.AddElement(new Chunk("At the end of the interest only period, principal and interest repayments commence on the next repayment date", font10));
            //pCell.FixedHeight = 35;
            return pCell;

        }

        PdfPCell OptionForTheSettlementDateOrPeriodDate()
        {

            Chunk c1 = new Chunk("settlement date", font10Italic);
            Chunk c2 = new Chunk("period end date", font10);
            paragraph = para();
            paragraph.Add(new Chunk("from the ", font10));
            paragraph.Add(c1);
            paragraph.Add(c2);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(paragraph);
            return pCell;

        }

        PdfPCell AmountOfReapymentCellForConstructionLoanAccountOfRepaymentTable()
        {

            pCell = getDisableBorderCell(0, 1, 1, 1);
            pCell.Colspan = 2;
            Chunk c1 = new Chunk(" disclosure date ", font10Italic);
            paragraph = para();
            paragraph.Add("\n");
            paragraph.Add(new Chunk("The amount of your repayments at the end of the construction period is unascertainable at the", font10));
            paragraph.Add(c1);
            paragraph.Add(new Chunk("We will tell you how much they are before they become due.  They will be broadly equivalent to the amount required to repay either the loan balance at that time, or the total amount of credit at that time (at our option), together with interest over the balance of the loan term.  These repayments may change from time to time in accordance with interest rate changes and otherwise as specified in this loan agreement.", font10));
            paragraph.SpacingAfter = 10;
            pCell.AddElement(paragraph);
            return pCell;
        }

        PdfPCell TransitionalLoanAccountForRepayment()
        {
            pCell = getCell();
            pCell.Colspan = 2;
            paragraph = para();
            paragraph.Add(new Chunk("Transitional loan account\n\n", font10Bold));
            paragraph.Add(new Chunk("Repayments on your transitional loan are interest only.  At the end of the transitional loan account term (see special conditions for the transitional loan account term), you must make a final repayment, being the entire balance of the transitional loan account including all interest, fees and charges. The amount of the final repayment is unascertainable at the disclosure date.  The amount will be broadly equivalent to the total loan balance outstanding, together with the last months’ interest payment, and any accrued fees or charges.  These repayments may change from time to time in accordance with interest rate changes and otherwise as specified in this loan agreement.", font10));
            pCell.AddElement(paragraph);
            return pCell;
        }
        public void createPepaymentsFirstPartTable(LoanAgreementType type)
        {

            // table.KeepTogether = true;

            RepaymentsTable = new PdfPTable(3);


            RepaymentsTable.TotalWidth = intTableWidth;
            RepaymentsTable.LockedWidth = true;
            float[] widths = new float[] { 15f, 60f, 25f };

            RepaymentsTable.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("Repayments", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getCell();
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pCell.Colspan = 2;
            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            RepaymentsTable.AddCell(pCell);

            pCell = getCell();
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            // paragraph.Add(new Chunk("$", font10Bold));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);



            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            pCell.Colspan = 2;
            paragraph = para();
            paragraph.Add(new Chunk("When must you make your first repayment?\n", font10Bold));
            paragraph.Add(new Chunk("Your first monthly repayment is due one month from the settlement date. ", font10));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);

            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            //pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            // paragraph.Add(new Chunk("$", font10Bold));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            pCell.Colspan = 2;
            paragraph = para();

            // big stuff
            paragraph.Add(new Chunk("When are they due?\n", font10Bold));

            paragraph.Add(new Chunk("You must make repayments on the same day each month as the settlement date.  You may make repayments more frequently if you wish for instance weekly or fortnightly – please see your T&Cs.  ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);

            RepaymentsTable.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);





            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            pCell.Colspan = 2;
            paragraph = para();

            // big stuff
            paragraph.Add(new Chunk("How many repayments will you make?\n", font10Bold));
            paragraph.SpacingAfter = 8;
            paragraph.Add(new Chunk("Assuming you make all monthly repayments  on the monthly due date, and do not make any early repayments or any redraws, the number of repayments you must make will be ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);
            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();

            paragraph.Add(new Chunk(" xxx repayments", font10));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = 25;
            RepaymentsTable.AddCell(pCell);


            pCell = getCell();
            pCell.Colspan = 2;
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("How much are your repayments?", font10Bold));


            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);



            pCell = getCell();
            pCell.Colspan = 2;
            paragraph = para();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            // big stuff
            Chunk c1 = new Chunk("If the interest rate changes on a variable account, your repayment amounts may change.", font10Bold);
            Chunk c2 = new Chunk("Based on the current interest rates, your repayments will be as described below.", font10);
            Phrase p1 = new Phrase();
            p1.Add(c2);
            p1.Add(c1);

            paragraph.Add(p1);

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);


            pCell = getCell();
            pCell.DisableBorderSide(1);
            pCell.DisableBorderSide(2);
            paragraph = para();
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);

        }
        public void createPepaymentsSecondPartTable(List<AccountOptionsModelcs> accountOptionModel)
        {




            //  RepaymentsTable.SetWidths(widths);

            foreach (var item in accountOptionModel)
            {
                switch (item.AccountType)
                {
                    case "Account A":
                        {
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(8);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.AccountType, font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);

                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.loanAgreementType + " " + item.AccountDetail, font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);

                            paragraph = para();

                            paragraph.Add(new Chunk("$XXX comprising principal and interest OR Interest only – see below", font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = DefaultEmptyCell(1, 1, 1, 1);
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(8);
                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(4);
                            paragraph = para();
                            paragraph.Add(new Chunk("with flexiable repayment option", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);

                            paragraph = para();
                            paragraph.Add(new Chunk("See flexiable reapyment option below", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);

                            break;
                        }
                    case "Account B":
                        {

                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);

                            pCell.DisableBorderSide(8);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.AccountType, font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.loanAgreementType.ToString() + " " + item.AccountDetail, font10));
                            paragraph.Add(new Chunk("\n" + "At the end of the fixed rate period, your repayments based on a variable rate of XXX% per annum will be $XXX.  If the variable rate changes, your repayments may change", font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();

                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk("$XXX", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            break;


                        }
                    case "Account C":
                        {

                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.FixedHeight = 35;
                            pCell.DisableBorderSide(8);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.AccountType, font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);

                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.loanAgreementType.ToString() + " " + item.AccountDetail, font10));
                            //  paragraph.Add(new Chunk("/nAt the end of the fixed rate period, your repayments based on a variable rate of XXX% per annum will be $XXX.  If the variable rate changes, your repayments may change", font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();

                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);

                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk("Line of Credit-see below", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            break;

                        }
                    case "Account D":
                        {
                            pCell = getCell();
                            pCell.FixedHeight = 35;
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(8);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.AccountType, font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.loanAgreementType.ToString() + " " + item.AccountDetail, font10));
                            //  paragraph.Add(new Chunk("/nAt the end of the fixed rate period, your repayments based on a variable rate of XXX% per annum will be $XXX.  If the variable rate changes, your repayments may change", font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();

                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk("Construction loan-see below", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            break;
                        }
                    case "Account E":
                        {
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(8);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.AccountType, font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();
                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            pCell.DisableBorderSide(4);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk(item.loanAgreementType.ToString() + " " + item.AccountDetail, font10));
                            //  paragraph.Add(new Chunk("/nAt the end of the fixed rate period, your repayments based on a variable rate of XXX% per annum will be $XXX.  If the variable rate changes, your repayments may change", font10));
                            pCell.AddElement(paragraph);

                            RepaymentsTable.AddCell(pCell);
                            pCell = getCell();

                            pCell.DisableBorderSide(1);
                            pCell.DisableBorderSide(2);
                            //pCell.FixedHeight = 25f;
                            paragraph = para();
                            paragraph.Add(new Chunk("Transitional loan-see below", font10));
                            pCell.AddElement(paragraph);
                            RepaymentsTable.AddCell(pCell);
                            break;
                        }
                }

            }

            pCell = getCell();
            paragraph = para();
            pCell.Colspan = 2;
            pCell.DisableBorderSide(1);

            pCell.DisableBorderSide(2);
            // big stuff
            paragraph.Add(new Chunk("Interest only periods\n", font10Bold));
            paragraph.Add(new Chunk("The interest only period ends:", font10));

            paragraph.SpacingAfter = 5;

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);
            pCell = getCell();
            pCell.DisableBorderSide(1);

            pCell.DisableBorderSide(2);
            paragraph = para();

            paragraph.Add(new Chunk(@"OPTION: XX years from the settlement date OR XX /XX / XX OR ‘N/A’", font10));
            pCell.AddElement(paragraph);

            RepaymentsTable.AddCell(pCell);
            pCell = getCell();
            paragraph = para();
            pCell.Colspan = 2;
            // big stuff
            paragraph.Add(new Chunk(@"During any interest only period, each monthly repayment is equal to interest for the previous month calculated on the daily balances of the amount you owe us at the applicable interest rate.
  
At the end of the interest only period, principal and interest repayments commence on the next repayment date.  We will tell you the amount of those repayments shortly before they commence.  Your repayments based on a variable interest rate of XXX% per annum will be $XXX.  If the variable rate Reference Rate changes, your repayments may change.  
", font10));


            paragraph.SpacingAfter = 5;

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell.AddElement(paragraph);
            RepaymentsTable.AddCell(pCell);
            pCell = getCell();

            paragraph = para();

            paragraph.Add(new Chunk(@""));
            pCell.AddElement(paragraph);

            RepaymentsTable.AddCell(pCell);


        }

        public void createPepaymentsThirdPartTable(LoanAgreementType type)
        {








            RepaymentsTable.AddCell(FlexibleRepaymentOptionCellAForRepaymentTable());
            RepaymentsTable.AddCell(FlexibleRepaymentOptionCellBForRepaymentTable(55));
            RepaymentsTable.AddCell(LineOfCreditFacilityCellAForRepaymentTable());
            RepaymentsTable.AddCell(DefaultEmptyCellBold());


        }


        public void CreateRepaymentsConstructionLoanAccountSection()
        {
            RepaymentsTable.AddCell(ListBoxOfConstructionLoanAccountCellForRepaymentTable());
            RepaymentsTable.AddCell(DefaultEmptyCell(1, 0, 1, 1));
            RepaymentsTable.AddCell(InterestOnlyPeriodCellForConstuctionOfRepaymentTable());
            RepaymentsTable.AddCell(DefaultEmptyCell(0, 0, 1, 1));

            RepaymentsTable.AddCell(PrincipalAndInterestRepaymentsCommenceDate());
            //RepaymentsTable.AddCell(DefaultEmptyCell(0, 0, 1, 1));
            RepaymentsTable.AddCell(OptionForTheSettlementDateOrPeriodDate());
            RepaymentsTable.AddCell(AmountOfReapymentCellForConstructionLoanAccountOfRepaymentTable());
            RepaymentsTable.AddCell(DefaultEmptyCell(0, 1, 1, 1));

        }

        public void CreateTransitionalLoanAccountForRepayment()
        {
            RepaymentsTable.AddCell(TransitionalLoanAccountForRepayment());
            RepaymentsTable.AddCell(DefaultEmptyCell(1, 1, 1, 1));
        }
        public void CreateRepaymentsFinalTable(LoanAgreementType type)
        {

            // table.KeepTogether = true;

            createPepaymentsFirstPartTable(type);
            createPepaymentsSecondPartTable(accountRepaytmentList);

            createPepaymentsThirdPartTable(type);
            CreateRepaymentsConstructionLoanAccountSection();
            CreateTransitionalLoanAccountForRepayment();
        }

        #endregion
        #region   FeesAndChargesTable
        public void CreateFeesAndChargesTable()
        {
            createFeesAndChargesFirstPartTable();
            createFeesAndChargesSecondPartTable();
            createFeesAndChargeThirdTable();
            createFeeAndChargeFourthTable();
            createFeeAndChargeFifthTable();
        }
        public void createFeesAndChargesFirstPartTable()
        {
            FeesAndChargesTable = new PdfPTable(2);


            FeesAndChargesTable.TotalWidth = intTableWidth;
            FeesAndChargesTable.LockedWidth = true;
            float[] widths = new float[] { 75f, 25f };

            FeesAndChargesTable.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("Fees and charges", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(1, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(1, 0, 1, 1);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("What fees will you play before or on settlement of your plan?", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(1, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(1, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Unless otherwise stated all fees are non-refundable.  These fees may be payable even if the loan does not proceed for any reason. ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);





            paragraph = para();
            paragraph.Add(new Chunk("Application fee.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk applicationFeeChunk = new Chunk("$" + "applicationFee", font10);
            paragraph.Add(applicationFeeChunk);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Settlement fee.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk SettlementFeeChunk = new Chunk("$" + "200", font10);
            paragraph.Add(SettlementFeeChunk);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("Valuation fee", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk ValuationFeeChunk = new Chunk("$" + "200", font10);
            paragraph.Add(ValuationFeeChunk);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();
            //   Chunk GuarantorFeeChunk = new Chunk("Guarantor fee payable per guarantor if we take a guarantee in respect of this loan.", font10);
            paragraph.Add(new Chunk("Guarantor fee payable per guarantor if we take a guarantee in respect of this loan.", font10));
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk GuarantorFeeChunk = new Chunk("$" + "200", font10);
            paragraph.Add(GuarantorFeeChunk);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph = para();
            //   Chunk GuarantorFeeChunk = new Chunk("Guarantor fee payable per guarantor if we take a guarantee in respect of this loan.", font10);
            paragraph.Add(new Chunk("Document preparation costs and out of pocket expenses", font10));
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk DocumentFee = new Chunk("$" + "200", font10);
            paragraph.Add(DocumentFee);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);




        }
        public void createFeesAndChargesSecondPartTable()
        {
            paragraph = para();
            paragraph.Add(new Chunk("Account maintenance fee for the first month ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);

            FeesAndChargesTable.AddCell(pCell);
            pCell = getDisableBorderCell(0, 0, 1, 1);
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk AccountMainttenanceFeeChunk = new Chunk("$" + "200", font10);

            paragraph.Add(AccountMainttenanceFeeChunk);
            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Facility package fee payable annually in advance. ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk FacilityChunk = new Chunk("$" + "200", font10);

            paragraph.Add(FacilityChunk);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Lenders mortgage insurance ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk InsuranceChunk = new Chunk("$" + "200", font10);

            paragraph.Add(InsuranceChunk);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);





            paragraph = para();
            paragraph.Add(new Chunk("Global risk fee", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk riskFee = new Chunk("$" + "200", font10);

            paragraph.Add(riskFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Mortgage stamp duty", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk stampDuty = new Chunk("$" + "200", font10);

            paragraph.Add(stampDuty);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Construction administration fee ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk administrationFee = new Chunk("$" + "200", font10);

            paragraph.Add(administrationFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Total of fees and charges payable on or before the settlement date (excluding unascertainable amounts ", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk total = new Chunk("$" + "200", font10);

            paragraph.Add(administrationFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("What fees are definitely payable after settlement of your loan?", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk feeAfterSettle = new Chunk("", font10);

            paragraph.Add(feeAfterSettle);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Facility package fee payable annually in advance on the anniversary of the settlement date ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk c1 = new Chunk("$395 per annum ", font10);

            paragraph.Add(c1);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Monthly account maintenance fee, payable on the same day as the settlement date each month ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk maintenance = new Chunk("$" + 500 + " per annum ", font10);

            paragraph.Add(maintenance);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Offset sub-account fee debited monthly in arrears on the same day each month as interest is debited to your account.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk offset = new Chunk("An amount equal to interest on" + 55 + "% of the notional daily credit balance of your offset sub-account during the relevant period calculated at the rate applicable to relevant linked account. ", font10);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph.Add(offset);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);




        }

        public void createFeesAndChargeThirdTable()
        {

            paragraph = para();
            paragraph.Add(new Chunk("Total fees payable assuming the loan runs for the entire loan term (excluding any unascertainable amounts or amounts which may or may not become payable)", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk totalFee = new Chunk("$" + "xxx", font10Bold);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph.Add(totalFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("What fees are payable throughout my loan?", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk ctemp = new Chunk("", font10Bold);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph.Add(totalFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);




            paragraph = para();
            paragraph.Add(new Chunk("The following fees and charges are payable when the service is provided, the expense incurred or the relevant event occurs unless otherwise specified.  We can require you to pay the fee or charge immediately, debit it to your account, or collect it with your regular repayments.  Unless otherwise stated all fees and charges are non-refundable.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk cL = new Chunk("", font10Bold);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph.Add(cL);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("See Terms and Conditions for details about transaction fee allowances.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk Term = new Chunk("", font10);

            paragraph.Add(Term);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Break costs", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk BreakCost = new Chunk("Unascertainable", font10);

            paragraph.Add(BreakCost);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);




            paragraph = para();
            paragraph.Add(new Chunk("Break costs may be  payable if, during a fixed rate period:\n", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);

            List BreakList = new List(List.UNORDERED);

            BreakList.SetListSymbol("\u2022");

            BreakList.Add(new ListItem(" the whole of the loan to which a fixed rate applies is repaid.  In this case, this fee is payable on the date of that repayment, or\n", font10));
            BreakList.Add(new ListItem(" any part of the loan to which a fixed rate applies is repaid ahead of the scheduled repayments.  [OPTION:If this fee is payable, it will be debited to your fixed account on the date of that repayment. END OPTION] If the total amount repaid ahead of the scheduled repayments in any 12 month period does not exceed $20,000 break costs do not apply, or if the whole or part of your fixed rate loan is changed by agreement to another type of annual percentage rate.", font10));


            BreakList.IndentationLeft = 20;
            pCell.PaddingTop = 10;
            pCell.PaddingBottom = 10;
            pCell.AddElement(BreakList);
            paragraph = para();
            paragraph.SpacingBefore = 10;
            paragraph.Add(new Chunk("Break costs may be  payable on fixed rate loans even if repayment is because of a demand by us after default." + "\n\n", font10));

            paragraph.Add(new Chunk("Break costs are further explained in the General Terms and Conditions", font10));

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;


            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk cost = new Chunk("", font10);



            paragraph.Add(cost);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);




            paragraph = para();
            paragraph.Add(new Chunk("Valuation fee payable if we decide to obtain a valuation of a security property for any reason.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk valuationFeeChunk = new Chunk("$" + "xxx", font10);

            paragraph.Add(valuationFeeChunk);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Construction inspection fee payable if we decide to inspect construction for any reason ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk inspection = new Chunk("$" + "xxx", font10);

            paragraph.Add(inspection);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);



        }
        public void createFeeAndChargeFourthTable()
        {

            paragraph = para();
            paragraph.Add(new Chunk("Replacement card fee if your cash card is lost, damaged, or stolen and you require a new cash card", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);
            //
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk DishonourFee = new Chunk("$25 per card", font10);

            paragraph.Add(DishonourFee);

            pCell.AddElement(paragraph);
            FeesAndChargesTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("Dishonour fee if we are unable to draw money from a direct debit authority you have provided to us, payable when we are unable to draw money.  In addition, you may be liable for enforcement expenses or for a Declined 3rd party direct debit fee payable when a third party makes a direct debit request and there are insufficient funds in the respective account to honour the third party request.", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            //  pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk Dishouour = new Chunk("$35 per dishonour", font10);

            paragraph.Add(Dishouour);

            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Over the limit fee payable each time an account, or sub- account exceeds its approved limit", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            //  pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk LimitFee = new Chunk("$70 per over the limit event", font10);

            paragraph.Add(LimitFee);

            pCell.AddElement(paragraph);
            // pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Manual processing fee payable if and when we are required to manually transact on  your account  ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk MannualProcessingFee = new Chunk("$100 per transaction", font10);

            paragraph.Add(MannualProcessingFee);

            pCell.AddElement(paragraph);
            // pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);


            paragraph = para();
            paragraph.Add(new Chunk("Arrears notice fee payable if we are required to administer your arrears account and issue an arrears notice    ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk ArrearsNoticeFee = new Chunk("$25 per arrears notice", font10);

            paragraph.Add(ArrearsNoticeFee);

            pCell.AddElement(paragraph);
            // pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);




        }

        public void createFeeAndChargeFifthTable()
        {
            paragraph = para();
            paragraph.Add(new Chunk("Discharge fee payable per security if and when we have taken a security interest with respect to this loan and we are required to discharge our security interest ", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk DischargeFee = new Chunk("$" + "xxx" + " per discharged security ", font10);
            // Chunk c2 = new Chunk("third party fees", font10Italic);


            paragraph.Add(DischargeFee);
            //   paragraph.Add(c2);
            pCell.AddElement(paragraph);
            //  pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            paragraph.Add(new Chunk("Government charges, taxes and duties (none of which are known at the ", font10));
            paragraph.Add(disclosureDate);
            paragraph.Add(" )");
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();
            Chunk ArrearsNoticeFee = new Chunk("Unascertainable", font10);
            //   Chunk c2 = new Chunk("third party fees", font10Italic);


            paragraph.Add(ArrearsNoticeFee);
            // paragraph.Add(c2);
            pCell.AddElement(paragraph);
            //  pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);






            paragraph = para();
            paragraph.Add(new Chunk("Special attendance fee (e.g. consent to second mortgage, lease, variation or substitution of security, discharge of mortgage)", font10));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            Chunk chunk1 = new Chunk("$" + "XXX" + " per attendance plus ", font10);
            Chunk chunk2 = new Chunk("third party fees", font10Italic);


            paragraph.Add(chunk1);
            paragraph.Add(chunk2);
            pCell.AddElement(paragraph);
            //  pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);




            paragraph = para();
            paragraph.Add(thirdParty);
            paragraph.Add(new Chunk(" are fees incurred by us in providing the service and include such costs as valuation fees, mortgage insurance premium, Lender’s risk and processing fee, legal costs, document custodian charges and titles office fees all of which are unascertainable at the ", font10));
            paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            //   Chunk chunk1 = new Chunk("Unascertainable", font10);


            paragraph.Add(unascertainable);

            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);



            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("If any payment to the Lender is for a taxable supply for the purposes of GST or any similar tax, you must also pay to the Lender on demand an additional amount equal to the tax relating to that supply ", font10));
            //paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);
            //     pCell.FixedHeight = 35;

            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            paragraph = para();

            //   Chunk chunk1 = new Chunk("Unascertainable", font10);


            paragraph.Add(unascertainable);

            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            FeesAndChargesTable.AddCell(pCell);




            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("Acting reasonably, we can change any of the financial information described above without your consent, including the fees and charges, the amount of repayments, the dates for debiting interest and the dates for making repayments, and interest rates (except during a fixed rate period).  We may introduce new fees and charges without your consent.  The reference rate will be published on our website at all times at www. XXX.  We will inform you of any changes either in writing or by advertisement in a newspaper circulating throughout your jurisdiction.  In making any changes, we will act reasonably. ", font10Bold));
            // paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(1, 1, 1, 1);

            pCell.Colspan = 2;
            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            FeesAndChargesTable.AddCell(pCell);

            FeesAndChargesTable.SpacingAfter = 25;


        }
        #endregion


        #region OtherInformationTable
        public void CreateOtherInformationTable()
        {
            createOtherInformationFirstPartTable();
            createOtherInformationSecondPartTable();
            createOtherInformationThirdPartTable();
        }
        public void createOtherInformationFirstPartTable()
        {


            otherInformationTable = new PdfPTable(2);


            otherInformationTable.TotalWidth = intTableWidth;
            otherInformationTable.LockedWidth = true;
            float[] widths = new float[] { 25f, 75f };

            otherInformationTable.SetWidths(widths);

            paragraph = para();
            paragraph.Add(new Chunk("OTHER INFORMATION", font10Bold));
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 0, 0);
            pCell.FixedHeight = 60;
            pCell.PaddingBottom = 15;
            pCell.PaddingTop = 25;
            pCell.Colspan = 2;

            pCell.AddElement(paragraph);
            // pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);



            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("What are you giving as security for your loan?", font10Bold));
            // paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(1, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("You acknowledge that the following security extends to and secures any money due under this loan agreement.  This security is called the mortgaged property.", font10));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(1, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            // pCell = getDisableBorderCell(1, 0, 1, 1);


            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("", font10Bold));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);





            paragraph = para();
            pCell = getDisableBorderCell(0, 0, 1, 1);
            List list = new List(List.ORDERED);
            list.SetListSymbol("\u2022");
            list.Add(new ListItem(" 	First registered mortgage by " + "Mortaagor" + " over " + "Description", font10));
            list.Add(new ListItem(" 	First registered mortgage by " + "Mortaagor" + " over " + "address of security].", font10));
            list.Add(new ListItem("   	Guarantee by " + "Someone", font10));


            //  paragraph.Add(thirdParty);


            pCell.AddElement(list);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);



            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("What is your loan term?", font10Bold));

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);





            paragraph = para();


            pCell = getDisableBorderCell(0, 0, 1, 1);
            paragraph.Add((new Chunk("XXX" + " years commencing on the ", font10)));
            paragraph.Add(settlementDate);
            //  pCell.AddElement(new Chunk("XXX" + " years commencing on the ", font10));
            pCell.AddElement(paragraph);



            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);




            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("What is your loan being used for?", font10Bold));
            // paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);





            paragraph = para();


            pCell = getDisableBorderCell(0, 0, 1, 1);

            pCell.AddElement(new Chunk("You have told us that the loan will be used for " + " loanPuropse", font10));
            // pCell.AddElement(settlementDate);



            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);



            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("How will the loan be paid on settlement?", font10Bold));
            // paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);





            paragraph = para();
            paragraph.Add(new Chunk("The loan will paid to:" + "\n", font10));

            pCell = getDisableBorderCell(0, 0, 1, 1);
            List loanPaidTo = new List(List.ORDERED);
            loanPaidTo.SetListSymbol("\u2022");
            loanPaidTo.Add(new ListItem(" 	a suspense account for payment to the mortgage insurer:  $" + "XXX", font10));
            loanPaidTo.Add(new ListItem(" 	XXX ", font10));
            loanPaidTo.Add(new ListItem("  Balance: unascertainable at the disclosure date.", font10));

            loanPaidTo.IndentationLeft = 20;


            pCell.AddElement(paragraph);
            pCell.AddElement(loanPaidTo);


            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("What commissions are paid or received in respect to your loan?", font10Bold));
            // paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            //  pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("The Lender may pay an introduction fee and a fee for ongoing management to the Manager, the amount of which is unascertainable at the disclosure date.  The Lender and other persons may pay or receive other commissions, fees or benefits in connection with this loan.", font10));
            //  paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);


            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("If you default, will a different rate apply?", font10Bold));
            //  paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            //  pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            Chunk defaultC1 = new Chunk("The default rate of interest at any time equals the interest rate applying to the relevant account at the time plus " + "XXX" + " % per annum.  If the interest rate applying to the account changes, the default rate will also change." + "\n\n", font10);
            Chunk defautlC2 = new Chunk("Your loan is priced by reference to the " + " [Moneyer standard variable rate / Moneyer Low Doc variable rate]. " + "Based on your current rate at the disclosure date less any discount (if applicable) of XX% per annum, your current default rate is " + "XX" + "% per annum.  This default rate will change if the reference rate changes.", font10);
            paragraph.Add(defaultC1);
            paragraph.Add(defautlC2);
            //  paragraph.Add(disclosureDate);

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);



            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("About interest rates", font10Bold));
            //  paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            //  pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            Chunk interestRates = new Chunk(@"We obtain funding for our loans from a variety of sources.  As a result, interest rates may differ from time to time between our different loans.  Accordingly, you may see us advertising a different rate to the rate applicable to your loan.

Our interest rates are set having regard to your credit history and your relationship with us.  If you have many accounts or large accounts with us, or if you perform on time all the time, a lower interest rate may apply.  Conversely, we may set a higher interest rate for smaller loans or poor performance.
", font10);
            // Chunk defautlC2=new Chunk("Your loan is priced by reference to the "+" [Moneyer standard variable rate / Moneyer Low Doc variable rate]. "+"Based on your current rate at the disclosure date less any discount (if applicable) of XX% per annum, your current default rate is "+"XX"+"% per annum.  This default rate will change if the reference rate changes.",font10);
            paragraph.Add(interestRates);
            //paragraph.Add(defautlC2);
            //  paragraph.Add(disclosureDate);

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);



        }
        public void createOtherInformationSecondPartTable()
        {

            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("105% offset sub-account", font10Bold));
            //paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            //  pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            Chunk offset = new Chunk("If you have an offset sub-account, you will receive a reduction in the interest debited to your relevant linked account each time interest is debited  equal to interest on 5% of the notional credit of your offset account during the relevant period calculated at the rate applicable to relevant linked account.  ", font10);
            // Chunk defautlC2 = new Chunk("Your loan is priced by reference to the " + " [Moneyer standard variable rate / Moneyer Low Doc variable rate]. " + "Based on your current rate at the disclosure date less any discount (if applicable) of XX% per annum, your current default rate is " + "XX" + "% per annum.  This default rate will change if the reference rate changes.", font10);
            paragraph.Add(offset);
            //paragraph.Add(defautlC2);
            //  paragraph.Add(disclosureDate);

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);








        }
        public void createOtherInformationThirdPartTable()
        {

            paragraph = para();
            // paragraph.Add(thirdParty);
            paragraph.Add(new Chunk("Are there any special conditions?", font10Bold));
            //paragraph.Add(disclosureDate);
            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            //  pCell = getDisableBorderCell(1, 0, 1, 1);



            paragraph = para();
            //  paragraph.Add(thirdParty);
            Chunk specialCondition = new Chunk("This loan agreement is subject to the following special conditions being met prior to the ", font10);
            // Chunk defautlC2 = new Chunk("Your loan is priced by reference to the " + " [Moneyer standard variable rate / Moneyer Low Doc variable rate]. " + "Based on your current rate at the disclosure date less any discount (if applicable) of XX% per annum, your current default rate is " + "XX" + "% per annum.  This default rate will change if the reference rate changes.", font10);
            paragraph.Add(specialCondition);
            paragraph.Add(settlementDate);
            //paragraph.Add(defautlC2);
            //  paragraph.Add(disclosureDate);

            paragraph.Alignment = Element.ALIGN_LEFT;
            pCell = getDisableBorderCell(0, 0, 1, 1);


            pCell.AddElement(paragraph);
            //  pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("While your loan is fixed rate, a maximum of $20,000 in your offset account will reduce the interest payable by you.  There will be no interest saving for the balance in your offset account. ", font10));
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 20;
            otherInformationTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("You must commence construction with 6 months of the settlement date and construction must be completed with 18 months of the settlement date.  ", font10));
            pCell.AddElement(paragraph);
            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.PaddingBottom = 20;
            otherInformationTable.AddCell(pCell);

            pCell = getDisableBorderCell(0, 0, 1, 1);

            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            otherInformationTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("You acknowledge that the security given by you in respect of this loan, also secures payment of loan number" + " XXX " + " provided to you.", font10));

            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 20;
            otherInformationTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            otherInformationTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("Obtain an acknowledgement from " + " [name of guarantor]" + " that the security given by " + " [name of guarantor]" + " in respect of this loan also secures loan number " + "XXX.  ", font10));

            pCell = getDisableBorderCell(0, 1, 1, 1);
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 20;
            otherInformationTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

            otherInformationTable.AddCell(pCell);

            paragraph = para();
            paragraph.Add(new Chunk("If you have not repaid all money owing by the date for final repayment, we may at our absolute discretion vary your loan agreement to convert your loan to a principal and interest term loan.", font10));

            pCell = getDisableBorderCell(0, 0, 1, 1);
            pCell.AddElement(paragraph);
            pCell.PaddingBottom = 10;
            otherInformationTable.AddCell(pCell);
            paragraph = para();
            paragraph.Add(new Chunk("Signed on behalf of us by:", font10));

            pCell = getDisableBorderCell(1, 0, 0, 0);
            pCell.Colspan = 2;
            pCell.AddElement(paragraph);
            pCell.PaddingTop = 10;
            otherInformationTable.AddCell(pCell);
        }


        #endregion

        #endregion



        #region ParagraphDocumentSectionCollection
        public Paragraph CreateDocumentLoanAgreementTitleAndType(LoanAgreementType type)
        {
            string notes = @"        MORTGAGE HOUSE OF AUSTRALIA " + type.ToString() + " LOAN AGREEMENT";
            Paragraph p = new Paragraph(notes, font10Bold);
            p.SpacingAfter = 10;

            return p;

        }

        public Paragraph CreateDocumentLendAndManagerSection()
        {
            string notes = @"The Manager has arranged for the Lender to make the loan which will be managed by the Manager. Normally, you deal with the Manager.  Lender and the Manager are together and separately referred  to as ‘we/us/our’";
            Paragraph p = new Paragraph(notes, font10);
            return p;
        }
        public Paragraph AcceptOffer()
        {
            string notes = @"How do I accept this offer? ";
            Paragraph p = new Paragraph(notes, font10Bold);
            p.SpacingAfter = 20;
            return p;
        }
        public Paragraph WarningParagraph()
        {
            string notes = @"If the borrower is a company or if this loan is predominantly used for business purposes or investment purposes (except for investment in residential property) it will not be regulated by the National Credit Code despite any statement that the National Credit Code applies to this loan.  The information statement below only applies to you if your loan is regulated by the National Credit Code. ";
            Paragraph p = new Paragraph(notes, font10Bold);
            p.SpacingAfter = 20;
            return p;
        }
        public Paragraph MustSignParagraph()
        {

            Chunk c1 = new Chunk("To accept this offer you must sign and date this document below and return it to our lawyers.  This contract comes into force on the", font10);
            Chunk c2 = new Chunk("or such earlier date as we decide");

            paragraph = para();
            paragraph.Add(c1);
            paragraph.Add(settlementDate);
            paragraph.Add(c2);
            paragraph.SpacingAfter = 20;

            //  Paragraph p = new Paragraph(notes, font10Bold);

            return paragraph;
        }
        public Paragraph CreateDocumentSpecialCharacterMeanings()
        {
            Chunk c1 = new Chunk("Words in", font10);
            Chunk c2 = new Chunk("italics", font10Italic);
            Chunk c3 = new Chunk(" have special meanings.  The ", font10);
            Chunk c4 = new Chunk(" is the date we first advance money to you. ", font10);
            paragraph = para();
            paragraph.Add(c1);
            paragraph.Add(c2);
            paragraph.Add(c3);
            paragraph.Add(settlementDate);
            paragraph.Add(c4);

            paragraph.SpacingAfter = 10f;
            return paragraph;
        }

        public Paragraph CreateDocumentFinancialTableHeadingsPartA(DateTime date)
        {

            Chunk c1 = new Chunk("Financial Table ", font10Bold);
            Chunk c2 = new Chunk(@"(This is a table setting out information prescribed by the National Credit Code (the", font10);
            Chunk c3 = new Chunk(" Code ", font10Bold);
            Chunk c4 = new Chunk(@") – a law designed to ensure you have all the information you need to know about your loan.  If this loan is not primarily for consumer purposes, your loan may not be regulated by the Code.)", font10);
            Phrase ph = new Phrase();


            paragraph = para();
            paragraph.Add(c1);
            paragraph.Add(c2);
            paragraph.Add(c3);
            paragraph.Add(c4);

            paragraph.SpacingAfter = 10f;
            return paragraph;


        }
        public Paragraph CreateDocumentFinancialTableHeadingsPartB(DateTime date)
        {

            Chunk c1 = new Chunk("The following information is prepared as at ", font10);
            Chunk c3 = new Chunk(@" the Code calls this the disclosure date).  This information may change before or after the ", font10);
            Chunk c2 = new Chunk(date.ToShortDateString(), font10Bold);




            paragraph = para();
            paragraph.Add(c1);
            paragraph.Add(c2);
            paragraph.Add(c3);
            paragraph.Add(settlementDate);


            paragraph.SpacingAfter = 10f;
            return paragraph;


        }




        public void SignatureTable()
        {

            signatureTable = new PdfPTable(3);



            signatureTable.TotalWidth = intTableWidth;
            signatureTable.LockedWidth = true;
            float[] widths = new float[] { 47.5f, 5f, 47.5f };
            //  pCell.DisableBorderSide
            signatureTable.SetWidths(widths);




            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            //paragraph.Add(new Chunk("Borrower 1 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);


            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);

            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            //paragraph.Add(new Chunk("Borrower 2 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);

            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);









            pCell = getDisableBorderCell(1, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 1 signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(1, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 2 signature", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(1, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 1 Name", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(1, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 2 Name", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);






            pCell = getDisableBorderCell(1, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 1 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);

            pCell.PaddingTop = -5;
            pCell.FixedHeight = 15;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 15;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(1, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 2 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 15;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            //paragraph.Add(new Chunk("Borrower 1 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);


            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            //paragraph.Add(new Chunk("Borrower 2 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            //   pCell.PaddingTop = -5;
            pCell.FixedHeight = 25;
            signatureTable.AddCell(pCell);



            pCell = getDisableBorderCell(1, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 3 signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);

            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 4 signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);





            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 3 name\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);

            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 1, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 4 name\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);



            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 3 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);

            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);


            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("", font10));

            pCell.AddElement(paragraph);
            // pCell.PaddingTop = 3;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);




            pCell = getDisableBorderCell(0, 0, 0, 0);
            paragraph = para();
            paragraph.Add(new Chunk("Borrower 4 date of signature\n", font8));
            paragraph.Add(new Chunk(""));
            pCell.AddElement(paragraph);
            pCell.PaddingTop = -5;
            pCell.FixedHeight = 30;
            signatureTable.AddCell(pCell);

        }



        #endregion


    }


    #region  Header And Footer Helper Section

    public class PageEventHelper : PdfPageEventHelper
    {
        BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/ARIAL.TTF", BaseFont.CP1252, BaseFont.EMBEDDED);



        Font ffont = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }
        //
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            int pageN = writer.PageNumber;
            PdfContentByte cb = writer.DirectContent;
            Font font8Bold = new Font(bf, 8, Font.BOLD);
            Font font8 = new Font(bf, 8);
            Phrase header = new Phrase("Money Home Loan Agreement-" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year, font8Bold);
            Phrase footer = new Phrase("13859586.14  AOC AOC                                                                                                                                                               " + pageN, font8);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                    header,
                    document.Right - 100,
                    document.Top + 25, 0);
            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
                    footer,
                    220 + document.LeftMargin,
                    document.Bottom - 10, 0);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            // important to avoid closing the stream, YES!
            writer.CloseStream = true;
        }
    }
    #endregion


}