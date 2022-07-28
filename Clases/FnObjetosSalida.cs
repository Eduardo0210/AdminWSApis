using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSSaityCore.Clases
{

    public class objDataUserApiGral
    {
       public string usuario { get; set; }
        public string empresa { get; set; }
        public string estatus { get; set; }
        public int kiidUser { get; set; }

    }

    public class objUsuarioApiGral
    { 
        public string userAccess { get; set; }
        public string passAccess { get; set; }
        public string snombre { get; set; }
        public string smail { get; set; }
        public string scelular { get; set; }

    }

        public class objGraficaRequest
    {

        public string hoy  { get; set; }
        public string uno { get; set; }
        public string dos { get; set; }
        public string tres { get; set; }
        public string cuatro { get; set; }
        public string cinco { get; set; }
        public string seis { get; set; }
        public string siete { get; set; }
        public string ocho { get; set; }
        public string nueve { get; set; }
        public string diez { get; set; }
        public string once { get; set; }
        public string doce { get; set; }
        public string trece { get; set; }
        public string catorce { get; set; }

    }
    public class objResponseView { 
        
        public int idResponse { get; set; }
        public string rfc { get; set; }
        public string name { get; set; }
        public string bReciver { get; set; }

        public string pdf { get; set; }

    }


    public class objRequestView
    {
        public int idRequest { get; set; }
        public DateTime date { get; set; }
        public string trackingKey { get; set; }
        public int bTransmitter { get; set; }
        public int bReciver { get; set; }
        public string beneficiary { get; set; }
        public string amount { get; set; }
        public DateTime dateRequest { get; set; }
        public string estatus { get; set; }
    }

    public class objRequestViewRfc 
    {
        public int kiidRequest { get; set; }
        public DateTime tiempoPeticion { get; set; }
        public string token { get; set; }
        public string nombres { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string fechaNacimiento { get; set; }       
        public string estatus { get; set; }
    }

    public class objRequestViewCurp
    {
        public int kiidRequest { get; set; }
        public DateTime tiempoPeticion { get; set; }
        public string token { get; set; }
        public string curp { get; set; }
        public string estatus { get; set; }
 
    }

    public class objRequestViewFaceRekognition
    {
        public int kiidRequest { get; set; }
        public DateTime tiempoPeticion { get; set; }
        public string token { get; set; }
        public string estatus { get; set; }
    }

    public class objRequestViewIne
    {
        public int kiidRequest { get; set; }
        public DateTime tiempoPeticion { get; set; }
        public string token { get; set; }
        public string estatus { get; set; }
    }


    public class objRequestViewPasaporte 
    {
        public int kiidRequest { get; set; }
        public DateTime tiempoPeticion { get; set; }
        public string token { get; set; }
        public string estatus { get; set; }
    }


    public class objExtDashboardGnrl { 
        
        public int remainingRequest { get; set; }
        public int totalRequest { get; set; }
        public int totalResponse  { get; set; }
        public int fallidos { get; set; }
    }

    public class objReportes : objRespGral
    {
        public List<string> columnas { get; set; }
        public string datos { get; set; }
    }

    public class objFunciones
    {
        public List<objFuncion> funciones { get; set; }
    }

    public class objFuncion
    {
        public int kiidFuncion { get; set; }
        public string funcion { get; set; }
    }

    public class objRespGral
    {
        public int estatus { get; set; }
        public string mensaje { get; set; }
    }


    public class objCorreos : objRespGral
    {
        public List<objCorreo> correos { get; set; }
    }

    public class objCorreo
    {
        public string correo { get; set; }
    }

    public class objTienda : objRespGral
    {
        public string stienda { get; set; }
        public string kiidtienda { get; set; }
    }

    public class clsFacultades
    {
        public string key { get; set; }
        public string funcion { get; set; }
        public string padre { get; set; }
    }




    public class clsLogin : objRespGral
    {
        public int idusuario { get; set; }
        public int idcliente { get; set; }
        public string usuario { get; set; }
        public int clientepadre { get; set; }
        public List<clsFacultades> facultades { get; set; }
        public int idClienteEspecifico { get; set; }
    }

    public class objUser : objRespGral
    {
        public int idusuario { get; set; }
    }

    public class objCodigoPostal : objRespGral
    {
        public string municipio { get; set; }
        public string estado { get; set; }
        public List<string> colonias { get; set; }
    }

    public class objAltaCliente : objRespGral
    {
        public string folio { get; set; }
        public string remesa { get; set; }
    }

    public class objZip : objRespGral
    {
        public string pathfile { get; set; }
    }

    public class objAltaMasivo : objRespGral
    {
        public string folioini { get; set; }
        public string foliofin { get; set; }
        public string remesa { get; set; }
    }

    public class objGeolocalizacion : objRespGral
    {
        public string latitud { get; set; }
        public string longitud { get; set; }
    }

    public class objFolioReg
    {
        public int idfolio { get; set; }
        public string folio { get; set; }
        public string nombre { get; set; }
        public string celular { get; set; }
        public string remesa { get; set; }
        public string folioint { get; set; }
        public string fecalta { get; set; }
        public string fectermino { get; set; }
        public string fecaviso { get; set; }
        public string fecultsms { get; set; }
        public string observac { get; set; }
        public string urlentregable { get; set; }
        public string estatus { get; set; }
        public string cliente { get; set; }
        public string usuarioAlta { get; set; }
        public string alta { get; set; }
        public string tienda { get; set; }
    }

    public class objFolios : objRespGral
    {
        public List<objFolioReg> folios { get; set; }
    }

    public class objDictaminados : objRespGral
    {
        public List<clsRepDictaminacion> folios { get; set; }
    }

    public class clsRepDictaminacion
    {

        public string cliente { get; set; }
        public int positivos { get; set; }
        public int negativos { get; set; }
        public int vencimientoVigencia { get; set; }
        public int telefonoIncorrecto { get; set; }
        public int otrasCausas { get; set; }

    }

    public class objConfDirecciones
    {
        public int iddireccion { get; set; }
        public string direccion { get; set; }
    }

    public class objTiendas
    {
        public int idtienda { get; set; }
        public string tienda { get; set; }
    }

    public class objConfDoctos
    {
        public int iddocumento { get; set; }
        public string documento { get; set; }
    }

    public class objConfAltaSol : objRespGral
    {
        public List<objConfDirecciones> direcciones { get; set; }
        public List<objTiendas> tiendas { get; set; }
        public List<objConfDoctos> documentos { get; set; }
    }

    public class objEsquemas
    {
        public int idesquema { get; set; }
        public string esquema { get; set; }
    }

    public class objCliente
    {
        public int idcliente { get; set; }
        public string cliente { get; set; }
    }

    public class objCatEsquemas : objRespGral
    {
        public List<objEsquemas> esquemas { get; set; }
    }

    public class objCatClientes : objRespGral
    {
        public List<objCliente> clientes { get; set; }
    }

    public class objDocumentoBase
    {
        public string documento { get; set; }
        public int benviado { get; set; }
    }

    public class objOtroCampo
    {
        public string descrip { get; set; }
        public string valor { get; set; }
    }


    public class objInfoFolio : objRespGral
    {
        public int idfolio { get; set; }
        public string folio { get; set; }
        public string nombre { get; set; }
        public string apellidop { get; set; }
        public string apellidom { get; set; }
        public string tel { get; set; }
        public string mail { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string fechaini { get; set; }
        public string fechacie { get; set; }
        public string resultado { get; set; }
        public string causa { get; set; }
        public string claveine { get; set; }
        public string folioint { get; set; }
        public string fecalta { get; set; }
        public string fecultsms { get; set; }
        public string fecultact { get; set; }
        public string estatusfolio { get; set; }
        public int porcavance { get; set; }
        public string nomcompleto { get; set; }
        public string cliente { get; set; }
        public List<objDocumentoBase> documentos { get; set; }
        public int bdictamen { get; set; }
        public int idesquema { get; set; }
        public List<objOtroCampo> otrosdatos { get; set; }

    }

    public class objEvento
    {
        public string fechora { get; set; }
        public string operacion { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string urlfoto { get; set; }
        public int bcoincide { get; set; }
        public int tipo { get; set; }
        public int idreferencia { get; set; }
    }

    public class objHistorial : objRespGral
    {
        public List<objEvento> eventos { get; set; }
    }
    public class objDirFolio
    {
        public int idLugar { get; set; }
        public string lugar { get; set; }
        public string calle { get; set; }
        public string numExt { get; set; }
        public string numInt { get; set; }
        public string colonia { get; set; }
        public string ciudad { get; set; }
        public string estado { get; set; }
        public string cp { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string direccion { get; set; }
        public string longUser { get; set; }
        public string latUser { get; set; }
        public string estatus { get; set; }
        public int bmisma { get; set; }
    }

    public class objDireccionesFolio : objRespGral
    {
        public List<objDirFolio> direcciones { get; set; }
    }

    public class objDoctoFolio
    {
        public int iddocto { get; set; }
        public int idfoldocto { get; set; }
        public string documento { get; set; }
        public string lugar { get; set; }
        public string estatus { get; set; }
        public string origen { get; set; }
        public string ubicesp { get; set; }
        public int bsoladic { get; set; }
        public string valor { get; set; }
        public int numdoctos { get; set; }
        public int tipo { get; set; }
    }

    public class objDocumentosFolio : objRespGral
    {
        public List<objDoctoFolio> documentos { get; set; }
    }

    public class objMarcaMapa
    {
        public string referencia { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public int tipo { get; set; }
    }

    public class objMarcasFolio : objRespGral
    {
        public List<objMarcaMapa> marcas { get; set; }
    }

    public class objCatalogo
    {
        public int valor { get; set; }
        public string descripcion { get; set; }
    }

    public class objValoresCat : objRespGral
    {
        public List<objCatalogo> items { get; set; }
    }

    public class objVerificacion
    {
        public int idverificacion { get; set; }
        public string verificacion { get; set; }
        public int check { get; set; }
    }

    public class objCampo
    {
        public int idcampo { get; set; }
        public string campo { get; set; }
        public string valor { get; set; }
    }

    public class objDetDocumento : objRespGral
    {
        public string documento { get; set; }
        public int iddocto { get; set; }
        public string estatusdoc { get; set; }
        public string fecenvio { get; set; }
        public string tipo { get; set; }
        public string lugenvesp { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string direcenv { get; set; }
        public string envioesperado { get; set; }
        public int numdoctos { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string img3 { get; set; }
        public string img4 { get; set; }
        public string img5 { get; set; }
        public string img6 { get; set; }
        public string img7 { get; set; }
        public string img8 { get; set; }
        public string img9 { get; set; }
        public string img10 { get; set; }
        public string img11 { get; set; }
        public string img12 { get; set; }
        public string img13 { get; set; }
        public string img14 { get; set; }
        public string img15 { get; set; }
        public string img16 { get; set; }
        public string img17 { get; set; }
        public string img18 { get; set; }
        public string img19 { get; set; }
        public string img20 { get; set; }
        public int badicional { get; set; }
        public List<objVerificacion> validaciones { get; set; }
        public List<objCampo> campos { get; set; }
        public int brechazar { get; set; }
        public int gpsFake { get; set; }
    }
}