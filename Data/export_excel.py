# -*- coding: utf-8 -*-
import sys
import xlrd
import xml.dom.minidom
import os
#import codecs

def show_select_view(excel_path_dir,generate_xml_dir,generate_csharp_dir):
    files = os.listdir(excel_path_dir)
    lst_file_name = []
    for file in files:
        if not (file.endswith("xlsx") or file.endswith("xls")):
            continue
        try:
            data = xlrd.open_workbook(os.path.join(excel_path_dir,file), encoding_override="utf-8")
        except Exception as err:
            continue
        sheets = data.sheets()
        hasData = False
        for table in sheets:
            if table.nrows < 4:
                continue
            if table.ncols < 2:
                continue
            name_cell = table.cell(0, 0)
            if is_cell_null(name_cell):
                continue
            hasData = True
            break
        if hasData:
            lst_file_name.append(file)
    for i in range(len(lst_file_name)):
        print((i+1),lst_file_name[i]+":")

    repeat = True
    select_indexs = []
    while repeat:
        select_indexs.clear()
        try:
            str = input("请输入导出的表格(1,3或1-3):")
            str_arr = str.split(',')
            for one_str in str_arr:
                str_arr_arr = one_str.split('-')
                length = len(str_arr_arr)
                if length == 1:
                    select_indexs.append(int(str_arr_arr[0]))
                elif length == 2:
                    start = int(str_arr_arr[0])
                    end = int(str_arr_arr[1]) + 1
                    for index in range(start,end):
                        select_indexs.append(index)
            repeat = False
        except:
            repeat = True

    try:
        if len(select_indexs) > 0:
            for select_index in select_indexs:
                #print(select_index)
                selectI = select_index - 1
                if selectI < 0 or selectI >= len(lst_file_name):
                    continue
                select_file_name = lst_file_name[selectI]
                select_file_path = os.path.join(excel_path_dir,select_file_name)
                export_xls_to_xmls(select_file_path,generate_xml_dir,generate_csharp_dir)

        print("导出配置成功")
    except Exception as err:
        print(err)


#将excel导出到xml的接口
def export_xls_to_xmls(excel_absolute_path,generate_xml_dir,generate_csharp_dir):
    data = xlrd.open_workbook(excel_absolute_path, encoding_override="utf-8")
    sheets = data.sheets()
    sheet_names=data.sheet_names()
    for index in range(len(sheets)):
        export_table_to_xml(sheets[index], generate_xml_dir,absolute_path = excel_absolute_path,sheet_name=sheet_names[index])
        if generate_csharp_dir is None:
            generate_csharp_dir = generate_xml_dir;
        export_table_to_csharp(sheets[index], generate_csharp_dir,absolute_path = excel_absolute_path,sheet_name=sheet_names[index])

#将一个表单导出到xml接口
def export_table_to_xml(table,generate_xml_dir,absolute_path = None,sheet_name = None):
    if table.nrows < 4:
        return
    if table.ncols < 2:
        return
    name_cell = table.cell(0, 0)
    if is_cell_null(name_cell):
        return
    node_cell_value = str(name_cell.value).lower()

    doc = xml.dom.minidom.Document()
    parent = doc.createElement("root")
    doc.appendChild(parent)
    for row in range(4,table.nrows):
        node = doc.createElement(node_cell_value)
        count = 0
        for col in range(1,table.ncols):
            first_cell = table.cell(0,col)
            if is_cell_null(first_cell):
                continue

            type_cell = table.cell(1,col)
            if is_cell_null(type_cell):
                raise Exception("{0}导出配置的第{1}行第{2}列的type值不能为空".format(name_cell.value, 2, col + 1))
            key_cell = table.cell(2,col)
            if is_cell_null(key_cell):
                raise Exception("{0}导出配置的第{1}行第{2}列的key值不能为空".format(name_cell.value,3,col+1))
            value_cell = table.cell(row,col)
            attr_value = convert_cell_to_value(str(type_cell.value),str(first_cell.value),value_cell)
            node.setAttribute(str(key_cell.value),str(attr_value))
            #print(str(key_cell.value),attr_value)
            if not is_cell_null(value_cell):
                count += 1
        if count > 0:
            parent.appendChild(node)

    xml_name = name_cell.value + ".xml"
    generate_xml_path = os.path.join(generate_xml_dir,xml_name)
    #使用这种方式中文不会出问题
    #f = codecs.open(generate_xml_path,'w','utf-8')
    f = open(generate_xml_path,'w',encoding='utf-8')
    #f.write(doc.toprettyxml(encoding='utf-8'))
    f.write(str(doc.toprettyxml(encoding='utf-8'),encoding='utf-8'))
    #doc.writexml(f)
    f.close()
    print("导出配置{0}-{1}到{2}".format(absolute_path, sheet_name, generate_xml_path))

