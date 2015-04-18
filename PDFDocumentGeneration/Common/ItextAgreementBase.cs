using PDFDocumentGeneration.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFDocumentGeneration.Common
{
    public class ItextAgreementBase : PdfPageEventHelper
    {
        protected Chunk chunk;
        protected Chunk thirdParty;
        protected Chunk disclosureDate;
        protected Chunk unascertainable;
        protected Chunk settlementDate;
        protected PdfPCell pCell;
        protected Paragraph paragraph;
        protected int intTableWidth = 500;
        protected Document document;
        protected Document debitDocument;
        protected BaseFont bf;
        protected Font font10;
        protected Font font8;
        protected Font font10Bold;
        protected Font font10Italic;
        protected Font font10UnderLineBold;
        protected PdfTemplate footerTemplate;
        protected PdfContentByte cb;
        protected iTextSharp.text.Font fontFooter;
        protected iTextSharp.text.Font fontGeneralText;
        protected iTextSharp.text.Font fontBoldText;
        protected iTextSharp.text.Font fontCellHeader;
        protected iTextSharp.text.Font fontLargeBoldText;
        protected PdfWriter wr;
        public ItextAgreementBase()
        {

            Rectangle pageSize = new Rectangle(PageSize.A4);
            document = new Document(pageSize, 48, 48, 95, 25);
            debitDocument = new Document(pageSize, 35, 35, 92, 25);
            buildFonts();
            thirdParty = new Chunk("Third part fees", font10Italic);
            disclosureDate = new Chunk("disclosureDate", font10Italic);
            unascertainable = new Chunk("Unascertainable", font10);
            settlementDate = new Chunk("settlement date", font10Italic);

        }
        protected Paragraph para()
        {
            // wr.InitialLeading = 30;
            Paragraph temp = new Paragraph();
            temp.Alignment = Element.ALIGN_LEFT;
            temp.Leading = 12;
            return temp;

        }
        
        protected PdfPTable getBaseTable()
        {
            PdfPTable table = new PdfPTable(2);
            // table.KeepTogether = true;
            table.TotalWidth = intTableWidth;
            table.LockedWidth = true;
            float[] widths = new float[] { 75f, 25f };
            table.SetWidths(widths);
            return table;


        }
        protected Chunk getChunk(string s,Font font )
        {


            chunk = new Chunk(s,font);

            return chunk;
        }
        protected PdfPCell RatePerAnnumCell(AccountOptionsModelcs model)
        {
            pCell = getCell();

            paragraph = para();
            Chunk c1 = new Chunk(model.money + " per annum", font10);
            paragraph.Add(c1);
            pCell.AddElement(paragraph);
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            return pCell;

        }
        protected PdfPCell DefaultEmptyCell(int top, int bottom, int left, int right)
        {
            pCell = new PdfPCell();

            if (top != 1)
            {
                pCell.DisableBorderSide(1);
            }

            if (bottom != 1)
            {
                pCell.DisableBorderSide(2);
            }
            if (left != 1)
            {
                pCell.DisableBorderSide(4);
            }
            if (left != 1)
            {
                pCell.DisableBorderSide(8);
            }

            pCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph = para();
            Chunk c1 = new Chunk("", font10);
            paragraph.Add(c1);
            pCell.AddElement(paragraph);
            return pCell;

        }
        protected PdfPCell DefaultEmptyCellBold()
        {
            pCell = getCell();
            pCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            paragraph = para();
            Chunk c1 = new Chunk("", font10Bold);
            paragraph.Add(c1);
            pCell.AddElement(paragraph);
            return pCell;

        }
        protected PdfPCell getCell()
        {
            var pCell = new PdfPCell();
            return pCell;

        }
        protected PdfPCell getBoldCell()
        {
            var pCell = new PdfPCell();
            
            return pCell;
        }
        protected PdfPCell getDisableBorderCell(int top, int bottom, int left, int right) // 0 disable ,1 visable
        {
            pCell = new PdfPCell();

            if (top != 1)
            {
                pCell.DisableBorderSide(1);
            }

            if (bottom != 1)
            {
                pCell.DisableBorderSide(2);
            }
            if (left != 1)
            {
                pCell.DisableBorderSide(4);
            }
            if (right != 1)
            {
                pCell.DisableBorderSide(8);
            }

            return pCell;

        }



        private void buildFonts()
        {
            // add more font types. These may be reused through the document creation
            bf = BaseFont.CreateFont("c:/windows/fonts/ARIAL.TTF", BaseFont.CP1252, BaseFont.EMBEDDED);

            font10Bold = new Font(bf, 10, Font.BOLD);
            font10 = new Font(bf, 10);
            font10Italic = new Font(bf, 10, Font.ITALIC);
            font8 = new Font(bf, 8);
            fontFooter = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.ITALIC, BaseColor.DARK_GRAY);
            fontGeneralText = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            fontBoldText = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            fontCellHeader = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            fontLargeBoldText = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 17, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            font10UnderLineBold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.UNDERLINE | iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        }
    }
}