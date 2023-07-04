using System.IO;
using Common.Bind;
using Common.IO.FileSystem;
using Common.IO.Streams;
using Common.Util.Http;

namespace Common.Unity.Util.Debug
{
    /// <summary>
    /// filesystem html adapter
    /// </summary>
    public class FileSystemHtmlAdapter : BindableBean<AbstractFileSystem>
    {
        private const string ParamPath = "path";
        
        public FileSystemHtmlAdapter(AbstractFileSystem model)
        {
            Bind(model);
        }

        public void Delete(string path)
        {
            Model.Delete(path);
        }
        
        public void Content(HttpQuery query, string path, bool download)
        {
            query.SetContentType(download ? "application/binary" : "text/txt");
            var fileName = Path.GetFileName(path);
            if (download)
            {
                query.SetFileName(fileName);
            }

            using (var src = Model.Read(path))
            {
                src.CopyTo(query.OutputStream);
            }
            query.Dispose();
        }
        
        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            var paths = Model.List();
            html.tableHeader("#", "path", "length", "action");
            foreach (var pth in paths)
            {
                var len = Model.GetLength(pth);
                html.tr().tdRowNum()
                    .td().a($"?cmd={nameof(Content)}&{ParamPath}={pth}", pth).endTd().td(len)
                    .td()
                    .a($"?cmd={nameof(Delete)}&{ParamPath}={pth}", "Delete")
                    .a($"?cmd={nameof(Content)}&{ParamPath}={pth}&inline=true", "Download")
                    .endTd().endTr();
            }
            html.endTable();
        }
    }
}