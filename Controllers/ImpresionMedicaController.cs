using iTextSharp.text;
using iTextSharp.text.pdf;
using MedToxMVC.Data;
using MedToxMVC.Models.Medicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using MedToxMVC.Models.Consultas;
using MedToxMVC.Models.Odontologias;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MedToxMVC.Helper;
using System.Threading.Tasks;
using MedToxMVC.Models.EnfermeriaModel;

namespace MedToxMVC.Controllers
{
    [Authorize]

    [Authorize(Roles = "Administrador, Medico")]

    public class ImpresionMedicaController : Controller
    {
        float[] widthsTitulosGenerales = new float[] { 1f };
        private DBOperaciones repo;

        public ImpresionMedicaController()
        {
            repo = new DBOperaciones();
        }

        public IActionResult IntegralMedico(int idHistorico)
        {
            var datosRepInt = repo.Getdosparam1<repIntegralImpresionModel>("sp_medicos_reporte_integral_impresion", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRepIntMed = new MemoryStream();

            Document docRepIntMed = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRepIntMed = PdfWriter.GetInstance(docRepIntMed, msRepIntMed);

            string elFolio = datosRepInt.FOLIO;
            //string elFolio = datos.evaluado;
            string elRealizo = datosRepInt.medico;
            string elCedRea = datosRepInt.cedMed;
            string elSuperviso = datosRepInt.supervisor;
            string elCedSup = datosRepInt.cedSup;
            //string elEvaluado = datosRepInt.evaluado;

            string elTitulo = "Reporte Integral de Evaluación";

            pwRepIntMed.PageEvent = HeaderFooterRepInt.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup,  elTitulo);

            docRepIntMed.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
            var fontFirma = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);

