using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;
using WSSaityCore.Formatos;
using System.Drawing;
using System.Net;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;
using CrystalDecisions.Shared;

namespace WSSaityCore.Clases
{
    public class FnEntregables
    {
        public string GenerarEntregableZIP(DataTable dtInfo, string sFolio)
        {
            string sPathFile = string.Format("{0}/{1}/{2}.zip", FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("CarpetaEnt"), sFolio);

            try
            {
                if (File.Exists(sPathFile))
                    File.Delete(sPathFile);
            }
            catch (Exception ex)
            {
                (new FnConxBD()).RegistraError("GenerarEntregableZIP", ex.Message);
                sPathFile = string.Format("{0}/{1}/{2}_{3:yyyyMMddHHmmss}.zip", FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("CarpetaEnt"), sFolio, DateTime.Now);
            }

            (new FnConxBD()).RegistraError("GenerarEntregableZIP", sPathFile);

            FileInfo fl = new FileInfo(sPathFile);
            FileStream fzip = fl.Open(FileMode.OpenOrCreate);
            ZipArchive zfile = new ZipArchive(fzip, ZipArchiveMode.Update);

            string js = JsonConvert.SerializeObject(dtInfo);
            (new FnConxBD()).RegistraError("GenerarEntregableZIP", js);

            try
            {

                foreach (DataRow dr in dtInfo.Rows)
                {
                    for (int i = 1; i <= Convert.ToInt32(dr["iNumDoctos"]); i++)
                    {
                        FileInfo sourceFile = new FileInfo(string.Format("{0}/{1}", FnConxBD.ObtenerValor("PathFisico"), dr[string.Format("sUrlFoto{0}", i)]));

                        (new FnConxBD()).RegistraError("GenerarEntregableZIP", sourceFile.Name);

                        FileStream sourceStream = sourceFile.OpenRead();
                        ZipArchiveEntry newEntry = zfile.CreateEntry(sourceFile.Name);
                        Stream stream = newEntry.Open();
                        sourceStream.CopyTo(stream);
                        stream.Close();
                        sourceStream.Close();
                    }
                }
                zfile.Dispose();
                fzip.Close();
                return sPathFile;
            }
            catch (Exception ex)
            {
                (new FnConxBD()).RegistraError("GenerarEntregableZIP", ex.Message);
                return "";
            }


        }

