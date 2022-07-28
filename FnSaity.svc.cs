using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;
using WSSaityCore.Clases;

namespace WSSaityCore
{
    public class FnSaity : IFnSaity
    {

        private void ValidarToken(oSeguridad oSeg)
        {
            DateTime fectokn = FnGenerales.TryFechaNull(oSeg.fechora, "yyMMddHHmmss");

            if (fectokn == DateTime.MinValue)
                throw new Exception("La Fecha del Token no tiene la sintaxis correcta, se debe enviar en formato yyMMddHHmmss");

            if (DateTime.Now.AddMinutes(-3) > fectokn)
                throw new Exception("El Token ya Expiro");

            object oResp = (new FnConxBD()).ObtDato(string.Format("Exec PCVALIDATOKEN {0}, '{1}', '{2}'", oSeg.idconex, oSeg.fechora, oSeg.token));

            if (oResp.ToString() == "0")
                throw new Exception("El Token NO es Valido");

            //if (oSeg.idusuario == null)
            //    oSeg.idusuario = 0;
        }

        public clsLogin FnValidaUsuario(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            clsLogin resp = new clsLogin();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaUsuario", SJson);

                oLogin obj = jss.Deserialize<oLogin>(SJson);

                ValidarToken(obj);

                string sQry = string.Format("Exec PCLOGIN '{0}', '{1}'", obj.usuario, obj.password);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);

                resp.estatus = Convert.ToInt32(dt.Rows[0]["estatus"]);
                resp.mensaje = dt.Rows[0]["mensaje"].ToString();
                resp.idcliente = Convert.ToInt32(dt.Rows[0]["idcliente"]);
                resp.idusuario = Convert.ToInt32(dt.Rows[0]["iduser"]);
                resp.usuario = dt.Rows[0]["usuario"].ToString();
                resp.clientepadre = Convert.ToInt32(dt.Rows[0]["clientepadre"]);
                resp.idClienteEspecifico = Convert.ToInt32(dt.Rows[0]["idClienteEspecifico"]);
                DataTable dtFac = FnConxBD.ConvertXmlToDataTable(dt.Rows[0]["facultades"].ToString());
                if (dtFac != null)
                {
                    resp.facultades = new List<clsFacultades>();
                    foreach (DataRow dr in dtFac.Rows)
                    {
                        resp.facultades.Add(new clsFacultades() { key = dr["skey"].ToString(), funcion = dr["sfuncion"].ToString(), padre = dr["smnupadre"].ToString() });
                    }
                }
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaUsuario", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;

        }

        public objCodigoPostal FnValidaCodPostal(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objCodigoPostal resp = null;
            oConsultaCP obj = null;
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaCodPostal", SJson);

                obj = jss.Deserialize<oConsultaCP>(SJson);

                ValidarToken(obj);

                string sURL = string.Format("https://api-codigos-postales.herokuapp.com/v2/codigo_postal/{0}", obj.codpostal);
                WebRequest request = WebRequest.Create(sURL);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string sCP = reader.ReadToEnd().ToUpper();

                resp = JsonConvert.DeserializeObject<objCodigoPostal>(sCP);
                response.Close();

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaCodPostal", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                (new FnConxBD()).RegistraError(SJson, ex.Message);
                try
                {
                    string sQry = string.Format("Exec PCCONSULTACP '{0}'", obj.codpostal);

                    if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                        (new FnConxBD()).RegistraError("FnValidaCodPostal", sQry);

                    DataTable dtCP = (new FnConxBD()).ObtenerTabla(sQry);

                    if (dtCP != null)
                    {
                        resp = new objCodigoPostal();
                        resp.estatus = 0;
                        resp.mensaje = "Exito";

                        if (dtCP.Rows.Count != 0)
                        {
                            resp.estado = dtCP.Rows[0]["sestado"].ToString();
                            resp.municipio = dtCP.Rows[0]["smunicipio"].ToString();
                            resp.colonias = new List<string>();
                            DataTable dtCol = FnConxBD.ConvertXmlToDataTable(dtCP.Rows[0]["colonias"].ToString());
                            foreach (DataRow dr in dtCol.Rows)
                            {
                                resp.colonias.Add(dr["scolonia"].ToString());
                            }
                        }
                    }

                }
                catch (Exception ex1)
                {
                    resp = new objCodigoPostal()
                    {
                        estatus = 1,
                        mensaje = ex1.Message
                    };
                    (new FnConxBD()).RegistraError(SJson, ex1.Message);
                }
            }