            #region emision - revision - codigo
            PdfPCell clRev = new PdfPCell(new Phrase("EMISION", fonEiqueta));
            clRev.BorderWidth = 0;
            clRev.VerticalAlignment = Element.ALIGN_BOTTOM;
            clRev.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi = new PdfPCell(new Phrase("REVISION", fonEiqueta));
            celEmi.BorderWidth = 0;
            celEmi.VerticalAlignment = Element.ALIGN_BOTTOM;
            celEmi.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCod = new PdfPCell(new Phrase("CODIGO", fonEiqueta));
            celCod.BorderWidth = 0;
            celCod.VerticalAlignment = Element.ALIGN_BOTTOM;
            celCod.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi_b = new PdfPCell(new Phrase(DateTime.Now.Year.ToString(), fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/05", fonEiqueta));
            celCod_b.BorderWidth = 0;
            celCod_b.VerticalAlignment = Element.ALIGN_TOP;
            celCod_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celCod_b.BorderWidthBottom = 0.75f;

            PdfPTable tblEmiRevCpd = new PdfPTable(3);
            tblEmiRevCpd.WidthPercentage = 100;
            float[] witdhs = new float[] { 33f, 33f, 33f };
            tblEmiRevCpd.SetWidths(witdhs);

            tblEmiRevCpd.AddCell(clRev);
            tblEmiRevCpd.AddCell(celEmi);
            tblEmiRevCpd.AddCell(celCod);

            tblEmiRevCpd.AddCell(celEmi_b);
            tblEmiRevCpd.AddCell(celRev_b);
            tblEmiRevCpd.AddCell(celCod_b);

            docRepIntMed.Add(tblEmiRevCpd);
            #endregion

            #region fechas
            Paragraph fechaImpresion = new Paragraph();
            fechaImpresion.Add(new Phrase("Código del evaluado: ", fonEiqueta));
            fechaImpresion.Add(Chunk.TABBING);
            fechaImpresion.Add(new Phrase(datosRepInt.codigoevaluado, fontDato));
            fechaImpresion.Add(Chunk.NEWLINE);
            fechaImpresion.Add(new Phrase("Fecha de impresion: ", fonEiqueta));
            fechaImpresion.Add(Chunk.TABBING);
            fechaImpresion.Add(new Phrase(DateTime.Now.Date.ToShortDateString(), fontDato));
            fechaImpresion.Add(Chunk.NEWLINE);
            fechaImpresion.Add(new Phrase("Fecha de ingreso: ", fonEiqueta));
            fechaImpresion.Add(Chunk.TABBING);
            fechaImpresion.Add(new Phrase(datosRepInt.fechaIngreso, fontDato));
            fechaImpresion.Alignment = Element.ALIGN_RIGHT;
            fechaImpresion.Add(Chunk.NEWLINE); fechaImpresion.Add(Chunk.NEWLINE);
            docRepIntMed.Add(fechaImpresion);
            #endregion

            Byte[] _fotoInt = (Byte[])datosRepInt.Picture;
            iTextSharp.text.Image _lafotoint = iTextSharp.text.Image.GetInstance(_fotoInt);
            _lafotoint.ScalePercent(50f);

            Paragraph derecha = new Paragraph();
            //derecha.Alignment = Element.ALIGN_RIGHT;
            _lafotoint.SetAbsolutePosition(510f, 555f);
            derecha.Add(_lafotoint);
            docRepIntMed.Add(derecha);

            #region Datos del Evaluado
            Paragraph datosEval = new Paragraph();
            datosEval.Alignment = Element.ALIGN_LEFT;
            datosEval.Add(new Phrase("Nombre:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.evaluado, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("RFC:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.rfc, fontDato));
            datosEval.Add(Chunk.TABBING); datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase("CURP:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.curp, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Edad:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.edad, fontDato));
            datosEval.Add(Chunk.TABBING); datosEval.Add(Chunk.TABBING); datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase("Sexo:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.sexo, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Dependencia:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.desc_dependencia, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Subdependencia:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.desc_subdep, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Comisión:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.comision, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Lugar de evaluación:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.lugarEval, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Tipo de Evaluación:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.cevaluacion, fontDato));
            datosEval.Add(Chunk.NEWLINE);
            
            datosEval.Add(new Phrase("Puesto:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.puesto, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Categoría del Puesto:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.categoria, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Función que declara:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.funDeclara, fontDato));
            datosEval.Add(Chunk.NEWLINE);

            datosEval.Add(new Phrase("Función institucional:", fonEiqueta));
            datosEval.Add(Chunk.TABBING);
            datosEval.Add(new Phrase(datosRepInt.funcion, fontDato));
            datosEval.Add(Chunk.NEWLINE); datosEval.Add(Chunk.NEWLINE);

            docRepIntMed.Add(datosEval);
            #endregion

            #region Dx
            Paragraph dx = new Paragraph();
            dx.Alignment = Element.ALIGN_LEFT;
            dx.Add(new Phrase("En base a su proceso de evaluación en la valoración médica y toxicológica se encuentra el evaluado:", fontDato));
            dx.Add(Chunk.NEWLINE); dx.Add(Chunk.NEWLINE);

            docRepIntMed.Add(dx);
            #endregion

            #region Dxb
            Paragraph dx2 = new Paragraph();
            dx2.Alignment = Element.ALIGN_CENTER;
            dx2.Add(new Phrase(datosRepInt.dx, fonEiqueta));
            dx2.Add(Chunk.NEWLINE); dx2.Add(Chunk.NEWLINE);

            docRepIntMed.Add(dx2);
            #endregion

            #region Sintesis
            Paragraph sintesis = new Paragraph();
            sintesis.Alignment = Element.ALIGN_JUSTIFIED;
            sintesis.Add(new Phrase("Síntesis técnica", fonEiqueta));
            sintesis.Add(Chunk.NEWLINE);
            sintesis.Add(new Phrase(datosRepInt.sintesis, fontDato));
            sintesis.Add(Chunk.NEWLINE); sintesis.Add(Chunk.NEWLINE);
            sintesis.Add(new Phrase(datosRepInt.FOLIO, fonEiqueta));

            docRepIntMed.Add(sintesis);
            #endregion

            docRepIntMed.Close();
            byte[] bytesStream = msRepIntMed.ToArray();
            msRepIntMed = new MemoryStream();
            msRepIntMed.Write(bytesStream, 0, bytesStream.Length);
            msRepIntMed.Position = 0;

            return new FileStreamResult(msRepIntMed, "application/pdf");
        }

        public IActionResult HistoriaClinica(int idHistorico)
        {
            var datosDtosGenerales = repo.Getdosparam1<DatosGeneralesImpresion>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 1 }).FirstOrDefault();
            var datosiianther = repo.Getdosparam1<antHeredofamiliarNoPatologico>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 2 }).FirstOrDefault();
            var datosiiantherMed = repo.Getdosparam1<UbicacionMedicamentoModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 2 }).FirstOrDefault();
            var datosantpat = repo.Getdosparam1<antecedentePatologicoModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 3 }).FirstOrDefault();
            var datosNicotina = repo.Getdosparam1<FagerstromModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 10 }).FirstOrDefault();
            var datosAndro = repo.Getdosparam1<androgenicoModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 5 }).FirstOrDefault();
            var datosGine = repo.Getdosparam1<ginecoModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 4 }).FirstOrDefault();
            var datosAnam = repo.Getdosparam1<anamnesisModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 6 }).FirstOrDefault();
            var datosInter = repo.Getdosparam1<interrogatorioModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 7 }).FirstOrDefault();
            var datosTatuaje = repo.Getdosparam1<pTatuajeModel>("sp_medicos_historia_clinica_impresion", new { @idhistorico = idHistorico, @opcion = 11 }).ToList();

            MemoryStream msHistoria = new MemoryStream();
            Document docHistoria = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwHistoria = PdfWriter.GetInstance(docHistoria, msHistoria);

            string elTitulo = "Historia Clínica";
            pwHistoria.PageEvent = HeaderFooterHistoria.getMultilineFooterHistoria(elTitulo);

            //Bloque para usar con PageEventHelper para la pagina "Pagina n de m
            //var pe = new PageEventHelper();
            //pwHistoria.PageEvent = pe;
            //pe.Title = "Dirección Médica y Toxicológica";
            //docHistoria.AddAuthor("OscarBM");
            //docHistoria.AddTitle("Historia Clínica");

            docHistoria.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

            #region emision - revision - codigo
            PdfPCell clRev = new PdfPCell(new Phrase("EMISION", fonEiqueta));
            clRev.BorderWidth = 0;
            clRev.VerticalAlignment = Element.ALIGN_BOTTOM;
            clRev.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi = new PdfPCell(new Phrase("REVISION", fonEiqueta));
            celEmi.BorderWidth = 0;
            celEmi.VerticalAlignment = Element.ALIGN_BOTTOM;
            celEmi.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCod = new PdfPCell(new Phrase("CODIGO", fonEiqueta));
            celCod.BorderWidth = 0;
            celCod.VerticalAlignment = Element.ALIGN_BOTTOM;
            celCod.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi_b = new PdfPCell(new Phrase(DateTime.Now.Year.ToString(), fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/02", fonEiqueta));
            celCod_b.BorderWidth = 0;
            celCod_b.VerticalAlignment = Element.ALIGN_TOP;
            celCod_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celCod_b.BorderWidthBottom = 0.75f;

            PdfPTable tblEmiRevCpd = new PdfPTable(3);
            tblEmiRevCpd.WidthPercentage = 100;
            float[] witdhs = new float[] { 33f, 33f, 33f };
            tblEmiRevCpd.SetWidths(witdhs);

            tblEmiRevCpd.AddCell(clRev);
            tblEmiRevCpd.AddCell(celEmi);
            tblEmiRevCpd.AddCell(celCod);

            tblEmiRevCpd.AddCell(celEmi_b);
            tblEmiRevCpd.AddCell(celRev_b);
            tblEmiRevCpd.AddCell(celCod_b);

            docHistoria.Add(tblEmiRevCpd);
            #endregion

            #region Codigo Evaluado
            Paragraph elCodigo = new Paragraph();
            elCodigo.Alignment = Element.ALIGN_RIGHT;
            elCodigo.Add(new Phrase("Código Evaluado: ", fonEiqueta));
            elCodigo.Add(Chunk.TABBING);
            elCodigo.Add(new Phrase(datosDtosGenerales.codigoevaluado, fontDato));
            elCodigo.Add(Chunk.NEWLINE);
            elCodigo.Add(new Phrase("Fecha: ", fonEiqueta));
            elCodigo.Add(Chunk.TABBING);
            elCodigo.Add(new Phrase(datosDtosGenerales.fregistro, fontDato));
            elCodigo.Add(Chunk.TABBING);
            elCodigo.Add(new Phrase("Folio: ", fonEiqueta));
            elCodigo.Add(Chunk.TABBING);
            elCodigo.Add(new Phrase(datosDtosGenerales.folio, fontDato));
            elCodigo.Add(Chunk.NEWLINE);

            docHistoria.Add(elCodigo);
            #endregion

            #region Titular I Datos Generales
            PdfPTable tableTituloDatoGeneral = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableTituloDatoGeneral.SetWidths(widthsTitulosGenerales);
            tableTituloDatoGeneral.HorizontalAlignment = Element.ALIGN_LEFT;
            tableTituloDatoGeneral.SpacingBefore = 10f;
            tableTituloDatoGeneral.SpacingAfter = 5f;

            PdfPCell cellTituloTituloDato = new PdfPCell(new Phrase("I. Datos Generales", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment=Element.ALIGN_MIDDLE,
                UseAscender=true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableTituloDatoGeneral.AddCell(cellTituloTituloDato);

            docHistoria.Add(tableTituloDatoGeneral);
            #endregion

            #region foto
            Byte[] _fotoInt = (Byte[])datosDtosGenerales.Picture;
            iTextSharp.text.Image _lafotoint = iTextSharp.text.Image.GetInstance(_fotoInt);
            _lafotoint.ScalePercent(70f);

            Paragraph derecha = new Paragraph();
            //derecha.Alignment = Element.ALIGN_RIGHT;
            _lafotoint.SetAbsolutePosition(483f, 520f);
            derecha.Add(_lafotoint);
            docHistoria.Add(derecha);
            #endregion

            #region Detalles Datos Generales
            Paragraph detalleGenerale = new Paragraph();
            detalleGenerale.Alignment = Element.ALIGN_LEFT;
            detalleGenerale.Add(new Phrase("Nombre: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.evaluado, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Alias: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.alias, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Edad: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.edad + " años", fontDato));
            detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase("Sexo: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.sexo, fontDato));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase("Estado civil: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.ac_edocivil, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Esolaridad: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.obsest, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("RFC: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.rfc, fontDato));
            detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase("CURP: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.curp, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Domicilio: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.domicilio, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Teléfono: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.telmovil, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Tipo de Evaluación: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.cevaluacion, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Dependencia: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.desc_dependencia, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Sub dependencia: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.desc_subdep, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Puesto: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.puesto, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Función: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.funcion, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Adscripción : ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.cAdscripcion, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Comisión: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase(datosDtosGenerales.comision, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Ocupación: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING);
            //detalleGenerale.Add(new Phrase(datosDtosGenerales.comision, fontDato));
            detalleGenerale.Add(new Phrase("Ponerlo cuando se pongan los datos de No Patológicos", fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            detalleGenerale.Add(new Phrase("Origen: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING); 
            detalleGenerale.Add(new Phrase(datosDtosGenerales.origen, fontDato));
            detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING); detalleGenerale.Add(Chunk.TABBING);
            detalleGenerale.Add(new Phrase("Religión: ", fonEiqueta));
            detalleGenerale.Add(Chunk.TABBING); 
            detalleGenerale.Add(new Phrase(datosDtosGenerales.ac_religion, fontDato));
            detalleGenerale.Add(Chunk.NEWLINE);

            docHistoria.Add(detalleGenerale);
            #endregion

            #region Titular II Antecedentes heredofamiliares
            PdfPTable tableAntHeredofamiliares = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableAntHeredofamiliares.SetWidths(widthsTitulosGenerales);
            tableAntHeredofamiliares.HorizontalAlignment = Element.ALIGN_LEFT;
            tableAntHeredofamiliares.SpacingBefore = 15f;
            tableAntHeredofamiliares.SpacingAfter = 5f;

            PdfPCell cellTituloAntHeredo = new PdfPCell(new Phrase("II. Antecedentes Heredofamiliares", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableAntHeredofamiliares.AddCell(cellTituloAntHeredo);

            docHistoria.Add(tableAntHeredofamiliares);
            #endregion

            #region Detalles II Antecedentes heredofamiliares
            PdfPTable tblAnte = new PdfPTable(4)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] values = new float[4];
            values[0] = 100;
            values[1] = 50;
            values[2] = 80;
            values[3] = 330;
            tblAnte.SetWidths(values);
            tblAnte.HorizontalAlignment = 0;
            tblAnte.SpacingAfter = 10f;
            //tblDatosEvaluado.SpacingBefore = 10f;
            tblAnte.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- 1a linea
            PdfPCell celTitPatologia = new PdfPCell(new Phrase("Patológias", fonEiqueta));
            celTitPatologia.BorderWidth = 0;
            celTitPatologia.VerticalAlignment = Element.ALIGN_CENTER;
            celTitPatologia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitSiNo = new PdfPCell(new Phrase("Sí / No", fonEiqueta));
            celTitSiNo.BorderWidth = 0;
            celTitSiNo.VerticalAlignment = Element.ALIGN_CENTER;
            celTitSiNo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitParentesco = new PdfPCell(new Phrase("Parentesco", fonEiqueta));
            celTitParentesco.BorderWidth = 0;
            celTitParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celTitParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitEspecificar = new PdfPCell(new Phrase("Especifique", fonEiqueta));
            celTitEspecificar.BorderWidth = 0;
            celTitEspecificar.VerticalAlignment = Element.ALIGN_CENTER;
            celTitEspecificar.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell celTitDm = new PdfPCell(new Phrase("DM", fonEiqueta));
            celTitDm.BorderWidth = 0;
            celTitDm.VerticalAlignment = Element.ALIGN_CENTER;
            celTitDm.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDmSN = new PdfPCell(new Phrase(datosiianther.dm_sn == true ? "Sí" : "No", fontDato));
            celDmSN.BorderWidth = 0;
            celDmSN.VerticalAlignment = Element.ALIGN_CENTER;
            celDmSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDmParentesco = new PdfPCell(new Phrase(datosiianther.dm_quien, fontDato));
            celDmParentesco.BorderWidth = 0;
            celDmParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celDmParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDmEspecifique = new PdfPCell(new Phrase(datosiianther.cDiabetes, fontDato));
            celDmEspecifique.BorderWidth = 0;
            celDmEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celDmEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell celTitDHas = new PdfPCell(new Phrase("HAS", fonEiqueta));
            celTitDHas.BorderWidth = 0;
            celTitDHas.VerticalAlignment = Element.ALIGN_CENTER;
            celTitDHas.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHasSN = new PdfPCell(new Phrase(datosiianther.has_sn == true ? "Sí" : "No", fontDato));
            celHasSN.BorderWidth = 0;
            celHasSN.VerticalAlignment = Element.ALIGN_CENTER;
            celHasSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHasParentesco = new PdfPCell(new Phrase(datosiianther.has_quien, fontDato));
            celHasParentesco.BorderWidth = 0;
            celHasParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celHasParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHasEspecifique = new PdfPCell(new Phrase(datosiianther.cHipertension, fontDato));
            celHasEspecifique.BorderWidth = 0;
            celHasEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celHasEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 3a linea
            PdfPCell celTitEpi = new PdfPCell(new Phrase("Epilepsia", fonEiqueta));
            celTitEpi.BorderWidth = 0;
            celTitEpi.VerticalAlignment = Element.ALIGN_CENTER;
            celTitEpi.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celEpiSN = new PdfPCell(new Phrase(datosiianther.ep_sn == true ? "Sí" : "No", fontDato));
            celEpiSN.BorderWidth = 0;
            celEpiSN.VerticalAlignment = Element.ALIGN_CENTER;
            celEpiSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celEpiParentesco = new PdfPCell(new Phrase(datosiianther.ep_quien, fontDato));
            celEpiParentesco.BorderWidth = 0;
            celEpiParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celEpiParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celEpiEspecifique = new PdfPCell(new Phrase(datosiianther.cNeurologicos, fontDato));
            celEpiEspecifique.BorderWidth = 0;
            celEpiEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celEpiEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 4a linea
            PdfPCell celTitTb = new PdfPCell(new Phrase("TB", fonEiqueta));
            celTitTb.BorderWidth = 0;
            celTitTb.VerticalAlignment = Element.ALIGN_CENTER;
            celTitTb.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTbSN = new PdfPCell(new Phrase(datosiianther.tb_sn == true ? "Sí" : "No", fontDato));
            celTbSN.BorderWidth = 0;
            celTbSN.VerticalAlignment = Element.ALIGN_CENTER;
            celTbSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTbParentesco = new PdfPCell(new Phrase(datosiianther.tb_quien, fontDato));
            celTbParentesco.BorderWidth = 0;
            celTbParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celTbParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTbEspecifique = new PdfPCell(new Phrase(datosiianther.cTuberculosis, fontDato));
            celTbEspecifique.BorderWidth = 0;
            celTbEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celTbEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 5a linea
            PdfPCell celTitAs = new PdfPCell(new Phrase("Asma", fonEiqueta));
            celTitAs.BorderWidth = 0;
            celTitAs.VerticalAlignment = Element.ALIGN_CENTER;
            celTitAs.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celAsSN = new PdfPCell(new Phrase(datosiianther.as_sn == true ? "Sí" : "No", fontDato));
            celAsSN.BorderWidth = 0;
            celAsSN.VerticalAlignment = Element.ALIGN_CENTER;
            celAsSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celAsParentesco = new PdfPCell(new Phrase(datosiianther.as_qiien, fontDato));
            celAsParentesco.BorderWidth = 0;
            celAsParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celAsParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celAsEspecifique = new PdfPCell(new Phrase(datosiianther.cAsma, fontDato));
            celAsEspecifique.BorderWidth = 0;
            celAsEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celAsEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 6a linea
            PdfPCell celTitCa = new PdfPCell(new Phrase("CA", fonEiqueta));
            celTitCa.BorderWidth = 0;
            celTitCa.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCa.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCaSN = new PdfPCell(new Phrase(datosiianther.ca_sn == true ? "Sí" : "No", fontDato));
            celCaSN.BorderWidth = 0;
            celCaSN.VerticalAlignment = Element.ALIGN_CENTER;
            celCaSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCaParentesco = new PdfPCell(new Phrase(datosiianther.ca_quien, fontDato));
            celCaParentesco.BorderWidth = 0;
            celCaParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celCaParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCaEspecifique = new PdfPCell(new Phrase(datosiianther.cCancer, fontDato));
            celCaEspecifique.BorderWidth = 0;
            celCaEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celCaEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 7a linea
            PdfPCell celTitCar = new PdfPCell(new Phrase("Cardiopatías", fonEiqueta));
            celTitCar.BorderWidth = 0;
            celTitCar.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCar.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCarSN = new PdfPCell(new Phrase(datosiianther.card_sn == true ? "Sí" : "No", fontDato));
            celCarSN.BorderWidth = 0;
            celCarSN.VerticalAlignment = Element.ALIGN_CENTER;
            celCarSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCarParentesco = new PdfPCell(new Phrase(datosiianther.card_quien, fontDato));
            celCarParentesco.BorderWidth = 0;
            celCarParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celCarParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCarEspecifique = new PdfPCell(new Phrase(datosiianther.cCardiopatias, fontDato));
            celCarEspecifique.BorderWidth = 0;
            celCarEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celCarEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 8a linea
            PdfPCell celTitHep = new PdfPCell(new Phrase("Hepatopatías", fonEiqueta));
            celTitHep.BorderWidth = 0;
            celTitHep.VerticalAlignment = Element.ALIGN_CENTER;
            celTitHep.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHepSN = new PdfPCell(new Phrase(datosiianther.hepa_sn == true ? "Sí" : "No", fontDato));
            celHepSN.BorderWidth = 0;
            celHepSN.VerticalAlignment = Element.ALIGN_CENTER;
            celHepSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHepParentesco = new PdfPCell(new Phrase(datosiianther.hepa_quien, fontDato));
            celHepParentesco.BorderWidth = 0;
            celHepParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celHepParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHepEspecifique = new PdfPCell(new Phrase(datosiianther.cHepatopatias, fontDato));
            celHepEspecifique.BorderWidth = 0;
            celHepEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celHepEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 9a linea
            PdfPCell celTitNef = new PdfPCell(new Phrase("Nefropatías", fonEiqueta));
            celTitNef.BorderWidth = 0;
            celTitNef.VerticalAlignment = Element.ALIGN_CENTER;
            celTitNef.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celNefSN = new PdfPCell(new Phrase(datosiianther.nefr_sn == true ? "Sí" : "No", fontDato));
            celNefSN.BorderWidth = 0;
            celNefSN.VerticalAlignment = Element.ALIGN_CENTER;
            celNefSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celNefParentesco = new PdfPCell(new Phrase(datosiianther.nefr_quien, fontDato));
            celNefParentesco.BorderWidth = 0;
            celNefParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celNefParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celNefEspecifique = new PdfPCell(new Phrase(datosiianther.cNefropatias, fontDato));
            celNefEspecifique.BorderWidth = 0;
            celNefEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celNefEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 10a linea
            PdfPCell celTitHem = new PdfPCell(new Phrase("Hematológicos", fonEiqueta));
            celTitHem.BorderWidth = 0;
            celTitHem.VerticalAlignment = Element.ALIGN_CENTER;
            celTitHem.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHemSN = new PdfPCell(new Phrase(datosiianther.bHematologicos == true ? "Sí" : "No", fontDato));
            celHemSN.BorderWidth = 0;
            celHemSN.VerticalAlignment = Element.ALIGN_CENTER;
            celHemSN.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHemParentesco = new PdfPCell(new Phrase(datosiianther.cHema_quien, fontDato));
            celHemParentesco.BorderWidth = 0;
            celHemParentesco.VerticalAlignment = Element.ALIGN_CENTER;
            celHemParentesco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHemEspecifique = new PdfPCell(new Phrase(datosiianther.cHematologicos, fontDato));
            celHemEspecifique.BorderWidth = 0;
            celHemEspecifique.VerticalAlignment = Element.ALIGN_CENTER;
            celHemEspecifique.HorizontalAlignment = Element.ALIGN_LEFT;

            tblAnte.AddCell(celTitPatologia);
            tblAnte.AddCell(celTitSiNo);
            tblAnte.AddCell(celTitParentesco);
            tblAnte.AddCell(celTitEspecificar);

            tblAnte.AddCell(celTitDm);
            tblAnte.AddCell(celDmSN);
            tblAnte.AddCell(celDmParentesco);
            tblAnte.AddCell(celDmEspecifique);

            tblAnte.AddCell(celTitDHas);
            tblAnte.AddCell(celHasSN);
            tblAnte.AddCell(celHasParentesco);
            tblAnte.AddCell(celHasEspecifique);

            tblAnte.AddCell(celTitEpi);
            tblAnte.AddCell(celEpiSN);
            tblAnte.AddCell(celEpiParentesco);
            tblAnte.AddCell(celEpiEspecifique);

            tblAnte.AddCell(celTitTb);
            tblAnte.AddCell(celTbSN);
            tblAnte.AddCell(celTbParentesco);
            tblAnte.AddCell(celTbEspecifique);

            tblAnte.AddCell(celTitAs);
            tblAnte.AddCell(celAsSN);
            tblAnte.AddCell(celAsParentesco);
            tblAnte.AddCell(celAsEspecifique);

            tblAnte.AddCell(celTitCa);
            tblAnte.AddCell(celCaSN);
            tblAnte.AddCell(celCaParentesco);
            tblAnte.AddCell(celCaEspecifique);

            tblAnte.AddCell(celTitCar);
            tblAnte.AddCell(celCarSN);
            tblAnte.AddCell(celCarParentesco);
            tblAnte.AddCell(celCarEspecifique);

            tblAnte.AddCell(celTitHep);
            tblAnte.AddCell(celHepSN);
            tblAnte.AddCell(celHepParentesco);
            tblAnte.AddCell(celHepEspecifique);

            tblAnte.AddCell(celTitNef);
            tblAnte.AddCell(celNefSN);
            tblAnte.AddCell(celNefParentesco);
            tblAnte.AddCell(celNefEspecifique);

            tblAnte.AddCell(celTitHem);
            tblAnte.AddCell(celHemSN);
            tblAnte.AddCell(celHemParentesco);
            tblAnte.AddCell(celHemEspecifique);

            docHistoria.Add(tblAnte);
            #endregion

            #region Titular III Antecedentes personales no patologicos
            PdfPTable tableAntNoPato = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableAntNoPato.SetWidths(widthsTitulosGenerales);
            tableAntNoPato.HorizontalAlignment = Element.ALIGN_LEFT;
            tableAntNoPato.SpacingBefore = 15f;
            tableAntNoPato.SpacingAfter = 5f;

            PdfPCell cellTituloAntNoPato = new PdfPCell(new Phrase("III. Antecedentes personales no patológicos", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableAntNoPato.AddCell(cellTituloAntNoPato);

            docHistoria.Add(tableAntNoPato);
            #endregion

            #region Detalles III Antecedentes personales no patológicos
            Paragraph detNP01 = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            detNP01.Add(new Phrase("Horario: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.cHorario, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Función: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.cFuncion, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Higiene: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.chigiene2, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Ejercicio: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.np_ejercicio == true ? "Si" : "No", fontDato));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase("Frecuencia: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.np_habitacion, fontDato));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase("Tipo: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.np_higiene, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Porta arma: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.np_arma == true ? "Si" : "No", fontDato));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.cArma, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Opera vehiculo oficial: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.np_vehiculo == true ? "Si" : "No", fontDato));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiianther.cVehiculo, fontDato));
            detNP01.Add(Chunk.NEWLINE); detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Medicamentos: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiiantherMed.ac_medicamento, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Motivo: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING); detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiiantherMed.ac_motivo, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Tiempo de uso: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiiantherMed.ac_tuso, fontDato));
            detNP01.Add(Chunk.NEWLINE);

            detNP01.Add(new Phrase("Prescrito por: ", fonEiqueta));
            detNP01.Add(Chunk.TABBING); detNP01.Add(Chunk.TABBING);
            detNP01.Add(new Phrase(datosiiantherMed.ac_prescrito, fontDato));
            detNP01.Add(Chunk.NEWLINE); 

            docHistoria.Add(detNP01);

            PdfPTable tblExp = new PdfPTable(2)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valuesExp = new float[2];
            valuesExp[0] = 130;
            valuesExp[1] = 430;
            tblExp.SetWidths(valuesExp);
            tblExp.HorizontalAlignment = 0;
            tblExp.SpacingAfter = 10f;
            tblExp.SpacingBefore = 10f;
            tblExp.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- 1a linea
            PdfPCell cellTit = new PdfPCell(new Phrase("Expocision", fonEiqueta));
            cellTit.Colspan = 2;
            cellTit.BorderWidth = 0;
            cellTit.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTit.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTit.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell cellFisicos = new PdfPCell(new Phrase("Factores físicos", fonEiqueta));
            cellFisicos.BorderWidth = 0;
            cellFisicos.HorizontalAlignment = Element.ALIGN_LEFT;
            cellFisicos.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellFisicos.UseAscender = true;

            PdfPCell cellFisicosTex = new PdfPCell(new Phrase(datosiianther.cFisicos, fontDato));
            cellFisicosTex.BorderWidth = 0;
            cellFisicosTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellFisicosTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellFisicosTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 3a linea
            PdfPCell cellQuimicos = new PdfPCell(new Phrase("Factores químicos", fonEiqueta));
            cellQuimicos.BorderWidth = 0;
            cellQuimicos.HorizontalAlignment = Element.ALIGN_LEFT;
            cellQuimicos.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellQuimicos.UseAscender = true;

            PdfPCell cellQuimicosTex = new PdfPCell(new Phrase(datosiianther.cQuimicos, fontDato));
            cellQuimicosTex.BorderWidth = 0;
            cellQuimicosTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellQuimicosTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellQuimicosTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 4a linea
            PdfPCell cellMecanicos = new PdfPCell(new Phrase("Factores mecánicos", fonEiqueta));
            cellMecanicos.BorderWidth = 0;
            cellMecanicos.HorizontalAlignment = Element.ALIGN_LEFT;
            cellMecanicos.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellMecanicos.UseAscender = true;

            PdfPCell cellMecanicosTex = new PdfPCell(new Phrase(datosiianther.cMecanico, fontDato));
            cellMecanicosTex.BorderWidth = 0;
            cellMecanicosTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellMecanicosTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellMecanicosTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 5a linea
            PdfPCell cellBiologicos = new PdfPCell(new Phrase("Factores biológicos", fonEiqueta));
            cellBiologicos.BorderWidth = 0;
            cellBiologicos.HorizontalAlignment = Element.ALIGN_LEFT;
            cellBiologicos.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellBiologicos.UseAscender = true;

            PdfPCell cellBiologicosTex = new PdfPCell(new Phrase(datosiianther.cBiologico, fontDato));
            cellBiologicosTex.BorderWidth = 0;
            cellBiologicosTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellBiologicosTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellBiologicosTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 6a linea
            PdfPCell cellPsicosocial = new PdfPCell(new Phrase("Factores psicosociales", fonEiqueta));
            cellPsicosocial.BorderWidth = 0;
            cellPsicosocial.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPsicosocial.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellPsicosocial.UseAscender = true;

            PdfPCell cellPsicosocialTex = new PdfPCell(new Phrase(datosiianther.cPsicosocial, fontDato));
            cellPsicosocialTex.BorderWidth = 0;
            cellPsicosocialTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPsicosocialTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellPsicosocialTex.UseAscender = true;

            tblExp.AddCell(cellTit);

            tblExp.AddCell(cellFisicos);
            tblExp.AddCell(cellFisicosTex);

            tblExp.AddCell(cellQuimicos);
            tblExp.AddCell(cellQuimicosTex);

            tblExp.AddCell(cellMecanicos);
            tblExp.AddCell(cellMecanicosTex);

            tblExp.AddCell(cellBiologicos);
            tblExp.AddCell(cellBiologicosTex);

            tblExp.AddCell(cellPsicosocial);
            tblExp.AddCell(cellPsicosocialTex);

            docHistoria.Add(tblExp);

            PdfPTable tblhabitacion = new PdfPTable(12)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valueshb = new float[12];
            valueshb[0] = 40;
            valueshb[1] = 15;
            valueshb[2] = 80;
            valueshb[3] = 15;
            valueshb[4] = 85;
            valueshb[5] = 15;
            valueshb[6] = 75;
            valueshb[7] = 15;
            valueshb[8] = 85;
            valueshb[9] = 20;
            valueshb[10] = 115;
            valueshb[11] = 20;
            tblhabitacion.SetWidths(valueshb);
            tblhabitacion.HorizontalAlignment = 0;
            tblhabitacion.SpacingAfter = 10f;
            //tblDatosEvaluado.SpacingBefore = 10f;
            tblhabitacion.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- 1a linea
            PdfPCell cellTitHabitacion = new PdfPCell(new Phrase("Habitación", fonEiqueta));
            cellTitHabitacion.Colspan = 12;
            cellTitHabitacion.BorderWidth = 0;
            cellTitHabitacion.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTitHabitacion.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTitHabitacion.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell cellAgua = new PdfPCell(new Phrase("Agua: ", fonEiqueta));
            cellAgua.BorderWidth = 0;
            cellAgua.HorizontalAlignment = Element.ALIGN_CENTER;
            cellAgua.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellAgua.UseAscender = true;

            PdfPCell cellAguaSN = new PdfPCell(new Phrase(datosiianther.np_agua == true ? "Si" : "No", fontDato));
            cellAguaSN.BorderWidth = 0;
            cellAguaSN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellAguaSN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellAguaSN.UseAscender = true;

            PdfPCell cellGas = new PdfPCell(new Phrase("Gas: ", fonEiqueta));
            cellGas.BorderWidth = 0;
            cellGas.HorizontalAlignment = Element.ALIGN_CENTER;
            cellGas.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellGas.UseAscender = true;

            PdfPCell cellGasSN = new PdfPCell(new Phrase(datosiianther.np_gas == true ? "Si" : "No", fontDato));
            cellGasSN.BorderWidth = 0;
            cellGasSN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellGasSN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellGasSN.UseAscender = true;

            PdfPCell cellEle = new PdfPCell(new Phrase("Electricidad: ", fonEiqueta));
            cellEle.BorderWidth = 0;
            cellEle.HorizontalAlignment = Element.ALIGN_CENTER;
            cellEle.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellEle.UseAscender = true;

            PdfPCell cellEleSN = new PdfPCell(new Phrase(datosiianther.np_electr == true ? "Si" : "No", fontDato));
            cellEleSN.BorderWidth = 0;
            cellEleSN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellEleSN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellEleSN.UseAscender = true;

            PdfPCell cellDre = new PdfPCell(new Phrase("Drenaje: ", fonEiqueta));
            cellDre.BorderWidth = 0;
            cellDre.HorizontalAlignment = Element.ALIGN_CENTER;
            cellDre.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDre.UseAscender = true;

            PdfPCell cellDreSN = new PdfPCell(new Phrase(datosiianther.np_drenaje == true ? "Si" : "No", fontDato));
            cellDreSN.BorderWidth = 0;
            cellDreSN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellDreSN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDreSN.UseAscender = true;

            PdfPCell cellHac = new PdfPCell(new Phrase("Hacinamiento: ", fonEiqueta));
            cellHac.BorderWidth = 0;
            cellHac.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHac.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellHac.UseAscender = true;

            PdfPCell cellHacsN = new PdfPCell(new Phrase(datosiianther.np_hacinamiento == true ? "Si" : "No", fontDato));
            cellHacsN.BorderWidth = 0;
            cellHacsN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellHacsN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellHacsN.UseAscender = true;

            PdfPCell cellZoo = new PdfPCell(new Phrase("Zoonosis: ", fonEiqueta));
            cellZoo.BorderWidth = 0;
            cellZoo.HorizontalAlignment = Element.ALIGN_CENTER;
            cellZoo.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellZoo.UseAscender = true;

            PdfPCell cellZooSN = new PdfPCell(new Phrase(datosiianther.np_zoonosis == true ? "Si" : "No", fontDato));
            cellZooSN.BorderWidth = 0;
            cellZooSN.HorizontalAlignment = Element.ALIGN_CENTER;
            cellZooSN.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellZooSN.UseAscender = true;

            tblhabitacion.AddCell(cellTitHabitacion);

            tblhabitacion.AddCell(cellAgua);
            tblhabitacion.AddCell(cellAguaSN);
            tblhabitacion.AddCell(cellGas);
            tblhabitacion.AddCell(cellGasSN);
            tblhabitacion.AddCell(cellEle);
            tblhabitacion.AddCell(cellEleSN);
            tblhabitacion.AddCell(cellDre);
            tblhabitacion.AddCell(cellDreSN);
            tblhabitacion.AddCell(cellHac);
            tblhabitacion.AddCell(cellHacsN);
            tblhabitacion.AddCell(cellZoo);
            tblhabitacion.AddCell(cellZooSN);

            docHistoria.Add(tblhabitacion);

            Paragraph detNP02 = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            detNP02.Add(new Phrase("Alimentación: ", fonEiqueta));
            detNP02.Add(Chunk.TABBING); detNP02.Add(Chunk.TABBING);
            detNP02.Add(new Phrase(datosiianther.np_alimento, fontDato));
            detNP02.Add(Chunk.NEWLINE);

            detNP02.Add(new Phrase("Inmunizaciones: ", fonEiqueta));
            detNP02.Add(Chunk.TABBING);
            detNP02.Add(new Phrase(datosiianther.np_inmunizac, fontDato));
            detNP02.Add(Chunk.NEWLINE);

            detNP02.Add(new Phrase("Observaciones: ", fonEiqueta));
            detNP02.Add(Chunk.TABBING);
            detNP02.Add(new Phrase(datosiianther.cObserva, fontDato));
            detNP02.Add(Chunk.NEWLINE);

            docHistoria.Add(detNP02);
            #endregion

            #region IV Antecedentes personales patológicos
            PdfPTable tableAntPato = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableAntPato.SetWidths(widthsTitulosGenerales);
            tableAntPato.HorizontalAlignment = Element.ALIGN_LEFT;
            tableAntPato.SpacingBefore = 15f;
            tableAntPato.SpacingAfter = 5f;

            PdfPCell cellTituloAntPato = new PdfPCell(new Phrase("IV. Antecedentes personales patológicos", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableAntPato.AddCell(cellTituloAntPato);

            docHistoria.Add(tableAntPato);
            #endregion

            #region Detalles IV Antecedentes personales patologicos
            Paragraph antPat = new Paragraph()
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            antPat.Add(new Phrase("Enfermedades congénticas", fonEiqueta));
            antPat.Add(Chunk.NEWLINE);
            antPat.Add(new Phrase(datosantpat.pt_congenita, fontDato));
            antPat.Add(Chunk.NEWLINE); antPat.Add(Chunk.NEWLINE);

            antPat.Add(new Phrase("Enfermedades de la infancia", fonEiqueta));
            antPat.Add(Chunk.NEWLINE);
            antPat.Add(new Phrase(datosantpat.pt_infancia, fontDato));
            antPat.Add(Chunk.NEWLINE); antPat.Add(Chunk.NEWLINE);

            antPat.Add(new Phrase("Enfermedades crónico degenerativas", fonEiqueta));
            antPat.Add(Chunk.NEWLINE);
            antPat.Add(new Phrase(datosantpat.pt_cronodeg, fontDato));
            antPat.Add(Chunk.NEWLINE);

            docHistoria.Add(antPat);

            PdfPTable tblNeuro = new PdfPTable(2)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesNeu = new float[2];
            valuesNeu[0] = 100;
            valuesNeu[1] = 460;

            tblNeuro.SetWidths(valuesNeu);
            tblNeuro.HorizontalAlignment = 0;
            tblNeuro.SpacingAfter = 10f;
            tblNeuro.SpacingBefore = 10f;
            tblNeuro.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- 1a linea
            PdfPCell cellQuirurgico = new PdfPCell(new Phrase("Quirúrgicos", fonEiqueta));
            cellQuirurgico.BorderWidth = 0;
            cellQuirurgico.HorizontalAlignment = Element.ALIGN_LEFT;
            cellQuirurgico.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellQuirurgico.UseAscender = true;

            PdfPCell cellQuirurgicoTex = new PdfPCell(new Phrase(datosantpat.pt_quirurgica, fontDato));
            cellQuirurgicoTex.BorderWidth = 0;
            cellQuirurgicoTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellQuirurgicoTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellQuirurgicoTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell cellTraumatico = new PdfPCell(new Phrase("Traumático", fonEiqueta));
            cellTraumatico.BorderWidth = 0;
            cellTraumatico.HorizontalAlignment = Element.ALIGN_LEFT;
            cellTraumatico.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellTraumatico.UseAscender = true;

            PdfPCell cellTraumaticoTex = new PdfPCell(new Phrase(datosantpat.pt_trauma, fontDato));
            cellTraumaticoTex.BorderWidth = 0;
            cellTraumaticoTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellTraumaticoTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellTraumaticoTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 3a linea
            PdfPCell cellAlergico = new PdfPCell(new Phrase("Alérgicos", fonEiqueta));
            cellAlergico.BorderWidth = 0;
            cellAlergico.HorizontalAlignment = Element.ALIGN_LEFT;
            cellAlergico.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellAlergico.UseAscender = true;

            PdfPCell cellAlergicoTex = new PdfPCell(new Phrase(datosantpat.pt_alergico, fontDato));
            cellAlergicoTex.BorderWidth = 0;
            cellAlergicoTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellAlergicoTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellAlergicoTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 4a linea
            PdfPCell cellTrans = new PdfPCell(new Phrase("Transfusión", fonEiqueta));
            cellTrans.BorderWidth = 0;
            cellTrans.HorizontalAlignment = Element.ALIGN_LEFT;
            cellTrans.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellTrans.UseAscender = true;

            PdfPCell cellTransTex = new PdfPCell(new Phrase(datosantpat.pt_transfusion, fontDato));
            cellTransTex.BorderWidth = 0;
            cellTransTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellTransTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellTransTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 5a linea
            PdfPCell cellIntox = new PdfPCell(new Phrase("Intoxicaciones", fonEiqueta));
            cellIntox.BorderWidth = 0;
            cellIntox.HorizontalAlignment = Element.ALIGN_LEFT;
            cellIntox.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellIntox.UseAscender = true;

            PdfPCell cellIntoxTex = new PdfPCell(new Phrase(datosantpat.pt_intoxica, fontDato));
            cellIntoxTex.BorderWidth = 0;
            cellIntoxTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellIntoxTex.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cellIntoxTex.UseAscender = true;

            //-------------------------------------------------------------------------------------------------------- 6a linea
            PdfPCell cellHospi = new PdfPCell(new Phrase("Hospitalización", fonEiqueta));
            cellHospi.BorderWidth = 0;
            cellHospi.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHospi.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell cellHospiTex = new PdfPCell(new Phrase(datosantpat.pt_hospiltal, fontDato));
            cellHospiTex.BorderWidth = 0;
            cellHospiTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHospiTex.VerticalAlignment = Element.ALIGN_MIDDLE;

            //-------------------------------------------------------------------------------------------------------- 7a linea
            PdfPCell cellObs = new PdfPCell(new Phrase("Observación", fonEiqueta));
            cellObs.BorderWidth = 0;
            cellObs.HorizontalAlignment = Element.ALIGN_LEFT;
            cellObs.VerticalAlignment = Element.ALIGN_MIDDLE;

            PdfPCell cellObsTex = new PdfPCell(new Phrase(datosantpat.cOservapatologicos, fontDato));
            cellObsTex.BorderWidth = 0;
            cellObsTex.HorizontalAlignment = Element.ALIGN_LEFT;
            cellObsTex.VerticalAlignment = Element.ALIGN_MIDDLE;

            tblNeuro.AddCell(cellQuirurgico);
            tblNeuro.AddCell(cellQuirurgicoTex);

            tblNeuro.AddCell(cellTraumatico);
            tblNeuro.AddCell(cellTraumaticoTex);

            tblNeuro.AddCell(cellAlergico);
            tblNeuro.AddCell(cellAlergicoTex);

            tblNeuro.AddCell(cellTrans);
            tblNeuro.AddCell(cellTransTex);

            tblNeuro.AddCell(cellIntox);
            tblNeuro.AddCell(cellIntoxTex);

            tblNeuro.AddCell(cellHospi);
            tblNeuro.AddCell(cellHospiTex);

            tblNeuro.AddCell(cellObs);
            tblNeuro.AddCell(cellObsTex);

            docHistoria.Add(tblNeuro);

            Paragraph vicios = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            vicios.Add(new Phrase("Tabaquismo: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_tabaco == true ? "Si" : "No", fontDato));
            vicios.Add(Chunk.TABBING); 

            vicios.Add(new Phrase("Cigarros: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_cigarros, fontDato));
            vicios.Add(Chunk.TABBING); vicios.Add(Chunk.TABBING);

            vicios.Add(new Phrase("Tiempo: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_años+" años", fontDato));
            vicios.Add(Chunk.TABBING); vicios.Add(Chunk.TABBING);

            vicios.Add(new Phrase("IT: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.cit, fontDato));
            vicios.Add(Chunk.NEWLINE);

            vicios.Add(new Phrase("Alcoholismo: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_alcohol == true ? "Si" : "No", fontDato));
            vicios.Add(Chunk.TABBING); 

            vicios.Add(new Phrase("Cantidad: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_frec_bebida, fontDato));
            vicios.Add(Chunk.TABBING); vicios.Add(Chunk.TABBING);
            vicios.Add(Chunk.NEWLINE);

            string testFag = string.Empty;
            int totalFag = 0;
            if (datosNicotina.p7 == 0)
                testFag = "No fuma";
            else
            {
                totalFag = datosNicotina.p1 + datosNicotina.p2 + datosNicotina.p3 + datosNicotina.p4 + datosNicotina.p5 + datosNicotina.p6;
                if (totalFag == 0 && totalFag <= 4)
                    testFag = "Dependencia baja";
                else if (totalFag >= 5 && totalFag <= 7)
                    testFag = "Dependencia moderada";
                else if (totalFag >= 8 && totalFag <= 10)
                    testFag = "Dependencia alta";
            }


            vicios.Add(new Phrase("Test Fagerström: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(testFag, fontDato));
            vicios.Add(Chunk.NEWLINE);

            string recAudit = string.Empty;
            if (datosiianther.cit == "I")
                recAudit = "EN EL TEST DE IDENTIFICACION DE LOS TRASTORNOS DEBIDO AL CONSUMO DE ALCOHOL EL EVALUADO SE ENCUENTRA CLASIFICADO EN EL NIVEL DE RIESGO I POR LO QUE SE SUGIERE EDUCACION SOBRE EL CONSUMO DE BEBIDAS ALCOHOLICAS";
            else if (datosiianther.cit == "II")
                recAudit = "EN EL TEST DE IDENTIFICACION DE LOS TRASTORNOS DEBIDO AL CONSUMO DE ALCOHOL EL EVALUADO SE ENCUENTRA CLASIFICADO EN EL NIVEL DE RIESGO II POR LO QUE SE SUGIERE OFRECER CONSEJO SIMPLE SOBRE EL CONSUMO DE BEBIDAS ALCOHOLICAS";
            else if (datosiianther.cit == "III")
                recAudit = "EN EL TEST DE IDENTIFICACION DE LOS TRASTORNOS DEBIDO AL CONSUMO DE ALCOHOL EL EVALUADO SE ENCUENTRA CLASIFICADO EN EL NIVEL DE RIESGO III POR LO QUE SE SUGIERE ACUDIR A TERAPIA BREVE Y MONITORIZACION CONTINUADA";
            else if (datosiianther.cit == "IV")
                recAudit = "EN EL TEST DE IDENTIFICACION DE LOS TRASTORNOS DEBIDO AL CONSUMO DE ALCOHOL EL EVALUADO SE ENCUENTRA CLASIFICADO EN EL NIVEL DE RIESGO IV POR LO QUE SE SUGIERE DERIVACION AL ESPECIALISTA PARA UNA EVALUACIÓN DIAGNOSTICA Y TRATAMIENTO";
            else if (datosiianther.cit == "")
                recAudit = "";

            vicios.Add(new Phrase("Recomendacion: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(recAudit, fontDato));
            vicios.Add(Chunk.NEWLINE);

            vicios.Add(new Phrase("Toxicomanías: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_toxico == true ? "Si" : "No", fontDato));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase("Cúal: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_cual_toxico, fontDato));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase("Tiempo: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.np_tiempo, fontDato));
            vicios.Add(Chunk.NEWLINE);

            vicios.Add(new Phrase("Observación: ", fonEiqueta));
            vicios.Add(Chunk.TABBING);
            vicios.Add(new Phrase(datosiianther.cObservatox, fontDato));
            vicios.Add(Chunk.NEWLINE); 

            docHistoria.Add(vicios);
            #endregion

            if (datosDtosGenerales.sexo == "HOMBRE")
                docHistoria.NewPage();

            #region Titulares antecedentes por genero
            string genero = string.Empty;
            if (datosDtosGenerales.sexo == "HOMBRE")
                genero = "V. ANTECEDENTES ANDROGENICOS";
            else
                genero = "V. ANTECEDENTES GINECOLOGICOS";

            PdfPTable tableAntGenero = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableAntGenero.SetWidths(widthsTitulosGenerales);
            tableAntGenero.HorizontalAlignment = Element.ALIGN_LEFT;
            tableAntGenero.SpacingBefore = 5f;
            tableAntGenero.SpacingAfter = 5f;

            PdfPCell cellTituloAntGen = new PdfPCell(new Phrase(genero, fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableAntGenero.AddCell(cellTituloAntGen);

            docHistoria.Add(tableAntGenero);
            #endregion

            #region detalles Androgenicos - Ginecologico
            if (datosDtosGenerales.sexo == "HOMBRE")
            {
                PdfPTable tblAndro = new PdfPTable(4)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] valuesAndro = new float[4];
                valuesAndro[0] = 150;
                valuesAndro[1] = 130;
                valuesAndro[2] = 150;
                valuesAndro[3] = 130;

                tblAndro.SetWidths(valuesAndro);
                tblAndro.HorizontalAlignment = 0;
                tblAndro.SpacingAfter = 5f;
                //tblAndro.SpacingBefore = 5f;
                tblAndro.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------1a Linea
                PdfPCell cellPub = new PdfPCell(new Phrase("Pubertad", fonEiqueta));
                cellPub.BorderWidth = 0;
                cellPub.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellPubTex = new PdfPCell(new Phrase(datosAndro.an_pubertad, fontDato));
                cellPubTex.BorderWidth = 0;
                cellPubTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEdad = new PdfPCell(new Phrase("Edad cuando le salió la barba", fonEiqueta));
                cellEdad.BorderWidth = 0;
                cellEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEdadText = new PdfPCell(new Phrase(datosAndro.an_barba, fontDato));
                cellEdadText.BorderWidth = 0;
                cellEdadText.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------2a Linea
                PdfPCell cellIvsa = new PdfPCell(new Phrase("IVSA", fonEiqueta));
                cellIvsa.BorderWidth = 0;
                cellIvsa.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellIvsaTex = new PdfPCell(new Phrase(datosAndro.an_ivisa, fontDato));
                cellIvsaTex.BorderWidth = 0;
                cellIvsaTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellParAnd = new PdfPCell(new Phrase("Parejas", fonEiqueta));
                cellParAnd.BorderWidth = 0;
                cellParAnd.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellParAndText = new PdfPCell(new Phrase(datosAndro.an_parejas, fontDato));
                cellParAndText.BorderWidth = 0;
                cellParAndText.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------3a Linea
                PdfPCell cellPreAnd = new PdfPCell(new Phrase("Conducta sexual", fonEiqueta));
                cellPreAnd.BorderWidth = 0;
                cellPreAnd.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellPreAndTex = new PdfPCell(new Phrase(datosAndro.an_preferencia, fontDato));
                cellPreAndTex.BorderWidth = 0;
                cellPreAndTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEtsAnd = new PdfPCell(new Phrase("ETS", fonEiqueta));
                cellEtsAnd.BorderWidth = 0;
                cellEtsAnd.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEtsAndTex = new PdfPCell(new Phrase(datosAndro.cEtsandro, fontDato));
                cellEtsAndTex.BorderWidth = 0;
                cellEtsAndTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------4a Linea
                PdfPCell cellObsAnd = new PdfPCell(new Phrase("Observaciones: ", fonEiqueta));
                cellObsAnd.BorderWidth = 0;
                cellObsAnd.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellObsAndTex = new PdfPCell(new Phrase(datosAndro.cOBservaandro, fontDato));
                cellObsAndTex.Colspan = 3;
                cellObsAndTex.BorderWidth = 0;
                cellObsAndTex.HorizontalAlignment = Element.ALIGN_LEFT;

                tblAndro.AddCell(cellPub);
                tblAndro.AddCell(cellPubTex);

                tblAndro.AddCell(cellEdad);
                tblAndro.AddCell(cellEdadText);

                tblAndro.AddCell(cellIvsa);
                tblAndro.AddCell(cellIvsaTex);

                tblAndro.AddCell(cellParAnd);
                tblAndro.AddCell(cellParAndText);

                tblAndro.AddCell(cellPreAnd);
                tblAndro.AddCell(cellPreAndTex);

                tblAndro.AddCell(cellEtsAnd);
                tblAndro.AddCell(cellEtsAndTex);

                tblAndro.AddCell(cellObsAnd);
                tblAndro.AddCell(cellObsAndTex);

                docHistoria.Add(tblAndro);

            }
            else
            {
                PdfPTable tblGin = new PdfPTable(4)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] valuesGin = new float[4];
                valuesGin[0] = 130;
                valuesGin[1] = 150;
                valuesGin[2] = 130;
                valuesGin[3] = 150;

                tblGin.SetWidths(valuesGin);
                tblGin.HorizontalAlignment = 0;
                tblGin.SpacingAfter = 5f;
                //tblAndro.SpacingBefore = 5f;
                tblGin.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------1a Linea
                PdfPCell cellMen = new PdfPCell(new Phrase("Menarca", fonEiqueta));
                cellMen.BorderWidth = 0;
                cellMen.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellMenTex = new PdfPCell(new Phrase(datosGine.gn_mena, fontDato));
                cellMenTex.BorderWidth = 0;
                cellMenTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellAbo = new PdfPCell(new Phrase("Abortos", fonEiqueta));
                cellAbo.BorderWidth = 0;
                cellAbo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellAboText = new PdfPCell(new Phrase(datosGine.gn_aborto, fontDato));
                cellAboText.BorderWidth = 0;
                cellAboText.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------2a Linea
                PdfPCell cellRit = new PdfPCell(new Phrase("Ritmo", fonEiqueta));
                cellRit.BorderWidth = 0;
                cellRit.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellRitTex = new PdfPCell(new Phrase(datosGine.gn_ritmo, fontDato));
                cellRitTex.BorderWidth = 0;
                cellRitTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellFup = new PdfPCell(new Phrase("FUP", fonEiqueta));
                cellFup.BorderWidth = 0;
                cellFup.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellFupTex = new PdfPCell(new Phrase(datosGine.gn_fup, fontDato));
                cellFupTex.BorderWidth = 0;
                cellFupTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------3a Linea
                PdfPCell cellFum = new PdfPCell(new Phrase("FUM", fonEiqueta));
                cellFum.BorderWidth = 0;
                cellFum.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellFumTex = new PdfPCell(new Phrase(datosGine.gn_ritmo, fontDato));
                cellFumTex.BorderWidth = 0;
                cellFumTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellFpp = new PdfPCell(new Phrase("FPP", fonEiqueta));
                cellFpp.BorderWidth = 0;
                cellFpp.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellFppTex = new PdfPCell(new Phrase(datosGine.gn_fup, fontDato));
                cellFppTex.BorderWidth = 0;
                cellFppTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------4a Linea
                PdfPCell cellIvsGin = new PdfPCell(new Phrase("IVSA", fonEiqueta));
                cellIvsGin.BorderWidth = 0;
                cellIvsGin.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellIvsGinTex = new PdfPCell(new Phrase(datosGine.gn_ivsa, fontDato));
                cellIvsGinTex.BorderWidth = 0;
                cellIvsGinTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEtsGin = new PdfPCell(new Phrase("ETS", fonEiqueta));
                cellEtsGin.BorderWidth = 0;
                cellEtsGin.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellEtsGinTex = new PdfPCell(new Phrase(datosGine.cEts, fontDato));
                cellEtsGinTex.BorderWidth = 0;
                cellEtsGinTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------5a Linea
                PdfPCell cellParGin = new PdfPCell(new Phrase("Parejas", fonEiqueta));
                cellParGin.BorderWidth = 0;
                cellParGin.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellParGinTex = new PdfPCell(new Phrase(datosGine.gn_numpar, fontDato));
                cellParGinTex.BorderWidth = 0;
                cellParGinTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellAnti = new PdfPCell(new Phrase("Anticoncepción", fonEiqueta));
                cellAnti.BorderWidth = 0;
                cellAnti.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellAntiTex = new PdfPCell(new Phrase(datosGine.gn_anticon, fontDato));
                cellAntiTex.BorderWidth = 0;
                cellAntiTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------5a Linea
                PdfPCell cellGes = new PdfPCell(new Phrase("Gestación", fonEiqueta));
                cellGes.BorderWidth = 0;
                cellGes.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellGesTex = new PdfPCell(new Phrase(datosGine.gn_gesta, fontDato));
                cellGesTex.BorderWidth = 0;
                cellGesTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellDocma = new PdfPCell(new Phrase("DOCMA", fonEiqueta));
                cellDocma.BorderWidth = 0;
                cellDocma.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellDocmaTex = new PdfPCell(new Phrase(datosGine.gn_docma, fontDato));
                cellDocmaTex.BorderWidth = 0;
                cellDocmaTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------6a Linea
                PdfPCell cellPar = new PdfPCell(new Phrase("Parto", fonEiqueta));
                cellPar.BorderWidth = 0;
                cellPar.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellParTex = new PdfPCell(new Phrase(datosGine.gn_gesta, fontDato));
                cellParTex.BorderWidth = 0;
                cellParTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellDocacu = new PdfPCell(new Phrase("DOCACU", fonEiqueta));
                cellDocacu.BorderWidth = 0;
                cellDocacu.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellDocacuText = new PdfPCell(new Phrase(datosGine.gn_docma, fontDato));
                cellDocacuText.BorderWidth = 0;
                cellDocacuText.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------7a Linea
                PdfPCell cellCes= new PdfPCell(new Phrase("Cesarea", fonEiqueta));
                cellCes.BorderWidth = 0;
                cellCes.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellCesTex = new PdfPCell(new Phrase(datosGine.gn_cesarea, fontDato));
                cellCesTex.BorderWidth = 0;
                cellCesTex.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellPreGin = new PdfPCell(new Phrase("Conducta sexual", fonEiqueta));
                cellPreGin.BorderWidth = 0;
                cellPreGin.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellPreGinTex = new PdfPCell(new Phrase(datosGine.gn_prefiere, fontDato));
                cellPreGinTex.BorderWidth = 0;
                cellPreGinTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------8a Linea
                PdfPCell celComp = new PdfPCell(new Phrase("Complicación", fonEiqueta));
                celComp.BorderWidth = 0;
                celComp.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celCompTex = new PdfPCell(new Phrase(datosGine.gn_complicac, fontDato));
                celCompTex.Colspan = 3;
                celCompTex.BorderWidth = 0;
                celCompTex.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------9a Linea
                PdfPCell cellObsGin = new PdfPCell(new Phrase("Observación", fonEiqueta));
                cellObsGin.BorderWidth = 0;
                cellObsGin.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cellObsGinTex = new PdfPCell(new Phrase(datosGine.cObservagineco, fontDato));
                cellObsGinTex.Colspan = 3;
                cellObsGinTex.BorderWidth = 0;
                cellObsGinTex.HorizontalAlignment = Element.ALIGN_LEFT;

                tblGin.AddCell(cellMen);
                tblGin.AddCell(cellMenTex);
                tblGin.AddCell(cellAbo);
                tblGin.AddCell(cellAboText);

                tblGin.AddCell(cellRit);
                tblGin.AddCell(cellRitTex);
                tblGin.AddCell(cellFup);
                tblGin.AddCell(cellFupTex);

                tblGin.AddCell(cellFum);
                tblGin.AddCell(cellFumTex);
                tblGin.AddCell(cellFpp);
                tblGin.AddCell(cellFppTex);

                tblGin.AddCell(cellIvsGin);
                tblGin.AddCell(cellIvsGinTex);
                tblGin.AddCell(cellEtsGin);
                tblGin.AddCell(cellEtsGinTex);

                tblGin.AddCell(cellParGin);
                tblGin.AddCell(cellParGinTex);
                tblGin.AddCell(cellAnti);
                tblGin.AddCell(cellAntiTex);

                tblGin.AddCell(cellGes);
                tblGin.AddCell(cellGesTex);
                tblGin.AddCell(cellDocma);
                tblGin.AddCell(cellDocmaTex);

                tblGin.AddCell(cellPar);
                tblGin.AddCell(cellParTex);
                tblGin.AddCell(cellDocacu);
                tblGin.AddCell(cellDocacuText);

                tblGin.AddCell(cellCes);
                tblGin.AddCell(cellCesTex);
                tblGin.AddCell(cellPreGin);
                tblGin.AddCell(cellPreGinTex);

                tblGin.AddCell(celComp);
                tblGin.AddCell(celCompTex);
                tblGin.AddCell(cellObsGin);
                tblGin.AddCell(cellObsGinTex);

                docHistoria.Add(tblGin);
            }
            #endregion

            #region VI Anamnesis sistemtica
            PdfPTable tableAnam = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableAnam.SetWidths(widthsTitulosGenerales);
            tableAnam.HorizontalAlignment = Element.ALIGN_LEFT;
            tableAnam.SpacingBefore = 5f;
            tableAnam.SpacingAfter = 10f;

            PdfPCell cellTituloAnam = new PdfPCell(new Phrase("VI Anamnesis  sistémica", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableAnam.AddCell(cellTituloAnam);

            docHistoria.Add(tableAnam);
            #endregion

            #region VI Anamnesis Detalle

            #region Sintomas generales
            PdfPTable tblGral = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesGral = new float[8];
            valuesGral[0] = 30;
            valuesGral[1] = 110;
            valuesGral[2] = 30;
            valuesGral[3] = 110;
            valuesGral[4] = 30;
            valuesGral[5] = 110;
            valuesGral[6] = 30;
            valuesGral[7] = 110;

            tblGral.SetWidths(valuesGral);
            tblGral.HorizontalAlignment = 0;
            tblGral.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblGral.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitGral = new PdfPCell(new Phrase("Síntomas generales", fonEiqueta));
            cellTitGral.Colspan = 8;
            cellTitGral.BorderWidth = 0;
            cellTitGral.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPonTex = new PdfPCell(new Phrase(datosAnam.bVariacion == true ? "X" : "", fonEiqueta));
            cellPonTex.BorderWidth = 0;
            cellPonTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPon = new PdfPCell(new Phrase("Variación Ponderal", fontDato));
            cellPon.BorderWidth = 0;
            cellPon.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellApeText = new PdfPCell(new Phrase(datosAnam.bApetito == true ? "X" : "", fonEiqueta));
            cellApeText.BorderWidth = 0;
            cellApeText.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellApe = new PdfPCell(new Phrase("Apetito", fontDato));
            cellApe.BorderWidth = 0;
            cellApe.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellSedTex = new PdfPCell(new Phrase(datosAnam.bSed == true ? "X" : "", fonEiqueta));
            cellSedTex.BorderWidth = 0;
            cellSedTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellSed = new PdfPCell(new Phrase("Sed", fontDato));
            cellSed.BorderWidth = 0;
            cellSed.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFieTex = new PdfPCell(new Phrase(datosAnam.bFiebre == true ? "X" : "", fonEiqueta));
            cellFieTex.BorderWidth = 0;
            cellFieTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellFie = new PdfPCell(new Phrase("Fiebre", fontDato));
            cellFie.BorderWidth = 0;
            cellFie.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDiaTex = new PdfPCell(new Phrase(datosAnam.bDiaforesis == true ? "X" : "", fonEiqueta));
            cellDiaTex.BorderWidth = 0;
            cellDiaTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDia = new PdfPCell(new Phrase("Diaforesis", fontDato));
            cellDia.BorderWidth = 0;
            cellDia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAdiText = new PdfPCell(new Phrase(datosAnam.bAdinamia == true ? "X" : "", fonEiqueta));
            cellAdiText.BorderWidth = 0;
            cellAdiText.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAdi = new PdfPCell(new Phrase("Adinamia", fontDato));
            cellAdi.BorderWidth = 0;
            cellAdi.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMalTex = new PdfPCell(new Phrase(datosAnam.bMalestar == true ? "X" : "", fonEiqueta));
            cellMalTex.BorderWidth = 0;
            cellMalTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMal = new PdfPCell(new Phrase("Sed", fontDato));
            cellMal.BorderWidth = 0;
            cellMal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellEscTex = new PdfPCell(new Phrase(datosAnam.bEscalofrio == true ? "X" : "", fonEiqueta));
            cellEscTex.BorderWidth = 0;
            cellEscTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEsc = new PdfPCell(new Phrase("Escalofríos", fontDato));
            cellEsc.BorderWidth = 0;
            cellEsc.HorizontalAlignment = Element.ALIGN_LEFT;

            tblGral.AddCell(cellTitGral);
            tblGral.AddCell(cellPonTex);
            tblGral.AddCell(cellPon);
            tblGral.AddCell(cellApeText);
            tblGral.AddCell(cellApe);
            tblGral.AddCell(cellSedTex);
            tblGral.AddCell(cellSed);
            tblGral.AddCell(cellFieTex);
            tblGral.AddCell(cellFie);
            tblGral.AddCell(cellDiaTex);
            tblGral.AddCell(cellDia);
            tblGral.AddCell(cellAdiText);
            tblGral.AddCell(cellAdi);
            tblGral.AddCell(cellMalTex);
            tblGral.AddCell(cellMal);
            tblGral.AddCell(cellEscTex);
            tblGral.AddCell(cellEsc);

            docHistoria.Add(tblGral);

            #endregion

            #region Piel y faneras
            PdfPTable tblPiel = new PdfPTable(6)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesPiel = new float[6];
            valuesPiel[0] = 30;
            valuesPiel[1] = 156;
            valuesPiel[2] = 30;
            valuesPiel[3] = 156;
            valuesPiel[4] = 30;
            valuesPiel[5] = 157;

            tblPiel.SetWidths(valuesPiel);
            tblPiel.HorizontalAlignment = 0;
            tblPiel.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblPiel.DefaultCell.Border = 0;

            PdfPCell cellTitPiel = new PdfPCell(new Phrase("Piel y faneras", fonEiqueta));
            cellTitPiel.Colspan = 8;
            cellTitPiel.BorderWidth = 0;
            cellTitPiel.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPruTex = new PdfPCell(new Phrase(datosAnam.bPrurito == true ? "X" : "", fonEiqueta));
            cellPruTex.BorderWidth = 0;
            cellPruTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPru = new PdfPCell(new Phrase("Prurito", fontDato));
            cellPru.BorderWidth = 0;
            cellPru.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellLesTex = new PdfPCell(new Phrase(datosAnam.bLesiones == true ? "X" : "", fonEiqueta));
            cellLesTex.BorderWidth = 0;
            cellLesTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellLes = new PdfPCell(new Phrase("Lesiones", fontDato));
            cellLes.BorderWidth = 0;
            cellLes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAltTex = new PdfPCell(new Phrase(datosAnam.bAlteraciones == true ? "X" : "", fonEiqueta));
            cellAltTex.BorderWidth = 0;
            cellAltTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAlt = new PdfPCell(new Phrase("Alteraciones en uñas y cabello", fontDato));
            cellAlt.BorderWidth = 0;
            cellAlt.HorizontalAlignment = Element.ALIGN_LEFT;

            tblPiel.AddCell(cellTitPiel);
            tblPiel.AddCell(cellPruTex);
            tblPiel.AddCell(cellPru);
            tblPiel.AddCell(cellLesTex);
            tblPiel.AddCell(cellLes);
            tblPiel.AddCell(cellAltTex);
            tblPiel.AddCell(cellAlt);

            docHistoria.Add(tblPiel);
            #endregion

            #region Aparato digestivo
            PdfPTable tblDig = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesDig = new float[8];
            valuesDig[0] = 30;
            valuesDig[1] = 110;
            valuesDig[2] = 30;
            valuesDig[3] = 110;
            valuesDig[4] = 30;
            valuesDig[5] = 110;
            valuesDig[6] = 30;
            valuesDig[7] = 110;

            tblDig.SetWidths(valuesDig);
            tblDig.HorizontalAlignment = 0;
            tblDig.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblDig.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitdig = new PdfPCell(new Phrase("Aparto digestivo", fonEiqueta));
            cellTitdig.Colspan = 8;
            cellTitdig.BorderWidth = 0;
            cellTitdig.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHalX = new PdfPCell(new Phrase(datosAnam.bHalitosis == true ? "X" : "", fonEiqueta));
            cellHalX.BorderWidth = 0;
            cellHalX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHal = new PdfPCell(new Phrase("Halitosis", fontDato));
            cellHal.BorderWidth = 0;
            cellHal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDisX = new PdfPCell(new Phrase(datosAnam.bDisfagia == true ? "X" : "", fonEiqueta));
            cellDisX.BorderWidth = 0;
            cellDisX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDis = new PdfPCell(new Phrase("Disfagia", fontDato));
            cellDis.BorderWidth = 0;
            cellDis.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellRefX = new PdfPCell(new Phrase(datosAnam.bReflujo == true ? "X" : "", fonEiqueta));
            cellRefX.BorderWidth = 0;
            cellRefX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellRef = new PdfPCell(new Phrase("Reflujo", fontDato));
            cellRef.BorderWidth = 0;
            cellRef.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAnoX = new PdfPCell(new Phrase(datosAnam.bAnorexia == true ? "X" : "", fonEiqueta));
            cellAnoX.BorderWidth = 0;
            cellAnoX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAno = new PdfPCell(new Phrase("Anorexia", fontDato));
            cellAno.BorderWidth = 0;
            cellAno.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellHipX = new PdfPCell(new Phrase(datosAnam.bHiporexia == true ? "X" : "", fonEiqueta));
            cellHipX.BorderWidth = 0;
            cellHipX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHip = new PdfPCell(new Phrase("Hiporexia", fontDato));
            cellHip.BorderWidth = 0;
            cellHip.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOdiX = new PdfPCell(new Phrase(datosAnam.bOdinofagia == true ? "X" : "", fonEiqueta));
            cellOdiX.BorderWidth = 0;
            cellOdiX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOdi = new PdfPCell(new Phrase("Odinofagia", fontDato));
            cellOdi.BorderWidth = 0;
            cellOdi.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPolX = new PdfPCell(new Phrase(datosAnam.bPolipdipsia == true ? "X" : "", fonEiqueta));
            cellPolX.BorderWidth = 0;
            cellPolX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPol = new PdfPCell(new Phrase("Polidipsia", fontDato));
            cellPol.BorderWidth = 0;
            cellPol.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellNauX = new PdfPCell(new Phrase(datosAnam.bNauseas == true ? "X" : "", fonEiqueta));
            cellNauX.BorderWidth = 0;
            cellNauX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellNau = new PdfPCell(new Phrase("Náuseas", fontDato));
            cellNau.BorderWidth = 0;
            cellNau.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellVomX = new PdfPCell(new Phrase(datosAnam.bVomito == true ? "X" : "", fonEiqueta));
            cellVomX.BorderWidth = 0;
            cellVomX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellVom = new PdfPCell(new Phrase("Vómito", fontDato));
            cellVom.BorderWidth = 0;
            cellVom.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDispX = new PdfPCell(new Phrase(datosAnam.bDispepsia == true ? "X" : "", fonEiqueta));
            cellDispX.BorderWidth = 0;
            cellDispX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDisp = new PdfPCell(new Phrase("Dispepsia", fontDato));
            cellDisp.BorderWidth = 0;
            cellDisp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellRecX = new PdfPCell(new Phrase(datosAnam.bRectorragia == true ? "X" : "", fonEiqueta));
            cellRecX.BorderWidth = 0;
            cellRecX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellRec = new PdfPCell(new Phrase("Rectorragia", fontDato));
            cellRec.BorderWidth = 0;
            cellRec.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMelX = new PdfPCell(new Phrase(datosAnam.bMelena == true ? "X" : "", fonEiqueta));
            cellMelX.BorderWidth = 0;
            cellMelX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMel = new PdfPCell(new Phrase("Melena", fontDato));
            cellMel.BorderWidth = 0;
            cellMel.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------4a Linea
            PdfPCell cellPirX = new PdfPCell(new Phrase(datosAnam.bPirosis == true ? "X" : "", fonEiqueta));
            cellPirX.BorderWidth = 0;
            cellPirX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPir = new PdfPCell(new Phrase("Pirosis", fontDato));
            cellPir.BorderWidth = 0;
            cellPir.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHemX = new PdfPCell(new Phrase(datosAnam.bHematemesis == true ? "X" : "", fonEiqueta));
            cellHemX.BorderWidth = 0;
            cellHemX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHem = new PdfPCell(new Phrase("Hematemesis", fontDato));
            cellHem.BorderWidth = 0;
            cellHem.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAcoX = new PdfPCell(new Phrase(datosAnam.bAcolia == true ? "X" : "", fonEiqueta));
            cellAcoX.BorderWidth = 0;
            cellAcoX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAco = new PdfPCell(new Phrase("Acolia", fontDato));
            cellAco.BorderWidth = 0;
            cellAco.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMetX = new PdfPCell(new Phrase(datosAnam.bMeteorismo == true ? "X" : "", fonEiqueta));
            cellMetX.BorderWidth = 0;
            cellMetX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMet = new PdfPCell(new Phrase("Meteorismo", fontDato));
            cellMet.BorderWidth = 0;
            cellMet.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------5a Linea
            PdfPCell cellTenX = new PdfPCell(new Phrase(datosAnam.bTenesmo == true ? "X" : "", fonEiqueta));
            cellTenX.BorderWidth = 0;
            cellTenX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellTen = new PdfPCell(new Phrase("Tenesmo", fontDato));
            cellTen.Colspan = 7;
            cellTen.BorderWidth = 0;
            cellTen.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsDig = new PdfPCell(new Phrase("Observaciones:", fonEiqueta));
            cellObsDig.Colspan = 2;
            cellObsDig.BorderWidth = 0;
            cellObsDig.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsDigX = new PdfPCell(new Phrase(datosAnam.cObservadigestivo, fontDato));
            cellObsDigX.Colspan = 6;
            cellObsDigX.BorderWidth = 0;
            cellObsDigX.HorizontalAlignment = Element.ALIGN_LEFT;

            tblDig.AddCell(cellTitdig);
            tblDig.AddCell(cellHalX);
            tblDig.AddCell(cellHal);
            tblDig.AddCell(cellDisX);
            tblDig.AddCell(cellDis);
            tblDig.AddCell(cellRefX);
            tblDig.AddCell(cellRef);
            tblDig.AddCell(cellAnoX);
            tblDig.AddCell(cellAno);

            tblDig.AddCell(cellHipX);
            tblDig.AddCell(cellHip);
            tblDig.AddCell(cellOdiX);
            tblDig.AddCell(cellOdi);
            tblDig.AddCell(cellPolX);
            tblDig.AddCell(cellPol);
            tblDig.AddCell(cellNauX);
            tblDig.AddCell(cellNau);

            tblDig.AddCell(cellVomX);
            tblDig.AddCell(cellVom);
            tblDig.AddCell(cellDispX);
            tblDig.AddCell(cellDisp);
            tblDig.AddCell(cellRecX);
            tblDig.AddCell(cellRec);
            tblDig.AddCell(cellMelX);
            tblDig.AddCell(cellMel);

            tblDig.AddCell(cellPirX);
            tblDig.AddCell(cellPir);
            tblDig.AddCell(cellHemX);
            tblDig.AddCell(cellHem);
            tblDig.AddCell(cellAcoX);
            tblDig.AddCell(cellAco);
            tblDig.AddCell(cellMetX);
            tblDig.AddCell(cellMet);

            tblDig.AddCell(cellTenX);
            tblDig.AddCell(cellTen);

            tblDig.AddCell(cellObsDig);
            tblDig.AddCell(cellObsDigX);

            docHistoria.Add(tblDig);
            #endregion

            #region Aparato Respiratorio
            PdfPTable tblRes = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesResp = new float[8];
            valuesResp[0] = 30;
            valuesResp[1] = 110;
            valuesResp[2] = 30;
            valuesResp[3] = 110;
            valuesResp[4] = 30;
            valuesResp[5] = 110;
            valuesResp[6] = 30;
            valuesResp[7] = 110;

            tblRes.SetWidths(valuesResp);
            tblRes.HorizontalAlignment = 0;
            tblRes.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblRes.DefaultCell.Border = 0;

            PdfPCell cellTitResp = new PdfPCell(new Phrase("Aparato respiratorio", fonEiqueta));
            cellTitResp.Colspan = 8;
            cellTitResp.BorderWidth = 0;
            cellTitResp.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellDolX = new PdfPCell(new Phrase(datosAnam.bDolor == true ? "X" : "", fonEiqueta));
            cellDolX.BorderWidth = 0;
            cellDolX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDol = new PdfPCell(new Phrase("Dolor torácico", fontDato));
            cellDol.BorderWidth = 0;
            cellDol.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDisneaX = new PdfPCell(new Phrase(datosAnam.bDisnea == true ? "X" : "", fonEiqueta));
            cellDisneaX.BorderWidth = 0;
            cellDisneaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDisnea = new PdfPCell(new Phrase("Disnea", fontDato));
            cellDisnea.BorderWidth = 0;
            cellDisnea.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHemopX = new PdfPCell(new Phrase(datosAnam.bHemoptisis == true ? "X" : "", fonEiqueta));
            cellHemopX.BorderWidth = 0;
            cellHemopX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHemop = new PdfPCell(new Phrase("Hemoptisis", fontDato));
            cellHemop.BorderWidth = 0;
            cellHemop.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellSibX = new PdfPCell(new Phrase(datosAnam.bSibilancias == true ? "X" : "", fonEiqueta));
            cellSibX.BorderWidth = 0;
            cellSibX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellSib = new PdfPCell(new Phrase("Sibilancias", fontDato));
            cellSib.BorderWidth = 0;
            cellSib.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellCiaX = new PdfPCell(new Phrase(datosAnam.bCianosis == true ? "X" : "", fonEiqueta));
            cellCiaX.BorderWidth = 0;
            cellCiaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellCia = new PdfPCell(new Phrase("Cianosis", fontDato));
            cellCia.BorderWidth = 0;
            cellCia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celltosX = new PdfPCell(new Phrase(datosAnam.bTos == true ? "X" : "", fonEiqueta));
            celltosX.BorderWidth = 0;
            celltosX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell celltos = new PdfPCell(new Phrase("Tos", fontDato));
            celltos.BorderWidth = 0;
            celltos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellExpX = new PdfPCell(new Phrase(datosAnam.bExpectoracion == true ? "X" : "", fonEiqueta));
            cellExpX.BorderWidth = 0;
            cellExpX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellExp = new PdfPCell(new Phrase("Expectoración", fontDato));
            cellExp.BorderWidth = 0;
            cellExp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOrtX = new PdfPCell(new Phrase(datosAnam.bOrtopnea == true ? "X" : "", fonEiqueta));
            cellOrtX.BorderWidth = 0;
            cellOrtX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOrt = new PdfPCell(new Phrase("Ortopnea", fontDato));
            cellOrt.BorderWidth = 0;
            cellOrt.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellObsRes = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsRes.Colspan = 2;
            cellObsRes.BorderWidth = 0;
            cellObsRes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsResTex = new PdfPCell(new Phrase(datosAnam.cObservarespiratorio, fontDato));
            cellObsResTex.Colspan = 6;
            cellObsResTex.BorderWidth = 0;
            cellObsResTex.HorizontalAlignment = Element.ALIGN_LEFT;

            tblRes.AddCell(cellTitResp);
            tblRes.AddCell(cellDolX);
            tblRes.AddCell(cellDol);
            tblRes.AddCell(cellDisneaX);
            tblRes.AddCell(cellDisnea);
            tblRes.AddCell(cellHemopX);
            tblRes.AddCell(cellHemop);
            tblRes.AddCell(cellSibX);
            tblRes.AddCell(cellSib);

            tblRes.AddCell(cellCiaX);
            tblRes.AddCell(cellCia);
            tblRes.AddCell(celltosX);
            tblRes.AddCell(celltos);
            tblRes.AddCell(cellExpX);
            tblRes.AddCell(cellExp);
            tblRes.AddCell(cellOrtX);
            tblRes.AddCell(cellOrt);

            tblRes.AddCell(cellObsRes);
            tblRes.AddCell(cellObsResTex);

            docHistoria.Add(tblRes);
            #endregion

            #region Aparato cardiovascular
            PdfPTable tblCard = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesCard = new float[8];
            valuesCard[0] = 30;
            valuesCard[1] = 110;
            valuesCard[2] = 30;
            valuesCard[3] = 110;
            valuesCard[4] = 30;
            valuesCard[5] = 110;
            valuesCard[6] = 30;
            valuesCard[7] = 110;

            tblCard.SetWidths(valuesCard);
            tblCard.HorizontalAlignment = 0;
            tblCard.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblCard.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitCard = new PdfPCell(new Phrase("Aparato cardiovascular", fonEiqueta));
            cellTitCard.Colspan = 8;
            cellTitCard.BorderWidth = 0;
            cellTitCard.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDolPreX = new PdfPCell(new Phrase(datosAnam.bPrecordial == true ? "X" : "", fonEiqueta));
            cellDolPreX.BorderWidth = 0;
            cellDolPreX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDolPre = new PdfPCell(new Phrase("Dolor Precordial", fontDato));
            cellDolPre.BorderWidth = 0;
            cellDolPre.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellEdeX = new PdfPCell(new Phrase(datosAnam.bEdema == true ? "X" : "", fonEiqueta));
            cellEdeX.BorderWidth = 0;
            cellEdeX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEde = new PdfPCell(new Phrase("Edema", fontDato));
            cellEde.BorderWidth = 0;
            cellEde.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDisCardX = new PdfPCell(new Phrase(datosAnam.bDisneacardiovascular == true ? "X" : "", fonEiqueta));
            cellDisCardX.BorderWidth = 0;
            cellDisCardX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDisCard = new PdfPCell(new Phrase("Disnea", fontDato));
            cellDisCard.BorderWidth = 0;
            cellDisCard.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPalX = new PdfPCell(new Phrase(datosAnam.bPalpitacion == true ? "X" : "", fonEiqueta));
            cellPalX.BorderWidth = 0;
            cellPalX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPal = new PdfPCell(new Phrase("Palpitaciones", fontDato));
            cellPal.BorderWidth = 0;
            cellPal.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellSinX = new PdfPCell(new Phrase(datosAnam.bSincope == true ? "X" : "", fonEiqueta));
            cellSinX.BorderWidth = 0;
            cellSinX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellSin = new PdfPCell(new Phrase("Síncope", fontDato));
            cellSin.BorderWidth = 0;
            cellSin.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellClaX = new PdfPCell(new Phrase(datosAnam.bClaudicacion == true ? "X" : "", fonEiqueta));
            cellClaX.BorderWidth = 0;
            cellClaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellCla = new PdfPCell(new Phrase("Claudicación", fontDato));
            cellCla.Colspan = 5;
            cellCla.BorderWidth = 0;
            cellCla.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellObsCardio = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsCardio.Colspan = 2;
            cellObsCardio.BorderWidth = 0;
            cellObsCardio.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsCardioText = new PdfPCell(new Phrase(datosAnam.cObservacardiovascular, fontDato));
            cellObsCardioText.Colspan = 6;
            cellObsCardioText.BorderWidth = 0;
            cellObsCardioText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblCard.AddCell(cellTitCard);
            tblCard.AddCell(cellDolPreX);
            tblCard.AddCell(cellDolPre);
            tblCard.AddCell(cellEdeX);
            tblCard.AddCell(cellEde);
            tblCard.AddCell(cellDisCardX);
            tblCard.AddCell(cellDisCard);
            tblCard.AddCell(cellPalX);
            tblCard.AddCell(cellPal);

            tblCard.AddCell(cellSinX);
            tblCard.AddCell(cellSin);
            tblCard.AddCell(cellClaX);
            tblCard.AddCell(cellCla);

            tblCard.AddCell(cellObsCardio);
            tblCard.AddCell(cellObsCardioText);

            docHistoria.Add(tblCard);
            #endregion

            #region Aparato urinario
            PdfPTable tblUri = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesUri = new float[8];
            valuesUri[0] = 30;
            valuesUri[1] = 110;
            valuesUri[2] = 30;
            valuesUri[3] = 110;
            valuesUri[4] = 30;
            valuesUri[5] = 110;
            valuesUri[6] = 30;
            valuesUri[7] = 110;

            tblUri.SetWidths(valuesUri);
            tblUri.HorizontalAlignment = 0;
            tblUri.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblUri.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitUri = new PdfPCell(new Phrase("Aparato urinario", fonEiqueta));
            cellTitUri.Colspan = 8;
            cellTitUri.BorderWidth = 0;
            cellTitUri.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDolLumX = new PdfPCell(new Phrase(datosAnam.bLumbar == true ? "X" : "", fonEiqueta));
            cellDolLumX.BorderWidth = 0;
            cellDolLumX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDolLum = new PdfPCell(new Phrase("Dolor lumbar", fontDato));
            cellDolLum.BorderWidth = 0;
            cellDolLum.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDisuriaX = new PdfPCell(new Phrase(datosAnam.bDisuria == true ? "X" : "", fonEiqueta));
            cellDisuriaX.BorderWidth = 0;
            cellDisuriaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDisuria = new PdfPCell(new Phrase("Disuria", fontDato));
            cellDisuria.BorderWidth = 0;
            cellDisuria.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPolaqX = new PdfPCell(new Phrase(datosAnam.bPolaquiuria == true ? "X" : "", fonEiqueta));
            cellPolaqX.BorderWidth = 0;
            cellPolaqX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPolaq = new PdfPCell(new Phrase("Polaquiuria", fontDato));
            cellPolaq.BorderWidth = 0;
            cellPolaq.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellIncX = new PdfPCell(new Phrase(datosAnam.bIncontinencia == true ? "X" : "", fonEiqueta));
            cellIncX.BorderWidth = 0;
            cellIncX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellInc = new PdfPCell(new Phrase("Incontinencia", fontDato));
            cellInc.BorderWidth = 0;
            cellInc.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellPoliuX = new PdfPCell(new Phrase(datosAnam.bPoliuria == true ? "X" : "", fonEiqueta));
            cellPoliuX.BorderWidth = 0;
            cellPoliuX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPoliu = new PdfPCell(new Phrase("Poliuria", fontDato));
            cellPoliu.BorderWidth = 0;
            cellPoliu.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOliX = new PdfPCell(new Phrase(datosAnam.bOliguria == true ? "X" : "", fonEiqueta));
            cellOliX.BorderWidth = 0;
            cellOliX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOli = new PdfPCell(new Phrase("Oliguria", fontDato));
            cellOli.BorderWidth = 0;
            cellOli.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellNicX = new PdfPCell(new Phrase(datosAnam.bNicturia == true ? "X" : "", fonEiqueta));
            cellNicX.BorderWidth = 0;
            cellNicX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellNic = new PdfPCell(new Phrase("Nicturia", fontDato));
            cellNic.BorderWidth = 0;
            cellNic.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHematX = new PdfPCell(new Phrase(datosAnam.bHematuria == true ? "X" : "", fonEiqueta));
            cellHematX.BorderWidth = 0;
            cellHematX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHemat = new PdfPCell(new Phrase("Hematuria", fontDato));
            cellHemat.BorderWidth = 0;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellTenUriX = new PdfPCell(new Phrase(datosAnam.bTenesmourinario == true ? "X" : "", fonEiqueta));
            cellTenUriX.BorderWidth = 0;
            cellTenUriX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellTenUri = new PdfPCell(new Phrase("Tenesmo", fontDato));
            cellTenUri.BorderWidth = 0;
            cellTenUri.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAnuX = new PdfPCell(new Phrase(datosAnam.bOliguria == true ? "X" : "", fonEiqueta));
            cellAnuX.BorderWidth = 0;
            cellAnuX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAnu = new PdfPCell(new Phrase("Anuria", fontDato));
            cellAnu.Colspan = 5;
            cellAnu.BorderWidth = 0;
            cellAnu.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------4a Linea
            PdfPCell cellObsUri = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsUri.Colspan = 2;
            cellObsUri.BorderWidth = 0;
            cellObsUri.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsUriText = new PdfPCell(new Phrase(datosAnam.cObservaurinario, fontDato));
            cellObsUriText.Colspan = 6;
            cellObsUriText.BorderWidth = 0;
            cellObsUriText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblUri.AddCell(cellTitUri);
            tblUri.AddCell(cellDolLumX);
            tblUri.AddCell(cellDolLum);
            tblUri.AddCell(cellDisuriaX);
            tblUri.AddCell(cellDisuria);
            tblUri.AddCell(cellPolaqX);
            tblUri.AddCell(cellPolaq);
            tblUri.AddCell(cellIncX);
            tblUri.AddCell(cellInc);

            tblUri.AddCell(cellTenUriX);
            tblUri.AddCell(cellTenUri);
            tblUri.AddCell(cellAnuX);
            tblUri.AddCell(cellAnu);

            tblUri.AddCell(cellObsUri);
            tblUri.AddCell(cellObsUriText);

            docHistoria.Add(tblUri);
            #endregion

            #region Aparato genital
            PdfPTable tblGen = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesGen = new float[8];
            valuesGen[0] = 30;
            valuesGen[1] = 110;
            valuesGen[2] = 30;
            valuesGen[3] = 110;
            valuesGen[4] = 30;
            valuesGen[5] = 110;
            valuesGen[6] = 30;
            valuesGen[7] = 110;

            tblGen.SetWidths(valuesGen);
            tblGen.HorizontalAlignment = 0;
            tblGen.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblGen.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitGenital = new PdfPCell(new Phrase("Aparato genital", fonEiqueta));
            cellTitGenital.Colspan = 8;
            cellTitGenital.BorderWidth = 0;
            cellTitGenital.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHipermTex = new PdfPCell(new Phrase(datosAnam.bHipermenorrea == true ? "X" : "", fonEiqueta));
            cellHipermTex.BorderWidth = 0;
            cellHipermTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHipermTe = new PdfPCell(new Phrase("Hipermenorrea", fontDato));
            cellHipermTe.BorderWidth = 0;
            cellHipermTe.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHipomTex = new PdfPCell(new Phrase(datosAnam.bHipomenorrea == true ? "X" : "", fonEiqueta));
            cellHipomTex.BorderWidth = 0;
            cellHipomTex.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHipomTe = new PdfPCell(new Phrase("Hipomenorrea", fontDato));
            cellHipomTe.BorderWidth = 0;
            cellHipomTe.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAmeX = new PdfPCell(new Phrase(datosAnam.bAmenorrea == true ? "X" : "", fonEiqueta));
            cellAmeX.BorderWidth = 0;
            cellAmeX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAme = new PdfPCell(new Phrase("Amenorrea", fontDato));
            cellAme.BorderWidth = 0;
            cellAme.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDispaX = new PdfPCell(new Phrase(datosAnam.bDispareunia == true ? "X" : "", fonEiqueta));
            cellDispaX.BorderWidth = 0;
            cellDispaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDispa = new PdfPCell(new Phrase("Dispareunia", fontDato));
            cellDispa.BorderWidth = 0;
            cellDispa.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellMetroX = new PdfPCell(new Phrase(datosAnam.bMetrorragia == true ? "X" : "", fonEiqueta));
            cellMetroX.BorderWidth = 0;
            cellMetroX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMetro = new PdfPCell(new Phrase("Metrorragia", fontDato));
            cellMetro.BorderWidth = 0;
            cellMetro.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellLeuX = new PdfPCell(new Phrase(datosAnam.bLeucorrea == true ? "X" : "", fonEiqueta));
            cellLeuX.BorderWidth = 0;
            cellLeuX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellLeu = new PdfPCell(new Phrase("Leucorrea", fontDato));
            cellLeu.BorderWidth = 0;
            cellLeu.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDismenX = new PdfPCell(new Phrase(datosAnam.bDismenorrea == true ? "X" : "", fonEiqueta));
            cellDismenX.BorderWidth = 0;
            cellDismenX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDismen = new PdfPCell(new Phrase("Dismenorrea", fontDato));
            cellDismen.Colspan = 3;
            cellDismen.BorderWidth = 0;
            cellDismen.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellObGenital = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObGenital.Colspan = 2;
            cellObGenital.BorderWidth = 0;
            cellObGenital.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObGenitalText = new PdfPCell(new Phrase(datosAnam.cObservagenital, fontDato));
            cellObGenitalText.Colspan = 6;
            cellObGenitalText.BorderWidth = 0;
            cellObGenitalText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblGen.AddCell(cellTitGenital);
            tblGen.AddCell(cellHipermTex);
            tblGen.AddCell(cellHipermTe);
            tblGen.AddCell(cellHipomTex);
            tblGen.AddCell(cellHipomTe);
            tblGen.AddCell(cellAmeX);
            tblGen.AddCell(cellAme);
            tblGen.AddCell(cellDispaX);
            tblGen.AddCell(cellDispa);

            tblGen.AddCell(cellMetroX);
            tblGen.AddCell(cellMetro);
            tblGen.AddCell(cellLeuX);
            tblGen.AddCell(cellLeu);
            tblGen.AddCell(cellDismenX);
            tblGen.AddCell(cellDismen);

            tblGen.AddCell(cellObGenital);
            tblGen.AddCell(cellObGenitalText);

            docHistoria.Add(tblGen);

            #endregion

            #region Sistema nervioso
            PdfPTable tblNer = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesNer = new float[8];
            valuesNer[0] = 30;
            valuesNer[1] = 110;
            valuesNer[2] = 30;
            valuesNer[3] = 110;
            valuesNer[4] = 30;
            valuesNer[5] = 110;
            valuesNer[6] = 30;
            valuesNer[7] = 110;

            tblNer.SetWidths(valuesNer);
            tblNer.HorizontalAlignment = 0;
            tblNer.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblNer.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitNer = new PdfPCell(new Phrase("Sistema nervioso", fonEiqueta));
            cellTitNer.Colspan = 8;
            cellTitNer.BorderWidth = 0;
            cellTitNer.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellCefX = new PdfPCell(new Phrase(datosAnam.bCefalea == true ? "X" : "", fonEiqueta));
            cellCefX.BorderWidth = 0;
            cellCefX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellCef = new PdfPCell(new Phrase("Cefalea", fontDato));
            cellCef.BorderWidth = 0;
            cellCef.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellConX = new PdfPCell(new Phrase(datosAnam.bConvulsiones == true ? "X" : "", fonEiqueta));
            cellConX.BorderWidth = 0;
            cellConX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellCon = new PdfPCell(new Phrase("Convulsiones", fontDato));
            cellCon.BorderWidth = 0;
            cellCon.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObnX = new PdfPCell(new Phrase(datosAnam.bObnubilacion == true ? "X" : "", fonEiqueta));
            cellObnX.BorderWidth = 0;
            cellObnX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellObn = new PdfPCell(new Phrase("Obnubilación", fontDato));
            cellObn.BorderWidth = 0;
            cellObn.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMarX = new PdfPCell(new Phrase(datosAnam.bMarcha == true ? "X" : "", fonEiqueta));
            cellMarX.BorderWidth = 0;
            cellMarX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMar = new PdfPCell(new Phrase("Marcha", fontDato));
            cellMar.BorderWidth = 0;
            cellMar.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellMemX = new PdfPCell(new Phrase(datosAnam.bMemoria == true ? "X" : "", fonEiqueta));
            cellMemX.BorderWidth = 0;
            cellMemX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMem = new PdfPCell(new Phrase("Memoria", fontDato));
            cellMem.BorderWidth = 0;
            cellMem.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellEquiX = new PdfPCell(new Phrase(datosAnam.bEquilibrio == true ? "X" : "", fonEiqueta));
            cellEquiX.BorderWidth = 0;
            cellEquiX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEqui = new PdfPCell(new Phrase("Equilibirio", fontDato));
            cellEqui.BorderWidth = 0;
            cellEqui.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellLenX = new PdfPCell(new Phrase(datosAnam.bLenguaje == true ? "X" : "", fonEiqueta));
            cellLenX.BorderWidth = 0;
            cellLenX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellLen = new PdfPCell(new Phrase("Lenguaje", fontDato));
            cellLen.BorderWidth = 0;
            cellLen.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellVigX = new PdfPCell(new Phrase(datosAnam.bVigilia == true ? "X" : "", fonEiqueta));
            cellVigX.BorderWidth = 0;
            cellVigX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellVig = new PdfPCell(new Phrase("Sueño - Vigilia", fontDato));
            cellVig.BorderWidth = 0;
            cellVig.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellSenX = new PdfPCell(new Phrase(datosAnam.bSensibilidad == true ? "X" : "", fonEiqueta));
            cellSenX.BorderWidth = 0;
            cellSenX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellSen = new PdfPCell(new Phrase("Sensibilidad", fontDato));
            cellSen.BorderWidth = 0;
            cellSen.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellParaX = new PdfPCell(new Phrase(datosAnam.bParalisis == true ? "X" : "", fonEiqueta));
            cellParaX.BorderWidth = 0;
            cellParaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellPara = new PdfPCell(new Phrase("Parálisis", fontDato));
            cellPara.Colspan = 5;
            cellPara.BorderWidth = 0;
            cellPara.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------4a Linea
            PdfPCell cellObsNer = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsNer.Colspan = 2;
            cellObsNer.BorderWidth = 0;
            cellObsNer.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsNerText = new PdfPCell(new Phrase(datosAnam.cObservanervioso, fontDato));
            cellObsNerText.Colspan = 6;
            cellObsNerText.BorderWidth = 0;
            cellObsNerText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblNer.AddCell(cellTitNer);
            tblNer.AddCell(cellCefX);
            tblNer.AddCell(cellCef);
            tblNer.AddCell(cellConX);
            tblNer.AddCell(cellCon);
            tblNer.AddCell(cellObnX);
            tblNer.AddCell(cellObn);
            tblNer.AddCell(cellMarX);
            tblNer.AddCell(cellMar);

            tblNer.AddCell(cellMemX);
            tblNer.AddCell(cellMem);
            tblNer.AddCell(cellEquiX);
            tblNer.AddCell(cellEqui);
            tblNer.AddCell(cellLenX);
            tblNer.AddCell(cellLen);
            tblNer.AddCell(cellVigX);
            tblNer.AddCell(cellVig);

            tblNer.AddCell(cellObsNer);
            tblNer.AddCell(cellObsNerText);

            docHistoria.Add(tblNer);

            #endregion

            #region Endocrino
            PdfPTable tblEnd = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valEndl = new float[8];
            valEndl[0] = 30;
            valEndl[1] = 110;
            valEndl[2] = 30;
            valEndl[3] = 110;
            valEndl[4] = 30;
            valEndl[5] = 110;
            valEndl[6] = 30;
            valEndl[7] = 110;

            tblEnd.SetWidths(valEndl);
            tblEnd.HorizontalAlignment = 0;
            tblEnd.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblEnd.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitEnd = new PdfPCell(new Phrase("Endocrino", fonEiqueta));
            cellTitEnd.Colspan = 8;
            cellTitEnd.BorderWidth = 0;
            cellTitEnd.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellBocX = new PdfPCell(new Phrase(datosAnam.bBocio == true ? "X" : "", fonEiqueta));
            cellBocX.BorderWidth = 0;
            cellBocX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellBoc = new PdfPCell(new Phrase("Bocio", fontDato));
            cellBoc.BorderWidth = 0;
            cellBoc.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellLetX = new PdfPCell(new Phrase(datosAnam.bLeargia == true ? "X" : "", fonEiqueta));
            cellLetX.BorderWidth = 0;
            cellLetX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellLet = new PdfPCell(new Phrase("Letargia", fontDato));
            cellLet.BorderWidth = 0;
            cellLet.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellIntX = new PdfPCell(new Phrase(datosAnam.bIntolerancia == true ? "X" : "", fonEiqueta));
            cellIntX.BorderWidth = 0;
            cellIntX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellInt = new PdfPCell(new Phrase("Intolerancia", fontDato));
            cellInt.BorderWidth = 0;
            cellInt.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellBochX = new PdfPCell(new Phrase(datosAnam.bBochornos == true ? "X" : "", fonEiqueta));
            cellBochX.BorderWidth = 0;
            cellBochX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellBoch = new PdfPCell(new Phrase("Bochornos", fontDato));
            cellBoch.BorderWidth = 0;
            cellBoch.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellObsEnd = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsEnd.Colspan = 2;
            cellObsEnd.BorderWidth = 0;
            cellObsEnd.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsEndText = new PdfPCell(new Phrase(datosAnam.cObservaendocrino, fontDato));
            cellObsEndText.Colspan = 6;
            cellObsEndText.BorderWidth = 0;
            cellObsEndText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblEnd.AddCell(cellTitEnd);
            tblEnd.AddCell(cellBocX);
            tblEnd.AddCell(cellBoc);
            tblEnd.AddCell(cellLetX);
            tblEnd.AddCell(cellLet);
            tblEnd.AddCell(cellIntX);
            tblEnd.AddCell(cellInt);
            tblEnd.AddCell(cellBochX);
            tblEnd.AddCell(cellBoch);

            tblEnd.AddCell(cellObsEnd);
            tblEnd.AddCell(cellObsEndText);

            docHistoria.Add(tblEnd);

            #endregion

            docHistoria.NewPage();

            #region Oftalmológico
            PdfPTable tblOft = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valOft = new float[8];
            valOft[0] = 30;
            valOft[1] = 110;
            valOft[2] = 30;
            valOft[3] = 110;
            valOft[4] = 30;
            valOft[5] = 110;
            valOft[6] = 30;
            valOft[7] = 110;

            tblOft.SetWidths(valOft);
            tblOft.HorizontalAlignment = 0;
            tblOft.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblOft.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitOft = new PdfPCell(new Phrase("Oftalmológico", fonEiqueta));
            cellTitOft.Colspan = 8;
            cellTitOft.BorderWidth = 0;
            cellTitOft.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDipX = new PdfPCell(new Phrase(datosAnam.bDiplopia == true ? "X" : "", fonEiqueta));
            cellDipX.BorderWidth = 0;
            cellDipX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDip = new PdfPCell(new Phrase("Diplopia", fontDato));
            cellDip.BorderWidth = 0;
            cellDip.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDolOcuX = new PdfPCell(new Phrase(datosAnam.bOcular == true ? "X" : "", fonEiqueta));
            cellDolOcuX.BorderWidth = 0;
            cellDolOcuX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDolOcu = new PdfPCell(new Phrase("Dolor ocular", fontDato));
            cellDolOcu.BorderWidth = 0;
            cellDolOcu.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFotX = new PdfPCell(new Phrase(datosAnam.bFotobia == true ? "X" : "", fonEiqueta));
            cellFotX.BorderWidth = 0;
            cellFotX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellFot = new PdfPCell(new Phrase("Fotofobia", fontDato));
            cellFot.BorderWidth = 0;
            cellFot.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellAmaX = new PdfPCell(new Phrase(datosAnam.bAmaurosis == true ? "X" : "", fonEiqueta));
            cellAmaX.BorderWidth = 0;
            cellAmaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellAma = new PdfPCell(new Phrase("Amaurosis", fontDato));
            cellAma.BorderWidth = 0;
            cellAma.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellFotopsiasX = new PdfPCell(new Phrase(datosAnam.bFotopsias == true ? "X" : "", fonEiqueta));
            cellFotopsiasX.BorderWidth = 0;
            cellFotopsiasX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellFotopsias = new PdfPCell(new Phrase("Fotopsias", fontDato));
            cellFotopsias.BorderWidth = 0;
            cellFotopsias.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMioX = new PdfPCell(new Phrase(datosAnam.bMiodesopsias == true ? "X" : "", fonEiqueta));
            cellMioX.BorderWidth = 0;
            cellMioX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMio = new PdfPCell(new Phrase("Miodedopsias", fontDato));
            cellMio.BorderWidth = 0;
            cellMio.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellEscozorX = new PdfPCell(new Phrase(datosAnam.bEscozor == true ? "X" : "", fonEiqueta));
            cellEscozorX.BorderWidth = 0;
            cellEscozorX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEscozor = new PdfPCell(new Phrase("Escozor", fontDato));
            cellEscozor.BorderWidth = 0;
            cellEscozor.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellLegX = new PdfPCell(new Phrase(datosAnam.bLeganas == true ? "X" : "", fonEiqueta));
            cellLegX.BorderWidth = 0;
            cellLegX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellLeg = new PdfPCell(new Phrase("Amaurosis", fontDato));
            cellLeg.BorderWidth = 0;
            cellLeg.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellObsOftal = new PdfPCell(new Phrase("Oftalmológico", fonEiqueta));
            cellObsOftal.Colspan = 2;
            cellObsOftal.BorderWidth = 0;
            cellObsOftal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsOftalText = new PdfPCell(new Phrase(datosAnam.cObservaoftamologico, fonEiqueta));
            cellObsOftalText.Colspan = 6;
            cellObsOftalText.BorderWidth = 0;
            cellObsOftalText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblOft.AddCell(cellTitOft);
            tblOft.AddCell(cellDipX);
            tblOft.AddCell(cellDip);
            tblOft.AddCell(cellDolOcuX);
            tblOft.AddCell(cellDolOcu);
            tblOft.AddCell(cellFotX);
            tblOft.AddCell(cellFot);
            tblOft.AddCell(cellAmaX);
            tblOft.AddCell(cellAma);

            tblOft.AddCell(cellFotopsiasX);
            tblOft.AddCell(cellFotopsias);
            tblOft.AddCell(cellMioX);
            tblOft.AddCell(cellMio);
            tblOft.AddCell(cellEscozorX);
            tblOft.AddCell(cellEscozor);
            tblOft.AddCell(cellLegX);
            tblOft.AddCell(cellLeg);

            tblOft.AddCell(cellObsOftal);
            tblOft.AddCell(cellObsOftalText);

            docHistoria.Add(tblOft);
            #endregion

            #region Otorrino
            PdfPTable tblOto = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valOto = new float[8];
            valOto[0] = 30;
            valOto[1] = 110;
            valOto[2] = 30;
            valOto[3] = 110;
            valOto[4] = 30;
            valOto[5] = 110;
            valOto[6] = 30;
            valOto[7] = 110;

            tblOto.SetWidths(valOto);
            tblOto.HorizontalAlignment = 0;
            tblOto.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblOto.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitOto = new PdfPCell(new Phrase("Otorrino", fonEiqueta));
            cellTitOto.Colspan = 8;
            cellTitOto.BorderWidth = 0;
            cellTitOto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOtaX = new PdfPCell(new Phrase(datosAnam.bOtalgia == true ? "X" : "", fonEiqueta));
            cellOtaX.BorderWidth = 0;
            cellOtaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOta = new PdfPCell(new Phrase("Otalgia", fontDato));
            cellOta.BorderWidth = 0;
            cellOta.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOtorreaX = new PdfPCell(new Phrase(datosAnam.bOtorrea == true ? "X" : "", fonEiqueta));
            cellOtorreaX.BorderWidth = 0;
            cellOtorreaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOtorrea = new PdfPCell(new Phrase("Otorrea", fontDato));
            cellOtorrea.BorderWidth = 0;
            cellOtorrea.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOtorragiaX = new PdfPCell(new Phrase(datosAnam.bOtorragia == true ? "X" : "", fonEiqueta));
            cellOtorragiaX.BorderWidth = 0;
            cellOtorragiaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOtorragia = new PdfPCell(new Phrase("Otorrogia", fontDato));
            cellOtorragia.BorderWidth = 0;
            cellOtorragia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellHipoacusiaX = new PdfPCell(new Phrase(datosAnam.bHipoacusia == true ? "X" : "", fonEiqueta));
            cellHipoacusiaX.BorderWidth = 0;
            cellHipoacusiaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellHipoacusia = new PdfPCell(new Phrase("Hipoacusia", fontDato));
            cellHipoacusia.BorderWidth = 0;
            cellHipoacusia.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellEpistaxisX = new PdfPCell(new Phrase(datosAnam.bEpistaxis == true ? "X" : "", fonEiqueta));
            cellEpistaxisX.BorderWidth = 0;
            cellEpistaxisX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEpistaxis = new PdfPCell(new Phrase("Epistaxis", fontDato));
            cellEpistaxis.BorderWidth = 0;
            cellEpistaxis.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellRinX = new PdfPCell(new Phrase(datosAnam.bRinorrea == true ? "X" : "", fonEiqueta));
            cellRinX.BorderWidth = 0;
            cellRinX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellRin = new PdfPCell(new Phrase("Rinorrea", fontDato));
            cellRin.BorderWidth = 0;
            cellRin.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellOdinoX = new PdfPCell(new Phrase(datosAnam.bOdinofagia == true ? "X" : "", fonEiqueta));
            cellOdinoX.BorderWidth = 0;
            cellOdinoX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellOdino = new PdfPCell(new Phrase("Odinofagia", fontDato));
            cellOdino.BorderWidth = 0;
            cellOdino.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFonX = new PdfPCell(new Phrase(datosAnam.bFonacion == true ? "X" : "", fonEiqueta));
            cellFonX.BorderWidth = 0;
            cellFonX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellFon = new PdfPCell(new Phrase("Fonación", fontDato));
            cellFon.BorderWidth = 0;
            cellFon.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellObsFonacion = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsFonacion.Colspan = 2;
            cellObsFonacion.BorderWidth = 0;
            cellObsFonacion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsFonacionTex = new PdfPCell(new Phrase(datosAnam.cObservaotorrino, fontDato));
            cellObsFonacionTex.Colspan = 6;
            cellObsFonacionTex.BorderWidth = 0;
            cellObsFonacionTex.HorizontalAlignment = Element.ALIGN_LEFT;

            tblOto.AddCell(cellTitOto);
            tblOto.AddCell(cellOtaX);
            tblOto.AddCell(cellOta);
            tblOto.AddCell(cellOtorreaX);
            tblOto.AddCell(cellOtorrea);
            tblOto.AddCell(cellOtorragiaX);
            tblOto.AddCell(cellOtorragia);
            tblOto.AddCell(cellHipoacusiaX);
            tblOto.AddCell(cellHipoacusiaX);

            tblOto.AddCell(cellEpistaxisX);
            tblOto.AddCell(cellEpistaxis);
            tblOto.AddCell(cellRinX);
            tblOto.AddCell(cellRin);
            tblOto.AddCell(cellOdinoX);
            tblOto.AddCell(cellOdino);
            tblOto.AddCell(cellFonX);
            tblOto.AddCell(cellFon);

            tblOto.AddCell(cellObsFonacion);
            tblOto.AddCell(cellObsFonacionTex);

            docHistoria.Add(tblOto);
            #endregion

            #region Locomotor
            PdfPTable tblLoc = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valLoc = new float[8];
            valLoc[0] = 30;
            valLoc[1] = 110;
            valLoc[2] = 30;
            valLoc[3] = 110;
            valLoc[4] = 30;
            valLoc[5] = 110;
            valLoc[6] = 30;
            valLoc[7] = 110;

            tblLoc.SetWidths(valLoc);
            tblLoc.HorizontalAlignment = 0;
            tblLoc.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblLoc.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitLoc = new PdfPCell(new Phrase("Locomotor", fonEiqueta));
            cellTitLoc.Colspan = 8;
            cellTitLoc.BorderWidth = 0;
            cellTitLoc.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell cellFueX = new PdfPCell(new Phrase(datosAnam.bFuerza == true ? "X" : "", fonEiqueta));
            cellFueX.BorderWidth = 0;
            cellFueX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellFue = new PdfPCell(new Phrase("Fuerza muscular", fontDato));
            cellFue.BorderWidth = 0;
            cellFue.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDefX = new PdfPCell(new Phrase(datosAnam.bDeformidades == true ? "X" : "", fonEiqueta));
            cellDefX.BorderWidth = 0;
            cellDefX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellDef = new PdfPCell(new Phrase("Deformidades", fontDato));
            cellDef.BorderWidth = 0;
            cellDef.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMiaX = new PdfPCell(new Phrase(datosAnam.bMialgias == true ? "X" : "", fonEiqueta));
            cellMiaX.BorderWidth = 0;
            cellMiaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellMia = new PdfPCell(new Phrase("Mialgias", fontDato));
            cellMia.BorderWidth = 0;
            cellMia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellArtX = new PdfPCell(new Phrase(datosAnam.bArtralgias == true ? "X" : "", fonEiqueta));
            cellArtX.BorderWidth = 0;
            cellArtX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellArt = new PdfPCell(new Phrase("Artralgias", fontDato));
            cellArt.BorderWidth = 0;
            cellArt.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellrigX = new PdfPCell(new Phrase(datosAnam.bRigidez == true ? "X" : "", fonEiqueta));
            cellrigX.BorderWidth = 0;
            cellrigX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellrig = new PdfPCell(new Phrase("Rigidez articular", fontDato));
            cellrig.BorderWidth = 0;
            cellrig.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellEdemaX = new PdfPCell(new Phrase(datosAnam.bEdemalocomotor == true ? "X" : "", fonEiqueta));
            cellEdemaX.BorderWidth = 0;
            cellEdemaX.HorizontalAlignment = Element.ALIGN_RIGHT;

            PdfPCell cellEdema = new PdfPCell(new Phrase("Edema", fontDato));
            cellEdema.Colspan = 5;
            cellEdema.BorderWidth = 0;
            cellEdema.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsLocomotor = new PdfPCell(new Phrase("Observaciones", fonEiqueta));
            cellObsLocomotor.Colspan = 2;
            cellObsLocomotor.BorderWidth = 0;
            cellObsLocomotor.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellObsLocomotorText = new PdfPCell(new Phrase(datosAnam.cObservalocomotor, fontDato));
            cellObsLocomotorText.Colspan = 6;
            cellObsLocomotorText.BorderWidth = 0;
            cellObsLocomotorText.HorizontalAlignment = Element.ALIGN_LEFT;

            tblLoc.AddCell(cellTitLoc);
            tblLoc.AddCell(cellFueX);
            tblLoc.AddCell(cellFue);
            tblLoc.AddCell(cellDefX);
            tblLoc.AddCell(cellDef);
            tblLoc.AddCell(cellMiaX);
            tblLoc.AddCell(cellMia);
            tblLoc.AddCell(cellArtX);
            tblLoc.AddCell(cellArt);

            tblLoc.AddCell(cellrigX);
            tblLoc.AddCell(cellrig);
            tblLoc.AddCell(cellEdemaX);
            tblLoc.AddCell(cellEdema);

            tblLoc.AddCell(cellObsLocomotor);
            tblLoc.AddCell(cellObsLocomotorText);

            docHistoria.Add(tblLoc);
            #endregion

            #endregion

            #region VII Exploracion fisica
            PdfPTable tableExpFis = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            tableExpFis.SetWidths(widthsTitulosGenerales);
            tableExpFis.HorizontalAlignment = Element.ALIGN_LEFT;
            tableExpFis.SpacingBefore = 5f;
            tableExpFis.SpacingAfter = 10f;

            PdfPCell cellTituloExploracion = new PdfPCell(new Phrase("VII Exploración física", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            tableExpFis.AddCell(cellTituloExploracion);

            docHistoria.Add(tableExpFis);
            #endregion

            #region VII Detalles exploracion física
            PdfPTable tblsignos = new PdfPTable(4)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valSig = new float[4];
            valSig[0] = 180;
            valSig[1] = 100;
            valSig[2] = 180;
            valSig[3] = 100;

            tblsignos.SetWidths(valSig);
            tblsignos.HorizontalAlignment = 0;
            tblsignos.SpacingAfter = 5f;
            //tblAndro.SpacingBefore = 5f;
            tblsignos.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellTitSignos = new PdfPCell(new Phrase("Signos vitales", fonEiqueta));
            cellTitSignos.Colspan = 4;
            cellTitSignos.BorderWidth = 0;
            cellTitSignos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellTensionX = new PdfPCell(new Phrase("Tensión arterial", fonEiqueta));
            cellTensionX.BorderWidth = 0;
            cellTensionX.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellTensionValor = new PdfPCell(new Phrase(datosInter.pa_tension, fontDato));
            cellTensionValor.BorderWidth = 0;
            cellTensionValor.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTensionValor.BorderWidthBottom = 1;

            PdfPCell cellFrecCard = new PdfPCell(new Phrase("Frecuencia cardiaca", fonEiqueta));
            cellFrecCard.BorderWidth = 0;
            cellFrecCard.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFrecCardValor = new PdfPCell(new Phrase(datosInter.pa_frec_card, fontDato));
            cellFrecCardValor.BorderWidth = 0;
            cellFrecCardValor.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------2a Linea
            PdfPCell celFreccRes = new PdfPCell(new Phrase("Frecuencia respiratoria", fonEiqueta));
            celFreccRes.BorderWidth = 0;
            celFreccRes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFreccResp = new PdfPCell(new Phrase(datosInter.pa_frec_resp, fontDato));
            cellFreccResp.BorderWidth = 0;
            cellFreccResp.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellTemperatura = new PdfPCell(new Phrase("Temperatura", fonEiqueta));
            cellTemperatura.BorderWidth = 0;
            cellTemperatura.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellTemperaturaValor = new PdfPCell(new Phrase(datosInter.pa_temperatura, fontDato));
            cellTemperaturaValor.BorderWidth = 0;
            cellTemperaturaValor.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------3a Linea
            PdfPCell cellPeso = new PdfPCell(new Phrase("Peso", fonEiqueta));
            cellPeso.BorderWidth = 0;
            cellPeso.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPesoValor = new PdfPCell(new Phrase(datosInter.pa_peso, fontDato));
            cellPesoValor.BorderWidth = 0;
            cellPesoValor.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellTalla = new PdfPCell(new Phrase("Talla", fonEiqueta));
            cellTalla.BorderWidth = 0;
            cellTalla.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellTallaValor = new PdfPCell(new Phrase(datosInter.pa_talla, fontDato));
            cellTallaValor.BorderWidth = 0;
            cellTallaValor.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------4a Linea
            PdfPCell cellMasa = new PdfPCell(new Phrase("Indica Masa Corporal", fonEiqueta));
            cellMasa.BorderWidth = 0;
            cellMasa.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellMasaValor = new PdfPCell(new Phrase(datosInter.pa_masa, fontDato));
            cellMasaValor.BorderWidth = 0;
            cellMasaValor.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellPAbdominal = new PdfPCell(new Phrase("P. Abdominal", fonEiqueta));
            cellPAbdominal.BorderWidth = 0;
            cellPAbdominal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellPAbdominalValor = new PdfPCell(new Phrase(datosInter.pCintura, fontDato));
            cellPAbdominalValor.BorderWidth = 0;
            cellPAbdominalValor.HorizontalAlignment = Element.ALIGN_CENTER;

            tblsignos.AddCell(cellTitSignos);
            tblsignos.AddCell(cellTensionX);
            tblsignos.AddCell(cellTensionValor);
            tblsignos.AddCell(cellFrecCard);
            tblsignos.AddCell(cellFrecCardValor);

            tblsignos.AddCell(celFreccRes);
            tblsignos.AddCell(cellFreccResp);
            tblsignos.AddCell(cellTemperatura);
            tblsignos.AddCell(cellTemperaturaValor);

            tblsignos.AddCell(cellPeso);
            tblsignos.AddCell(cellPesoValor);
            tblsignos.AddCell(cellTalla);
            tblsignos.AddCell(cellTallaValor);

            tblsignos.AddCell(cellMasa);
            tblsignos.AddCell(cellMasaValor);
            tblsignos.AddCell(cellPAbdominal);
            tblsignos.AddCell(cellPAbdominalValor);

            docHistoria.Add(tblsignos);

            Paragraph interroga = new Paragraph()
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            interroga.Add(new Phrase("Habitus exterior", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.cHabitus, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Cabeza", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_cabeza, fontDato));
            interroga.Add(Chunk.NEWLINE);// interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Cuello", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_cuello, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Torax", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_torax, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Abdomen", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_abdomen, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Genitourinario", fonEiqueta));
            interroga.Add(Chunk.TABBING);
            interroga.Add(new Phrase(datosInter.pa_genito_uri, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Músuculo esquelético", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_muscular, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Neurología", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_neurologia, fontDato));
            interroga.Add(Chunk.NEWLINE); //interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Observación de exploración física", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.cObservaInt, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Observación de electrocardiograma", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.pa_electro, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Optometría", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.cOptometria, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            interroga.Add(new Phrase("Plantoscopia", fonEiqueta));
            interroga.Add(Chunk.NEWLINE);
            interroga.Add(new Phrase(datosInter.cPlantoscopia, fontDato));
            interroga.Add(Chunk.NEWLINE); interroga.Add(Chunk.NEWLINE);

            docHistoria.Add(interroga);
            #endregion

            #region VIII Tatuajes
            if (datosTatuaje.Count > 0)
            {
                PdfPTable tblTatuaje = new PdfPTable(1)
                {
                    TotalWidth = 560f,
                    LockedWidth = true
                };

                tblTatuaje.SetWidths(widthsTitulosGenerales);
                tblTatuaje.HorizontalAlignment = Element.ALIGN_LEFT;
                tblTatuaje.SpacingBefore = 5f;
                tblTatuaje.SpacingAfter = 10f;

                PdfPCell cellTitTat = new PdfPCell(new Phrase("VIII Tatuajes", fonEiqueta))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    UseAscender = true,
                    BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                    BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                    Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
                };
                tblTatuaje.AddCell(cellTitTat);

                docHistoria.Add(tblTatuaje);

                #region Detalle
                PdfPTable tblTatDet = new PdfPTable(4)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] valTatDet = new float[4];
                valTatDet[0] = 30;
                valTatDet[1] = 140;
                valTatDet[2] = 160;
                valTatDet[3] = 203;

                tblTatDet.SetWidths(valTatDet);
                tblTatDet.HorizontalAlignment = 0;
                tblTatDet.SpacingAfter = 5f;
                //tblAndro.SpacingBefore = 5f;
                tblTatDet.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------1a Linea
                PdfPCell cellDetTat_1 = new PdfPCell(new Phrase("No.", fonEiqueta));
                cellDetTat_1.BorderWidth = 0;
                cellDetTat_1.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDetTat_2 = new PdfPCell(new Phrase("Ubicación", fonEiqueta));
                cellDetTat_2.BorderWidth = 0;
                cellDetTat_2.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDetTat_3 = new PdfPCell(new Phrase("Detalle", fonEiqueta));
                cellDetTat_3.BorderWidth = 0;
                cellDetTat_3.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell cellDetTat_4 = new PdfPCell(new Phrase("Tatuaje", fonEiqueta));
                cellDetTat_4.BorderWidth = 0;
                cellDetTat_4.HorizontalAlignment = Element.ALIGN_CENTER;

                tblTatDet.AddCell(cellDetTat_1);
                tblTatDet.AddCell(cellDetTat_2);
                tblTatDet.AddCell(cellDetTat_3);
                tblTatDet.AddCell(cellDetTat_4);

                foreach(var item in datosTatuaje)
                {

                    Byte[] _fotoTat = (Byte[])item.imgTatuaje;
                    iTextSharp.text.Image _la_fotoTat = iTextSharp.text.Image.GetInstance(_fotoTat);
                    _la_fotoTat.ScalePercent(60f);

                    PdfPCell cellDetTat_1x = new PdfPCell(new Phrase(item.idTatuajeevaluado.ToString(), fontDato));
                    cellDetTat_1x.BorderWidth = 0;
                    cellDetTat_1x.HorizontalAlignment = Element.ALIGN_LEFT;

                    PdfPCell cellDetTat_2x = new PdfPCell(new Phrase(item.cUbicacion, fontDato));
                    cellDetTat_2x.BorderWidth = 0;
                    cellDetTat_2x.HorizontalAlignment = Element.ALIGN_LEFT;

                    PdfPCell cellDetTat_3x = new PdfPCell(new Phrase(item.cDescripcion, fontDato));
                    cellDetTat_3x.BorderWidth = 0;
                    cellDetTat_3x.HorizontalAlignment = Element.ALIGN_LEFT;

                    //PdfPCell cellDetTat_4x = new PdfPCell(File(item.imgTatuaje, "image/jpeg"));
                    PdfPCell cellDetTat_4x = new PdfPCell(_la_fotoTat);
                    cellDetTat_4x.BorderWidth = 0;
                    cellDetTat_4x.PaddingTop=5;
                    cellDetTat_4x.HorizontalAlignment = Element.ALIGN_CENTER;

                    tblTatDet.AddCell(cellDetTat_1x);
                    tblTatDet.AddCell(cellDetTat_2x);
                    tblTatDet.AddCell(cellDetTat_3x);
                    tblTatDet.AddCell(cellDetTat_4x);
                }

                docHistoria.Add(tblTatDet);
                #endregion
            }
            #endregion

            #region Firmas
            PdfPTable tblFir = new PdfPTable(3)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valFirl = new float[3];
            valFirl[0] = 205;
            valFirl[1] = 150;
            valFirl[2] = 205;

            tblFir.SetWidths(valFirl);
            tblFir.HorizontalAlignment = 0;
            tblFir.SpacingAfter = 5f;
            tblFir.SpacingBefore = 60f;
            tblFir.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------1a Linea
            PdfPCell cellFir1a = new PdfPCell(new Phrase(datosDtosGenerales.evaluado, fontDato));
            cellFir1a.BorderWidth = 0;
            cellFir1a.HorizontalAlignment = Element.ALIGN_CENTER;
            cellFir1a.BorderWidthTop = 1;

            PdfPCell cellFir1b = new PdfPCell(new Phrase(""));
            cellFir1b.BorderWidth = 0;
            cellFir1b.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFir1c = new PdfPCell(new Phrase(datosDtosGenerales.medico, fontDato));
            cellFir1c.BorderWidth = 0;
            cellFir1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cellFir1c.BorderWidthTop = 1;

            PdfPCell cellFir2a = new PdfPCell(new Phrase("Nombre y firma del evaluado", fontDato));
            cellFir2a.BorderWidth = 0;
            cellFir2a.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellFir2b = new PdfPCell(new Phrase(""));
            cellFir2b.BorderWidth = 0;
            cellFir2b.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellFir2c = new PdfPCell(new Phrase("Ced. Prof: "+datosDtosGenerales.cedMed, fontDato));
            cellFir2c.BorderWidth = 0;
            cellFir2c.HorizontalAlignment = Element.ALIGN_CENTER;

            tblFir.AddCell(cellFir1a);
            tblFir.AddCell(cellFir1b);
            tblFir.AddCell(cellFir1c);

            tblFir.AddCell(cellFir2a);
            tblFir.AddCell(cellFir2b);
            tblFir.AddCell(cellFir2c);

            docHistoria.Add(tblFir);
            #endregion

            //docHistoria.NewPage(); //Para incrementar la paginacion con PageEventHelper

            docHistoria.Close();
            byte[] bytesStream = msHistoria.ToArray();
            msHistoria = new MemoryStream();
            msHistoria.Write(bytesStream, 0, bytesStream.Length);
            msHistoria.Position = 0;

            return new FileStreamResult(msHistoria, "application/pdf");
        }

        public IActionResult PadecimientoOdonMed(int idHistorico)
        {
            var datosDxOdon = repo.Getdosparam1<OdontologiasModel>("sp_medicos_historia_clinica_impresion_diagnosticos", new { @idhistorico = idHistorico, @opcion = 1 }).FirstOrDefault();
            var datosDxMed = repo.Getdosparam1<EnfermedadesModel>("sp_medicos_historia_clinica_impresion_diagnosticos", new { @idhistorico = idHistorico, @opcion = 2 }).ToList();

            MemoryStream msPad = new MemoryStream();
            Document docPad = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwPad = PdfWriter.GetInstance(docPad, msPad);

            string elTitulo = "Diagnósticos";
            pwPad.PageEvent = HeaderFooterHistoria.getMultilineFooterHistoria(elTitulo);

            docPad.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

            #region emision - revision - codigo
            PdfPCell clRev = new PdfPCell(new Phrase("EMISION", fonEiqueta));
            clRev.BorderWidth = 0;
            clRev.VerticalAlignment = Element.ALIGN_BOTTOM;
            clRev.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi = new PdfPCell(new Phrase("REVISION", fonEiqueta));
            celEmi.BorderWidth = 0;
            celEmi.VerticalAlignment = Element.ALIGN_BOTTOM;
            celEmi.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCod = new PdfPCell(new Phrase("CODIGO", fonEiqueta));
            celCod.BorderWidth = 0;
            celCod.VerticalAlignment = Element.ALIGN_BOTTOM;
            celCod.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celEmi_b = new PdfPCell(new Phrase(DateTime.Now.Year.ToString(), fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/INT", fonEiqueta));
            celCod_b.BorderWidth = 0;
            celCod_b.VerticalAlignment = Element.ALIGN_TOP;
            celCod_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celCod_b.BorderWidthBottom = 0.75f;

            PdfPTable tblEmiRevCpd = new PdfPTable(3);
            tblEmiRevCpd.WidthPercentage = 100;
            float[] witdhs = new float[] { 33f, 33f, 33f };
            tblEmiRevCpd.SetWidths(witdhs);

            tblEmiRevCpd.AddCell(clRev);
            tblEmiRevCpd.AddCell(celEmi);
            tblEmiRevCpd.AddCell(celCod);

            tblEmiRevCpd.AddCell(celEmi_b);
            tblEmiRevCpd.AddCell(celRev_b);
            tblEmiRevCpd.AddCell(celCod_b);

            docPad.Add(tblEmiRevCpd);
            #endregion

            #region Titular Padecimientos Odontologicos
            PdfPTable PadOdo = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            PadOdo.SetWidths(widthsTitulosGenerales);
            PadOdo.HorizontalAlignment = Element.ALIGN_LEFT;
            PadOdo.SpacingBefore = 15f;
            PadOdo.SpacingAfter = 10f;

            PdfPCell celPadOdo = new PdfPCell(new Phrase("Impresión diagnóstico odontológica", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            PadOdo.AddCell(celPadOdo);

            docPad.Add(PadOdo);
            #endregion

            #region DxdOdo
            Paragraph dxOdo = new Paragraph(new Phrase(datosDxOdon.diagnostico, fontDato));
            docPad.Add(dxOdo);
            #endregion

            #region Titular Padecimientos Medicos
            PdfPTable PadMed = new PdfPTable(1)
            {
                TotalWidth = 560f,
                LockedWidth = true
            };

            PadMed.SetWidths(widthsTitulosGenerales);
            PadMed.HorizontalAlignment = Element.ALIGN_LEFT;
            PadMed.SpacingBefore = 15f;
            PadMed.SpacingAfter = 15f;

            PdfPCell celPadMed = new PdfPCell(new Phrase("Impresión diagnóstica médica", fonEiqueta))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,    //0 izquiereda  1 centro
                VerticalAlignment = Element.ALIGN_MIDDLE,
                UseAscender = true,
                BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238),
                BorderColor = new iTextSharp.text.BaseColor(0, 0, 0),
                Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER
            };
            PadMed.AddCell(celPadMed);

            docPad.Add(PadMed);
            #endregion

            #region DxMed
            Paragraph DxMedPrin = new Paragraph(new Phrase("Diagnósticos Principales", fonEiqueta));
            docPad.Add(DxMedPrin);

            if (datosDxMed.Count > 0)
            {
                foreach(var item in datosDxMed)
                {
                    if (item.principal == true)
                    {
                        Paragraph DxMedPrinAdd = new Paragraph();
                        DxMedPrinAdd.Add(Chunk.TABBING);
                        DxMedPrinAdd.Add(new Phrase(item.cEnfermedades, fontDato));
                        docPad.Add(DxMedPrinAdd);
                    }
                }
            }

            Paragraph DxMedSec = new Paragraph(new Phrase("Diagnósticos Secundarios", fonEiqueta));
            docPad.Add(Chunk.NEWLINE);
            docPad.Add(DxMedSec);

            if (datosDxMed.Count > 0)
            {
                foreach (var item in datosDxMed)
                {
                    if (item.principal == false)
                    {
                        Paragraph DxMedSecAdd = new Paragraph();
                        DxMedSecAdd.Add(Chunk.TABBING);
                        DxMedSecAdd.Add(new Phrase(item.cEnfermedades, fontDato));
                        docPad.Add(DxMedSecAdd);
                    }
                }
            }

            #endregion

            docPad.Close();
            byte[] bytesStream = msPad.ToArray();
            msPad = new MemoryStream();
            msPad.Write(bytesStream, 0, bytesStream.Length);
            msPad.Position = 0;

            return new FileStreamResult(msPad, "application/pdf");
        }
    }
    public class HeaderFooterRepInt : PdfPageEventHelper
    {
        private string _Folio;
        private string _Realizo;
        private string _CedRea;
        private string _Superviso;
        private string _CedSup;
        private string _Titulo;
        //private string _Evaluado;

        public string folio
        {
            get { return _Folio; }
            set { _Folio = value; }
        }

        public string realizo
        {
            get { return _Realizo; }
            set { _Realizo = value; }
        }

        public string cedrea
        {
            get { return _CedRea; }
            set { _CedRea = value; }
        }

        public string superviso
        {
            get { return _Superviso; }
            set { _Superviso = value; }
        }

        public string cedsup
        {
            get { return _CedSup; }
            set { _CedSup = value; }
        }

        public string titulo
        {
            get { return _Titulo; }
            set { _Titulo = value; }
        }

        //private string evaluado
        //{
        //    get { return _Evaluado; }
        //    set { _Evaluado = value; }
        //}

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Rectangle page = document.PageSize;
            string imageizq = @"C:/inetpub/wwwroot/fotoUser/gobedohor.png";
            iTextSharp.text.Image jpgSupIzq = iTextSharp.text.Image.GetInstance(imageizq);
            jpgSupIzq.ScaleToFit(80f, 80f);

            PdfPCell clLogoSupIzq = new PdfPCell();
            clLogoSupIzq.BorderWidth = 0;
            clLogoSupIzq.VerticalAlignment = Element.ALIGN_BOTTOM;
            clLogoSupIzq.AddElement(jpgSupIzq);

            string imageder = @"C:/inetpub/wwwroot/fotoUser/nuevoCeccc.png";
            iTextSharp.text.Image jpgSupDer = iTextSharp.text.Image.GetInstance(imageder);
            jpgSupDer.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
            jpgSupDer.ScaleToFit(100f, 100f);

            PdfPCell clLogoSupDer = new PdfPCell();
            clLogoSupDer.BorderWidth = 0;
            clLogoSupDer.VerticalAlignment = Element.ALIGN_BOTTOM;
            clLogoSupDer.AddElement(jpgSupDer);

            Chunk chkTit = new Chunk("Dirección Médica y Toxicológica", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add(chkTit);

            Chunk chkSub = new Chunk(_Titulo, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Alignment = Element.ALIGN_CENTER;
            paragraph1.Add(chkSub);

            PdfPCell clTitulo = new PdfPCell();
            clTitulo.BorderWidth = 0;
            clTitulo.AddElement(paragraph);

            PdfPCell clSubTit = new PdfPCell();
            clSubTit.BorderWidth = 0;
            clSubTit.AddElement(paragraph1);

            PdfPTable tblTitulo = new PdfPTable(1);
            tblTitulo.WidthPercentage = 100;
            tblTitulo.AddCell(clTitulo);
            tblTitulo.AddCell(clSubTit);

            PdfPCell clTablaTitulo = new PdfPCell();
            clTablaTitulo.BorderWidth = 0;
            clTablaTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
            clTablaTitulo.AddElement(tblTitulo);

            PdfPTable tblEncabezado = new PdfPTable(3);
            tblEncabezado.WidthPercentage = 100;
            float[] widths = new float[] { 20f, 60f, 20f };
            tblEncabezado.SetWidths(widths);

            tblEncabezado.AddCell(clLogoSupIzq);
            tblEncabezado.AddCell(clTablaTitulo);
            tblEncabezado.AddCell(clLogoSupDer);

            base.OnOpenDocument(writer, document);

            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            tabFot.SpacingAfter = 5F;
            PdfPCell cell;
            //ancho de la tabla
            tabFot.TotalWidth = 560;
            cell = new PdfPCell(tblEncabezado);
            cell.Border = Rectangle.NO_BORDER;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 20, document.Top + tabFot.TotalHeight + 10, writer.DirectContent);
            tabFot.SpacingAfter = 30f;

            var fontFooter = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.BLACK);
            var fontFooterTitulo = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);

            PdfPTable footer = new PdfPTable(3);
            footer.TotalWidth = page.Width - 40;

            PdfPCell cf1 = new PdfPCell(new Phrase(_Realizo, fontFooter));
            cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1.Border = PdfPCell.NO_BORDER;
            cf1.BorderWidthTop = 0.75f;
            footer.AddCell(cf1);

            PdfPCell cf2 = new PdfPCell(new Phrase(_Superviso, fontFooter));
            cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2.Border = PdfPCell.NO_BORDER;
            cf2.BorderWidthTop = 0.75f;
            footer.AddCell(cf2);

            PdfPCell cf3 = new PdfPCell(new Phrase("Ruíz Hernandez Bulmaro", fontFooter));
            cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3.Border = PdfPCell.NO_BORDER;
            cf3.BorderWidthTop = 0.75f;
            footer.AddCell(cf3);

            PdfPCell cf1b = new PdfPCell(new Phrase("Ced. Prof:"+_CedRea, fontFooter));
            cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1b);

            PdfPCell cf2b = new PdfPCell(new Phrase("CED. PROF: " + _CedSup, fontFooter));
            cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2b);

            PdfPCell cf3b = new PdfPCell(new Phrase("CED. PROF: 3392175", fontFooter));
            cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3b);

            PdfPCell cf1c = new PdfPCell(new Phrase("Evaluó", fontFooter));
            cf1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1c);

            PdfPCell cf2c = new PdfPCell(new Phrase("Supervisó", fontFooter));
            cf2c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2c);

            PdfPCell cf3c = new PdfPCell(new Phrase("Validó", fontFooter));
            cf3c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3c);

            PdfPCell leyenda = new PdfPCell(new Phrase("Toda información contenida en este formato está clasificada como Confidencial, de conformidad con lo dispuesto en los artículos 133, 136 fracción I y 139 de la Ley de Transparencias y Acceso a la Información Pública del Estado de Chiapas", fontFooter));
            leyenda.Colspan = 3;
            leyenda.HorizontalAlignment = Element.ALIGN_CENTER;
            leyenda.Border = PdfPCell.NO_BORDER;
            footer.AddCell(leyenda);

            footer.WriteSelectedRows(0, -1, 20, 60, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterRepInt getMultilineFooter(string Folio, string Realizo, string CedRea, string Superviso, string CedSup, string Titulo)
        {
            HeaderFooterRepInt result = new HeaderFooterRepInt();

            result.folio = Folio;
            result.realizo = Realizo;
            result.cedrea = CedRea;
            result.superviso = Superviso;
            result.cedsup = CedSup;
            result.titulo = Titulo;
            //result.evaluado = Evaluado;

            return result;
        }

    }

    public class HeaderFooterHistoria : PdfPageEventHelper
    {
        private string _Titulo;
        public string titulo
        {
            get { return _Titulo; }
            set { _Titulo = value; }
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Rectangle page = document.PageSize;
            string imageizq = @"C:/inetpub/wwwroot/fotoUser/gobedohor.png";
            iTextSharp.text.Image jpgSupIzq = iTextSharp.text.Image.GetInstance(imageizq);
            jpgSupIzq.ScaleToFit(80f, 80f);

            PdfPCell clLogoSupIzq = new PdfPCell();
            clLogoSupIzq.BorderWidth = 0;
            clLogoSupIzq.VerticalAlignment = Element.ALIGN_BOTTOM;
            clLogoSupIzq.AddElement(jpgSupIzq);

            string imageder = @"C:/inetpub/wwwroot/fotoUser/nuevoCeccc.png";
            iTextSharp.text.Image jpgSupDer = iTextSharp.text.Image.GetInstance(imageder);
            jpgSupDer.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
            jpgSupDer.ScaleToFit(100f, 100f);

            PdfPCell clLogoSupDer = new PdfPCell();
            clLogoSupDer.BorderWidth = 0;
            clLogoSupDer.VerticalAlignment = Element.ALIGN_BOTTOM;
            clLogoSupDer.AddElement(jpgSupDer);

            Chunk chkTit = new Chunk("Dirección Médica y Toxicológica", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add(chkTit);

            Chunk chkSub = new Chunk(_Titulo, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Alignment = Element.ALIGN_CENTER;
            paragraph1.Add(chkSub);

            PdfPCell clTitulo = new PdfPCell();
            clTitulo.BorderWidth = 0;
            clTitulo.AddElement(paragraph);

            PdfPCell clSubTit = new PdfPCell();
            clSubTit.BorderWidth = 0;
            clSubTit.AddElement(paragraph1);

            PdfPTable tblTitulo = new PdfPTable(1);
            tblTitulo.WidthPercentage = 100;
            tblTitulo.AddCell(clTitulo);
            tblTitulo.AddCell(clSubTit);

            PdfPCell clTablaTitulo = new PdfPCell();
            clTablaTitulo.BorderWidth = 0;
            clTablaTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
            clTablaTitulo.AddElement(tblTitulo);

            PdfPTable tblEncabezado = new PdfPTable(3);
            tblEncabezado.WidthPercentage = 100;
            float[] widths = new float[] { 20f, 60f, 20f };
            tblEncabezado.SetWidths(widths);

            tblEncabezado.AddCell(clLogoSupIzq);
            tblEncabezado.AddCell(clTablaTitulo);
            tblEncabezado.AddCell(clLogoSupDer);

            base.OnOpenDocument(writer, document);

            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            tabFot.SpacingAfter = 5F;
            PdfPCell cell;
            //ancho de la tabla
            tabFot.TotalWidth = 560;
            cell = new PdfPCell(tblEncabezado);
            cell.Border = Rectangle.NO_BORDER;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 20, document.Top + tabFot.TotalHeight + 10, writer.DirectContent);
            tabFot.SpacingAfter = 30f;

            var fontFooterTitulo = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);

            PdfPTable footer = new PdfPTable(1);
            footer.TotalWidth = page.Width - 40;

            PdfPCell cellFooter = new PdfPCell(new Phrase("Página: " + writer.CurrentPageNumber, fontFooterTitulo));
            cellFooter.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellFooter.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cellFooter);

            footer.WriteSelectedRows(0, -1, 20, 30, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterHistoria getMultilineFooterHistoria(string Titulo)
        {
            HeaderFooterHistoria result = new HeaderFooterHistoria();

            result.titulo = Titulo;

            return result;
        }

    }
}