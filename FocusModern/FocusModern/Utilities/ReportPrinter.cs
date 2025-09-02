using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace FocusModern.Utilities
{
    /// <summary>
    /// Simple printer for tabular DataGridView reports with a title and subtitle.
    /// Supports Print Preview and printing to the "Microsoft Print to PDF" device
    /// when available (to export a PDF without extra dependencies).
    /// </summary>
    public class ReportPrinter
    {
        private readonly DataGridView grid;
        private readonly string title;
        private readonly string subtitle;

        private PrintDocument doc;
        private int currentRow;
        private List<float> columnWidths;
        private float tableWidth;

        public ReportPrinter(DataGridView grid, string title, string subtitle)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.title = title ?? string.Empty;
            this.subtitle = subtitle ?? string.Empty;
        }

        public void ShowPreview(IWin32Window owner)
        {
            PrepareDocument();
            using (var preview = new PrintPreviewDialog { Document = doc, Width = 1200, Height = 800 })
            {
                preview.ShowDialog(owner);
            }
        }

        public void Print()
        {
            PrepareDocument();
            using (var dlg = new PrintDialog { Document = doc, UseEXDialog = true })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    doc.Print();
                }
            }
        }

        public void ExportPdf(IWin32Window owner)
        {
            var pdfPrinter = FindPdfPrinter();
            if (pdfPrinter == null)
            {
                MessageBox.Show(owner, "'Microsoft Print to PDF' printer not found. Use Print Preview and choose a PDF printer, or install the PDF printer.", "PDF Printer Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", FileName = MakeSafeFileName(title) + ".pdf" })
            {
                if (sfd.ShowDialog(owner) != DialogResult.OK) return;

                PrepareDocument();
                doc.PrinterSettings.PrinterName = pdfPrinter;
                doc.PrinterSettings.PrintToFile = true;
                doc.PrinterSettings.PrintFileName = sfd.FileName;
                doc.Print();
            }
        }

        private string FindPdfPrinter()
        {
            foreach (string p in PrinterSettings.InstalledPrinters)
            {
                if (p.IndexOf("Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase) >= 0)
                    return p;
            }
            return null;
        }

        private void PrepareDocument()
        {
            currentRow = 0;
            doc = new PrintDocument();
            doc.DocumentName = title;
            doc.DefaultPageSettings.Margins = new Margins(50, 50, 60, 60); // 5mm margins approx
            doc.PrintPage += OnPrintPage;

            // Compute proportional widths based on displayed columns
            var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            float totalDisplayed = cols.Sum(c => (float)(c.Width <= 0 ? 100 : c.Width));
            columnWidths = cols.Select(c => (float)(c.Width <= 0 ? 100 : c.Width) / totalDisplayed).ToList();
        }

        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var bounds = e.MarginBounds;
            float y = bounds.Top;

            // Header
            using (var titleFont = new Font("Segoe UI Semibold", 14))
            using (var subFont = new Font("Segoe UI", 9))
            using (var headerPen = new Pen(Color.Gray))
            {
                g.DrawString(title, titleFont, Brushes.Black, bounds.Left, y);
                y += titleFont.GetHeight(g) + 2;
                if (!string.IsNullOrWhiteSpace(subtitle))
                {
                    g.DrawString(subtitle, subFont, Brushes.DimGray, bounds.Left, y);
                    y += subFont.GetHeight(g) + 8;
                }
                g.DrawLine(headerPen, bounds.Left, y, bounds.Right, y);
                y += 6;
            }

            // Table header
            var cols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            float x = bounds.Left;
            tableWidth = bounds.Width;

            using (var headerFont = new Font("Segoe UI Semibold", 9))
            using (var headerBack = new SolidBrush(Color.FromArgb(245, 247, 250)))
            using (var borderPen = new Pen(Color.Silver))
            {
                for (int i = 0; i < cols.Count; i++)
                {
                    float colWidth = columnWidths[i] * tableWidth;
                    var rect = new RectangleF(x, y, colWidth, 22);
                    g.FillRectangle(headerBack, rect);
                    g.DrawRectangle(Pens.Silver, rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(cols[i].HeaderText, headerFont, Brushes.Black, rect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });
                    x += colWidth;
                }
                y += 24;

                // Rows
                using (var rowFont = new Font("Segoe UI", 9))
                {
                    while (currentRow < grid.Rows.Count)
                    {
                        var row = grid.Rows[currentRow];
                        if (row.IsNewRow) { currentRow++; continue; }

                        float rowHeight = 20f;
                        if (y + rowHeight > bounds.Bottom)
                        {
                            e.HasMorePages = true;
                            return;
                        }

                        x = bounds.Left;
                        for (int i = 0; i < cols.Count; i++)
                        {
                            float colWidth = columnWidths[i] * tableWidth;
                            var rect = new RectangleF(x, y, colWidth, rowHeight);
                            g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width, rect.Height);
                            var cell = row.Cells[cols[i].Name];
                            string text = cell?.FormattedValue?.ToString() ?? string.Empty;
                            g.DrawString(text, rowFont, Brushes.Black, rect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });
                            x += colWidth;
                        }
                        y += rowHeight;
                        currentRow++;
                    }
                }
            }

            e.HasMorePages = false;
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return string.IsNullOrWhiteSpace(name) ? "report" : name;
        }
    }
}

