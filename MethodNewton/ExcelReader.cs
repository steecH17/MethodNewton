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

        public (List<double>, List<double>) ExcelFileRead()
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();

            // Загрузить файл Excel
            Workbook wb = new Workbook(_path);

            // Получить все рабочие листы
            WorksheetCollection collection = wb.Worksheets;

            // Получить рабочий лист, используя его индекс
            Worksheet worksheet = collection[0];

            int rows = worksheet.Cells.MaxDataRow;

            for (int i = 2; i <= rows; i++)
            {
                for(int j = 1; j < 3; j++)
                {
                    if(j == 1)
                    {
                        xList.Add(worksheet.Cells[i,j].DoubleValue);
                    }
                    if(j == 2)
                    {
                        yList.Add(worksheet.Cells[i, j].DoubleValue);
                    }
                }
            }

            return (xList, yList);
        }
        
    }
}
