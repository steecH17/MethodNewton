using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;

namespace MethodNewton
{
    public class ExcelReader(string path)
    {
        private readonly string _path = path;

        public (List<List<double>>, List<double>) ExcelFileRead()
        {
            List<List<double>> xList = new List<List<double>>();
            List<double> yList = new List<double>();

            // Загрузить файл Excel
            Workbook wb = new Workbook(_path);

            // Получить все рабочие листы
            WorksheetCollection collection = wb.Worksheets;

            // Получить рабочий лист, используя его индекс
            Worksheet worksheet = collection[0];

            int rows = worksheet.Cells.MaxDataRow;
            int columns = worksheet.Cells.MaxDataColumn;

            int xCount = 0;
            for(int row = 2; row <= rows; row++)
            {
                xList.Add(new List<double>());

                for(int col = 1; col <= columns; col++)
                {
                    if(col < columns)
                        xList[xCount].Add(worksheet.Cells[row,col].DoubleValue);
                    else
                        yList.Add(worksheet.Cells[row,col].DoubleValue);
                }
                    xCount++;
            }

            return (xList, yList);
        }
        
    }
}
