using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Lang;
using Common.Util;
using Common.Util.Http;
using Common.Util.Reflect;
using Newtonsoft.Json;

namespace Common.IO.Streams
{
    /// <summary>
    /// HtmlWriter
    /// </summary>
    public class HtmlWriter : XmlWriter
    {
        public const string ATTR_CHECKED = "checked";
        public const string ATTR_COLS = "cols";
        public const string ATTR_DISABLED = "disabled";
        public const string ATTR_ID = "id";
        public const string ATTR_METHOD = "method";
        public const string ATTR_NAME = "name";
        public const string ATTR_ROWS = "rows";
        public const string ATTR_TYPE = "type";
        public const string ATTR_VALUE = "value";

        public const string A = "a";
        public const string BUTTON = "button";
        public const string BODY = "body";
        public const string BR = "br";
        public const string DIV = "div";
        public const string FORM = "form";
        public const string H1 = "h1";
        public const string H2 = "h2";
        public const string H3 = "h3";
        public const string HEAD = "head";
        public const string HR = "hr";
        public const string HTML = "html";
        public const string IMG = "img";
        public const string INPUT = "input";
        public const string LABEL = "label";
        public const string SELECT = "select";
        public const string OPTION = "option";
        public const string LI = "li";
        public const string OL = "ol";
        public const string P = "p";
        public const string PRE = "pre";
        public const string SCRIPT = "script";
        public const string SPAN = "span";
        public const string STRONG = "strong";
        public const string STYLE = "style";
        public const string TABLE = "table";
        public const string TEXTAREA = "textarea";
        public const string TD = "td";
        public const string TH = "th";
        public const string THEAD = "thead";
        public const string TR = "tr";
        public const string UL = "ul";

        public const string INPUT_TYPE_SUBMIT = "submit";
        public const string INPUT_TYPE_CHECKBOX = "checkbox";
        public const string INPUT_TYPE_HIDDEN = "hidden";
        public const string INPUT_TYPE_NUMBER = "number";
        public const string INPUT_TYPE_RADIO = "radio";
        public const string INPUT_TYPE_TEXT = "text";

        public const string PARAM_CMD = "cmd";

        public JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();

        public HtmlWriter(TextWriter target) : base(target)
        {
        }

        public HtmlWriter attrs(params object[] namesAndValues)
        {
            for (int i = 0; i < namesAndValues.Length; i += 2)
            {
                object name = namesAndValues[i];
                object value = namesAndValues[i + 1];
                attr(name.ToString(), value);
            }

            return this;
        }

        public HtmlWriter attrId(string value)
        {
            return (HtmlWriter) attr("id", value);
        }

        public HtmlWriter attrName(string value)
        {
            return (HtmlWriter) attr("name", value);
        }

        public HtmlWriter attrClass(string value)
        {
            return (HtmlWriter) attr("class", value);
        }

        public HtmlWriter attrValue(string value)
        {
            return (HtmlWriter) attr(ATTR_VALUE, value);
        }

        public HtmlWriter attrStyle(string value)
        {
            return (HtmlWriter) attr("style", value);
        }

        public HtmlWriter attrTitle(string value)
        {
            return (HtmlWriter) attr("title", value);
        }

        public HtmlWriter attrValign(string value)
        {
            return (HtmlWriter) attr("valign", value);
        }

        public HtmlWriter attrAlign(string value)
        {
            return (HtmlWriter) attr("align", value);
        }

        public HtmlWriter attrAlignRight()
        {
            return attrAlign("right");
        }

        public HtmlWriter attrRowspan(int value)
        {
            return (HtmlWriter) attr("rowspan", ToString(value));
        }

        public HtmlWriter attrColspan(int value)
        {
            return (HtmlWriter) attr("colspan", ToString(value));
        }

        public HtmlWriter a()
        {
            return (HtmlWriter) tag(A);
        }

        public HtmlWriter button()
        {
            return (HtmlWriter) tag(BUTTON);
        }

        public HtmlWriter endButton()
        {
            return (HtmlWriter) end(BUTTON);
        }

