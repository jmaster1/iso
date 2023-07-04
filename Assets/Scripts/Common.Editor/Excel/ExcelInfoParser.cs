using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common.Api.Info;
using Common.IO.Serialize;
using Common.Lang;
using Common.Lang.Collections;
using Common.Util;
using Common.Util.Reflect;
using NPOI.SS.UserModel;

namespace Common.Editor.Excel
{
    /// <summary>
    /// parse info descriptors (forms/lists) from excel workbooks
    /// </summary>
    public class ExcelInfoParser : AbstractExcelDataParser
    {
        /// <summary>
        /// prefix in sheet cell A1 value required to recognize sheet entity list source
        /// </summary>
        public const string ListPrefix = "list:";
        
        /// <summary>
        /// prefix in sheet cell A1 value required to recognize sheet entity form source
        /// </summary>
        public const string FormPrefix = "form:";
        
        /// <summary>
        /// prefix to recognize custom parser class
        /// </summary>
        public const string CustomPrefix = "custom:";
        
        /// <summary>
        /// json serializer
        /// </summary>
        public JsonObjectSerializer JsonSerializer = new NewtonsoftJsonObjectSerializer();
        
        public Map<string, List<object>> ParsedLists = new Map<string, List<object>>();
        
        public Map<string, object> ParsedForms = new Map<string, object>();
        
        public Map<string, object> ParsedCustoms = new Map<string, object>();
        
        protected override string[] GetAcceptableSheetPrefixes()
        {
            return new[] {ListPrefix, FormPrefix, CustomPrefix};
        }

        public override string GetSourceFolderName()
        {
            return InfoApi.Folder;
        }
        
        /// <summary>
        /// inject target.field value parsed from cell
        /// </summary>
        /// <returns>false if cell is null or empty</returns>
        bool InjectFieldValue(object target, FieldInfo field, ICell cell)
        {
            var text = ExcelApi.GetStringValue(cell);
            if (string.IsNullOrEmpty(text)) return false;
            var fieldType = field.FieldType;
            try
            {
                var val = TextParser.Parse(text, fieldType);
                field.SetValue(target, val);
            }
            catch (Exception ex)
            {
                LangHelper.Handle(ex, $"target={target}, field={field}, text={text}, cell={cell.RowIndex}:{cell.ColumnIndex}");
            }
            return true;
        }
        
        protected override void ParseSheet(ISheet sheet, string prefix, string suffix)
        {
            var sheetName = sheet.SheetName;
            var type = Type.GetType(suffix);
            LangHelper.Validate(type != null, $"Type not found: ${suffix}");
            if (ListPrefix.Equals(prefix))
            {
                var list = ParseSheetList(sheet, type);
                //
                // add or merge with existing
                var existing = ParsedLists.Find(sheetName);
                if (existing != null)
                {
                    existing.AddRange(list);
                }
                else
                {
                    ParsedLists.Add(sheetName, list);
                }
            } else if (FormPrefix.Equals(prefix))
            {
                var form = ParseSheetForm(sheet, type);
                ParsedForms.Add(sheetName, form);
            } else if (CustomPrefix.Equals(prefix))
            {
                var obj = ParseSheetCustom(sheet, type);
                ParsedCustoms.Add(sheetName, obj);
            }
        }

        /// <summary>
        /// parse custom data using specified type that should be extension of ExcelCustomInfoParser
        /// </summary>
        private object ParseSheetCustom(ISheet sheet, Type type)
        {
            var parser = (ExcelCustomInfoParser) ReflectHelper.NewInstance(type);
            var row = sheet.GetRow(1);
            var cell = row.GetCell(0);
            var ret = parser.Parse(sheet, cell);
            return ret;
        }

        /// <summary>
        /// parse list of objects from sheet,
        /// row 2 must contain column headings matching type field names,
        /// rows above contain elements to parse
        /// </summary>
        private List<object> ParseSheetList(ISheet sheet, Type type)
        {
            Log.InfoFormat("ParseSheetList, sheet={0}, type={1}", sheet.SheetName, type.FullName);
            var list = new List<object>();
            //
            // collect field names
            var row = sheet.GetRow(1);
            var cells = row.Cells;
            var fields = new Map<FieldInfo, int>();
            for (var i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                var fieldName = ExcelApi.GetStringValue(cell);
                if(string.IsNullOrEmpty(fieldName) || ExcelApi.IsIgnorableName(fieldName)) continue;
                var field = ReflectHelper.GetField(type, fieldName);
                fields.Add(field, i);
            }
            //
            // parse elements from rows
            var lastRowNum = sheet.LastRowNum;
            for (var i = 2; i <= lastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;
                var obj = ReflectHelper.NewInstance(type);
                var skip = true;
                foreach (var field in fields.Keys)
                {
                    var col = fields.Get(field);
                    var cell = row.GetCell(col);
                    if (cell == null || cell.CellType == CellType.Blank) continue;
                    if(!InjectFieldValue(obj, field, cell)) continue;
                    skip = false;
                }

                if (!skip)
                {
                    list.Add(obj);
                }
            }
            return list;
        }

        /// <summary>
        /// parse single entity from sheet,
        /// column A must contain field names, column B - values
        /// </summary>
        private object ParseSheetForm(ISheet sheet, Type type)
        {
            Log.InfoFormat("ParseSheetForm, sheet={0}, type={1}", sheet.SheetName, type.FullName);
            var obj = ReflectHelper.NewInstance(type);
            var lastRowNum = sheet.LastRowNum;
            for (int i = 1; i <= lastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if(row == null) continue;
                var fieldCell = row.GetCell(0);
                var valueCell = row.GetCell(1);
                if (fieldCell != null && valueCell != null)
                {
                    var fieldName = ExcelApi.GetStringValue(fieldCell);
                    if(string.IsNullOrEmpty(fieldName)) continue;
                    var field = ReflectHelper.GetField(type, fieldName);
                    InjectFieldValue(obj, field, valueCell);
                }
            }
            return obj;
        }
        
        public override void Export(string dir)
        {
            Export(dir, ParsedLists, "List");
            Export(dir, ParsedForms, "Form");
            Export(dir, ParsedCustoms, "Custom");
        }

        void Export<T>(string dir, Map<string, T> map, string type)
        {
            Log.InfoFormat("Exporting {0} objects of type {1} to {2}", 
                map.Count, type, dir);
            foreach (var name in map.Keys)
            {
                var obj = map.Get(name);
                Export(dir, name, obj);
            }
        }

        public void Export(string dir, string name, object obj)
        {
            try
            {
                var json = JsonSerializer.ToJson(obj);
                var filePath = Path.Combine(dir, name + ".json");
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                LangHelper.Handle(ex, "Export({0}, {1}, {2} failed", dir, name, obj);
            }
        }
    }
}