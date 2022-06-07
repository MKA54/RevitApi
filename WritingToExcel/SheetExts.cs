using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace WritingToExcel
{
    public static class SheetExts
    {
        public static void SetCellValue<T>(this ISheet sheet, int rowIndex, int columnIndex, T value)
        {
            var cellReference = new CellReference(rowIndex, columnIndex);

            var row = sheet.GetRow(cellReference.Row);
            row = row ?? sheet.CreateRow(cellReference.Row);

            var cell = row.GetCell(cellReference.Col);
            cell = cell ?? row.CreateCell(cellReference.Col);

            if (value is string)
            {
                cell.SetCellValue((string)(object)value);
            }
            else if (value is double)
            {
                cell.SetCellValue((double)(object)value);
            }
            else if (value is int)
            {
                cell.SetCellValue((int)(object)value);
            }
        }
    }
}