        /*public string GenerarEntregablePDF(DataTable dtInfo, string sFolio)
        {
            dtsDatosFolio dts = new dtsDatosFolio();

            try
            {
                    
                #region Datos Generales
                DataRow dr = dtInfo.Rows[0];

                DataTable dtG = dts.Tables["dtDatosGral"];
                dtG.Rows.Add();
                dtG.Rows[0]["sfolio"] = dr["sfolio"];
                dtG.Rows[0]["sfolioint"] = dr["sfolioint"];
                dtG.Rows[0]["snombre"] = dr["snombre"];
                dtG.Rows[0]["sapellidop"] = dr["sapellidop"];
                dtG.Rows[0]["sapellidom"] = dr["sapellidom"];
                dtG.Rows[0]["fecsolicitud"] = dr["fecsolicitud"];
                dtG.Rows[0]["fectermino"] = dr["fectermino"];
                dtG.Rows[0]["resultado"] = dr["resultado"];
                dtG.Rows[0]["causa"] = dr["causa"];
                dtG.Rows[0]["cliente"] = dr["cliente"];
                dtG.Rows[0]["tienda"] = dr["tienda"];
                dtG.Rows[0]["remesa"] = dr["remesa"];
                dtG.Rows[0]["telefono"] = dr["telefono"];
                dtG.Rows[0]["fecdictamen"] = dr["fecdictamen"];
                dtG.Rows[0]["horadictamen"] = dr["horadictamen"];


                DataTable dtDir = FnConxBD.ConvertXmlToDataTable(dr["direcciones"].ToString());
                DataTable dtValidar = FnConxBD.ConvertXmlToDataTable(dr["validaciones"].ToString());
                DataTable dtValores = FnConxBD.ConvertXmlToDataTable(dr["valores"].ToString());
                DataTable dtImagen = FnConxBD.ConvertXmlToDataTable(dr["imagenes"].ToString());
                DataTable dtPdfs = FnConxBD.ConvertXmlToDataTable(dr["pdfs"].ToString());
                DataTable dtDatos = FnConxBD.ConvertXmlToDataTable(dr["datosadic"].ToString());
                DataTable dtMapa = FnConxBD.ConvertXmlToDataTable(dr["mapas"].ToString());

                dtG.Rows[0]["bdireccion"] = dtDir != null && dtDir.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bvalidacion"] = dtValidar != null && dtValidar.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bvalores"] = dtValores != null && dtValores.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bimagenes"] = dtImagen != null && dtImagen.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bdatosadic"] = dtDatos != null && dtDatos.Rows.Count > 0 ? 1 : 0;

                dtG.Rows[0]["logo"] = File.ReadAllBytes(FnConxBD.ObtenerValor("PathLogo"));
                dtG.Rows[0]["observaciones"] = dr["observacdic"];
                dtG.Rows[0]["dictaminenpor"] = dr["dictamenpor"];
                //dtG.Rows[0]["firmadictamen"] = dr[""];
                dtG.Rows[0]["correo"] = dr["correo"];


                #endregion

                #region Agrega Valores Adicionales
                if (dtValores != null && dtValores.Rows.Count > 0)
                {
                    DataTable dtVA = dts.Tables["dtValores"];

                    foreach (DataRow d in dtValores.Rows)
                    {
                        dtVA.Rows.Add(d["solicitud"], d["valor"], dr["sfolio"]);
                    }

                }
                #endregion

                #region Agrega Validaciones
                if (dtValidar != null && dtValidar.Rows.Count > 0)
                {
                    DataTable dtV = dts.Tables["dtValidaciones"];

                    foreach (DataRow d in dtValidar.Rows)
                    {
                        dtV.Rows.Add(dr["sfolio"], d["descrip"], d["valor"]);
                    }

                }
                #endregion

                #region Otros Valores
                if (dtDatos != null && dtDatos.Rows.Count > 0)
                {
                    DataTable dtDat = dts.Tables["dtCampos"];

                    foreach (DataRow d in dtDatos.Rows)
                    {
                        dtDat.Rows.Add(dr["sfolio"], d["scampo"], d["svalor"]);
                    }

                }
                #endregion

                #region Direcciones
                if (dtDir != null && dtDir.Rows.Count > 0)
                {
                    DataTable dtDr = dts.Tables["dtDirecciones"];

                    foreach (DataRow d in dtDir.Rows)
                    {
                        dtDr.Rows.Add(dr["sfolio"], d["slugar"], d["scalle"], d["snumext"], d["snumint"], d["scolonia"], d["sciudad"], d["sestado"], d["scp"], d["slatitud"], d["slongitud"], d["sdireccion"], d["slatituduser"], d["slongituduser"]);
                    }

                }
                #endregion

                #region Imagenes

                if (dtImagen != null && dtImagen.Rows.Count > 0)
                {
                    DataTable dtImg = dts.Tables["dtImagenes"];
                        
                    foreach (DataRow d in dtImagen.Rows)
                    {
                        for (int i = 1; i <= Convert.ToInt32(d["iNumDoctos"]); i++)
                        {
                            dtImg.Rows.Add(string.Format("{0}{1}", d["documento"], Convert.ToInt32(d["iNumDoctos"]) == 1 ? "" : " " + i.ToString("00")),  File.ReadAllBytes(string.Format("{0}\\{1}", "C:\\inetpub\\wwwroot\\", d[string.Format("sUrlFoto{0}", i)])), d["sdireccion"], d["slatitud"], d["slongitud"], dr["sfolio"]);
                        }
                    }

                }

                #endregion

                #region Mapa

                string sMark = "";
                if (dtMapa != null)
                {
                    foreach (DataRow dMap in dtMapa.Rows)
                    {
                        sMark += string.Format("&markers=color:{0}%7C{1},{2}",
                            (dMap["tipo"].ToString() == "1" ? "blue" : dMap["tipo"].ToString() == "2" ? "red" : "green"), dMap["slatitud"], dMap["slongitud"]);
                    }


                }

                string sUrlMap = string.Format("https://maps.googleapis.com/maps/api/staticmap?size=900x500&maptype=roadmap{0}&key=AIzaSyBCrjOdhxDgpD-N3PAxYCZkIC3JCpx2qGY", sMark);

                WebClient client = new WebClient();
                Stream stream = client.OpenRead(sUrlMap);
                Bitmap bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
                dtG.Rows[0]["mapareferencia"] = FnGenerales.ImageToByte(bitmap);

                #endregion                

                #region Generar PDF

                string sPathFile = string.Format("{0}\\{1}\\{2}.pdf", "C:\\inetpub\\wwwroot\\QA", FnConxBD.ObtenerValor("CarpetaEnt"), sFolio);
               
                try
                {
                    if (File.Exists(sPathFile))
                        File.Delete(sPathFile);

                }
                catch (Exception ex)
                {
                    (new FnConxBD()).RegistraError("GenerarEntregablePDF Exist File", ex.Message);
                    sPathFile = string.Format(sPathFile);
                }

                (new FnConxBD()).RegistraError("GenerarEntregablePDF Path File", sPathFile);

                rptEntregable rpt = new rptEntregable();
                rpt.SetDataSource(dts);

                if (rpt.Subreports!=null && rpt.Subreports.Count!=0)
                {
                    for(int i=0; i<rpt.Subreports.Count; i++)
                    {
                        rpt.Subreports[i].SetDataSource(dts);
                    }
                }
              
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, sPathFile);
                rpt.Close();
                rpt.Dispose();              

                #endregion

                #region Adjuntar PDF

                List<byte[]> lPdf = new List<byte[]>();

                if (dtPdfs != null && dtPdfs.Rows.Count > 0)
                {
                    (new FnConxBD()).RegistraError("GenerarEntregablePDF Integra PDF", sPathFile);
          
                    lPdf.Add(File.ReadAllBytes(sPathFile));

                    foreach (DataRow d in dtPdfs.Rows)
                    {
                        MergeText(string.Format("{0}{1}", FnConxBD.ObtenerValor("PathFisico"), d["sUrlFoto1"]), d["sUrlFoto1"].ToString(), string.Format("Direccion: {0} Latitud: {1} Longitud: {2}", d["sdireccion"], d["slatitud"],d["slongitud"]));
                        lPdf.Add(File.ReadAllBytes(string.Format("{0}/{1}", FnConxBD.ObtenerValor("PathFisico"), d["sUrlFoto1"])));
                    }

                    try
                    {
                        File.WriteAllBytes(sPathFile, MergeFiles(lPdf));
                    }
                    catch (Exception ex)
                    {

                        (new FnConxBD()).RegistraError("GenerarEntregablePDF Integra PDF", ex.Message);
                    }
                }

                #endregion

                return sPathFile;
            }
            catch (Exception ex)
            {
                (new FnConxBD()).RegistraError("GenerarEntregablePDF Gen PDF", ex.Message);
                return "";
            }

         
        }*/

