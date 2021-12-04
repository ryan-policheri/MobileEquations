using System;
using System.IO;
using DotNetCommon.SystemFunctions;
using OfficeOpenXml;

namespace MobileEquations.Benchmarker
{
    public class ExcelService : ExcelBuilderBase
    {
        private readonly BenchmarkerConfig _config;

        public ExcelService(BenchmarkerConfig config)
        {
            _config = config;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void ExportTests(TrialReporter reporter)
        {
            string outputFile = SystemFunctions.CombineDirectoryComponents(_config.BenchmarkDatasetDirectory, $"BenchmarkResults_{SystemFunctions.GetDateTimeAsFileNameSafeString()}");
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet allData = package.Workbook.Worksheets.Add("All Data");
                TableWithMeta allDataTableWithMeta = reporter.GetAllData();
                this.BuildWorksheetHeader(allData, allDataTableWithMeta.Header, 1, allDataTableWithMeta.ColumnCount);
                BuildDataSection(allData, allDataTableWithMeta.Table, 2);
                AutoFitColumns(allData);

                ExcelWorksheet averagedByFile = package.Workbook.Worksheets.Add("Averaged By File");
                TableWithMeta averagedByFileTableWithMeta = reporter.GetAveragedByFileData();
                this.BuildWorksheetHeader(averagedByFile, averagedByFileTableWithMeta.Header, 1, averagedByFileTableWithMeta.ColumnCount);
                BuildDataSection(averagedByFile, averagedByFileTableWithMeta.Table, 2);
                AutoFitColumns(averagedByFile);

                ExcelWorksheet averagedAcrossFiles = package.Workbook.Worksheets.Add("Averaged Across Files");
                TableWithMeta averagedAcrossFilesTableWithMeta = reporter.GetAveragedByFileData();
                this.BuildWorksheetHeader(averagedAcrossFiles, averagedAcrossFilesTableWithMeta.Header, 1, averagedAcrossFilesTableWithMeta.ColumnCount);
                BuildDataSection(averagedAcrossFiles, averagedAcrossFilesTableWithMeta.Table, 2);
                AutoFitColumns(averagedAcrossFiles);

                FileInfo file = new FileInfo(outputFile);
                package.SaveAs(file);
            }
        }
    }
}