def is_cell_null(cell):
    if cell.value is None or cell.value == '':
        return True
    return False

def convert_cell_to_value(key,first_line,cell):
    func = dic_xml_value.get(key,None)
    if func is None:
        raise Exception("dic_xml_value = {0}的解析函数".format(key))
    return func(first_line,cell)

def convert_cell_to_int(first_line,cell):
    if first_line == "required" or first_line == "required_key":
        if is_cell_null(cell):
            return 0
        return int(cell.value)
    elif first_line == "repeated":
        if is_cell_null(cell):
            return ""
        return str(cell.value)
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))


def convert_cell_to_float(first_line,cell):
    if first_line == "required" or first_line == "required_key":
        if is_cell_null(cell):
            return 0
        f_value = float(cell.value)
        # 将float转为1+31+32的定点数表示
        return int((1 << 32) * f_value)
    elif first_line == "repeated":
        if is_cell_null(cell):
            return ""
        arr = str(cell.value).split(',')
        result_str = ""
        for index in range(len(arr)):
            if arr[index] is None or arr[index] == '':
                result_str += "0"+","
            else:
                result_str += str(int((1 << 32) * float(arr[index]))) + ",";
        if result_str.endswith(','):
            result_str = result_str[0:(len(result_str) - 1)]
        return result_str
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

def convert_cell_to_string(first_line,cell):
    if is_cell_null(cell):
        return ""
    return cell.value

def convert_cell_to_bool(first_line,cell):
	if is_cell_null(cell):
		return "FALSE"
	if str(cell.value) == "0":
		return "FALSE"
	else:
		return "TRUE"
	return str(cell.value)

dic_xml_value = {
    "int": convert_cell_to_int,
    "float":convert_cell_to_float,
    "string":convert_cell_to_string,
    "bool":convert_cell_to_bool,
}

def export_table_to_csharp(table,generate_csharp_dir,absolute_path = None,sheet_name = None):
    if table.nrows < 4:
        return
    if table.ncols < 2:
        return
    name_cell = table.cell(0, 0)
    if is_cell_null(name_cell):
        return
    csharp_name = name_cell.value + ".cs"
    generate_csharp_path = os.path.join(generate_csharp_dir, csharp_name)
    f = open(generate_csharp_path, 'w')
    f.write("using System;\n")
    f.write("using System.Security;\n")
    f.write("using Framework;\n")
    f.write("using System.Collections.Generic;\n")
    f.write("namespace GameData\n")
    f.write("{\n")
    f.write("\t[ResCfg]\n")
    f.write("\tpublic class {0}\n".format(name_cell.value))
    f.write("\t{\n")
    for col in range(1, table.ncols):
        first_cell = table.cell(0, col)
        if is_cell_null(first_cell):
            continue
        type_cell = table.cell(1, col)
        if is_cell_null(type_cell):
            raise Exception("{0}导出配置的第{1}行第{2}列的type值不能为空".format(name_cell.value, 2, col + 1))
        key_cell = table.cell(2, col)
        if is_cell_null(key_cell):
            raise Exception("{0}导出配置的第{1}行第{2}列的key值不能为空".format(name_cell.value, 3, col + 1))
        temp_str = csharp_convert_key_to_string(str(first_cell.value),str(type_cell.value),str(key_cell.value))
        f.write("\t\t"+temp_str)
        f.write("\n")
    f.write("\t\tpublic {0}(SecurityElement node)\n".format(name_cell.value))
    f.write("\t\t{\n")
    for col in range(1, table.ncols):
        first_cell = table.cell(0, col)
        if is_cell_null(first_cell):
            continue
        type_cell = table.cell(1, col)
        if is_cell_null(type_cell):
            raise Exception("{0}导出配置的第{1}行第{2}列的type值不能为空".format(name_cell.value, 2, col + 1))
        key_cell = table.cell(2, col)
        if is_cell_null(key_cell):
            raise Exception("{0}导出配置的第{1}行第{2}列的key值不能为空".format(name_cell.value, 3, col + 1))
        temp_str = csharp_parse_key_to_string(str(first_cell.value),type_cell.value, key_cell.value)
        f.write(temp_str)
        f.write("\n")
    f.write("\t\t}\n")
    f.write("\t}\n")
    f.write("}")
    f.close()
    print("导出配置{0}-{1}到{2}".format(absolute_path, sheet_name, generate_csharp_path))

