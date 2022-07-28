using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;

namespace WSSaityCore.Clases
{
    public class FnMaps
    {
        public objGeolocalizacion FnObtenerGeolocalizacion(string scalle, string numext, string scolonia, string sciudad)
        {
            XmlTextReader _xml;
            DataSet dsDir;
            string _urlmaps, sKeyMaps = "AIzaSyBCrjOdhxDgpD-N3PAxYCZkIC3JCpx2qGY";
            object _idResult, _idGeo, _lat, _lng;
            objGeolocalizacion o = new objGeolocalizacion();

            string _direcc = string.Format("{0} {1}, {2}, {3}", scalle, numext, scolonia, sciudad).Replace(" ", "+");

            try
            {
                _urlmaps = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&language=es&key={1}", _direcc, sKeyMaps);
                _xml = new XmlTextReader(_urlmaps);
                dsDir = new DataSet();
                dsDir.ReadXml(_xml);

                if (!dsDir.Tables.Contains("GeocodeResponse"))
                    throw new Exception("Ocurrio un error al evaluar la respuesta de google maps:\r\n\tNo se encotro la tabla GeocodeResponse");

                if (dsDir.Tables["GeocodeResponse"].Rows[0]["status"].ToString() != "OK")
                    throw new Exception(string.Format("Google maps respondio:\r\n\t{0}", dsDir.Tables["GeocodeResponse"].Rows[0]["status"]));

                if (dsDir.Tables["result"].Rows.Count == 0)
                    throw new Exception("No se encontro información de la Dirección");

                _idResult = dsDir.Tables["result"].Rows[0]["result_id"];
                _idGeo = dsDir.Tables["geometry"].Select(string.Format("result_id={0}", _idResult))[0]["geometry_id"];


                _lat = dsDir.Tables["location"].Select(string.Format("geometry_id={0}", _idGeo))[0]["lat"];
                _lng = dsDir.Tables["location"].Select(string.Format("geometry_id={0}", _idGeo))[0]["lng"];

                o.estatus = 0;
                o.mensaje = "";
                o.latitud = _lat.ToString();
                o.longitud = _lng.ToString();

            }
            catch (Exception ex)
            {
                o.estatus = 1;
                o.mensaje = ex.Message;
                o.latitud = "0";
                o.longitud = "0";

            }

            return o;
        }
    }
}