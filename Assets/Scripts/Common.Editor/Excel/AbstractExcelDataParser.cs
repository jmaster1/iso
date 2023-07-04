using System.IO;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using NPOI.SS.UserModel;

namespace Common.Editor.Excel
{
    /// <summary>
    /// base class for parsing data from excel file
    /// </summary>
    public abstract class AbstractExcelDataParser : GenericBean
    {
        public ExcelApi ExcelApi = new ExcelApi();
        
        public TextParser TextParser = TextParser.Instance;
        
        /// <summary>
        /// check if given sheet A1 cell  starts with specified prefix,
        /// initialize suffix if so
        /// </summary>
        public bool AcceptSheetHeader(ISheet sheet, string prefix, out string suffix)
        {
            var row = sheet.GetRow(0);
            var cell = row?.GetCell(0);
            var val = ExcelApi.GetStringValue(cell);
            var prefixMatch = StringHelper.StartsWith(val, prefix);
            suffix = null;
            if (prefixMatch)
            {
                suffix = val.Substring(prefix.Length);
            }
            return prefixMatch;
        }
        
        /// <summary>
        /// find and parse all messages books in specified folder
        /// </summary>
        public void ParseFolder(string folder)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("ParseFolder({0})", folder);
            }
            foreach (var file in Directory.GetFiles(folder))
            {
                ParseFile(file);
            }
        }

        public bool ParseFile(string file)
        {
            if (!ExcelApi.IsExcelFile(file)) return false;
            var fileName = Path.GetFileName(file);
            if(ExcelApi.IsIgnorableName(fileName)) return false;
            var book = ExcelApi.LoadWorkbook(file);
            ParseBook(book, file);
            return true;
        }

        /// <summary>
        /// parse from all acceptable sheets from workbook
        /// </summary>
        /// <param name="book"></param>
        public void ParseBook(IWorkbook book, string name = null)
        {
            if (Log.IsDebugEnabled) Log.DebugFormat("ParseBook({0})", name);
            for (var i = 0; i < book.NumberOfSheets; i++)
            {
                var sheet = book.GetSheetAt(i);
                var sheetName = sheet.SheetName;
                if (ExcelApi.IsIgnorableName(sheetName))
                {
                    continue;
                }
                var prefixes = GetAcceptableSheetPrefixes();
                foreach (var prefix in prefixes)
                {
                    string suffix;
                    if (AcceptSheetHeader(sheet, prefix, out suffix))
                    {
                        ParseSheet(sheet, prefix, suffix);
                    }
                }
            }
        }

        /// <summary>
        /// retrieve acceptable prefixes for sheet header values
        /// </summary>
        protected abstract string[] GetAcceptableSheetPrefixes();

        /// <summary>
        /// subclasses should implement sheet content parse
        /// </summary>
        protected abstract void ParseSheet(ISheet sheet, string prefix, string suffix);

        /// <summary>
        /// retrieve folder name where excel documents for this parser located 
        /// </summary>
        public abstract string GetSourceFolderName();

        /// <summary>
        /// export parsed resources to specified directory
        /// </summary>
        public abstract void Export(string dir);
    }
}