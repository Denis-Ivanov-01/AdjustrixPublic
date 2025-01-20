using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adjustment.Project;
using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace Adjustment.Reports.Leveling
{
    public class LevelingReportPdf : IReportGenerator
    {
        //document metadata
        private const string name = "AdjustmentReport";
        private const string extension = ".pdf";

        //styling and layout
        private const string fontStyle = "Times New Roman";

        //data
        private AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;
        private LevelingProject Project;

        public LevelingReportPdf(AdjustmentResult<HeightDelta, AdjustedBenchmark> adjustmentResult, 
            LevelingProject project)
        {
            AdjustmentResult = adjustmentResult;
            Project = project;
        }
        public void Generate(string dir)
        {
            Document document = new();
            document.Info.Title = "Leveling Adjustment Report";
            PrepareStyles(document);

            Section section = document.AddSection();
            AddReportHeader(section);

            section.AddParagraph();
            section.AddParagraph($"Number of measurements: {AdjustmentResult.AdjustedMeasurements.Count}");
            section.AddParagraph($"Number of unknown points: {AdjustmentResult.AdjustedPoints.Count}");
            section.AddParagraph($"Network Redundancy: {AdjustmentResult.DistinctTraverses.Count}");
            section.AddParagraph($"Per unit variance: {Math.Round(AdjustmentResult.PerUnitVariance, 2)} [mm]");

            GenerateResidualsTable(section);
            GeneratePointsTable(section);
            // Render the document to a PDF
            string reportPath = GetReportPath(dir);
            PdfDocumentRenderer renderer = new PdfDocumentRenderer
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.Save(reportPath);
        }

        private void AddReportHeader(Section section)
        {
            section.AddParagraph("Adjustment Result Report", "CustomHeading");
            section.AddParagraph();
            section.AddParagraph($"Project: {Project.Name}", "SecondaryHeading");
            section.AddParagraph($"Client: {Project.Client}", "SecondaryHeading");
            section.AddParagraph($"Contractor: {Project.Contractor}", "SecondaryHeading");
            section.AddParagraph($"Site: {Project.SiteName}", "SecondaryHeading");
        }

        private static void PrepareStyles(Document document)
        {
            Style heading1 = document.Styles["Heading1"];
            heading1.Font.Name = fontStyle;
            heading1.Font.Bold = true;
            heading1.Font.Size = 18;
            heading1.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            Style normal = document.Styles["Normal"];
            normal.Font.Name = fontStyle;
            normal.Font.Size = 12;
            normal.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            Style secondary = document.AddStyle("SecondaryHeading", "Heading1");
            secondary.Font.Size = 14;
            secondary.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            Style custom = document.AddStyle("CustomHeading", "Heading1");
            custom.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            Style tableHeading = document.AddStyle("TableHeading", "Normal");
            tableHeading.Font.Bold = true;
            tableHeading.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        }

        private void GeneratePointsTable(Section section)
        {
            section.AddParagraph();
            section.AddParagraph("Adjusted points", "TableHeading");

            // Create the table
            Table table = section.AddTable();
            table.Style = "Normal";
            table.Borders.Width = 0.5; // Set border width for the table

            // Define columns
            Column col1 = table.AddColumn(Unit.FromMillimeter(40)); // Column 1: Number of Point
            col1.Format.Alignment = ParagraphAlignment.Center;

            Column col2 = table.AddColumn(Unit.FromMillimeter(40)); // Column 2: Elevation
            col2.Format.Alignment = ParagraphAlignment.Center;

            Column col3 = table.AddColumn(Unit.FromMillimeter(40)); // Column 3: Variance (mm)
            col3.Format.Alignment = ParagraphAlignment.Center;

            // Add the header row
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray; // Add background color for header
            headerRow.Format.Font.Bold = true;

            headerRow.Cells[0].AddParagraph("Number of Point");
            headerRow.Cells[1].AddParagraph("Elevation [m]");
            headerRow.Cells[2].AddParagraph("Variance [mm]");

            // Add data rows
            foreach (var point in AdjustmentResult.AdjustedPoints)
            {
                Row row = table.AddRow();
                row.Cells[0].AddParagraph(point.Number);
                row.Cells[1].AddParagraph(point.Value.ToString("F3"));
                row.Cells[2].AddParagraph(point.Variance.ToString("F2"));
            }

            // Adjust table alignment (center it on the page)
            table.Rows.Alignment = RowAlignment.Center;
        }

        private void GenerateResidualsTable(Section section)
        {
            section.AddParagraph();
            section.AddParagraph("Residuals", "TableHeading");

            Table table = section.AddTable();
            table.Borders.Width = 0.5;

            Column col1 = table.AddColumn(Unit.FromCentimeter(10));
            col1.Format.Alignment = ParagraphAlignment.Center;

            Column col2 = table.AddColumn(Unit.FromCentimeter(4));
            col2.Format.Alignment = ParagraphAlignment.Center;

            Row header = table.AddRow();
            header.Shading.Color = Colors.LightGray; // Add background color for header
            header.Format.Font.Bold = true;

            header.Cells[0].AddParagraph("Residual");
            header.Cells[1].AddParagraph("Value [mm]");

            for (int i=0; i < AdjustmentResult.DistinctTraverses.Count; i++)
            {
                Row row = table.AddRow();
                row[0].AddParagraph(JoinTraverseNames(AdjustmentResult.DistinctTraverses[i]));
                row[1].AddParagraph((AdjustmentResult.Residuals[i] * 1000).ToString("F2"));
            }

            table.Rows.Alignment = RowAlignment.Center;
        }

        private string JoinTraverseNames(List<PointBase> traverse)
        {
            return string.Join(" - ", traverse.Select(p => p.Number));
        }

        private string GetReportPath(string dir)
        {
            string name = GetName(dir);
            string fullName = Path.ChangeExtension(name, extension);
            return Path.Combine(dir, fullName);
        }
        private string GetName(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            string[] names = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
            }

            int counter = 1;
            string finalName = name;
            while (names.Contains(finalName))
            {
                finalName = $"{name}_{counter}";
                counter++;
            }
            return finalName;
        }
    }
}
