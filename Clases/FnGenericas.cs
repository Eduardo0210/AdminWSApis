using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WSSaityCore.Clases
{
    public static class FnSeguridad
    {
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

     
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

    }

    public static class FnGenerales
    {
        public static DateTime TryFecha(string _fec, string _format)
        {
            try
            {
                return DateTime.ParseExact(_fec, _format, null);
            }
            catch
            {
                return DateTime.Now;
            }

        }

        public static DateTime TryFechaNull(string _fec, string _format)
        {
            try
            {
                return DateTime.ParseExact(_fec, _format, null);
            }
            catch
            {
                return DateTime.MinValue;
            }

        }

        public static byte[] ImageToByte(Image img)
        {
            string sTemp = Path.GetTempFileName();
            FileStream fs = new FileStream(sTemp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            img.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            fs.Position = 0;

            int imgLength = Convert.ToInt32(fs.Length);
            byte[] bytes = new byte[imgLength];
            fs.Read(bytes, 0, imgLength);
            fs.Close();
            return bytes;
        }
    }

}