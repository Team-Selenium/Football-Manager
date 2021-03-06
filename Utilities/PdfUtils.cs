﻿namespace Utilities
{
    using System.Diagnostics;
    using System.IO;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using MsSql.Data;

    /// <summary>
    /// Generator for PDF reports and saving them
    /// </summary>
    public static class PdfUtils
    {
        private const int RowCountInPdfExport = 5;
        private const string PdfPath = @"..\..\..\Data Sources\PDF\";
        private const string PdfFileName = "CitiesReport.pdf";

        /// <summary>
        /// Creates PDF reports and saves them as a PDF file
        /// </summary>
        public static void GeneratePdfReport()
        {
            File.Delete(PdfPath + PdfFileName);
            var repo = new MSSqlRepository();
            var pdfData = repo.GetDataForPdfExport();
            var doc = new Document();

            PdfWriter.GetInstance(doc, new FileStream(PdfPath + PdfFileName, FileMode.Create));

            doc.Open();

            PdfPTable table = new PdfPTable(RowCountInPdfExport);
            table.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            table.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            var headerBackground = new BaseColor(204, 204, 204);
            PdfPCell headerCell = new PdfPCell(new Phrase("Sample PDF Export From the Football Manager"));
            headerCell.Colspan = RowCountInPdfExport;
            headerCell.Padding = 10f;
            headerCell.HorizontalAlignment = 1;
            headerCell.BackgroundColor = headerBackground;
            table.AddCell(headerCell);

            string[] col = { "Date", "Staduim", "Home Team", "Away Team", "Result" };

            for (int i = 0; i < col.Length; ++i)
            {
                PdfPCell columnCell = new PdfPCell(new Phrase(col[i]));
                columnCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                columnCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                columnCell.BackgroundColor = headerBackground;
                table.AddCell(columnCell);
            }

            foreach (var item in pdfData)
            {
                PdfPCell townCell = new PdfPCell(new Phrase(item.Key));
                townCell.Colspan = RowCountInPdfExport;
                townCell.Padding = 10f;
                table.AddCell(townCell);

                foreach (var match in item.Value)
                {
                    table.AddCell(match.Date.Day + "." + match.Date.Month + "." + match.Date.Year);
                    table.AddCell(match.Stadium);
                    table.AddCell(match.HomeTeam);
                    table.AddCell(match.AwayTeam);
                    table.AddCell(match.Result);
                }
            }

            doc.Add(table);
            doc.Close();
            Process.Start(PdfPath);
        }
    }
}
