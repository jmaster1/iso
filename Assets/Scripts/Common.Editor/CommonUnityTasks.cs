using System;
using System.Collections.Generic;
using System.IO;
using Common.Editor.Excel;
using Common.Unity.Util;
//using UnityEditor;

// using Common.Unity.Util;
// using Common.Unity.Util.Log;
// using UnityEditor;
// using UnityEngine;

namespace Common.Editor
{
    /// <summary>
    /// unity build tasks for unicom
    /// </summary>
    public static class CommonUnityTasks
    {
        /// <summary>
        /// folder name  containing source documents 
        /// </summary>
        public static string Docs = "Docs";

        /// <summary>
        /// folder name containing resources assets 
        /// </summary>
        public static string Resources = "Resources";

        /// <summary>
        /// this should return assets directory location
        /// i.e. {path_to_project}/Assets
        /// </summary>
        public static Func<string> DataPathProvider;

        public static Action PostBuildAction;

        static CommonUnityTasks()
        {
            //UnityLogConfigurator.Configure();
        }
        
        private static List<AbstractExcelDataParser> GetParsers()
        {
            return new List<AbstractExcelDataParser>
            {
                new ExcelMessagesParser(),
                new ExcelInfoParser()
            };
        }
        
        public static void BuildLocalization()
        {
            Build(new ExcelMessagesParser());
        }

        public static void BuildInfo()
        {
            Build(new ExcelInfoParser());
        }

        public static void BuildAll()
        {
            BuildLocalization();
            BuildInfo();
        }
        
        public static void Build(AbstractExcelDataParser parser)
        {
            var dataPath = DataPathProvider();
            var root = Path.GetFullPath(dataPath);
            var folderName = parser.GetSourceFolderName();
            var docsPath = Path.Combine(root, Docs, folderName);
            var outPath = Path.Combine(dataPath, Resources, folderName);
            Build(parser, docsPath, outPath);
            PostBuildAction?.Invoke();
        }
        
        public static void Build(AbstractExcelDataParser parser, string docsPath, string outPath)
        {
            parser.ParseFolder(docsPath);
            if (outPath == null) return;
            Directory.CreateDirectory(outPath);
            parser.Export(outPath);
        }
        
        // public static void Build(List<string> files)
        // {
        //     if (files.Count == 0) return;
        //     var parsers = GetParsers();
        //     foreach (var parser in parsers)
        //     {
        //         var found = false;
        //         var searchPath = Path.Combine( Docs, parser.GetSourceFolderName());
        //         foreach (var file in files.Where(file => file.Contains(searchPath)))
        //         {
        //             found |= parser.ParseFile(file);
        //         }
        //         if (!found) continue;
        //         var outPath = Path.Combine(Application.dataPath, Resources, parser.GetSourceFolderName());
        //         Directory.CreateDirectory(outPath);
        //         parser.Export(outPath);
        //     }
        //     AssetDatabase.Refresh();                 
        // }
    }
}