        public string GenerarEntregablePDF(DataTable dtInfo, string sFolio)// PRODUCCION
        {
            dtsDatosFolio dts = new dtsDatosFolio();

            try
            {

                #region Datos Generales
                DataRow dr = dtInfo.Rows[0];

                DataTable dtG = dts.Tables["dtDatosGral"];
                dtG.Rows.Add();
                dtG.Rows[0]["sfolio"] = dr["sfolio"];
                dtG.Rows[0]["sfolioint"] = dr["sfolioint"];
                dtG.Rows[0]["snombre"] = dr["snombre"];
                dtG.Rows[0]["sapellidop"] = dr["sapellidop"];
                dtG.Rows[0]["sapellidom"] = dr["sapellidom"];
                dtG.Rows[0]["fecsolicitud"] = dr["fecsolicitud"];
                dtG.Rows[0]["fectermino"] = dr["fectermino"];
                dtG.Rows[0]["resultado"] = dr["resultado"];
                dtG.Rows[0]["causa"] = dr["causa"];
                dtG.Rows[0]["cliente"] = dr["cliente"];
                dtG.Rows[0]["tienda"] = dr["tienda"];
                dtG.Rows[0]["remesa"] = dr["remesa"];
                dtG.Rows[0]["telefono"] = dr["telefono"];
                dtG.Rows[0]["fecdictamen"] = dr["fecdictamen"];
                dtG.Rows[0]["horadictamen"] = dr["horadictamen"];


                DataTable dtDir = FnConxBD.ConvertXmlToDataTable(dr["direcciones"].ToString());
                DataTable dtValidar = FnConxBD.ConvertXmlToDataTable(dr["validaciones"].ToString());
                DataTable dtValores = FnConxBD.ConvertXmlToDataTable(dr["valores"].ToString());
                DataTable dtImagen = FnConxBD.ConvertXmlToDataTable(dr["imagenes"].ToString());
                DataTable dtPdfs = FnConxBD.ConvertXmlToDataTable(dr["pdfs"].ToString());
                DataTable dtDatos = FnConxBD.ConvertXmlToDataTable(dr["datosadic"].ToString());
                DataTable dtMapa = FnConxBD.ConvertXmlToDataTable(dr["mapas"].ToString());

                dtG.Rows[0]["bdireccion"] = dtDir != null && dtDir.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bvalidacion"] = dtValidar != null && dtValidar.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bvalores"] = dtValores != null && dtValores.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bimagenes"] = dtImagen != null && dtImagen.Rows.Count > 0 ? 1 : 0;
                dtG.Rows[0]["bdatosadic"] = dtDatos != null && dtDatos.Rows.Count > 0 ? 1 : 0;
                if (dr["cliente"].Equals("RESTRUCTURACION"))
                {
                    dtG.Rows[0]["logo"] = File.ReadAllBytes(FnConxBD.ObtenerValor("PathLogoFincomun"));
                }
                else
                {

                    dtG.Rows[0]["logo"] = File.ReadAllBytes(FnConxBD.ObtenerValor("PathLogo"));
                }
                dtG.Rows[0]["observaciones"] = dr["observacdic"];
                dtG.Rows[0]["dictaminenpor"] = dr["dictamenpor"];
                //dtG.Rows[0]["firmadictamen"] = dr[""];
                dtG.Rows[0]["correo"] = dr["correo"];


                #endregion

                #region Agrega Valores Adicionales
                if (dtValores != null && dtValores.Rows.Count > 0)
                {
                    DataTable dtVA = dts.Tables["dtValores"];

                    foreach (DataRow d in dtValores.Rows)
                    {
                        dtVA.Rows.Add(d["solicitud"], d["valor"], dr["sfolio"]);
                    }

                }
                #endregion

                #region Agrega Validaciones
                if (dtValidar != null && dtValidar.Rows.Count > 0)
                {
                    DataTable dtV = dts.Tables["dtValidaciones"];

                    foreach (DataRow d in dtValidar.Rows)
                    {
                        dtV.Rows.Add(dr["sfolio"], d["descrip"], d["valor"]);
                    }

                }
                #endregion

                #region Otros Valores
                if (dtDatos != null && dtDatos.Rows.Count > 0)
                {
                    DataTable dtDat = dts.Tables["dtCampos"];

                    foreach (DataRow d in dtDatos.Rows)
                    {
                        if (dr["cliente"].Equals("VERIFICACION DE DATOS"))
                        {
                            if (!d["scampo"].Equals("EJECUTIVO"))
                            {

                                dtDat.Rows.Add(dr["sfolio"], d["scampo"], d["svalor"]);
                            }
                        }
                        else
                        {
                            dtDat.Rows.Add(dr["sfolio"], d["scampo"], d["svalor"]);
                        }
                    }

                }
                #endregion

                #region Direcciones
                if (dtDir != null && dtDir.Rows.Count > 0)
                {
                    DataTable dtDr = dts.Tables["dtDirecciones"];

                    foreach (DataRow d in dtDir.Rows)
                    {
                        dtDr.Rows.Add(dr["sfolio"], d["slugar"], d["scalle"], d["snumext"], d["snumint"], d["scolonia"], d["sciudad"], d["sestado"], d["scp"], d["slatitud"], d["slongitud"], d["sdireccion"], d["slatituduser"], d["slongituduser"]);
                    }

                }
                #endregion

                #region Imagenes

                if (dtImagen != null && dtImagen.Rows.Count > 0)
                {
                    DataTable dtImg = dts.Tables["dtImagenes"];

                    foreach (DataRow d in dtImagen.Rows)
                    {
                        for (int i = 1; i <= Convert.ToInt32(d["iNumDoctos"]); i++)
                        {
                            dtImg.Rows.Add(string.Format("{0}{1}", d["documento"], Convert.ToInt32(d["iNumDoctos"]) == 1 ? "" : " " + i.ToString("00")), File.ReadAllBytes(string.Format("{0}/{1}", FnConxBD.ObtenerValor("PathFisico"), d[string.Format("sUrlFoto{0}", i)])), d["sdireccion"], d["slatitud"], d["slongitud"], dr["sfolio"]);
                        }
                    }

                }

                #endregion

                #region Mapa

                string sMark = "";
                if (dtMapa != null)
                {
                    foreach (DataRow dMap in dtMapa.Rows)
                    {
                        sMark += string.Format("&markers=color:{0}%7C{1},{2}",
                            (dMap["tipo"].ToString() == "1" ? "blue" : dMap["tipo"].ToString() == "2" ? "red" : "green"), dMap["slatitud"], dMap["slongitud"]);
                    }


                }

                string sUrlMap = string.Format("https://maps.googleapis.com/maps/api/staticmap?size=900x500&maptype=roadmap{0}&key=AIzaSyBCrjOdhxDgpD-N3PAxYCZkIC3JCpx2qGY", sMark);

                WebClient client = new WebClient();
                Stream stream = client.OpenRead(sUrlMap);
                Bitmap bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
                dtG.Rows[0]["mapareferencia"] = FnGenerales.ImageToByte(bitmap);

                #endregion

                #region Generar PDF

                string sPathFile = string.Format("{0}/{1}/{2}.pdf", FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("CarpetaEnt"), sFolio);

                try
                {
                    if (File.Exists(sPathFile))
                        File.Delete(sPathFile);

                }
                catch (Exception ex)
                {
                    (new FnConxBD()).RegistraError("GenerarEntregablePDF Exist File", ex.Message);
                    sPathFile = string.Format("{0}/{1}/{2}_{3:yyyyMMddHHmmss}.pdf", FnConxBD.ObtenerValor("PathFisico"), FnConxBD.ObtenerValor("CarpetaEnt"), sFolio, DateTime.Now);
                }

                (new FnConxBD()).RegistraError("GenerarEntregablePDF Path File", sPathFile);

                rptEntregable rpt = new rptEntregable();
                rpt.SetDataSource(dts);

                if (rpt.Subreports != null && rpt.Subreports.Count != 0)
                {
                    for (int i = 0; i < rpt.Subreports.Count; i++)
                    {
                        rpt.Subreports[i].SetDataSource(dts);
                    }
                }

                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, sPathFile);
                rpt.Close();
                rpt.Dispose();

