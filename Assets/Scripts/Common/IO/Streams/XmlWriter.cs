using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Common.Lang;

namespace Common.IO.Streams
{
	/// <summary>
	/// XmlWriter
	/// </summary>
	public class XmlWriter : FilterTextWriter, IClearable
	{

		/// <summary>
		/// element stack
		/// </summary>
		readonly Stack<string> tagStack = new Stack<string>();

		/// <summary>
		/// shows whether current tag is open
		/// </summary>
		bool tagOpen;

		public XmlWriter(TextWriter target) : base(target)
		{
		}

		public void Clear()
		{
			tagStack.Clear();
			tagOpen = false;
		}

		/// <summary>
		/// write string to writer
		/// </summary>
		protected XmlWriter w(string text)
		{
			if (text != null) Write(text);
			return this;
		}

		/// <summary>
		/// write text inside cdata
		/// </summary>
		public XmlWriter cdata(string content)
		{
			closeTag();
			w("<![CDATA[").w(content).w("]]>");
			return this;
		}

		/// <summary>
		/// close currently open tag, if any
		/// </summary>
		/// <returns>true if last open tag closed, false otherwise</returns>
		protected bool closeTag()
		{
			if (tagOpen)
			{
				w(">");
				tagOpen = false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// open new tag
		/// </summary>
		public XmlWriter tag(string tag)
		{
			closeTag();
			w("<");
			w(tag);
			tagStack.Push(tag);
			tagOpen = true;
			return this;
		}

		public XmlWriter tag(string name, string content)
		{
			tag(name);
			text(content);
			return end(name);
		}

		public XmlWriter end(string tag)
		{
			closeTag();
			string tagActual = tagStack.Pop();
			if (tag != null && !tagActual.Equals(tag))
			{
				throw new InvalidOperationException(
					$"Expected close tag: {tag}, actual: {tagActual}");
			}

			w("</").w(tagActual).w(">\r\n");
			return this;
		}

		public XmlWriter end()
		{
			return end(null);
		}

		/// <summary>
		/// write text as is
		/// </summary>
		public virtual XmlWriter plain(string text)
		{
			closeTag();
			return w(text);
		}

		public XmlWriter raw(string text)
		{
			closeTag();
			w(text);
			return this;
		}

		public XmlWriter text(object val, bool pre)
		{
			closeTag();
			string txt = val == null ? null : ToString(val);
			string html = ToHtmlText(txt, pre);
			w(html);
			return this;
		}

		public static string ToHtmlText(string txt, bool pre)
		{
			return HttpUtility.HtmlEncode(txt);
		}

		protected string ToString(object val)
		{
			return val == null ? "" : val.ToString();
		}

		public XmlWriter textPre(object val)
		{
			return text(val, true);
		}

		public XmlWriter text(object val)
		{
			return text(val, false);
		}

		public XmlWriter raw(object val)
		{
			return text(val == null ? null : ToString(val));
		}

		public XmlWriter comment(string text)
		{
			raw("<!--");
			raw(text);
			raw("-->");
			return this;
		}
	
		/// <summary>
		/// write attribute name/value, if value is not null
		/// </summary>
		public XmlWriter attr(string name, object value)
		{
			if (!tagOpen) throw new InvalidOperationException("No open tag in context");
			if (value != null)
			{
				Write(' ');
				Write(name);
				Write('=');
				Write('\"');
				Write(HttpUtility.HtmlAttributeEncode(ToString(value)));
				Write('\"');
			}
			return this;
		}
	}
}