def csharp_parse_key_to_string(first_line,type,key):
    func = dic_csharp_parse_value.get(type,None)
    if func is None:
        raise Exception("csharp_dic_property_value找不到key={0}的解析函数".format(key))
    return func(first_line,key)

def csharp_parse_int_property(first_line,key):
    if first_line == "required" or first_line == "required_key":
        return "\t\t\t{0} = int.Parse(node.Attribute(\"{1}\"));".format(key,key)
    elif first_line == "repeated":
        temp_str = "\t\t\t{0} = new List<int>();\n".format(key)
        temp_str += "\t\t\tstring str_{0} = node.Attribute(\"{1}\");\n".format(key,key)
        temp_str += "\t\t\tif(!string.IsNullOrEmpty(str_{0}))\n".format(key)
        temp_str += "\t\t\t{\n";
        temp_str += "\t\t\t\tstring[] {0}Arr = str_{1}.Split(\',\');\n".format(key,key)
        temp_str += "\t\t\t\tif ({0}Arr != null || {1}Arr.Length > 0)\n".format(key,key)
        temp_str += "\t\t\t\t{\n"
        temp_str += "\t\t\t\t\tfor (int i = 0; i < {0}Arr.Length; i++)\n".format(key)
        temp_str += "\t\t\t\t\t{\n"
        temp_str += "\t\t\t\t\t\t{0}.Add(int.Parse({1}Arr[i]));\n".format(key,key)
        temp_str += "\t\t\t\t\t}\n"
        temp_str += "\t\t\t\t}\n"
        temp_str += "\t\t\t}"
        return temp_str
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))


def csharp_parse_float_property(first_line,key):
    if first_line == "required" or first_line == "required_key":
        return "\t\t\t{0} = FP.FromSourceLong(long.Parse(node.Attribute(\"{1}\")));".format(key, key)
    elif first_line == "repeated":
        temp_str = "\t\t\t{0} = new List<FP>();\n".format(key)
        temp_str += "\t\t\tstring str_{0} = node.Attribute(\"{1}\");\n".format(key,key)
        temp_str += "\t\t\tif(!string.IsNullOrEmpty(str_{0}))\n".format(key)
        temp_str += "\t\t\t{\n";
        temp_str += "\t\t\t\tstring[] {0}Arr = str_{1}.Split(\',\');\n".format(key,key)
        temp_str += "\t\t\t\tif ({0}Arr != null || {1}Arr.Length > 0)\n".format(key,key)
        temp_str += "\t\t\t\t{\n"
        temp_str += "\t\t\t\t\tfor (int i = 0; i < {0}Arr.Length; i++)\n".format(key)
        temp_str += "\t\t\t\t\t{\n"
        temp_str += "\t\t\t\t\t\t{0}.Add(FP.FromSourceLong(long.Parse({1}Arr[i])));\n".format(key,key)
        temp_str += "\t\t\t\t\t}\n"
        temp_str += "\t\t\t\t}\n"
        temp_str += "\t\t\t}"
        return temp_str
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

def csharp_parse_string_property(first_line,key):
    if first_line == "required" or first_line == "required_key":
        return "\t\t\t{0} = node.Attribute(\"{1}\");".format(key, key)
    elif first_line == "repeated":
        temp_str = "\t\t\t{0} = new List<string>();\n".format(key)
        temp_str += "\t\t\tstring str_{0} = node.Attribute(\"{1}\");\n".format(key,key)
        temp_str += "\t\t\tif(!string.IsNullOrEmpty(str_{0}))\n".format(key)
        temp_str += "\t\t\t{\n";
        temp_str += "\t\t\t\tstring[] {0}Arr = str_{1}.Split(\',\');\n".format(key,key)
        temp_str += "\t\t\t\tif ({0}Arr != null || {1}Arr.Length > 0)\n".format(key,key)
        temp_str += "\t\t\t\t{\n"
        temp_str += "\t\t\t\t\tfor (int i = 0; i < {0}Arr.Length; i++)\n".format(key)
        temp_str += "\t\t\t\t\t{\n"
        temp_str += "\t\t\t\t\t\t{0}.Add({1}Arr[i]);\n".format(key,key)
        temp_str += "\t\t\t\t\t}\n"
        temp_str += "\t\t\t\t}\n"
        temp_str += "\t\t\t}"
        return temp_str
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

