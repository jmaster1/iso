using Common.Lang.Entity;
using Common.Util;
using NPOI.SS.UserModel;

namespace Common.Editor.Excel
{
    
    /// <summary>
    /// base class for custom excel data parser
    /// </summary>
    public abstract class ExcelCustomInfoParser : GenericBean
    {
        
        public ExcelApi ExcelApi = GetBean<ExcelApi>();
        
        public TextParser TextParser = TextParser.Instance;
        
        /// <summary>
        /// parse data from sheet starting at specified cell
        /// </summary>
        public abstract object Parse(ISheet sheet, ICell startCell);
    }
}