            return resp;
        }

        public objConfAltaSol FnObtConfClienteAltaSol(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objConfAltaSol resp = new objConfAltaSol();

            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtConfClienteAltaSol", SJson);

                oDatosCliente obj = jss.Deserialize<oDatosCliente>(SJson);

                ValidarToken(obj);

                string sQry = string.Format("Exec PCOBTENERCONFALTACLIEN {0}, {1}", obj.idcliente, obj.idusuario);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);


                DataTable dtDir = FnConxBD.ConvertXmlToDataTable(dt.Rows[0]["Direcciones"].ToString());

                if (dtDir != null)
                {
                    resp.direcciones = new List<objConfDirecciones>();
                    foreach (DataRow dr in dtDir.Rows)
                    {
                        resp.direcciones.Add(new objConfDirecciones()
                        {
                            iddireccion = Convert.ToInt32(dr["kiidlugar"]),
                            direccion = dr["slugar"].ToString()
                        });
                    }
                }

                DataTable dtTien = FnConxBD.ConvertXmlToDataTable(dt.Rows[0]["Tiendas"].ToString());

                if (dtTien != null)
                {
                    resp.tiendas = new List<objTiendas>();
                    foreach (DataRow dr in dtTien.Rows)
                    {
                        resp.tiendas.Add(new objTiendas()
                        {
                            idtienda = Convert.ToInt32(dr["kiidtienda"]),
                            tienda = dr["stienda"].ToString()
                        });
                    }
                }

                DataTable dtDoctos = FnConxBD.ConvertXmlToDataTable(dt.Rows[0]["Documentos"].ToString());

                if (dtDoctos != null)
                {
                    resp.documentos = new List<objConfDoctos>();
                    foreach (DataRow dr in dtDoctos.Rows)
                    {
                        resp.documentos.Add(new objConfDoctos()
                        {
                            iddocumento = Convert.ToInt32(dr["kiidocto"]),
                            documento = dr["sdocto"].ToString()
                        });
                    }
                }
                resp.estatus = 0;
                resp.mensaje = "Exito";


                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtConfClienteAltaSol", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;

        }

        public objAltaCliente FnRegistraSolicitud(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objAltaCliente resp = new objAltaCliente();

            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistraSolicitud", SJson);

                oDatosPersonales obj = jss.Deserialize<oDatosPersonales>(SJson);

                ValidarToken(obj);

                string xDirec = "<direcciones>";
                if (obj.direcciones != null)
                {
                    foreach (oDireccion d in obj.direcciones)
                    {
                        objGeolocalizacion o = (new FnMaps()).FnObtenerGeolocalizacion(d.calle, d.numext, d.colonia, d.municipio);

                        xDirec += string.Format("<direccion><idlugar>{10}</idlugar><calle>{0}</calle><numext>{1}</numext><numint>{2}</numint><colonia>{3}</colonia><ciudad>{4}</ciudad><estado>{5}</estado><codpost>{6}</codpost><pais>{7}</pais><latitud>{8}</latitud><longitud>{9}</longitud></direccion>",
                            d.calle, d.numext, d.numint, d.colonia, d.municipio, d.estado, d.cp, d.pais, o.latitud, o.longitud, d.iddireccion);
                    }
                }
                xDirec += "</direcciones>";

                string xOtrosCampos = "<items>";

                if (obj.otrosdatos != null)
                {
                    foreach (string sKey in obj.otrosdatos.Keys)
                        xOtrosCampos += string.Format("<item><campo>{0}</campo><valor>{1}</valor></item>", sKey, obj.otrosdatos[sKey]);
                }
                xOtrosCampos += "</items>";

                string xDocumentos = "<documentos>";

                if (obj.documentos != null)
                {
                    foreach (oDocumetosAC o in obj.documentos)
                        xDocumentos += string.Format("<documento><iddocto>{0}</iddocto><bsolicitar>{1}</bsolicitar></documento>", o.idDocumento, o.solicitado);
                }
                xDocumentos += "</documentos>";

                int valfolio = 0;

                var numeroTienda = (obj.numeroTienda != "" ? obj.numeroTienda : "NULL");
                var fechaAlta = (obj.numeroTienda != "" ? obj.numeroTienda : "NULL");
                if (obj.valfolio != null)
                    valfolio = obj.valfolio;


                string sQry = string.Format("Exec PAREGISTRARFOLIO '{0}', '{1}', '{2}', '{3}','{4}', {5}, '{6}', '{7}', {8}, {9},'{10}', {11}, '{12}','{13}','{14}','{15}'",
                 obj.nombres, obj.apaterno, obj.amaterno, obj.celular, obj.correo, obj.idesquema, obj.folioint, xDirec, obj.idtienda, valfolio, xOtrosCampos, obj.idusuario, xDocumentos,obj.numeroTienda,obj.fechaAlta,obj.agencia);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);

                resp.estatus = Convert.ToInt32(dt.Rows[0]["estatus"]);
                resp.mensaje = dt.Rows[0]["mensaje"].ToString();
                resp.folio = dt.Rows[0]["folio"].ToString();
                resp.remesa = dt.Rows[0]["remesa"].ToString();


                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistraSolicitud", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;

        }

        private string sXmlPersona(oDatosPersonales p)
        {
            string s;

            s = string.Format("<item><nombre>{0}</nombre><apaterno>{1}</apaterno><amaterno>{2}</amaterno><mail>{3}</mail><celular>{4}</celular><folioint>{5}</folioint><tienda>{6}</tienda><direcciones>@direcc</direcciones><documentos>@doctos</documentos></item>",
                p.nombres.ToUpper(), p.apaterno.ToUpper(), p.amaterno.ToUpper(), p.correo, p.celular, p.folioint.ToUpper(), p.idtienda);

            string d = "";
            if (p.direcciones != null)
            {
                foreach (oDireccion oD in p.direcciones)
                {
                    objGeolocalizacion o = (new FnMaps()).FnObtenerGeolocalizacion(oD.calle, oD.numext, oD.colonia, oD.municipio);

                    d += string.Format("[direccion][idlugar]{0}[/idlugar][calle]{1}[/calle][numext]{2}[/numext][numint]{3}[/numint][colonia]{4}[/colonia][ciudad]{5}[/ciudad][estado]{6}[/estado][codpost]{7}[/codpost][pais]{8}[/pais][latitud]{9}[/latitud][longitud]{10}[/longitud][/direccion]",
                        oD.iddireccion, oD.calle.ToUpper(), oD.numext.ToUpper(), oD.numint.ToUpper(), oD.colonia.ToUpper(), oD.municipio.ToUpper(), oD.estado, oD.cp.ToUpper(), oD.pais.ToUpper(), o.latitud, o.longitud);
                }
            }

            string f = "";

            if (p.documentos != null)
            {
                foreach (oDocumetosAC oF in p.documentos)
                {
                    f += string.Format("[documento][iddocto]{0}[/iddocto][bsolicitar]{1}[/bsolicitar][/documento]",
                        oF.idDocumento, oF.solicitado);
                }
            }

            s = s.Replace("@direcc", d).Replace("@doctos", f);

            return s;

        }

        public objTienda FnValidaTienda(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objTienda resp = new objTienda();

            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistraMasivo", SJson);


                oNumeroTienda obj = jss.Deserialize<oNumeroTienda>(SJson);

                string sQry = string.Format("Exec PCOBTENERTIENDA '{0}'",
                   obj.numTienda);
                (new FnConxBD()).RegistraError("FnRegistraMasivo", sQry);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);

                resp.estatus = int.Parse(dt.Rows[0]["estatus"].ToString());
                resp.mensaje = dt.Rows[0]["mensaje"].ToString();
                resp.stienda = dt.Rows[0]["stienda"].ToString();
                resp.kiidtienda = dt.Rows[0]["kiidtienda"].ToString();



            }
            catch (Exception ex)
            {

                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);

            }
            return resp;

        }


        public objAltaMasivo FnRegistraMasivo(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objAltaMasivo resp = new objAltaMasivo();

            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistraMasivo", SJson);

                oDatosMasivos obj = jss.Deserialize<oDatosMasivos>(SJson);

                ValidarToken(obj);

                string sXml = "<items>";


                foreach (oDatosPersonales p in obj.datos)
                {
                    sXml += sXmlPersona(p);
                }

                sXml += "</items>";

                string sQry = string.Format("Exec PAREGISTRMASIVO {0}, '{1}', {2}",
                    obj.idesquema, sXml, obj.idusuario);

                (new FnConxBD()).RegistraError("FnRegistraMasivo", sQry);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);

                resp.estatus = 00;
                resp.mensaje = "Exito";
                resp.folioini = dt.Rows[0]["minfolio"].ToString();
                resp.foliofin = dt.Rows[0]["maxfolio"].ToString();
                resp.remesa = dt.Rows[0]["remesa"].ToString();

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistraSolicitud", JsonConvert.SerializeObject(resp));


            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;

        }

        public string TestConection()
        {
            objRespGral _DetResult;

            try
            {
                DataTable _datos = (new FnConxBD()).ObtenerTabla("select getdate()");

                _DetResult = new objRespGral()
                {
                    estatus = 0,
                    mensaje = "CONEXION EXITOSA"
                };
            }
            catch (Exception ex)
            {

                _DetResult = new objRespGral()
                {
                    estatus = 1,
                    mensaje = ex.Message
                };
            }

            return JsonConvert.SerializeObject(_DetResult);
        }

        public objCatEsquemas FnObtenerEsquemaReporte(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objCatEsquemas resp = new objCatEsquemas();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemaReporte", SJson);

                oDatosCliente obj = jss.Deserialize<oDatosCliente>(SJson);

                ValidarToken(obj);



                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCOBTENERESQUEMA '{0}'", obj.idcliente));

                if (_datos != null)
                {
                    resp.esquemas = new List<objEsquemas>();
                    foreach (DataRow dr in _datos.Rows)
                    {
                        resp.esquemas.Add(new objEsquemas()
                        {
                            idesquema = Convert.ToInt32(dr["idesquema"]),
                            esquema = dr["esquema"].ToString()
                        });
                    }
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemaReporte", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }


        public objCatEsquemas FnObtenerEsquemas(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objCatEsquemas resp = new objCatEsquemas();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemas", SJson);

                oSeguridad obj = jss.Deserialize<oSeguridad>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCOBTENERCLIENTES {0}", obj.idusuario));

                if (_datos != null)
                {
                    resp.esquemas = new List<objEsquemas>();
                    foreach (DataRow dr in _datos.Rows)
                    {
                        resp.esquemas.Add(new objEsquemas()
                        {
                            idesquema = Convert.ToInt32(dr["idclien"]),
                            esquema = dr["scliente"].ToString()
                        });
                    }
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemas", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objCatClientes FnObtenerClientes(string SJson)
        {

            JavaScriptSerializer jss = new JavaScriptSerializer();
            objCatClientes resp = new objCatClientes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerClientes", SJson);

                oSeguridad obj = jss.Deserialize<oSeguridad>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla("Exec PCOBTENERCLIENTESP");

                if (_datos != null)
                {
                    resp.clientes = new List<objCliente>();
                    foreach (DataRow dr in _datos.Rows)
                    {
                        resp.clientes.Add(new objCliente()
                        {
                            idcliente = Convert.ToInt32(dr["kiidcliente"]),
                            cliente = dr["scliente"].ToString()
                        });
                    }
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemas", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }
            return resp;

        }


        public objCatEsquemas FnObtenerEsquemasTodos(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objCatEsquemas resp = new objCatEsquemas();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemasTodos", SJson);

                oSeguridad obj = jss.Deserialize<oSeguridad>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCOBTENERCLIENTES {0}, 1", obj.idusuario));

                if (_datos != null)
                {
                    resp.esquemas = new List<objEsquemas>();
                    foreach (DataRow dr in _datos.Rows)
                    {
                        resp.esquemas.Add(new objEsquemas()
                        {
                            idesquema = Convert.ToInt32(dr["idclien"]),
                            esquema = dr["scliente"].ToString()
                        });
                    }
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerEsquemas", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objFolios FnConsultaFolios(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objFolios resp = new objFolios();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnConsultaFolios", SJson);

                oConsultaFolios obj = jss.Deserialize<oConsultaFolios>(SJson);

                ValidarToken(obj);
                  
                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCOBTENERFOLIOS {0}, {1}, '%{2}%', '%{3}%', '%{4}%', '{5}', '{6}', {7}",
                    obj.opc, obj.idcliente, obj.sfolio, obj.sremesa, obj.snombre, obj.fecini, obj.fecfin, obj.idusuario));

                (new FnConxBD()).RegistraError("FnConsultaFolios", "Peticion: " + string.Format("Exec PCOBTENERFOLIOS {0}, {1}, '%{2}%', '%{3}%', '%{4}%', '{5}', '{6}', {7}",
                    obj.opc, obj.idcliente, obj.sfolio, obj.sremesa, obj.snombre, obj.fecini, obj.fecfin, obj.idusuario));

                if (_datos != null)
                {
                    resp.folios = new List<objFolioReg>();
                    foreach (DataRow dr in _datos.Rows)
                    {
                        resp.folios.Add(new objFolioReg()
                        {
                            idfolio = Convert.ToInt32(dr["kiidfolio"]),
                            folio = dr["sfolio"].ToString(),
                            nombre = dr["nombre"].ToString(),
                            celular = dr["celular"].ToString(),
                            remesa = dr["snumremesa"].ToString(),
                            folioint = dr["sfolioint"].ToString(),
                            fecalta = dr["fecalta"].ToString(),
                            fectermino = dr["fectermino"].ToString(),
                            fecaviso = dr["firmaaviso"].ToString(),
                            fecultsms = dr["ultsms"].ToString(),
                            observac = dr["observac"].ToString(),
                            urlentregable = dr["sentregable"].ToString(),
                            estatus = dr["sestfolio"].ToString(),
                            cliente = dr["cliente"].ToString(),
                            usuarioAlta = dr["usuario"].ToString(),
                            alta = dr["alta"].ToString(),
                            tienda = dr["tienda"].ToString()
                        });

                    }
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                //if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                //    (new FnConxBD()).RegistraError("FnConsultaFolios", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objInfoFolio FnInfoFolioGeneral(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objInfoFolio resp = new objInfoFolio();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioGeneral", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PCOBTINFOFOLIO] '{0}'", obj.sfolio));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                DataRow dr = _datos.Rows[0];

                resp.idfolio = Convert.ToInt32(dr["kiidfolio"]);
                resp.folio = dr["folio"].ToString();
                resp.nombre = dr["nombre"].ToString();
                resp.apellidop = dr["paterno"].ToString();
                resp.apellidom = dr["materno"].ToString();
                resp.tel = dr["tel"].ToString();
                resp.mail = dr["correo"].ToString();
                resp.curp = dr["curp"].ToString();
                resp.rfc = dr["rfc"].ToString();
                resp.estatus = Convert.ToInt32(dr["estatus"]);
                resp.fechaini = dr["feciniciover"].ToString();
                resp.fechacie = dr["fecfinal"].ToString();
                resp.resultado = dr["resultado"].ToString();
                resp.causa = dr["causa"].ToString();
                resp.claveine = dr["claveine"].ToString();
                resp.folioint = dr["folioint"].ToString();
                resp.estatusfolio = dr["sestfolio"].ToString();
                resp.fecalta = dr["fecalta"].ToString();
                resp.fecultact = dr["fecultact"].ToString();
                resp.fecultsms = dr["fecultsms"].ToString();
                resp.porcavance = Convert.ToInt32(dr["porcavance"]);
                resp.nomcompleto = dr["nombrecompleto"].ToString();
                resp.cliente = dr["cliente"].ToString();
                resp.bdictamen = Convert.ToInt32(dr["bdictamen"]);
                resp.idesquema = Convert.ToInt32(dr["idesquema"]);

                DataTable dtDoc = FnConxBD.ConvertXmlToDataTable(dr["documentos"].ToString());

                if (dtDoc != null)
                {
                    resp.documentos = new List<objDocumentoBase>();
                    foreach (DataRow drD in dtDoc.Rows)
                    {
                        resp.documentos.Add(new objDocumentoBase()
                        {
                            documento = drD["nomdocto"].ToString(),
                            benviado = Convert.ToInt32(drD["benviado"])
                        });
                    }
                }

                DataTable dtCampos = FnConxBD.ConvertXmlToDataTable(dr["otrosdatos"].ToString());
                if (dtCampos != null)
                {
                    resp.otrosdatos = new List<objOtroCampo>();
                    foreach (DataRow drD in dtCampos.Rows)
                    {
                        resp.otrosdatos.Add(new objOtroCampo()
                        {
                            descrip = drD["descrip"].ToString(),
                            valor = drD["valor"].ToString()
                        });
                    }
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioGeneral", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objHistorial FnInfoFolioHistoria(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objHistorial resp = new objHistorial();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioHistoria", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PCOBTHISTORIAL] '{0}'", obj.sfolio));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.eventos = new List<objEvento>();

                foreach (DataRow dr in _datos.Rows)
                {
                    resp.eventos.Add(new objEvento()
                    {
                        operacion = dr["operacion"].ToString(),
                        latitud = dr["latitud"].ToString(),
                        longitud = dr["longitud"].ToString(),
                        fechora = dr["fechora"].ToString(),
                        idreferencia = Convert.ToInt32(dr["idreferencia"]),
                        bcoincide = Convert.ToInt32(dr["bcoincide"]),
                        tipo = Convert.ToInt32(dr["idtipo"]),
                        urlfoto = dr["foto"].ToString()
                    });
                }

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioHistoria", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objDireccionesFolio FnInfoFolioDirecciones(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objDireccionesFolio resp = new objDireccionesFolio();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioDirecciones", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PCOBTDIRSFOLIO] '{0}'", obj.sfolio));

                if (_datos == null)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.direcciones = new List<objDirFolio>();

                foreach (DataRow dr in _datos.Rows)
                {
                    resp.direcciones.Add(new objDirFolio()
                    {
                        idLugar = Convert.ToInt32(dr["idlugar"]),
                        calle = dr["calle"].ToString(),
                        numExt = dr["numext"].ToString(),
                        numInt = dr["numint"].ToString(),
                        colonia = dr["colonia"].ToString(),
                        ciudad = dr["ciudad"].ToString(),
                        estado = dr["estado"].ToString(),
                        cp = dr["cp"].ToString(),
                        latitud = dr["latitud"].ToString(),
                        longitud = dr["longitud"].ToString(),
                        latUser = dr["latituduser"].ToString(),
                        longUser = dr["longituduser"].ToString(),
                        direccion = dr["direccion"].ToString(),
                        estatus = dr["estatus"].ToString(),
                        lugar = dr["slugar"].ToString(),
                        bmisma = Convert.ToInt32(dr["bmisma"])
                    });
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioDirecciones", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objDocumentosFolio FnInfoFolioDocumentos(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objDocumentosFolio resp = new objDocumentosFolio();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioDocumentos", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCOBTDOCFOLIO '{0}'", obj.sfolio));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.documentos = new List<objDoctoFolio>();

                foreach (DataRow dr in _datos.Rows)
                {
                    resp.documentos.Add(new objDoctoFolio()
                    {
                        iddocto = Convert.ToInt32(dr["kiidocto"]),
                        idfoldocto = Convert.ToInt32(dr["kiidfoldoc"]),
                        bsoladic = Convert.ToInt32(dr["bsoladic"]),
                        ubicesp = dr["bubicesp"].ToString(),
                        numdoctos = Convert.ToInt32(dr["numdoctos"]),
                        tipo = Convert.ToInt32(dr["itipo"]),
                        origen = dr["origen"].ToString(),
                        documento = dr["sdocto"].ToString(),
                        valor = dr["valor"].ToString(),
                        estatus = dr["estatus"].ToString(),
                        lugar = dr["slugar"].ToString()
                    });
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioDocumentos", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objMarcasFolio FnInfoFolioMarcasMapa(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objMarcasFolio resp = new objMarcasFolio();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioMarcasMapa", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                // ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PCOBTMAPAFOLIO] '{0}'", obj.sfolio));

                if (_datos == null)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.marcas = new List<objMarcaMapa>();

                foreach (DataRow dr in _datos.Rows)
                {
                    resp.marcas.Add(new objMarcaMapa()
                    {
                        referencia = dr["referencia"].ToString(),
                        latitud = dr["slatitud"].ToString(),
                        longitud = dr["slongitud"].ToString(),
                        tipo = Convert.ToInt32(dr["tipo"])
                    });
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioMarcasMapa", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objValoresCat FnObtenerCatalogo(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objValoresCat resp = new objValoresCat();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerCatalogo", SJson);

                oCatalogo obj = jss.Deserialize<oCatalogo>(SJson);

                ValidarToken(obj);

                string sQry = "";

                switch (obj.opc)
                {
                    case 0: // Catalogo de Resultados Dictaminacion
                        sQry = string.Format("Exec PCCATRESULTADO '{0}'", obj.idreferencia);
                        break;

                    case 1:
                    case 2: //Catalogo de Causas si la Dictaminacion es 1 o 2

                        sQry = string.Format("Exec PCCATCAUSAS {0}, '{1}'", obj.resultado, obj.sFolio);
                        break;

                    case 3: //Catalogo de Rechazo por idCliente
                        sQry = string.Format("Exec PCCATRECHAZO {0}, 0", obj.idreferencia);
                        break;
                    case 5:
                        sQry = string.Format("Exec PCCATEJECUTIVO {0}", obj.idreferencia);
                        break;

                }

                DataTable _datos = (new FnConxBD()).ObtenerTabla(sQry);

                if (_datos == null)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.items = new List<objCatalogo>();

                foreach (DataRow dr in _datos.Rows)
                {
                    resp.items.Add(new objCatalogo()
                    {
                        valor = Convert.ToInt32(dr[0]),
                        descripcion = dr[1].ToString()
                    });
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerCatalogo", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objZip FnObtenerZip(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objZip resp = new objZip();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnInfoFolioMarcasMapa", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                string sQry = string.Format("Exec PCOBTDATOSENTREGABLE '{0}', 0, {1}", obj.sfolio, 1);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                string _rutaEnt = (new FnEntregables()).GenerarEntregableZIP(dtInfo, obj.sfolio);

                resp.estatus = 0;
                resp.mensaje = "Exito";
                resp.pathfile = _rutaEnt.Replace(FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("PathWeb"));

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnDictamenFolio", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objRespGral FnDictamenFolio(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnDictamenFolio", SJson);

                oDictamen obj = jss.Deserialize<oDictamen>(SJson);

                //  ValidarToken(obj);

                string sQry = string.Format("Exec PADICTAMENFOLIO '{0}', {1}, {2}, {3}, '{4}'", obj.sfolio, obj.idresultado, obj.idcausa, obj.idusuario, obj.observac);
                DataTable _datos = (new FnConxBD()).ObtenerTabla(sQry);

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");


                #region Valida si Requiere Generar Entregable

                if (_datos.Rows[0]["bgenentreg"].ToString() == "1")
                {
                    sQry = string.Format("Exec PCOBTDATOSENTREGABLE '', {0}, {1}", _datos.Rows[0]["kiidfolio"], _datos.Rows[0]["kiidtipoentregable"]);
                    DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                    if (dtInfo == null || dtInfo.Rows.Count == 0)
                        throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                    string _rutaEnt = "";

                    if (_datos.Rows[0]["kiidtipoentregable"].ToString() == "1")
                        _rutaEnt = (new FnEntregables()).GenerarEntregableZIP(dtInfo, obj.sfolio);
                    else
                        _rutaEnt = (new FnEntregables()).GenerarEntregablePDF(dtInfo, obj.sfolio);
                    //_rutaEnt = (new FnEntregables()).GenerarEntregablePDF(dtInfo,"BC200408005665");//PRUEBA

                    sQry = string.Format("Exec PAACTUALIZARENTREGABLE {0}, '{1}'", _datos.Rows[0]["kiidfolio"], _rutaEnt.Replace(FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("PathWeb")));
                    (new FnConxBD()).EjecutaConsulta(sQry);
                }

                #endregion


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnDictamenFolio", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objDetDocumento FnObtDetDocumento(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objDetDocumento resp = new objDetDocumento();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtDetDocumento", SJson);

                oDetDocumento obj = jss.Deserialize<oDetDocumento>(SJson);

                ValidarToken(obj);

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PCOBTDETALLEDOC] {0}", obj.iddocumento));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                DataRow dr = _datos.Rows[0];

                resp.documento = dr["sdocto"].ToString();
                resp.iddocto = Convert.ToInt32(dr["kiidfoldoc"]);
                resp.estatusdoc = dr["estatus"].ToString();
                resp.fecenvio = dr["dfecenvio"].ToString();
                resp.tipo = dr["tipo"].ToString();
                resp.lugenvesp = dr["lugaresp"].ToString();
                resp.latitud = dr["latitud"].ToString();
                resp.longitud = dr["longitud"].ToString();
                resp.direcenv = dr["direcenv"].ToString();
                resp.envioesperado = dr["envioesperado"].ToString();
                resp.gpsFake = Convert.ToInt32(dr["gpsFake"]);
                resp.numdoctos = Convert.ToInt32(dr["iNumDoctos"]);
                resp.img1 = dr["img1"].ToString();
                resp.img2 = dr["img2"].ToString();
                resp.img3 = dr["img3"].ToString();
                resp.img4 = dr["img4"].ToString();
                resp.img5 = dr["img5"].ToString();
                resp.img6 = dr["img6"].ToString();
                resp.img7 = dr["img7"].ToString();
                resp.img8 = dr["img8"].ToString();
                resp.img9 = dr["img9"].ToString();
                resp.img10 = dr["img10"].ToString();
                resp.img11 = dr["img11"].ToString();
                resp.img12 = dr["img12"].ToString();
                resp.img13 = dr["img13"].ToString();
                resp.img14 = dr["img14"].ToString();
                resp.img15 = dr["img15"].ToString();
                resp.img16 = dr["img16"].ToString();
                resp.img17 = dr["img17"].ToString();
                resp.img18 = dr["img18"].ToString();
                resp.img19 = dr["img19"].ToString();
                resp.img20 = dr["img20"].ToString();
                resp.badicional = Convert.ToInt32(dr["badicional"]);
                resp.brechazar = Convert.ToInt32(dr["brechazar"]);


                DataTable dtVer = FnConxBD.ConvertXmlToDataTable(dr["validaciones"].ToString());

                if (dtVer != null)
                {
                    resp.validaciones = new List<objVerificacion>();
                    foreach (DataRow drD in dtVer.Rows)
                    {
                        resp.validaciones.Add(new objVerificacion()
                        {
                            verificacion = drD["verificacion"].ToString(),
                            idverificacion = Convert.ToInt32(drD["idref"]),
                            check = Convert.ToInt32(drD["bchek"])
                        });
                    }
                }

                dtVer = FnConxBD.ConvertXmlToDataTable(dr["campos"].ToString());

                if (dtVer != null)
                {
                    resp.campos = new List<objCampo>();
                    foreach (DataRow drD in dtVer.Rows)
                    {
                        resp.campos.Add(new objCampo()
                        {
                            campo = drD["campo"].ToString(),
                            idcampo = Convert.ToInt32(drD["idref"]),
                            valor = drD["valor"].ToString()
                        });
                    }
                }


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtDetDocumento", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objRespGral FnVerificarDocumento(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnVerificarDocumento", SJson);

                oVerificacion obj = jss.Deserialize<oVerificacion>(SJson);

                ValidarToken(obj);

                string xVerif = "<verificaciones>";

                if (obj.verificaciones != null)
                {
                    foreach (oVerificDocto o in obj.verificaciones)
                    {
                        xVerif += string.Format("<verificacion><idverif>{0}</idverif><bchek>{1}</bchek><valor>{2}</valor></verificacion>",
                            o.idverificacion, o.check, o.valor);
                    }
                }

                xVerif += "</verificaciones>";

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PAREGVERIFICDOCTO] {0}, '{1}', {2}, '{3}'", obj.iddocumento, xVerif, obj.opcrechazo, obj.mensaje));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnVerificarDocumento", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objRespGral FnRechazarDocumento(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRechazarDocumento", SJson);

                oRechazos obj = jss.Deserialize<oRechazos>(SJson);

                ValidarToken(obj);

                string xVerif = "<doctos>";

                if (obj.rechazos != null)
                {
                    foreach (oRecDocto o in obj.rechazos)
                    {
                        xVerif += string.Format("<docto><iddocto>{0}</iddocto><opc>{1}</opc><mensaje>{2}</mensaje></docto>",
                            o.iddocumento, o.opcrechazo, o.mensaje);
                    }
                }

                xVerif += "</doctos>";

                DataTable _datos = (new FnConxBD()).ObtenerTabla(string.Format("Exec [PARECHAZARDOCTOS] {0}, '{1}'", obj.sfolio, xVerif));

                if (_datos == null || _datos.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información");

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRechazarDocumento", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objRespGral FnRotarImagen(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRotarImagen", SJson);

                oRotarImg obj = jss.Deserialize<oRotarImg>(SJson);

                ValidarToken(obj);

                obj.sruta = obj.sruta.Replace("http://www.saity.mx/", "");
                obj.sruta = obj.sruta.Replace("https://www.saity.mx/", "");
                obj.sruta = obj.sruta.Replace("http://saity.mx/", "");
                obj.sruta = obj.sruta.Replace("https://saity.mx/", "");

                string _ruta = string.Format("{0}/{1}", FnConxBD.ObtenerValor("PathFisico"), obj.sruta);

                if (obj.opcion == 1)
                {
                    Image img = Image.FromFile(_ruta);
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    img.Save(_ruta);
                }
                else
                {
                    Image img = Image.FromFile(_ruta);
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    img.Save(_ruta);
                }

                resp.estatus = 0;
                resp.mensaje = _ruta.Replace(FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("PathWeb"));

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRotarImagen", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objRespGral FnReiniciarPassword(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReiniciarPassword", SJson);

                oResetPass obj = jss.Deserialize<oResetPass>(SJson);

                ValidarToken(obj);

                DataTable dt = (new FnConxBD()).ObtenerTabla(string.Format("Exec PARESETPASSWORD '{0}'", obj.smail));

                if (dt.Rows[0]["CodResp"].ToString() == "1")
                    throw new Exception(dt.Rows[0]["Mens"].ToString());

                resp.estatus = 0;
                resp.mensaje = dt.Rows[0]["Mens"].ToString();

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReiniciarPassword", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objUser FnValidaTokenPassword(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objUser resp = new objUser();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaTokenPassword", SJson);

                oTokenPass obj = jss.Deserialize<oTokenPass>(SJson);

                ValidarToken(obj);

                DataTable dt = (new FnConxBD()).ObtenerTabla(string.Format("Exec PCVALIDATOKENPASS '{0}'", obj.tokenpass));

                if (dt.Rows[0]["iduser"].ToString() == "0")
                    throw new Exception(dt.Rows[0]["mensaje"].ToString());

                resp.estatus = 0;
                resp.idusuario = Convert.ToInt32(dt.Rows[0]["iduser"]);
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnValidaTokenPassword", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public clsLogin FnCambioPassword(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            clsLogin resp = new clsLogin();

            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnCambioPassword", SJson);

                oCambiPassword obj = jss.Deserialize<oCambiPassword>(SJson);

                ValidarToken(obj);

                string sQry = string.Format("Exec PACAMBIOPASSWORD {0}, '{1}', '{2}'", obj.idusuario, obj.password, obj.tokenpass);

                DataTable dt = (new FnConxBD()).ObtenerTabla(sQry);

                if (dt.Rows[0]["CodResp"].ToString() == "1")
                    throw new Exception(dt.Rows[0]["mensaje"].ToString());

                resp.estatus = Convert.ToInt32(dt.Rows[0]["CodResp"]);
                resp.mensaje = dt.Rows[0]["mensaje"].ToString();
                resp.idcliente = Convert.ToInt32(dt.Rows[0]["idcliente"]);
                resp.idusuario = Convert.ToInt32(dt.Rows[0]["iduser"]);
                resp.usuario = dt.Rows[0]["usuario"].ToString();

                DataTable dtFac = FnConxBD.ConvertXmlToDataTable(dt.Rows[0]["facultades"].ToString());
                if (dtFac != null)
                {
                    resp.facultades = new List<clsFacultades>();
                    foreach (DataRow dr in dtFac.Rows)
                    {
                        resp.facultades.Add(new clsFacultades() { key = dr["skey"].ToString(), funcion = dr["sfuncion"].ToString(), padre = dr["smnupadre"].ToString() });
                    }
                }

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnCambioPassword", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;

        }

        public objRespGral FnRegenerarEntregable(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objRespGral resp = new objRespGral();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegenerarEntregable", SJson);

                oFolio obj = jss.Deserialize<oFolio>(SJson);

                ValidarToken(obj);

                string sQry = string.Format("Exec PCOBTDATOSENTREGABLE '{0}', {1}, 0", obj.sfolio, obj.ifolio);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                string _rutaEnt = "";

                if (!dtInfo.Columns.Contains("sfolio"))
                    _rutaEnt = (new FnEntregables()).GenerarEntregableZIP(dtInfo, obj.sfolio);
                else
                    _rutaEnt = (new FnEntregables()).GenerarEntregablePDF(dtInfo, obj.sfolio);

                sQry = string.Format("Exec PAACTUALIZARENTREGABLE {0}, '{1}'", obj.ifolio, _rutaEnt.Replace(FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("PathWeb")));
                (new FnConxBD()).EjecutaConsulta(sQry);


                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegenerarEntregable", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        #region Reportes
        public objReportes FnReporeDictaminados(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objReportes resp = new objReportes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporeDictaminados", SJson);

                oConsultaReporte obj = jss.Deserialize<oConsultaReporte>(SJson);

                ValidarToken(obj);


                string sQry = string.Format("Exec PCREPORTEDICTAMEN '{0}', '{1}', {2}, {3}", obj.fecinicio, obj.fecfinal, obj.idcliente, obj.idesquema);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                List<string> lColumnas = new List<string>();
                foreach (DataColumn dc in dtInfo.Columns)
                {
                    lColumnas.Add(dc.Caption);
                }

                string datos = JsonConvert.SerializeObject(dtInfo);

                resp.columnas = lColumnas;
                resp.datos = datos;
                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteSMSEnviados", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }

        public objReportes FnReporteCifras(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objReportes resp = new objReportes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteCifras", SJson);

                oConsultaReporteCifras obj = jss.Deserialize<oConsultaReporteCifras>(SJson);

                ValidarToken(obj);


                string sQry = string.Format("Exec PCREPORTECIFRAS '{0}', '{1}', {2}, {3}, {4}", obj.fecinicio, obj.fecfinal, obj.idcliente, obj.idesquema, obj.remesa);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                List<string> lColumnas = new List<string>();
                foreach (DataColumn dc in dtInfo.Columns)
                {
                    lColumnas.Add(dc.Caption);
                }

                string datos = JsonConvert.SerializeObject(dtInfo);

                resp.columnas = lColumnas;
                resp.datos = datos;
                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteCifras", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }



        public objReportes FnReporteSMSEnviados(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objReportes resp = new objReportes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteSMSEnviados", SJson);

                oConsultaReporte obj = jss.Deserialize<oConsultaReporte>(SJson);

                ValidarToken(obj);


                string sQry = string.Format("Exec PCREPORTESMS '{0}', '{1}', {2}, {3}", obj.fecinicio, obj.fecfinal, obj.idcliente, obj.idesquema);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                List<string> lColumnas = new List<string>();
                foreach (DataColumn dc in dtInfo.Columns)
                {
                    lColumnas.Add(dc.Caption);
                }

                string datos = JsonConvert.SerializeObject(dtInfo);

                resp.columnas = lColumnas;
                resp.datos = datos;
                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteSMSEnviados", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }



        public objReportes FnReporeConsumos(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objReportes resp = new objReportes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporeConsumos", SJson);

                oConsultaReporteConsumos obj = jss.Deserialize<oConsultaReporteConsumos>(SJson);

                //  ValidarToken(obj);


                string sQry = string.Format("Exec PCREPORTECONSUMOS '{0}', '{1}', {2}, {3}, {4}, {5}, {6}", obj.tipo, obj.anio, obj.semestre, obj.mes, obj.semana, obj.idcliente, obj.idesquema);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                List<string> lColumnas = new List<string>();
                foreach (DataColumn dc in dtInfo.Columns)
                {
                    lColumnas.Add(dc.Caption);
                }

                string datos = JsonConvert.SerializeObject(dtInfo);

                resp.columnas = lColumnas;
                resp.datos = datos;
                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteSMSEnviados", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }


        public objReportes FnReporeCancelados(string SJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            objReportes resp = new objReportes();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporeCancelados", SJson);

                oConsultaReporte obj = jss.Deserialize<oConsultaReporte>(SJson);

                //  ValidarToken(obj);


                string sQry = string.Format("Exec PCREPORTECANCELADOS '{0}', '{1}', {2}, {3}", obj.fecinicio, obj.fecfinal, obj.idcliente, obj.idesquema);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                List<string> lColumnas = new List<string>();
                foreach (DataColumn dc in dtInfo.Columns)
                {
                    lColumnas.Add(dc.Caption);
                }

                string datos = JsonConvert.SerializeObject(dtInfo);

                resp.columnas = lColumnas;
                resp.datos = datos;
                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnReporteSMSEnviados", JsonConvert.SerializeObject(resp));

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;
                (new FnConxBD()).RegistraError(SJson, ex.Message);
            }

            return resp;
        }



        #endregion

        public objRespGral FnBorrarFolio(string folio)
        {

            objRespGral resp = new objRespGral();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnEliminarFolio", folio);

                string sQry = string.Format("Exec PAELIMINARFOLIO2 '{0}'", folio);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                resp.estatus = 0;
                resp.mensaje = "Exito";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegenerarEntregable", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;

            }



            return resp;


        }

        public objFunciones FnObtenerFunciones(int tipoUsuario)
        {
            objFunciones resp = new objFunciones();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerFunciones", "" + tipoUsuario);

                string sQry = string.Format("Exec PCOBTENERFUNCIONES '{0}'", tipoUsuario);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");
                resp.funciones = new List<objFuncion>();
                foreach (DataRow row in dtInfo.Rows)
                {

                    resp.funciones.Add(new objFuncion() { kiidFuncion = int.Parse(row["kiidFuncion"].ToString()), funcion = row["sfuncion"].ToString() });
                }

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegenerarEntregable", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {


                return resp;
            }

            return resp;
        }



        public objRespGral FnRegistrarUsuario(string SJson)
        {
            objRespGral resp = new objRespGral();
            JavaScriptSerializer jss = new JavaScriptSerializer();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerFunciones", SJson);

                oAltaUsuario usuario = jss.Deserialize<oAltaUsuario>(SJson);
                string sQry = string.Format("Exec PAREGISTRARUSUARIO {0},'{1}','{2}','','{3}','{4}',{5},{6},0,1,'{7}'"
                    , 0, usuario.nombre, usuario.usuario,
                    usuario.celular, usuario.mailUsuario, usuario.tipoUsuario, usuario.cliente, usuario.funciones);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");

                resp.estatus = 0;
                resp.mensaje = "Registrado";

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegenerarEntregable", JsonConvert.SerializeObject(resp));
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;

                return resp;
            }

            return resp;
        }



        public objCorreos FnRegresarCorreos(int cliente)
        {
            objCorreos resp = new objCorreos();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnObtenerFunciones", "" + cliente);


                resp.correos = new List<objCorreo>();
                string sQry = string.Format("Exec PCREGRESARCORREOS {0}", cliente);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo == null || dtInfo.Rows.Count == 0)
                    throw new Exception("Ocurrio un error al obtener la información para generar el entregable");


                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.correos.Add(new objCorreo() { correo = row[0].ToString() });
                }
                resp.estatus = 0;
                resp.mensaje = "Éxito";

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;

                return resp;
            }
            return resp;


        }

        public objRespGral FnGuardarDatoIndividual(int tipo, string dato, string valor, string folio)
        {
            objRespGral resp = new objRespGral();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnGuardarDatoIndividual", "FOLIO:" + folio +" TIPO :" +tipo + " DATO: " +dato + " VALOR: "+valor );

                string sQry = string.Format("Exec PAGUARDARDATOINDIVIDUAL {0},'{1}','{2}','{3}'", tipo, dato, valor, folio);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);


                resp.estatus = 0;
                resp.mensaje = "Éxito";

            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;

                return resp;
            }

            return resp;
        }

        public objRespGral FnRegresarEstatus(string folio)
        {
            objRespGral resp = new objRespGral();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnGuardarDatoIndividual", "FOLIO:" + folio);

                string sQry = string.Format("Exec PAREGRESARDICTAMEN '{0}'", folio);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                if (dtInfo.Rows[0]["codeResp"].ToString().Equals("0"))
                {
                    resp.estatus = 0;
                    resp.mensaje = "Folio actualizado";
                }
                else
                {
                    resp.estatus = 99;
                    resp.mensaje = "El folio no fue encontrado en la Base de datos";
                }
            }
            catch (Exception ex)
            {
                resp.estatus = 99;
                resp.mensaje = ex.Message;

                return resp;
            }
            return resp;
        }

        public string FnEnlazarFirmaDocto(string idDocto, string idFirma, string folio)
        {
            string respuesta = "";
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnEnlazarFirmaDocto", "Enlazando firma de folio : " + folio + " con el documento: " + idDocto);

                string sQry = string.Format("Exec PAENLAZARFIRMADOCTO '{0}' , '{1}', '{2}'", idDocto, idFirma, folio);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry); 

                respuesta = dtInfo.Rows[0][0].ToString();
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }

        public string FnRegistrarSms(string folio, string idDocumento, string idFirma)
        {
            string respuesta = "";
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PAINSERTARMENSAJESBEBLE '{0}' , '{1}', '{2}'", folio, idDocumento, idFirma);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                respuesta = dtInfo.Rows[0][0].ToString();
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public string FnObtenerDashboardApi(int idUsuario)
        {
            string respuesta = "";

            objExtDashboardGnrl resp = new objExtDashboardGnrl();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETDASHBOARD {0} ", idUsuario);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                resp.remainingRequest = int.Parse(dtInfo.Rows[0][0].ToString());
                resp.totalRequest = int.Parse(dtInfo.Rows[0][1].ToString());
                resp.totalResponse = int.Parse(dtInfo.Rows[0][2].ToString());
                resp.fallidos = int.Parse(dtInfo.Rows[0][3].ToString());


                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public string FnObtenerRequestsdApi(int idUsuario, int opcion,string fechaInicio,string fechaFin)
        {
            string respuesta = "";

            List<objRequestView> resp = new List<objRequestView>();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETREQUEST {0}, {1}, '{2}','{3}'", idUsuario,opcion,fechaInicio,fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows) {

                    resp.Add(new objRequestView()
                    {
                        idRequest = int.Parse(row[0].ToString()),
                        date = DateTime.Parse(row[1].ToString()),
                        trackingKey = row[2].ToString(),
                        bTransmitter = int.Parse(row[3].ToString()),
                        bReciver = int.Parse(row[4].ToString()),
                        beneficiary = row[5].ToString(),
                        amount = row[6].ToString(),
                        dateRequest = DateTime.Parse(row[7].ToString()),
                        estatus = row[10].ToString()
                    }) ;
                
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }



        public string FnObtenerResponsedApi(int idRequest)
        {     
            string respuesta = "";

            objResponseView resp = new objResponseView();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETRESPONSE {0} ", idRequest);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);


                resp.idResponse = int.Parse(dtInfo.Rows[0][0].ToString());
                resp.rfc = dtInfo.Rows[0][1].ToString();
                resp.name = dtInfo.Rows[0][2].ToString();
                resp.bReciver = dtInfo.Rows[0][3].ToString();
                resp.pdf = dtInfo.Rows[0][4].ToString();


                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public string FnObtenerRequestsRfc(int idUsuario, int opcion, string fechaInicio, string fechaFin)
        {
            string respuesta = "";

            List<objRequestViewRfc> resp = new List<objRequestViewRfc>();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETREQUEST_RFC {0}, {1}, '{2}','{3}'", idUsuario, opcion, fechaInicio, fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.Add(new objRequestViewRfc()
                    {
                        kiidRequest = int.Parse(row[0].ToString()),
                        tiempoPeticion = DateTime.Parse(row[1].ToString()),
                        token = row[2].ToString(),
                        nombres = row[3].ToString(),
                        primerApellido = row[4].ToString(),
                        segundoApellido = row[5].ToString(),
                        fechaNacimiento = row[6].ToString(),
                        estatus = (row[7].ToString() == "1" ? "COMPLETED" : "FAIL")
                    }) ;
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99: "+ e.Message;
            }
            return respuesta;
        }

        public string FnObtenerRequestsCurp(int idUsuario, int opcion, string fechaInicio, string fechaFin)
        {
            string respuesta = "";

            List<objRequestViewCurp> resp = new List<objRequestViewCurp>();
            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                //fechaInicio = fechaInicio.Replace("/", "-");
                //fechaFin = fechaFin.Replace("/", "-");
                string sQry = string.Format("Exec PA_EX_GETREQUEST_CURP {0},{1},'{2}','{3}'", idUsuario, opcion, fechaInicio, fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.Add(new objRequestViewCurp()
                    {
                        kiidRequest = int.Parse(row[0].ToString()),
                        tiempoPeticion = DateTime.Parse(row[1].ToString()),
                        token = row[2].ToString(),
                        curp = row[3].ToString(),                        
                        estatus = (row[4].ToString() == "1" ? "COMPLETED" : "FAIL")
                    });
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99: " + e.Message;
            }
            return respuesta;
        }

        public string FnObtenerRequestsRekognition(int idUsuario, int opcion, string fechaInicio, string fechaFin)
        {
            string respuesta = "";

            List<objRequestViewFaceRekognition> resp = new List<objRequestViewFaceRekognition>();
            try
            {

                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");
                
                string sQry = string.Format("Exec PA_EX_GETREQUEST_FACEREKOGNITION {0},{1},'{2}','{3}'", idUsuario, opcion, fechaInicio, fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.Add(new objRequestViewFaceRekognition()
                    {
                        kiidRequest = int.Parse(row[0].ToString()),
                        tiempoPeticion = DateTime.Parse(row[1].ToString()),
                        token = row[2].ToString(),
                        estatus = (row[3].ToString() == "1" ? "COMPLETED" : "FAIL")
                    });
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99: " + e.Message;
            }
            return respuesta;
        }

        public string FnObtenerRequestsVigIne(int idUsuario, int opcion, string fechaInicio, string fechaFin)
        {
            string respuesta = "";

            List<objRequestViewIne> resp = new List<objRequestViewIne>();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETREQUEST_VIGINE {0},{1},'{2}','{3}'", idUsuario, opcion, fechaInicio, fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.Add(new objRequestViewIne()
                    {
                        kiidRequest = int.Parse(row[0].ToString()),
                        tiempoPeticion = DateTime.Parse(row[1].ToString()),
                        token = row[2].ToString(),
                        estatus = (row[3].ToString() == "1" ? "COMPLETED" : "FAIL")
                    });
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99: " + e.Message;
            }
            return respuesta;
        }

        public string FnObtenerRequestsVigPass(int idUsuario, int opcion, string fechaInicio, string fechaFin)
        {
            string respuesta = "";

            List<objRequestViewPasaporte> resp = new List<objRequestViewPasaporte>();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETREQUEST_VIGPASS {0},{1},'{2}','{3}'", idUsuario, opcion, fechaInicio, fechaFin);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                foreach (DataRow row in dtInfo.Rows)
                {
                    resp.Add(new objRequestViewPasaporte()
                    {
                        kiidRequest = int.Parse(row[0].ToString()),
                        tiempoPeticion = DateTime.Parse(row[1].ToString()),
                        token = row[2].ToString(),
                        estatus = (row[3].ToString() == "1" ? "COMPLETED" : "FAIL")
                    });
                }
                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99: " + e.Message;
            }
            return respuesta;
        }

        public string FnObtenerGraficaRequest(int idUsuario)
        {
            string respuesta = "";

            objGraficaRequest resp = new objGraficaRequest();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EXT_GETGRAFICA {0} ", idUsuario);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                resp.catorce = dtInfo.Rows[0][0].ToString();
                resp.trece = dtInfo.Rows[0][1].ToString();
                resp.doce = dtInfo.Rows[0][2].ToString();
                resp.once = dtInfo.Rows[0][3].ToString();
                resp.diez = dtInfo.Rows[0][4].ToString();
                resp.nueve = dtInfo.Rows[0][5].ToString();
                resp.ocho = dtInfo.Rows[0][6].ToString();
                resp.siete = dtInfo.Rows[0][7].ToString();
                resp.seis = dtInfo.Rows[0][8].ToString();
                resp.cinco = dtInfo.Rows[0][9].ToString();
                resp.cuatro = dtInfo.Rows[0][10].ToString();
                resp.tres = dtInfo.Rows[0][11].ToString();
                resp.dos = dtInfo.Rows[0][12].ToString();
                resp.uno = dtInfo.Rows[0][13].ToString();
                resp.hoy = dtInfo.Rows[0][14].ToString();

                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public string FnOtenerDatosUsuarioApi(int idUsuario)
        {
            string respuesta = "";

            objUsuarioApiGral resp = new objUsuarioApiGral();
            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EXT_GETDATOSUSUARIO {0} ", idUsuario);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                resp.userAccess = dtInfo.Rows[0][0].ToString();
                resp.passAccess = dtInfo.Rows[0][1].ToString();
                resp.snombre = dtInfo.Rows[0][2].ToString();
                resp.smail = dtInfo.Rows[0][3].ToString();
                resp.scelular = dtInfo.Rows[0][3].ToString();


                

                respuesta = JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }



        public string FnGuardarDatosUsuarioApi(int idUsuario,string userApi, string passApi, string name, string mail, string phone)
        {
            string respuesta = "";

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EXT_SETDATOSUSUARIO {0}, '{1}', '{2}', '{3}', '{4}', '{5}'", idUsuario, userApi, passApi, name, mail,phone);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                respuesta = dtInfo.Rows[0][0].ToString();

            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public string FnObtenerDatosUsuariosApi()
        {
            string respuesta = "";

            List<objDataUserApiGral> users = new List<objDataUserApiGral>();

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EX_GETINFORMACIONGENERAL");
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);

                
                foreach (DataRow row in dtInfo.Rows) {

                    users.Add(new objDataUserApiGral()
                    {

                        usuario = row[0].ToString(),
                        empresa = row[1].ToString(),
                        estatus = row[2].ToString(),
                        kiidUser = int.Parse(row[3].ToString())
                    });

                    respuesta = JsonConvert.SerializeObject(users);
                               
                }
                
            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


        public List<string> FnObteneRB64DoctoIlegible(int idTRack)
        {
            List<string> respuesta = new List<string>();           

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec [PA_EXT_GETDOCUMENT] {0}",idTRack);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);


                respuesta.Add(dtInfo.Rows[0][0].ToString());
                respuesta.Add(dtInfo.Rows[0][1].ToString());
            }
            catch (Exception e)
            {
                respuesta[0] = "99";
            }
            return respuesta;
        }


        public string saveUnreadableName(int idTRack,string nombres, string primerApellido,string segundoApellido)
        {
            string respuesta = "";

            try
            {
                if (FnSeguridad.ObtenerValor("bDebug").ToString() == "1")
                    (new FnConxBD()).RegistraError("FnRegistrarMensaje", "Registro de sms");

                string sQry = string.Format("Exec PA_EXT_SETNAMEFROMIMAGE {0},'{1}','{2}','{3}'", idTRack,nombres,primerApellido,segundoApellido);
                DataTable dtInfo = (new FnConxBD()).ObtenerTabla(sQry);


                respuesta = dtInfo.Rows[0][0].ToString();

            }
            catch (Exception e)
            {
                respuesta = "99";
            }
            return respuesta;
        }


    }
}

