using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WSSaityCore.Clases;

namespace WSSaityCore
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFnSaity" in both code and config file together.
    [ServiceContract]
    public interface IFnSaity
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnValidaUsuario/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        clsLogin FnValidaUsuario(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnValidaCodPostal/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objCodigoPostal FnValidaCodPostal(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtConfClienteAltaSol/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objConfAltaSol FnObtConfClienteAltaSol(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRegistraSolicitud/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objAltaCliente FnRegistraSolicitud(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRegistraMasivo/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objAltaMasivo FnRegistraMasivo(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerEsquemas/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objCatEsquemas FnObtenerEsquemas(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerEsquemaReporte/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objCatEsquemas FnObtenerEsquemaReporte(string SJson);


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerClientes/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objCatClientes FnObtenerClientes(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerEsquemasTodos/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objCatEsquemas FnObtenerEsquemasTodos(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnConsultaFolios/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objFolios FnConsultaFolios(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerZip/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objZip FnObtenerZip(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnInfoFolioGeneral/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objInfoFolio FnInfoFolioGeneral(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnInfoFolioHistoria/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objHistorial FnInfoFolioHistoria(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnInfoFolioDirecciones/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objDireccionesFolio FnInfoFolioDirecciones(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnInfoFolioDocumentos/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objDocumentosFolio FnInfoFolioDocumentos(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnInfoFolioMarcasMapa/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objMarcasFolio FnInfoFolioMarcasMapa(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerCatalogo/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objValoresCat FnObtenerCatalogo(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnDictamenFolio/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnDictamenFolio(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRegenerarEntregable/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnRegenerarEntregable(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtDetDocumento/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objDetDocumento FnObtDetDocumento(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnVerificarDocumento/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnVerificarDocumento(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRechazarDocumento/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnRechazarDocumento(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRotarImagen/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnRotarImagen(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReiniciarPassword/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnReiniciarPassword(string SJson);


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnValidaTokenPassword/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objUser FnValidaTokenPassword(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnCambioPassword/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        clsLogin FnCambioPassword(string SJson);


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReporeDictaminados/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objReportes FnReporeDictaminados(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReporteSMSEnviados/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objReportes FnReporteSMSEnviados(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReporeConsumos/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objReportes FnReporeConsumos(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReporeCancelados/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objReportes FnReporeCancelados(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnReporteCifras/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objReportes FnReporteCifras(string SJson);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "TestConection")]
        string TestConection();


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnValidaTienda/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objTienda FnValidaTienda(string SJson);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnBorrarFolio/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnBorrarFolio(string folio);


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnObtenerFunciones/{tipoUsuario}",
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        objFunciones FnObtenerFunciones(int tipoUsuario);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRegistrarUsuario/{SJson}",
        BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        objRespGral FnRegistrarUsuario(string SJson);


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/FnRegresarCorreos/{cliente}",
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        objCorreos FnRegresarCorreos(int cliente);

        [OperationContract]
        objRespGral FnGuardarDatoIndividual(int tipo, string dato, string valor, string folio);

        [OperationContract]
        objRespGral FnRegresarEstatus(string folio);
        [OperationContract]
        string FnRegistrarSms(string folio, string idDocumento, string idFirma);
        [OperationContract]
        string FnEnlazarFirmaDocto(string idDocto, string idFirma, string folio);
        [OperationContract]
        string FnObtenerDashboardApi(int idUsuario);
        [OperationContract]
        string FnObtenerRequestsdApi(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        string FnObtenerResponsedApi(int idRequest);
        [OperationContract]
        string FnObtenerGraficaRequest(int idUsuario);
        [OperationContract]
        string FnOtenerDatosUsuarioApi(int idUsuario);
        [OperationContract]
        string FnGuardarDatosUsuarioApi(int idUsuario, string userApi, string passApi, string name, string mail, string phone);
        [OperationContract]
        string FnObtenerDatosUsuariosApi();
        [OperationContract]
        string FnObtenerRequestsRfc(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        string FnObtenerRequestsCurp(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        string FnObtenerRequestsRekognition(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        string FnObtenerRequestsVigIne(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        string FnObtenerRequestsVigPass(int idUsuario, int opcion, string fechaInicio, string fechaFin);
        [OperationContract]
        List<string> FnObteneRB64DoctoIlegible(int idTRack);
        [OperationContract]
        string saveUnreadableName(int idTRack, string nombres, string primerApellido, string segundoApellido,int kiidEncargado);
    };
}