        public HtmlWriter a(string href, object content)
        {
            a().attrHref(href).textHtml(content);
            return endA();
        }

        public HtmlWriter textHtml(object content)
        {
            HtmlPre htmlPre = content as HtmlPre;
            if (htmlPre != null)
            {
                pre(htmlPre.text);
            }
            else
            {
                base.text(content);
            }

            return this;
        }

        public HtmlWriter endA()
        {
            return (HtmlWriter) end(A);
        }

        public HtmlWriter attrHref(object value)
        {
            return (HtmlWriter) attr("href", value);
        }

        public HtmlWriter attrOnClick(object value)
        {
            return (HtmlWriter) attr("onclick", value);
        }

        public HtmlWriter form()
        {
            return (HtmlWriter) tag(FORM);
        }

        public HtmlWriter form(string name, string method)
        {
            return (HtmlWriter) form().attr(ATTR_NAME, name).attr(ATTR_METHOD, method);
        }

        public HtmlWriter formPost()
        {
            return form(null, "post");
        }

        public HtmlWriter endForm()
        {
            return (HtmlWriter) end(FORM);
        }

        public HtmlWriter textarea()
        {
            return (HtmlWriter) tag(TEXTAREA);
        }

        public HtmlWriter endTextarea()
        {
            return (HtmlWriter) end(TEXTAREA);
        }

        public HtmlWriter textarea(string name, int cols, int rows)
        {
            return (HtmlWriter) tag(TEXTAREA).attr(ATTR_NAME, name).attr(ATTR_COLS, cols).attr(ATTR_ROWS, rows);
        }

        public HtmlWriter input()
        {
            return (HtmlWriter) tag(INPUT);
        }

        public HtmlWriter endInput()
        {
            return (HtmlWriter) end(INPUT);
        }

        public HtmlWriter inputNumber(string name, object value, params object[] attrs)
        {
            tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_NUMBER).attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            if (attrs != null)
            {
                for (int i = 0; i < attrs.Length;)
                {
                    object attrName = attrs[i++];
                    object attrVal = attrs[i++];
                    attr(ToString(attrName), attrVal);
                }
            }

            return (HtmlWriter) end();
        }

        public HtmlWriter inputText(string name, object value, params object[] attrs)
        {
            tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_TEXT).attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            if (attrs != null)
            {
                for (var i = 0; i < attrs.Length;)
                {
                    var attrName = attrs[i++];
                    var attrVal = attrs[i++];
                    attr(ToString(attrName), attrVal);
                }
            }

