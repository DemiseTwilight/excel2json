using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Excel2JsonConfig",menuName = "excel2json/CreateConfig",order = 0)]
public class Excel2JsonConfig : ScriptableObject {
    public string excelPath;
    public string outputPath;
    public string dataModelPath;

    [Header("表头")]
    public int header;
    [Header("指定编码")]
    public Encoding encoding;
    [Header("自动将字段名转化为小写")]
    public bool lowcase=false;
    [Header("序列化为数组")]
    public bool exportArray=false;
    [Header("指定日期格式")]
    public string dateFormat = "yyyy/MM/dd";
    [Header("序列化时强制带上sheetName")]
    public bool forceSheetName = false;
    [Header("排除指定前缀的表单和列")]
    public string exclude;
    [Header("自动识别单元格中的Json对象和Json数组")]
    public bool cellJson = false;
    [Header("全部转换为string")]
    //方便LitJson.JsonMapper.ToObject<List<Dictionary<string, string>>>(textAsset.text)等使用方式 之后根据自己的需求进行解析
    public bool allString = false;
}
