using System;
using System.IO;
using Common.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Common.Editor.Excel
{
    /// <summary>
    /// excel manipulation api
    /// </summary>
    public class ExcelApi
    {
        public const string ExtXls = ".xls";
        public const string ExtXlsx = ".xlsx";
        
        /// <summary>
        /// prefix used to denote ignorable content in excel
        /// (generally in sheet/column names) 
        /// </summary>
        public const string IgnorablePrefix = "_";
        
        /// <summary>
        /// prefix for file created by excel when opening file in locked mode
        /// </summary>
        public const string PrefixLockedFile = "~$";
            
        /// <summary>
        /// load workbook from file denoted by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IWorkbook LoadWorkbook(string path)
        {
            IWorkbook book = null;
            try
            {
                using (FileStream stream = IoHelper.ReadFile(path))
                {
                    string ext = Path.GetExtension(path);
                    bool xls = StringHelper.EqualsIgnoreCase(ext, ExtXls);
                    book = xls ? (IWorkbook) new HSSFWorkbook(stream) : new XSSFWorkbook(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load workbook from '{path}'", ex);
            }
            return book;
        }
        
        public void SaveWorkbook(IWorkbook book, string path)
        {
            using (FileStream stream = IoHelper.WriteFile(path))
            {
                book.Write(stream);
            }
        }

        /// <summary>
        /// retrieve cell value as string
        /// </summary>
        public string GetStringValue(ICell cell)
        {
            LangHelper.Validate(cell != null);
            CellType type = cell.CellType;
            switch (type)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    return StringHelper.ToString(cell.NumericCellValue);
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
            }
            return null;
        }
        
        public bool IsIgnorableName(string name)
        {
            return name != null && name.StartsWith(IgnorablePrefix);
        }

        /// <summary>
        /// check if given file is excel file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsExcelFile(string path)
        {
            var name = Path.GetFileName(path);
            if (name.StartsWith(PrefixLockedFile))
            {
                return false;
            }

            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }

            return StringHelper.EqualsIgnoreCase(ext, ExtXls) ||
                   StringHelper.EqualsIgnoreCase(ext, ExtXlsx);
        }

        /// <summary>
        /// find first cell with given content
        /// </summary>
        public ICell FindCell(ISheet sheet, string find)
        {
            for (int rn = sheet.FirstRowNum; rn <= sheet.LastRowNum; rn++)
            {
                IRow row = sheet.GetRow(rn);
                if(row == null) continue;
                for (int cn = row.FirstCellNum; cn <= row.LastCellNum; cn++)
                {
                    if (cn == -1) continue;
                    ICell cell = row.GetCell(cn);
                    if(cell == null) continue;
                    string value = GetStringValue(cell);
                    if (StringHelper.Equals(value, find)) return cell;
                }
            }
            return null;
        }

        public int RemoveEmptyRows(ISheet sheet)
        {
            int ret = 0;
            for (int rn = sheet.FirstRowNum; rn <= sheet.LastRowNum; rn++)
            {
                IRow row = sheet.GetRow(rn);
                if(row == null) continue;
                if (row.FirstCellNum == -1)
                {
                    sheet.RemoveRow(row);
                    ret++;
                }
            }
            return ret;
        }
    }
}