using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace excel2json {
    /// <summary>
    /// Excel导出工具
    /// 需要依赖：CommandLineParser:http://nuget.org/packages/commandlineparser
    ///         excel2json:https://neil3d.github.io/app/excel2json.html
    ///         Pack: Newtonsoft.Json
    /// 使用_inputDir指定输入的表格目录，建议放置在不会打包的目录下，除非你确实需要在成品包中使用或修改它。
    /// 使用_outputDir指定输出目录
    /// 使用_dataModelDir指定数据模型输出目录
    /// </summary>
    public static class ExcelExporter {
        // private static string _inputDir = Path.Combine(Application.dataPath, "Editor","Excels");
        // private static string _outputDir = Path.Combine(Application.dataPath, "Configs");
        // private static string _dataModelDir = Path.Combine(Application.dataPath, "Scripts","DataModel");

        private static string _inputPath;
        private static string _outputPath;
        private static string _dataModelPath;

        private static int _header;
        private static bool _lowcase;
        private static bool _exportArray;
        private static string _dateFormat;
        private static bool _forceSheetName;
        private static bool _cellJson;
        private static bool _allString;
        private static string _exclude;
        
        [MenuItem("Tools/表工具/导出到json", false, 0)]
        public static void UpdateConfig() {
            FindConfig();
            Exporter(false, true, false);
            AssetDatabase.Refresh();
            
        }
        [MenuItem("Tools/表工具/生成全部数据模型", false, 1)]
        public static void UpdateDataModel() {
            FindConfig();
            Exporter(false, false, true);
            AssetDatabase.Refresh();
        }
        [MenuItem("Tools/表工具/导出json并生成数据模型", false, 2)]
        public static void ExporterAll() {
            FindConfig();
            Exporter(false, true, true);
            AssetDatabase.Refresh();
        }
        [MenuItem("Assets/表工具/导出该文件到json",false,0)]
        public static void UpdateFileToConfig() {
            FindConfig();
            Exporter(true, true, false);
            AssetDatabase.Refresh();
        }
        [MenuItem("Assets/表工具/导出该文件的数据模型",false,1)]
        public static void UpdateFileToDataModel() {
            FindConfig();
            Exporter(true, false, true);
            AssetDatabase.Refresh();
        }
        [MenuItem("Assets/表工具/导出该文件的json并导出该文件的数据模型",false,2)]
        public static void ExporterFileToAll() {
            FindConfig();
            Exporter(true, true, true);
            AssetDatabase.Refresh();
        }

        private static void Exporter(bool target,bool json,bool dataModel) {
            List<Task> tasks = new List<Task>();
            if (target) {
                foreach(Object o in Selection.objects) {
                    string excelPath = AssetDatabase.GetAssetPath(o);
                    if(!string.IsNullOrEmpty(excelPath)) {
                        tasks.Add(ExportTask(excelPath,json,dataModel));
                    }
                }
            } else {
                var files = GetAllExcelPath();
                if (files.Length == 0) {
                    return;
                }
                Directory.CreateDirectory(_outputPath);
                foreach (var excelPath in files) {
                    tasks.Add(ExportTask(excelPath,json,dataModel));
                }
            }
            Task.WaitAll(tasks.ToArray());
            Debug.Log("导出成功");
        }

        private static Task ExportTask(string excelPath,bool json,bool dataModel) {
            var fileTrueName = Path.GetFileNameWithoutExtension(excelPath);
            string outputPath = default;
            string dateModelPath = default;
            if (json) {
                outputPath = Path.Combine(_outputPath, fileTrueName + ".json");
            } 
            if(dataModel) {
                dateModelPath = Path.Combine(_dataModelPath, fileTrueName + ".cs");
            }

            // Run(path, outputPath, dateModelPath); 单线程方案
            return Task.Factory.StartNew(()=>Run(excelPath,outputPath,dateModelPath));
        }
        private static void Run(string excelPath,string exportPath = default,string dateModelPath=default) {
            string excelName = Path.GetFileNameWithoutExtension(excelPath);
            //-- Load Excel
            ExcelLoader excel = new ExcelLoader(excelPath, _header);

            //-- export
            if (exportPath!=default) {
                JsonExporter exporter = new JsonExporter(excel, _lowcase, _exportArray, _dateFormat, _forceSheetName, _header, _exclude, _cellJson, _allString);
                exporter.SaveToFile(exportPath, new UTF8Encoding(false));
            }

            if (dateModelPath != default) {
                CSDefineGenerator generator = new CSDefineGenerator(excelName, excel, _exclude);
                generator.SaveToFile(dateModelPath, new UTF8Encoding(false));
                
                new DataModelExporter(excelName,excel)
                    .SaveToFile(dateModelPath,new UTF8Encoding(false));
            }
        }

        private static string[] GetAllExcelPath() {
            var files = Directory.GetFiles(_inputPath, "*.xlsx", SearchOption.AllDirectories);
            if (files.Length == 0) {
                Logger.Log($"路径{_inputPath}中没有文件");
            }
            return files;
        }

        [MenuItem("Tools/表工具/检查配置文件数据", false, 100)]
        private static void FindConfig() {
            var guids=AssetDatabase.FindAssets("t:Excel2JsonConfig");
            if (guids.Length > 0) {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var config = AssetDatabase.LoadAssetAtPath<Excel2JsonConfig>(path);
                // Debug.Log("excelPath:"+config.excelPath);
                // Debug.Log("OutputPath:"+config.outputPath);
                // Debug.Log("DataModelPath:"+config.dataModelPath);

                _inputPath = config.excelPath;
                _outputPath = config.outputPath;
                _dataModelPath = config.dataModelPath;
                _header = config.header;
                _lowcase = config.lowcase;
                _exportArray = config.exportArray;
                _dateFormat = config.dateFormat;
                _forceSheetName = config.forceSheetName;
                _cellJson = config.cellJson;
                _allString = config.allString;
                _exclude = config.exclude;
            }
        }
    }
}