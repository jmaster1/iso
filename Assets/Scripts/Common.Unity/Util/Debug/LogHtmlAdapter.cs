using System;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Entity;
using Common.Util;
using Common.Util.Http;
using log4net;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Common.Unity.Util.Debug
{
    /// <summary>
    /// LogManager html adapter
    /// </summary>
    public class  LogHtmlAdapter : GenericBean
    {
        private static ILoggerRepository Repo => LogManager.GetRepository();

        private static readonly Level[] Levels = {Level.Debug, Level.Info, Level.Warn, Level.Error, Level.Fatal};

        private static readonly LogLevelStringConverter LogLevelStringConverter = new LogLevelStringConverter(); 

        public void SetLevel(string loggerName, string levelName)
        {
            var level = Repo.LevelMap[levelName];
            var logger = (Logger)Repo.GetLogger(loggerName);
            logger.Level = level;
        }
        
        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.h3("Current loggers");
            var loggers = Repo.GetCurrentLoggers();
            Array.Sort(loggers, (l0, l1) => StringHelper.Compare(l0.Name, l1.Name));
            html.tableHeader("#", "name", "level");
            foreach (var e in loggers)
            {
                var logger = (Logger)e;
                html.tr().tdRowNum().td(e.Name).td().form()
                    .inputHidden("loggerName", e.Name)
                    .select("levelName", logger.EffectiveLevel, Levels, LogLevelStringConverter)
                    .submitCmd(nameof(SetLevel))
                    .endForm().endTd().endTr();
            }
            html.endTable();
        }
    }
    
    public class LogLevelStringConverter : IStringConverter<Level>
    {
        private static ILoggerRepository Repo => LogManager.GetRepository();
        
        public string ToString(Level val)
        {
            return val.Name;
        }

        public Level FromString(string str)
        {
            return Repo.LevelMap[str];
        }
    }
}