def csharp_parse_bool_property(first_line,key):
    if first_line == "required" or first_line == "required_key":
        return "\t\t\t{0} = bool.Parse(node.Attribute(\"{1}\"));".format(key, key)
    elif first_line == "repeated":
        temp_str = "\t\t\t{0} = new List<bool>();\n".format(key)
        temp_str += "\t\t\tstring str_{0} = node.Attribute(\"{1}\");\n".format(key,key)
        temp_str += "\t\t\tif(!string.IsNullOrEmpty(str_{0}))\n".format(key)
        temp_str += "\t\t\t{\n";
        temp_str += "\t\t\t\tstring[] {0}Arr = str_{1}.Split(\',\');\n".format(key,key)
        temp_str += "\t\t\t\tif ({0}Arr != null || {1}Arr.Length > 0)\n".format(key,key)
        temp_str += "\t\t\t\t{\n"
        temp_str += "\t\t\t\t\tfor (int i = 0; i < {0}Arr.Length; i++)\n".format(key)
        temp_str += "\t\t\t\t\t{\n"
        temp_str += "\t\t\t\t\t\t{0}.Add(bool.Parse({1}Arr[i]));\n".format(key,key)
        temp_str += "\t\t\t\t\t}\n"
        temp_str += "\t\t\t\t}\n"
        temp_str += "\t\t\t}"
        return temp_str
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

dic_csharp_parse_value = {
    "int": csharp_parse_int_property,
    "float":csharp_parse_float_property,
    "string":csharp_parse_string_property,
    "bool":csharp_parse_bool_property,
}

def csharp_convert_key_to_string(first_line,type,key):
    func = dic_csharp_parse_key.get(type,None)
    if func is None:
        raise Exception("csharp_dic_parse_property_value找不到key={0}的解析函数".format(key))
    return func(key,first_line)

def csharp_int_property(key,first_line):
    if first_line == "required":
        return "public int "+key+" { get; private set; }"
    elif first_line == "required_key":
        return "[ResCfgKey]\n\t\tpublic int "+key+" { get; private set; }"
    elif first_line == "repeated":
        return "public List<int> "+key+" { get; private set; }"
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))
def csharp_float_property(key,first_line):
    if first_line == "required":
        return "public FP "+key+" { get; private set; }"
    elif first_line == "required_key":
        return "[ResCfgKey]\n\t\tpublic FP "+key+" { get; private set; }"
    elif first_line == "repeated":
        return "public List<FP> " + key + " { get; private set; }"
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

def csharp_string_property(key,first_line):
    if first_line == "required":
        return "public string "+key+" { get; private set; }"
    elif first_line == "required_key":
        return "[ResCfgKey]\n\t\tpublic string "+key+" { get; private set; }"
    elif first_line == "repeated":
        return "public List<string> " + key + " { get; private set; }"
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

def csharp_bool_property(key,first_line):
    if first_line == "required":
        return "public bool "+key+" { get; private set; }"
    elif first_line == "required_key":
        return "[ResCfgKey]\n\t\tpublic bool "+key+" { get; private set; }"
    elif first_line == "repeated":
        return "public List<bool> " + key + " { get; private set; }"
    raise Exception("first_line = {0}的字符串编写错误".format(first_line))

dic_csharp_parse_key = {
    "int": csharp_int_property,
    "float":csharp_float_property,
    "string":csharp_string_property,
    "bool":csharp_bool_property,
}

if __name__ == "__main__":
    if len(sys.argv) > 2:
        excel_dir = str(sys.argv[1])
        xml_dir = str(sys.argv[2])
        csharp_dir = None
        if len(sys.argv) > 3:
            csharp_dir = str(sys.argv[3])
        show_select_view(excel_dir,xml_dir,csharp_dir)

    #show_select_view("D:/OtherWorkspace/FrameSync/tool/ExportExcelToXml/res","D:/OtherWorkspace/FrameSync/tool/ExportExcelToXml/res")
    #export_xls_to_xmls("D:/PythonWorkspace/ExportExcelToXml/res/测试表格.xlsx","D:/PythonWorkspace/ExportExcelToXml/res");