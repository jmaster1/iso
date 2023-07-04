using System.IO;
using Common.Util;
using log4net;
using log4net.Config;
using UnityEngine;

namespace Common.Unity.Util.Log
{
    /// <summary>
    /// log4net configurator that reads from /Assets/Resources/Log/Default|Editor/log4net.xml file
    /// see https://logging.apache.org/log4net/release/manual/configuration.html
    /// </summary>
    public static class UnityLogConfigurator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Configure()
        {
            //
            // init log4net context variables
            var configDir = Application.isEditor ? "Editor" : "Default";
            configDir = Path.Combine("Log", configDir);
            var defaultConfigFilepath = Path.Combine(configDir, "log4net");
            //
            // replace variables
            GlobalContext.Properties["persistentDataPath"] = Application.persistentDataPath;
            GlobalContext.Properties["persistentPrivateDataPath"] = UnityHelper.PersistentPrivateDataPath;

            var xml = Resources.Load<TextAsset>(defaultConfigFilepath)?.text;
            if (xml == null) return;
            //
            // merge with log4net.local.xml
            var localConfigFilepath = Path.Combine(configDir, "log4net.local");
            var xmlLocal = Resources.Load<TextAsset>(localConfigFilepath)?.text;
            if (xmlLocal != null)
            {
                var insertIndex = StringHelper.IndexOf(xml, "</log4net>");
                xml = xml.Substring(0, insertIndex) + xmlLocal + xml.Substring(insertIndex);
            }
            XmlConfigurator.Configure(IoHelper.StringStream(xml));
        }
    }
}
