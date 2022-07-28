using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace WSSaityCore.Clases
{
    public class FnConxBD
    {
        SqlConnection Conn;

        public static string ObtenerValor(string sKey)
        {
            try
            {
                string _valor = System.Web.Configuration.WebConfigurationManager.AppSettings[sKey];

                if (_valor == null)
                    throw new Exception(string.Format("No se encuentra la Clave: {0}", sKey));
                return _valor;
            }
            catch
            {
                throw new Exception(string.Format("No se encuentra la Clave: {0}", sKey));
            }
        }

        private bool Conectar()
        {
            if (Conn == null)
            {
                Conn = new SqlConnection();
                //Conn.ConnectionString = "DSN=SPV; UID=userspv; PWD=userspv;";
                Conn.ConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};CURRENT LANGUAGE=SPANISH;",
                    ObtenerValor("Servidor"), ObtenerValor("DataBase"), ObtenerValor("Usuario"), ObtenerValor("Password"));
            }

            if (Conn.State == ConnectionState.Closed)
            {
                try
                {
                    Conn.Open();
                }
                catch (Exception ex)
                {
                    RegistraError(ex.Message, "Conexión a la Base");
                    return false;
                }
            }
            return true;
        }

        public void RegistraError(string error, string sql)
        {
            try
            {
                string _Direc = string.Format("{0}/Logs/", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
                string _Log = string.Format("{0}/Saity{1}.log", _Direc, DateTime.Today.ToString("yyyyMMdd"));

                if (!Directory.Exists(_Direc))
                    Directory.CreateDirectory(_Direc);

                using (StreamWriter sw = File.AppendText(_Log))
                {
                    Log(error, sql, sw);
                }
            }
            catch { }
        }

        public void Log(string error, string sql, TextWriter w)
        {
            w.Write("\r\nNueva Entrada : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine(string.Format("  Consulta: {0}", sql));
            w.WriteLine(string.Format("     Error: {0}", error));
            w.WriteLine("-------------------------------");
        }

        public bool TestSQL()
        {
            object _resp = ObtDato("select getdate()");
            if (_resp == null)
                return false;
            else
                return true;
        }

        public string ObtDato(string sQry)
        {
            if (!Conectar()) return null;
            try
            {
                SqlCommand cmd = new SqlCommand(sQry, Conn);
                string _obj = cmd.ExecuteScalar().ToString();
                Conn.Close();
                return _obj;
            }
            catch (Exception ex)
            {
                RegistraError(ex.Message, sQry);
                return null;
            }

        }

        public Int32 EjecutaConsulta(string sQry)
        {
            if (!Conectar()) return -1;
            try
            {
                SqlCommand cmd = new SqlCommand(sQry, Conn);
                int _res = cmd.ExecuteNonQuery();
                Conn.Close();
                return _res;
            }
            catch (Exception ex)
            {
                RegistraError(ex.Message, sQry);
                return -1;
            }
        }

        public DataTable ObtenerTabla(string sQry)
        {
            DataSet _ds = new DataSet();
            DataTable _dt;
            if (!Conectar()) return null;
            try
            {
                SqlDataAdapter adap = new SqlDataAdapter(sQry, Conn);
                adap.Fill(_ds);
                _dt = _ds.Tables[0];
                adap.Dispose();
                Conn.Close();
                return _dt;
            }
            catch (Exception ex)
            {
                RegistraError(ex.Message, sQry);
                throw new Exception(ex.Message);
                return null;
            }

        }

        public string GuardarDocumento(int idRefer, string sNombreDoc, byte[] aArchivo)
        {

            if (!Conectar()) return null;
            try
            {
                int _lgbytes = aArchivo.Length;
                SqlParameter[] mPar = new SqlParameter[1];
                string sQry = string.Format("Execute [BLIDOCTOS].[dbo].[PAGUARDADOCIMAGEN] 0, '{1}', @file, {0}", idRefer, sNombreDoc);

                mPar[0] = new SqlParameter("@file", SqlDbType.VarBinary, _lgbytes, ParameterDirection.Input, true, 0, 0, "aArchivo", DataRowVersion.Current, aArchivo);

                SqlCommand rs = new SqlCommand(sQry, Conn);
                for (int iInc = 0; iInc <= mPar.Length - 1; iInc++)
                { rs.Parameters.Add(mPar[iInc]); }

                object idFile = rs.ExecuteScalar();
                Conn.Close();

                return idFile.ToString();
            }
            catch (Exception ex)
            {
                RegistraError(ex.Message, "Registrar Foto");
                throw new Exception(ex.Message);
                return "0";
            }
        }

        public static DataSet ConvertXmlToDataSet(string _xml)
        {
            try
            {
                StringReader sr = new StringReader(_xml);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);

                return ds;
            }
            catch { return null; }
        }

        public static DataTable ConvertXmlToDataTable(string _xml)
        {
            try
            {
                StringReader sr = new StringReader(_xml);
                DataSet ds = new DataSet();
                ds.ReadXml(sr);

                return ds.Tables[0];
            }
            catch { return null; }
        }
    }
}