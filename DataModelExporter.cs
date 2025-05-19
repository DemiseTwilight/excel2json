using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace excel2json {
    /// <summary>
    /// 数据模型导出类，复杂情况请参考CSDefineGenerator类
    /// 第一行为生成的键名
    /// 第二行为数据结构
    /// 第三行为注释
    /// </summary>
    public class DataModelExporter {
        struct FieldDef
        {
            public string name;
            public string type;
            public string comment;
        }
        string _mCode;
        public string Code => _mCode;
        public DataModelExporter(string excelName, ExcelLoader excel) {
            //-- 创建代码字符串
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//");
            sb.AppendLine("// Auto Generated Code By excel2json");
            sb.AppendLine("// https://neil3d.github.io/app/excel2json.html");
            sb.AppendLine("// 1. 每个文件形成一个 Struct 定义, 文件的名称作为 Struct 的名称");
            sb.AppendLine("// 2. 表格约定：第一行是变量名称，第二行是变量类型");
            sb.AppendLine();
            sb.AppendFormat("// Generate From {0}.xlsx", excelName);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append(ExportSheet(excelName, excel.Sheets[0]));
            //如果需要在一个表格文件里放多张表
            // for (int i = 0; i < excel.Sheets.Count; i++)
            // {
            //     DataTable sheet = excel.Sheets[i];
            //     sb.Append(ExportSheet(excelName,sheet));
            // }
            sb.AppendLine("// End of Auto Generated Code");
            _mCode = sb.ToString();
        }

        private string ExportSheet(string excelName,DataTable sheet) {
            List<FieldDef> fieldList = new List<FieldDef>();
            DataRow typeRow = sheet.Rows[0];
            DataRow commentRow = sheet.Rows[1];

            foreach (DataColumn column in sheet.Columns)
            {
                FieldDef field;
                field.name = column.ToString();
                field.type = typeRow[column].ToString();
                field.comment = commentRow[column].ToString();

                fieldList.Add(field);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"namespace DataModel {");
            sb.AppendLine($"\tpublic class {excelName} {{");
            
            foreach (FieldDef field in fieldList) {
                sb.AppendLine($"\t\t/// <summary>");
                sb.AppendLine($"\t\t/// {field.comment}");
                sb.AppendLine($"\t\t/// </summary>");
                sb.AppendLine($"\t\tpublic {field.type} {field.name};");
                sb.AppendLine();
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }
        
        public void SaveToFile(string filePath, Encoding encoding)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, encoding))
                    writer.Write(_mCode);
            }
        }
    }
}