using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WSSaityCore.Clases
{
    [DataContract]
    public class oConsultaReporteConsumos : oConsultaReporte
    {
        [DataMember]
        public int tipo { get; set; }
        [DataMember]
        public string anio { get; set; }
        [DataMember]
        public int semestre { get; set; }
        [DataMember]
        public int mes { get; set; }
        [DataMember]
        public int semana { get; set; }


    }

    [DataContract]
    public class oConsultaReporteCifras : oConsultaReporte
    {
        [DataMember]
        public bool remesa { get; set; }

    }

    [DataContract]
    public class oConsultaReporte : oSeguridad
    {
        [DataMember]
        public string fecinicio { get; set; }
        [DataMember]
        public string fecfinal { get; set; }
        [DataMember]

        public int idcliente { get; set; }
        [DataMember]
        public int idesquema { get; set; }

    }


    [DataContract]
    public class oSeguridad
    {
        [DataMember]
        public int idconex { get; set; }
        [DataMember]
        public string fechora { get; set; }
        [DataMember]
        public string token { get; set; }
        [DataMember]
        public int idusuario { get; set; }

    }

    [DataContract]
    public class oLogin : oSeguridad
    {

        [DataMember]
        public string usuario { get; set; }
        [DataMember]
        public string password { get; set; }
        

    }

    [DataContract]
    public class oConsultaCP : oSeguridad
    {

        [DataMember]
        public string codpostal { get; set; }

    }

    [DataContract]
    public class oDatosCliente : oSeguridad
    {

        [DataMember]
        public int idcliente { get; set; }

    }

    [DataContract]
    public class oDireccion
    {
        [DataMember]
        public int iddireccion { get; set; }
        [DataMember]
        public string calle { get; set; }
        [DataMember]
        public string numext { get; set; }
        [DataMember]
        public string numint { get; set; }
        [DataMember]
        public string colonia { get; set; }
        [DataMember]
        public string municipio { get; set; }
        [DataMember]
        public string estado { get; set; }
        [DataMember]
        public string cp { get; set; }
        [DataMember]
        public string pais { get; set; }
        [DataMember]
        public string latitud { get; set; }
        [DataMember]
        public string longitud { get; set; }

    }

    [DataContract]
    public class oDocumetosAC
    {
        [DataMember]
        public int idDocumento { get; set; }
        [DataMember]
        public int solicitado { get; set; }
    }


    [DataContract]
    public class oNumeroTienda : oSeguridad
    {
        [DataMember]
        public string numTienda { get; set; }
    }



    [DataContract]
    public class oDatosPersonales : oSeguridad
    {
        [DataMember]
        public string nombres { get; set; }
        [DataMember]
        public string apaterno { get; set; }
        [DataMember]
        public string amaterno { get; set; }
        [DataMember]
        public string celular { get; set; }
        [DataMember]
        public string correo { get; set; }
        [DataMember]
        public string folioint { get; set; }
        [DataMember]
        public int idtienda { get; set; }
        [DataMember]
        public List<oDireccion> direcciones { get; set; }
        [DataMember]
        public int idesquema { get; set; }
        [DataMember]
        public int valfolio { get; set; }
        [DataMember]
        public Dictionary<string, string> otrosdatos { get; set; }
        [DataMember]
        public List<oDocumetosAC> documentos { get; set; }
        [DataMember]
        public String numeroTienda { get; set; }
        [DataMember]
        public String fechaAlta { get; set; }
        [DataMember]
        public String agencia { get; set; }

    }

    [DataContract]
    public class oDatosMasivos : oSeguridad
    {
        [DataMember]
        public List<oDatosPersonales> datos { get; set; }
        [DataMember]
        public int idesquema { get; set; }
    }


    [DataContract]
    public class oConsultaFolios : oSeguridad
    {
        [DataMember]
        public int opc { get; set; }
        [DataMember]
        public int idcliente { get; set; }
        [DataMember]
        public string sfolio { get; set; }
        [DataMember]
        public string sremesa { get; set; }
        [DataMember]
        public string snombre { get; set; }
        [DataMember]
        public string fecini { get; set; }
        [DataMember]
        public string fecfin { get; set; }
    }

    [DataContract]
    public class oFolio : oSeguridad
    {
        [DataMember]
        public string sfolio { get; set; }
        [DataMember]
        public int ifolio { get; set; }
    }

    [DataContract]
    public class oCatalogo : oSeguridad
    {
        [DataMember]
        public int opc { get; set; }
        [DataMember]
        public string idreferencia { get; set; }
        [DataMember]
        public string sFolio { get; set; }
        [DataMember]
        public string resultado { get; set; }
    }

    [DataContract]
    public class oDictamen : oFolio
    {
        [DataMember]
        public int idresultado { get; set; }
        [DataMember]
        public int idcausa { get; set; }
        [DataMember]
        public string observac { get; set; }
    }

    [DataContract]
    public class oDetDocumento : oSeguridad
    {
        [DataMember]
        public int iddocumento { get; set; }
    }

    [DataContract]
    public class oVerificDocto
    {
        [DataMember]
        public int idverificacion { get; set; }
        [DataMember]
        public int check { get; set; }
        [DataMember]
        public string valor { get; set; }
    }

    [DataContract]
    public class oVerificacion : oSeguridad
    {
        [DataMember]
        public int iddocumento { get; set; }
        [DataMember]
        public List<oVerificDocto> verificaciones { get; set; }
        [DataMember]
        public int opcrechazo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
    }

    [DataContract]
    public class oRecDocto
    {
        [DataMember]
        public int iddocumento { get; set; }
        [DataMember]
        public int opcrechazo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
    }

    [DataContract]
    public class oRechazos : oSeguridad
    {
        [DataMember]
        public string sfolio { get; set; }
        [DataMember]
        public List<oRecDocto> rechazos { get; set; }
    }

    [DataContract]
    public class oRotarImg : oSeguridad
    {
        [DataMember]
        public string sruta { get; set; }
        [DataMember]
        public int opcion { get; set; }
    }

    [DataContract]
    public class oResetPass : oSeguridad
    {
        [DataMember]
        public string smail { get; set; }
    }

    [DataContract]
    public class oTokenPass : oSeguridad
    {
        [DataMember]
        public string tokenpass { get; set; }
    }

    [DataContract]
    public class oCambiPassword : oSeguridad
    {

        [DataMember]
        public string tokenpass { get; set; }
        [DataMember]
        public string password { get; set; }

    }

    [DataContract]
    public class oAltaUsuario : oSeguridad
    {
        [DataMember]
        public string nombre { get; set; }
        [DataMember]
        public string usuario { get; set; }
        [DataMember]
        public string celular { get; set; }
        [DataMember]
        public int tipoUsuario { get; set; }
        [DataMember]
        public string funciones { get; set; }
        [DataMember]
        public int cliente { get; set; }
        [DataMember]
        public string mailCheck { get; set; }
        [DataMember]
        public string mailUsuario { get; set; }

    }
}