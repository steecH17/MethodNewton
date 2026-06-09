using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;

namespace MethodNewton
{
    public class ExcelPrinter(string path)
    {
        private readonly string _path = path;
        //разделить все на отдельные этапы
        //отдельно печать заголовка
        //отдельно каждой строчки в цикле

        public void SaveExcelFile(List<List<double>> xData, List<double> yData)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            int startRow = 2;
            int startCol = 1;
            int countArgs = xData[0].Count;

            for (int k = startCol; k < countArgs + startCol; k++)
            {
                string argIndex = (k - startCol + 1).ToString();
                worksheet.Cells[startRow - 1, k].PutValue("X" + argIndex);
                SetHeaderStyle(worksheet.Cells[startRow - 1, k]);
            }
            worksheet.Cells[startRow - 1, countArgs + startCol].PutValue("Y");
            SetHeaderStyle(worksheet.Cells[startRow - 1, countArgs + startCol]);


            for (int i = 0; i < xData.Count; i++)
            {
                for (int j = 0; j < countArgs; j++)
                {
                    worksheet.Cells[i + startRow, j + startCol].PutValue(xData[i][j]);
                }

                worksheet.Cells[i + startRow, countArgs + startCol].PutValue(yData[i]);
            }

            worksheet.AutoFitColumns();

            workbook.Save(_path);
        }

        public void SaveResultsExcel(List<List<double>> xData, List<double> yData, List<double> yCalc, string path)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            int countArgs = xData[0].Count;
            int currentRow = 0;

            // --- 1. ЗАГОЛОВКИ ---
            // Колонки X
            for (int i = 0; i < countArgs; i++)
            {
                Cell cell = worksheet.Cells[currentRow, i];
                cell.PutValue($"X{i + 1}");
                SetHeaderStyle(cell);
            }
            // Колонка Y исходное
            Cell yCell = worksheet.Cells[currentRow, countArgs];
            yCell.PutValue("Y (Исходное)");
            SetHeaderStyle(yCell);

            // Колонка f(x) расчетное
            Cell fCell = worksheet.Cells[currentRow, countArgs + 1];
            fCell.PutValue("f(x) (Расчетное)");
            SetHeaderStyle(fCell);

            // Колонка Невязка
            Cell errCell = worksheet.Cells[currentRow, countArgs + 2];
            errCell.PutValue("Невязка");
            SetHeaderStyle(errCell);

            currentRow++;

            // --- 2. ДАННЫЕ ---
            for (int i = 0; i < xData.Count; i++)
            {
                // Пишем X-сы
                for (int j = 0; j < countArgs; j++)
                {
                    worksheet.Cells[currentRow, j].PutValue(xData[i][j]);
                }

                // Пишем Y исходный
                worksheet.Cells[currentRow, countArgs].PutValue(yData[i]);

                // Пишем f(x) расчетный
                double calcValue = yCalc[i];
                worksheet.Cells[currentRow, countArgs + 1].PutValue(calcValue);

                // Пишем абсолютную невязку
                worksheet.Cells[currentRow, countArgs + 2].PutValue(Math.Abs(yData[i] - calcValue));

                currentRow++;
            }

            worksheet.AutoFitColumns();
            workbook.Save(path);
        }

        private void SetHeaderStyle(Cell cell)
        {
            var headerStyle = cell.GetStyle();
            headerStyle.Font.IsBold = true;
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;
            headerStyle.ForegroundColor = Color.LightGreen;
            headerStyle.Pattern = BackgroundType.Solid;
            cell.SetStyle(headerStyle);
        }

    }
}