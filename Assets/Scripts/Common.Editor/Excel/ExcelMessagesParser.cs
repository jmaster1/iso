using System;
using System.IO;
using Common.Lang;
using Common.Util;
using NPOI.SS.UserModel;
using Common.Api.Local;
using Common.Api.Resource;
using Common.IO.Streams;

namespace Common.Editor.Excel
{
    /// <summary>
    /// parse messages from excel workbooks
    /// </summary>
    public class ExcelMessagesParser : AbstractExcelDataParser
    {
        /// <summary>
        /// prefix in sheet cell A1 value required to recognize sheet as messages source
        /// </summary>
        public const string CellPrefix = "messages:";
        
        /// <summary>
        /// parsed messages (result)
        /// </summary>
        public Map<string, MessagesTable> Tables = new Map<string, MessagesTable>();

        protected override string[] GetAcceptableSheetPrefixes()
        {
            return new[] {CellPrefix};
        }

        /// <summary>
        /// parse messages from given sheet
        /// </summary>
        protected override void ParseSheet(ISheet sheet, string prefix, string suffix)
        {
            string sheetName = sheet.SheetName;
            Log.InfoFormat("ParseSheet({0})", sheetName);
            //
            // retrieve heading row
            IRow headingRow = sheet.GetRow(1);
            for (int col = 1;; col++)
            {
                ICell headingCell = headingRow.GetCell(col);
                if(headingCell == null) {
                    break;
                }
                string locale = ExcelApi.GetStringValue(headingCell);
                if(locale == null || ExcelApi.IsIgnorableName(locale)) {
                    continue;
                }
                //
                // parse keys/values
                for (int i = 2, n = sheet.LastRowNum; i <= n; i++)
                {
                    IRow row = sheet.GetRow(i);
                    //
                    // parse key
                    ICell cell = row?.GetCell(0);
                    if(cell == null) {
                        continue;
                    }
                    string key = ExcelApi.GetStringValue(cell);
                    if(string.IsNullOrEmpty(key)) {
                        continue;
                    }
                    LangHelper.Validate(!key.Contains(StringHelper.Space), 
                        $"key value MUST NOT contains space symbol(s), key={key}, row={i}");
                    //
                    // parse value
                    cell = row.GetCell(col);
                    if (cell == null) continue;
                    String value = ExcelApi.GetStringValue(cell);
                    if (value == null) continue;
                    //
                    // get or create table
                    MessagesTable table;
                    if (!Tables.TryGetValue(locale, out table))
                    {
                        table = new MessagesTable();
                        table.Map = new Map<int, string>(1024);
                        Tables.Add(table.Id = locale, table);
                    }
                    //
                    // add to table
                    int hash = StringHelper.Hash(key);
                    LangHelper.Validate(!table.Map.ContainsKey(hash), 
                        $"Duplicate key: {key}");
                    table.Map.Add(hash, value);
                }
            }
        }

        public override string GetSourceFolderName()
        {
            return LocalApi.Folder;
        }
        
        /// <summary>
        /// export all the tables to specified directory
        /// </summary>
        public override void Export(string dir)
        {
            Log.InfoFormat("Exporting {0} tables to {1}", Tables.Count, dir);
            foreach (var e in Tables)
            {
                string fileName = LocalApi.GetMessagesFileName(e.Value.Id) + ResourceApi.FileExtension;
                string filePath = Path.Combine(dir, fileName);
                BinaryWriterEx.Write(e.Value, filePath);
            }
        }
    }
}