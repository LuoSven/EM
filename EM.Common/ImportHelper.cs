using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common
{
    /// <summary>文件导入工具类
    /// 
    /// </summary>
    public class Import : IDisposable
    {
        #region Private Propties
        /// <summary>
        /// 服务端文件全路径
        /// </summary>
        private String strServerFileName = "";
        /// <summary>
        /// 导入文件的后缀名
        /// </summary>
        private String strFileExtend = "";
        /// <summary>
        /// 上传导入文件
        /// </summary>
        private System.Web.UI.HtmlControls.HtmlInputFile htmlInputFile;
        #endregion

        /// <summary>
        /// 上传文件，返回DataTable
        /// </summary>
        /// <param name="inputfile">上传控件</param>
        /// <param name="dt">返回的数据集</param>
        public void ImportData(System.Web.UI.HtmlControls.HtmlInputFile inputfile, ref DataTable dt)
        {
            htmlInputFile = inputfile;
            //得到扩展名
            strFileExtend = inputfile.Value.Substring(htmlInputFile.Value.LastIndexOf(".") + 1);
            //获取导入文件类型
            switch (strFileExtend.Trim().ToUpper())
            {
                case "XLSX":
                case "XLS":
                    strServerFileName = SaveServerFile();
                    dt = TransformXlsToDataTable();
                    break;
                case "CSV":
                case "TXT":
                    strServerFileName = SaveServerFile();
                    dt = TransformCSVToDataTable();
                    break;
                case "":
                    throw new Exception("请选择导入文件!");
                default:
                    throw new Exception("导入的数据类型不正确!");
            }
        }

        #region Excel文件上传
        /// <summary>
        /// 保存上传文件至服务器
        /// 并返回服务器端文件路径
        /// </summary>
        /// <param name="inputfile">上传控件</param>
        /// <returns>服务器端文件路径</returns>
        private String SaveServerFile()
        {
            string orifilename = string.Empty;
            string uploadfilepath = string.Empty;
            string modifyfilename = string.Empty;
            int fileSize = 0;//文件大小
            try
            {
                if (htmlInputFile.Value != string.Empty)
                {
                    //得到文件的大小
                    fileSize = htmlInputFile.PostedFile.ContentLength;
                    if (fileSize == 0)
                    {
                        throw new Exception("找不到该文件！");
                    }
                    //路径
                    uploadfilepath = System.Web.HttpContext.Current.Server.MapPath(".") + "\\uploadtemp\\";
                    //新文件名
                    modifyfilename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()
                        + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()
                        + DateTime.Now.Millisecond.ToString();
                    modifyfilename += "." + strFileExtend;
                    //判断是否有该目录
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(uploadfilepath);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    orifilename = uploadfilepath + modifyfilename;
                    //如果存在,删除文件
                    if (File.Exists(orifilename))
                    {
                        File.Delete(orifilename);
                    }
                    // 上传文件
                    htmlInputFile.PostedFile.SaveAs(orifilename);
                }
                else
                {
                    throw new Exception("没有选择文件!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return orifilename;
        }

        /// <summary>
        /// 从Excel提取数据--》Dataset
        /// </summary>
        private DataTable TransformXlsToDataTable()
        {
            if (strServerFileName == string.Empty)
            {
                throw new ArgumentNullException("上传文件失败！");
            }
            //
            StringBuilder oleDBConnString = new StringBuilder();
            oleDBConnString.Append("Provider=Microsoft.ACE.OLEDB.12.0;");
            oleDBConnString.Append("Data Source=");
            oleDBConnString.Append(strServerFileName);
            oleDBConnString.Append(";Extended Properties=Excel 12.0;");
            OleDbConnection oleDBConn = null;
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter oleAdMaster = null;
                DataTable m_tableName = new DataTable();

                oleDBConn = new OleDbConnection(oleDBConnString.ToString());
                oleDBConn.Open();
                m_tableName = oleDBConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (m_tableName != null && m_tableName.Rows.Count > 0)
                {
                    m_tableName.TableName = m_tableName.Rows[0]["TABLE_NAME"].ToString();
                }
                string sqlMaster;
                sqlMaster = " SELECT *  FROM [" + m_tableName.TableName + "]";
                oleAdMaster = new OleDbDataAdapter(sqlMaster, oleDBConn);
                oleAdMaster.Fill(ds, "m_tableName");
                oleAdMaster.Dispose();
                oleDBConn.Close();
                oleDBConn.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oleDBConn.State == ConnectionState.Open)
                {
                    oleDBConn.Close();
                }
                //删除上传的文件
                if (File.Exists(this.strServerFileName))
                    File.Delete(this.strServerFileName);
            }
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }
        #endregion

        #region CSV/TXT文件上传
        /// <summary>
        /// Get dataset from csv file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <returns>Data Set</returns>
        private DataTable TransformCSVToDataTable()
        {
            String strfilepath = this.strServerFileName.Substring(0, strServerFileName.LastIndexOf("\\") + 1);
            String strfilename = this.strServerFileName.Substring(strServerFileName.LastIndexOf("\\") + 1);
            string strconn = @"driver={microsoft text driver (*.txt; *.csv)};dbq=";
            strconn += strfilepath;                                                        //filepath, for example: c:\
            strconn += ";extensions=asc,csv,tab,txt;";
            OdbcConnection objconn = new OdbcConnection(strconn);
            DataSet dscsv = new DataSet();
            try
            {
                string strsql = "select * from " + strfilename;                     //filename, for example: 1.csv
                OdbcDataAdapter odbccsvdataadapter = new OdbcDataAdapter(strsql, objconn);

                odbccsvdataadapter.Fill(dscsv);
                return dscsv.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objconn.State == ConnectionState.Open)
                {
                    objconn.Close();
                }
                objconn.Dispose();
                if (File.Exists(this.strServerFileName))
                    File.Delete(this.strServerFileName);
            }
        }

        private String[] TransformCSVToString()
        {
            String[] data;
            try
            {
                data = File.ReadAllLines(this.strServerFileName);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (File.Exists(this.strServerFileName))
                    File.Delete(this.strServerFileName);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
