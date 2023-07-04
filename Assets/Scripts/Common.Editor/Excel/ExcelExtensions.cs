using NPOI.SS.UserModel;

namespace Common.Editor.Excel
{
    /// <summary>
    /// extensions to NPOI classes
    /// </summary>
    public static class ExcelExtensions
    {
        public static IRow NextRow(this IRow t)
        {
            return t.Sheet.GetRow(t.RowNum + 1);
        }
    }
}