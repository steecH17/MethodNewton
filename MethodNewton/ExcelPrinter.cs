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