            return (HtmlWriter) end();
        }

        public HtmlWriter inputRadio(string name, object value, bool check)
        {
            tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_RADIO).attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            if (check)
            {
                attr(ATTR_CHECKED, true);
            }

            return (HtmlWriter) end();
        }

        public HtmlWriter select()
        {
            return (HtmlWriter) tag(SELECT);
        }

        public HtmlWriter select(string name)
        {
            return select().attrName(name).attrId(name);
        }
        
        public HtmlWriter select<T>(string name, T selectedObject, IEnumerable options, 
            IStringConverter<T> converterValue, IStringConverter<T> converterContent = null)
        {
            select(name);
            foreach (T e in options)
            {
                var value = converterValue.ToString(e);
                var content = converterContent == null ? value : converterContent.ToString(e);
                var selected = LangHelper.Equals(selectedObject, e);
                selectOption(value, content, selected);
            }

            return endSelect();
        }

        public HtmlWriter selectForEnumerable<T>(string name, IEnumerable<T> values, T selected = default)
            where T : IIdAware<string>
        {
            select(name);
            var sortedValues = values.OrderBy(key => key.GetId());
            foreach (var e in sortedValues)
            {
                selectOption(e.GetId(), e.GetId(), e.Equals(selected));
            }

            return endSelect();
        }

        public HtmlWriter selectForEnum<T>(string name, T selected = default) where T : Enum
        {
            T[] values = LangHelper.EnumValues<T>();
            select(name);
            foreach (T e in values)
            {
                selectOption(e.ToString(), e.ToString(), e.Equals(selected));
            }

            return endSelect();
        }

        public HtmlWriter endSelect()
        {
            return (HtmlWriter) end(SELECT);
        }

        public HtmlWriter selectOption()
        {
            return (HtmlWriter) tag(OPTION);
        }

        public HtmlWriter endSelectOption()
        {
            return (HtmlWriter) end(OPTION);
        }

        public HtmlWriter selectOption(string value, string content, bool selected)
        {
            selectOption().attr("value", value);
            if (selected)
            {
                attr("selected", "true");
            }

            textHtml(content);
            endSelectOption();
            return this;
        }

        public HtmlWriter table()
        {
            trIndex = 0;
            return (HtmlWriter) tag(TABLE);
        }

        public HtmlWriter endTable()
        {
            return (HtmlWriter) end(TABLE);
        }

        public HtmlWriter tr()
        {
            return (HtmlWriter) tag(TR);
        }

        public HtmlWriter endTr()
        {
            return (HtmlWriter) end(TR);
        }

        public HtmlWriter td()
        {
            return (HtmlWriter) tag(TD);
        }

        public HtmlWriter tdCheck(bool b)
        {
            return td(b ? "✓" : StringHelper.EmptyString);
        }

        public HtmlWriter endTd()
        {
            return (HtmlWriter) end(TD);
        }

        public HtmlWriter td(string attrClass, string text)
        {
            td().attrClass(attrClass).textHtml(text);
            return endTd();
        }

        /// <summary>
        /// write td with content
        /// </summary>
        public HtmlWriter td(object value)
        {
            td();
            if (!value.IsEnum() && value.IsNumericType())
            {
                attrAlignRight();
            }

            textHtml(value);
            return endTd();
        }

        /// <summary>
        /// render td with linked content
        /// </summary>
        public void tdA(string content, string href)
        {
            td().a(href, content).endTd();
        }

        /// <summary>
        /// write td with numeric content (right-aligned)
        /// </summary>
        public HtmlWriter tdNum(object text)
        {
            td().attrAlignRight().textHtml(text);
            return endTd();
        }

        /// <summary>
        /// write td with preformatted content
        /// </summary>
        public HtmlWriter tdPre(object content)
        {
            return td().pre(content).endTd();
        }

        public HtmlWriter thead()
        {
            return (HtmlWriter) tag(THEAD);
        }

        public HtmlWriter endThead()
        {
            return (HtmlWriter) end(THEAD);
        }

        public HtmlWriter th()
        {
            return (HtmlWriter) tag(TH);
        }

        public HtmlWriter endTh()
        {
            return (HtmlWriter) end(TH);
        }

        public HtmlWriter th(object text)
        {
            th().textHtml(text);
            return endTh();
        }

        public HtmlWriter th(params object[] values)
        {
            foreach (var value in values)
            {
                th(value);
            }

            return this;
        }

        public HtmlWriter h1(string text)
        {
            return (HtmlWriter) tag(H1, text);
        }

        public HtmlWriter h2(string text)
        {
            return (HtmlWriter) tag(H2, text);
        }

        public HtmlWriter h3(string text)
        {
            return (HtmlWriter) tag(H3, text);
        }

        public HtmlWriter p(string text)
        {
            return (HtmlWriter) tag(P, text);
        }

        public HtmlWriter strong(string text)
        {
            return (HtmlWriter) tag(STRONG, text);
        }

        public HtmlWriter ol()
        {
            return (HtmlWriter) tag(OL);
        }

        public HtmlWriter endOl()
        {
            return (HtmlWriter) end(OL);
        }

        public HtmlWriter ul()
        {
            return (HtmlWriter) tag(UL);
        }

        public HtmlWriter endUl()
        {
            return (HtmlWriter) end(UL);
        }

        public HtmlWriter li()
        {
            return (HtmlWriter) tag(LI);
        }

        public HtmlWriter li(object content)
        {
            li().textHtml(content);
            return endLi();
        }

        public HtmlWriter endLi()
        {
            return (HtmlWriter) end(LI);
        }

        public HtmlWriter pre()
        {
            return (HtmlWriter) tag(PRE);
        }

        public HtmlWriter endPre()
        {
            return (HtmlWriter) end(PRE);
        }

        public HtmlWriter pre(object content)
        {
            return pre().textHtml(content).endPre();
        }

        public HtmlWriter style()
        {
            return (HtmlWriter) tag(STYLE);
        }

        public HtmlWriter endStyle()
        {
            return (HtmlWriter) end(STYLE);
        }

        public HtmlWriter style(string content)
        {
            style().plain(content);
            return endStyle();
        }

        public HtmlWriter div()
        {
            return (HtmlWriter) tag(DIV);
        }

        public HtmlWriter endDiv()
        {
            return (HtmlWriter) end(DIV);
        }

        public HtmlWriter span()
        {
            return (HtmlWriter) tag(SPAN);
        }

        public HtmlWriter endSpan()
        {
            return (HtmlWriter) end(SPAN);
        }

        public HtmlWriter html()
        {
            return (HtmlWriter) tag(HTML);
        }

        public HtmlWriter endHtml()
        {
            return (HtmlWriter) end(HTML);
        }

        public HtmlWriter head()
        {
            return (HtmlWriter) tag(HEAD);
        }

        public HtmlWriter endHead()
        {
            return (HtmlWriter) end(HEAD);
        }

        public HtmlWriter body()
        {
            return (HtmlWriter) tag(BODY);
        }

        public HtmlWriter endBody()
        {
            return (HtmlWriter) end(BODY);
        }

        public HtmlWriter br()
        {
            return (HtmlWriter) plain("<br/>");
        }

        public HtmlWriter script()
        {
            return (HtmlWriter) tag(SCRIPT);
        }

        public HtmlWriter endScript()
        {
            return (HtmlWriter) end(SCRIPT);
        }

        public HtmlWriter script(string content)
        {
            return (HtmlWriter) tag(SCRIPT).raw(content).end();
        }

        /// <summary>
        /// writes <script src="${src}"><script>
        /// </summary>
        public HtmlWriter scriptSrc(string src)
        {
            return (HtmlWriter) tag(SCRIPT).attr("src", src).end();
        }

        public HtmlWriter submit()
        {
            return (HtmlWriter) tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_SUBMIT).end(INPUT);
        }

        public HtmlWriter submit(string value)
        {
            return (HtmlWriter) tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_SUBMIT).attr(ATTR_VALUE, value).end(INPUT);
        }

        public HtmlWriter submit(string name, string value)
        {
            return (HtmlWriter) tag(INPUT)
                .attr(ATTR_TYPE, INPUT_TYPE_SUBMIT)
                .attr(ATTR_NAME, name)
                .attr(ATTR_VALUE, value)
                .attr(ATTR_ID, value)
                .end(INPUT);
        }

        /// <summary>
        /// render submit buttons with name="cmd" and specified values
        /// </summary>
        public HtmlWriter cmd(params string[] values)
        {
            foreach (var e in values)
            {
                submit(PARAM_CMD, e);
            }

            return this;
        }

        public HtmlWriter submit(string name, string value, bool disabled)
        {
            tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_SUBMIT).attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            if (disabled)
            {
                attr(ATTR_DISABLED, disabled);
            }

            return (HtmlWriter) end(INPUT);
        }

        public HtmlWriter inputHidden(string name, object value)
        {
            input().attr(ATTR_TYPE, INPUT_TYPE_HIDDEN)
                .attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            return endInput();
        }

        public HtmlWriter inputCheckbox(string name, object value, bool check)
        {
            tag(INPUT).attr(ATTR_TYPE, INPUT_TYPE_CHECKBOX).attr(ATTR_NAME, name).attr(ATTR_VALUE, value);
            if (check)
            {
                attr(ATTR_CHECKED, true);
            }

            return (HtmlWriter) end();
        }

        public HtmlWriter inputCheckboxLabel(string name, bool check, string label)
        {
            tag(LABEL);
            inputCheckbox(name, null, check);
            textHtml(label);
            return (HtmlWriter) end(LABEL);
        }

        public HtmlWriter inputRadioLabel(string name, string value, bool check, string label)
        {
            tag(LABEL);
            inputRadio(name, value, check);
            textHtml(label);
            return (HtmlWriter) end(LABEL);
        }

        public HtmlWriter button(string name, object value, string text)
        {
            tag("button")
                .attr(ATTR_TYPE, "submit")
                .attr(ATTR_NAME, name)
                .attr(ATTR_VALUE, value);
            textHtml(text);
            return (HtmlWriter) end();
        }

        public HtmlWriter hr()
        {
            return (HtmlWriter) tag(HR).end(HR);
        }

        /// <summary>
        /// writes property table using name=value pairs, each pair in a row of 2 cells
        /// </summary>
        public void propertyTable(params object[] namesAndValues)
        {
            table().attr("border", "1");
            for (int i = 0; i < namesAndValues.Length; i += 2)
            {
                object name = namesAndValues[i];
                object value = namesAndValues[i + 1];
                tr().td(name).td(value).endTr();
            }
            endTable();
        }

        /// <summary>
        /// opens table and writes column headings row,
        /// don't forget to call endTable() on end
        /// </summary>
        public HtmlWriter tableHeader(params object[] columnNames)
        {
            table().attr("border", "1")
                .attr("cellspacing", "0")
                .attr("cellpadding", "2");
            tr();
            foreach (object name in columnNames)
            {
                th(name);
            }

            return endTr();
        }

        public HtmlWriter linkStylesheet(string href)
        {
            return (HtmlWriter) tag("link").attr("rel", "stylesheet").attr("href", href).end();
        }

        /// <summary>
        /// create http url parameters part
        /// </summary>
        public string createParameters(params object[] namesAndValues)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('?');
            for (int i = 0; i < namesAndValues.Length; i += 2)
            {
                object name = namesAndValues[i];
                object value = namesAndValues[i + 1];
                sb.Append(name);
                sb.Append('=');
                sb.Append(value);
                sb.Append('&');
            }

            return sb.ToString();
        }

        public HtmlWriter nbsp()
        {
            return (HtmlWriter) plain("&nbsp;");
        }

        public HtmlWriter commandsForm(params object[] values)
        {
            form();
            foreach (var e in values)
            {
                submit(PARAM_CMD, ToString(e));
            }

            endForm();
            return this;
        }

        public HtmlWriter img(string src)
        {
            return (HtmlWriter) tag(IMG).attr("src", src);
        }

        public HtmlWriter eol()
        {
            return (HtmlWriter) raw(StringHelper.EOL);
        }

        int trIndex;

        /// <summary>
        /// render td with current table row number
        /// </summary>
        public HtmlWriter tdRowNum()
        {
            return tdNum(++trIndex);
        }

        public HtmlWriter submitCmd(string cmd)
        {
            return submit(PARAM_CMD, cmd);
        }

        public HtmlWriter attrTargetBlank()
        {
            return (HtmlWriter) attr("target", "_blank");
        }

        /// <summary>
        /// write json text from object
        /// </summary>
        public HtmlWriter json(object val)
        {
            string json = ToJson(val);
            textHtml(json);
            return this;
        }

        public string ToJson(object val)
        {
            using (StringWriter sw = new StringWriter())
            {
                JsonSerializer.Serialize(sw, val);
                return sw.ToString();
            }
        }

        public HtmlPre ToJsonPre(object val)
        {
            return new HtmlPre(ToJson(val));
        }

        public HtmlWriter txt(object val)
        {
            return textHtml(val);
        }

        public HtmlWriter endTag()
        {
            end();
            return this;
        }

        public HtmlWriter endImg()
        {
            end(IMG);
            return this;
        }

        public HtmlWriter renderInvokeMethods(object bean, object parent = null)
        {
            HttpInvokeHandler.RenderHttpInvokeMethods(this, bean, parent);
            return this;
        }

        public new HtmlWriter text(object val)
        {
            base.text(val);
            return this;
        }

        /// <summary>
        /// retrieve color from int formatted as #rrggbbaa
        /// </summary>
        public string color(uint color)
        {
            return "#" + color.ToString("X8");
        }

        public HtmlWriter attrHtml(string name, object value)
        {
            return (HtmlWriter) base.attr(name, value);
        }
    }
}