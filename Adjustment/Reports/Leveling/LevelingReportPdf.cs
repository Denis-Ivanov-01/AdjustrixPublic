using System.Collections.Specialized;
using Adjustment.Project;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace Adjustment.Reports.Leveling
{

    public class LevelingReportPdf : IReportGenerator
    {
        private const double defaultTableRowHeight = 15;

        private const string extension = ".pdf";
        private const string fontStyle = "Times New Roman";

        private readonly AdjustmentResult<HeightDelta, AdjustedBenchmark> AdjustmentResult;
        private readonly LevelingProject Project;
        private readonly StringsDictionary strings;

        public LevelingReportPdf(AdjustmentResult<HeightDelta, AdjustedBenchmark> adjustmentResult,
        LevelingProject project,
                                 StringsDictionary stringsDictionary)
        {
            AdjustmentResult = adjustmentResult;
            Project = project;
            strings = stringsDictionary;
        }

        public void Generate(string dir)
        {
            Document document = new();
            document.Info.Title = strings.Title;
            PrepareStyles(document);

            Section section = document.AddSection();
            AddReportHeader(section);

            section.AddParagraph();
            section.AddParagraph($"{strings.NumberOfMeasurements}: {AdjustmentResult.AdjustedMeasurements.Count}");
            section.AddParagraph($"{strings.NumberOfUnknownPoints}: {AdjustmentResult.AdjustedPoints.Count}");
            section.AddParagraph($"{strings.NetworkRedundancy}: {AdjustmentResult.DistinctTraverses.Count}");
            section.AddParagraph($"{strings.PerUnitVariance}: {Math.Round(AdjustmentResult.PerUnitVariance, 2)} [mm]");

            GenerateResidualsTable(section);
            GeneratePointsTable(section);
            GenerateMeasurementsTable(section);

            string reportPath = GetReportPath(dir);
            PdfDocumentRenderer renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();
            renderer.Save(reportPath);
        }

        private void AddReportHeader(Section section)
        {
            section.AddParagraph(strings.Title, "CustomHeading");
            section.AddParagraph();
            section.AddParagraph($"{strings.Project}: {Project.Name}", "SecondaryHeading");
            section.AddParagraph($"{strings.Client}: {Project.Client}", "SecondaryHeading");
            section.AddParagraph($"{strings.Contractor}: {Project.Contractor}", "SecondaryHeading");
            section.AddParagraph($"{strings.Site}: {Project.SiteName}", "SecondaryHeading");
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

        private void GenerateMeasurementsTable(Section section)
        {
            section.AddParagraph();
            section.AddParagraph(strings.Measurements, "TableHeading");

            Table table = GetTable(section);
            double availableWidth = GetAvailableWidth();

            Column col1 = table.AddColumn(availableWidth * 0.3);
            FormatTableColumn(col1);

            Column col2 = table.AddColumn(availableWidth * 0.3);
            FormatTableColumn(col2);

            Column col3 = table.AddColumn(availableWidth * 0.2);
            FormatTableColumn(col3);

            Column col4 = table.AddColumn(availableWidth * 0.2);
            FormatTableColumn(col4);

            Row headerRow = GetTableHeaderRow(table);
            headerRow.Cells[0].AddParagraph(strings.FromPoint);
            headerRow.Cells[1].AddParagraph(strings.ToPoint);
            headerRow.Cells[2].AddParagraph(strings.Value);
            headerRow.Cells[3].AddParagraph(strings.Variance);

            for (int i = 0; i < AdjustmentResult.AdjustedMeasurements.Count; i++)
            {
                HeightDelta meas = AdjustmentResult.AdjustedMeasurements[i];
                Row row = GetTableRow(table);
                row.Cells[0].AddParagraph(meas.FromPoint.Number);
                row.Cells[1].AddParagraph(meas.ToPoint.Number);
                row.Cells[2].AddParagraph(meas.Value.ToString("F3"));
                row.Cells[3].AddParagraph(AdjustmentResult.MeasurementVariances[i].ToString("F2"));
            }

            table.Rows.Alignment = RowAlignment.Center;
        }

        private void GeneratePointsTable(Section section)
        {
            section.AddParagraph();
            section.AddParagraph(strings.AdjustedPointsTitle, "TableHeading");
            Table table = GetTable(section);
            double availableWidth = GetAvailableWidth();

            // Define columns to span the available width
            Column col1 = table.AddColumn(availableWidth * 0.40);
            FormatTableColumn(col1);

            Column col2 = table.AddColumn(availableWidth * 0.30);
            FormatTableColumn(col2);

            Column col3 = table.AddColumn(availableWidth * 0.30);
            FormatTableColumn(col3);

            Row headerRow = GetTableHeaderRow(table);
            headerRow.Cells[0].AddParagraph(strings.NumberOfPoint);
            headerRow.Cells[1].AddParagraph(strings.Elevation);
            headerRow.Cells[2].AddParagraph(strings.Variance);

            foreach (var point in AdjustmentResult.AdjustedPoints)
            {
                Row row = GetTableRow(table);
                row.Cells[0].AddParagraph(point.Number);
                row.Cells[1].AddParagraph(point.Value.ToString("F3"));
                row.Cells[2].AddParagraph(point.Variance.ToString("F2"));
            }

            table.Rows.Alignment = RowAlignment.Center;
        }

        private static Table GetTable(Section section)
        {
            Table table = section.AddTable();
            table.Borders.Width = 0.5;
            return table;
        }

        private static double GetAvailableWidth()
        {
            double pageWidth = Unit.FromMillimeter(210);
            double leftMargin = Unit.FromCentimeter(2.5);
            double rightMargin = Unit.FromCentimeter(2);
            double availableWidth = pageWidth - leftMargin - rightMargin;
            return availableWidth;
        }

        private void GenerateResidualsTable(Section section)
        {
            section.AddParagraph();
            section.AddParagraph(strings.ResidualsTitle, "TableHeading");

            double availableWidth = GetAvailableWidth();

            Table table = GetTable(section);

            Column col1 = table.AddColumn(availableWidth * 0.65);
            FormatTableColumn(col1);

            Column col2 = table.AddColumn(availableWidth * 0.35);
            FormatTableColumn(col2);

            Row header = GetTableHeaderRow(table);
            header.Cells[0].AddParagraph(strings.Residual);
            header.Cells[1].AddParagraph(strings.Value);

            for (int i = 0; i < AdjustmentResult.DistinctTraverses.Count; i++)
            {
                Row row = GetTableRow(table);
                row[0].AddParagraph(JoinTraverseNames(AdjustmentResult.DistinctTraverses[i]));
                row[1].AddParagraph((AdjustmentResult.Residuals[i] * 1000).ToString("F2"));
            }

            table.Rows.Alignment = RowAlignment.Center;
        }

        private static void FormatTableColumn(Column col)
        {
            col.Format.Alignment = ParagraphAlignment.Center;
        }

        private static Row GetTableRow(Table table)
        {
            Row row = table.AddRow();
            row.Height = defaultTableRowHeight;
            return row;
        }

        private static Row GetTableHeaderRow(Table table)
        {
            Row header = table.AddRow();
            header.Height = defaultTableRowHeight;
            header.Format.Font.Bold = true;
            header.Shading.Color = Colors.LightGray;
            return header;
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
            string finalName = "AdjustmentReport";
            while (names.Contains(finalName))
            {
                finalName = $"AdjustmentReport_{counter}";
                counter++;
            }
            return finalName;
        }
    }

    /// <summary>
    /// Intended to be used to store the strings for the PDF Leveling report.
    /// In order to support multiple languages.
    /// </summary>
    public class StringsDictionary
    {
        public string Title { get; set; }
        public string Contractor { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Site { get; set; }
        public string NumberOfMeasurements { get; set; }
        public string NumberOfUnknownPoints { get; set; }
        public string NetworkRedundancy { get; set; }
        public string PerUnitVariance { get; set; }
        public string AdjustedPointsTitle { get; set; }
        public string ResidualsTitle { get; set; }
        public string NumberOfPoint { get; set; }
        public string Elevation { get; set; }
        public string Variance { get; set; }
        public string Residual { get; set; }
        public string Value { get; set; }
        public string Measurements { get; set; }
        public string FromPoint { get; set; }
        public string ToPoint { get; set; }
    }
}