                #endregion

                #region Adjuntar PDF

                List<byte[]> lPdf = new List<byte[]>();

                if (dtPdfs != null && dtPdfs.Rows.Count > 0)
                {
                    (new FnConxBD()).RegistraError("GenerarEntregablePDF Integra PDF", sPathFile);

                    lPdf.Add(File.ReadAllBytes(sPathFile));

                    foreach (DataRow d in dtPdfs.Rows)
                    {
                        MergeText(string.Format("{0}{1}", FnConxBD.ObtenerValor("PathFisico"), d["sUrlFoto1"]), d["sUrlFoto1"].ToString(), string.Format("Direccion: {0} Latitud: {1} Longitud: {2}", d["sdireccion"], d["slatitud"], d["slongitud"]));
                        lPdf.Add(File.ReadAllBytes(string.Format("{0}/{1}", FnConxBD.ObtenerValor("PathFisico"), d["sUrlFoto1"])));
                    }

                    try
                    {
                        File.WriteAllBytes(sPathFile, MergeFiles(lPdf));
                    }
                    catch (Exception ex)
                    {

                        (new FnConxBD()).RegistraError("GenerarEntregablePDF Integra PDF", ex.Message);
                    }
                }

                #endregion

                return sPathFile;
            }
            catch (Exception ex)
            {
                (new FnConxBD()).RegistraError("GenerarEntregablePDF Gen PDF", ex.Message);
                return "";
            }


        }

        private byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            Document document = new Document();
            using (MemoryStream ms = new MemoryStream())
            {
                PdfCopy copy = new PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Iterate through all pdf documents
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        documentPageCounter++;
                        PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                        PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                        //// Write header
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //new Phrase(string.Format("Archivo {0}", fileCounter)), importedPage.Width / 2, importedPage.Height - 30,
                        //importedPage.Width < importedPage.Height ? 0 : 1);

                        //// Write footer
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase(String.Format("Page {0}", documentPageCounter)), importedPage.Width / 2, 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);

                        pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }


        private void MergeText(string oldData, string url, string texto)
        {
            try
            {
                string oldFile = oldData.Replace("/", "\\");
                (new FnConxBD()).RegistraError("Old Data:", oldFile);

                var arrayFile = url.Split('/');
                string newFile = "C:\\xampp\\htdocs\\saityphp" + arrayFile[0] + "\\" + arrayFile[1] + "\\" + arrayFile[2] + "\\" + arrayFile[3] + "\\new.pdf";

                (new FnConxBD()).RegistraError("Save File:", newFile);
                // open the reader
                PdfReader reader = new PdfReader(oldFile);
                Rectangle size = reader.GetPageSizeWithRotation(1);
                Document document = new Document(size);

                // open the writer
                FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                // the pdf content
                PdfContentByte cb = writer.DirectContent;

                // select the font properties
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetColorFill(BaseColor.RED);
                cb.SetFontAndSize(bf, 8);

                // write the text in the pdf content
                cb.BeginText();
                string text = texto;
                // put the alignment and coordinates here
                cb.ShowTextAligned(1, text, 200, document.Top, 0);
                cb.EndText();


                // create the new page and add it to the pdf
                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);

                // close the streams and voilá the file should be changed :)
                document.Close();
                fs.Close();
                writer.Close();
                reader.Close();

                File.Delete(oldFile);
                File.Move(newFile, oldFile);
            }
            catch (Exception e)
            {
                (new FnConxBD()).RegistraError("Text:", e.Message);
            }

        }

    }
}