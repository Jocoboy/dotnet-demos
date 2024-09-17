using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Attributes;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions.Excel
{
    public class ExcelHelper
    {
        #region 字段
        /// <summary>
        /// 行数据读取错误提示
        /// </summary>
        private readonly static string _errorTipForReadRow = "第{0}行数据有误，请检查";
        #endregion

        #region 公用
        /// <summary>
        /// 获取单元格值
        /// </summary>
        /// <param name="sheet">工作表</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns></returns>
        private static string GetCellValue(ISheet sheet, int row, int col)
        {
            return sheet.GetRow(row)?.GetCell(col)?.ToString()?.Trim();
        }

        /// <summary>
        /// 获取或创建行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rownum"></param>
        /// <returns></returns>
        private static IRow GetOrCreateRow(ISheet sheet, int rownum)
        {
            IRow row = sheet.GetRow(rownum);
            if (row == null)
            {
                row = sheet.CreateRow(rownum);
            }
            return row;
        }

        /// <summary>
        /// 获取或创建单元格
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colnum"></param>
        /// <returns></returns>
        private static ICell GetOrCreateCell(IRow row, int colnum)
        {
            ICell cell = row.GetCell(colnum);
            if (cell == null)
            {
                cell = row.CreateCell(colnum);
            }
            return cell;
        }

        /// <summary>
        /// 数据切割
        /// </summary>
        /// <typeparam name="T">要切割的集合的数据类型</typeparam>
        /// <param name="list">要切割的数据</param>
        /// <param name="splitCount">切割后每份数据条数</param>
        /// <returns></returns>
        private static List<List<T>> DataSplit<T>(List<T> list, int splitCount)
        {
            List<List<T>> splitDataList = new List<List<T>>();
            if (list.Count <= splitCount)
            {
                splitDataList.Add(list);
            }
            else
            {
                T[] split = null;
                int quotient = Math.DivRem(list.Count, splitCount, out int mod);
                for (int i = 0; i < quotient; i++)
                {
                    split = new T[splitCount];
                    for (int j = 0; j < splitCount; j++)
                    {
                        split[j] = list[splitCount * i + j];
                    }
                    splitDataList.Add(split.ToList());
                }
                if (mod > 0)
                {
                    split = new T[mod];
                    for (int i = 0; i < mod; i++)
                    {
                        split[i] = list[splitCount * quotient + i];
                    }
                    splitDataList.Add(split.ToList());
                }
            }
            return splitDataList;
        }

        /// <summary>
        /// 校验是否为 Excel 2003 xls
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static bool ValidateIsExcel2003(Stream stream)
        {
            return ValidateBinaryFileHeader(stream, new byte[] { 0xD0, 0xCF, 0x11 });
        }

        /// <summary>
        /// 校验是否为 Excel 2007 xlsx
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static bool ValidateIsExcel2007(Stream stream)
        {
            return ValidateBinaryFileHeader(stream, new byte[] { 0x50, 0x4B, 0x03 });
        }

        /// <summary>
        /// 校验二进制文件头标识
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private static bool ValidateBinaryFileHeader(Stream stream, byte[] header)
        {
            using (Stream validStream = new MemoryStream())
            {
                // 防止读取校验之后，流关闭，先复制，再校验
                stream.CopyTo(validStream);
                stream.Position = 0;
                validStream.Position = 0;

                using (BinaryReader reader = new BinaryReader(validStream))
                {
                    foreach (var item in header)
                    {
                        try
                        {
                            if (item != reader.ReadByte())
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 判断是否为可空类型，例如: int? bool?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        #endregion

        #region 读取 Excel
        /// <summary>
        /// 读取 Excel 数据，转为实体对象集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="excelPath">Excel 文件路径</param>
        /// <returns></returns>
        public static List<T> ReadExcelToEntity<T>(string excelPath)
        {
            return ReadExcelToEntity<T>(excelPath, 0, 0, 0);
        }

        /// <summary>
        /// 读取 Excel 数据，转为实体对象集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="excelPath">Excel 文件路径</param>
        /// <param name="sheetIndex">Excel 工作表索引</param>
        /// <param name="startRow">数据起始行 (标题)</param>
        /// <param name="startCol">数据起始列</param>
        /// <returns></returns>
        public static List<T> ReadExcelToEntity<T>(string excelPath, int sheetIndex, int startRow, int startCol)
        {
            using (Stream excelStream = File.OpenRead(excelPath))
            {
                return ReadExcelToEntity<T>(excelStream, sheetIndex, startRow, startCol);
            }
        }

        /// <summary>
        /// 读取 Excel 数据，转为实体对象集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="excelStream">Excel 文件流</param>
        /// <returns></returns>
        public static List<T> ReadExcelToEntity<T>(Stream excelStream)
        {
            return ReadExcelToEntity<T>(excelStream, 0, 0, 0);
        }

        /// <summary>
        /// 读取 Excel 数据，转为实体对象集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="excelStream">Excel 文件流</param>
        /// <param name="sheetIndex">Excel 工作表索引</param>
        /// <param name="startRow">数据起始行 (标题)</param>
        /// <param name="startCol">数据起始列</param>
        /// <returns></returns>
        public static List<T> ReadExcelToEntity<T>(Stream excelStream, int sheetIndex, int startRow, int startCol)
        {
            List<T> result = new List<T>();

            if (excelStream.Length <= 0)
            {
                return result;
            }

            // 读取工作簿中的指定工作表
            IWorkbook workbook = null;
            if (ValidateIsExcel2003(excelStream))
            {
                workbook = new HSSFWorkbook(excelStream);
            }
            else if (ValidateIsExcel2007(excelStream))
            {
                workbook = new XSSFWorkbook(excelStream);
            }
            else
            {
                return result;
            }
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            // 获取实体类属性与特性
            PropertyInfo[] props = typeof(T).GetProperties();
            ExcelAttribute[] excelAttrs = new ExcelAttribute[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                var excelAttr = props[i].GetCustomAttribute<ExcelAttribute>(true);
                excelAttrs[i] = excelAttr;
            }

            // 忽略项筛选
            List<PropertyInfo> propList = new List<PropertyInfo>();
            List<ExcelAttribute> excelAttrList = new List<ExcelAttribute>();
            for (int i = 0; i < props.Length; i++)
            {
                if (excelAttrs[i]?.IsIgnore == true)
                {
                    continue;
                }
                propList.Add(props[i]);
                excelAttrList.Add(excelAttrs[i]);
            }
            props = propList.ToArray();
            excelAttrs = excelAttrList.ToArray();

            // 检查标题。起始行必须是标题（对象属性名称或特性中定义的标题名称），且顺序一致，否则直接返回空数据
            for (int col = 0; col < props.Length; col++)
            {
                var title = GetCellValue(sheet, startRow, col + startCol);
                if (!string.Equals(title, props[col].Name, StringComparison.OrdinalIgnoreCase))
                {
                    // 属性名称匹配不上，则尝试匹配特性标题名称
                    if (!string.IsNullOrWhiteSpace(excelAttrs[col]?.Title))
                    {
                        if (!string.Equals(title, excelAttrs[col].Title, StringComparison.OrdinalIgnoreCase))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
            }

            // 读取数据
            for (int r = startRow + 1; r <= sheet.LastRowNum; r++)
            {
                T entity = (T)Activator.CreateInstance(typeof(T));
                bool rowEmpty = true;
                bool rowHasRequiredFieldNull = false;

                for (int col = 0; col < props.Length; col++)
                {
                    Type propType = props[col].PropertyType;
                    object value = GetCellValue(sheet, r, col + startCol);
                    if (!string.IsNullOrWhiteSpace(value?.ToString()))
                    {
                        rowEmpty = false;

                        // 类型转换
                        if (value.GetType() != propType)
                        {
                            try
                            {
                                if (IsNullableType(propType))
                                {
                                    propType = Nullable.GetUnderlyingType(propType);
                                }
                                value = Convert.ChangeType(value, propType);
                            }
                            catch
                            {
                                throw new CustomException(string.Format(_errorTipForReadRow, r + 1));
                            }
                        }

                        // 设置属性值
                        props[col].SetValue(entity, value);
                    }

                    if (string.IsNullOrWhiteSpace(value?.ToString()) && propType.IsValueType && !IsNullableType(propType))
                    {
                        rowHasRequiredFieldNull = true;
                    }
                }

                // 忽略空行
                if (rowEmpty)
                {
                    continue;
                }

                // 必填字段无对应数据
                if (rowHasRequiredFieldNull)
                {
                    throw new CustomException(string.Format(_errorTipForReadRow, r + 1));
                }
                result.Add(entity);
            }

            return result;
        }
        #endregion

        #region 导出 Excel
        /// <summary>
        /// 导出 Excel
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(List<T> entities)
        {
            return OutputExcel(
                entities: entities,
                selectedFields: null);
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <param name="selectedFields">自定义导出字段</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(List<T> entities, string[] selectedFields)
        {
            return OutputExcel(
                entities: entities,
                titles: null,
                selectedFields: selectedFields);
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <param name="titles">标题</param>
        /// <param name="selectedFields">自定义导出字段</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(List<T> entities, string[] titles, string[] selectedFields)
        {
            return OutputExcel(
                entities: entities,
                template: null,
                sheetIndex: 0,
                startRow: 0,
                startCol: 0,
                isGenerateTitle: true,
                titles: titles,
                selectedFields: selectedFields);
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        /// <param name="template">模板文件路径</param>
        /// <param name="sheetIndex">模板工作表索引</param>
        /// <param name="startRow">数据起始行</param>
        /// <param name="startCol">数据起始列</param>
        /// <param name="isGenerateTitle">是否生成标题</param>
        /// <param name="titles">标题</param>
        /// <param name="selectedFields">自定义导出字段</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(List<T> entities, string template, int sheetIndex, int startRow, int startCol, bool isGenerateTitle, string[] titles, string[] selectedFields)
        {
            // 获取实体类属性与特性
            PropertyInfo[] props = typeof(T).GetProperties();
            ExcelAttribute[] excelAttrs = new ExcelAttribute[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                var excelAttr = props[i].GetCustomAttribute<ExcelAttribute>(true);
                excelAttrs[i] = excelAttr;
            }

            // 全空字段统计
            bool?[] allNullArr = new bool?[props.Length];
            for (int i = 0; i < excelAttrs.Length; i++)
            {
                if (excelAttrs[i]?.IfAllNullIgnore == true)
                {
                    bool isAllNull = !entities.Any(e => props[i].GetValue(e) != null);
                    allNullArr[i] = isAllNull;
                }
            }

            #region 忽略项筛选
            {
                List<PropertyInfo> propList = new List<PropertyInfo>();
                List<ExcelAttribute> excelAttrList = new List<ExcelAttribute>();
                List<string> titleList = new List<string>();

                for (int i = 0; i < props.Length; i++)
                {
                    if (excelAttrs[i]?.IsIgnore == true)
                    {
                        continue;
                    }

                    if (allNullArr[i] == true)
                    {
                        continue;
                    }

                    // 有设置自定义导出字段时，排除不导出的字段
                    if (selectedFields != null && selectedFields.Length > 0
                        && !selectedFields.Any(f => string.Equals(f, props[i].Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    // 通过筛选的数据放在临时变量中
                    propList.Add(props[i]);
                    excelAttrList.Add(excelAttrs[i]);
                    if (titles != null && titles.Length > i)
                    {
                        titleList.Add(titles[i]);
                    }
                }

                // 筛选结果赋值
                props = propList.ToArray();
                excelAttrs = excelAttrList.ToArray();
                if (titles != null)
                {
                    titles = titleList.ToArray();
                }
            }
            #endregion

            // 变量申明
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            T entity = default;
            PropertyInfo prop = null;
            ExcelAttribute attr = null;
            string value = null;

            List<List<T>> sheetDataList = new List<List<T>>();
            #region 数据切割
            {
                /*
                 * Excel 最大行数，不能超过 65,536 即 2 的 16 次方
                 * 设置切割值，这里默认 5 万条数据一页，超过 5 万条，分多个工作表导出
                 * 
                 */

                int rowsPerSheet = 50000;
                sheetDataList = DataSplit(entities, rowsPerSheet);
            }
            #endregion

            #region 创建工作表
            {
                // 模板
                if (!string.IsNullOrWhiteSpace(template))
                {
                    sheet = GetTemplateSheet(template, sheetIndex);
                }
                else
                {
                    sheet = new HSSFWorkbook().CreateSheet();
                }

                // 多工作表命名
                sheet.Workbook.SetSheetName(0, "sheet1");
                if (sheetDataList.Count > 1)
                {
                    for (int i = 1; i < sheetDataList.Count; i++)
                    {
                        sheet.CopySheet("sheet" + (i + 1));
                    }
                }
            }
            #endregion

            for (int i = 0; i < sheetDataList.Count; i++)
            {
                sheet = sheet.Workbook.GetSheetAt(i);

                #region 生成标题
                {
                    // 生成标题
                    if (isGenerateTitle)
                    {
                        row = GetOrCreateRow(sheet, startRow);
                        for (int col = 0; col < props.Length; col++)
                        {
                            prop = props[col];
                            attr = excelAttrs[col];

                            cell = GetOrCreateCell(row, col + startCol);

                            // 标题取值 优先级，titles参数，特性标题，属性名
                            value = prop.Name;
                            if (titles != null && titles.Length > col && titles[col] != null)
                            {
                                value = titles[col];
                            }
                            else if (attr != null && attr.Title != null)
                            {
                                value = attr.Title;
                            }
                            cell.SetCellValue(value);
                        }
                    }
                }
                #endregion

                #region 生成数据
                {
                    // 遍历行
                    var list = sheetDataList[i];
                    for (int r = 0; r < list.Count; r++)
                    {
                        row = GetOrCreateRow(sheet, r + startRow + (isGenerateTitle ? 1 : 0));
                        entity = list[r];

                        // 遍历列
                        for (int col = 0; col < props.Length; col++)
                        {
                            prop = props[col];
                            attr = excelAttrs[col];

                            cell = GetOrCreateCell(row, col + startCol);
                            value = prop.GetValue(entity)?.ToString();
                            if (prop.PropertyType == typeof(DateTime) && attr != null && !string.IsNullOrWhiteSpace(attr.TimeFormat))
                            {
                                value = Convert.ToDateTime(value).ToString(attr.TimeFormat);
                            }
                            cell.SetCellValue(value);
                        }
                    }
                }
                #endregion
            }

            // 文件流转化
            byte[] buffer = null;
            using (MemoryStream ms = new MemoryStream())
            {
                sheet.Workbook.Write(ms);
                buffer = ms.GetBuffer();
                ms.Flush();
            }
            return buffer;
        }

        /// <summary>
        /// 获取模板工作表
        /// </summary>
        /// <param name="template">模板文件路径</param>
        /// <param name="sheetIndex">模板工作表索引</param>
        /// <returns></returns>
        public static ISheet GetTemplateSheet(string template, int sheetIndex)
        {
            IWorkbook workbook = null;
            using (Stream excelStream = File.OpenRead(template))
            {
                workbook = new HSSFWorkbook(excelStream);
            }
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            // 删除多余工作表
            int sheetNum = workbook.NumberOfSheets;
            for (int i = sheetNum - 1; i >= 0; i--)
            {
                if (i != sheetIndex)
                {
                    workbook.RemoveSheetAt(i);
                }
            }
            return sheet;
        }
        #endregion

    }
}
