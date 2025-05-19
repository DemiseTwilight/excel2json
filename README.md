# excel2jsonForUnity
这是一个基于excel2json的Unity版本，去掉了命令行并整合了依赖项
excel2json C#版以及GUI版见：https://github.com/neil3d/excel2json.git

详细帮助文档请见：
[https://neil3d.github.io/coding/excel2json.html](https://neil3d.github.io/coding/excel2json.html)

## 通过配置文件设置
可通过配置文件Excel2JsonConfig设置路径以及可选参数

## 依赖
Libs文件下附带了全部依赖，如果想使用自己的json解析器请删除Newtonsoft.Json.dll并修改部分代码。