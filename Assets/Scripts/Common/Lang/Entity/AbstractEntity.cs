using System.Diagnostics;
using System.Text;
using Common.Api.Local;
using Common.Util;

namespace Common.Lang.Entity
{
    /// <summary>
    /// lightweight pojo base class 
    /// </summary>
    public abstract class AbstractEntity : IClearable
    {
        /// <summary>
        /// LocalApi should be externally injected to enable localization support
        /// </summary>
        public static LocalApi LocalApi;

        /// <summary>
        /// retrieve localized message for named property of this bean using pattern
        /// {obj.type.name}.{propertyName}.{suffix}
        /// </summary>
        public string GetObjectMessage(string propertyName, string suffix0 = null, string suffix1 = null)
        {
            return LocalApi.GetObjectMessage(this, propertyName, suffix0, suffix1);
        }

        public string GetObjectMessage(object propertyName, object suffix0 = null, object suffix1 = null)
        {
            return GetObjectMessage(propertyName?.ToString(), suffix0?.ToString(), suffix1?.ToString());
        }

        public virtual void Clear()
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

        /// <summary>
        /// subclasses may override this in order to populate children to buffer, example
        /// sb.BeginList();
        /// foreach (T e in map) {
        ///   sb.ElementSeparator();
        ///   sb.Append(e.type + "=" + e.Amount);
        /// }
        /// sb.EndList();
        /// </summary>
        public virtual void ToString(StringBuilder sb)
        {
            var str = base.ToString();
            sb.Append(str);
        }

        protected void Validate(bool condition, string message = null)
        {
            LangHelper.Validate(condition, message);
        }
        
        protected void Validate(bool condition, string format, object arg0,
            object arg1 = null, object arg2 = null, object arg3 = null, object arg4 = null)
        {
            LangHelper.Validate(condition, format, arg0, arg1, arg2, arg3, arg4);
        }
        
        /// <summary>
        /// same as validate, but bound to debug condition
        /// (i.e. will be shrinked in live)
        /// </summary>
        [Conditional("DEBUG")]
        public void Assert(bool condition)
        {
            LangHelper.Validate(condition);
        }
    }
}