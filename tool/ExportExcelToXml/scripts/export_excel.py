import xlrd
import xml.dom.minidom
import os
import sys

def show_select_view(excel_path_dir,generate_xml_dir):
    files = os.listdir(excel_path_dir)
    lst_file_name = []
    for file in files:
        if not (file.endswith("xlsx") or file.endswith("xls")):
            continue
        try:
            data = xlrd.open_workbook(os.path.join(excel_path_dir,file), encoding_override="utf-8")
        except:
            continue
        sheets = data.sheets()
        hasData = False
        for table in sheets:
            if table.nrows < 3:
                continue
            if table.ncols < 2:
                continue
            name_cell = table.cell(0, 0)
            if is_cell_null(name_cell):
                continue
            node_cell = table.cell(1, 0)
            if is_cell_null(node_cell):
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
                export_xls_to_xmls(select_file_path,generate_xml_dir)

        print("导出配置成功")
    except:
        print("导出配置失败")


#将excel导出到xml的接口
def export_xls_to_xmls(excel_absolute_path,generate_xml_dir):
    data = xlrd.open_workbook(excel_absolute_path, encoding_override="utf-8")
    sheets = data.sheets()
    sheet_names=data.sheet_names()
    for index in range(len(sheets)):
        export_table_to_xml(sheets[index], generate_xml_dir,absolute_path = excel_absolute_path,sheet_name=sheet_names[index])

def export_table_to_xml(table,generate_xml_dir,absolute_path = None,sheet_name = None):
    if table.nrows < 3:
        return
    if table.ncols < 2:
        return
    name_cell = table.cell(0, 0)
    if is_cell_null(name_cell):
        return
    node_cell = table.cell(1,0)
    if is_cell_null(node_cell):
        return

    doc = xml.dom.minidom.Document()
    parent = doc.createElement("root")
    doc.appendChild(parent)
    for row in range(3,table.nrows):
        node = doc.createElement(node_cell.value)
        count = 0
        for col in range(1,table.ncols):
            type_cell = table.cell(0,col)
            if is_cell_null(type_cell):
                continue
            key_cell = table.cell(1,col)
            if is_cell_null(key_cell):
                raise Exception("{0}导出配置的第{1}行第{2}列的key值不能为空".format(name_cell.value,2,col+1))
            value_cell = table.cell(row,col)
            attr_value = convert_cell_to_value(str(type_cell.value),value_cell)
            node.setAttribute(str(key_cell.value),str(attr_value))
            #print(str(key_cell.value),attr_value)
            if not is_cell_null(value_cell):
                count += 1
        if count > 0:
            parent.appendChild(node)

    xml_name = name_cell.value + ".xml"
    generate_xml_path = os.path.join(generate_xml_dir,xml_name)
    f = open(generate_xml_path,'w')
    f.write(doc.toprettyxml())
    #doc.writexml(f)
    f.close()
    print("导出配置{0}-{1}到{2}".format(absolute_path, sheet_name, generate_xml_path))

def is_cell_null(cell):
    if cell.value is None or cell.value == '':
        return True
    return False

def convert_cell_to_value(key,cell):
    func = dic_value.get(key,None)
    if func is None:
        raise Exception("找不到类型为{0}的解析函数".format(key))
    return func(cell)

def convert_cell_to_int(cell):
    if is_cell_null(cell):
        return 0
    return int(cell.value)

def convert_cell_to_float(cell):
    if is_cell_null(cell):
        return 0
    f_value = float(cell.value)
    #将float转为1+31+32的定点数表示
    return int((1 << 32) * f_value)

def convert_cell_to_string(cell):
    if is_cell_null(cell):
        return ""
    return str(cell.value)

dic_value = {
    "int": convert_cell_to_int,
    "float":convert_cell_to_float,
    "string":convert_cell_to_string,
}

if __name__ == "__main__":
    if len(sys.argv) > 2:
        excel_dir = str(sys.argv[1])
        xml_dir = str(sys.argv[2])
        show_select_view(excel_dir,xml_dir)
    #export_xls_to_xmls("D:/PythonWorkspace/ExportExcelToXml/res/测试表格.xlsx","D:/PythonWorkspace/ExportExcelToXml/res");