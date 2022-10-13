using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedToxMVC.Data;
using MedToxMVC.Models.Consultas;
using MedToxMVC.Models.Odontologias;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using MedToxMVC.Helper;

namespace MedToxMVC.Controllers
{
    [Authorize]

    [Authorize(Roles = "Administrador, Odontologia")]
    public class ImpresionOdontologiaController : Controller
    {

        float[] widthsTitulosGenerales = new float[] { 1f };
        private DBOperaciones repo;

        public ImpresionOdontologiaController()
        {
            repo = new DBOperaciones();
        }

        public IActionResult HistoriaOdontologica(int idHistorico)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosOdo = repo.Getdosparam1<OdontologiasModel>("sp_medicos_odontologia_obtener_evaluacion_idhistorico", new { @idhistorico = idHistorico }).FirstOrDefault();
            var cedulasOdonto = repo.Getdosparam1<OdontoCedulas>("sp_medicos_odontologia_obtener_odontologo_supervisor_cedulas", new { @idhistorico = idHistorico }).FirstOrDefault();
            var hayTatuaje = repo.Getdosparam1<OdontoModel>("sp_medicos_odontologia_hay_foto_dental", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRepOdo = new MemoryStream();

            Document docRepOdo = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRepOdo = PdfWriter.GetInstance(docRepOdo, msRepOdo);

            string elFolio = datos.folio.ToString();
            //string elFolio = datos.evaluado;
            string elRealizo = cedulasOdonto.nombreOdontologo;
            string elCedRea = cedulasOdonto.cedOdontologo;
            string elSuperviso = cedulasOdonto.nombreSupervisor;
            string elCedSup = cedulasOdonto.cedSup;
            string elEvaluado = datos.evaluado;

            string elTitulo = "Historia Clínica Odontológica";

            //pwRepOdo.PageEvent = HeaderFooterOdontologia.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo, elEvaluado);
            pwRepOdo.PageEvent = HeaderFooterOdontologia.getMultilineFooter(elTitulo);

            docRepOdo.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
            var fontFirma = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
            //var fontFirma = FontFactory.GetFont("Arial", 10, Font.BOLD + Font.UNDERLINE, BaseColor.BLACK);

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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/03", fonEiqueta));
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

            docRepOdo.Add(tblEmiRevCpd);
            #endregion

            #region Ficha de identificacion
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 5f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Ficha de Identificación", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloTituloFamiliar.UseAscender = true;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRepOdo.Add(Datospersonales);
            #endregion

            #region Tabla Datos Personales
            PdfPTable tblDatosEvaluado = new PdfPTable(4)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] values = new float[4];
            values[0] = 80;
            values[1] = 270;
            values[2] = 100;
            values[3] = 110;
            tblDatosEvaluado.SetWidths(values);
            tblDatosEvaluado.HorizontalAlignment = 0;
            tblDatosEvaluado.SpacingAfter = 5f;
            //tblDatosEvaluado.SpacingBefore = 10f;
            tblDatosEvaluado.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- 1a linea
            PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
            celTitnombre.BorderWidth = 0;
            celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
            celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos.evaluado, fontDato));
            celDatoEvaluado.BorderWidth = 0;
            celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
            celTitCodigo.BorderWidth = 0;
            celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos.codigoevaluado, fontDato));
            celDatoCodigo.BorderWidth = 0;
            celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 2a linea
            PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
            celTitSexo.BorderWidth = 0;
            celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
            celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos.sexo, fontDato));
            celDatoSexo.BorderWidth = 0;
            celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
            celTitEvaluacion.BorderWidth = 0;
            celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
            celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos.evaluacion, fontDato));
            celDatoEvaluacion.BorderWidth = 0;
            celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 3a linea
            PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
            celTitEdad.BorderWidth = 0;
            celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
            celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos.edad.ToString(), fontDato)); ;
            celDatoEdad.BorderWidth = 0;
            celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
            celTitFolio.BorderWidth = 0;
            celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
            celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos.folio, fontDato));
            celDatoFolio.BorderWidth = 0;
            celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 4a linea
            PdfPCell celTitCurp = new PdfPCell(new Phrase("RFC", fonEiqueta));
            celTitCurp.BorderWidth = 0;
            celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp.Substring(0, 10), fontDato)); ;
            celDatoCurp.BorderWidth = 0;
            celDatoCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitFecha = new PdfPCell(new Phrase("Fecha", fonEiqueta));
            celTitFecha.BorderWidth = 0;
            celTitFecha.VerticalAlignment = Element.ALIGN_CENTER;
            celTitFecha.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoFecha = new PdfPCell(new Phrase(DateTime.Now.ToShortDateString(), fontDato));
            celDatoFecha.BorderWidth = 0;
            celDatoFecha.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoFecha.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 5a linea
            PdfPCell celTitDependencia = new PdfPCell(new Phrase("Dependencia", fonEiqueta));
            celTitDependencia.BorderWidth = 0;
            celTitDependencia.VerticalAlignment = Element.ALIGN_CENTER;
            celTitDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos.dependencia, fontDato)) { Colspan = 3 };
            celDatoDependencia.BorderWidth = 0;
            celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 6a linea
            PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
            celTitPuesto.BorderWidth = 0;
            celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
            celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos.puesto, fontDato)) { Colspan = 3 };
            celDatoPuesto.BorderWidth = 0;
            celDatoPuesto.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

            tblDatosEvaluado.AddCell(celTitnombre);
            tblDatosEvaluado.AddCell(celDatoEvaluado);
            tblDatosEvaluado.AddCell(celTitCodigo);
            tblDatosEvaluado.AddCell(celDatoCodigo);

            tblDatosEvaluado.AddCell(celTitSexo);
            tblDatosEvaluado.AddCell(celDatoSexo);
            tblDatosEvaluado.AddCell(celTitEvaluacion);
            tblDatosEvaluado.AddCell(celDatoEvaluacion);

            tblDatosEvaluado.AddCell(celTitEdad);
            tblDatosEvaluado.AddCell(celDatoEdad);
            tblDatosEvaluado.AddCell(celTitFolio);
            tblDatosEvaluado.AddCell(celDatoFolio);

            tblDatosEvaluado.AddCell(celTitCurp);
            tblDatosEvaluado.AddCell(celDatoCurp);
            tblDatosEvaluado.AddCell(celTitFecha);
            tblDatosEvaluado.AddCell(celDatoFecha);

            tblDatosEvaluado.AddCell(celTitDependencia);
            tblDatosEvaluado.AddCell(celDatoDependencia);

            tblDatosEvaluado.AddCell(celTitPuesto);
            tblDatosEvaluado.AddCell(celDatoPuesto);

            docRepOdo.Add(tblDatosEvaluado);

            #endregion

            #region Exploracion
            PdfPTable TblExploracion = new PdfPTable(1);
            TblExploracion.TotalWidth = 560f;
            TblExploracion.LockedWidth = true;

            TblExploracion.SetWidths(widthsTitulosGenerales);
            TblExploracion.HorizontalAlignment = 0;
            TblExploracion.SpacingBefore = 5f;
            TblExploracion.SpacingAfter = 5f;

            PdfPCell cellTituloTituloExploracion = new PdfPCell(new Phrase("Exploración", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloExploracion.HorizontalAlignment = 1;
            cellTituloTituloExploracion.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloTituloExploracion.UseAscender = true;
            cellTituloTituloExploracion.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloExploracion.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloExploracion.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            TblExploracion.AddCell(cellTituloTituloExploracion);

            docRepOdo.Add(TblExploracion);
            #endregion

            #region Datos Exploracion
            Paragraph exploracion = new Paragraph()
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            exploracion.Add(new Phrase("ATM :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_atm, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Labios :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_labios, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Paladar :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_paladar, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Carrillos :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_carrillos, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Istmo de las fauces :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_istmo, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Lengua :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_lengua, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Piso de la boca :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_piso_boca, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Encía :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_encia, fontDato));
            exploracion.Add(Chunk.NEWLINE);
            exploracion.Add(new Phrase("Diente :  ", fonEiqueta));
            exploracion.Add(new Phrase(datosOdo.od_diente, fontDato));
            exploracion.Add(Chunk.NEWLINE);

            docRepOdo.Add(exploracion);
            #endregion

            #region Odontograma
            PdfPTable TblOdontograma = new PdfPTable(1);
            TblOdontograma.TotalWidth = 560f;
            TblOdontograma.LockedWidth = true;

            TblOdontograma.SetWidths(widthsTitulosGenerales);
            TblOdontograma.HorizontalAlignment = 0;
            TblOdontograma.SpacingBefore = 10f;
            TblOdontograma.SpacingAfter = 5f;

            PdfPCell cellTituloTituloOdontograma = new PdfPCell(new Phrase("Odontograma", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloOdontograma.HorizontalAlignment = 1;
            cellTituloTituloOdontograma.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloTituloOdontograma.UseAscender = true;
            cellTituloTituloOdontograma.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloOdontograma.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloOdontograma.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            TblOdontograma.AddCell(cellTituloTituloOdontograma);

            docRepOdo.Add(TblOdontograma);
            #endregion

            #region Odontograma Blanco

            //string imgOdontograma = @"C:/inetpub/wwwroot/fotoUser/odontograma_mvc.jpg";
            string imgOdontograma = @"C:/inetpub/wwwroot/fotoUser/odontograma_mvc2.png";
            iTextSharp.text.Image jpgOdontograma = iTextSharp.text.Image.GetInstance(imgOdontograma);
            jpgOdontograma.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            //jpgOdontograma.ScaleToFit(450f, 450f);
            jpgOdontograma.ScaleToFit(450f, 450f);

            PdfPCell centerOdontograma = new PdfPCell();
            centerOdontograma.BorderWidth = 0;
            centerOdontograma.HorizontalAlignment = Element.ALIGN_CENTER;
            //centerOdontograma.VerticalAlignment = Element.ALIGN_MIDDLE;
            //centerOdontograma.UseAscender = true;
            centerOdontograma.AddElement(jpgOdontograma);

            PdfPTable tblOdontrogramaBlanco = new PdfPTable(1);
            tblOdontrogramaBlanco.WidthPercentage = 100;
            float[] widthsOdontograma = new float[] { 100f };
            tblOdontrogramaBlanco.SetWidths(widthsOdontograma);

            tblOdontrogramaBlanco.AddCell(centerOdontograma);

            docRepOdo.Add(tblOdontrogramaBlanco);

            #endregion

            #region Otros datos odontograma
            PdfPTable tblOtrosDatosOdo = new PdfPTable(4)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesOtros = new float[4];
            valuesOtros[0] = 220;
            valuesOtros[1] = 60;
            valuesOtros[2] = 220;
            valuesOtros[3] = 60;

            tblOtrosDatosOdo.SetWidths(valuesOtros);
            tblOtrosDatosOdo.HorizontalAlignment = 0;
            tblOtrosDatosOdo.SpacingAfter = 5f;
            tblOtrosDatosOdo.SpacingBefore = 5f;

            PdfPCell celAusente = new PdfPCell(new Phrase("O.D. Ausente o perdidos:", fonEiqueta));
            celAusente.BorderWidth = 0;
            celAusente.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAusente.UseAscender = true;
            celAusente.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDatoAusente = new PdfPCell(new Phrase(datosOdo.od_ausentes.ToString(), fontDato));
            cellDatoAusente.BorderWidth = 0;
            cellDatoAusente.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDatoAusente.UseAscender = true;
            cellDatoAusente.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCariado = new PdfPCell(new Phrase("O.D. Cariados:", fonEiqueta));
            celCariado.BorderWidth = 0;
            celCariado.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCariado.UseAscender = true;
            celCariado.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDatoCariado = new PdfPCell(new Phrase(datosOdo.od_perdidos.ToString(), fontDato));
            cellDatoCariado.BorderWidth = 0;
            cellDatoCariado.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDatoCariado.UseAscender = true;
            cellDatoCariado.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celObturado = new PdfPCell(new Phrase("O.D. Obturados:", fonEiqueta));
            celObturado.BorderWidth = 0;
            celObturado.VerticalAlignment = Element.ALIGN_MIDDLE;
            celObturado.UseAscender = true;
            celObturado.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDatoObturado = new PdfPCell(new Phrase(datosOdo.od_obturados.ToString(), fontDato));
            cellDatoObturado.BorderWidth = 0;
            cellDatoObturado.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDatoObturado.UseAscender = true;
            cellDatoObturado.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celReemplazado = new PdfPCell(new Phrase("O.D. Reemplazados:", fonEiqueta));
            celReemplazado.BorderWidth = 0;
            celReemplazado.VerticalAlignment = Element.ALIGN_MIDDLE;
            celReemplazado.UseAscender = true;
            celReemplazado.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDatoReemplazado = new PdfPCell(new Phrase(datosOdo.od_reemplezados.ToString(), fontDato));
            cellDatoReemplazado.BorderWidth = 0;
            cellDatoReemplazado.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDatoReemplazado.UseAscender = true;
            cellDatoReemplazado.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celOclusion = new PdfPCell(new Phrase("Tipo Oclusión:", fonEiqueta));
            celOclusion.BorderWidth = 0;
            celOclusion.VerticalAlignment = Element.ALIGN_MIDDLE;
            celOclusion.UseAscender = true;
            celOclusion.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell cellDatoOclusion = new PdfPCell(new Phrase(datosOdo.od_tipooclusion.ToString(), fontDato)) { Colspan = 3 };
            cellDatoOclusion.BorderWidth = 0;
            cellDatoOclusion.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellDatoOclusion.UseAscender = true;
            cellDatoOclusion.HorizontalAlignment = Element.ALIGN_LEFT;

            tblOtrosDatosOdo.AddCell(celAusente);
            tblOtrosDatosOdo.AddCell(cellDatoAusente);
            tblOtrosDatosOdo.AddCell(celCariado);
            tblOtrosDatosOdo.AddCell(cellDatoCariado);
            tblOtrosDatosOdo.AddCell(celObturado);
            tblOtrosDatosOdo.AddCell(cellDatoObturado);
            tblOtrosDatosOdo.AddCell(celReemplazado);
            tblOtrosDatosOdo.AddCell(cellDatoReemplazado);
            tblOtrosDatosOdo.AddCell(celOclusion);
            tblOtrosDatosOdo.AddCell(cellDatoOclusion);

            docRepOdo.Add(tblOtrosDatosOdo);

            #endregion

            #region Oclusion - Diagnostico - Recomendación - Observacion
            Paragraph final = new Paragraph()
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };

            final.Add(new Phrase("Diagnóstico: ", fonEiqueta));
            final.Add(Chunk.TABBING);
            final.Add(new Phrase(datosOdo.diagnostico, fontDato));
            final.Add(Chunk.NEWLINE);
            final.Add(new Phrase("Recomendación: ", fonEiqueta));
            final.Add(Chunk.TABBING);
            final.Add(new Phrase(datosOdo.recomendacion, fontDato));
            final.Add(Chunk.NEWLINE);
            final.Add(new Phrase("Observaciones: ", fonEiqueta));
            final.Add(Chunk.TABBING);
            final.Add(new Phrase(datosOdo.od_observa, fontDato));

            docRepOdo.Add(final);
            #endregion

            if(hayTatuaje.hayTat != 0)
            {
                Paragraph nuevaPagina = new Paragraph();
                //nuevaPagina.Add(Chunk.NEWPAGE);
                docRepOdo.NewPage();

                #region Titulo Foto de Incrustaciones
                PdfPTable TblTituloIncrustacion = new PdfPTable(1);
                TblTituloIncrustacion.TotalWidth = 560f;
                TblTituloIncrustacion.LockedWidth = true;

                TblTituloIncrustacion.SetWidths(widthsTitulosGenerales);
                TblTituloIncrustacion.HorizontalAlignment = 0;
                TblTituloIncrustacion.SpacingBefore = 10f;
                TblTituloIncrustacion.SpacingAfter = 5f;

                PdfPCell cellTituloTituloIncrustacion = new PdfPCell(new Phrase("Hallazgo Odontológico", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloTituloIncrustacion.HorizontalAlignment = 1;
                cellTituloTituloIncrustacion.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTituloTituloIncrustacion.UseAscender = true;
                cellTituloTituloIncrustacion.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloTituloIncrustacion.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloTituloIncrustacion.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                TblTituloIncrustacion.AddCell(cellTituloTituloIncrustacion);

                docRepOdo.Add(TblTituloIncrustacion);
                #endregion

                #region Imagen
                PdfPTable tblDatosImagen = new PdfPTable(2)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] valuesImagen = new float[2];
                valuesImagen[0] = 280;
                valuesImagen[1] = 280;
                tblDatosImagen.SetWidths(valuesImagen);
                tblDatosImagen.HorizontalAlignment = 0;
                tblDatosImagen.SpacingAfter = 10f;
                tblDatosImagen.DefaultCell.Border = 1;

                PdfPCell cellDescripcionImagen = new PdfPCell(new Phrase(hayTatuaje.descripcion, fontDato));
                cellDescripcionImagen.BorderWidth = 1;
                cellDescripcionImagen.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellDescripcionImagen.UseAscender = true;
                cellDescripcionImagen.HorizontalAlignment = Element.ALIGN_LEFT;

                //----------------------------------------------------------------------------------------- Convertir byte a imagen para usar con itexsharp
                Byte[] bytesIncrustacion = (Byte[])hayTatuaje.imgTatuajeRecuperado;
                iTextSharp.text.Image imgInc = iTextSharp.text.Image.GetInstance(bytesIncrustacion);
                imgInc.ScalePercent(70f);
                //----------------------------------------------------------------------------------------- Convertir

                PdfPCell cellImagen = new PdfPCell(imgInc);
                cellImagen.BorderWidth = 1;
                cellImagen.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellImagen.UseAscender = true;
                cellImagen.HorizontalAlignment = Element.ALIGN_CENTER;

                tblDatosImagen.AddCell(cellDescripcionImagen);
                tblDatosImagen.AddCell(cellImagen);

                docRepOdo.Add(tblDatosImagen);
                #endregion
            }

            #region Firmas
            PdfPTable tblFirmas = new PdfPTable(5)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesFirma = new float[5];
            valuesFirma[0] = 170;
            valuesFirma[1] = 25;
            valuesFirma[2] = 170;
            valuesFirma[3] = 25;
            valuesFirma[4] = 170;
            tblFirmas.SetWidths(valuesFirma);
            tblFirmas.HorizontalAlignment = 0;
            tblFirmas.SpacingBefore = 40f;
            tblFirmas.SpacingAfter = 10f;
            tblFirmas.DefaultCell.Border = 0;

            //------------------------------------------------------- 1a Linea
            PdfPCell cellEval_a = new PdfPCell(new Phrase(datos.evaluado, fontFirma));
            cellEval_a.BorderWidthTop = 1;
            cellEval_a.BorderWidthLeft = 0;
            cellEval_a.BorderWidthRight = 0;
            cellEval_a.BorderWidthBottom = 0;
            cellEval_a.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio1_a = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio1_a.BorderWidth = 0;
            cellVacio1_a.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellRea_a = new PdfPCell(new Phrase(cedulasOdonto.nombreOdontologo, fontFirma));
            cellRea_a.BorderWidthTop = 1;
            cellRea_a.BorderWidthLeft = 0;
            cellRea_a.BorderWidthRight = 0;
            cellRea_a.BorderWidthBottom = 0;
            cellRea_a.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio2_a = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio2_a.BorderWidth = 0;
            cellVacio2_a.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellSup_a = new PdfPCell(new Phrase(cedulasOdonto.nombreSupervisor, fontFirma));
            cellSup_a.BorderWidthTop = 1;
            cellSup_a.BorderWidthLeft = 0;
            cellSup_a.BorderWidthRight = 0;
            cellSup_a.BorderWidthBottom = 0;
            cellSup_a.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------- 2a Linea
            PdfPCell cellEval_b = new PdfPCell(new Phrase("Nombre y firma evaluado", fontFirma));
            cellEval_b.BorderWidth = 0;
            cellEval_b.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio1_b = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio1_b.BorderWidth = 0;
            cellVacio1_b.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellRea_b = new PdfPCell(new Phrase("CED. PROF:" + cedulasOdonto.cedOdontologo, fontFirma));
            cellRea_b.BorderWidth = 0;
            cellRea_b.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio2_b = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio2_b.BorderWidth = 0;
            cellVacio2_b.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellSup_b = new PdfPCell(new Phrase("CED. PROF:" + cedulasOdonto.cedSup, fontFirma));
            cellSup_b.BorderWidth = 0;
            cellSup_b.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------- 3a Linea
            PdfPCell cellEval_c = new PdfPCell(new Phrase(datos.folio, fontFirma));
            cellEval_c.BorderWidth = 0;
            cellEval_c.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio1_c = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio1_c.BorderWidth = 0;
            cellVacio1_c.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellRea_c = new PdfPCell(new Phrase("REALIZO", fontFirma));
            cellRea_c.BorderWidth = 0;
            cellRea_c.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellVacio2_c = new PdfPCell(new Phrase(" ", fontFirma));
            cellVacio2_c.BorderWidth = 0;
            cellVacio2_c.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellSup_c = new PdfPCell(new Phrase("SUPERVISO", fontFirma));
            cellSup_c.BorderWidth = 0;
            cellSup_c.HorizontalAlignment = Element.ALIGN_CENTER;

            tblFirmas.AddCell(cellEval_a);
            tblFirmas.AddCell(cellVacio1_a);
            tblFirmas.AddCell(cellRea_a);
            tblFirmas.AddCell(cellVacio2_a);
            tblFirmas.AddCell(cellSup_a);

            tblFirmas.AddCell(cellEval_b);
            tblFirmas.AddCell(cellVacio1_b);
            tblFirmas.AddCell(cellRea_b);
            tblFirmas.AddCell(cellVacio2_b);
            tblFirmas.AddCell(cellSup_b);

            tblFirmas.AddCell(cellEval_c);
            tblFirmas.AddCell(cellVacio1_c);
            tblFirmas.AddCell(cellRea_c);
            tblFirmas.AddCell(cellVacio2_c);
            tblFirmas.AddCell(cellSup_c);

            docRepOdo.Add(tblFirmas);
            #endregion

            docRepOdo.Close();
            byte[] bytesStream = msRepOdo.ToArray();
            msRepOdo = new MemoryStream();
            msRepOdo.Write(bytesStream, 0, bytesStream.Length);
            msRepOdo.Position = 0;

            return new FileStreamResult(msRepOdo, "application/pdf");
        }

        public IActionResult IndexListaAsociacion()
        {
            return View();
        }

        public IActionResult ListaAsociaOdontologo(string fecha)
        {
            var listaAsociacion = repo.Getdosparam1<AsociaOdon>("sp_medicos_odontologia_obtener_lista_asociacion", new { @fecha = fecha, @usuario = "-" }).ToList();

            MemoryStream msAsoOdo = new MemoryStream();

            Document docAsoOdo = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwAsoOdo = PdfWriter.GetInstance(docAsoOdo, msAsoOdo);

            //string elFolio = "";
            //string elRealizo = "";
            //string elCedRea = "";
            //string elSuperviso = "";
            //string elCedSup = "";
            //string elEvaluado = "";

            string elTitulo = "Relación de Odontólogos y evaluado";

            pwAsoOdo.PageEvent = HeaderFooterOdontologia.getMultilineFooter(elTitulo);
            //pwAsoOdo.PageEvent = HeaderFooterOdontologia.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo, elEvaluado);

            docAsoOdo.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

            #region Fecha Impresión
            Paragraph lafecha = new Paragraph()
            {
                Alignment = Element.ALIGN_RIGHT
            };            

            lafecha.Add(new Phrase("Fecha Impresión: ", fonEiqueta));
            lafecha.Add(Chunk.TABBING);
            lafecha.Add(new Phrase(DateTime.Now.ToShortDateString(), fontDato));
            lafecha.Add(Chunk.NEWLINE); lafecha.Add(Chunk.NEWLINE);

            docAsoOdo.Add(lafecha);
            #endregion

            #region Lista
            foreach (var Odontoloco in listaAsociacion)
            {
                Paragraph nombreOdo = new Paragraph()
                {
                    Alignment = Element.ALIGN_LEFT
                };
                nombreOdo.Add(Chunk.NEWLINE);
                nombreOdo.Add(Chunk.NEWLINE);
                nombreOdo.Add(new Phrase("Odontólogo: ", fonEiqueta));
                nombreOdo.Add(Chunk.TABBING);
                nombreOdo.Add(new Phrase(Odontoloco.nomOdo, fonEiqueta));
                nombreOdo.Add(Chunk.NEWLINE);

                docAsoOdo.Add(nombreOdo);

                var sublista = repo.Getdosparam1<AsociaOdon>("sp_medicos_odontologia_obtener_lista_asociacion", new { @fecha = fecha, @usuario = Odontoloco.usuario }).ToList();
                foreach(var subListilla in sublista)
                {
                    Paragraph datosSubLista = new Paragraph()
                    {
                        Alignment = Element.ALIGN_LEFT
                    };

                    datosSubLista.Add(new Phrase(subListilla.grupo, fontDato));
                    datosSubLista.Add(Chunk.TABBING);
                    datosSubLista.Add(new Phrase(subListilla.codigoevaluado, fontDato));
                    datosSubLista.Add(Chunk.TABBING); datosSubLista.Add(Chunk.TABBING);
                    datosSubLista.Add(new Phrase(subListilla.evaluado, fontDato));
                    datosSubLista.Add(Chunk.TABBING); datosSubLista.Add(Chunk.TABBING);
                    datosSubLista.Add(new Phrase(subListilla.folio, fontDato));
                    datosSubLista.Add(Chunk.TABBING);
                    datosSubLista.Add(Chunk.TABBING);
                    datosSubLista.Add(new Phrase(subListilla.gaf, fontDato));

                    docAsoOdo.Add(datosSubLista);
                }

            }
            #endregion

            docAsoOdo.Close();
            byte[] bytesStream = msAsoOdo.ToArray();
            msAsoOdo = new MemoryStream();
            msAsoOdo.Write(bytesStream, 0, bytesStream.Length);
            msAsoOdo.Position = 0;

            return new FileStreamResult(msAsoOdo, "application/pdf");
        }

        public IActionResult ListaEntregaDiaria()
        {
            var listaEntrega = repo.Getdosparam1<OdontoModel>("sp_medicos_odontologia_obtener_lista_entrega_diaria", new { @usuario = SessionHelper.GetName(User) }).ToList();

            MemoryStream msEntrega = new MemoryStream();

            Document docEntrega = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwEntrega = PdfWriter.GetInstance(docEntrega, msEntrega);

            string elTitulo = "Registro y Entrega de Historias Odontológicas";

            pwEntrega.PageEvent = HeaderFooterOdontologia.getMultilineFooter(elTitulo);

            docEntrega.Open();

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

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.0", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/13-14", fonEiqueta));
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

            docEntrega.Add(tblEmiRevCpd);
            #endregion

            #region Fecha Impresión
            Paragraph lafecha = new Paragraph()
            {
                Alignment = Element.ALIGN_RIGHT
            };

            lafecha.Add(new Phrase("Fecha: ", fonEiqueta));
            lafecha.Add(Chunk.TABBING);
            lafecha.Add(new Phrase(DateTime.Now.ToShortDateString(), fontDato));
            lafecha.Add(Chunk.NEWLINE); lafecha.Add(Chunk.NEWLINE);

            docEntrega.Add(lafecha);
            #endregion

            #region Nombre odontologo
            Paragraph odontologo = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            odontologo.Add(new Phrase("Nombre del Odontologa(o): " + listaEntrega[0].odontologa, fonEiqueta));
            odontologo.Add(Chunk.NEWLINE);
            docEntrega.Add(odontologo);
            #endregion

            #region Lista Entrega
            PdfPTable tblEntrega = new PdfPTable(5)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valEntrega = new float[5];
            valEntrega[0] = 165;
            valEntrega[1] = 50;
            valEntrega[2] = 80;
            valEntrega[3] = 165;
            valEntrega[4] = 100;
            tblEntrega.SetWidths(valEntrega);
            tblEntrega.HorizontalAlignment = 0;
            tblEntrega.SpacingAfter = 30f;
            tblEntrega.SpacingBefore = 10f;
            tblEntrega.DefaultCell.Border = 0;

            PdfPCell celNombreEvaluado = new PdfPCell(new Phrase("Nombre Evaluado", fonEiqueta));
            celNombreEvaluado.BorderWidth = 0;
            celNombreEvaluado.BorderWidthBottom = 1;
            celNombreEvaluado.VerticalAlignment = Element.ALIGN_MIDDLE;
            celNombreEvaluado.UseAscender = true;
            celNombreEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;
            tblEntrega.AddCell(celNombreEvaluado);

            PdfPCell celGafete = new PdfPCell(new Phrase("Gaf", fonEiqueta));
            celGafete.BorderWidth = 0;
            celGafete.BorderWidthBottom = 1;
            celGafete.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGafete.UseAscender = true;
            celGafete.HorizontalAlignment = Element.ALIGN_LEFT;
            tblEntrega.AddCell(celGafete);

            PdfPCell celFecha = new PdfPCell(new Phrase("Fecha", fonEiqueta));
            celFecha.BorderWidth = 0;
            celFecha.BorderWidthBottom = 1;
            celFecha.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFecha.UseAscender = true;
            celFecha.HorizontalAlignment = Element.ALIGN_LEFT;
            tblEntrega.AddCell(celFecha);

            PdfPCell celMedico = new PdfPCell(new Phrase("Medico", fonEiqueta));
            celMedico.BorderWidth = 0;
            celMedico.BorderWidthBottom = 1;
            celMedico.VerticalAlignment = Element.ALIGN_MIDDLE;
            celMedico.UseAscender = true;
            celMedico.HorizontalAlignment = Element.ALIGN_LEFT;
            tblEntrega.AddCell(celMedico);

            PdfPCell celEvaluacion = new PdfPCell(new Phrase("Firma Recibido", fonEiqueta));
            celEvaluacion.BorderWidth = 0;
            celEvaluacion.BorderWidthBottom = 1;
            celEvaluacion.VerticalAlignment = Element.ALIGN_MIDDLE;
            celEvaluacion.UseAscender = true;
            celEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;
            tblEntrega.AddCell(celEvaluacion);

            foreach (var item in listaEntrega)
            {
                PdfPCell celNombreEvaluado_a = new PdfPCell(new Phrase(item.evaluado, fontDato));
                celNombreEvaluado_a.BorderWidth = 0;
                celNombreEvaluado_a.FixedHeight = 40f;
                celNombreEvaluado_a.VerticalAlignment = Element.ALIGN_MIDDLE;
                celNombreEvaluado_a.UseAscender = true;
                celNombreEvaluado_a.HorizontalAlignment = Element.ALIGN_LEFT;
                tblEntrega.AddCell(celNombreEvaluado_a);

                PdfPCell celGafete_a = new PdfPCell(new Phrase(item.folio, fontDato));
                celGafete_a.BorderWidth = 0;
                celGafete_a.VerticalAlignment = Element.ALIGN_MIDDLE;
                celGafete_a.UseAscender = true;
                celGafete_a.HorizontalAlignment = Element.ALIGN_LEFT;
                tblEntrega.AddCell(celGafete_a);

                PdfPCell celFecha_a = new PdfPCell(new Phrase(item.fechaAlta, fontDato));
                celFecha_a.BorderWidth = 0;
                celFecha_a.VerticalAlignment = Element.ALIGN_MIDDLE;
                celFecha_a.UseAscender = true;
                celFecha_a.HorizontalAlignment = Element.ALIGN_LEFT;
                tblEntrega.AddCell(celFecha_a);

                PdfPCell celMedico_a = new PdfPCell(new Phrase(item.medico, fontDato));
                celMedico_a.BorderWidth = 0;
                celMedico_a.VerticalAlignment = Element.ALIGN_MIDDLE;
                celMedico_a.UseAscender = true;
                celMedico_a.HorizontalAlignment = Element.ALIGN_LEFT;
                tblEntrega.AddCell(celMedico_a);

                PdfPCell celEvaluacion_a = new PdfPCell(new Phrase("______________", fontDato));
                celEvaluacion_a.BorderWidth = 0;
                celEvaluacion_a.VerticalAlignment = Element.ALIGN_MIDDLE;
                celEvaluacion_a.UseAscender = true;
                celEvaluacion_a.HorizontalAlignment = Element.ALIGN_LEFT;
                tblEntrega.AddCell(celEvaluacion_a);
            }

            docEntrega.Add(tblEntrega);
            #endregion

            #region Firma Odontologo
            Paragraph firmaOdontologo = new Paragraph()
            {
                Alignment = Element.ALIGN_CENTER
            };
            firmaOdontologo.Add(new Phrase("___________________________", fontDato));
            firmaOdontologo.Add(Chunk.NEWLINE);
            firmaOdontologo.Add(new Phrase("Firma Odontologa(o)", fontDato));
            docEntrega.Add(firmaOdontologo);
            #endregion

            docEntrega.Close();
            byte[] bytesStream = msEntrega.ToArray();
            msEntrega = new MemoryStream();
            msEntrega.Write(bytesStream, 0, bytesStream.Length);
            msEntrega.Position = 0;

            return new FileStreamResult(msEntrega, "application/pdf");

        }
    }

    public class HeaderFooterOdontologia : PdfPageEventHelper
    {
        private string _Folio;
        private string _Realizo;
        private string _CedRea;
        private string _Superviso;
        private string _CedSup;
        private string _Titulo;
        private string _Evaluado;

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

        private string evaluado
        {
            get { return _Evaluado; }
            set { _Evaluado = value; }
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

            var fontFooter = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.BLACK);
            var fontFooterTitulo = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);

            //PdfPTable footer = new PdfPTable(3);
            //footer.TotalWidth = page.Width - 40;

            //PdfPCell cf1 = new PdfPCell(new Phrase(_Evaluado, fontFooter));
            //cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf1.Border = PdfPCell.NO_BORDER;
            //cf1.BorderWidthTop = 0.75f;
            //footer.AddCell(cf1);

            //PdfPCell cf2 = new PdfPCell(new Phrase(_Realizo, fontFooter));
            //cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf2.Border = PdfPCell.NO_BORDER;
            //cf2.BorderWidthTop = 0.75f;
            //footer.AddCell(cf2);

            //PdfPCell cf3 = new PdfPCell(new Phrase(_Superviso, fontFooter));
            //cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf3.Border = PdfPCell.NO_BORDER;
            //cf3.BorderWidthTop = 0.75f;
            //footer.AddCell(cf3);

            //PdfPCell cf1b = new PdfPCell(new Phrase("Nombre y firma evaluado", fontFooter));
            //cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf1b.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf1b);

            //PdfPCell cf2b = new PdfPCell(new Phrase("CED. PROF: " + _CedRea, fontFooter));
            //cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf2b.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf2b);

            //PdfPCell cf3b = new PdfPCell(new Phrase("CED. PROF: " + _CedSup, fontFooter));
            //cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf3b.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf3b);

            //PdfPCell cf1c = new PdfPCell(new Phrase(_Folio, fontFooter));
            //cf1c.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf1c.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf1c);

            //PdfPCell cf2c = new PdfPCell(new Phrase("REALIZO", fontFooter));
            //cf2c.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf2c.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf2c);

            //PdfPCell cf3c = new PdfPCell(new Phrase("SUPERVISO", fontFooter));
            //cf3c.HorizontalAlignment = Element.ALIGN_CENTER;
            //cf3c.Border = PdfPCell.NO_BORDER;
            //footer.AddCell(cf3c);

            //footer.WriteSelectedRows(0, -1, 20, 60, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterOdontologia getMultilineFooter(string Titulo)
        {
            HeaderFooterOdontologia result = new HeaderFooterOdontologia();

            result.titulo = Titulo;

            return result;
        }

        //public static HeaderFooterOdontologia getMultilineFooter(string Folio, string Realizo, string CedRea, string Superviso, string CedSup, string Titulo, string Evaluado)
        //{
        //    HeaderFooterOdontologia result = new HeaderFooterOdontologia();

        //    result.folio = Folio;
        //    result.realizo = Realizo;
        //    result.cedrea = CedRea;
        //    result.superviso = Superviso;
        //    result.cedsup = CedSup;
        //    result.titulo = Titulo;
        //    result.evaluado = Evaluado;

        //    return result;
        //}

    }

}
