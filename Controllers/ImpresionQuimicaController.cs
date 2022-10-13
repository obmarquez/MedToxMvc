using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedToxMVC.Data;
using MedToxMVC.Models.Consultas;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace MedToxMVC.Controllers
{
    [Authorize]

    [Authorize(Roles = "Administrador, Quimica, Nutricion")]
    public class ImpresionQuimicaController : Controller
    {
        float[] widthsTitulosGenerales = new float[] { 1f };
        private DBOperaciones repo;

        public ImpresionQuimicaController()
        {
            repo = new DBOperaciones();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult reporteEgo(int idHistorico)
        {
            //var datosC3 = repo.Get<ConsultasModel>("sp_general_obtener_certificacion_acreditacion").FirstOrDefault();
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosEGO = repo.Getdosparam1<EgoModel>("sp_medicos_rep_orina", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datosEGO.FOLIO.ToString();
            string elRealizo = datosEGO.realizo.ToString();
            string elCedRea = datosEGO.ced_prof_realizo.ToString();
            string elSuperviso = datosEGO.superviso.ToString();
            string elCedSup = datosEGO.ced_prof_superviso.ToString();
            string elTitulo = "Examen General de Orina";

            pwRep.PageEvent = HeaderFooterEGO.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

            docRep.Open();

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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/07", fonEiqueta));
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

            docRep.Add(tblEmiRevCpd);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);

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
            tblDatosEvaluado.SpacingAfter = 20f;
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
            PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
            celTitCurp.BorderWidth = 0;
            celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp, fontDato)); ;
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

            docRep.Add(tblDatosEvaluado);

            #endregion

            #region Datos Examen Fisico
            PdfPTable tblEgo = new PdfPTable(3)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valuesEgo = new float[3];
            valuesEgo[0] = 200;
            valuesEgo[1] = 200;
            valuesEgo[2] = 160;
            tblEgo.SetWidths(valuesEgo);
            tblEgo.HorizontalAlignment = 0;
            tblEgo.SpacingAfter = 20f;
            tblEgo.DefaultCell.Border = 0;

            //------------------------------------------------------------------------ Linea 1
            PdfPCell celTituloEgo = new PdfPCell(new Phrase("Examen físico", fonEiqueta)) { Colspan = 2 };
            celTituloEgo.BorderWidth = 0;
            celTituloEgo.BorderWidthBottom = 1;
            celTituloEgo.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloEgo.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTituloReferencia = new PdfPCell(new Phrase("Referencia", fonEiqueta));
            celTituloReferencia.BorderWidth = 0;
            celTituloReferencia.BorderWidthBottom = 1;
            celTituloReferencia.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 2 (Aspecto)
            PdfPCell celTitAspecto = new PdfPCell(new Phrase("Aspecto", fonEiqueta));
            celTitAspecto.BorderWidth = 0;
            celTitAspecto.VerticalAlignment = Element.ALIGN_CENTER;
            celTitAspecto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoAspecto = new PdfPCell(new Phrase(datosEGO.Aspecto, fontDato));
            celDatoAspecto.BorderWidth = 0;
            celTitAspecto.VerticalAlignment = Element.ALIGN_CENTER;
            celTitAspecto.HorizontalAlignment = Element.ALIGN_LEFT;
            
            PdfPCell celDatoAspectoVacio = new PdfPCell(new Phrase("", fontDato));
            celDatoAspectoVacio.BorderWidth = 0;
            celDatoAspectoVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoAspectoVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            //------------------------------------------------------------------------ Linea 3 (Color)
            PdfPCell celTitColor = new PdfPCell(new Phrase("Color", fonEiqueta));
            celTitColor.BorderWidth = 0;
            celTitColor.VerticalAlignment = Element.ALIGN_CENTER;
            celTitColor.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoColor = new PdfPCell(new Phrase(datosEGO.Color, fontDato));
            celDatoColor.BorderWidth = 0;
            celDatoColor.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoColor.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoColorVacio = new PdfPCell(new Phrase("", fontDato));
            celDatoColorVacio.BorderWidth = 0;
            celDatoColorVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoColorVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            //------------------------------------------------------------------------ Linea 4 (PH)
            PdfPCell celTitPH = new PdfPCell(new Phrase("PH", fonEiqueta));
            celTitPH.BorderWidth = 0;
            celTitPH.VerticalAlignment = Element.ALIGN_CENTER;
            celTitPH.HorizontalAlignment = Element.ALIGN_LEFT;

            //PdfPCell celDatoPh = new PdfPCell(new Phrase(datosEGO.PH.ToString("F2"), fontDato));

            decimal Double1 = Convert.ToDecimal(datosEGO.PH);
            PdfPCell celDatoPh = new PdfPCell(new Phrase(Double1.ToString("F2"), fontDato));
            celDatoPh.BorderWidth = 0;
            celDatoPh.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPh.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoPHVacio = new PdfPCell(new Phrase("", fontDato));
            celDatoPHVacio.BorderWidth = 0;
            celDatoPHVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPHVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            //------------------------------------------------------------------------ Linea 5 (Densidad)
            PdfPCell celTitDensidad = new PdfPCell(new Phrase("Densidad", fonEiqueta));
            celTitDensidad.BorderWidth = 0;
            celTitDensidad.VerticalAlignment = Element.ALIGN_CENTER;
            celTitDensidad.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoDensidad = new PdfPCell(new Phrase(datosEGO.Densidad, fontDato));
            celDatoDensidad.BorderWidth = 0;
            celDatoDensidad.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoDensidad.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoDensidadVacio = new PdfPCell(new Phrase("", fontDato));
            celDatoDensidadVacio.BorderWidth = 0;
            celDatoDensidadVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoDensidadVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            tblEgo.AddCell(celTituloEgo);
            tblEgo.AddCell(celTituloReferencia);

            tblEgo.AddCell(celTitAspecto);
            tblEgo.AddCell(celDatoAspecto);
            tblEgo.AddCell(celDatoAspectoVacio);

            tblEgo.AddCell(celTitColor);
            tblEgo.AddCell(celDatoColor);
            tblEgo.AddCell(celDatoColorVacio);

            tblEgo.AddCell(celTitPH);
            tblEgo.AddCell(celDatoPh);
            tblEgo.AddCell(celDatoPHVacio);

            tblEgo.AddCell(celTitDensidad);
            tblEgo.AddCell(celDatoDensidad);
            tblEgo.AddCell(celDatoDensidadVacio);

            docRep.Add(tblEgo);
            #endregion

            #region Examen químico
            PdfPTable tblQuimico = new PdfPTable(3)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valueQuimico = new float[3];
            valueQuimico[0]=200;
            valueQuimico[1] = 200;
            valueQuimico[2] = 160;
            tblQuimico.SetWidths(valueQuimico);
            tblQuimico.SpacingAfter = 20f;
            tblQuimico.DefaultCell.Border = 0;

            //------------------------------------------------------------------------ Linea 1
            PdfPCell celTituloQui = new PdfPCell(new Phrase("Examen químico", fonEiqueta)) { Colspan = 2 };
            celTituloQui.BorderWidth = 0;
            celTituloQui.BorderWidthBottom = 1;
            celTituloQui.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloQui.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTituloQuimicaReferencia = new PdfPCell(new Phrase("", fonEiqueta));
            celTituloQuimicaReferencia.BorderWidth = 0;
            celTituloQuimicaReferencia.BorderWidthBottom = 1;
            celTituloQuimicaReferencia.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloQuimicaReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 2 (Glucosa)
            PdfPCell celTitGlucosa = new PdfPCell(new Phrase("Glucosa", fonEiqueta));
            celTitGlucosa.BorderWidth = 0;
            celTitGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
            celTitGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoGlucosa = new PdfPCell(new Phrase(datosEGO.Glucosa, fontDato));
            celDatoGlucosa.BorderWidth = 0;
            celDatoGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaGlucosa = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaGlucosa.BorderWidth = 0;
            celReferenciaGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 3 (Bilirrubina)
            PdfPCell celBilirrubina = new PdfPCell(new Phrase("Bilirrubina", fonEiqueta));
            celBilirrubina.BorderWidth = 0;
            celBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
            celBilirrubina.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoBilirrubina = new PdfPCell(new Phrase(datosEGO.Bilirrubina, fontDato));
            celDatoBilirrubina.BorderWidth = 0;
            celDatoBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoBilirrubina.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaBilirrubina = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaBilirrubina.BorderWidth = 0;
            celReferenciaBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaBilirrubina.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 4 (Cetona)
            PdfPCell celCetona = new PdfPCell(new Phrase("Cetona", fonEiqueta));
            celCetona.BorderWidth = 0;
            celCetona.VerticalAlignment = Element.ALIGN_CENTER;
            celCetona.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCetona = new PdfPCell(new Phrase(datosEGO.Cetona, fontDato));
            celDatoCetona.BorderWidth = 0;
            celDatoCetona.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCetona.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaCetona = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaCetona.BorderWidth = 0;
            celReferenciaCetona.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaCetona.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 5 (Proteína)
            PdfPCell celProteina = new PdfPCell(new Phrase("Proteína", fonEiqueta));
            celProteina.BorderWidth = 0;
            celProteina.VerticalAlignment = Element.ALIGN_CENTER;
            celProteina.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoProteina = new PdfPCell(new Phrase(datosEGO.Proteinas, fontDato));
            celDatoProteina.BorderWidth = 0;
            celDatoProteina.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoProteina.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaProteina = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaProteina.BorderWidth = 0;
            celReferenciaProteina.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaProteina.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 6 (Uribilinogeno)
            PdfPCell celUrobilinogeno = new PdfPCell(new Phrase("Urobilinógeno", fonEiqueta));
            celUrobilinogeno.BorderWidth = 0;
            celUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
            celUrobilinogeno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoUrobilinogeno = new PdfPCell(new Phrase(datosEGO.Urobilinogeno, fontDato));
            celDatoUrobilinogeno.BorderWidth = 0;
            celDatoUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoUrobilinogeno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaUrobilinogeno = new PdfPCell(new Phrase("0.2 MG/DL", fontDato));
            celReferenciaUrobilinogeno.BorderWidth = 0;
            celReferenciaUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaUrobilinogeno.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 6 (Sangre)
            PdfPCell celSangre = new PdfPCell(new Phrase("Sangre", fonEiqueta));
            celSangre.BorderWidth = 0;
            celSangre.VerticalAlignment = Element.ALIGN_CENTER;
            celSangre.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoSangre = new PdfPCell(new Phrase(datosEGO.Sangre, fontDato));
            celDatoSangre.BorderWidth = 0;
            celDatoSangre.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoSangre.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaSangre = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaSangre.BorderWidth = 0;
            celReferenciaSangre.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaSangre.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 7 (Nitritos)
            PdfPCell celNitritos = new PdfPCell(new Phrase("Nitritos", fonEiqueta));
            celNitritos.BorderWidth = 0;
            celNitritos.VerticalAlignment = Element.ALIGN_CENTER;
            celNitritos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoNitrito = new PdfPCell(new Phrase(datosEGO.Nitritos, fontDato));
            celDatoNitrito.BorderWidth = 0;
            celDatoNitrito.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoNitrito.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaNitrito = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaNitrito.BorderWidth = 0;
            celReferenciaNitrito.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaNitrito.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 8 (Acido ascorbico)
            PdfPCell celAscorbico = new PdfPCell(new Phrase("Ácido ascórbico", fonEiqueta));
            celAscorbico.BorderWidth = 0;
            celAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
            celAscorbico.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoAscorbico = new PdfPCell(new Phrase(datosEGO.AcidoAscorbico, fontDato));
            celDatoAscorbico.BorderWidth = 0;
            celDatoAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoAscorbico.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaAscorbico = new PdfPCell(new Phrase("NEGATIVO", fontDato));
            celReferenciaAscorbico.BorderWidth = 0;
            celReferenciaAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaAscorbico.HorizontalAlignment = Element.ALIGN_CENTER;

            tblQuimico.AddCell(celTituloQui);
            tblQuimico.AddCell(celTituloQuimicaReferencia);

            tblQuimico.AddCell(celTitGlucosa);
            tblQuimico.AddCell(celDatoGlucosa);
            tblQuimico.AddCell(celReferenciaGlucosa);

            tblQuimico.AddCell(celCetona);
            tblQuimico.AddCell(celDatoCetona);
            tblQuimico.AddCell(celReferenciaCetona);

            tblQuimico.AddCell(celBilirrubina);
            tblQuimico.AddCell(celDatoBilirrubina);
            tblQuimico.AddCell(celReferenciaBilirrubina);

            tblQuimico.AddCell(celProteina);
            tblQuimico.AddCell(celDatoProteina);
            tblQuimico.AddCell(celReferenciaProteina);

            tblQuimico.AddCell(celUrobilinogeno);
            tblQuimico.AddCell(celDatoUrobilinogeno);
            tblQuimico.AddCell(celReferenciaUrobilinogeno);

            tblQuimico.AddCell(celSangre);
            tblQuimico.AddCell(celDatoSangre);
            tblQuimico.AddCell(celReferenciaSangre);

            tblQuimico.AddCell(celNitritos);
            tblQuimico.AddCell(celDatoNitrito);
            tblQuimico.AddCell(celReferenciaNitrito);

            tblQuimico.AddCell(celAscorbico);
            tblQuimico.AddCell(celDatoAscorbico);
            tblQuimico.AddCell(celReferenciaAscorbico);

            docRep.Add(tblQuimico);

            #endregion

            #region Examen Microscopico
            PdfPTable tblMicro = new PdfPTable(3)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valueMicro = new float[3];
            valueMicro[0] = 200;
            valueMicro[1] = 200;
            valueMicro[2] = 160;
            tblMicro.SetWidths(valueMicro);
            tblMicro.SpacingAfter = 40f;
            tblMicro.DefaultCell.Border = 0;

            //------------------------------------------------------------------------ Linea 1
            PdfPCell celTituloMic = new PdfPCell(new Phrase("Examen Microscópico", fonEiqueta)) { Colspan = 2 };
            celTituloMic.BorderWidth = 0;
            celTituloMic.BorderWidthBottom = 1;
            celTituloMic.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloMic.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTituloMicReferencia= new PdfPCell(new Phrase("", fonEiqueta));
            celTituloMicReferencia.BorderWidth = 0;
            celTituloMicReferencia.BorderWidthBottom = 1;
            celTituloMicReferencia.VerticalAlignment = Element.ALIGN_CENTER;
            celTituloMicReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 2 (Leucocitos)
            PdfPCell celTitLeucocitos = new PdfPCell(new Phrase("Leucocitos", fonEiqueta));
            celTitLeucocitos.BorderWidth = 0;
            celTitLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celTitLeucocitos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoLeucocitos = new PdfPCell(new Phrase(datosEGO.Leucocitos, fontDato));
            celDatoLeucocitos.BorderWidth = 0;
            celDatoLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoLeucocitos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaLeucocitos = new PdfPCell(new Phrase("X CPO", fontDato));
            celReferenciaLeucocitos.BorderWidth = 0;
            celReferenciaLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaLeucocitos.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 3 (Eritrocitos)
            PdfPCell celTitEritrocitos = new PdfPCell(new Phrase("Eritrocitos", fonEiqueta));
            celTitEritrocitos.BorderWidth = 0;
            celTitEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celTitEritrocitos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoEritrocitos = new PdfPCell(new Phrase(datosEGO.Eritrocitos, fontDato));
            celDatoEritrocitos.BorderWidth = 0;
            celDatoEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoEritrocitos.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaEritrocitos = new PdfPCell(new Phrase("X CPO", fontDato));
            celReferenciaEritrocitos.BorderWidth = 0;
            celReferenciaEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaEritrocitos.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 4 (Bacterias)
            PdfPCell celTitBacterias = new PdfPCell(new Phrase("Bacterias", fonEiqueta));
            celTitBacterias.BorderWidth = 0;
            celTitBacterias.VerticalAlignment = Element.ALIGN_CENTER;
            celTitBacterias.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoBacterias = new PdfPCell(new Phrase(datosEGO.Bacterias, fontDato));
            celDatoBacterias.BorderWidth = 0;
            celDatoBacterias.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoBacterias.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaBacterias = new PdfPCell(new Phrase("", fontDato));
            celReferenciaBacterias.BorderWidth = 0;
            celReferenciaBacterias.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaBacterias.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 5 (Células)
            PdfPCell celTitCelulas = new PdfPCell(new Phrase("Células", fonEiqueta));
            celTitCelulas.BorderWidth = 0;
            celTitCelulas.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCelulas.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCelulas = new PdfPCell(new Phrase(datosEGO.Celulas, fontDato));
            celDatoCelulas.BorderWidth = 0;
            celDatoCelulas.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCelulas.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaCelulas = new PdfPCell(new Phrase("", fontDato));
            celReferenciaCelulas.BorderWidth = 0;
            celReferenciaCelulas.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaCelulas.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 6 (Cilindros)
            PdfPCell celTitCilindros = new PdfPCell(new Phrase("Cilindros de", fonEiqueta));
            celTitCilindros.BorderWidth = 0;
            celTitCilindros.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCilindros.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCilindros = new PdfPCell(new Phrase(datosEGO.Cilindros, fontDato));
            celDatoCilindros.BorderWidth = 0;
            celDatoCilindros.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCilindros.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaCilindros = new PdfPCell(new Phrase("X CPO", fontDato));
            celReferenciaCilindros.BorderWidth = 0;
            celReferenciaCilindros.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaCilindros.HorizontalAlignment = Element.ALIGN_CENTER;

            //------------------------------------------------------------------------ Linea 7 (Cristales)
            PdfPCell celTitCristlaes = new PdfPCell(new Phrase("Cristales de", fonEiqueta));
            celTitCristlaes.BorderWidth = 0;
            celTitCristlaes.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCristlaes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCristales = new PdfPCell(new Phrase(datosEGO.Cristales, fontDato));
            celDatoCristales.BorderWidth = 0;
            celDatoCristales.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCristales.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celReferenciaCristales = new PdfPCell(new Phrase("X CPO", fontDato));
            celReferenciaCristales.BorderWidth = 0;
            celReferenciaCristales.VerticalAlignment = Element.ALIGN_CENTER;
            celReferenciaCristales.HorizontalAlignment = Element.ALIGN_CENTER;

            tblMicro.AddCell(celTituloMic);
            tblMicro.AddCell(celTituloMicReferencia);

            tblMicro.AddCell(celTitLeucocitos);
            tblMicro.AddCell(celDatoLeucocitos);
            tblMicro.AddCell(celReferenciaLeucocitos);

            tblMicro.AddCell(celTitEritrocitos);
            tblMicro.AddCell(celDatoEritrocitos);
            tblMicro.AddCell(celReferenciaEritrocitos);

            tblMicro.AddCell(celTitBacterias);
            tblMicro.AddCell(celDatoBacterias);
            tblMicro.AddCell(celReferenciaBacterias);

            tblMicro.AddCell(celTitCelulas);
            tblMicro.AddCell(celDatoCelulas);
            tblMicro.AddCell(celReferenciaCelulas);

            tblMicro.AddCell(celTitCilindros);
            tblMicro.AddCell(celDatoCilindros);
            tblMicro.AddCell(celReferenciaCilindros);

            tblMicro.AddCell(celTitCristlaes);
            tblMicro.AddCell(celDatoCristales);
            tblMicro.AddCell(celReferenciaCristales);

            docRep.Add(tblMicro);

            #endregion

            Paragraph observaciones = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            //observaciones.Add(new Paragraph("Observaciones ", fonEiqueta));
            //observaciones.Add(Chunk.TABBING);
            //observaciones.Add(new Paragraph(datosEGO.Observaciones, fontDato));
            observaciones.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
            observaciones.Add("Observaciones: ");
            observaciones.Add(Chunk.TABBING);
            observaciones.Font= FontFactory.GetFont("Arial", 10, Font.NORMAL);
            observaciones.Add(datosEGO.Observaciones);

            docRep.Add(observaciones);

            docRep.Close();
            byte[] bytesStream = msRep.ToArray();
            msRep = new MemoryStream();
            msRep.Write(bytesStream, 0, bytesStream.Length);
            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        //public IActionResult reporteBH(int idHistorico)
        //{
        //    var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
        //    var datosBH = repo.Getdosparam1<BhModel>("sp_medicos_rep_bh", new { @idhistorico = idHistorico }).FirstOrDefault();

        //    MemoryStream msRep = new MemoryStream();

        //    Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
        //    PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

        //    string elFolio = datosBH.FOLIO.ToString();
        //    string elRealizo = datosBH.realizo.ToString();
        //    string elCedRea = datosBH.ced_rea.ToString();
        //    string elSuperviso = datosBH.superviso.ToString();
        //    string elCedSup = datosBH.ced_sup.ToString();
        //    string elTitulo = "Biometría Hemática";

        //    pwRep.PageEvent = HeaderFooterEGO.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

        //    docRep.Open();

        //    var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
        //    var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

        //    #region emision - revision - codigo
        //    PdfPCell clRev = new PdfPCell(new Phrase("EMISION", fonEiqueta));
        //    clRev.BorderWidth = 0;
        //    clRev.VerticalAlignment = Element.ALIGN_BOTTOM;
        //    clRev.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celEmi = new PdfPCell(new Phrase("REVISION", fonEiqueta));
        //    celEmi.BorderWidth = 0;
        //    celEmi.VerticalAlignment = Element.ALIGN_BOTTOM;
        //    celEmi.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celCod = new PdfPCell(new Phrase("CODIGO", fonEiqueta));
        //    celCod.BorderWidth = 0;
        //    celCod.VerticalAlignment = Element.ALIGN_BOTTOM;
        //    celCod.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celEmi_b = new PdfPCell(new Phrase(DateTime.Now.Year.ToString(), fonEiqueta));
        //    celEmi_b.BorderWidth = 0;
        //    celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
        //    celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
        //    celEmi_b.BorderWidthBottom = 0.75f;

        //    PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
        //    celRev_b.BorderWidth = 0;
        //    celRev_b.VerticalAlignment = Element.ALIGN_TOP;
        //    celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
        //    celRev_b.BorderWidthBottom = 0.75f;

        //    PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/07", fonEiqueta));
        //    celCod_b.BorderWidth = 0;
        //    celCod_b.VerticalAlignment = Element.ALIGN_TOP;
        //    celCod_b.HorizontalAlignment = Element.ALIGN_CENTER;
        //    celCod_b.BorderWidthBottom = 0.75f;

        //    PdfPTable tblEmiRevCpd = new PdfPTable(3);
        //    tblEmiRevCpd.WidthPercentage = 100;
        //    float[] witdhs = new float[] { 33f, 33f, 33f };
        //    tblEmiRevCpd.SetWidths(witdhs);

        //    tblEmiRevCpd.AddCell(clRev);
        //    tblEmiRevCpd.AddCell(celEmi);
        //    tblEmiRevCpd.AddCell(celCod);

        //    tblEmiRevCpd.AddCell(celEmi_b);
        //    tblEmiRevCpd.AddCell(celRev_b);
        //    tblEmiRevCpd.AddCell(celCod_b);

        //    docRep.Add(tblEmiRevCpd);
        //    #endregion

        //    #region Titulo Datos personales
        //    PdfPTable Datospersonales = new PdfPTable(1);
        //    Datospersonales.TotalWidth = 560f;
        //    Datospersonales.LockedWidth = true;

        //    Datospersonales.SetWidths(widthsTitulosGenerales);
        //    Datospersonales.HorizontalAlignment = 0;
        //    Datospersonales.SpacingBefore = 10f;
        //    Datospersonales.SpacingAfter = 10f;

        //    PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //    cellTituloTituloFamiliar.HorizontalAlignment = 1;
        //    cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //    cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //    cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //    Datospersonales.AddCell(cellTituloTituloFamiliar);

        //    docRep.Add(Datospersonales);
        //    #endregion

        //    #region Tabla Datos Personales
        //    PdfPTable tblDatosEvaluado = new PdfPTable(4)
        //    {
        //        TotalWidth = 560,
        //        LockedWidth = true
        //    };

        //    float[] values = new float[4];
        //    values[0] = 80;
        //    values[1] = 270;
        //    values[2] = 100;
        //    values[3] = 110;
        //    tblDatosEvaluado.SetWidths(values);
        //    tblDatosEvaluado.HorizontalAlignment = 0;
        //    tblDatosEvaluado.SpacingAfter = 10f;
        //    //tblDatosEvaluado.SpacingBefore = 10f;
        //    tblDatosEvaluado.DefaultCell.Border = 0;

        //    //-------------------------------------------------------------------------------------------------------- 1a linea
        //    PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
        //    celTitnombre.BorderWidth = 0;
        //    celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos.evaluado, fontDato));
        //    celDatoEvaluado.BorderWidth = 0;
        //    celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
        //    celTitCodigo.BorderWidth = 0;
        //    celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos.codigoevaluado, fontDato));
        //    celDatoCodigo.BorderWidth = 0;
        //    celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- 2a linea
        //    PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
        //    celTitSexo.BorderWidth = 0;
        //    celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos.sexo, fontDato));
        //    celDatoSexo.BorderWidth = 0;
        //    celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
        //    celTitEvaluacion.BorderWidth = 0;
        //    celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos.evaluacion, fontDato));
        //    celDatoEvaluacion.BorderWidth = 0;
        //    celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- 3a linea
        //    PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
        //    celTitEdad.BorderWidth = 0;
        //    celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos.edad.ToString(), fontDato)); ;
        //    celDatoEdad.BorderWidth = 0;
        //    celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
        //    celTitFolio.BorderWidth = 0;
        //    celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos.folio, fontDato));
        //    celDatoFolio.BorderWidth = 0;
        //    celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- 4a linea
        //    PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
        //    celTitCurp.BorderWidth = 0;
        //    celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp, fontDato)); ;
        //    celDatoCurp.BorderWidth = 0;
        //    celDatoCurp.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoCurp.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celTitFecha = new PdfPCell(new Phrase("Fecha", fonEiqueta));
        //    celTitFecha.BorderWidth = 0;
        //    celTitFecha.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitFecha.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoFecha = new PdfPCell(new Phrase(DateTime.Now.ToShortDateString(), fontDato));
        //    celDatoFecha.BorderWidth = 0;
        //    celDatoFecha.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoFecha.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- 5a linea
        //    PdfPCell celTitDependencia = new PdfPCell(new Phrase("Dependencia", fonEiqueta));
        //    celTitDependencia.BorderWidth = 0;
        //    celTitDependencia.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos.dependencia, fontDato)) { Colspan = 3 };
        //    celDatoDependencia.BorderWidth = 0;
        //    celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- 6a linea
        //    PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
        //    celTitPuesto.BorderWidth = 0;
        //    celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos.puesto, fontDato)) { Colspan = 3 };
        //    celDatoPuesto.BorderWidth = 0;
        //    celDatoPuesto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celDatoPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

        //    tblDatosEvaluado.AddCell(celTitnombre);
        //    tblDatosEvaluado.AddCell(celDatoEvaluado);
        //    tblDatosEvaluado.AddCell(celTitCodigo);
        //    tblDatosEvaluado.AddCell(celDatoCodigo);

        //    tblDatosEvaluado.AddCell(celTitSexo);
        //    tblDatosEvaluado.AddCell(celDatoSexo);
        //    tblDatosEvaluado.AddCell(celTitEvaluacion);
        //    tblDatosEvaluado.AddCell(celDatoEvaluacion);

        //    tblDatosEvaluado.AddCell(celTitEdad);
        //    tblDatosEvaluado.AddCell(celDatoEdad);
        //    tblDatosEvaluado.AddCell(celTitFolio);
        //    tblDatosEvaluado.AddCell(celDatoFolio);

        //    tblDatosEvaluado.AddCell(celTitCurp);
        //    tblDatosEvaluado.AddCell(celDatoCurp);
        //    tblDatosEvaluado.AddCell(celTitFecha);
        //    tblDatosEvaluado.AddCell(celDatoFecha);

        //    tblDatosEvaluado.AddCell(celTitDependencia);
        //    tblDatosEvaluado.AddCell(celDatoDependencia);

        //    tblDatosEvaluado.AddCell(celTitPuesto);
        //    tblDatosEvaluado.AddCell(celDatoPuesto);

        //    docRep.Add(tblDatosEvaluado);

        //    #endregion

        //    #region Titulo Serie Blanca
        //    PdfPTable Serieblanca = new PdfPTable(1);
        //    Serieblanca.TotalWidth = 560f;
        //    Serieblanca.LockedWidth = true;

        //    Serieblanca.SetWidths(widthsTitulosGenerales);
        //    Serieblanca.HorizontalAlignment = 0;
        //    Serieblanca.SpacingAfter = 10f;

        //    PdfPCell cellTituloSerieblanca = new PdfPCell(new Phrase("SERIE BLANCA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //    cellTituloSerieblanca.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
        //    cellTituloSerieblanca.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //    cellTituloSerieblanca.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //    cellTituloSerieblanca.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //    Serieblanca.AddCell(cellTituloSerieblanca);

        //    docRep.Add(Serieblanca);
        //    #endregion

        //    #region Datos serie blanca
        //    PdfPTable tblSB = new PdfPTable(5)
        //    {
        //        TotalWidth = 560,
        //        LockedWidth = true
        //    };
        //    float[] valSB = new float[5];
        //    valSB[0] = 170;
        //    valSB[1] = 100;
        //    valSB[2] = 130;
        //    valSB[3] = 80;
        //    valSB[4] = 80;
        //    tblSB.SetWidths(valSB);
        //    tblSB.HorizontalAlignment = 0;
        //    tblSB.SpacingAfter = 10f;
        //    //tblDatosEvaluado.SpacingBefore = 10f;
        //    tblSB.DefaultCell.Border = 0;

        //    //-------------------------------------------------------------------------------------------------------- Leucocitos totales
        //    PdfPCell celSBLeucocito = new PdfPCell(new Phrase("Leucocitos totales", fonEiqueta));
        //    celSBLeucocito.BorderWidth = 0;
        //    celSBLeucocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBLeucocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Leucocitos = Convert.ToDecimal(datosBH.wbc);
        //    PdfPCell celSBDatoLeuco = new PdfPCell(new Phrase(Leucocitos.ToString("F2"), fontDato));
        //    celSBDatoLeuco.BorderWidth = 0;
        //    celSBDatoLeuco.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoLeuco.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBRef = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
        //    celSBRef.BorderWidth = 0;
        //    celSBRef.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBRef.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBAbs = new PdfPCell(new Phrase("4.0 - 10.5", fontDato));
        //    celSBAbs.BorderWidth = 0;
        //    celSBAbs.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbs.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBVacio = new PdfPCell(new Phrase("", fontDato));
        //    celSBVacio.BorderWidth = 0;
        //    celSBVacio.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBVacio.HorizontalAlignment = Element.ALIGN_LEFT;

        //    //-------------------------------------------------------------------------------------------------------- Titulos
        //    PdfPCell celSBcelda1 = new PdfPCell(new Phrase("", fonEiqueta));
        //    celSBcelda1.BorderWidth = 0;
        //    celSBcelda1.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBcelda1.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBcelda2 = new PdfPCell(new Phrase("Valor relativo", fonEiqueta));
        //    celSBcelda2.BorderWidth = 0;
        //    celSBcelda2.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBcelda2.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBCelda3 = new PdfPCell(new Phrase("Observado Absolutos", fonEiqueta));
        //    celSBCelda3.BorderWidth = 0;
        //    celSBCelda3.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBCelda3.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBCelda4 = new PdfPCell(new Phrase("Valores de Referencia", fonEiqueta)) { Colspan = 2 };
        //    celSBCelda4.BorderWidth = 0;
        //    celSBCelda4.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBCelda4.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Titulos
        //    PdfPCell celSBcelda1b = new PdfPCell(new Phrase("", fonEiqueta));
        //    celSBcelda1b.BorderWidth = 0;
        //    celSBcelda1b.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBcelda1b.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBcelda2b = new PdfPCell(new Phrase("", fonEiqueta));
        //    celSBcelda2b.BorderWidth = 0;
        //    celSBcelda2b.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBcelda2b.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBCelda3b = new PdfPCell(new Phrase("", fonEiqueta));
        //    celSBCelda3b.BorderWidth = 0;
        //    celSBCelda3b.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBCelda3b.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell celSBCelda4b = new PdfPCell(new Phrase("Relativos", fonEiqueta));
        //    celSBCelda4b.BorderWidth = 0;
        //    celSBCelda4b.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBCelda4b.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBCelda5b = new PdfPCell(new Phrase("Absolutos", fonEiqueta));
        //    celSBCelda5b.BorderWidth = 0;
        //    celSBCelda5b.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBCelda5b.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Linfocitos
        //    PdfPCell celSBLinfocito = new PdfPCell(new Phrase("Linfocitos", fonEiqueta));
        //    celSBLinfocito.BorderWidth = 0;
        //    celSBLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBLinfocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Linfocitos = Convert.ToDecimal(datosBH.Limph2);
        //    PdfPCell celSBDatoLinfocito = new PdfPCell(new Phrase(Linfocitos.ToString("F2"), fontDato));
        //    celSBDatoLinfocito.BorderWidth = 0;
        //    celSBDatoLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoLinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //    decimal Linfocitoabsoluto = Convert.ToDecimal(datosBH.Limph);
        //    PdfPCell celSBDatoLinfocitoabsoluto = new PdfPCell(new Phrase(Linfocitoabsoluto.ToString("F2"), fontDato));
        //    celSBDatoLinfocitoabsoluto.BorderWidth = 0;
        //    celSBDatoLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBAbslinfocito = new PdfPCell(new Phrase("20.0 - 40.0", fontDato));
        //    celSBAbslinfocito.BorderWidth = 0;
        //    celSBAbslinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbslinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBLinfocitoabsoluto = new PdfPCell(new Phrase("1.5 - 4.0", fontDato));
        //    celSBLinfocitoabsoluto.BorderWidth = 0;
        //    celSBLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Monocitos
        //    PdfPCell celSBMonocito = new PdfPCell(new Phrase("Monocitos", fonEiqueta));
        //    celSBMonocito.BorderWidth = 0;
        //    celSBMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBMonocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Monocitos = Convert.ToDecimal(datosBH.Mid2);
        //    PdfPCell celSBDatoMonocito = new PdfPCell(new Phrase(Monocitos.ToString("F2"), fontDato));
        //    celSBDatoMonocito.BorderWidth = 0;
        //    celSBDatoMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //    decimal Monocitoabsoluto = Convert.ToDecimal(datosBH.Mid);
        //    PdfPCell celSBDatoMonocitoabsoluto = new PdfPCell(new Phrase(Monocitoabsoluto.ToString("F2"), fontDato));
        //    celSBDatoMonocitoabsoluto.BorderWidth = 0;
        //    celSBDatoMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBAbsMonocito = new PdfPCell(new Phrase("3.0 - 10.0", fontDato));
        //    celSBAbsMonocito.BorderWidth = 0;
        //    celSBAbsMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbsMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBMonocitoabsoluto = new PdfPCell(new Phrase("0.2 - 0.9", fontDato));
        //    celSBMonocitoabsoluto.BorderWidth = 0;
        //    celSBMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Neutrofilos
        //    PdfPCell celSBNeutrofilos = new PdfPCell(new Phrase("Neutrófilos", fonEiqueta));
        //    celSBNeutrofilos.BorderWidth = 0;
        //    celSBNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBNeutrofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Neutrofilos = Convert.ToDecimal(datosBH.Neu2);
        //    PdfPCell celSBDatoNeutrofilos = new PdfPCell(new Phrase(Neutrofilos.ToString("F2"), fontDato));
        //    celSBDatoNeutrofilos.BorderWidth = 0;
        //    celSBDatoNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoNeutrofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //    decimal Neutrofiloabs = Convert.ToDecimal(datosBH.Neu);
        //    PdfPCell celSBDatoNeutrofiloabsoluto = new PdfPCell(new Phrase(Neutrofiloabs.ToString("F2"), fontDato));
        //    celSBDatoNeutrofiloabsoluto.BorderWidth = 0;
        //    celSBDatoNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBAbsNeutrofilo = new PdfPCell(new Phrase("50.0 - 70.0", fontDato));
        //    celSBAbsNeutrofilo.BorderWidth = 0;
        //    celSBAbsNeutrofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbsNeutrofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBNeutrofiloabsoluto = new PdfPCell(new Phrase("1.8 - 7.2", fontDato));
        //    celSBNeutrofiloabsoluto.BorderWidth = 0;
        //    celSBNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Eosinofilos
        //    PdfPCell celSBEosinofilos = new PdfPCell(new Phrase("Eosinófilos", fonEiqueta));
        //    celSBEosinofilos.BorderWidth = 0;
        //    celSBEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBEosinofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Eosinofilos = Convert.ToDecimal(datosBH.Eos2);
        //    PdfPCell celSBDatoEosinofilos = new PdfPCell(new Phrase(Eosinofilos.ToString("F2"), fontDato));
        //    celSBDatoEosinofilos.BorderWidth = 0;
        //    celSBDatoEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoEosinofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //    decimal Eofinofiloabs = Convert.ToDecimal(datosBH.Eos);
        //    PdfPCell celSBDatoEosinofiloabsoluto = new PdfPCell(new Phrase(Eofinofiloabs.ToString("F2"), fontDato));
        //    celSBDatoEosinofiloabsoluto.BorderWidth = 0;
        //    celSBDatoEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBAbsEosinofilo = new PdfPCell(new Phrase("0 - 3", fontDato));
        //    celSBAbsEosinofilo.BorderWidth = 0;
        //    celSBAbsEosinofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbsEosinofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBEosinofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.7", fontDato));
        //    celSBEosinofiloabsoluto.BorderWidth = 0;
        //    celSBEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    //-------------------------------------------------------------------------------------------------------- Basofilos
        //    PdfPCell celSBBasofilos = new PdfPCell(new Phrase("Basófilos", fonEiqueta));
        //    celSBBasofilos.BorderWidth = 0;
        //    celSBBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBBasofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //    decimal Basofilos = Convert.ToDecimal(datosBH.Bas2);
        //    PdfPCell celSBDatoBasofilos = new PdfPCell(new Phrase(Basofilos.ToString("F2"), fontDato));
        //    celSBDatoBasofilos.BorderWidth = 0;
        //    celSBDatoBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoBasofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //    decimal Basofiloabs = Convert.ToDecimal(datosBH.Bas);
        //    PdfPCell celSBDatoBasofiloabsoluto = new PdfPCell(new Phrase(Basofiloabs.ToString("F2"), fontDato));
        //    celSBDatoBasofiloabsoluto.BorderWidth = 0;
        //    celSBDatoBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBDatoBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBAbsBasofilo = new PdfPCell(new Phrase("0 - 1 ", fontDato));
        //    celSBAbsBasofilo.BorderWidth = 0;
        //    celSBAbsBasofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBAbsBasofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //    PdfPCell celSBBasofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.15", fontDato));
        //    celSBBasofiloabsoluto.BorderWidth = 0;
        //    celSBBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //    celSBBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //    tblSB.AddCell(celSBLeucocito);
        //    tblSB.AddCell(celSBDatoLeuco);
        //    tblSB.AddCell(celSBRef);
        //    tblSB.AddCell(celSBAbs);
        //    tblSB.AddCell(celSBVacio);

        //    tblSB.AddCell(celSBcelda1);
        //    tblSB.AddCell(celSBcelda2);
        //    tblSB.AddCell(celSBCelda3);
        //    tblSB.AddCell(celSBCelda4);

        //    tblSB.AddCell(celSBcelda1b);
        //    tblSB.AddCell(celSBcelda2b);
        //    tblSB.AddCell(celSBCelda3b);
        //    tblSB.AddCell(celSBCelda4b);
        //    tblSB.AddCell(celSBCelda5b);

        //    tblSB.AddCell(celSBLinfocito);
        //    tblSB.AddCell(celSBDatoLinfocito);
        //    tblSB.AddCell(celSBDatoLinfocitoabsoluto);
        //    tblSB.AddCell(celSBAbslinfocito);
        //    tblSB.AddCell(celSBLinfocitoabsoluto);

        //    tblSB.AddCell(celSBMonocito);
        //    tblSB.AddCell(celSBDatoMonocito);
        //    tblSB.AddCell(celSBDatoMonocitoabsoluto);
        //    tblSB.AddCell(celSBAbsMonocito);
        //    tblSB.AddCell(celSBMonocitoabsoluto);

        //    tblSB.AddCell(celSBNeutrofilos);
        //    tblSB.AddCell(celSBDatoNeutrofilos);
        //    tblSB.AddCell(celSBDatoNeutrofiloabsoluto);
        //    tblSB.AddCell(celSBAbsNeutrofilo);
        //    tblSB.AddCell(celSBNeutrofiloabsoluto);

        //    tblSB.AddCell(celSBEosinofilos);
        //    tblSB.AddCell(celSBDatoEosinofilos);
        //    tblSB.AddCell(celSBDatoEosinofiloabsoluto);
        //    tblSB.AddCell(celSBAbsEosinofilo);
        //    tblSB.AddCell(celSBEosinofiloabsoluto);

        //    tblSB.AddCell(celSBBasofilos);
        //    tblSB.AddCell(celSBDatoBasofilos);
        //    tblSB.AddCell(celSBDatoBasofiloabsoluto);
        //    tblSB.AddCell(celSBAbsBasofilo);
        //    tblSB.AddCell(celSBBasofiloabsoluto);

        //    docRep.Add(tblSB);

        //    #endregion

        //    #region Titulo Serie Roja
        //    PdfPTable Serieroja = new PdfPTable(1);
        //    Serieroja.TotalWidth = 560f;
        //    Serieroja.LockedWidth = true;

        //    Serieroja.SetWidths(widthsTitulosGenerales);
        //    Serieroja.HorizontalAlignment = 0;
        //    Serieroja.SpacingAfter = 10f;

        //    PdfPCell cellTituloSerieroja = new PdfPCell(new Phrase("SERIE ROJA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //    cellTituloSerieroja.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
        //    cellTituloSerieroja.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //    cellTituloSerieroja.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //    cellTituloSerieroja.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //    Serieroja.AddCell(cellTituloSerieroja);

        //    docRep.Add(Serieroja);
        //    #endregion

        //    #region Datos serie roja
        //    PdfPTable DtsSR = new PdfPTable(5)
        //    {
        //        TotalWidth = 560,
        //        LockedWidth = true
        //    };

        //    float[] valSR = new float[5];
        //    valSR[0] = 150;
        //    valSR[1] = 50;
        //    valSR[2] = 100;
        //    valSR[3] = 130;
        //    valSR[4] = 130;
        //    DtsSR.SetWidths(valSR);
        //    DtsSR.HorizontalAlignment = 0;
        //    DtsSR.SpacingAfter = 20f;
        //    DtsSR.DefaultCell.Border = 0;

        //    //----------------------------------------------------------------------------------------Valores de referencia
        //    PdfPCell c1 = new PdfPCell(new Phrase("", fontDato)) { Colspan = 3 };
        //    c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c1.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c1);

        //    PdfPCell c2 = new PdfPCell(new Phrase("Valores de referencia", fonEiqueta)) { Colspan = 2 };
        //    c2.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c2.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c2);

        //    //----------------------------------------------------------------------------------------Hemogloblina
        //    string rangoHemoglobina = datos.sexo == "HOMBRE" ? "14.0 - 17.0" : "11.0 - 14.0";
        //    PdfPCell hemo = new PdfPCell(new Phrase("Hemoglobina", fonEiqueta));
        //    hemo.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hemo.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hemo);

        //    PdfPCell fr_hemo = new PdfPCell(new Phrase(datosBH.fr_hgb, fonEiqueta));
        //    fr_hemo.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_hemo.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_hemo);

        //    decimal hemovalor = Convert.ToDecimal(datosBH.HGB);
        //    PdfPCell chemovalor = new PdfPCell(new Phrase(hemovalor.ToString("F2"), fontDato));
        //    chemovalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    chemovalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(chemovalor);

        //    PdfPCell hemoUnidad = new PdfPCell(new Phrase("g/dL", fontDato));
        //    hemoUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hemoUnidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hemoUnidad);

        //    PdfPCell hemoreferencia = new PdfPCell(new Phrase(rangoHemoglobina, fontDato));
        //    hemoreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hemoreferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hemoreferencia);

        //    //----------------------------------------------------------------------------------------Eritrocitos
        //    PdfPCell eritro = new PdfPCell(new Phrase("Eritrocitos", fonEiqueta));
        //    eritro.HorizontalAlignment = Element.ALIGN_LEFT;
        //    eritro.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(eritro);

        //    PdfPCell fr_eri = new PdfPCell(new Phrase(datosBH.fr_rbc, fonEiqueta));
        //    fr_eri.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_eri.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_eri);

        //    decimal eritrovalor = Convert.ToDecimal(datosBH.RBC);
        //    PdfPCell cheritrovalor = new PdfPCell(new Phrase(eritrovalor.ToString("F2"), fontDato));
        //    cheritrovalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    cheritrovalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(cheritrovalor);

        //    PdfPCell eriUnidad = new PdfPCell(new Phrase("x 10^6 / uL", fontDato));
        //    eriUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    eriUnidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(eriUnidad);

        //    PdfPCell erireferencia = new PdfPCell(new Phrase("4.00 - 5.50", fontDato));
        //    erireferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    erireferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(erireferencia);

        //    //----------------------------------------------------------------------------------------HTC
        //    PdfPCell htc = new PdfPCell(new Phrase("HTC", fonEiqueta));
        //    htc.HorizontalAlignment = Element.ALIGN_LEFT;
        //    htc.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(htc);

        //    PdfPCell fr_htc = new PdfPCell(new Phrase(datosBH.fr_htc, fonEiqueta));
        //    fr_htc.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_htc.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_htc);

        //    decimal htcovalor = Convert.ToDecimal(datosBH.HTC);
        //    PdfPCell chtcvalor = new PdfPCell(new Phrase(htcovalor.ToString("F2"), fontDato));
        //    chtcvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    chtcvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(chtcvalor);

        //    PdfPCell htcUnidad = new PdfPCell(new Phrase("%", fontDato));
        //    htcUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    htcUnidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(htcUnidad);

        //    PdfPCell htcreferencia = new PdfPCell(new Phrase("40 - 50", fontDato));
        //    htcreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    htcreferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(htcreferencia);

        //    //----------------------------------------------------------------------------------------VGM
        //    PdfPCell vgm = new PdfPCell(new Phrase("VGM", fonEiqueta));
        //    vgm.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vgm.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vgm);

        //    PdfPCell fr_mcv = new PdfPCell(new Phrase(datosBH.fr_mcv, fonEiqueta));
        //    fr_mcv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_mcv.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_mcv);

        //    decimal vgm_valor = Convert.ToDecimal(datosBH.MCv);
        //    PdfPCell c_vgmvalor = new PdfPCell(new Phrase(vgm_valor.ToString("F2"), fontDato));
        //    c_vgmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_vgmvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_vgmvalor);

        //    PdfPCell vgm_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //    vgm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vgm_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vgm_Unidad);

        //    PdfPCell vgm_creferencia = new PdfPCell(new Phrase("82.0 - 95.0", fontDato));
        //    vgm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vgm_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vgm_creferencia);

        //    //----------------------------------------------------------------------------------------HCM
        //    PdfPCell hcm = new PdfPCell(new Phrase("HCM", fonEiqueta));
        //    hcm.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hcm.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hcm);

        //    PdfPCell fr_mch = new PdfPCell(new Phrase(datosBH.fr_mch, fonEiqueta));
        //    fr_mch.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_mch.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_mch);

        //    decimal hcm_valor = Convert.ToDecimal(datosBH.MCH);
        //    PdfPCell c_hcmvalor = new PdfPCell(new Phrase(hcm_valor.ToString("F2"), fontDato));
        //    c_hcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_hcmvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_hcmvalor);

        //    PdfPCell hcm_Unidad = new PdfPCell(new Phrase("pg/fL", fontDato));
        //    hcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hcm_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hcm_Unidad);

        //    PdfPCell hcm_creferencia = new PdfPCell(new Phrase("27.0 - 31.0", fontDato));
        //    hcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    hcm_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(hcm_creferencia);

        //    //----------------------------------------------------------------------------------------CHCM
        //    PdfPCell chcm = new PdfPCell(new Phrase("CHCM", fonEiqueta));
        //    chcm.HorizontalAlignment = Element.ALIGN_LEFT;
        //    chcm.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(chcm);

        //    PdfPCell fr_mchc = new PdfPCell(new Phrase(datosBH.fr_mchc, fonEiqueta));
        //    fr_mchc.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_mchc.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_mchc);

        //    decimal chcm_valor = Convert.ToDecimal(datosBH.MCHC);
        //    PdfPCell c_chcmvalor = new PdfPCell(new Phrase(chcm_valor.ToString("F2"), fontDato));
        //    c_chcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_chcmvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_chcmvalor);

        //    PdfPCell chcm_Unidad = new PdfPCell(new Phrase("g/dL", fontDato));
        //    chcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    chcm_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(chcm_Unidad);

        //    PdfPCell chcm_creferencia = new PdfPCell(new Phrase("32.0 - 36.0", fontDato));
        //    chcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    chcm_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(chcm_creferencia);

        //    //----------------------------------------------------------------------------------------Plaquetas
        //    PdfPCell plaquetas = new PdfPCell(new Phrase("Plaquetas", fonEiqueta));
        //    plaquetas.HorizontalAlignment = Element.ALIGN_LEFT;
        //    plaquetas.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(plaquetas);

        //    PdfPCell fr_plt = new PdfPCell(new Phrase(datosBH.fr_plt, fonEiqueta));
        //    fr_plt.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_plt.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_plt);

        //    decimal plaquetas_valor = Convert.ToDecimal(datosBH.PLT);
        //    PdfPCell c_plaquetasvalor = new PdfPCell(new Phrase(plaquetas_valor.ToString("F2"), fontDato));
        //    c_plaquetasvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_plaquetasvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_plaquetasvalor);

        //    PdfPCell plaquetas_Unidad = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
        //    plaquetas_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    plaquetas_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(plaquetas_Unidad);

        //    PdfPCell plaquetas_creferencia = new PdfPCell(new Phrase("175 - 400", fontDato));
        //    plaquetas_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    plaquetas_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(plaquetas_creferencia);

        //    //----------------------------------------------------------------------------------------PCT
        //    PdfPCell pct = new PdfPCell(new Phrase("PCT", fonEiqueta));
        //    pct.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pct.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pct);

        //    PdfPCell fr_pct = new PdfPCell(new Phrase(datosBH.fr_pct, fonEiqueta));
        //    fr_pct.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_pct.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_pct);

        //    decimal pct_valor = Convert.ToDecimal(datosBH.PCT);
        //    PdfPCell c_pctvalor = new PdfPCell(new Phrase(pct_valor.ToString("F3"), fontDato));
        //    c_pctvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_pctvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_pctvalor);

        //    PdfPCell pct_Unidad = new PdfPCell(new Phrase("%", fontDato));
        //    pct_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pct_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pct_Unidad);

        //    PdfPCell pct_creferencia = new PdfPCell(new Phrase("0.108 - 0.282", fontDato));
        //    pct_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pct_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pct_creferencia);

        //    //----------------------------------------------------------------------------------------RDWCV
        //    PdfPCell rdwcv = new PdfPCell(new Phrase("RDWCV", fonEiqueta));
        //    rdwcv.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwcv.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwcv);

        //    PdfPCell fr_rdwcv = new PdfPCell(new Phrase(datosBH.fr_rdwcv, fonEiqueta));
        //    fr_rdwcv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_rdwcv.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_rdwcv);

        //    decimal rdwcv_valor = Convert.ToDecimal(datosBH.RDWCV);
        //    PdfPCell c_rdwcvvalor = new PdfPCell(new Phrase(rdwcv_valor.ToString("F2"), fontDato));
        //    c_rdwcvvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_rdwcvvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_rdwcvvalor);

        //    PdfPCell rdwcv_Unidad = new PdfPCell(new Phrase("%", fontDato));
        //    rdwcv_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwcv_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwcv_Unidad);

        //    PdfPCell rdwcv_creferencia = new PdfPCell(new Phrase("11.5 - 14.5", fontDato));
        //    rdwcv_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwcv_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwcv_creferencia);

        //    //----------------------------------------------------------------------------------------RDWSD
        //    PdfPCell rdwsd = new PdfPCell(new Phrase("RDWSD", fonEiqueta));
        //    rdwsd.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwsd.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwsd);

        //    PdfPCell fr_rdwsd = new PdfPCell(new Phrase(datosBH.fr_rdwsd, fonEiqueta));
        //    fr_rdwsd.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_rdwsd.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_rdwsd);

        //    decimal rdwsd_valor = Convert.ToDecimal(datosBH.RDWSD);
        //    PdfPCell c_rdwsdvalor = new PdfPCell(new Phrase(rdwsd_valor.ToString("F2"), fontDato));
        //    c_rdwsdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_rdwsdvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_rdwsdvalor);

        //    PdfPCell rdwsd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //    rdwsd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwsd_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwsd_Unidad);

        //    PdfPCell rdwsd_creferencia = new PdfPCell(new Phrase("35.5 - 56.0", fontDato));
        //    rdwsd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    rdwsd_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(rdwsd_creferencia);

        //    //----------------------------------------------------------------------------------------VPM
        //    PdfPCell vpmd = new PdfPCell(new Phrase("VPM", fonEiqueta));
        //    vpmd.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vpmd.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vpmd);

        //    PdfPCell fr_mpv = new PdfPCell(new Phrase(datosBH.fr_mpv, fonEiqueta));
        //    fr_mpv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_mpv.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_mpv);

        //    decimal vpmd_valor = Convert.ToDecimal(datosBH.MPV);
        //    PdfPCell c_vpmdvalor = new PdfPCell(new Phrase(vpmd_valor.ToString("F2"), fontDato));
        //    c_vpmdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_vpmdvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_vpmdvalor);

        //    PdfPCell vpmd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //    vpmd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vpmd_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vpmd_Unidad);

        //    PdfPCell vpmd_creferencia = new PdfPCell(new Phrase("7.0 - 11.0", fontDato));
        //    vpmd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    vpmd_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(vpmd_creferencia);

        //    //----------------------------------------------------------------------------------------PDW
        //    PdfPCell pdw = new PdfPCell(new Phrase("PDW", fonEiqueta));
        //    pdw.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pdw.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pdw);

        //    PdfPCell fr_pdw = new PdfPCell(new Phrase(datosBH.fr_pdw, fonEiqueta));
        //    fr_pdw.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    fr_pdw.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(fr_pdw);

        //    decimal pdw_valor = Convert.ToDecimal(datosBH.PDW);
        //    PdfPCell c_pdwvalor = new PdfPCell(new Phrase(pdw_valor.ToString("F2"), fontDato));
        //    c_pdwvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //    c_pdwvalor.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(c_pdwvalor);

        //    PdfPCell pdw_Unidad = new PdfPCell(new Phrase("", fontDato));
        //    pdw_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pdw_Unidad.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pdw_Unidad);

        //    PdfPCell pdw_creferencia = new PdfPCell(new Phrase("15.0 - 17.0", fontDato));
        //    pdw_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //    pdw_creferencia.Border = PdfPCell.NO_BORDER;
        //    DtsSR.AddCell(pdw_creferencia);

        //    docRep.Add(DtsSR);
        //    #endregion

        //    Paragraph observaciones_bh = new Paragraph()
        //    {
        //        Alignment = Element.ALIGN_LEFT
        //    };
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //    observaciones_bh.Add("Metodologia: ");
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //    observaciones_bh.Add(Chunk.TABBING);
        //    observaciones_bh.Add("Impedancia eléctrica y colorimetría por equipo Mindray BV-30s.");
        //    observaciones_bh.Add(Chunk.NEWLINE);
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //    observaciones_bh.Add("Observaciones: ");
        //    observaciones_bh.Add(Chunk.TABBING);
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //    observaciones_bh.Add(datosBH.Observacion);
        //    observaciones_bh.Add(Chunk.NEWLINE);
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //    observaciones_bh.Add("Espécimen: ");
        //    observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //    observaciones_bh.Add(Chunk.TABBING);
        //    observaciones_bh.Add("Sangre total");

        //    docRep.Add(observaciones_bh);

        //    Paragraph valorfueraderango = new Paragraph()
        //    {
        //        Alignment = Element.ALIGN_RIGHT
        //    };
        //    valorfueraderango.Font = FontFactory.GetFont("Arial", 9, Font.NORMAL);
        //    valorfueraderango.Add("* = valor fuera de rango");

        //    docRep.Add(valorfueraderango);

        //    docRep.Close();
        //    byte[] bytesStream = msRep.ToArray();
        //    msRep = new MemoryStream();
        //    msRep.Write(bytesStream, 0, bytesStream.Length);
        //    msRep.Position = 0;

        //    return new FileStreamResult(msRep, "application/pdf");
        //}

        public IActionResult reporteBH(int idHistorico)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosBH = repo.Getdosparam1<BhModel>("sp_medicos_rep_bh", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datosBH.FOLIO.ToString();
            string elRealizo = datosBH.realizo.ToString();
            string elCedRea = datosBH.ced_rea.ToString();
            string elSuperviso = datosBH.superviso.ToString();
            string elCedSup = datosBH.ced_sup.ToString();
            string elTitulo = "Biometría Hemática";

            pwRep.PageEvent = HeaderFooterEGO.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

            docRep.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

            #region Flechas
            string flechaAbajo = @"C:/inetpub/wwwroot/fotoUser/fabajo.png";
            string flechaArriba = @"C:/inetpub/wwwroot/fotoUser/farriba.png";
            //string flechaNada = @"C:/inetpub/wwwroot/fotoUser/fnada.png";

            iTextSharp.text.Image fAbajo = iTextSharp.text.Image.GetInstance(flechaAbajo);
            fAbajo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            fAbajo.ScaleToFit(8f, 8f);

            iTextSharp.text.Image fArriba = iTextSharp.text.Image.GetInstance(flechaArriba);
            fArriba.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            fArriba.ScaleToFit(8f, 8f);
            #endregion

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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/07", fonEiqueta));
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

            docRep.Add(tblEmiRevCpd);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);
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
            tblDatosEvaluado.SpacingAfter = 10f;
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
            PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
            celTitCurp.BorderWidth = 0;
            celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp, fontDato)); ;
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

            docRep.Add(tblDatosEvaluado);

            #endregion

            #region Titulo Serie Blanca
            PdfPTable Serieblanca = new PdfPTable(1);
            Serieblanca.TotalWidth = 560f;
            Serieblanca.LockedWidth = true;

            Serieblanca.SetWidths(widthsTitulosGenerales);
            Serieblanca.HorizontalAlignment = 0;
            Serieblanca.SpacingAfter = 10f;

            PdfPCell cellTituloSerieblanca = new PdfPCell(new Phrase("SERIE BLANCA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloSerieblanca.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
            cellTituloSerieblanca.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloSerieblanca.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloSerieblanca.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Serieblanca.AddCell(cellTituloSerieblanca);

            docRep.Add(Serieblanca);
            #endregion

            #region Datos serie blanca
            PdfPTable tblSB = new PdfPTable(7)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valSB = new float[7];
            valSB[0] = 130;
            valSB[1] = 20;
            valSB[2] = 120;
            valSB[3] = 20;
            valSB[4] = 110;
            valSB[5] = 80;
            valSB[6] = 80;
            tblSB.SetWidths(valSB);
            tblSB.HorizontalAlignment = 0;
            tblSB.SpacingAfter = 10f;
            //tblDatosEvaluado.SpacingBefore = 10f;
            tblSB.DefaultCell.Border = 0;

            //-------------------------------------------------------------------------------------------------------- Leucocitos totales
            //[0] - 130
            PdfPCell celSBLeucocito = new PdfPCell(new Phrase("Leucocitos totales", fonEiqueta));
            celSBLeucocito.BorderWidth = 0;
            celSBLeucocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBLeucocito.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSFlechas = new PdfPCell();
            switch (datosBH.fr_wbc)
            {
                case -1:
                    celSFlechas.AddElement(fAbajo);
                    break;
                case 1:
                    celSFlechas.AddElement(fArriba);
                    break;
            }
            celSFlechas.BorderWidth = 0;
            celSFlechas.VerticalAlignment = Element.ALIGN_CENTER;
            celSFlechas.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            decimal Leucocitos = Convert.ToDecimal(datosBH.wbc);
            PdfPCell celSBDatoLeuco = new PdfPCell(new Phrase(Leucocitos.ToString("F2"), fontDato));
            celSBDatoLeuco.BorderWidth = 0;
            celSBDatoLeuco.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoLeuco.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celVacio01 = new PdfPCell(new Phrase("", fontDato));
            celVacio01.BorderWidth = 0;
            celVacio01.VerticalAlignment = Element.ALIGN_CENTER;
            celVacio01.HorizontalAlignment = Element.ALIGN_CENTER;

            //[4] - 110
            PdfPCell celSBRef = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
            celSBRef.BorderWidth = 0;
            celSBRef.VerticalAlignment = Element.ALIGN_CENTER;
            celSBRef.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbs = new PdfPCell(new Phrase("4.0 - 10.5", fontDato));
            celSBAbs.BorderWidth = 0;
            celSBAbs.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbs.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBVacio = new PdfPCell(new Phrase("", fontDato));
            celSBVacio.BorderWidth = 0;
            celSBVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celSBVacio.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Titulos
            //[0] - 130
            PdfPCell celSBcelda1 = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda1.BorderWidth = 0;
            celSBcelda1.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda1.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBcelda1FleVai = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda1FleVai.BorderWidth = 0;
            celSBcelda1FleVai.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda1FleVai.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            PdfPCell celSBcelda2 = new PdfPCell(new Phrase("Valor relativo", fonEiqueta));
            celSBcelda2.BorderWidth = 0;
            celSBcelda2.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda2.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBcelda1Fle = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda1Fle.BorderWidth = 0;
            celSBcelda1Fle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda1Fle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            PdfPCell celSBCelda3 = new PdfPCell(new Phrase("Observado Absolutos", fonEiqueta));
            celSBCelda3.BorderWidth = 0;
            celSBCelda3.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda3.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 160
            PdfPCell celSBCelda4 = new PdfPCell(new Phrase("Valores de Referencia", fonEiqueta)) { Colspan = 2 };
            celSBCelda4.BorderWidth = 0;
            celSBCelda4.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda4.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Titulos
            //[0] - 130
            PdfPCell celSBcelda1b = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda1b.BorderWidth = 0;
            celSBcelda1b.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda1b.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBcelda1bFle = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda1bFle.BorderWidth = 0;
            celSBcelda1bFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda1Fle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            PdfPCell celSBcelda2b = new PdfPCell(new Phrase("", fonEiqueta));
            celSBcelda2b.BorderWidth = 0;
            celSBcelda2b.VerticalAlignment = Element.ALIGN_CENTER;
            celSBcelda2b.HorizontalAlignment = Element.ALIGN_LEFT;

            //[3] - 20
            PdfPCell celSBCelda3b = new PdfPCell(new Phrase("", fonEiqueta));
            celSBCelda3b.BorderWidth = 0;
            celSBCelda3b.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda3b.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            PdfPCell celSBCelda3bVacia = new PdfPCell(new Phrase("", fonEiqueta));
            celSBCelda3bVacia.BorderWidth = 0;
            celSBCelda3bVacia.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda3bVacia.HorizontalAlignment = Element.ALIGN_LEFT;

            //[5] - 80
            PdfPCell celSBCelda4b = new PdfPCell(new Phrase("Relativos", fonEiqueta));
            celSBCelda4b.BorderWidth = 0;
            celSBCelda4b.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda4b.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBCelda5b = new PdfPCell(new Phrase("Absolutos", fonEiqueta));
            celSBCelda5b.BorderWidth = 0;
            celSBCelda5b.VerticalAlignment = Element.ALIGN_CENTER;
            celSBCelda5b.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Linfocitos
            //[0] - 130
            PdfPCell celSBLinfocito = new PdfPCell(new Phrase("Linfocitos", fonEiqueta));
            celSBLinfocito.BorderWidth = 0;
            celSBLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBLinfocito.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBDatoLinfocitoVacio = new PdfPCell(new Phrase("", fontDato));
            celSBDatoLinfocitoVacio.BorderWidth = 0;
            celSBDatoLinfocitoVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoLinfocitoVacio.HorizontalAlignment = Element.ALIGN_CENTER;

            //[2] - 120
            decimal Linfocitos = Convert.ToDecimal(datosBH.Limph2);
            PdfPCell celSBDatoLinfocito = new PdfPCell(new Phrase(Linfocitos.ToString("F2"), fontDato));
            celSBDatoLinfocito.BorderWidth = 0;
            celSBDatoLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoLinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBLinfocitoFle = new PdfPCell();
            switch (datosBH.fr_limph)
            {
                case -1:
                    celSBLinfocitoFle.AddElement(fAbajo);
                    break;
                case 1:
                    celSBLinfocitoFle.AddElement(fArriba);
                    break;
            }
            celSBLinfocitoFle.BorderWidth = 0;
            celSBLinfocitoFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBLinfocitoFle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal Linfocitoabsoluto = Convert.ToDecimal(datosBH.Limph);
            PdfPCell celSBDatoLinfocitoabsoluto = new PdfPCell(new Phrase(Linfocitoabsoluto.ToString("F2"), fontDato));
            celSBDatoLinfocitoabsoluto.BorderWidth = 0;
            celSBDatoLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbslinfocito = new PdfPCell(new Phrase("20.0 - 40.0", fontDato));
            celSBAbslinfocito.BorderWidth = 0;
            celSBAbslinfocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbslinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBLinfocitoabsoluto = new PdfPCell(new Phrase("1.5 - 4.0", fontDato));
            celSBLinfocitoabsoluto.BorderWidth = 0;
            celSBLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Monocitos
            //[0] - 130
            PdfPCell celSBMonocito = new PdfPCell(new Phrase("Monocitos", fonEiqueta));
            celSBMonocito.BorderWidth = 0;
            celSBMonocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBMonocito.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBMonocitoVacio = new PdfPCell(new Phrase("", fontDato));
            celSBMonocitoVacio.BorderWidth = 0;
            celSBMonocitoVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celSBMonocitoVacio.HorizontalAlignment = Element.ALIGN_CENTER;

            //[2] - 120
            decimal Monocitos = Convert.ToDecimal(datosBH.Mid2);
            PdfPCell celSBDatoMonocito = new PdfPCell(new Phrase(Monocitos.ToString("F2"), fontDato));
            celSBDatoMonocito.BorderWidth = 0;
            celSBDatoMonocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBMonocitoFle = new PdfPCell();
            switch (datosBH.fr_mid)
            {
                case -1:
                    celSBMonocitoFle.AddElement(fAbajo);
                    break;
                case 1:
                    celSBMonocitoFle.AddElement(fArriba);
                    break;
            }
            celSBMonocitoFle.BorderWidth = 0;
            celSBMonocitoFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBMonocitoFle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal Monocitoabsoluto = Convert.ToDecimal(datosBH.Mid);
            PdfPCell celSBDatoMonocitoabsoluto = new PdfPCell(new Phrase(Monocitoabsoluto.ToString("F2"), fontDato));
            celSBDatoMonocitoabsoluto.BorderWidth = 0;
            celSBDatoMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbsMonocito = new PdfPCell(new Phrase("3.0 - 10.0", fontDato));
            celSBAbsMonocito.BorderWidth = 0;
            celSBAbsMonocito.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbsMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBMonocitoabsoluto = new PdfPCell(new Phrase("0.2 - 0.9", fontDato));
            celSBMonocitoabsoluto.BorderWidth = 0;
            celSBMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Neutrofilos segmetnados
            //[0] - 130
            PdfPCell celSBNeutrofilos = new PdfPCell(new Phrase("Neutrófilos segmentados", fonEiqueta));
            celSBNeutrofilos.BorderWidth = 0;
            celSBNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilos.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBNeutrofilosVacio = new PdfPCell(new Phrase("", fonEiqueta));
            celSBNeutrofilosVacio.BorderWidth = 0;
            celSBNeutrofilosVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilosVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            decimal Neutrofilos = Convert.ToDecimal(datosBH.Neu2);
            PdfPCell celSBDatoNeutrofilos = new PdfPCell(new Phrase(Neutrofilos.ToString("F2"), fontDato));
            celSBDatoNeutrofilos.BorderWidth = 0;
            celSBDatoNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoNeutrofilos.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBNeutrofilosFle = new PdfPCell();
            switch (datosBH.fr_neu)
            {
                case -1:
                    celSBNeutrofilosFle.AddElement(fAbajo);
                    break;
                case 1:
                    celSBNeutrofilosFle.AddElement(fArriba);
                    break;
            }
            celSBNeutrofilosFle.BorderWidth = 0;
            celSBNeutrofilosFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilosFle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal Neutrofiloabs = Convert.ToDecimal(datosBH.Neu);
            PdfPCell celSBDatoNeutrofiloabsoluto = new PdfPCell(new Phrase(Neutrofiloabs.ToString("F2"), fontDato));
            celSBDatoNeutrofiloabsoluto.BorderWidth = 0;
            celSBDatoNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbsNeutrofilo = new PdfPCell(new Phrase("50.0 - 70.0", fontDato));
            celSBAbsNeutrofilo.BorderWidth = 0;
            celSBAbsNeutrofilo.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbsNeutrofilo.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBNeutrofiloabsoluto = new PdfPCell(new Phrase("1.8 - 7.2", fontDato));
            celSBNeutrofiloabsoluto.BorderWidth = 0;
            celSBNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Neutrofilos en banda
            //[0] - 130
            PdfPCell celSBNeutrofilosenBanda = new PdfPCell(new Phrase("Neutrófilos en Banda", fonEiqueta));
            celSBNeutrofilosenBanda.BorderWidth = 0;
            celSBNeutrofilosenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilosenBanda.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBNeutrofilosVacioenBanda = new PdfPCell(new Phrase("", fonEiqueta));
            celSBNeutrofilosVacioenBanda.BorderWidth = 0;
            celSBNeutrofilosVacioenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilosVacioenBanda.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            decimal NeutrofilosenBanda = Convert.ToDecimal(datosBH.Banda2);
            PdfPCell celSBDatoNeutrofilosenBanda = new PdfPCell(new Phrase(NeutrofilosenBanda.ToString("F2"), fontDato));
            celSBDatoNeutrofilosenBanda.BorderWidth = 0;
            celSBDatoNeutrofilosenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoNeutrofilosenBanda.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBNeutrofilosFleenBanda = new PdfPCell();
            switch (datosBH.fr_banda)
            {
                case -1:
                    celSBNeutrofilosFleenBanda.AddElement(fAbajo);
                    break;
                case 1:
                    celSBNeutrofilosFleenBanda.AddElement(fArriba);
                    break;
            }
            celSBNeutrofilosFleenBanda.BorderWidth = 0;
            celSBNeutrofilosFleenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofilosFleenBanda.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal NeutrofiloabsenBanda = Convert.ToDecimal(datosBH.Banda);
            PdfPCell celSBDatoNeutrofiloabsolutoenBanda = new PdfPCell(new Phrase(NeutrofiloabsenBanda.ToString("F2"), fontDato));
            celSBDatoNeutrofiloabsolutoenBanda.BorderWidth = 0;
            celSBDatoNeutrofiloabsolutoenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoNeutrofiloabsolutoenBanda.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbsNeutrofiloenBanda = new PdfPCell(new Phrase("0", fontDato));
            celSBAbsNeutrofiloenBanda.BorderWidth = 0;
            celSBAbsNeutrofiloenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbsNeutrofiloenBanda.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBNeutrofiloabsolutoenBanda = new PdfPCell(new Phrase("0", fontDato));
            celSBNeutrofiloabsolutoenBanda.BorderWidth = 0;
            celSBNeutrofiloabsolutoenBanda.VerticalAlignment = Element.ALIGN_CENTER;
            celSBNeutrofiloabsolutoenBanda.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Eosinofilos
            //[0] - 130
            PdfPCell celSBEosinofilos = new PdfPCell(new Phrase("Eosinófilos", fonEiqueta));
            celSBEosinofilos.BorderWidth = 0;
            celSBEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBEosinofilos.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBEosinofilosVacio = new PdfPCell(new Phrase("", fonEiqueta));
            celSBEosinofilosVacio.BorderWidth = 0;
            celSBEosinofilosVacio.VerticalAlignment = Element.ALIGN_CENTER;
            celSBEosinofilosVacio.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            decimal Eosinofilos = Convert.ToDecimal(datosBH.Eos2);
            PdfPCell celSBDatoEosinofilos = new PdfPCell(new Phrase(Eosinofilos.ToString("F2"), fontDato));
            celSBDatoEosinofilos.BorderWidth = 0;
            celSBDatoEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoEosinofilos.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBEosinofilosFle = new PdfPCell();
            switch (datosBH.fr_eos)
            {
                case -1:
                    celSBEosinofilosFle.AddElement(fAbajo);
                    break;
                case 1:
                    celSBEosinofilosFle.AddElement(fArriba);
                    break;
            }
            celSBEosinofilosFle.BorderWidth = 0;
            celSBEosinofilosFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBEosinofilosFle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal Eofinofiloabs = Convert.ToDecimal(datosBH.Eos);
            PdfPCell celSBDatoEosinofiloabsoluto = new PdfPCell(new Phrase(Eofinofiloabs.ToString("F2"), fontDato));
            celSBDatoEosinofiloabsoluto.BorderWidth = 0;
            celSBDatoEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbsEosinofilo = new PdfPCell(new Phrase("0 - 3", fontDato));
            celSBAbsEosinofilo.BorderWidth = 0;
            celSBAbsEosinofilo.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbsEosinofilo.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBEosinofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.7", fontDato));
            celSBEosinofiloabsoluto.BorderWidth = 0;
            celSBEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //-------------------------------------------------------------------------------------------------------- Basofilos
            //[0] - 130
            PdfPCell celSBBasofilos = new PdfPCell(new Phrase("Basófilos", fonEiqueta));
            celSBBasofilos.BorderWidth = 0;
            celSBBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBBasofilos.HorizontalAlignment = Element.ALIGN_LEFT;

            //[1] - 20
            PdfPCell celSBBasofilosVacios = new PdfPCell(new Phrase("", fonEiqueta));
            celSBBasofilosVacios.BorderWidth = 0;
            celSBBasofilosVacios.VerticalAlignment = Element.ALIGN_CENTER;
            celSBBasofilosVacios.HorizontalAlignment = Element.ALIGN_LEFT;

            //[2] - 120
            decimal Basofilos = Convert.ToDecimal(datosBH.Bas2);
            PdfPCell celSBDatoBasofilos = new PdfPCell(new Phrase(Basofilos.ToString("F2"), fontDato));
            celSBDatoBasofilos.BorderWidth = 0;
            celSBDatoBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoBasofilos.HorizontalAlignment = Element.ALIGN_CENTER;

            //[3] - 20
            PdfPCell celSBBasofilosFle = new PdfPCell();
            switch (datosBH.fr_bas)
            {
                case -1:
                    celSBBasofilosFle.AddElement(fAbajo);
                    break;
                case 1:
                    celSBBasofilosFle.AddElement(fArriba);
                    break;
            }
            celSBBasofilosFle.BorderWidth = 0;
            celSBBasofilosFle.VerticalAlignment = Element.ALIGN_CENTER;
            celSBBasofilosFle.HorizontalAlignment = Element.ALIGN_LEFT;

            //[4] - 110
            decimal Basofiloabs = Convert.ToDecimal(datosBH.Bas);
            PdfPCell celSBDatoBasofiloabsoluto = new PdfPCell(new Phrase(Basofiloabs.ToString("F2"), fontDato));
            celSBDatoBasofiloabsoluto.BorderWidth = 0;
            celSBDatoBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBDatoBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            //[5] - 80
            PdfPCell celSBAbsBasofilo = new PdfPCell(new Phrase("0 - 1 ", fontDato));
            celSBAbsBasofilo.BorderWidth = 0;
            celSBAbsBasofilo.VerticalAlignment = Element.ALIGN_CENTER;
            celSBAbsBasofilo.HorizontalAlignment = Element.ALIGN_CENTER;

            //[6] - 80
            PdfPCell celSBBasofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.15", fontDato));
            celSBBasofiloabsoluto.BorderWidth = 0;
            celSBBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
            celSBBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

            tblSB.AddCell(celSBLeucocito);
            tblSB.AddCell(celSFlechas);
            tblSB.AddCell(celSBDatoLeuco);
            tblSB.AddCell(celVacio01);
            tblSB.AddCell(celSBRef);
            tblSB.AddCell(celSBAbs);
            tblSB.AddCell(celSBVacio);

            tblSB.AddCell(celSBcelda1);
            tblSB.AddCell(celSBcelda1Fle);
            tblSB.AddCell(celSBcelda2);
            tblSB.AddCell(celSBcelda1Fle);
            tblSB.AddCell(celSBCelda3);
            tblSB.AddCell(celSBCelda4);

            tblSB.AddCell(celSBcelda1b);
            tblSB.AddCell(celSBcelda1bFle);
            tblSB.AddCell(celSBcelda2b);
            tblSB.AddCell(celSBCelda3b);
            tblSB.AddCell(celSBCelda3bVacia);
            tblSB.AddCell(celSBCelda4b);
            tblSB.AddCell(celSBCelda5b);

            tblSB.AddCell(celSBLinfocito);
            tblSB.AddCell(celSBDatoLinfocitoVacio);
            tblSB.AddCell(celSBDatoLinfocito);
            tblSB.AddCell(celSBLinfocitoFle);
            tblSB.AddCell(celSBDatoLinfocitoabsoluto);
            tblSB.AddCell(celSBAbslinfocito);
            tblSB.AddCell(celSBLinfocitoabsoluto);

            tblSB.AddCell(celSBMonocito);
            tblSB.AddCell(celSBMonocitoVacio);
            tblSB.AddCell(celSBDatoMonocito);
            tblSB.AddCell(celSBMonocitoFle);
            tblSB.AddCell(celSBDatoMonocitoabsoluto);
            tblSB.AddCell(celSBAbsMonocito);
            tblSB.AddCell(celSBMonocitoabsoluto);

            tblSB.AddCell(celSBNeutrofilos);
            tblSB.AddCell(celSBNeutrofilosVacio);
            tblSB.AddCell(celSBDatoNeutrofilos);
            tblSB.AddCell(celSBNeutrofilosFle);
            tblSB.AddCell(celSBDatoNeutrofiloabsoluto);
            tblSB.AddCell(celSBAbsNeutrofilo);
            tblSB.AddCell(celSBNeutrofiloabsoluto);

            tblSB.AddCell(celSBNeutrofilosenBanda);
            tblSB.AddCell(celSBNeutrofilosVacioenBanda);
            tblSB.AddCell(celSBDatoNeutrofilosenBanda);
            tblSB.AddCell(celSBNeutrofilosFleenBanda);
            tblSB.AddCell(celSBDatoNeutrofiloabsolutoenBanda);
            tblSB.AddCell(celSBAbsNeutrofiloenBanda);
            tblSB.AddCell(celSBNeutrofiloabsolutoenBanda);


            tblSB.AddCell(celSBEosinofilos);
            tblSB.AddCell(celSBEosinofilosVacio);
            tblSB.AddCell(celSBDatoEosinofilos);
            tblSB.AddCell(celSBEosinofilosFle);
            tblSB.AddCell(celSBDatoEosinofiloabsoluto);
            tblSB.AddCell(celSBAbsEosinofilo);
            tblSB.AddCell(celSBEosinofiloabsoluto);

            tblSB.AddCell(celSBBasofilos);
            tblSB.AddCell(celSBBasofilosVacios);
            tblSB.AddCell(celSBDatoBasofilos);
            tblSB.AddCell(celSBBasofilosFle);
            tblSB.AddCell(celSBDatoBasofiloabsoluto);
            tblSB.AddCell(celSBAbsBasofilo);
            tblSB.AddCell(celSBBasofiloabsoluto);

            docRep.Add(tblSB);

            #endregion

            #region Titulo Serie Roja
            PdfPTable Serieroja = new PdfPTable(1);
            Serieroja.TotalWidth = 560f;
            Serieroja.LockedWidth = true;

            Serieroja.SetWidths(widthsTitulosGenerales);
            Serieroja.HorizontalAlignment = 0;
            Serieroja.SpacingAfter = 10f;

            PdfPCell cellTituloSerieroja = new PdfPCell(new Phrase("SERIE ROJA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloSerieroja.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
            cellTituloSerieroja.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloSerieroja.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloSerieroja.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Serieroja.AddCell(cellTituloSerieroja);

            docRep.Add(Serieroja);
            #endregion

            #region Datos serie roja
            PdfPTable DtsSR = new PdfPTable(5)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valSR = new float[5];
            valSR[0] = 150;
            valSR[1] = 50;
            valSR[2] = 100;
            valSR[3] = 130;
            valSR[4] = 130;
            DtsSR.SetWidths(valSR);
            DtsSR.HorizontalAlignment = 0;
            DtsSR.SpacingAfter = 20f;
            DtsSR.DefaultCell.Border = 0;

            //----------------------------------------------------------------------------------------Valores de referencia
            PdfPCell c1 = new PdfPCell(new Phrase("", fontDato)) { Colspan = 2 };
            c1.HorizontalAlignment = Element.ALIGN_CENTER;
            c1.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c1);

            PdfPCell c2 = new PdfPCell(new Phrase("Valores de referencia", fonEiqueta)) { Colspan = 3 };
            c2.HorizontalAlignment = Element.ALIGN_CENTER;
            c2.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c2);

            //----------------------------------------------------------------------------------------Hemogloblina
            string rangoHemoglobina = datos.sexo == "HOMBRE" ? "14.0 - 17.0" : "11.0 - 14.0";

            PdfPCell hemo = new PdfPCell(new Phrase("Hemoglobina", fonEiqueta));
            hemo.HorizontalAlignment = Element.ALIGN_LEFT;
            hemo.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hemo);

            PdfPCell fr_hemo = new PdfPCell();
            switch (datosBH.fr_hgb)
            {
                case -1:
                    fr_hemo.AddElement(fAbajo);
                    break;
                case 1:
                    fr_hemo.AddElement(fArriba);
                    break;
            }
            fr_hemo.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_hemo.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_hemo);

            decimal hemovalor = Convert.ToDecimal(datosBH.HGB);
            PdfPCell chemovalor = new PdfPCell(new Phrase(hemovalor.ToString("F2"), fontDato));
            chemovalor.HorizontalAlignment = Element.ALIGN_CENTER;
            chemovalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(chemovalor);

            PdfPCell hemoUnidad = new PdfPCell(new Phrase("g/dL", fontDato));
            hemoUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
            hemoUnidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hemoUnidad);

            PdfPCell hemoreferencia = new PdfPCell(new Phrase(rangoHemoglobina, fontDato));
            hemoreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            hemoreferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hemoreferencia);

            //----------------------------------------------------------------------------------------Eritrocitos
            PdfPCell eritro = new PdfPCell(new Phrase("Eritrocitos", fonEiqueta));
            eritro.HorizontalAlignment = Element.ALIGN_LEFT;
            eritro.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(eritro);

            PdfPCell fr_eri = new PdfPCell();
            switch (datosBH.fr_rbc)
            {
                case -1:
                    fr_eri.AddElement(fAbajo);
                    break;
                case 1:
                    fr_eri.AddElement(fArriba);
                    break;
            }
            fr_eri.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_eri.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_eri);

            decimal eritrovalor = Convert.ToDecimal(datosBH.RBC);
            PdfPCell cheritrovalor = new PdfPCell(new Phrase(eritrovalor.ToString("F2"), fontDato));
            cheritrovalor.HorizontalAlignment = Element.ALIGN_CENTER;
            cheritrovalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(cheritrovalor);

            PdfPCell eriUnidad = new PdfPCell(new Phrase("x 10^6 / uL", fontDato));
            eriUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
            eriUnidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(eriUnidad);

            PdfPCell erireferencia = new PdfPCell(new Phrase("4.00 - 5.50", fontDato));
            erireferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            erireferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(erireferencia);

            //----------------------------------------------------------------------------------------HTC
            PdfPCell htc = new PdfPCell(new Phrase("HTC", fonEiqueta));
            htc.HorizontalAlignment = Element.ALIGN_LEFT;
            htc.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(htc);

            PdfPCell fr_htc = new PdfPCell();
            switch (datosBH.fr_htc)
            {
                case -1:
                    fr_htc.AddElement(fAbajo);
                    break;
                case 1:
                    fr_htc.AddElement(fArriba);
                    break;
            }
            fr_htc.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_htc.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_htc);

            decimal htcovalor = Convert.ToDecimal(datosBH.HTC);
            PdfPCell chtcvalor = new PdfPCell(new Phrase(htcovalor.ToString("F2"), fontDato));
            chtcvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            chtcvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(chtcvalor);

            PdfPCell htcUnidad = new PdfPCell(new Phrase("%", fontDato));
            htcUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
            htcUnidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(htcUnidad);

            PdfPCell htcreferencia = new PdfPCell(new Phrase("40 - 50", fontDato));
            htcreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            htcreferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(htcreferencia);

            //----------------------------------------------------------------------------------------VGM
            PdfPCell vgm = new PdfPCell(new Phrase("VGM", fonEiqueta));
            vgm.HorizontalAlignment = Element.ALIGN_LEFT;
            vgm.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vgm);

            PdfPCell fr_mcv = new PdfPCell();
            switch (datosBH.fr_mcv)
            {
                case -1:
                    fr_mcv.AddElement(fAbajo);
                    break;
                case 1:
                    fr_mcv.AddElement(fArriba);
                    break;
            }
            fr_mcv.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_mcv.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_mcv);

            decimal vgm_valor = Convert.ToDecimal(datosBH.MCv);
            PdfPCell c_vgmvalor = new PdfPCell(new Phrase(vgm_valor.ToString("F2"), fontDato));
            c_vgmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_vgmvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_vgmvalor);

            PdfPCell vgm_Unidad = new PdfPCell(new Phrase("fL", fontDato));
            vgm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            vgm_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vgm_Unidad);

            PdfPCell vgm_creferencia = new PdfPCell(new Phrase("80.0 - 100.0", fontDato));
            vgm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            vgm_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vgm_creferencia);

            //----------------------------------------------------------------------------------------HCM
            PdfPCell hcm = new PdfPCell(new Phrase("HCM", fonEiqueta));
            hcm.HorizontalAlignment = Element.ALIGN_LEFT;
            hcm.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hcm);

            PdfPCell fr_mch = new PdfPCell();
            switch (datosBH.fr_mch)
            {
                case -1:
                    fr_mch.AddElement(fAbajo);
                    break;
                case 1:
                    fr_mch.AddElement(fArriba);
                    break;
            }
            fr_mch.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_mch.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_mch);

            decimal hcm_valor = Convert.ToDecimal(datosBH.MCH);
            PdfPCell c_hcmvalor = new PdfPCell(new Phrase(hcm_valor.ToString("F2"), fontDato));
            c_hcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_hcmvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_hcmvalor);

            PdfPCell hcm_Unidad = new PdfPCell(new Phrase("pg/fL", fontDato));
            hcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            hcm_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hcm_Unidad);

            PdfPCell hcm_creferencia = new PdfPCell(new Phrase("27.0 - 31.0", fontDato));
            hcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            hcm_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(hcm_creferencia);

            //----------------------------------------------------------------------------------------CHCM
            PdfPCell chcm = new PdfPCell(new Phrase("CHCM", fonEiqueta));
            chcm.HorizontalAlignment = Element.ALIGN_LEFT;
            chcm.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(chcm);

            PdfPCell fr_mchc = new PdfPCell();
            switch (datosBH.fr_mchc)
            {
                case -1:
                    fr_mchc.AddElement(fAbajo);
                    break;
                case 1:
                    fr_mchc.AddElement(fArriba);
                    break;
            }
            fr_mchc.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_mchc.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_mchc);

            decimal chcm_valor = Convert.ToDecimal(datosBH.MCHC);
            PdfPCell c_chcmvalor = new PdfPCell(new Phrase(chcm_valor.ToString("F2"), fontDato));
            c_chcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_chcmvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_chcmvalor);

            PdfPCell chcm_Unidad = new PdfPCell(new Phrase("g/dL", fontDato));
            chcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            chcm_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(chcm_Unidad);

            PdfPCell chcm_creferencia = new PdfPCell(new Phrase("32.0 - 36.0", fontDato));
            chcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            chcm_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(chcm_creferencia);

            //----------------------------------------------------------------------------------------Plaquetas
            PdfPCell plaquetas = new PdfPCell(new Phrase("Plaquetas", fonEiqueta));
            plaquetas.HorizontalAlignment = Element.ALIGN_LEFT;
            plaquetas.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(plaquetas);

            PdfPCell fr_plt = new PdfPCell();
            switch (datosBH.fr_plt)
            {
                case -1:
                    fr_plt.AddElement(fAbajo);
                    break;
                case 1:
                    fr_plt.AddElement(fArriba);
                    break;
            }
            fr_plt.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_plt.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_plt);

            decimal plaquetas_valor = Convert.ToDecimal(datosBH.PLT);
            PdfPCell c_plaquetasvalor = new PdfPCell(new Phrase(plaquetas_valor.ToString("F2"), fontDato));
            c_plaquetasvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_plaquetasvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_plaquetasvalor);

            PdfPCell plaquetas_Unidad = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
            plaquetas_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            plaquetas_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(plaquetas_Unidad);

            PdfPCell plaquetas_creferencia = new PdfPCell(new Phrase("150 - 450", fontDato));
            plaquetas_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            plaquetas_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(plaquetas_creferencia);

            //----------------------------------------------------------------------------------------PCT
            PdfPCell pct = new PdfPCell(new Phrase("PCT", fonEiqueta));
            pct.HorizontalAlignment = Element.ALIGN_LEFT;
            pct.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pct);

            PdfPCell fr_pct = new PdfPCell();
            switch (datosBH.fr_pct)
            {
                case -1:
                    fr_pct.AddElement(fAbajo);
                    break;
                case 1:
                    fr_pct.AddElement(fArriba);
                    break;
            }
            fr_pct.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_pct.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_pct);

            decimal pct_valor = Convert.ToDecimal(datosBH.PCT);
            PdfPCell c_pctvalor = new PdfPCell(new Phrase(pct_valor.ToString("F3"), fontDato));
            c_pctvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_pctvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_pctvalor);

            PdfPCell pct_Unidad = new PdfPCell(new Phrase("%", fontDato));
            pct_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            pct_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pct_Unidad);

            PdfPCell pct_creferencia = new PdfPCell(new Phrase("0.108 - 0.282", fontDato));
            pct_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            pct_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pct_creferencia);

            //----------------------------------------------------------------------------------------RDWCV
            PdfPCell rdwcv = new PdfPCell(new Phrase("RDWCV", fonEiqueta));
            rdwcv.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwcv.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwcv);

            PdfPCell fr_rdwcv = new PdfPCell();
            switch (datosBH.fr_rdwcv)
            {
                case -1:
                    fr_rdwcv.AddElement(fAbajo);
                    break;
                case 1:
                    fr_rdwcv.AddElement(fArriba);
                    break;
            }
            fr_rdwcv.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_rdwcv.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_rdwcv);

            decimal rdwcv_valor = Convert.ToDecimal(datosBH.RDWCV);
            PdfPCell c_rdwcvvalor = new PdfPCell(new Phrase(rdwcv_valor.ToString("F2"), fontDato));
            c_rdwcvvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_rdwcvvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_rdwcvvalor);

            PdfPCell rdwcv_Unidad = new PdfPCell(new Phrase("%", fontDato));
            rdwcv_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwcv_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwcv_Unidad);

            PdfPCell rdwcv_creferencia = new PdfPCell(new Phrase("11.5 - 14.5", fontDato));
            rdwcv_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwcv_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwcv_creferencia);

            //----------------------------------------------------------------------------------------RDWSD
            PdfPCell rdwsd = new PdfPCell(new Phrase("RDWSD", fonEiqueta));
            rdwsd.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwsd.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwsd);

            PdfPCell fr_rdwsd = new PdfPCell();
            switch (datosBH.fr_rdwsd)
            {
                case -1:
                    fr_rdwsd.AddElement(fAbajo);
                    break;
                case 1:
                    fr_rdwsd.AddElement(fArriba);
                    break;
            }
            fr_rdwsd.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_rdwsd.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_rdwsd);

            decimal rdwsd_valor = Convert.ToDecimal(datosBH.RDWSD);
            PdfPCell c_rdwsdvalor = new PdfPCell(new Phrase(rdwsd_valor.ToString("F2"), fontDato));
            c_rdwsdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_rdwsdvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_rdwsdvalor);

            PdfPCell rdwsd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
            rdwsd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwsd_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwsd_Unidad);

            PdfPCell rdwsd_creferencia = new PdfPCell(new Phrase("35.5 - 56.0", fontDato));
            rdwsd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            rdwsd_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(rdwsd_creferencia);

            //----------------------------------------------------------------------------------------VPM
            PdfPCell vpmd = new PdfPCell(new Phrase("VPM", fonEiqueta));
            vpmd.HorizontalAlignment = Element.ALIGN_LEFT;
            vpmd.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vpmd);

            PdfPCell fr_mpv = new PdfPCell();
            switch (datosBH.fr_mpv)
            {
                case -1:
                    fr_mpv.AddElement(fAbajo);
                    break;
                case 1:
                    fr_mpv.AddElement(fArriba);
                    break;
            }
            fr_mpv.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_mpv.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_mpv);

            decimal vpmd_valor = Convert.ToDecimal(datosBH.MPV);
            PdfPCell c_vpmdvalor = new PdfPCell(new Phrase(vpmd_valor.ToString("F2"), fontDato));
            c_vpmdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_vpmdvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_vpmdvalor);

            PdfPCell vpmd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
            vpmd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            vpmd_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vpmd_Unidad);

            PdfPCell vpmd_creferencia = new PdfPCell(new Phrase("7.0 - 11.0", fontDato));
            vpmd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            vpmd_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(vpmd_creferencia);

            //----------------------------------------------------------------------------------------PDW
            PdfPCell pdw = new PdfPCell(new Phrase("PDW", fonEiqueta));
            pdw.HorizontalAlignment = Element.ALIGN_LEFT;
            pdw.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pdw);

            PdfPCell fr_pdw = new PdfPCell();
            switch (datosBH.fr_pdw)
            {
                case -1:
                    fr_pdw.AddElement(fAbajo);
                    break;
                case 1:
                    fr_pdw.AddElement(fArriba);
                    break;
            }
            fr_pdw.HorizontalAlignment = Element.ALIGN_RIGHT;
            fr_pdw.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(fr_pdw);

            decimal pdw_valor = Convert.ToDecimal(datosBH.PDW);
            PdfPCell c_pdwvalor = new PdfPCell(new Phrase(pdw_valor.ToString("F2"), fontDato));
            c_pdwvalor.HorizontalAlignment = Element.ALIGN_CENTER;
            c_pdwvalor.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(c_pdwvalor);

            PdfPCell pdw_Unidad = new PdfPCell(new Phrase("", fontDato));
            pdw_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
            pdw_Unidad.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pdw_Unidad);

            PdfPCell pdw_creferencia = new PdfPCell(new Phrase("15.0 - 17.0", fontDato));
            pdw_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
            pdw_creferencia.Border = PdfPCell.NO_BORDER;
            DtsSR.AddCell(pdw_creferencia);

            docRep.Add(DtsSR);
            #endregion

            Paragraph observaciones_bh = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
            observaciones_bh.Add("Metodologia: ");
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            observaciones_bh.Add(Chunk.TABBING);
            observaciones_bh.Add("Impedancia eléctrica y colorimetría por equipo Mindray BC-30s.");
            observaciones_bh.Add(Chunk.NEWLINE);
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
            observaciones_bh.Add("Observaciones: ");
            observaciones_bh.Add(Chunk.TABBING);
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            observaciones_bh.Add(datosBH.Observacion);
            observaciones_bh.Add(Chunk.NEWLINE);
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
            observaciones_bh.Add("Espécimen: ");
            observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            observaciones_bh.Add(Chunk.TABBING);
            observaciones_bh.Add("Sangre total");

            docRep.Add(observaciones_bh);

            Paragraph valorfueraderango = new Paragraph()
            {
                Alignment = Element.ALIGN_RIGHT
            };
            valorfueraderango.Font = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            valorfueraderango.Add("* = valor fuera de rango");

            docRep.Add(valorfueraderango);

            docRep.Close();
            byte[] bytesStream = msRep.ToArray();
            msRep = new MemoryStream();
            msRep.Write(bytesStream, 0, bytesStream.Length);
            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        public IActionResult reporteQS(int idHistorico)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosQS = repo.Getdosparam1<QSModel>("sp_medicos_qs", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datosQS.FOLIO.ToString();
            string elRealizo = datosQS.realizo.ToString();
            string elCedRea = datosQS.ced_rea.ToString();
            string elSuperviso = datosQS.superviso.ToString();
            string elCedSup = datosQS.ced_sup.ToString();
            string elTitulo = "Qúimica Sanguínea";

            pwRep.PageEvent = HeaderFooterEGO.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

            docRep.Open();

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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/07", fonEiqueta));
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

            docRep.Add(tblEmiRevCpd);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);
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
            tblDatosEvaluado.SpacingAfter = 10f;
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
            PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
            celTitCurp.BorderWidth = 0;
            celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp, fontDato)); ;
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

            docRep.Add(tblDatosEvaluado);

            #endregion

            #region Titulo Datos Estudio Quimico
            PdfPTable TitResultadoQS = new PdfPTable(1);
            TitResultadoQS.TotalWidth = 560f;
            TitResultadoQS.LockedWidth = true;

            TitResultadoQS.SetWidths(widthsTitulosGenerales);
            TitResultadoQS.HorizontalAlignment = 0;
            TitResultadoQS.SpacingBefore = 20f;
            TitResultadoQS.SpacingAfter = 10f;

            PdfPCell cellTituloQS = new PdfPCell(new Phrase("PRUEBA                          CONCENTRACION        UNIDAD      RESULTADO          RANGO REFERENCIA          UNIDAD", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloQS.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
            cellTituloQS.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloQS.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloQS.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            TitResultadoQS.AddCell(cellTituloQS);

            docRep.Add(TitResultadoQS);
            #endregion

            #region DatosQuiSan
            PdfPTable DtsQuiSam = new PdfPTable(6)
            {
                TotalWidth = 560,
                LockedWidth = true
            };

            float[] valQS = new float[6];
            valQS[0] = 110;
            valQS[1] = 100;
            valQS[2] = 65;
            valQS[3] = 70;
            valQS[4] = 150;
            valQS[5] = 65;
            DtsQuiSam.SetWidths(valQS);
            DtsQuiSam.HorizontalAlignment = 0;
            DtsQuiSam.SpacingAfter = 20f;
            DtsQuiSam.DefaultCell.Border = 0;

            //------------------------------------------------------------------------------------Glucosa
            PdfPCell cTitPruebaGlucosa = new PdfPCell(new Phrase("Glucosa", fonEiqueta));
            cTitPruebaGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaGlucosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaGlucosa);

            decimal con_Glu = Convert.ToDecimal(datosQS.Glucosa);
            PdfPCell cTitCncentracionGlucosa = new PdfPCell(new Phrase(con_Glu.ToString("F2"), fontDato));
            cTitCncentracionGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionGlucosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionGlucosa);

            PdfPCell cTitUnidadGlucosa = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadGlucosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadGlucosa);

            PdfPCell cTitResultadoGluscosa = new PdfPCell(new Phrase(datosQS.resGlu, fontDato));
            cTitResultadoGluscosa.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoGluscosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoGluscosa);

            PdfPCell cTitRangosGlucosa = new PdfPCell(new Phrase("74 - 106", fontDato));
            cTitRangosGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosGlucosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosGlucosa);

            PdfPCell cTitUnidad2Glucosa = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Glucosa.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Glucosa.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Glucosa);

            //------------------------------------------------------------------------------------Acido urico
            PdfPCell cTitPruebaAcido = new PdfPCell(new Phrase("Ácido úrico", fonEiqueta));
            cTitPruebaAcido.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaAcido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaAcido);

            decimal con_Aci = Convert.ToDecimal(datosQS.Acido);
            PdfPCell cTitCncentracionAcido = new PdfPCell(new Phrase(con_Aci.ToString("F2"), fontDato));
            cTitCncentracionAcido.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionAcido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionAcido);

            PdfPCell cTitUnidadAcido = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadAcido.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadAcido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadAcido);

            PdfPCell cTitResultadoAcido = new PdfPCell(new Phrase(datosQS.resAci, fontDato));
            cTitResultadoAcido.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoAcido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoAcido);

            PdfPCell cTitRangosAcido = new PdfPCell(new Phrase(datos.sexo == "HOMBRE" ? "3.5 - 7.2" : "2.6 - 6.0", fontDato));
            cTitRangosAcido.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosAcido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosAcido);

            PdfPCell cTitUnidad2Acido = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Acido.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Acido.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Acido);

            //------------------------------------------------------------------------------------Colesterol
            PdfPCell cTitPruebaColesterol = new PdfPCell(new Phrase("Colesterol", fonEiqueta));
            cTitPruebaColesterol.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaColesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaColesterol);

            decimal con_Col = Convert.ToDecimal(datosQS.Colesterol);
            PdfPCell cTitCncentracionColesterol = new PdfPCell(new Phrase(con_Col.ToString("F2"), fontDato));
            cTitCncentracionColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionColesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionColesterol);

            PdfPCell cTitUnidadColesterol = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadColesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadColesterol);

            PdfPCell cTitResultadoColesterol = new PdfPCell(new Phrase(datosQS.resCol, fontDato));
            cTitResultadoColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoColesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoColesterol);

            PdfPCell cTitRangosColesterol = new PdfPCell(new Phrase("menor o igual a 200", fontDato));
            cTitRangosColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosColesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosColesterol);

            PdfPCell cTitUnidad2Colesterol = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Colesterol.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Colesterol.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Colesterol);

            //------------------------------------------------------------------------------------Trigliceridos
            PdfPCell cTitPruebaTrigliceridos = new PdfPCell(new Phrase("Trigliceridos", fonEiqueta));
            cTitPruebaTrigliceridos.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaTrigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaTrigliceridos);

            decimal con_Tri = Convert.ToDecimal(datosQS.Trigliceridos);
            PdfPCell cTitCncentracionTrigliceridos = new PdfPCell(new Phrase(con_Tri.ToString("F2"), fontDato));
            cTitCncentracionTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionTrigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionTrigliceridos);

            PdfPCell cTitUnidadTrigliceridos = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadTrigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadTrigliceridos);

            PdfPCell cTitResultadoTrigliceridos = new PdfPCell(new Phrase(datosQS.resTri, fontDato));
            cTitResultadoTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoTrigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoTrigliceridos);

            PdfPCell cTitRangosTrigliceridos = new PdfPCell(new Phrase("30 - 150", fontDato));
            cTitRangosTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosTrigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosTrigliceridos);

            PdfPCell cTitUnidad2Trigliceridos = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Trigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Trigliceridos.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Trigliceridos);

            //------------------------------------------------------------------------------------Urea
            PdfPCell cTitPruebaUrea = new PdfPCell(new Phrase("Urea", fonEiqueta));
            cTitPruebaUrea.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaUrea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaUrea);

            decimal con_Ure = Convert.ToDecimal(datosQS.Urea);
            PdfPCell cTitCncentracionUrea = new PdfPCell(new Phrase(con_Ure.ToString("F2"), fontDato));
            cTitCncentracionUrea.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionUrea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionUrea);

            PdfPCell cTitUnidadUrea = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadUrea.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadUrea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadUrea);

            PdfPCell cTitResultadoUrea = new PdfPCell(new Phrase(datosQS.resUre, fontDato));
            cTitResultadoUrea.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoUrea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoUrea);

            PdfPCell cTitRangosUrea = new PdfPCell(new Phrase("15.0 - 38.5", fontDato));
            cTitRangosUrea.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosUrea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosUrea);

            PdfPCell cTitUnidad2Urea = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Urea.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Urea.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Urea);

            //------------------------------------------------------------------------------------Creatinina
            PdfPCell cTitPruebaCreatinina = new PdfPCell(new Phrase("Creatinina", fonEiqueta));
            cTitPruebaCreatinina.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaCreatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaCreatinina);

            decimal con_Cre = Convert.ToDecimal(datosQS.Creatinina);
            PdfPCell cTitCncentracionCreatinina = new PdfPCell(new Phrase(con_Cre.ToString("F2"), fontDato));
            cTitCncentracionCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionCreatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionCreatinina);

            PdfPCell cTitUnidadCreatinina = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadCreatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadCreatinina);

            PdfPCell cTitResultadoCreatinina = new PdfPCell(new Phrase(datosQS.resCre, fontDato));
            cTitResultadoCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoCreatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoCreatinina);

            PdfPCell cTitRangosCreatinina = new PdfPCell(new Phrase(datos.sexo == "HOMBRE" ? "0.8 - 1.3" : "0.55 - 1.0", fontDato));
            cTitRangosCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosCreatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosCreatinina);

            PdfPCell cTitUnidad2Creatinina = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2Creatinina.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2Creatinina.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2Creatinina);

            //------------------------------------------------------------------------------------Colesterol Alta
            PdfPCell cTitPruebaColesterolHDL = new PdfPCell(new Phrase("Colesterol Alta (HDL)", fonEiqueta));
            cTitPruebaColesterolHDL.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaColesterolHDL);

            decimal con_HDL = Convert.ToDecimal(datosQS.colesterolAlta);
            PdfPCell cTitCncentracionColesterolHDL = new PdfPCell(new Phrase(con_HDL.ToString("F2"), fontDato));
            cTitCncentracionColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionColesterolHDL);

            PdfPCell cTitUnidadColesterolHDL = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadColesterolHDL);

            PdfPCell cTitResultadoColesterolHDL = new PdfPCell(new Phrase(datosQS.resColAlt, fontDato));
            cTitResultadoColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoColesterolHDL);

            PdfPCell cTitRangosColesterolHDL = new PdfPCell(new Phrase("40 - 60", fontDato));
            cTitRangosColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosColesterolHDL);

            PdfPCell cTitUnidad2ColesterolHDL = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2ColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2ColesterolHDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2ColesterolHDL);

            //------------------------------------------------------------------------------------Colesterol Baja
            PdfPCell cTitPruebaColesterolLDL = new PdfPCell(new Phrase("Colesterol Baja (LDL)", fonEiqueta));
            cTitPruebaColesterolLDL.HorizontalAlignment = Element.ALIGN_LEFT;
            cTitPruebaColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitPruebaColesterolLDL);

            decimal con_LDL = Convert.ToDecimal(datosQS.colesterolBaja);
            PdfPCell cTitCncentracionColesterolLDL = new PdfPCell(new Phrase(con_LDL.ToString("F2"), fontDato));
            cTitCncentracionColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitCncentracionColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitCncentracionColesterolLDL);

            PdfPCell cTitUnidadColesterolLDL = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidadColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidadColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidadColesterolLDL);

            PdfPCell cTitResultadoColesterolLDL = new PdfPCell(new Phrase(datosQS.resColBaj, fontDato));
            cTitResultadoColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitResultadoColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitResultadoColesterolLDL);

            PdfPCell cTitRangosColesterolLDL = new PdfPCell(new Phrase("menor a 159", fontDato));
            cTitRangosColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitRangosColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitRangosColesterolLDL);

            PdfPCell cTitUnidad2ColesterolLDL = new PdfPCell(new Phrase("mg/dL", fontDato));
            cTitUnidad2ColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
            cTitUnidad2ColesterolLDL.Border = PdfPCell.NO_BORDER;
            DtsQuiSam.AddCell(cTitUnidad2ColesterolLDL);

            docRep.Add(DtsQuiSam);

            #endregion

            #region final
            Paragraph metodo = new Paragraph();
            metodo.Add(new Phrase("Metodología:", fonEiqueta));
            metodo.Add(Chunk.TABBING);
            metodo.Add(new Phrase(datosQS.metodologia, fontDato));
            metodo.Add(Chunk.NEWLINE);

            metodo.Add(new Phrase("Espécimen:", fonEiqueta));
            metodo.Add(Chunk.TABBING);
            metodo.Add(new Phrase("Suero", fontDato));
            metodo.Add(Chunk.NEWLINE);

            metodo.Add(new Phrase("Observaciones:", fonEiqueta));
            metodo.Add(Chunk.TABBING);
            metodo.Add(new Phrase(datosQS.Observacion, fontDato));
            metodo.Add(Chunk.NEWLINE);

            docRep.Add(metodo);
            #endregion

            docRep.Close();
            byte[] bytesStream = msRep.ToArray();
            msRep = new MemoryStream();
            msRep.Write(bytesStream, 0, bytesStream.Length);
            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        public IActionResult reporteTX(int idHistorico)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosTX = repo.Getdosparam1<ToxModel>("sp_medicos_tox", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datosTX.folio;
            string elRealizo = datosTX.realizo;
            string elCedRea = datosTX.ced_realizo;
            string elSuperviso = datosTX.supervisor;
            string elCedSup = datosTX.ced_superviso;
            string elTitulo = "FORMATOS PARA EMISION DE RESULTADOS DE LA EVALUACION TOXICOLOGICA";

            //pwRep.PageEvent = HeaderFooterEGO.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);
            pwRep.PageEvent = HeaderFooterTX.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

            docRep.Open();

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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/04", fonEiqueta));
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

            docRep.Add(tblEmiRevCpd);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos personales", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);
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
            tblDatosEvaluado.SpacingAfter = 30f;
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
            PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
            celTitCurp.BorderWidth = 0;
            celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
            celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos.curp, fontDato)); ;
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

            PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos.dependencia, fontDato));
            celDatoDependencia.BorderWidth = 0;
            celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celLugarEva = new PdfPCell(new Phrase("Lugar evaluacion", fonEiqueta));
            celLugarEva.BorderWidth = 0;
            celLugarEva.VerticalAlignment = Element.ALIGN_CENTER;
            celLugarEva.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celLugarEvaDato = new PdfPCell(new Phrase("CECCC", fontDato));
            celLugarEvaDato.BorderWidth = 0;
            celLugarEvaDato.VerticalAlignment = Element.ALIGN_CENTER;
            celLugarEvaDato.HorizontalAlignment = Element.ALIGN_LEFT;

            //-------------------------------------------------------------------------------------------------------- 6a linea
            PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
            celTitPuesto.BorderWidth = 0;
            celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
            celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos.puesto, fontDato));
            celDatoPuesto.BorderWidth = 0;
            celDatoPuesto.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTitMuestra = new PdfPCell(new Phrase("Tipo de muestra", fonEiqueta));
            celTitMuestra.BorderWidth = 0;
            celTitMuestra.VerticalAlignment = Element.ALIGN_CENTER;
            celTitMuestra.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoMuestra = new PdfPCell(new Phrase(datosTX.muestra, fontDato));
            celDatoMuestra.BorderWidth = 0;
            celDatoMuestra.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoMuestra.HorizontalAlignment = Element.ALIGN_LEFT;

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
            tblDatosEvaluado.AddCell(celLugarEva);
            tblDatosEvaluado.AddCell(celLugarEvaDato);

            tblDatosEvaluado.AddCell(celTitPuesto);
            tblDatosEvaluado.AddCell(celDatoPuesto);
            tblDatosEvaluado.AddCell(celTitMuestra);
            tblDatosEvaluado.AddCell(celDatoMuestra);

            docRep.Add(tblDatosEvaluado);

            #endregion

            #region Titulo Resultados de examen
            PdfPTable TituloResultados = new PdfPTable(1);
            TituloResultados.TotalWidth = 560f;
            TituloResultados.LockedWidth = true;

            TituloResultados.SetWidths(widthsTitulosGenerales);
            TituloResultados.HorizontalAlignment = 0;
            TituloResultados.SpacingBefore = 20f;
            TituloResultados.SpacingAfter = 10f;

            PdfPCell cellTituloTituloRes = new PdfPCell(new Phrase("Registro de resultado de examen toxicológico", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloRes.HorizontalAlignment = 1;
            cellTituloTituloRes.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloRes.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloRes.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            TituloResultados.AddCell(cellTituloTituloRes);

            docRep.Add(TituloResultados);
            #endregion

            #region Datosresultados
            Paragraph fechaHoraProcesamiento = new Paragraph();
            fechaHoraProcesamiento.Alignment = Element.ALIGN_LEFT;
            fechaHoraProcesamiento.Add(new Phrase("Fecha de procesamiento: ", fonEiqueta));
            fechaHoraProcesamiento.Add(Chunk.TABBING);
            fechaHoraProcesamiento.Add(new Phrase(datosTX.fprocesamiento, fontDato));
            fechaHoraProcesamiento.Add(Chunk.TABBING); fechaHoraProcesamiento.Add(Chunk.TABBING); fechaHoraProcesamiento.Add(Chunk.TABBING);
            fechaHoraProcesamiento.Add(new Phrase("Hora de procesamiento: ", fonEiqueta));
            fechaHoraProcesamiento.Add(Chunk.TABBING);
            fechaHoraProcesamiento.Add(new Phrase(datosTX.hprocesamiento, fontDato));
            fechaHoraProcesamiento.Add(Chunk.NEWLINE); fechaHoraProcesamiento.Add(Chunk.NEWLINE);

            docRep.Add(fechaHoraProcesamiento);

            Paragraph resultado = new Paragraph();
            resultado.Alignment = Element.ALIGN_CENTER;
            resultado.Add(new Phrase("Resultado", fonEiqueta));
            resultado.Add(Chunk.TABBING); resultado.Add(Chunk.TABBING);
            resultado.Add(new Phrase(datosTX.resultado, fontDato));
            resultado.Add(Chunk.NEWLINE); resultado.Add(Chunk.NEWLINE);

            docRep.Add(resultado);

            PdfPTable dtsRes = new PdfPTable(2)
            {
                TotalWidth = 300,
                LockedWidth = true
            };

            float[] valTX = new float[2];
            valTX[0] = 150;
            valTX[1] = 150;
            dtsRes.SetWidths(valTX);
            dtsRes.HorizontalAlignment = Element.ALIGN_CENTER;
            dtsRes.SpacingAfter = 30f;
            dtsRes.DefaultCell.Border = 1;

            //--------------------------------------------------------------------Titulos
            PdfPCell cAnalito = new PdfPCell(new Phrase("ANALITO", fonEiqueta));
            cAnalito.HorizontalAlignment = Element.ALIGN_CENTER;
            cAnalito.Border = PdfPCell.NO_BORDER;
            cAnalito.BorderWidthTop = 0.75f;
            dtsRes.AddCell(cAnalito);

            PdfPCell cResultado = new PdfPCell(new Phrase("RESULTADO", fonEiqueta));
            cResultado.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultado.Border = PdfPCell.NO_BORDER;
            cResultado.BorderWidthTop = 0.75f;
            dtsRes.AddCell(cResultado);

            //--------------------------------------------------------------------Mariguana
            PdfPCell cMariguana = new PdfPCell(new Phrase("Marihuana", fonEiqueta));
            cMariguana.HorizontalAlignment = Element.ALIGN_LEFT;
            cMariguana.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cMariguana);

            PdfPCell cResultadoMarihuana = new PdfPCell(new Phrase(datosTX.mariguana, fontDato));
            cResultadoMarihuana.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoMarihuana.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoMarihuana);

            //--------------------------------------------------------------------Cocaína
            PdfPCell cCocaina = new PdfPCell(new Phrase("Cocaína", fonEiqueta));
            cCocaina.HorizontalAlignment = Element.ALIGN_LEFT;
            cCocaina.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cCocaina);

            PdfPCell cResultadoCocaina = new PdfPCell(new Phrase(datosTX.cocaina, fontDato));
            cResultadoCocaina.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoCocaina.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoCocaina);

            //--------------------------------------------------------------------Anfetamina
            PdfPCell cAntetamina = new PdfPCell(new Phrase("Anfetaminas", fonEiqueta));
            cAntetamina.HorizontalAlignment = Element.ALIGN_LEFT;
            cAntetamina.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cAntetamina);

            PdfPCell cResultadoAnfetamina = new PdfPCell(new Phrase(datosTX.anfetaminas, fontDato));
            cResultadoAnfetamina.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoAnfetamina.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoAnfetamina);

            //--------------------------------------------------------------------Benzodiacepinas
            PdfPCell cBenzo = new PdfPCell(new Phrase("Benzodiacepinas", fonEiqueta));
            cBenzo.HorizontalAlignment = Element.ALIGN_LEFT;
            cBenzo.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cBenzo);

            PdfPCell cResultadoBenzo = new PdfPCell(new Phrase(datosTX.benzodiacepinas, fontDato));
            cResultadoBenzo.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoBenzo.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoBenzo);

            //--------------------------------------------------------------------Barbitúricos
            PdfPCell cBarbituricos = new PdfPCell(new Phrase("Barbitúricos", fonEiqueta));
            cBarbituricos.HorizontalAlignment = Element.ALIGN_LEFT;
            cBarbituricos.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cBarbituricos);

            PdfPCell cResultadoBarbituricos = new PdfPCell(new Phrase(datosTX.barbituricos, fontDato));
            cResultadoBarbituricos.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoBarbituricos.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoBarbituricos);

            //--------------------------------------------------------------------Metanfetaminas
            PdfPCell cMetanfetaminas = new PdfPCell(new Phrase("Metanfetaminas", fonEiqueta));
            cMetanfetaminas.HorizontalAlignment = Element.ALIGN_LEFT;
            cMetanfetaminas.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cMetanfetaminas);

            PdfPCell cResultadoMetanfetaminas = new PdfPCell(new Phrase(datosTX.metanfetaminas, fontDato));
            cResultadoMetanfetaminas.HorizontalAlignment = Element.ALIGN_CENTER;
            cResultadoMetanfetaminas.Border = PdfPCell.NO_BORDER;
            dtsRes.AddCell(cResultadoMetanfetaminas);

            docRep.Add(dtsRes);

            #endregion

            #region metodologia y observaciones
            Paragraph metObs = new Paragraph();
            metObs.Add(Chunk.NEWLINE);
            metObs.Alignment = Element.ALIGN_LEFT;
            metObs.Add(new Phrase("Metodología utilizada", fonEiqueta));
            metObs.Add(Chunk.TABBING);
            metObs.Add(new Phrase(datosTX.metodo, fontDato));
            metObs.Add(Chunk.NEWLINE); metObs.Add(Chunk.NEWLINE);
            metObs.Add(new Phrase("Observaciones", fonEiqueta));
            metObs.Add(Chunk.NEWLINE); metObs.Add(Chunk.TABBING);
            metObs.Add(new Phrase(datosTX.observacion, fontDato));

            docRep.Add(metObs);
            #endregion

            docRep.Close();
            byte[] bytesStream = msRep.ToArray();
            msRep = new MemoryStream();
            msRep.Write(bytesStream, 0, bytesStream.Length);
            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        //Todas los reportes
        public IActionResult allEgos(int id, string fechaAll)
        {
            var _id = id;
            var _fechaAll = fechaAll;

            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = _fechaAll }).ToList();

            var _total = datos.Count();

            MemoryStream msRepAll = new MemoryStream();
            Document docRepAll = new Document(PageSize.LETTER, 30f, 20f, 20f, 40f);
            PdfWriter pwRepAll = PdfWriter.GetInstance(docRepAll, msRepAll);
            docRepAll.Open();

            for (int idd = 0; idd < _total; idd++)
            {
                int _elId = datos[idd].idhistorico;
                var datosEGO = repo.Getdosparam1<EgoModel>("sp_medicos_rep_orina", new { @idhistorico = _elId }).FirstOrDefault();

                var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

                #region encabezado
                //-------------------------------------------------------------------------------------------------------- 1a linea
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

                Chunk chkSub = new Chunk("Examen General de Orina", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
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

                docRepAll.Add(tblEncabezado);

                #endregion

                #region emision - revision - codigo
                Chunk chkemision = new Chunk("EMISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision = new Paragraph();
                paragraphemision.Alignment = Element.ALIGN_CENTER;
                paragraphemision.Add(chkemision);

                PdfPCell clEmision = new PdfPCell();
                clEmision.BorderWidth = 0;
                clEmision.AddElement(paragraphemision);

                Chunk chkrevision = new Chunk("REVISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision = new Paragraph();
                paragrarevision.Alignment = Element.ALIGN_CENTER;
                paragrarevision.Add(chkrevision);

                PdfPCell clrevision = new PdfPCell();
                clrevision.BorderWidth = 0;
                clrevision.AddElement(paragrarevision);

                Chunk chkcodigo = new Chunk("CODIGO", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo = new Paragraph();
                paragracodigo.Alignment = Element.ALIGN_CENTER;
                paragracodigo.Add(chkcodigo);

                PdfPCell clcodigo = new PdfPCell();
                clcodigo.BorderWidth = 0;
                clcodigo.AddElement(paragracodigo);

                Chunk chkemision_b = new Chunk(DateTime.Now.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision_b = new Paragraph();
                paragraphemision_b.Alignment = Element.ALIGN_CENTER;
                paragraphemision_b.Add(chkemision_b);

                PdfPCell clEmision_b = new PdfPCell();
                clEmision_b.BorderWidth = 0;
                clEmision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clEmision_b.UseAscender = true;
                clEmision_b.AddElement(paragraphemision_b);

                Chunk chkrevision_b = new Chunk("1.1", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision_b = new Paragraph();
                paragrarevision_b.Alignment = Element.ALIGN_CENTER;
                paragrarevision_b.Add(chkrevision_b);

                PdfPCell clrevision_b = new PdfPCell();
                clrevision_b.BorderWidth = 0;
                clrevision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clrevision_b.UseAscender = true;
                clrevision_b.AddElement(paragrarevision_b);

                Chunk chkcodigo_b = new Chunk("CECCC/DMT/07", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo_b = new Paragraph();
                paragracodigo_b.Alignment = Element.ALIGN_CENTER;
                paragracodigo_b.Add(chkcodigo_b);

                PdfPCell clcodigo_b = new PdfPCell();
                clcodigo_b.BorderWidth = 0;
                clcodigo_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clcodigo_b.UseAscender = true;
                clcodigo_b.AddElement(paragracodigo_b);

                PdfPTable tblemision = new PdfPTable(3);
                tblemision.WidthPercentage = 100;
                float[] widthsemision = new float[] { 20f, 60f, 20f };
                tblemision.SetWidths(widthsemision);

                tblemision.AddCell(clEmision);
                tblemision.AddCell(clrevision);
                tblemision.AddCell(clcodigo);

                tblemision.AddCell(clEmision_b);
                tblemision.AddCell(clrevision_b);
                tblemision.AddCell(clcodigo_b);

                docRepAll.Add(tblemision);
                #endregion

                #region Titulo Datos personales
                PdfPTable Datospersonales = new PdfPTable(1);
                Datospersonales.TotalWidth = 560f;
                Datospersonales.LockedWidth = true;

                Datospersonales.SetWidths(widthsTitulosGenerales);
                Datospersonales.HorizontalAlignment = 0;
                Datospersonales.SpacingBefore = 20f;
                Datospersonales.SpacingAfter = 10f;

                PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloTituloFamiliar.HorizontalAlignment = 1;
                cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                Datospersonales.AddCell(cellTituloTituloFamiliar);

                docRepAll.Add(Datospersonales);

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
                tblDatosEvaluado.SpacingAfter = 10f;
                //tblDatosEvaluado.SpacingBefore = 10f;
                tblDatosEvaluado.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------------------------------- 1a linea
                PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
                celTitnombre.BorderWidth = 0;
                celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
                celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos[idd].evaluado, fontDato));
                celDatoEvaluado.BorderWidth = 0;
                celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
                celTitCodigo.BorderWidth = 0;
                celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos[idd].codigoevaluado, fontDato));
                celDatoCodigo.BorderWidth = 0;
                celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 2a linea
                PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
                celTitSexo.BorderWidth = 0;
                celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos[idd].sexo, fontDato));
                celDatoSexo.BorderWidth = 0;
                celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
                celTitEvaluacion.BorderWidth = 0;
                celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos[idd].evaluacion, fontDato));
                celDatoEvaluacion.BorderWidth = 0;
                celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 3a linea
                PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
                celTitEdad.BorderWidth = 0;
                celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos[idd].edad.ToString(), fontDato)); ;
                celDatoEdad.BorderWidth = 0;
                celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celTitFolio.BorderWidth = 0;
                celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos[idd].folio, fontDato));
                celDatoFolio.BorderWidth = 0;
                celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 4a linea
                PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
                celTitCurp.BorderWidth = 0;
                celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos[idd].curp, fontDato)); ;
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

                PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos[idd].dependencia, fontDato)) { Colspan = 3 };
                celDatoDependencia.BorderWidth = 0;
                celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 6a linea
                PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
                celTitPuesto.BorderWidth = 0;
                celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
                celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos[idd].puesto, fontDato)) { Colspan = 3 };
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

                docRepAll.Add(tblDatosEvaluado);

                #endregion

                #region Datos Examen Fisico
                PdfPTable tblEgo = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };

                float[] valuesEgo = new float[3];
                valuesEgo[0] = 200;
                valuesEgo[1] = 200;
                valuesEgo[2] = 160;
                tblEgo.SetWidths(valuesEgo);
                tblEgo.HorizontalAlignment = 0;
                tblEgo.SpacingAfter = 10f;
                tblEgo.DefaultCell.Border = 0;

                //------------------------------------------------------------------------ Linea 1
                PdfPCell celTituloEgo = new PdfPCell(new Phrase("Examen físico", fonEiqueta)) { Colspan = 2 };
                celTituloEgo.BorderWidth = 0;
                celTituloEgo.BorderWidthBottom = 1;
                celTituloEgo.VerticalAlignment = Element.ALIGN_MIDDLE;
                celTituloEgo.UseAscender = true;
                celTituloEgo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celTituloReferencia = new PdfPCell(new Phrase("Referencia", fonEiqueta));
                celTituloReferencia.BorderWidth = 0;
                celTituloReferencia.BorderWidthBottom = 1;
                celTituloReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 2 (Aspecto)
                PdfPCell celTitAspecto = new PdfPCell(new Phrase("Aspecto", fonEiqueta));
                celTitAspecto.BorderWidth = 0;
                celTitAspecto.VerticalAlignment = Element.ALIGN_CENTER;
                celTitAspecto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoAspecto = new PdfPCell(new Phrase(datosEGO.Aspecto, fontDato));
                celDatoAspecto.BorderWidth = 0;
                celTitAspecto.VerticalAlignment = Element.ALIGN_CENTER;
                celTitAspecto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoAspectoVacio = new PdfPCell(new Phrase("", fontDato));
                celDatoAspectoVacio.BorderWidth = 0;
                celDatoAspectoVacio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoAspectoVacio.HorizontalAlignment = Element.ALIGN_LEFT;

                //------------------------------------------------------------------------ Linea 3 (Color)
                PdfPCell celTitColor = new PdfPCell(new Phrase("Color", fonEiqueta));
                celTitColor.BorderWidth = 0;
                celTitColor.VerticalAlignment = Element.ALIGN_CENTER;
                celTitColor.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoColor = new PdfPCell(new Phrase(datosEGO.Color, fontDato));
                celDatoColor.BorderWidth = 0;
                celDatoColor.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoColor.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoColorVacio = new PdfPCell(new Phrase("", fontDato));
                celDatoColorVacio.BorderWidth = 0;
                celDatoColorVacio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoColorVacio.HorizontalAlignment = Element.ALIGN_LEFT;

                //------------------------------------------------------------------------ Linea 4 (PH)
                PdfPCell celTitPH = new PdfPCell(new Phrase("PH", fonEiqueta));
                celTitPH.BorderWidth = 0;
                celTitPH.VerticalAlignment = Element.ALIGN_CENTER;
                celTitPH.HorizontalAlignment = Element.ALIGN_LEFT;

                //PdfPCell celDatoPh = new PdfPCell(new Phrase(datosEGO.PH.ToString("F2"), fontDato));

                decimal Double1 = Convert.ToDecimal(datosEGO.PH);
                PdfPCell celDatoPh = new PdfPCell(new Phrase(Double1.ToString("F2"), fontDato));
                celDatoPh.BorderWidth = 0;
                celDatoPh.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoPh.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoPHVacio = new PdfPCell(new Phrase("", fontDato));
                celDatoPHVacio.BorderWidth = 0;
                celDatoPHVacio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoPHVacio.HorizontalAlignment = Element.ALIGN_LEFT;

                //------------------------------------------------------------------------ Linea 5 (Densidad)
                PdfPCell celTitDensidad = new PdfPCell(new Phrase("Densidad", fonEiqueta));
                celTitDensidad.BorderWidth = 0;
                celTitDensidad.VerticalAlignment = Element.ALIGN_CENTER;
                celTitDensidad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoDensidad = new PdfPCell(new Phrase(datosEGO.Densidad, fontDato));
                celDatoDensidad.BorderWidth = 0;
                celDatoDensidad.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoDensidad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoDensidadVacio = new PdfPCell(new Phrase("", fontDato));
                celDatoDensidadVacio.BorderWidth = 0;
                celDatoDensidadVacio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoDensidadVacio.HorizontalAlignment = Element.ALIGN_LEFT;

                tblEgo.AddCell(celTituloEgo);
                tblEgo.AddCell(celTituloReferencia);

                tblEgo.AddCell(celTitAspecto);
                tblEgo.AddCell(celDatoAspecto);
                tblEgo.AddCell(celDatoAspectoVacio);

                tblEgo.AddCell(celTitColor);
                tblEgo.AddCell(celDatoColor);
                tblEgo.AddCell(celDatoColorVacio);

                tblEgo.AddCell(celTitPH);
                tblEgo.AddCell(celDatoPh);
                tblEgo.AddCell(celDatoPHVacio);

                tblEgo.AddCell(celTitDensidad);
                tblEgo.AddCell(celDatoDensidad);
                tblEgo.AddCell(celDatoDensidadVacio);

                docRepAll.Add(tblEgo);
                #endregion

                #region Examen químico
                PdfPTable tblQuimico = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };

                float[] valueQuimico = new float[3];
                valueQuimico[0] = 200;
                valueQuimico[1] = 200;
                valueQuimico[2] = 160;
                tblQuimico.SetWidths(valueQuimico);
                tblQuimico.SpacingAfter = 10f;
                tblQuimico.DefaultCell.Border = 0;

                //------------------------------------------------------------------------ Linea 1
                PdfPCell celTituloQui = new PdfPCell(new Phrase("Examen químico", fonEiqueta)) { Colspan = 2 };
                celTituloQui.BorderWidth = 0;
                celTituloQui.BorderWidthBottom = 1;
                celTituloQui.VerticalAlignment = Element.ALIGN_CENTER;
                celTituloQui.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celTituloQuimicaReferencia = new PdfPCell(new Phrase("", fonEiqueta));
                celTituloQuimicaReferencia.BorderWidth = 0;
                celTituloQuimicaReferencia.BorderWidthBottom = 1;
                celTituloQuimicaReferencia.VerticalAlignment = Element.ALIGN_CENTER;
                celTituloQuimicaReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 2 (Glucosa)
                PdfPCell celTitGlucosa = new PdfPCell(new Phrase("Glucosa", fonEiqueta));
                celTitGlucosa.BorderWidth = 0;
                celTitGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
                celTitGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoGlucosa = new PdfPCell(new Phrase(datosEGO.Glucosa, fontDato));
                celDatoGlucosa.BorderWidth = 0;
                celDatoGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaGlucosa = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaGlucosa.BorderWidth = 0;
                celReferenciaGlucosa.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 3 (Bilirrubina)
                PdfPCell celBilirrubina = new PdfPCell(new Phrase("Bilirrubina", fonEiqueta));
                celBilirrubina.BorderWidth = 0;
                celBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
                celBilirrubina.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoBilirrubina = new PdfPCell(new Phrase(datosEGO.Bilirrubina, fontDato));
                celDatoBilirrubina.BorderWidth = 0;
                celDatoBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoBilirrubina.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaBilirrubina = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaBilirrubina.BorderWidth = 0;
                celReferenciaBilirrubina.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaBilirrubina.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 4 (Cetona)
                PdfPCell celCetona = new PdfPCell(new Phrase("Cetona", fonEiqueta));
                celCetona.BorderWidth = 0;
                celCetona.VerticalAlignment = Element.ALIGN_CENTER;
                celCetona.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCetona = new PdfPCell(new Phrase(datosEGO.Cetona, fontDato));
                celDatoCetona.BorderWidth = 0;
                celDatoCetona.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCetona.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaCetona = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaCetona.BorderWidth = 0;
                celReferenciaCetona.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaCetona.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 5 (Proteína)
                PdfPCell celProteina = new PdfPCell(new Phrase("Proteína", fonEiqueta));
                celProteina.BorderWidth = 0;
                celProteina.VerticalAlignment = Element.ALIGN_CENTER;
                celProteina.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoProteina = new PdfPCell(new Phrase(datosEGO.Proteinas, fontDato));
                celDatoProteina.BorderWidth = 0;
                celDatoProteina.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoProteina.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaProteina = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaProteina.BorderWidth = 0;
                celReferenciaProteina.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaProteina.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 6 (Uribilinogeno)
                PdfPCell celUrobilinogeno = new PdfPCell(new Phrase("Urobilinógeno", fonEiqueta));
                celUrobilinogeno.BorderWidth = 0;
                celUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
                celUrobilinogeno.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoUrobilinogeno = new PdfPCell(new Phrase(datosEGO.Urobilinogeno, fontDato));
                celDatoUrobilinogeno.BorderWidth = 0;
                celDatoUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoUrobilinogeno.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaUrobilinogeno = new PdfPCell(new Phrase("0.2 MG/DL", fontDato));
                celReferenciaUrobilinogeno.BorderWidth = 0;
                celReferenciaUrobilinogeno.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaUrobilinogeno.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 6 (Sangre)
                PdfPCell celSangre = new PdfPCell(new Phrase("Sangre", fonEiqueta));
                celSangre.BorderWidth = 0;
                celSangre.VerticalAlignment = Element.ALIGN_CENTER;
                celSangre.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoSangre = new PdfPCell(new Phrase(datosEGO.Sangre, fontDato));
                celDatoSangre.BorderWidth = 0;
                celDatoSangre.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoSangre.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaSangre = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaSangre.BorderWidth = 0;
                celReferenciaSangre.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaSangre.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 7 (Nitritos)
                PdfPCell celNitritos = new PdfPCell(new Phrase("Nitritos", fonEiqueta));
                celNitritos.BorderWidth = 0;
                celNitritos.VerticalAlignment = Element.ALIGN_CENTER;
                celNitritos.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoNitrito = new PdfPCell(new Phrase(datosEGO.Nitritos, fontDato));
                celDatoNitrito.BorderWidth = 0;
                celDatoNitrito.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoNitrito.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaNitrito = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaNitrito.BorderWidth = 0;
                celReferenciaNitrito.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaNitrito.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 8 (Acido ascorbico)
                PdfPCell celAscorbico = new PdfPCell(new Phrase("Ácido ascórbico", fonEiqueta));
                celAscorbico.BorderWidth = 0;
                celAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
                celAscorbico.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoAscorbico = new PdfPCell(new Phrase(datosEGO.AcidoAscorbico, fontDato));
                celDatoAscorbico.BorderWidth = 0;
                celDatoAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoAscorbico.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaAscorbico = new PdfPCell(new Phrase("NEGATIVO", fontDato));
                celReferenciaAscorbico.BorderWidth = 0;
                celReferenciaAscorbico.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaAscorbico.HorizontalAlignment = Element.ALIGN_CENTER;

                tblQuimico.AddCell(celTituloQui);
                tblQuimico.AddCell(celTituloQuimicaReferencia);

                tblQuimico.AddCell(celTitGlucosa);
                tblQuimico.AddCell(celDatoGlucosa);
                tblQuimico.AddCell(celReferenciaGlucosa);

                tblQuimico.AddCell(celCetona);
                tblQuimico.AddCell(celDatoCetona);
                tblQuimico.AddCell(celReferenciaCetona);

                tblQuimico.AddCell(celBilirrubina);
                tblQuimico.AddCell(celDatoBilirrubina);
                tblQuimico.AddCell(celReferenciaBilirrubina);

                tblQuimico.AddCell(celProteina);
                tblQuimico.AddCell(celDatoProteina);
                tblQuimico.AddCell(celReferenciaProteina);

                tblQuimico.AddCell(celUrobilinogeno);
                tblQuimico.AddCell(celDatoUrobilinogeno);
                tblQuimico.AddCell(celReferenciaUrobilinogeno);

                tblQuimico.AddCell(celSangre);
                tblQuimico.AddCell(celDatoSangre);
                tblQuimico.AddCell(celReferenciaSangre);

                tblQuimico.AddCell(celNitritos);
                tblQuimico.AddCell(celDatoNitrito);
                tblQuimico.AddCell(celReferenciaNitrito);

                tblQuimico.AddCell(celAscorbico);
                tblQuimico.AddCell(celDatoAscorbico);
                tblQuimico.AddCell(celReferenciaAscorbico);

                docRepAll.Add(tblQuimico);

                #endregion

                #region Examen Microscopico
                PdfPTable tblMicro = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };

                float[] valueMicro = new float[3];
                valueMicro[0] = 200;
                valueMicro[1] = 200;
                valueMicro[2] = 160;
                tblMicro.SetWidths(valueMicro);
                tblMicro.SpacingAfter = 25f;
                tblMicro.DefaultCell.Border = 0;

                //------------------------------------------------------------------------ Linea 1
                PdfPCell celTituloMic = new PdfPCell(new Phrase("Examen Microscópico", fonEiqueta)) { Colspan = 2 };
                celTituloMic.BorderWidth = 0;
                celTituloMic.BorderWidthBottom = 1;
                celTituloMic.VerticalAlignment = Element.ALIGN_CENTER;
                celTituloMic.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celTituloMicReferencia = new PdfPCell(new Phrase("", fonEiqueta));
                celTituloMicReferencia.BorderWidth = 0;
                celTituloMicReferencia.BorderWidthBottom = 1;
                celTituloMicReferencia.VerticalAlignment = Element.ALIGN_CENTER;
                celTituloMicReferencia.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 2 (Leucocitos)
                PdfPCell celTitLeucocitos = new PdfPCell(new Phrase("Leucocitos", fonEiqueta));
                celTitLeucocitos.BorderWidth = 0;
                celTitLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celTitLeucocitos.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoLeucocitos = new PdfPCell(new Phrase(datosEGO.Leucocitos, fontDato));
                celDatoLeucocitos.BorderWidth = 0;
                celDatoLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoLeucocitos.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaLeucocitos = new PdfPCell(new Phrase("X CPO", fontDato));
                celReferenciaLeucocitos.BorderWidth = 0;
                celReferenciaLeucocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaLeucocitos.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 3 (Eritrocitos)
                PdfPCell celTitEritrocitos = new PdfPCell(new Phrase("Eritrocitos", fonEiqueta));
                celTitEritrocitos.BorderWidth = 0;
                celTitEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEritrocitos.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEritrocitos = new PdfPCell(new Phrase(datosEGO.Eritrocitos, fontDato));
                celDatoEritrocitos.BorderWidth = 0;
                celDatoEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEritrocitos.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaEritrocitos = new PdfPCell(new Phrase("X CPO", fontDato));
                celReferenciaEritrocitos.BorderWidth = 0;
                celReferenciaEritrocitos.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaEritrocitos.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 4 (Bacterias)
                PdfPCell celTitBacterias = new PdfPCell(new Phrase("Bacterias", fonEiqueta));
                celTitBacterias.BorderWidth = 0;
                celTitBacterias.VerticalAlignment = Element.ALIGN_CENTER;
                celTitBacterias.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoBacterias = new PdfPCell(new Phrase(datosEGO.Bacterias, fontDato));
                celDatoBacterias.BorderWidth = 0;
                celDatoBacterias.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoBacterias.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaBacterias = new PdfPCell(new Phrase("", fontDato));
                celReferenciaBacterias.BorderWidth = 0;
                celReferenciaBacterias.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaBacterias.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 5 (Células)
                PdfPCell celTitCelulas = new PdfPCell(new Phrase("Células", fonEiqueta));
                celTitCelulas.BorderWidth = 0;
                celTitCelulas.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCelulas.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCelulas = new PdfPCell(new Phrase(datosEGO.Celulas, fontDato));
                celDatoCelulas.BorderWidth = 0;
                celDatoCelulas.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCelulas.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaCelulas = new PdfPCell(new Phrase("", fontDato));
                celReferenciaCelulas.BorderWidth = 0;
                celReferenciaCelulas.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaCelulas.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 6 (Cilindros)
                PdfPCell celTitCilindros = new PdfPCell(new Phrase("Cilindros de", fonEiqueta));
                celTitCilindros.BorderWidth = 0;
                celTitCilindros.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCilindros.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCilindros = new PdfPCell(new Phrase(datosEGO.Cilindros, fontDato));
                celDatoCilindros.BorderWidth = 0;
                celDatoCilindros.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCilindros.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaCilindros = new PdfPCell(new Phrase("X CPO", fontDato));
                celReferenciaCilindros.BorderWidth = 0;
                celReferenciaCilindros.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaCilindros.HorizontalAlignment = Element.ALIGN_CENTER;

                //------------------------------------------------------------------------ Linea 7 (Cristales)
                PdfPCell celTitCristlaes = new PdfPCell(new Phrase("Cristales de", fonEiqueta));
                celTitCristlaes.BorderWidth = 0;
                celTitCristlaes.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCristlaes.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCristales = new PdfPCell(new Phrase(datosEGO.Cristales, fontDato));
                celDatoCristales.BorderWidth = 0;
                celDatoCristales.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCristales.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celReferenciaCristales = new PdfPCell(new Phrase("X CPO", fontDato));
                celReferenciaCristales.BorderWidth = 0;
                celReferenciaCristales.VerticalAlignment = Element.ALIGN_CENTER;
                celReferenciaCristales.HorizontalAlignment = Element.ALIGN_CENTER;

                tblMicro.AddCell(celTituloMic);
                tblMicro.AddCell(celTituloMicReferencia);

                tblMicro.AddCell(celTitLeucocitos);
                tblMicro.AddCell(celDatoLeucocitos);
                tblMicro.AddCell(celReferenciaLeucocitos);

                tblMicro.AddCell(celTitEritrocitos);
                tblMicro.AddCell(celDatoEritrocitos);
                tblMicro.AddCell(celReferenciaEritrocitos);

                tblMicro.AddCell(celTitBacterias);
                tblMicro.AddCell(celDatoBacterias);
                tblMicro.AddCell(celReferenciaBacterias);

                tblMicro.AddCell(celTitCelulas);
                tblMicro.AddCell(celDatoCelulas);
                tblMicro.AddCell(celReferenciaCelulas);

                tblMicro.AddCell(celTitCilindros);
                tblMicro.AddCell(celDatoCilindros);
                tblMicro.AddCell(celReferenciaCilindros);

                tblMicro.AddCell(celTitCristlaes);
                tblMicro.AddCell(celDatoCristales);
                tblMicro.AddCell(celReferenciaCristales);

                docRepAll.Add(tblMicro);

                #endregion

                #region Observaciones
                Paragraph observaciones = new Paragraph()
                {
                    Alignment = Element.ALIGN_LEFT
                };
                observaciones.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
                observaciones.Add("Observaciones: ");
                observaciones.Add(Chunk.TABBING);
                observaciones.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                observaciones.Add(datosEGO.Observaciones);

                docRepAll.Add(observaciones);
                #endregion

                #region firmas
                PdfPTable tblFirmas = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] sizeFirmas = new float[3];
                sizeFirmas[0] = 180;
                sizeFirmas[1] = 200;
                sizeFirmas[2] = 180;
                tblFirmas.SetWidths(sizeFirmas);
                tblFirmas.SpacingBefore = 70f;
                tblFirmas.DefaultCell.Border = 0;

                PdfPCell celFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celFolio.BorderWidth = 0;
                celFolio.BorderWidthTop = 0.75f;
                celFolio.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo = new PdfPCell(new Phrase(datosEGO.realizo, fontDato));
                celNombreRealizo.BorderWidth = 0;
                celNombreRealizo.BorderWidthTop = 0.75f;
                celNombreRealizo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable = new PdfPCell(new Phrase(datosEGO.superviso, fontDato));
                celNombreResponsable.BorderWidth = 0;
                celNombreResponsable.BorderWidthTop = 0.75f;
                celNombreResponsable.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_b = new PdfPCell(new Phrase(datosEGO.FOLIO, fontDato));
                celFolio_b.BorderWidth = 0;
                celFolio_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_b = new PdfPCell(new Phrase(datosEGO.ced_prof_realizo, fontDato));
                celNombreRealizo_b.BorderWidth = 0;
                celNombreRealizo_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_b = new PdfPCell(new Phrase(datosEGO.ced_prof_superviso, fontDato));
                celNombreResponsable_b.BorderWidth = 0;
                celNombreResponsable_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_c = new PdfPCell(new Phrase("", fontDato));
                celFolio_c.BorderWidth = 0;
                celFolio_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_c = new PdfPCell(new Phrase("Realizó", fonEiqueta));
                celNombreRealizo_c.BorderWidth = 0;
                celNombreRealizo_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_c = new PdfPCell(new Phrase("Responsable Sanitario", fonEiqueta));
                celNombreResponsable_c.BorderWidth = 0;
                celNombreResponsable_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_d = new PdfPCell(new Phrase("Este documento es confidencial no tendrá ningún valor jurídico si presenta tachaduras o enmendaduras", fontDato)) { Colspan = 3 };
                celFolio_d.BorderWidth = 0;
                celFolio_d.HorizontalAlignment = Element.ALIGN_CENTER;

                tblFirmas.AddCell(celFolio);
                tblFirmas.AddCell(celNombreRealizo);
                tblFirmas.AddCell(celNombreResponsable);

                tblFirmas.AddCell(celFolio_b);
                tblFirmas.AddCell(celNombreRealizo_b);
                tblFirmas.AddCell(celNombreResponsable_b);

                tblFirmas.AddCell(celFolio_c);
                tblFirmas.AddCell(celNombreRealizo_c);
                tblFirmas.AddCell(celNombreResponsable_c);

                tblFirmas.AddCell(celFolio_d);

                docRepAll.Add(tblFirmas);

                #endregion

                docRepAll.NewPage();
            }

            docRepAll.Close();
            byte[] bytesStream = msRepAll.ToArray();
            msRepAll = new MemoryStream();
            msRepAll.Write(bytesStream, 0, bytesStream.Length);
            msRepAll.Position = 0;

            return new FileStreamResult(msRepAll, "application/pdf");
        }

        //public IActionResult allBHs(int id, string fechaAll)
        //{
        //    var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = fechaAll }).ToList();

        //    var _total = datos.Count();

        //    MemoryStream msRepAll = new MemoryStream();
        //    Document docRepAll = new Document(PageSize.LETTER, 30f, 20f, 20f, 40f);
        //    PdfWriter pwRepAll = PdfWriter.GetInstance(docRepAll, msRepAll);
        //    docRepAll.Open();

        //    for(int idBh=0; idBh<_total; idBh++)
        //    {
        //        int _elId = datos[idBh].idhistorico;
        //        var datosBH = repo.Getdosparam1<BhModel>("sp_medicos_rep_bh", new { @idhistorico = _elId }).FirstOrDefault();

        //        var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
        //        var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

        //        #region encabezado
        //        //-------------------------------------------------------------------------------------------------------- 1a linea
        //        string imageizq = @"C:/inetpub/wwwroot/fotoUser/gobedohor.png";
        //        iTextSharp.text.Image jpgSupIzq = iTextSharp.text.Image.GetInstance(imageizq);
        //        jpgSupIzq.ScaleToFit(80f, 80f);

        //        PdfPCell clLogoSupIzq = new PdfPCell();
        //        clLogoSupIzq.BorderWidth = 0;
        //        clLogoSupIzq.VerticalAlignment = Element.ALIGN_BOTTOM;
        //        clLogoSupIzq.AddElement(jpgSupIzq);

        //        string imageder = @"C:/inetpub/wwwroot/fotoUser/nuevoCeccc.png";
        //        iTextSharp.text.Image jpgSupDer = iTextSharp.text.Image.GetInstance(imageder);
        //        jpgSupDer.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
        //        jpgSupDer.ScaleToFit(100f, 100f);

        //        PdfPCell clLogoSupDer = new PdfPCell();
        //        clLogoSupDer.BorderWidth = 0;
        //        clLogoSupDer.VerticalAlignment = Element.ALIGN_BOTTOM;
        //        clLogoSupDer.AddElement(jpgSupDer);

        //        Chunk chkTit = new Chunk("Dirección Médica y Toxicológica", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragraph = new Paragraph();
        //        paragraph.Alignment = Element.ALIGN_CENTER;
        //        paragraph.Add(chkTit);

        //        Chunk chkSub = new Chunk("Biometría Hemática", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragraph1 = new Paragraph();
        //        paragraph1.Alignment = Element.ALIGN_CENTER;
        //        paragraph1.Add(chkSub);

        //        PdfPCell clTitulo = new PdfPCell();
        //        clTitulo.BorderWidth = 0;
        //        clTitulo.AddElement(paragraph);

        //        PdfPCell clSubTit = new PdfPCell();
        //        clSubTit.BorderWidth = 0;
        //        clSubTit.AddElement(paragraph1);

        //        PdfPTable tblTitulo = new PdfPTable(1);
        //        tblTitulo.WidthPercentage = 100;
        //        tblTitulo.AddCell(clTitulo);
        //        tblTitulo.AddCell(clSubTit);

        //        PdfPCell clTablaTitulo = new PdfPCell();
        //        clTablaTitulo.BorderWidth = 0;
        //        clTablaTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        clTablaTitulo.AddElement(tblTitulo);

        //        PdfPTable tblEncabezado = new PdfPTable(3);
        //        tblEncabezado.WidthPercentage = 100;
        //        float[] widths = new float[] { 20f, 60f, 20f };
        //        tblEncabezado.SetWidths(widths);

        //        tblEncabezado.AddCell(clLogoSupIzq);
        //        tblEncabezado.AddCell(clTablaTitulo);
        //        tblEncabezado.AddCell(clLogoSupDer);

        //        docRepAll.Add(tblEncabezado);

        //        #endregion

        //        #region emision - revision - codigo
        //        Chunk chkemision = new Chunk("EMISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragraphemision = new Paragraph();
        //        paragraphemision.Alignment = Element.ALIGN_CENTER;
        //        paragraphemision.Add(chkemision);

        //        PdfPCell clEmision = new PdfPCell();
        //        clEmision.BorderWidth = 0;
        //        clEmision.AddElement(paragraphemision);

        //        Chunk chkrevision = new Chunk("REVISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragrarevision = new Paragraph();
        //        paragrarevision.Alignment = Element.ALIGN_CENTER;
        //        paragrarevision.Add(chkrevision);

        //        PdfPCell clrevision = new PdfPCell();
        //        clrevision.BorderWidth = 0;
        //        clrevision.AddElement(paragrarevision);

        //        Chunk chkcodigo = new Chunk("CODIGO", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragracodigo = new Paragraph();
        //        paragracodigo.Alignment = Element.ALIGN_CENTER;
        //        paragracodigo.Add(chkcodigo);

        //        PdfPCell clcodigo = new PdfPCell();
        //        clcodigo.BorderWidth = 0;
        //        clcodigo.AddElement(paragracodigo);

        //        Chunk chkemision_b = new Chunk(DateTime.Now.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragraphemision_b = new Paragraph();
        //        paragraphemision_b.Alignment = Element.ALIGN_CENTER;
        //        paragraphemision_b.Add(chkemision_b);

        //        PdfPCell clEmision_b = new PdfPCell();
        //        clEmision_b.BorderWidth = 0;
        //        clEmision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        clEmision_b.UseAscender = true;
        //        clEmision_b.AddElement(paragraphemision_b);

        //        Chunk chkrevision_b = new Chunk("1.1", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragrarevision_b = new Paragraph();
        //        paragrarevision_b.Alignment = Element.ALIGN_CENTER;
        //        paragrarevision_b.Add(chkrevision_b);

        //        PdfPCell clrevision_b = new PdfPCell();
        //        clrevision_b.BorderWidth = 0;
        //        clrevision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        clrevision_b.UseAscender = true;
        //        clrevision_b.AddElement(paragrarevision_b);

        //        Chunk chkcodigo_b = new Chunk("CECCC/DMT/07", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        //        Paragraph paragracodigo_b = new Paragraph();
        //        paragracodigo_b.Alignment = Element.ALIGN_CENTER;
        //        paragracodigo_b.Add(chkcodigo_b);

        //        PdfPCell clcodigo_b = new PdfPCell();
        //        clcodigo_b.BorderWidth = 0;
        //        clcodigo_b.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        clcodigo_b.UseAscender = true;
        //        clcodigo_b.AddElement(paragracodigo_b);

        //        PdfPTable tblemision = new PdfPTable(3);
        //        tblemision.WidthPercentage = 100;
        //        float[] widthsemision = new float[] { 20f, 60f, 20f };
        //        tblemision.SetWidths(widthsemision);

        //        tblemision.AddCell(clEmision);
        //        tblemision.AddCell(clrevision);
        //        tblemision.AddCell(clcodigo);

        //        tblemision.AddCell(clEmision_b);
        //        tblemision.AddCell(clrevision_b);
        //        tblemision.AddCell(clcodigo_b);

        //        docRepAll.Add(tblemision);
        //        #endregion

        //        #region Titulo Datos personales
        //        PdfPTable Datospersonales = new PdfPTable(1);
        //        Datospersonales.TotalWidth = 560f;
        //        Datospersonales.LockedWidth = true;

        //        Datospersonales.SetWidths(widthsTitulosGenerales);
        //        Datospersonales.HorizontalAlignment = 0;
        //        Datospersonales.SpacingBefore = 20f;
        //        Datospersonales.SpacingAfter = 10f;

        //        PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //        cellTituloTituloFamiliar.HorizontalAlignment = 1;
        //        cellTituloTituloFamiliar.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellTituloTituloFamiliar.UseAscender = true;
        //        cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //        cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //        cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //        Datospersonales.AddCell(cellTituloTituloFamiliar);

        //        docRepAll.Add(Datospersonales);

        //        #endregion

        //        #region Tabla Datos Personales
        //        PdfPTable tblDatosEvaluado = new PdfPTable(4)
        //        {
        //            TotalWidth = 560,
        //            LockedWidth = true
        //        };

        //        float[] values = new float[4];
        //        values[0] = 80;
        //        values[1] = 270;
        //        values[2] = 100;
        //        values[3] = 110;
        //        tblDatosEvaluado.SetWidths(values);
        //        tblDatosEvaluado.HorizontalAlignment = 0;
        //        tblDatosEvaluado.SpacingAfter = 20f;
        //        //tblDatosEvaluado.SpacingBefore = 10f;
        //        tblDatosEvaluado.DefaultCell.Border = 0;

        //        //-------------------------------------------------------------------------------------------------------- 1a linea
        //        PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
        //        celTitnombre.BorderWidth = 0;
        //        celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos[idBh].evaluado, fontDato));
        //        celDatoEvaluado.BorderWidth = 0;
        //        celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
        //        celTitCodigo.BorderWidth = 0;
        //        celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos[idBh].codigoevaluado, fontDato));
        //        celDatoCodigo.BorderWidth = 0;
        //        celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- 2a linea
        //        PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
        //        celTitSexo.BorderWidth = 0;
        //        celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos[idBh].sexo, fontDato));
        //        celDatoSexo.BorderWidth = 0;
        //        celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
        //        celTitEvaluacion.BorderWidth = 0;
        //        celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos[idBh].evaluacion, fontDato));
        //        celDatoEvaluacion.BorderWidth = 0;
        //        celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- 3a linea
        //        PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
        //        celTitEdad.BorderWidth = 0;
        //        celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos[idBh].edad.ToString(), fontDato)); ;
        //        celDatoEdad.BorderWidth = 0;
        //        celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
        //        celTitFolio.BorderWidth = 0;
        //        celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos[idBh].folio, fontDato));
        //        celDatoFolio.BorderWidth = 0;
        //        celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- 4a linea
        //        PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
        //        celTitCurp.BorderWidth = 0;
        //        celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos[idBh].curp, fontDato)); ;
        //        celDatoCurp.BorderWidth = 0;
        //        celDatoCurp.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoCurp.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celTitFecha = new PdfPCell(new Phrase("Fecha", fonEiqueta));
        //        celTitFecha.BorderWidth = 0;
        //        celTitFecha.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitFecha.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoFecha = new PdfPCell(new Phrase(DateTime.Now.ToShortDateString(), fontDato));
        //        celDatoFecha.BorderWidth = 0;
        //        celDatoFecha.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoFecha.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- 5a linea
        //        PdfPCell celTitDependencia = new PdfPCell(new Phrase("Dependencia", fonEiqueta));
        //        celTitDependencia.BorderWidth = 0;
        //        celTitDependencia.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos[idBh].dependencia, fontDato)) { Colspan = 3 };
        //        celDatoDependencia.BorderWidth = 0;
        //        celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- 6a linea
        //        PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
        //        celTitPuesto.BorderWidth = 0;
        //        celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos[idBh].puesto, fontDato)) { Colspan = 3 };
        //        celDatoPuesto.BorderWidth = 0;
        //        celDatoPuesto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celDatoPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

        //        tblDatosEvaluado.AddCell(celTitnombre);
        //        tblDatosEvaluado.AddCell(celDatoEvaluado);
        //        tblDatosEvaluado.AddCell(celTitCodigo);
        //        tblDatosEvaluado.AddCell(celDatoCodigo);

        //        tblDatosEvaluado.AddCell(celTitSexo);
        //        tblDatosEvaluado.AddCell(celDatoSexo);
        //        tblDatosEvaluado.AddCell(celTitEvaluacion);
        //        tblDatosEvaluado.AddCell(celDatoEvaluacion);

        //        tblDatosEvaluado.AddCell(celTitEdad);
        //        tblDatosEvaluado.AddCell(celDatoEdad);
        //        tblDatosEvaluado.AddCell(celTitFolio);
        //        tblDatosEvaluado.AddCell(celDatoFolio);

        //        tblDatosEvaluado.AddCell(celTitCurp);
        //        tblDatosEvaluado.AddCell(celDatoCurp);
        //        tblDatosEvaluado.AddCell(celTitFecha);
        //        tblDatosEvaluado.AddCell(celDatoFecha);

        //        tblDatosEvaluado.AddCell(celTitDependencia);
        //        tblDatosEvaluado.AddCell(celDatoDependencia);

        //        tblDatosEvaluado.AddCell(celTitPuesto);
        //        tblDatosEvaluado.AddCell(celDatoPuesto);

        //        docRepAll.Add(tblDatosEvaluado);

        //        #endregion

        //        #region Titulo Serie Blanca
        //        PdfPTable Serieblanca = new PdfPTable(1);
        //        Serieblanca.TotalWidth = 560f;
        //        Serieblanca.LockedWidth = true;

        //        Serieblanca.SetWidths(widthsTitulosGenerales);
        //        Serieblanca.HorizontalAlignment = 0;
        //        Serieblanca.SpacingAfter = 10f;

        //        PdfPCell cellTituloSerieblanca = new PdfPCell(new Phrase("SERIE BLANCA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //        cellTituloSerieblanca.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
        //        cellTituloSerieblanca.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellTituloSerieblanca.UseAscender = true;
        //        cellTituloSerieblanca.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //        cellTituloSerieblanca.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //        cellTituloSerieblanca.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //        Serieblanca.AddCell(cellTituloSerieblanca);

        //        docRepAll.Add(Serieblanca);
        //        #endregion

        //        #region Datos serie blanca
        //        PdfPTable tblSB = new PdfPTable(5)
        //        {
        //            TotalWidth = 560,
        //            LockedWidth = true
        //        };
        //        float[] valSB = new float[5];
        //        valSB[0] = 170;
        //        valSB[1] = 100;
        //        valSB[2] = 130;
        //        valSB[3] = 80;
        //        valSB[4] = 80;
        //        tblSB.SetWidths(valSB);
        //        tblSB.HorizontalAlignment = 0;
        //        tblSB.SpacingAfter = 10f;
        //        //tblDatosEvaluado.SpacingBefore = 10f;
        //        tblSB.DefaultCell.Border = 0;

        //        //-------------------------------------------------------------------------------------------------------- Leucocitos totales
        //        PdfPCell celSBLeucocito = new PdfPCell(new Phrase("Leucocitos totales", fonEiqueta));
        //        celSBLeucocito.BorderWidth = 0;
        //        celSBLeucocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBLeucocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Leucocitos = Convert.ToDecimal(datosBH.wbc);
        //        PdfPCell celSBDatoLeuco = new PdfPCell(new Phrase(Leucocitos.ToString("F2"), fontDato));
        //        celSBDatoLeuco.BorderWidth = 0;
        //        celSBDatoLeuco.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoLeuco.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBRef = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
        //        celSBRef.BorderWidth = 0;
        //        celSBRef.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBRef.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBAbs = new PdfPCell(new Phrase("4.0 - 10.5", fontDato));
        //        celSBAbs.BorderWidth = 0;
        //        celSBAbs.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbs.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBVacio = new PdfPCell(new Phrase("", fontDato));
        //        celSBVacio.BorderWidth = 0;
        //        celSBVacio.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBVacio.HorizontalAlignment = Element.ALIGN_LEFT;

        //        //-------------------------------------------------------------------------------------------------------- Titulos
        //        PdfPCell celSBcelda1 = new PdfPCell(new Phrase("", fonEiqueta));
        //        celSBcelda1.BorderWidth = 0;
        //        celSBcelda1.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBcelda1.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBcelda2 = new PdfPCell(new Phrase("Valor relativo", fonEiqueta));
        //        celSBcelda2.BorderWidth = 0;
        //        celSBcelda2.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBcelda2.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBCelda3 = new PdfPCell(new Phrase("Observado Absolutos", fonEiqueta));
        //        celSBCelda3.BorderWidth = 0;
        //        celSBCelda3.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBCelda3.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBCelda4 = new PdfPCell(new Phrase("Valores de Referencia", fonEiqueta)) { Colspan = 2 };
        //        celSBCelda4.BorderWidth = 0;
        //        celSBCelda4.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBCelda4.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Titulos
        //        PdfPCell celSBcelda1b = new PdfPCell(new Phrase("", fonEiqueta));
        //        celSBcelda1b.BorderWidth = 0;
        //        celSBcelda1b.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBcelda1b.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBcelda2b = new PdfPCell(new Phrase("", fonEiqueta));
        //        celSBcelda2b.BorderWidth = 0;
        //        celSBcelda2b.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBcelda2b.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBCelda3b = new PdfPCell(new Phrase("", fonEiqueta));
        //        celSBCelda3b.BorderWidth = 0;
        //        celSBCelda3b.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBCelda3b.HorizontalAlignment = Element.ALIGN_LEFT;

        //        PdfPCell celSBCelda4b = new PdfPCell(new Phrase("Relativos", fonEiqueta));
        //        celSBCelda4b.BorderWidth = 0;
        //        celSBCelda4b.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBCelda4b.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBCelda5b = new PdfPCell(new Phrase("Absolutos", fonEiqueta));
        //        celSBCelda5b.BorderWidth = 0;
        //        celSBCelda5b.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBCelda5b.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Linfocitos
        //        PdfPCell celSBLinfocito = new PdfPCell(new Phrase("Linfocitos", fonEiqueta));
        //        celSBLinfocito.BorderWidth = 0;
        //        celSBLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBLinfocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Linfocitos = Convert.ToDecimal(datosBH.Limph2);
        //        PdfPCell celSBDatoLinfocito = new PdfPCell(new Phrase(Linfocitos.ToString("F2"), fontDato));
        //        celSBDatoLinfocito.BorderWidth = 0;
        //        celSBDatoLinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoLinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //        decimal Linfocitoabsoluto = Convert.ToDecimal(datosBH.Limph);
        //        PdfPCell celSBDatoLinfocitoabsoluto = new PdfPCell(new Phrase(Linfocitoabsoluto.ToString("F2"), fontDato));
        //        celSBDatoLinfocitoabsoluto.BorderWidth = 0;
        //        celSBDatoLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBAbslinfocito = new PdfPCell(new Phrase("20.0 - 40.0", fontDato));
        //        celSBAbslinfocito.BorderWidth = 0;
        //        celSBAbslinfocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbslinfocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBLinfocitoabsoluto = new PdfPCell(new Phrase("1.5 - 4.0", fontDato));
        //        celSBLinfocitoabsoluto.BorderWidth = 0;
        //        celSBLinfocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBLinfocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Monocitos
        //        PdfPCell celSBMonocito = new PdfPCell(new Phrase("Monocitos", fonEiqueta));
        //        celSBMonocito.BorderWidth = 0;
        //        celSBMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBMonocito.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Monocitos = Convert.ToDecimal(datosBH.Mid2);
        //        PdfPCell celSBDatoMonocito = new PdfPCell(new Phrase(Monocitos.ToString("F2"), fontDato));
        //        celSBDatoMonocito.BorderWidth = 0;
        //        celSBDatoMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //        decimal Monocitoabsoluto = Convert.ToDecimal(datosBH.Mid);
        //        PdfPCell celSBDatoMonocitoabsoluto = new PdfPCell(new Phrase(Monocitoabsoluto.ToString("F2"), fontDato));
        //        celSBDatoMonocitoabsoluto.BorderWidth = 0;
        //        celSBDatoMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBAbsMonocito = new PdfPCell(new Phrase("3.0 - 10.0", fontDato));
        //        celSBAbsMonocito.BorderWidth = 0;
        //        celSBAbsMonocito.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbsMonocito.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBMonocitoabsoluto = new PdfPCell(new Phrase("0.2 - 0.9", fontDato));
        //        celSBMonocitoabsoluto.BorderWidth = 0;
        //        celSBMonocitoabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBMonocitoabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Neutrofilos
        //        PdfPCell celSBNeutrofilos = new PdfPCell(new Phrase("Neutrófilos", fonEiqueta));
        //        celSBNeutrofilos.BorderWidth = 0;
        //        celSBNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBNeutrofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Neutrofilos = Convert.ToDecimal(datosBH.Neu2);
        //        PdfPCell celSBDatoNeutrofilos = new PdfPCell(new Phrase(Neutrofilos.ToString("F2"), fontDato));
        //        celSBDatoNeutrofilos.BorderWidth = 0;
        //        celSBDatoNeutrofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoNeutrofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //        decimal Neutrofiloabs = Convert.ToDecimal(datosBH.Neu);
        //        PdfPCell celSBDatoNeutrofiloabsoluto = new PdfPCell(new Phrase(Neutrofiloabs.ToString("F2"), fontDato));
        //        celSBDatoNeutrofiloabsoluto.BorderWidth = 0;
        //        celSBDatoNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBAbsNeutrofilo = new PdfPCell(new Phrase("50.0 - 70.0", fontDato));
        //        celSBAbsNeutrofilo.BorderWidth = 0;
        //        celSBAbsNeutrofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbsNeutrofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBNeutrofiloabsoluto = new PdfPCell(new Phrase("1.8 - 7.2", fontDato));
        //        celSBNeutrofiloabsoluto.BorderWidth = 0;
        //        celSBNeutrofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBNeutrofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Eosinofilos
        //        PdfPCell celSBEosinofilos = new PdfPCell(new Phrase("Eosinófilos", fonEiqueta));
        //        celSBEosinofilos.BorderWidth = 0;
        //        celSBEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBEosinofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Eosinofilos = Convert.ToDecimal(datosBH.Eos2);
        //        PdfPCell celSBDatoEosinofilos = new PdfPCell(new Phrase(Eosinofilos.ToString("F2"), fontDato));
        //        celSBDatoEosinofilos.BorderWidth = 0;
        //        celSBDatoEosinofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoEosinofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //        decimal Eofinofiloabs = Convert.ToDecimal(datosBH.Eos);
        //        PdfPCell celSBDatoEosinofiloabsoluto = new PdfPCell(new Phrase(Eofinofiloabs.ToString("F2"), fontDato));
        //        celSBDatoEosinofiloabsoluto.BorderWidth = 0;
        //        celSBDatoEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBAbsEosinofilo = new PdfPCell(new Phrase("0 - 3", fontDato));
        //        celSBAbsEosinofilo.BorderWidth = 0;
        //        celSBAbsEosinofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbsEosinofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBEosinofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.7", fontDato));
        //        celSBEosinofiloabsoluto.BorderWidth = 0;
        //        celSBEosinofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBEosinofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        //-------------------------------------------------------------------------------------------------------- Basofilos
        //        PdfPCell celSBBasofilos = new PdfPCell(new Phrase("Basófilos", fonEiqueta));
        //        celSBBasofilos.BorderWidth = 0;
        //        celSBBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBBasofilos.HorizontalAlignment = Element.ALIGN_LEFT;

        //        decimal Basofilos = Convert.ToDecimal(datosBH.Bas2);
        //        PdfPCell celSBDatoBasofilos = new PdfPCell(new Phrase(Basofilos.ToString("F2"), fontDato));
        //        celSBDatoBasofilos.BorderWidth = 0;
        //        celSBDatoBasofilos.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoBasofilos.HorizontalAlignment = Element.ALIGN_CENTER;

        //        decimal Basofiloabs = Convert.ToDecimal(datosBH.Bas);
        //        PdfPCell celSBDatoBasofiloabsoluto = new PdfPCell(new Phrase(Basofiloabs.ToString("F2"), fontDato));
        //        celSBDatoBasofiloabsoluto.BorderWidth = 0;
        //        celSBDatoBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBDatoBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBAbsBasofilo = new PdfPCell(new Phrase("0 - 1 ", fontDato));
        //        celSBAbsBasofilo.BorderWidth = 0;
        //        celSBAbsBasofilo.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBAbsBasofilo.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celSBBasofiloabsoluto = new PdfPCell(new Phrase("0.0 - 0.15", fontDato));
        //        celSBBasofiloabsoluto.BorderWidth = 0;
        //        celSBBasofiloabsoluto.VerticalAlignment = Element.ALIGN_CENTER;
        //        celSBBasofiloabsoluto.HorizontalAlignment = Element.ALIGN_CENTER;

        //        tblSB.AddCell(celSBLeucocito);
        //        tblSB.AddCell(celSBDatoLeuco);
        //        tblSB.AddCell(celSBRef);
        //        tblSB.AddCell(celSBAbs);
        //        tblSB.AddCell(celSBVacio);

        //        tblSB.AddCell(celSBcelda1);
        //        tblSB.AddCell(celSBcelda2);
        //        tblSB.AddCell(celSBCelda3);
        //        tblSB.AddCell(celSBCelda4);

        //        tblSB.AddCell(celSBcelda1b);
        //        tblSB.AddCell(celSBcelda2b);
        //        tblSB.AddCell(celSBCelda3b);
        //        tblSB.AddCell(celSBCelda4b);
        //        tblSB.AddCell(celSBCelda5b);

        //        tblSB.AddCell(celSBLinfocito);
        //        tblSB.AddCell(celSBDatoLinfocito);
        //        tblSB.AddCell(celSBDatoLinfocitoabsoluto);
        //        tblSB.AddCell(celSBAbslinfocito);
        //        tblSB.AddCell(celSBLinfocitoabsoluto);

        //        tblSB.AddCell(celSBMonocito);
        //        tblSB.AddCell(celSBDatoMonocito);
        //        tblSB.AddCell(celSBDatoMonocitoabsoluto);
        //        tblSB.AddCell(celSBAbsMonocito);
        //        tblSB.AddCell(celSBMonocitoabsoluto);

        //        tblSB.AddCell(celSBNeutrofilos);
        //        tblSB.AddCell(celSBDatoNeutrofilos);
        //        tblSB.AddCell(celSBDatoNeutrofiloabsoluto);
        //        tblSB.AddCell(celSBAbsNeutrofilo);
        //        tblSB.AddCell(celSBNeutrofiloabsoluto);

        //        tblSB.AddCell(celSBEosinofilos);
        //        tblSB.AddCell(celSBDatoEosinofilos);
        //        tblSB.AddCell(celSBDatoEosinofiloabsoluto);
        //        tblSB.AddCell(celSBAbsEosinofilo);
        //        tblSB.AddCell(celSBEosinofiloabsoluto);

        //        tblSB.AddCell(celSBBasofilos);
        //        tblSB.AddCell(celSBDatoBasofilos);
        //        tblSB.AddCell(celSBDatoBasofiloabsoluto);
        //        tblSB.AddCell(celSBAbsBasofilo);
        //        tblSB.AddCell(celSBBasofiloabsoluto);

        //        docRepAll.Add(tblSB);

        //        #endregion

        //        #region Titulo Serie Roja
        //        PdfPTable Serieroja = new PdfPTable(1);
        //        Serieroja.TotalWidth = 560f;
        //        Serieroja.LockedWidth = true;

        //        Serieroja.SetWidths(widthsTitulosGenerales);
        //        Serieroja.HorizontalAlignment = 0;
        //        Serieroja.SpacingAfter = 10f;

        //        PdfPCell cellTituloSerieroja = new PdfPCell(new Phrase("SERIE ROJA", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
        //        cellTituloSerieroja.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
        //        cellTituloSerieroja.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
        //        cellTituloSerieroja.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
        //        cellTituloSerieroja.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
        //        Serieroja.AddCell(cellTituloSerieroja);

        //        docRepAll.Add(Serieroja);
        //        #endregion

        //        #region Datos serie roja
        //        PdfPTable DtsSR = new PdfPTable(5)
        //        {
        //            TotalWidth = 560,
        //            LockedWidth = true
        //        };

        //        float[] valSR = new float[5];
        //        valSR[0] = 150;
        //        valSR[1] = 50;
        //        valSR[2] = 100;
        //        valSR[3] = 130;
        //        valSR[4] = 130;
        //        DtsSR.SetWidths(valSR);
        //        DtsSR.HorizontalAlignment = 0;
        //        DtsSR.SpacingAfter = 10f;
        //        DtsSR.DefaultCell.Border = 0;

        //        //----------------------------------------------------------------------------------------Valores de referencia
        //        PdfPCell c1 = new PdfPCell(new Phrase("", fontDato)) { Colspan = 3 };
        //        c1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c1.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c1);

        //        PdfPCell c2 = new PdfPCell(new Phrase("Valores de referencia", fonEiqueta)) { Colspan = 2 };
        //        c2.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c2.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c2);

        //        //----------------------------------------------------------------------------------------Hemogloblina
        //        string rangoHemoglobina_b = datos[idBh].sexo == "HOMBRE" ? "14.0 - 17.0" : "11.0 - 14.0";
        //        PdfPCell hemo = new PdfPCell(new Phrase("Hemoglobina", fonEiqueta));
        //        hemo.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hemo.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hemo);

        //        PdfPCell fr_hemo = new PdfPCell(new Phrase(datosBH.fr_hgb, fonEiqueta));
        //        fr_hemo.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_hemo.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_hemo);

        //        decimal hemovalor = Convert.ToDecimal(datosBH.HGB);
        //        PdfPCell chemovalor = new PdfPCell(new Phrase(hemovalor.ToString("F2"), fontDato));
        //        chemovalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        chemovalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(chemovalor);

        //        PdfPCell hemoUnidad = new PdfPCell(new Phrase("g/dL", fontDato));
        //        hemoUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hemoUnidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hemoUnidad);

        //        PdfPCell hemoreferencia = new PdfPCell(new Phrase(rangoHemoglobina_b, fontDato));
        //        hemoreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hemoreferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hemoreferencia);

        //        //----------------------------------------------------------------------------------------Eritrocitos
        //        PdfPCell eritro = new PdfPCell(new Phrase("Eritrocitos", fonEiqueta));
        //        eritro.HorizontalAlignment = Element.ALIGN_LEFT;
        //        eritro.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(eritro);

        //        PdfPCell fr_eri = new PdfPCell(new Phrase(datosBH.fr_rbc, fonEiqueta));
        //        fr_eri.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_eri.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_eri);

        //        decimal eritrovalor = Convert.ToDecimal(datosBH.RBC);
        //        PdfPCell cheritrovalor = new PdfPCell(new Phrase(eritrovalor.ToString("F2"), fontDato));
        //        cheritrovalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cheritrovalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(cheritrovalor);

        //        PdfPCell eriUnidad = new PdfPCell(new Phrase("x 10^6 / uL", fontDato));
        //        eriUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        eriUnidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(eriUnidad);

        //        PdfPCell erireferencia = new PdfPCell(new Phrase("4.00 - 5.50", fontDato));
        //        erireferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        erireferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(erireferencia);

        //        //----------------------------------------------------------------------------------------HTC
        //        PdfPCell htc = new PdfPCell(new Phrase("HTC", fonEiqueta));
        //        htc.HorizontalAlignment = Element.ALIGN_LEFT;
        //        htc.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(htc);

        //        PdfPCell fr_htc = new PdfPCell(new Phrase(datosBH.fr_htc, fonEiqueta));
        //        fr_htc.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_htc.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_htc);

        //        decimal htcovalor = Convert.ToDecimal(datosBH.HTC);
        //        PdfPCell chtcvalor = new PdfPCell(new Phrase(htcovalor.ToString("F2"), fontDato));
        //        chtcvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        chtcvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(chtcvalor);

        //        PdfPCell htcUnidad = new PdfPCell(new Phrase("%", fontDato));
        //        htcUnidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        htcUnidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(htcUnidad);

        //        PdfPCell htcreferencia = new PdfPCell(new Phrase("40 - 50", fontDato));
        //        htcreferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        htcreferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(htcreferencia);

        //        //----------------------------------------------------------------------------------------VGM
        //        PdfPCell vgm = new PdfPCell(new Phrase("VGM", fonEiqueta));
        //        vgm.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vgm.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vgm);

        //        PdfPCell fr_mcv = new PdfPCell(new Phrase(datosBH.fr_mcv, fonEiqueta));
        //        fr_mcv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_mcv.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_mcv);

        //        decimal vgm_valor = Convert.ToDecimal(datosBH.MCv);
        //        PdfPCell c_vgmvalor = new PdfPCell(new Phrase(vgm_valor.ToString("F2"), fontDato));
        //        c_vgmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_vgmvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_vgmvalor);

        //        PdfPCell vgm_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //        vgm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vgm_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vgm_Unidad);

        //        PdfPCell vgm_creferencia = new PdfPCell(new Phrase("82.0 - 95.0", fontDato));
        //        vgm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vgm_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vgm_creferencia);

        //        //----------------------------------------------------------------------------------------HCM
        //        PdfPCell hcm = new PdfPCell(new Phrase("HCM", fonEiqueta));
        //        hcm.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hcm.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hcm);

        //        PdfPCell fr_mch = new PdfPCell(new Phrase(datosBH.fr_mch, fonEiqueta));
        //        fr_mch.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_mch.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_mch);

        //        decimal hcm_valor = Convert.ToDecimal(datosBH.MCH);
        //        PdfPCell c_hcmvalor = new PdfPCell(new Phrase(hcm_valor.ToString("F2"), fontDato));
        //        c_hcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_hcmvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_hcmvalor);

        //        PdfPCell hcm_Unidad = new PdfPCell(new Phrase("pg/fL", fontDato));
        //        hcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hcm_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hcm_Unidad);

        //        PdfPCell hcm_creferencia = new PdfPCell(new Phrase("27.0 - 31.0", fontDato));
        //        hcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        hcm_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(hcm_creferencia);

        //        //----------------------------------------------------------------------------------------CHCM
        //        PdfPCell chcm = new PdfPCell(new Phrase("CHCM", fonEiqueta));
        //        chcm.HorizontalAlignment = Element.ALIGN_LEFT;
        //        chcm.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(chcm);

        //        PdfPCell fr_mchc = new PdfPCell(new Phrase(datosBH.fr_mchc, fonEiqueta));
        //        fr_mchc.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_mchc.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_mchc);

        //        decimal chcm_valor = Convert.ToDecimal(datosBH.MCHC);
        //        PdfPCell c_chcmvalor = new PdfPCell(new Phrase(chcm_valor.ToString("F2"), fontDato));
        //        c_chcmvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_chcmvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_chcmvalor);

        //        PdfPCell chcm_Unidad = new PdfPCell(new Phrase("g/dL", fontDato));
        //        chcm_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        chcm_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(chcm_Unidad);

        //        PdfPCell chcm_creferencia = new PdfPCell(new Phrase("32.0 - 36.0", fontDato));
        //        chcm_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        chcm_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(chcm_creferencia);

        //        //----------------------------------------------------------------------------------------Plaquetas
        //        PdfPCell plaquetas = new PdfPCell(new Phrase("Plaquetas", fonEiqueta));
        //        plaquetas.HorizontalAlignment = Element.ALIGN_LEFT;
        //        plaquetas.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(plaquetas);

        //        PdfPCell fr_plt = new PdfPCell(new Phrase(datosBH.fr_plt, fonEiqueta));
        //        fr_plt.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_plt.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_plt);

        //        decimal plaquetas_valor = Convert.ToDecimal(datosBH.PLT);
        //        PdfPCell c_plaquetasvalor = new PdfPCell(new Phrase(plaquetas_valor.ToString("F2"), fontDato));
        //        c_plaquetasvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_plaquetasvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_plaquetasvalor);

        //        PdfPCell plaquetas_Unidad = new PdfPCell(new Phrase("x 10^3 / uL", fontDato));
        //        plaquetas_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        plaquetas_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(plaquetas_Unidad);

        //        PdfPCell plaquetas_creferencia = new PdfPCell(new Phrase("175 - 400", fontDato));
        //        plaquetas_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        plaquetas_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(plaquetas_creferencia);

        //        //----------------------------------------------------------------------------------------PCT
        //        PdfPCell pct = new PdfPCell(new Phrase("PCT", fonEiqueta));
        //        pct.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pct.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pct);

        //        PdfPCell fr_pct = new PdfPCell(new Phrase(datosBH.fr_pct, fonEiqueta));
        //        fr_pct.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_pct.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_pct);

        //        decimal pct_valor = Convert.ToDecimal(datosBH.PCT);
        //        PdfPCell c_pctvalor = new PdfPCell(new Phrase(pct_valor.ToString("F3"), fontDato));
        //        c_pctvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_pctvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_pctvalor);

        //        PdfPCell pct_Unidad = new PdfPCell(new Phrase("%", fontDato));
        //        pct_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pct_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pct_Unidad);

        //        PdfPCell pct_creferencia = new PdfPCell(new Phrase("0.108 - 0.282", fontDato));
        //        pct_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pct_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pct_creferencia);

        //        //----------------------------------------------------------------------------------------RDWCV
        //        PdfPCell rdwcv = new PdfPCell(new Phrase("RDWCV", fonEiqueta));
        //        rdwcv.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwcv.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwcv);

        //        PdfPCell fr_rdwcv = new PdfPCell(new Phrase(datosBH.fr_rdwcv, fonEiqueta));
        //        fr_rdwcv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_rdwcv.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_rdwcv);

        //        decimal rdwcv_valor = Convert.ToDecimal(datosBH.RDWCV);
        //        PdfPCell c_rdwcvvalor = new PdfPCell(new Phrase(rdwcv_valor.ToString("F2"), fontDato));
        //        c_rdwcvvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_rdwcvvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_rdwcvvalor);

        //        PdfPCell rdwcv_Unidad = new PdfPCell(new Phrase("%", fontDato));
        //        rdwcv_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwcv_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwcv_Unidad);

        //        PdfPCell rdwcv_creferencia = new PdfPCell(new Phrase("11.5 - 14.5", fontDato));
        //        rdwcv_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwcv_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwcv_creferencia);

        //        //----------------------------------------------------------------------------------------RDWSD
        //        PdfPCell rdwsd = new PdfPCell(new Phrase("RDWSD", fonEiqueta));
        //        rdwsd.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwsd.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwsd);

        //        PdfPCell fr_rdwsd = new PdfPCell(new Phrase(datosBH.fr_rdwsd, fonEiqueta));
        //        fr_rdwsd.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_rdwsd.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_rdwsd);

        //        decimal rdwsd_valor = Convert.ToDecimal(datosBH.RDWSD);
        //        PdfPCell c_rdwsdvalor = new PdfPCell(new Phrase(rdwsd_valor.ToString("F2"), fontDato));
        //        c_rdwsdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_rdwsdvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_rdwsdvalor);

        //        PdfPCell rdwsd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //        rdwsd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwsd_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwsd_Unidad);

        //        PdfPCell rdwsd_creferencia = new PdfPCell(new Phrase("35.5 - 56.0", fontDato));
        //        rdwsd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rdwsd_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(rdwsd_creferencia);

        //        //----------------------------------------------------------------------------------------VPM
        //        PdfPCell vpmd = new PdfPCell(new Phrase("VPM", fonEiqueta));
        //        vpmd.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vpmd.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vpmd);

        //        PdfPCell fr_mpv = new PdfPCell(new Phrase(datosBH.fr_mpv, fonEiqueta));
        //        fr_mpv.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_mpv.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_mpv);

        //        decimal vpmd_valor = Convert.ToDecimal(datosBH.MPV);
        //        PdfPCell c_vpmdvalor = new PdfPCell(new Phrase(vpmd_valor.ToString("F2"), fontDato));
        //        c_vpmdvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_vpmdvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_vpmdvalor);

        //        PdfPCell vpmd_Unidad = new PdfPCell(new Phrase("fL", fontDato));
        //        vpmd_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vpmd_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vpmd_Unidad);

        //        PdfPCell vpmd_creferencia = new PdfPCell(new Phrase("7.0 - 11.0", fontDato));
        //        vpmd_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        vpmd_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(vpmd_creferencia);

        //        //----------------------------------------------------------------------------------------PDW
        //        PdfPCell pdw = new PdfPCell(new Phrase("PDW", fonEiqueta));
        //        pdw.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pdw.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pdw);

        //        PdfPCell fr_pdw = new PdfPCell(new Phrase(datosBH.fr_pdw, fonEiqueta));
        //        fr_pdw.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        fr_pdw.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(fr_pdw);

        //        decimal pdw_valor = Convert.ToDecimal(datosBH.PDW);
        //        PdfPCell c_pdwvalor = new PdfPCell(new Phrase(pdw_valor.ToString("F2"), fontDato));
        //        c_pdwvalor.HorizontalAlignment = Element.ALIGN_CENTER;
        //        c_pdwvalor.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(c_pdwvalor);

        //        PdfPCell pdw_Unidad = new PdfPCell(new Phrase("", fontDato));
        //        pdw_Unidad.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pdw_Unidad.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pdw_Unidad);

        //        PdfPCell pdw_creferencia = new PdfPCell(new Phrase("15.0 - 17.0", fontDato));
        //        pdw_creferencia.HorizontalAlignment = Element.ALIGN_LEFT;
        //        pdw_creferencia.Border = PdfPCell.NO_BORDER;
        //        DtsSR.AddCell(pdw_creferencia);

        //        docRepAll.Add(DtsSR);
        //        #endregion

        //        #region extra
        //        Paragraph observaciones_bh = new Paragraph()
        //        {
        //            Alignment = Element.ALIGN_LEFT
        //        };
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //        observaciones_bh.Add("Metodologia: ");
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //        observaciones_bh.Add(Chunk.TABBING);
        //        observaciones_bh.Add("Impedancia eléctrica y colorimetría por equipo Mindray BV-30s.");
        //        observaciones_bh.Add(Chunk.NEWLINE);
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //        observaciones_bh.Add("Observaciones: ");
        //        observaciones_bh.Add(Chunk.TABBING);
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //        observaciones_bh.Add(datosBH.Observacion);
        //        observaciones_bh.Add(Chunk.NEWLINE);
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.BOLD);
        //        observaciones_bh.Add("Espécimen: ");
        //        observaciones_bh.Font = FontFactory.GetFont("Arial", 10, Font.NORMAL);
        //        observaciones_bh.Add(Chunk.TABBING);
        //        observaciones_bh.Add("Sangre total");

        //        docRepAll.Add(observaciones_bh);

        //        Paragraph valorfueraderango = new Paragraph()
        //        {
        //            Alignment = Element.ALIGN_RIGHT
        //        };
        //        valorfueraderango.Font = FontFactory.GetFont("Arial", 9, Font.NORMAL);
        //        valorfueraderango.Add("* = valor fuera de rango");

        //        docRepAll.Add(valorfueraderango);
        //        #endregion

        //        #region firmas
        //        PdfPTable tblFirmas = new PdfPTable(3)
        //        {
        //            TotalWidth = 560,
        //            LockedWidth = true
        //        };
        //        float[] sizeFirmas = new float[3];
        //        sizeFirmas[0] = 180;
        //        sizeFirmas[1] = 200;
        //        sizeFirmas[2] = 180;
        //        tblFirmas.SetWidths(sizeFirmas);
        //        tblFirmas.SpacingBefore = 25f;
        //        tblFirmas.DefaultCell.Border = 0;

        //        PdfPCell celFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
        //        celFolio.BorderWidth = 0;
        //        celFolio.BorderWidthTop = 0.75f;
        //        celFolio.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreRealizo = new PdfPCell(new Phrase(datosBH.realizo, fontDato));
        //        celNombreRealizo.BorderWidth = 0;
        //        celNombreRealizo.BorderWidthTop = 0.75f;
        //        celNombreRealizo.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreResponsable = new PdfPCell(new Phrase(datosBH.superviso, fontDato));
        //        celNombreResponsable.BorderWidth = 0;
        //        celNombreResponsable.BorderWidthTop = 0.75f;
        //        celNombreResponsable.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celFolio_b = new PdfPCell(new Phrase(datosBH.FOLIO, fontDato));
        //        celFolio_b.BorderWidth = 0;
        //        celFolio_b.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreRealizo_b = new PdfPCell(new Phrase(datosBH.ced_rea, fontDato));
        //        celNombreRealizo_b.BorderWidth = 0;
        //        celNombreRealizo_b.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreResponsable_b = new PdfPCell(new Phrase(datosBH.ced_sup, fontDato));
        //        celNombreResponsable_b.BorderWidth = 0;
        //        celNombreResponsable_b.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celFolio_c = new PdfPCell(new Phrase("", fontDato));
        //        celFolio_c.BorderWidth = 0;
        //        celFolio_c.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreRealizo_c = new PdfPCell(new Phrase("Realizó", fonEiqueta));
        //        celNombreRealizo_c.BorderWidth = 0;
        //        celNombreRealizo_c.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celNombreResponsable_c = new PdfPCell(new Phrase("Responsable Sanitario", fonEiqueta));
        //        celNombreResponsable_c.BorderWidth = 0;
        //        celNombreResponsable_c.HorizontalAlignment = Element.ALIGN_CENTER;

        //        PdfPCell celFolio_d = new PdfPCell(new Phrase("Este documento es confidencial no tendrá ningún valor jurídico si presenta tachaduras o enmendaduras.", fontDato)) { Colspan = 3 };
        //        celFolio_d.BorderWidth = 0;
        //        celFolio_d.HorizontalAlignment = Element.ALIGN_CENTER;

        //        tblFirmas.AddCell(celFolio);
        //        tblFirmas.AddCell(celNombreRealizo);
        //        tblFirmas.AddCell(celNombreResponsable);

        //        tblFirmas.AddCell(celFolio_b);
        //        tblFirmas.AddCell(celNombreRealizo_b);
        //        tblFirmas.AddCell(celNombreResponsable_b);

        //        tblFirmas.AddCell(celFolio_c);
        //        tblFirmas.AddCell(celNombreRealizo_c);
        //        tblFirmas.AddCell(celNombreResponsable_c);

        //        tblFirmas.AddCell(celFolio_d);

        //        docRepAll.Add(tblFirmas);
        //        #endregion

        //        docRepAll.NewPage();
        //    }

        //    docRepAll.Close();
        //    byte[] bytesStream = msRepAll.ToArray();
        //    msRepAll = new MemoryStream();
        //    msRepAll.Write(bytesStream, 0, bytesStream.Length);
        //    msRepAll.Position = 0;

        //    return new FileStreamResult(msRepAll, "application/pdf");
        //}

        public IActionResult allQS(int id, string fechaAll)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = fechaAll }).ToList();

            var _total = datos.Count();

            MemoryStream msRepAll = new MemoryStream();
            Document docRepAll = new Document(PageSize.LETTER, 30f, 20f, 20f, 40f);
            PdfWriter pwRepAll = PdfWriter.GetInstance(docRepAll, msRepAll);
            docRepAll.Open();

            for(int idQS = 0; idQS < _total; idQS++)
            {
                int _elId = datos[idQS].idhistorico;
                var datosQS = repo.Getdosparam1<QSModel>("sp_medicos_qs", new { @idhistorico = _elId }).FirstOrDefault();

                var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

                #region encabezado
                //-------------------------------------------------------------------------------------------------------- 1a linea
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

                Chunk chkSub = new Chunk("Química Sanguínea", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
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

                docRepAll.Add(tblEncabezado);

                #endregion

                #region emision - revision - codigo
                Chunk chkemision = new Chunk("EMISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision = new Paragraph();
                paragraphemision.Alignment = Element.ALIGN_CENTER;
                paragraphemision.Add(chkemision);

                PdfPCell clEmision = new PdfPCell();
                clEmision.BorderWidth = 0;
                clEmision.AddElement(paragraphemision);

                Chunk chkrevision = new Chunk("REVISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision = new Paragraph();
                paragrarevision.Alignment = Element.ALIGN_CENTER;
                paragrarevision.Add(chkrevision);

                PdfPCell clrevision = new PdfPCell();
                clrevision.BorderWidth = 0;
                clrevision.AddElement(paragrarevision);

                Chunk chkcodigo = new Chunk("CODIGO", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo = new Paragraph();
                paragracodigo.Alignment = Element.ALIGN_CENTER;
                paragracodigo.Add(chkcodigo);

                PdfPCell clcodigo = new PdfPCell();
                clcodigo.BorderWidth = 0;
                clcodigo.AddElement(paragracodigo);

                Chunk chkemision_b = new Chunk(DateTime.Now.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision_b = new Paragraph();
                paragraphemision_b.Alignment = Element.ALIGN_CENTER;
                paragraphemision_b.Add(chkemision_b);

                PdfPCell clEmision_b = new PdfPCell();
                clEmision_b.BorderWidth = 0;
                clEmision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clEmision_b.UseAscender = true;
                clEmision_b.AddElement(paragraphemision_b);

                Chunk chkrevision_b = new Chunk("1.1", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision_b = new Paragraph();
                paragrarevision_b.Alignment = Element.ALIGN_CENTER;
                paragrarevision_b.Add(chkrevision_b);

                PdfPCell clrevision_b = new PdfPCell();
                clrevision_b.BorderWidth = 0;
                clrevision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clrevision_b.UseAscender = true;
                clrevision_b.AddElement(paragrarevision_b);

                Chunk chkcodigo_b = new Chunk("CECCC/DMT/07", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo_b = new Paragraph();
                paragracodigo_b.Alignment = Element.ALIGN_CENTER;
                paragracodigo_b.Add(chkcodigo_b);

                PdfPCell clcodigo_b = new PdfPCell();
                clcodigo_b.BorderWidth = 0;
                clcodigo_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clcodigo_b.UseAscender = true;
                clcodigo_b.AddElement(paragracodigo_b);

                PdfPTable tblemision = new PdfPTable(3);
                tblemision.WidthPercentage = 100;
                float[] widthsemision = new float[] { 20f, 60f, 20f };
                tblemision.SetWidths(widthsemision);

                tblemision.AddCell(clEmision);
                tblemision.AddCell(clrevision);
                tblemision.AddCell(clcodigo);

                tblemision.AddCell(clEmision_b);
                tblemision.AddCell(clrevision_b);
                tblemision.AddCell(clcodigo_b);

                docRepAll.Add(tblemision);
                #endregion

                #region Titulo Datos personales
                PdfPTable Datospersonales = new PdfPTable(1);
                Datospersonales.TotalWidth = 560f;
                Datospersonales.LockedWidth = true;

                Datospersonales.SetWidths(widthsTitulosGenerales);
                Datospersonales.HorizontalAlignment = 0;
                Datospersonales.SpacingBefore = 10f;
                Datospersonales.SpacingAfter = 10f;

                PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloTituloFamiliar.HorizontalAlignment = 1;
                cellTituloTituloFamiliar.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTituloTituloFamiliar.UseAscender = true;
                cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                Datospersonales.AddCell(cellTituloTituloFamiliar);

                docRepAll.Add(Datospersonales);
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
                tblDatosEvaluado.SpacingAfter = 20f;
                //tblDatosEvaluado.SpacingBefore = 10f;
                tblDatosEvaluado.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------------------------------- 1a linea
                PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
                celTitnombre.BorderWidth = 0;
                celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
                celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos[idQS].evaluado, fontDato));
                celDatoEvaluado.BorderWidth = 0;
                celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
                celTitCodigo.BorderWidth = 0;
                celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos[idQS].codigoevaluado, fontDato));
                celDatoCodigo.BorderWidth = 0;
                celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 2a linea
                PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
                celTitSexo.BorderWidth = 0;
                celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos[idQS].sexo, fontDato));
                celDatoSexo.BorderWidth = 0;
                celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
                celTitEvaluacion.BorderWidth = 0;
                celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos[idQS].evaluacion, fontDato));
                celDatoEvaluacion.BorderWidth = 0;
                celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 3a linea
                PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
                celTitEdad.BorderWidth = 0;
                celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos[idQS].edad.ToString(), fontDato)); ;
                celDatoEdad.BorderWidth = 0;
                celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celTitFolio.BorderWidth = 0;
                celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos[idQS].folio, fontDato));
                celDatoFolio.BorderWidth = 0;
                celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 4a linea
                PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
                celTitCurp.BorderWidth = 0;
                celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos[idQS].curp, fontDato)); ;
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

                PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos[idQS].dependencia, fontDato)) { Colspan = 3 };
                celDatoDependencia.BorderWidth = 0;
                celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 6a linea
                PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
                celTitPuesto.BorderWidth = 0;
                celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
                celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos[idQS].puesto, fontDato)) { Colspan = 3 };
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

                docRepAll.Add(tblDatosEvaluado);

                #endregion

                #region Titulo Datos Estudio Quimico
                PdfPTable TitResultadoQS = new PdfPTable(1);
                TitResultadoQS.TotalWidth = 560f;
                TitResultadoQS.LockedWidth = true;

                TitResultadoQS.SetWidths(widthsTitulosGenerales);
                TitResultadoQS.HorizontalAlignment = 0;
                TitResultadoQS.SpacingBefore = 30f;
                TitResultadoQS.SpacingAfter = 10f;

                PdfPCell cellTituloQS = new PdfPCell(new Phrase("PRUEBA                          CONCENTRACION        UNIDAD      RESULTADO          RANGO REFERENCIA          UNIDAD", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloQS.HorizontalAlignment = 0; //0 - izquierda; 1 - centro; 2 - derecha
                cellTituloQS.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTituloQS.UseAscender = true;
                cellTituloQS.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloQS.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloQS.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                TitResultadoQS.AddCell(cellTituloQS);

                docRepAll.Add(TitResultadoQS);
                #endregion

                #region DatosQuiSan
                PdfPTable DtsQuiSam = new PdfPTable(6)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };

                float[] valQS = new float[6];
                valQS[0] = 110;
                valQS[1] = 100;
                valQS[2] = 65;
                valQS[3] = 70;
                valQS[4] = 150;
                valQS[5] = 65;
                DtsQuiSam.SetWidths(valQS);
                DtsQuiSam.HorizontalAlignment = 0;
                DtsQuiSam.SpacingAfter = 20f;
                DtsQuiSam.DefaultCell.Border = 0;

                //------------------------------------------------------------------------------------Glucosa
                PdfPCell cTitPruebaGlucosa = new PdfPCell(new Phrase("Glucosa", fonEiqueta));
                cTitPruebaGlucosa.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaGlucosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaGlucosa);

                decimal con_Glu = Convert.ToDecimal(datosQS.Glucosa);
                PdfPCell cTitCncentracionGlucosa = new PdfPCell(new Phrase(con_Glu.ToString("F2"), fontDato));
                cTitCncentracionGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionGlucosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionGlucosa);

                PdfPCell cTitUnidadGlucosa = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadGlucosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadGlucosa);

                PdfPCell cTitResultadoGluscosa = new PdfPCell(new Phrase(datosQS.resGlu, fontDato));
                cTitResultadoGluscosa.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoGluscosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoGluscosa);

                PdfPCell cTitRangosGlucosa = new PdfPCell(new Phrase("74 - 106", fontDato));
                cTitRangosGlucosa.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosGlucosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosGlucosa);

                PdfPCell cTitUnidad2Glucosa = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Glucosa.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Glucosa.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Glucosa);

                //------------------------------------------------------------------------------------Acido urico
                PdfPCell cTitPruebaAcido = new PdfPCell(new Phrase("Ácido úrico", fonEiqueta));
                cTitPruebaAcido.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaAcido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaAcido);

                decimal con_Aci = Convert.ToDecimal(datosQS.Acido);
                PdfPCell cTitCncentracionAcido = new PdfPCell(new Phrase(con_Aci.ToString("F2"), fontDato));
                cTitCncentracionAcido.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionAcido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionAcido);

                PdfPCell cTitUnidadAcido = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadAcido.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadAcido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadAcido);

                PdfPCell cTitResultadoAcido = new PdfPCell(new Phrase(datosQS.resAci, fontDato));
                cTitResultadoAcido.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoAcido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoAcido);

                PdfPCell cTitRangosAcido = new PdfPCell(new Phrase(datos[idQS].sexo == "HOMBRE" ? "3.5 - 7.2" : "2.6 - 6.0", fontDato));
                cTitRangosAcido.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosAcido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosAcido);

                PdfPCell cTitUnidad2Acido = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Acido.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Acido.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Acido);

                //------------------------------------------------------------------------------------Colesterol
                PdfPCell cTitPruebaColesterol = new PdfPCell(new Phrase("Colesterol", fonEiqueta));
                cTitPruebaColesterol.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaColesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaColesterol);

                decimal con_Col = Convert.ToDecimal(datosQS.Colesterol);
                PdfPCell cTitCncentracionColesterol = new PdfPCell(new Phrase(con_Col.ToString("F2"), fontDato));
                cTitCncentracionColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionColesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionColesterol);

                PdfPCell cTitUnidadColesterol = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadColesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadColesterol);

                PdfPCell cTitResultadoColesterol = new PdfPCell(new Phrase(datosQS.resCol, fontDato));
                cTitResultadoColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoColesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoColesterol);

                PdfPCell cTitRangosColesterol = new PdfPCell(new Phrase("menor o igual a 200", fontDato));
                cTitRangosColesterol.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosColesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosColesterol);

                PdfPCell cTitUnidad2Colesterol = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Colesterol.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Colesterol.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Colesterol);

                //------------------------------------------------------------------------------------Trigliceridos
                PdfPCell cTitPruebaTrigliceridos = new PdfPCell(new Phrase("Trigliceridos", fonEiqueta));
                cTitPruebaTrigliceridos.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaTrigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaTrigliceridos);

                decimal con_Tri = Convert.ToDecimal(datosQS.Trigliceridos);
                PdfPCell cTitCncentracionTrigliceridos = new PdfPCell(new Phrase(con_Tri.ToString("F2"), fontDato));
                cTitCncentracionTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionTrigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionTrigliceridos);

                PdfPCell cTitUnidadTrigliceridos = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadTrigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadTrigliceridos);

                PdfPCell cTitResultadoTrigliceridos = new PdfPCell(new Phrase(datosQS.resTri, fontDato));
                cTitResultadoTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoTrigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoTrigliceridos);

                PdfPCell cTitRangosTrigliceridos = new PdfPCell(new Phrase("30 - 150", fontDato));
                cTitRangosTrigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosTrigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosTrigliceridos);

                PdfPCell cTitUnidad2Trigliceridos = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Trigliceridos.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Trigliceridos.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Trigliceridos);

                //------------------------------------------------------------------------------------Urea
                PdfPCell cTitPruebaUrea = new PdfPCell(new Phrase("Urea", fonEiqueta));
                cTitPruebaUrea.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaUrea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaUrea);

                decimal con_Ure = Convert.ToDecimal(datosQS.Urea);
                PdfPCell cTitCncentracionUrea = new PdfPCell(new Phrase(con_Ure.ToString("F2"), fontDato));
                cTitCncentracionUrea.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionUrea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionUrea);

                PdfPCell cTitUnidadUrea = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadUrea.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadUrea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadUrea);

                PdfPCell cTitResultadoUrea = new PdfPCell(new Phrase(datosQS.resUre, fontDato));
                cTitResultadoUrea.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoUrea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoUrea);

                PdfPCell cTitRangosUrea = new PdfPCell(new Phrase("15.0 - 38.5", fontDato));
                cTitRangosUrea.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosUrea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosUrea);

                PdfPCell cTitUnidad2Urea = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Urea.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Urea.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Urea);

                //------------------------------------------------------------------------------------Creatinina
                PdfPCell cTitPruebaCreatinina = new PdfPCell(new Phrase("Creatinina", fonEiqueta));
                cTitPruebaCreatinina.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaCreatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaCreatinina);

                decimal con_Cre = Convert.ToDecimal(datosQS.Creatinina);
                PdfPCell cTitCncentracionCreatinina = new PdfPCell(new Phrase(con_Cre.ToString("F2"), fontDato));
                cTitCncentracionCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionCreatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionCreatinina);

                PdfPCell cTitUnidadCreatinina = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadCreatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadCreatinina);

                PdfPCell cTitResultadoCreatinina = new PdfPCell(new Phrase(datosQS.resCre, fontDato));
                cTitResultadoCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoCreatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoCreatinina);

                PdfPCell cTitRangosCreatinina = new PdfPCell(new Phrase(datos[idQS].sexo == "HOMBRE" ? "0.8 - 1.3" : "0.55 - 1.0", fontDato));
                cTitRangosCreatinina.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosCreatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosCreatinina);

                PdfPCell cTitUnidad2Creatinina = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2Creatinina.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2Creatinina.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2Creatinina);

                //------------------------------------------------------------------------------------Colesterol Alta
                PdfPCell cTitPruebaColesterolHDL = new PdfPCell(new Phrase("Colesterol Alta (HDL)", fonEiqueta));
                cTitPruebaColesterolHDL.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaColesterolHDL);

                decimal con_HDL = Convert.ToDecimal(datosQS.colesterolAlta);
                PdfPCell cTitCncentracionColesterolHDL = new PdfPCell(new Phrase(con_HDL.ToString("F2"), fontDato));
                cTitCncentracionColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionColesterolHDL);

                PdfPCell cTitUnidadColesterolHDL = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadColesterolHDL);

                PdfPCell cTitResultadoColesterolHDL = new PdfPCell(new Phrase(datosQS.resColAlt, fontDato));
                cTitResultadoColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoColesterolHDL);

                PdfPCell cTitRangosColesterolHDL = new PdfPCell(new Phrase("40 - 60", fontDato));
                cTitRangosColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosColesterolHDL);

                PdfPCell cTitUnidad2ColesterolHDL = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2ColesterolHDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2ColesterolHDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2ColesterolHDL);

                //------------------------------------------------------------------------------------Colesterol Baja
                PdfPCell cTitPruebaColesterolLDL = new PdfPCell(new Phrase("Colesterol Baja (LDL)", fonEiqueta));
                cTitPruebaColesterolLDL.HorizontalAlignment = Element.ALIGN_LEFT;
                cTitPruebaColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitPruebaColesterolLDL);

                decimal con_LDL = Convert.ToDecimal(datosQS.colesterolBaja);
                PdfPCell cTitCncentracionColesterolLDL = new PdfPCell(new Phrase(con_LDL.ToString("F2"), fontDato));
                cTitCncentracionColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitCncentracionColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitCncentracionColesterolLDL);

                PdfPCell cTitUnidadColesterolLDL = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidadColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidadColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidadColesterolLDL);

                PdfPCell cTitResultadoColesterolLDL = new PdfPCell(new Phrase(datosQS.resColBaj, fontDato));
                cTitResultadoColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitResultadoColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitResultadoColesterolLDL);

                PdfPCell cTitRangosColesterolLDL = new PdfPCell(new Phrase("menor a 159", fontDato));
                cTitRangosColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitRangosColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitRangosColesterolLDL);

                PdfPCell cTitUnidad2ColesterolLDL = new PdfPCell(new Phrase("mg/dL", fontDato));
                cTitUnidad2ColesterolLDL.HorizontalAlignment = Element.ALIGN_CENTER;
                cTitUnidad2ColesterolLDL.Border = PdfPCell.NO_BORDER;
                DtsQuiSam.AddCell(cTitUnidad2ColesterolLDL);

                docRepAll.Add(DtsQuiSam);

                #endregion

                #region extra
                Paragraph metodo = new Paragraph();
                metodo.Add(new Phrase("Metodología:", fonEiqueta));
                metodo.Add(Chunk.TABBING);
                metodo.Add(new Phrase(datosQS.metodologia, fontDato));
                metodo.Add(Chunk.NEWLINE);

                metodo.Add(new Phrase("Espécimen:", fonEiqueta));
                metodo.Add(Chunk.TABBING);
                metodo.Add(new Phrase("Suero", fontDato));
                metodo.Add(Chunk.NEWLINE);

                metodo.Add(new Phrase("Observaciones:", fonEiqueta));
                metodo.Add(Chunk.TABBING);
                metodo.Add(new Phrase(datosQS.Observacion, fontDato));
                metodo.Add(Chunk.NEWLINE);

                docRepAll.Add(metodo);
                #endregion

                #region firmas
                PdfPTable tblFirmas = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] sizeFirmas = new float[3];
                sizeFirmas[0] = 180;
                sizeFirmas[1] = 200;
                sizeFirmas[2] = 180;
                tblFirmas.SetWidths(sizeFirmas);
                tblFirmas.SpacingBefore = 200f;
                tblFirmas.DefaultCell.Border = 0;

                PdfPCell celFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celFolio.BorderWidth = 0;
                celFolio.BorderWidthTop = 0.75f;
                celFolio.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo = new PdfPCell(new Phrase(datosQS.realizo, fontDato));
                celNombreRealizo.BorderWidth = 0;
                celNombreRealizo.BorderWidthTop = 0.75f;
                celNombreRealizo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable = new PdfPCell(new Phrase(datosQS.superviso, fontDato));
                celNombreResponsable.BorderWidth = 0;
                celNombreResponsable.BorderWidthTop = 0.75f;
                celNombreResponsable.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_b = new PdfPCell(new Phrase(datosQS.FOLIO, fontDato));
                celFolio_b.BorderWidth = 0;
                celFolio_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_b = new PdfPCell(new Phrase(datosQS.ced_rea, fontDato));
                celNombreRealizo_b.BorderWidth = 0;
                celNombreRealizo_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_b = new PdfPCell(new Phrase(datosQS.ced_sup, fontDato));
                celNombreResponsable_b.BorderWidth = 0;
                celNombreResponsable_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_c = new PdfPCell(new Phrase("", fontDato));
                celFolio_c.BorderWidth = 0;
                celFolio_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_c = new PdfPCell(new Phrase("Realizó", fonEiqueta));
                celNombreRealizo_c.BorderWidth = 0;
                celNombreRealizo_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_c = new PdfPCell(new Phrase("Responsable Sanitario", fonEiqueta));
                celNombreResponsable_c.BorderWidth = 0;
                celNombreResponsable_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_d = new PdfPCell(new Phrase("Este documento es confidencial no tendrá ningún valor jurídico si presenta tachaduras o enmendaduras.", fontDato)) { Colspan = 3 };
                celFolio_d.BorderWidth = 0;
                celFolio_d.HorizontalAlignment = Element.ALIGN_CENTER;

                tblFirmas.AddCell(celFolio);
                tblFirmas.AddCell(celNombreRealizo);
                tblFirmas.AddCell(celNombreResponsable);

                tblFirmas.AddCell(celFolio_b);
                tblFirmas.AddCell(celNombreRealizo_b);
                tblFirmas.AddCell(celNombreResponsable_b);

                tblFirmas.AddCell(celFolio_c);
                tblFirmas.AddCell(celNombreRealizo_c);
                tblFirmas.AddCell(celNombreResponsable_c);

                tblFirmas.AddCell(celFolio_d);

                docRepAll.Add(tblFirmas);
                #endregion

                docRepAll.NewPage();
            }

            docRepAll.Close();
            byte[] bytesStream = msRepAll.ToArray();
            msRepAll = new MemoryStream();
            msRepAll.Write(bytesStream, 0, bytesStream.Length);
            msRepAll.Position = 0;

            return new FileStreamResult(msRepAll, "application/pdf");
        }

        public IActionResult allTOX(int id, string fechaAll)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = fechaAll }).ToList();

            var _total = datos.Count();

            MemoryStream msRepAll = new MemoryStream();
            Document docRepAll = new Document(PageSize.LETTER, 30f, 20f, 20f, 40f);
            PdfWriter pwRepAll = PdfWriter.GetInstance(docRepAll, msRepAll);
            docRepAll.Open();

            for(int iTox = 0; iTox < _total; iTox++)
            {
                int _elId = datos[iTox].idhistorico;
                var datosTX = repo.Getdosparam1<ToxModel>("sp_medicos_tox", new { @idhistorico = _elId }).FirstOrDefault();
                //var datosTXall = repo.Getdosparam1<ToxModel>("sp_medicos_tox", new { @idhistorico = _elId }).FirstOrDefault();

                var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);

                #region encabezado
                //-------------------------------------------------------------------------------------------------------- 1a linea
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

                Chunk chkTit = new Chunk("CENTRO ESTATAL DE CONTROL DE CONFIANZA CERTIFICADO DEL ESTADO DE CHIAPAS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraph = new Paragraph();
                paragraph.Alignment = Element.ALIGN_CENTER;
                paragraph.Add(chkTit);

                Chunk chkSub = new Chunk("DIRECCIÓN MEDICA Y TOXICOLÓGICA", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraph1 = new Paragraph();
                paragraph1.Alignment = Element.ALIGN_CENTER;
                paragraph1.Add(chkSub);

                Chunk chkSub_b = new Chunk("FORMATO PARA EMISION DE RESULTADOS DE LA EVALUACION TOXICOLOGICA", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraph1_b = new Paragraph();
                paragraph1_b.Alignment = Element.ALIGN_CENTER;
                paragraph1_b.Add(chkSub_b);

                PdfPCell clTitulo = new PdfPCell();
                clTitulo.BorderWidth = 0;
                clTitulo.AddElement(paragraph);

                PdfPCell clSubTit = new PdfPCell();
                clSubTit.BorderWidth = 0;
                clSubTit.AddElement(paragraph1);

                PdfPCell clSubTit_b = new PdfPCell();
                clSubTit_b.BorderWidth = 0;
                clSubTit_b.AddElement(paragraph1_b);

                PdfPTable tblTitulo = new PdfPTable(1);
                tblTitulo.WidthPercentage = 100;
                tblTitulo.AddCell(clTitulo);
                tblTitulo.AddCell(clSubTit);
                tblTitulo.AddCell(clSubTit_b);

                PdfPCell clTablaTitulo = new PdfPCell();
                clTablaTitulo.BorderWidth = 0;
                clTablaTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
                clTablaTitulo.AddElement(tblTitulo);

                PdfPTable tblEncabezado = new PdfPTable(3);
                tblEncabezado.WidthPercentage = 100;
                float[] widths = new float[] { 15f, 70f, 15f };
                tblEncabezado.SetWidths(widths);

                tblEncabezado.AddCell(clLogoSupIzq);
                tblEncabezado.AddCell(clTablaTitulo);
                tblEncabezado.AddCell(clLogoSupDer);

                docRepAll.Add(tblEncabezado);

                #endregion

                #region emision - revision - codigo
                Chunk chkemision = new Chunk("EMISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision = new Paragraph();
                paragraphemision.Alignment = Element.ALIGN_CENTER;
                paragraphemision.Add(chkemision);

                PdfPCell clEmision = new PdfPCell();
                clEmision.BorderWidth = 0;
                clEmision.AddElement(paragraphemision);

                Chunk chkrevision = new Chunk("REVISION", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision = new Paragraph();
                paragrarevision.Alignment = Element.ALIGN_CENTER;
                paragrarevision.Add(chkrevision);

                PdfPCell clrevision = new PdfPCell();
                clrevision.BorderWidth = 0;
                clrevision.AddElement(paragrarevision);

                Chunk chkcodigo = new Chunk("CODIGO", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo = new Paragraph();
                paragracodigo.Alignment = Element.ALIGN_CENTER;
                paragracodigo.Add(chkcodigo);

                PdfPCell clcodigo = new PdfPCell();
                clcodigo.BorderWidth = 0;
                clcodigo.AddElement(paragracodigo);

                Chunk chkemision_b = new Chunk(DateTime.Now.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragraphemision_b = new Paragraph();
                paragraphemision_b.Alignment = Element.ALIGN_CENTER;
                paragraphemision_b.Add(chkemision_b);

                PdfPCell clEmision_b = new PdfPCell();
                clEmision_b.BorderWidth = 0;
                clEmision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clEmision_b.UseAscender = true;
                clEmision_b.AddElement(paragraphemision_b);

                Chunk chkrevision_b = new Chunk("1.1", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragrarevision_b = new Paragraph();
                paragrarevision_b.Alignment = Element.ALIGN_CENTER;
                paragrarevision_b.Add(chkrevision_b);

                PdfPCell clrevision_b = new PdfPCell();
                clrevision_b.BorderWidth = 0;
                clrevision_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clrevision_b.UseAscender = true;
                clrevision_b.AddElement(paragrarevision_b);

                Chunk chkcodigo_b = new Chunk("CECCC/DMT/04", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                Paragraph paragracodigo_b = new Paragraph();
                paragracodigo_b.Alignment = Element.ALIGN_CENTER;
                paragracodigo_b.Add(chkcodigo_b);

                PdfPCell clcodigo_b = new PdfPCell();
                clcodigo_b.BorderWidth = 0;
                clcodigo_b.VerticalAlignment = Element.ALIGN_MIDDLE;
                clcodigo_b.UseAscender = true;
                clcodigo_b.AddElement(paragracodigo_b);

                PdfPTable tblemision = new PdfPTable(3);
                tblemision.WidthPercentage = 100;
                float[] widthsemision = new float[] { 20f, 60f, 20f };
                tblemision.SetWidths(widthsemision);

                tblemision.AddCell(clEmision);
                tblemision.AddCell(clrevision);
                tblemision.AddCell(clcodigo);

                tblemision.AddCell(clEmision_b);
                tblemision.AddCell(clrevision_b);
                tblemision.AddCell(clcodigo_b);

                docRepAll.Add(tblemision);
                #endregion

                #region Titulo Datos personales
                PdfPTable Datospersonales = new PdfPTable(1);
                Datospersonales.TotalWidth = 560f;
                Datospersonales.LockedWidth = true;

                Datospersonales.SetWidths(widthsTitulosGenerales);
                Datospersonales.HorizontalAlignment = 0;
                Datospersonales.SpacingBefore = 20f;
                Datospersonales.SpacingAfter = 10f;

                PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos personales", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloTituloFamiliar.HorizontalAlignment = 1;
                cellTituloTituloFamiliar.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTituloTituloFamiliar.UseAscender = true;
                cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                Datospersonales.AddCell(cellTituloTituloFamiliar);

                docRepAll.Add(Datospersonales);

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
                tblDatosEvaluado.SpacingAfter = 30f;
                //tblDatosEvaluado.SpacingBefore = 10f;
                tblDatosEvaluado.DefaultCell.Border = 0;

                //-------------------------------------------------------------------------------------------------------- 1a linea
                PdfPCell celTitnombre = new PdfPCell(new Phrase("Nombre", fonEiqueta));
                celTitnombre.BorderWidth = 0;
                celTitnombre.VerticalAlignment = Element.ALIGN_CENTER;
                celTitnombre.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluado = new PdfPCell(new Phrase(datos[iTox].evaluado, fontDato));
                celDatoEvaluado.BorderWidth = 0;
                celDatoEvaluado.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluado.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitCodigo = new PdfPCell(new Phrase("Código", fonEiqueta));
                celTitCodigo.BorderWidth = 0;
                celTitCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCodigo = new PdfPCell(new Phrase(datos[iTox].codigoevaluado, fontDato));
                celDatoCodigo.BorderWidth = 0;
                celDatoCodigo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoCodigo.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 2a linea
                PdfPCell celTitSexo = new PdfPCell(new Phrase("Sexo", fonEiqueta));
                celTitSexo.BorderWidth = 0;
                celTitSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celTitSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoSexo = new PdfPCell(new Phrase(datos[iTox].sexo, fontDato));
                celDatoSexo.BorderWidth = 0;
                celDatoSexo.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoSexo.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitEvaluacion = new PdfPCell(new Phrase("Tipo Evaluación", fonEiqueta));
                celTitEvaluacion.BorderWidth = 0;
                celTitEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEvaluacion = new PdfPCell(new Phrase(datos[iTox].evaluacion, fontDato));
                celDatoEvaluacion.BorderWidth = 0;
                celDatoEvaluacion.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEvaluacion.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 3a linea
                PdfPCell celTitEdad = new PdfPCell(new Phrase("Edad", fonEiqueta));
                celTitEdad.BorderWidth = 0;
                celTitEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celTitEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoEdad = new PdfPCell(new Phrase(datos[iTox].edad.ToString(), fontDato)); ;
                celDatoEdad.BorderWidth = 0;
                celDatoEdad.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoEdad.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celTitFolio.BorderWidth = 0;
                celTitFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celTitFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoFolio = new PdfPCell(new Phrase(datos[iTox].folio, fontDato));
                celDatoFolio.BorderWidth = 0;
                celDatoFolio.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoFolio.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 4a linea
                PdfPCell celTitCurp = new PdfPCell(new Phrase("CURP", fonEiqueta));
                celTitCurp.BorderWidth = 0;
                celTitCurp.VerticalAlignment = Element.ALIGN_CENTER;
                celTitCurp.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoCurp = new PdfPCell(new Phrase(datos[iTox].curp, fontDato)); ;
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

                PdfPCell celDatoDependencia = new PdfPCell(new Phrase(datos[iTox].dependencia, fontDato));
                celDatoDependencia.BorderWidth = 0;
                celDatoDependencia.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoDependencia.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celLugarEva = new PdfPCell(new Phrase("Lugar evaluacion", fonEiqueta));
                celLugarEva.BorderWidth = 0;
                celLugarEva.VerticalAlignment = Element.ALIGN_CENTER;
                celLugarEva.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celLugarEvaDato = new PdfPCell(new Phrase("CECCC", fontDato));
                celLugarEvaDato.BorderWidth = 0;
                celLugarEvaDato.VerticalAlignment = Element.ALIGN_CENTER;
                celLugarEvaDato.HorizontalAlignment = Element.ALIGN_LEFT;

                //-------------------------------------------------------------------------------------------------------- 6a linea
                PdfPCell celTitPuesto = new PdfPCell(new Phrase("Puesto", fonEiqueta));
                celTitPuesto.BorderWidth = 0;
                celTitPuesto.VerticalAlignment = Element.ALIGN_CENTER;
                celTitPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoPuesto = new PdfPCell(new Phrase(datos[iTox].puesto, fontDato));
                celDatoPuesto.BorderWidth = 0;
                celDatoPuesto.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoPuesto.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celTitMuestra = new PdfPCell(new Phrase("Tipo de muestra", fonEiqueta));
                celTitMuestra.BorderWidth = 0;
                celTitMuestra.VerticalAlignment = Element.ALIGN_CENTER;
                celTitMuestra.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell celDatoMuestra = new PdfPCell(new Phrase(datosTX.muestra, fontDato));
                //PdfPCell celDatoMuestra = new PdfPCell(new Phrase("Orina", fontDato));
                celDatoMuestra.BorderWidth = 0;
                celDatoMuestra.VerticalAlignment = Element.ALIGN_CENTER;
                celDatoMuestra.HorizontalAlignment = Element.ALIGN_LEFT;

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
                tblDatosEvaluado.AddCell(celLugarEva);
                tblDatosEvaluado.AddCell(celLugarEvaDato);

                tblDatosEvaluado.AddCell(celTitPuesto);
                tblDatosEvaluado.AddCell(celDatoPuesto);
                tblDatosEvaluado.AddCell(celTitMuestra);
                tblDatosEvaluado.AddCell(celDatoMuestra);

                docRepAll.Add(tblDatosEvaluado);

                #endregion

                #region Titulo Resultados de examen
                PdfPTable TituloResultados = new PdfPTable(1);
                TituloResultados.TotalWidth = 560f;
                TituloResultados.LockedWidth = true;

                TituloResultados.SetWidths(widthsTitulosGenerales);
                TituloResultados.HorizontalAlignment = 0;
                TituloResultados.SpacingBefore = 30f;
                TituloResultados.SpacingAfter = 15f;

                PdfPCell cellTituloTituloRes = new PdfPCell(new Phrase("Registro de resultado de examen toxicológico", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
                cellTituloTituloRes.HorizontalAlignment = 1;
                cellTituloTituloRes.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellTituloTituloRes.UseAscender = true;
                cellTituloTituloRes.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
                cellTituloTituloRes.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                cellTituloTituloRes.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                TituloResultados.AddCell(cellTituloTituloRes);

                docRepAll.Add(TituloResultados);
                #endregion

                #region Datosresultados
                Paragraph fechaHoraProcesamiento = new Paragraph();
                fechaHoraProcesamiento.Alignment = Element.ALIGN_LEFT;
                fechaHoraProcesamiento.Add(new Phrase("Fecha de procesamiento: ", fonEiqueta));
                fechaHoraProcesamiento.Add(Chunk.TABBING);
                fechaHoraProcesamiento.Add(new Phrase(datosTX.fprocesamiento, fontDato));
                fechaHoraProcesamiento.Add(Chunk.TABBING); fechaHoraProcesamiento.Add(Chunk.TABBING); fechaHoraProcesamiento.Add(Chunk.TABBING);
                fechaHoraProcesamiento.Add(new Phrase("Hora de procesamiento: ", fonEiqueta));
                fechaHoraProcesamiento.Add(Chunk.TABBING);
                fechaHoraProcesamiento.Add(new Phrase(datosTX.hprocesamiento, fontDato));
                fechaHoraProcesamiento.Add(Chunk.NEWLINE); fechaHoraProcesamiento.Add(Chunk.NEWLINE);

                docRepAll.Add(fechaHoraProcesamiento);

                Paragraph resultado = new Paragraph();
                resultado.Alignment = Element.ALIGN_CENTER;
                resultado.Add(new Phrase("Resultado", fonEiqueta));
                resultado.Add(Chunk.TABBING); resultado.Add(Chunk.TABBING);
                resultado.Add(new Phrase(datosTX.resultado, fontDato));
                resultado.Add(Chunk.NEWLINE); resultado.Add(Chunk.NEWLINE);

                docRepAll.Add(resultado);

                PdfPTable dtsRes = new PdfPTable(2)
                {
                    TotalWidth = 300,
                    LockedWidth = true
                };

                float[] valTX = new float[2];
                valTX[0] = 150;
                valTX[1] = 150;
                dtsRes.SetWidths(valTX);
                dtsRes.HorizontalAlignment = Element.ALIGN_CENTER;
                dtsRes.SpacingAfter = 30f;
                dtsRes.DefaultCell.Border = 1;

                //--------------------------------------------------------------------Titulos
                PdfPCell cAnalito = new PdfPCell(new Phrase("ANALITO", fonEiqueta));
                cAnalito.HorizontalAlignment = Element.ALIGN_CENTER;
                cAnalito.Border = PdfPCell.NO_BORDER;
                cAnalito.BorderWidthTop = 0.75f;
                dtsRes.AddCell(cAnalito);

                PdfPCell cResultado = new PdfPCell(new Phrase("RESULTADO", fonEiqueta));
                cResultado.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultado.Border = PdfPCell.NO_BORDER;
                cResultado.BorderWidthTop = 0.75f;
                dtsRes.AddCell(cResultado);

                //--------------------------------------------------------------------Mariguana
                PdfPCell cMariguana = new PdfPCell(new Phrase("Marihuana", fonEiqueta));
                cMariguana.HorizontalAlignment = Element.ALIGN_LEFT;
                cMariguana.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cMariguana);

                PdfPCell cResultadoMarihuana = new PdfPCell(new Phrase(datosTX.mariguana, fontDato));
                cResultadoMarihuana.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoMarihuana.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoMarihuana);

                //--------------------------------------------------------------------Cocaína
                PdfPCell cCocaina = new PdfPCell(new Phrase("Cocaína", fonEiqueta));
                cCocaina.HorizontalAlignment = Element.ALIGN_LEFT;
                cCocaina.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cCocaina);

                PdfPCell cResultadoCocaina = new PdfPCell(new Phrase(datosTX.cocaina, fontDato));
                cResultadoCocaina.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoCocaina.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoCocaina);

                //--------------------------------------------------------------------Anfetamina
                PdfPCell cAntetamina = new PdfPCell(new Phrase("Anfetaminas", fonEiqueta));
                cAntetamina.HorizontalAlignment = Element.ALIGN_LEFT;
                cAntetamina.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cAntetamina);

                PdfPCell cResultadoAnfetamina = new PdfPCell(new Phrase(datosTX.anfetaminas, fontDato));
                cResultadoAnfetamina.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoAnfetamina.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoAnfetamina);

                //--------------------------------------------------------------------Benzodiacepinas
                PdfPCell cBenzo = new PdfPCell(new Phrase("Benzodiacepinas", fonEiqueta));
                cBenzo.HorizontalAlignment = Element.ALIGN_LEFT;
                cBenzo.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cBenzo);

                PdfPCell cResultadoBenzo = new PdfPCell(new Phrase(datosTX.benzodiacepinas, fontDato));
                cResultadoBenzo.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoBenzo.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoBenzo);

                //--------------------------------------------------------------------Barbitúricos
                PdfPCell cBarbituricos = new PdfPCell(new Phrase("Barbitúricos", fonEiqueta));
                cBarbituricos.HorizontalAlignment = Element.ALIGN_LEFT;
                cBarbituricos.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cBarbituricos);

                PdfPCell cResultadoBarbituricos = new PdfPCell(new Phrase(datosTX.barbituricos, fontDato));
                cResultadoBarbituricos.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoBarbituricos.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoBarbituricos);

                //--------------------------------------------------------------------Metanfetaminas
                PdfPCell cMetanfetaminas = new PdfPCell(new Phrase("Metanfetaminas", fonEiqueta));
                cMetanfetaminas.HorizontalAlignment = Element.ALIGN_LEFT;
                cMetanfetaminas.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cMetanfetaminas);

                PdfPCell cResultadoMetanfetaminas = new PdfPCell(new Phrase(datosTX.metanfetaminas, fontDato));
                cResultadoMetanfetaminas.HorizontalAlignment = Element.ALIGN_CENTER;
                cResultadoMetanfetaminas.Border = PdfPCell.NO_BORDER;
                dtsRes.AddCell(cResultadoMetanfetaminas);

                docRepAll.Add(dtsRes);

                #endregion

                #region metodologia y observaciones
                Paragraph metObs = new Paragraph();
                metObs.Add(Chunk.NEWLINE);
                metObs.Alignment = Element.ALIGN_LEFT;
                metObs.Add(new Phrase("Metodología utilizada", fonEiqueta));
                metObs.Add(Chunk.TABBING);
                metObs.Add(new Phrase(datosTX.metodo, fontDato));
                metObs.Add(Chunk.NEWLINE); metObs.Add(Chunk.NEWLINE);
                metObs.Add(new Phrase("Observaciones", fonEiqueta));
                metObs.Add(Chunk.NEWLINE); metObs.Add(Chunk.TABBING);
                metObs.Add(new Phrase(datosTX.observacion, fontDato));

                docRepAll.Add(metObs);
                #endregion

                #region firmas
                PdfPTable tblFirmas = new PdfPTable(3)
                {
                    TotalWidth = 560,
                    LockedWidth = true
                };
                float[] sizeFirmas = new float[3];
                sizeFirmas[0] = 180;
                sizeFirmas[1] = 200;
                sizeFirmas[2] = 180;
                tblFirmas.SetWidths(sizeFirmas);
                tblFirmas.SpacingBefore = 80f;
                tblFirmas.DefaultCell.Border = 0;

                PdfPCell celFolio = new PdfPCell(new Phrase("Folio", fonEiqueta));
                celFolio.BorderWidth = 0;
                celFolio.BorderWidthTop = 0.75f;
                celFolio.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo = new PdfPCell(new Phrase(datosTX.realizo, fontDato));
                celNombreRealizo.BorderWidth = 0;
                celNombreRealizo.BorderWidthTop = 0.75f;
                celNombreRealizo.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable = new PdfPCell(new Phrase(datosTX.supervisor, fontDato));
                celNombreResponsable.BorderWidth = 0;
                celNombreResponsable.BorderWidthTop = 0.75f;
                celNombreResponsable.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_b = new PdfPCell(new Phrase(datosTX.folio, fontDato));
                celFolio_b.BorderWidth = 0;
                celFolio_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_b = new PdfPCell(new Phrase(datosTX.ced_realizo, fontDato));
                celNombreRealizo_b.BorderWidth = 0;
                celNombreRealizo_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_b = new PdfPCell(new Phrase(datosTX.ced_superviso, fontDato));
                celNombreResponsable_b.BorderWidth = 0;
                celNombreResponsable_b.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_c = new PdfPCell(new Phrase("", fontDato));
                celFolio_c.BorderWidth = 0;
                celFolio_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreRealizo_c = new PdfPCell(new Phrase("Supervisor Ocular", fonEiqueta));
                celNombreRealizo_c.BorderWidth = 0;
                celNombreRealizo_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celNombreResponsable_c = new PdfPCell(new Phrase("Responsable Sanitario", fonEiqueta));
                celNombreResponsable_c.BorderWidth = 0;
                celNombreResponsable_c.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell celFolio_d = new PdfPCell(new Phrase("ESTE FORMATO ES DE CARACTER RESERVADO", fontDato)) { Colspan = 3 };
                celFolio_d.BorderWidth = 0;
                celFolio_d.HorizontalAlignment = Element.ALIGN_CENTER;

                tblFirmas.AddCell(celFolio);
                tblFirmas.AddCell(celNombreRealizo);
                tblFirmas.AddCell(celNombreResponsable);

                tblFirmas.AddCell(celFolio_b);
                tblFirmas.AddCell(celNombreRealizo_b);
                tblFirmas.AddCell(celNombreResponsable_b);

                tblFirmas.AddCell(celFolio_c);
                tblFirmas.AddCell(celNombreRealizo_c);
                tblFirmas.AddCell(celNombreResponsable_c);

                tblFirmas.AddCell(celFolio_d);

                docRepAll.Add(tblFirmas);
                #endregion

                docRepAll.NewPage();
            }

            docRepAll.Close();
            byte[] bytesStream = msRepAll.ToArray();
            msRepAll = new MemoryStream();
            msRepAll.Write(bytesStream, 0, bytesStream.Length);
            msRepAll.Position = 0;

            return new FileStreamResult(msRepAll, "application/pdf");
        }

        public IActionResult repNutricion(int idHistorico)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_rep_cabeceras", new { @idhistorico = idHistorico }).FirstOrDefault();
            var datosNut = repo.Getdosparam1<MedToxMVC.Models.Nutricion.NutricionModel>("sp_medicos_nutricion_obtener_evaluaciones_a_realizar", new { @idhistorico = idHistorico }).FirstOrDefault();
            var cedulas = repo.Getdosparam1<NutriCedulas>("sp_medicos_nutricion_obtener_nutriologo_supervisor_cedulas", new { @idhistorico = idHistorico }).FirstOrDefault();

            MemoryStream msRepNut = new MemoryStream();

            Document docRepNut = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRepNut = PdfWriter.GetInstance(docRepNut, msRepNut);

            string elFolio = datos.folio.ToString();
            string elRealizo = cedulas.nombreNutrilogo;
            string elCedRea = cedulas.cedNut;
            string elSuperviso = cedulas.nombreSupervisor;
            string elCedSup = cedulas.cedSup;

            string elTitulo = "Historia Clínica de Nutrición";

            pwRepNut.PageEvent = HeaderFooterNutricion.getMultilineFooter(elFolio, elRealizo, elCedRea, elSuperviso, elCedSup, elTitulo);

            docRepNut.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
            var fontFirma = FontFactory.GetFont("Arial", 10, Font.BOLD + Font.UNDERLINE, BaseColor.BLACK);

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

            //PdfPCell celEmi_b = new PdfPCell(new Phrase("2021", fonEiqueta));
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

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/28", fonEiqueta));
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

            docRepNut.Add(tblEmiRevCpd);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos del Evaluado", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloTituloFamiliar.UseAscender = true;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRepNut.Add(Datospersonales);
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
            tblDatosEvaluado.SpacingAfter = 10f;
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

            docRepNut.Add(tblDatosEvaluado);

            #endregion

            #region Titulo Datos Antropometricas
            PdfPTable DatosAntro = new PdfPTable(1);
            DatosAntro.TotalWidth = 560f;
            DatosAntro.LockedWidth = true;

            DatosAntro.SetWidths(widthsTitulosGenerales);
            DatosAntro.HorizontalAlignment = 0;
            //DatosAntro.SpacingBefore = 10f;
            DatosAntro.SpacingAfter = 10f;

            PdfPCell cellTituloTituloAntro = new PdfPCell(new Phrase("Medidas Antropométricas", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloAntro.HorizontalAlignment = 1;
            cellTituloTituloAntro.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloTituloAntro.UseAscender = true;
            cellTituloTituloAntro.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloAntro.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloAntro.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            DatosAntro.AddCell(cellTituloTituloAntro);

            docRepNut.Add(DatosAntro);
            #endregion

            #region Tabla Datos Antropometricas
            PdfPTable tblDatosAntro = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesNut = new float[8];
            valuesNut[0] = 70;
            valuesNut[1] = 70;
            valuesNut[2] = 70;
            valuesNut[3] = 70;
            valuesNut[4] = 70;
            valuesNut[5] = 70;
            valuesNut[6] = 70;
            valuesNut[7] = 70;
            tblDatosAntro.SetWidths(valuesNut);
            tblDatosAntro.HorizontalAlignment = 0;
            tblDatosAntro.SpacingAfter = 10f;
            tblDatosAntro.DefaultCell.Border = 0;

            PdfPCell celPesoActual = new PdfPCell(new Phrase("Peso Actual", fonEiqueta));
            celPesoActual.BorderWidth = 0;
            celPesoActual.VerticalAlignment = Element.ALIGN_CENTER;
            celPesoActual.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoPesoActual = new PdfPCell(new Phrase(datosNut.pesoactual.ToString() + " Kg.", fontDato));
            celDatoPesoActual.BorderWidth = 0;
            celDatoPesoActual.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPesoActual.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTalla = new PdfPCell(new Phrase("Talla", fonEiqueta));
            celTalla.BorderWidth = 0;
            celTalla.VerticalAlignment = Element.ALIGN_CENTER;
            celTalla.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoTalla = new PdfPCell(new Phrase(datosNut.talla.ToString() + " m.", fontDato));
            celDatoTalla.BorderWidth = 0;
            celDatoTalla.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoTalla.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celPesoIdeal = new PdfPCell(new Phrase("Peso Ideal", fonEiqueta));
            celPesoIdeal.BorderWidth = 0;
            celPesoIdeal.VerticalAlignment = Element.ALIGN_CENTER;
            celPesoIdeal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoPesoIdeal = new PdfPCell(new Phrase(datosNut.pesoideal.ToString() + " Kg.", fontDato));
            celDatoPesoIdeal.BorderWidth = 0;
            celDatoPesoIdeal.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoPesoIdeal.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celFrecIMC = new PdfPCell(new Phrase("IMC", fonEiqueta));
            celFrecIMC.BorderWidth = 0;
            celFrecIMC.VerticalAlignment = Element.ALIGN_CENTER;
            celFrecIMC.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosIMC = new PdfPCell(new Phrase(datosNut.imc.ToString(), fontDato));
            celDatosIMC.BorderWidth = 0;
            celDatosIMC.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosIMC.HorizontalAlignment = Element.ALIGN_LEFT;

            tblDatosAntro.AddCell(celPesoActual);
            tblDatosAntro.AddCell(celDatoPesoActual);
            tblDatosAntro.AddCell(celTalla);
            tblDatosAntro.AddCell(celDatoTalla);
            tblDatosAntro.AddCell(celPesoIdeal);
            tblDatosAntro.AddCell(celDatoPesoIdeal);
            tblDatosAntro.AddCell(celFrecIMC);
            tblDatosAntro.AddCell(celDatosIMC);

            docRepNut.Add(tblDatosAntro);
            #endregion

            #region Titulo Signos Vitales
            PdfPTable Datossignos = new PdfPTable(1);
            Datossignos.TotalWidth = 560f;
            Datossignos.LockedWidth = true;

            Datossignos.SetWidths(widthsTitulosGenerales);
            Datossignos.HorizontalAlignment = 0;
            //Datossignos.SpacingBefore = 10f;
            Datossignos.SpacingAfter = 10f;

            PdfPCell cellTituloSignos = new PdfPCell(new Phrase("Signos Vitales", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloSignos.HorizontalAlignment = 1;
            cellTituloSignos.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloSignos.UseAscender = true;
            cellTituloSignos.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloSignos.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloSignos.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datossignos.AddCell(cellTituloSignos);

            docRepNut.Add(Datossignos);
            #endregion

            #region Tabla Datos Signos Vitales
            PdfPTable tblDatosSignos = new PdfPTable(8)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesSig = new float[8];
            valuesSig[0] = 90;
            valuesSig[1] = 90;
            valuesSig[2] = 80;
            valuesSig[3] = 40;
            valuesSig[4] = 90;
            valuesSig[5] = 40;
            valuesSig[6] = 80;
            valuesSig[7] = 50;
            tblDatosSignos.SetWidths(valuesSig);
            tblDatosSignos.HorizontalAlignment = 0;
            tblDatosSignos.SpacingAfter = 10f;
            tblDatosSignos.DefaultCell.Border = 0;

            PdfPCell celPesoTension = new PdfPCell(new Phrase("Tensión Arterial", fonEiqueta));
            celPesoTension.BorderWidth = 0;
            celPesoTension.VerticalAlignment = Element.ALIGN_CENTER;
            celPesoTension.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoTension = new PdfPCell(new Phrase(datosNut.tensionarterial.Trim().ToString() + " mmHg.", fontDato)); ;
            celDatoTension.BorderWidth = 0;
            celDatoTension.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoTension.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celFreCar = new PdfPCell(new Phrase("Frec. Cardiaca", fonEiqueta));
            celFreCar.BorderWidth = 0;
            celFreCar.VerticalAlignment = Element.ALIGN_CENTER;
            celFreCar.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoFrecuenciaCardiaca = new PdfPCell(new Phrase(datosNut.cardiaca.ToString(), fontDato));
            celDatoFrecuenciaCardiaca.BorderWidth = 0;
            celDatoFrecuenciaCardiaca.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoFrecuenciaCardiaca.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celPesoFrecuenciaRespiratoria = new PdfPCell(new Phrase("Frec. Respiratoria", fonEiqueta));
            celPesoFrecuenciaRespiratoria.BorderWidth = 0;
            celPesoFrecuenciaRespiratoria.VerticalAlignment = Element.ALIGN_CENTER;
            celPesoFrecuenciaRespiratoria.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoFrecuenciaRespiratoria = new PdfPCell(new Phrase(datosNut.respiratoria.ToString(), fontDato)); ;
            celDatoFrecuenciaRespiratoria.BorderWidth = 0;
            celDatoFrecuenciaRespiratoria.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoFrecuenciaRespiratoria.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTemperatura = new PdfPCell(new Phrase("Temperatura", fonEiqueta));
            celTemperatura.BorderWidth = 0;
            celTemperatura.VerticalAlignment = Element.ALIGN_CENTER;
            celTemperatura.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoTemperatura = new PdfPCell(new Phrase(datosNut.temperatura.ToString()+ " °C.", fontDato));
            celDatoTemperatura.BorderWidth = 0;
            celDatoTemperatura.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoTemperatura.HorizontalAlignment = Element.ALIGN_LEFT;

            tblDatosSignos.AddCell(celPesoTension);
            tblDatosSignos.AddCell(celDatoTension);
            tblDatosSignos.AddCell(celFreCar);
            tblDatosSignos.AddCell(celDatoFrecuenciaCardiaca);
            tblDatosSignos.AddCell(celPesoFrecuenciaRespiratoria);
            tblDatosSignos.AddCell(celDatoFrecuenciaRespiratoria);
            tblDatosSignos.AddCell(celTemperatura);
            tblDatosSignos.AddCell(celDatoTemperatura);

            docRepNut.Add(tblDatosSignos);
            #endregion

            #region Titulo Habitos alimentarios
            PdfPTable DatosHabitos = new PdfPTable(1);
            DatosHabitos.TotalWidth = 560f;
            DatosHabitos.LockedWidth = true;

            DatosHabitos.SetWidths(widthsTitulosGenerales);
            DatosHabitos.HorizontalAlignment = 0;
            //Datossignos.SpacingBefore = 10f;
            DatosHabitos.SpacingAfter = 10f;

            PdfPCell cellTituloHabitos = new PdfPCell(new Phrase("Hábitos Alimentarios", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloHabitos.HorizontalAlignment = 1;
            cellTituloHabitos.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloHabitos.UseAscender = true;
            cellTituloHabitos.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloHabitos.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloHabitos.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            DatosHabitos.AddCell(cellTituloHabitos);

            docRepNut.Add(DatosHabitos);
            #endregion

            #region Datos habitos
            PdfPTable tblDatosHabitos = new PdfPTable(4)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesHab = new float[4];
            valuesHab[0] = 150;
            valuesHab[1] = 65;
            valuesHab[2] = 280;
            valuesHab[3] = 65;
            tblDatosHabitos.SetWidths(valuesHab);
            tblDatosHabitos.HorizontalAlignment = 0;
            tblDatosHabitos.SpacingAfter = 10f;
            tblDatosHabitos.DefaultCell.Border = 0;

            PdfPCell celDondecome = new PdfPCell(new Phrase("Dónde consume sus alimentos", fonEiqueta)) { Colspan = 4 };
            celDondecome.BorderWidth = 0;
            celDondecome.VerticalAlignment = Element.ALIGN_CENTER;
            celDondecome.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDesayuno = new PdfPCell(new Phrase("Desayuno", fonEiqueta));
            celDesayuno.BorderWidth = 0;
            celDesayuno.VerticalAlignment = Element.ALIGN_CENTER;
            celDesayuno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell delDatoDesayuno = new PdfPCell(new Phrase(datosNut.Hadesayuno, fontDato));
            delDatoDesayuno.BorderWidth = 0;
            delDatoDesayuno.VerticalAlignment = Element.ALIGN_CENTER;
            delDatoDesayuno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHorarioDesayuno = new PdfPCell(new Phrase("Horario", fonEiqueta));
            celHorarioDesayuno.BorderWidth = 0;
            celHorarioDesayuno.VerticalAlignment = Element.ALIGN_CENTER;
            celHorarioDesayuno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosHorarioDesayuno = new PdfPCell(new Phrase(datosNut.Hcdesayuno, fontDato));
            celDatosHorarioDesayuno.BorderWidth = 0;
            celDatosHorarioDesayuno.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosHorarioDesayuno.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celcomida = new PdfPCell(new Phrase("Comida", fonEiqueta));
            celcomida.BorderWidth = 0;
            celcomida.VerticalAlignment = Element.ALIGN_CENTER;
            celcomida.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoComida = new PdfPCell(new Phrase(datosNut.Hacomida, fontDato));
            celDatoComida.BorderWidth = 0;
            celDatoComida.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoComida.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHorarioComida = new PdfPCell(new Phrase("Horario", fonEiqueta));
            celHorarioComida.BorderWidth = 0;
            celHorarioComida.VerticalAlignment = Element.ALIGN_CENTER;
            celHorarioComida.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosHorarioComida = new PdfPCell(new Phrase(datosNut.Hccomida, fontDato));
            celDatosHorarioComida.BorderWidth = 0;
            celDatosHorarioComida.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosHorarioComida.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCena = new PdfPCell(new Phrase("Cena", fonEiqueta));
            celCena.BorderWidth = 0;
            celCena.VerticalAlignment = Element.ALIGN_CENTER;
            celCena.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatoCena = new PdfPCell(new Phrase(datosNut.Hacena, fontDato));
            celDatoCena.BorderWidth = 0;
            celDatoCena.VerticalAlignment = Element.ALIGN_CENTER;
            celDatoCena.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celHorarioCena = new PdfPCell(new Phrase("Horario", fonEiqueta));
            celHorarioCena.BorderWidth = 0;
            celHorarioCena.VerticalAlignment = Element.ALIGN_CENTER;
            celHorarioCena.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosHorarioCena = new PdfPCell(new Phrase(datosNut.Hccena, fontDato));
            celDatosHorarioCena.BorderWidth = 0;
            celDatosHorarioCena.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosHorarioCena.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celBebidAlcoholica = new PdfPCell(new Phrase("Bebidas alcohólicas", fonEiqueta));
            celBebidAlcoholica.BorderWidth = 0;
            celBebidAlcoholica.VerticalAlignment = Element.ALIGN_CENTER;
            celBebidAlcoholica.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosBebidaAlcoholica = new PdfPCell(new Phrase(datosNut.bebidaalcohilica == true ? "Si" : "No", fontDato));
            celDatosBebidaAlcoholica.BorderWidth = 0;
            celDatosBebidaAlcoholica.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosBebidaAlcoholica.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celBebidaAlcoholicaPorMes = new PdfPCell(new Phrase("Número de veces por mes", fonEiqueta));
            celBebidaAlcoholicaPorMes.BorderWidth = 0;
            celBebidaAlcoholicaPorMes.VerticalAlignment = Element.ALIGN_CENTER;
            celBebidaAlcoholicaPorMes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosCantidaddBebidaAlcoholica = new PdfPCell(new Phrase(datosNut.Baveces.ToString(), fontDato));
            celDatosCantidaddBebidaAlcoholica.BorderWidth = 0;
            celDatosCantidaddBebidaAlcoholica.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosCantidaddBebidaAlcoholica.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celBebidEmbotellada = new PdfPCell(new Phrase("Bebidas embotelladas", fonEiqueta));
            celBebidEmbotellada.BorderWidth = 0;
            celBebidEmbotellada.VerticalAlignment = Element.ALIGN_CENTER;
            celBebidEmbotellada.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosBebidaEmbotellada = new PdfPCell(new Phrase(datosNut.bebidaembotellada == true ? "Si" : "No", fontDato));
            celDatosBebidaEmbotellada.BorderWidth = 0;
            celDatosBebidaEmbotellada.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosBebidaEmbotellada.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celBebidaEmbotelladaPorMes = new PdfPCell(new Phrase("Núm. de veces por semana", fonEiqueta));
            celBebidaEmbotelladaPorMes.BorderWidth = 0;
            celBebidaEmbotelladaPorMes.VerticalAlignment = Element.ALIGN_CENTER;
            celBebidaEmbotelladaPorMes.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosCantidaddBebidaEmbotellada = new PdfPCell(new Phrase(datosNut.Beveces.ToString(), fontDato));
            celDatosCantidaddBebidaEmbotellada.BorderWidth = 0;
            celDatosCantidaddBebidaEmbotellada.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosCantidaddBebidaEmbotellada.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celPadecimiento = new PdfPCell(new Phrase("Padecimiento", fonEiqueta));
            celPadecimiento.BorderWidth = 0;
            celPadecimiento.VerticalAlignment = Element.ALIGN_CENTER;
            celPadecimiento.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosPadecimiento = new PdfPCell(new Phrase(datosNut.padecimiento, fontDato)) { Colspan = 3 };
            celDatosPadecimiento.BorderWidth = 0;
            celDatosPadecimiento.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosPadecimiento.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celActividad = new PdfPCell(new Phrase("Realiza actividad física", fonEiqueta));
            celActividad.BorderWidth = 0;
            celActividad.VerticalAlignment = Element.ALIGN_CENTER;
            celActividad.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celDatosActividad = new PdfPCell(new Phrase(datosNut.realizaActividad + " " + datosNut.actividadFisica, fontDato)) { Colspan = 3 };
            celDatosActividad.BorderWidth = 0;
            celDatosActividad.VerticalAlignment = Element.ALIGN_CENTER;
            celDatosActividad.HorizontalAlignment = Element.ALIGN_LEFT;

            tblDatosHabitos.AddCell(celDondecome);

            tblDatosHabitos.AddCell(celDesayuno);
            tblDatosHabitos.AddCell(delDatoDesayuno);
            tblDatosHabitos.AddCell(celHorarioDesayuno);
            tblDatosHabitos.AddCell(celDatosHorarioDesayuno);

            tblDatosHabitos.AddCell(celcomida);
            tblDatosHabitos.AddCell(celDatoComida);
            tblDatosHabitos.AddCell(celHorarioComida);
            tblDatosHabitos.AddCell(celDatosHorarioComida);

            tblDatosHabitos.AddCell(celCena);
            tblDatosHabitos.AddCell(celDatoCena);
            tblDatosHabitos.AddCell(celHorarioCena);
            tblDatosHabitos.AddCell(celDatosHorarioCena);

            tblDatosHabitos.AddCell(celBebidAlcoholica);
            tblDatosHabitos.AddCell(celDatosBebidaAlcoholica);
            tblDatosHabitos.AddCell(celBebidaAlcoholicaPorMes);
            tblDatosHabitos.AddCell(celDatosBebidaAlcoholica);

            tblDatosHabitos.AddCell(celBebidEmbotellada);
            tblDatosHabitos.AddCell(celDatosBebidaEmbotellada);
            tblDatosHabitos.AddCell(celBebidaEmbotelladaPorMes);
            tblDatosHabitos.AddCell(celDatosCantidaddBebidaEmbotellada);

            tblDatosHabitos.AddCell(celPadecimiento);
            tblDatosHabitos.AddCell(celDatosPadecimiento);

            tblDatosHabitos.AddCell(celActividad);
            tblDatosHabitos.AddCell(celDatosActividad);

            docRepNut.Add(tblDatosHabitos);
            #endregion

            #region Titulo Recordatorio 24 horas
            PdfPTable DatosRecordatorio = new PdfPTable(1);
            DatosRecordatorio.TotalWidth = 560f;
            DatosRecordatorio.LockedWidth = true;

            DatosRecordatorio.SetWidths(widthsTitulosGenerales);
            DatosRecordatorio.HorizontalAlignment = 0;
            DatosRecordatorio.SpacingBefore = 10f;
            DatosRecordatorio.SpacingAfter = 10f;

            PdfPCell cellTituloRecordatorios = new PdfPCell(new Phrase("Recordario de 24 horas", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloRecordatorios.HorizontalAlignment = 1;
            cellTituloRecordatorios.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTituloRecordatorios.UseAscender = true;
            cellTituloRecordatorios.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloRecordatorios.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloRecordatorios.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            DatosRecordatorio.AddCell(cellTituloRecordatorios);

            docRepNut.Add(DatosRecordatorio);
            #endregion

            #region Datos Recordatorio
            PdfPTable tblDatosRecordatorio = new PdfPTable(7)
            {
                TotalWidth = 560,
                LockedWidth = true
            };
            float[] valuesRed = new float[7];
            valuesRed[0] = 110;
            valuesRed[1] = 75;
            valuesRed[2] = 75;
            valuesRed[3] = 75;
            valuesRed[4] = 75;
            valuesRed[5] = 75;
            valuesRed[6] = 75;
            tblDatosRecordatorio.SetWidths(valuesRed);
            tblDatosRecordatorio.HorizontalAlignment = 0;
            tblDatosRecordatorio.SpacingAfter = 10f;
            tblDatosRecordatorio.DefaultCell.Border = 0;

            PdfPCell celAlimento = new PdfPCell(new Phrase("Alimento", fonEiqueta)) { Rowspan = 2 };
            celAlimento.BorderWidth = 1;
            celAlimento.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAlimento.UseAscender = true;
            celAlimento.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celDesayunoRec = new PdfPCell(new Phrase("DESAYUNO", fonEiqueta)) { Colspan = 2 };
            celDesayunoRec.BorderWidth = 1;
            celDesayunoRec.VerticalAlignment = Element.ALIGN_MIDDLE;
            celDesayunoRec.UseAscender = true;
            celDesayunoRec.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celComidaRec = new PdfPCell(new Phrase("COMIDA", fonEiqueta)) { Colspan = 2 };
            celComidaRec.BorderWidth = 1;
            celComidaRec.VerticalAlignment = Element.ALIGN_MIDDLE;
            celComidaRec.UseAscender = true;
            celComidaRec.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCenaRec = new PdfPCell(new Phrase("CENA", fonEiqueta)) { Colspan = 2 };
            celCenaRec.BorderWidth = 1;
            celCenaRec.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCenaRec.UseAscender = true;
            celCenaRec.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celDesCan = new PdfPCell(new Phrase("CANTIDAD", fonEiqueta));
            celDesCan.BorderWidth = 1;
            celDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celDesCan.UseAscender = true;
            celDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celDesCaL = new PdfPCell(new Phrase("CALIDAD", fonEiqueta));
            celDesCaL.BorderWidth = 1;
            celDesCaL.VerticalAlignment = Element.ALIGN_MIDDLE;
            celDesCaL.UseAscender = true;
            celDesCaL.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celComCan = new PdfPCell(new Phrase("CANTIDAD", fonEiqueta));
            celComCan.BorderWidth = 1;
            celComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celComCan.UseAscender = true;
            celComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celComCaL = new PdfPCell(new Phrase("CALIDAD", fonEiqueta));
            celComCaL.BorderWidth = 1;
            celComCaL.VerticalAlignment = Element.ALIGN_MIDDLE;
            celComCaL.UseAscender = true;
            celComCaL.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCenCan = new PdfPCell(new Phrase("CANTIDAD", fonEiqueta));
            celCenCan.BorderWidth = 1;
            celCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCenCan.UseAscender = true;
            celCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCenCaL = new PdfPCell(new Phrase("CALIDAD", fonEiqueta));
            celCenCaL.BorderWidth = 1;
            celCenCaL.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCenCaL.UseAscender = true;
            celCenCaL.HorizontalAlignment = Element.ALIGN_CENTER;

            //Frijol
            PdfPCell celFrijol = new PdfPCell(new Phrase("FRIJOL", fonEiqueta));
            celFrijol.BorderWidth = 0;
            celFrijol.BorderWidthBottom = 0;
            celFrijol.BorderWidthLeft = 1;
            celFrijol.BorderWidthRight = 1;
            celFrijol.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijol.UseAscender = true;
            celFrijol.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celFrijolDesCan = new PdfPCell(new Phrase(datosNut.frijolCantDesayuno.ToString(), fontDato));
            celFrijolDesCan.BorderWidth = 0;
            celFrijolDesCan.BorderWidthRight = 1;
            celFrijolDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolDesCan.UseAscender = true;
            celFrijolDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrijolDesCal= new PdfPCell(new Phrase((datosNut.frijolCantDesayuno * 120).ToString(), fontDato));
            celFrijolDesCal.BorderWidth = 0;
            celFrijolDesCal.BorderWidthRight = 1;
            celFrijolDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolDesCal.UseAscender = true;
            celFrijolDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrijolComCan = new PdfPCell(new Phrase(datosNut.frijolCantComida.ToString(), fontDato));
            celFrijolComCan.BorderWidth = 0;
            celFrijolComCan.BorderWidthRight = 1;
            celFrijolComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolComCan.UseAscender = true;
            celFrijolComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrijolComCal = new PdfPCell(new Phrase((datosNut.frijolCantComida * 120).ToString(), fontDato));
            celFrijolComCal.BorderWidth = 0;
            celFrijolComCal.BorderWidthRight = 1;
            celFrijolComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolComCal.UseAscender = true;
            celFrijolComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrijolCenCan = new PdfPCell(new Phrase(datosNut.frijolCantCena.ToString(), fontDato));
            celFrijolCenCan.BorderWidth = 0;
            celFrijolCenCan.BorderWidthRight = 1;
            celFrijolCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolCenCan.UseAscender = true;
            celFrijolCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrijolCenCal = new PdfPCell(new Phrase((datosNut.frijolCantCena * 120).ToString(), fontDato));
            celFrijolCenCal.BorderWidth = 0;
            celFrijolCenCal.BorderWidthRight = 1;
            celFrijolCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrijolCenCal.UseAscender = true;
            celFrijolCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            //Tortilla
            PdfPCell celTortilla = new PdfPCell(new Phrase("TORTILLA", fonEiqueta));
            celTortilla.BorderWidth = 0;
            celTortilla.BorderWidthLeft = 1;
            celTortilla.BorderWidthRight = 1;
            celTortilla.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortilla.UseAscender = true;
            celTortilla.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTortillaDesCan = new PdfPCell(new Phrase(datosNut.toritillaCantDesayuno.ToString(), fontDato));
            celTortillaDesCan.BorderWidth = 0;
            celTortillaDesCan.BorderWidthRight = 1;
            celTortillaDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaDesCan.UseAscender = true;
            celTortillaDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTortillaDesCal = new PdfPCell(new Phrase((datosNut.toritillaCantDesayuno * 70).ToString(), fontDato));
            celTortillaDesCal.BorderWidth = 0;
            celTortillaDesCal.BorderWidthRight = 1;
            celTortillaDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaDesCal.UseAscender = true;
            celTortillaDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTortillaComCan = new PdfPCell(new Phrase(datosNut.tortillaCantComida.ToString(), fontDato));
            celTortillaComCan.BorderWidth = 0;
            celTortillaComCan.BorderWidthRight = 1;
            celTortillaComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaComCan.UseAscender = true;
            celTortillaComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTortillaComCal = new PdfPCell(new Phrase((datosNut.tortillaCantComida * 70).ToString(), fontDato));
            celTortillaComCal.BorderWidth = 0;
            celTortillaComCal.BorderWidthRight = 1;
            celTortillaComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaComCal.UseAscender = true;
            celTortillaComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTortillaCenCan = new PdfPCell(new Phrase(datosNut.torillaCantCena.ToString(), fontDato));
            celTortillaCenCan.BorderWidth = 0;
            celTortillaCenCan.BorderWidthRight = 1;
            celTortillaCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaCenCan.UseAscender = true;
            celTortillaCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTortillaCenCal = new PdfPCell(new Phrase((datosNut.torillaCantCena * 70).ToString(), fontDato));
            celTortillaCenCal.BorderWidth = 0;
            celTortillaCenCal.BorderWidthRight = 1;
            celTortillaCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTortillaCenCal.UseAscender = true;
            celTortillaCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            //CQH
            PdfPCell celCQH = new PdfPCell(new Phrase("C. Q. H.", fonEiqueta));
            celCQH.BorderWidth = 0;
            celCQH.BorderWidthLeft = 1;
            celCQH.BorderWidthRight = 1;
            celCQH.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQH.UseAscender = true;
            celCQH.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celCQHDesCan = new PdfPCell(new Phrase(datosNut.CQHCantDesayuno.ToString(), fontDato));
            celCQHDesCan.BorderWidth = 0;
            celCQHDesCan.BorderWidthRight = 1;
            celCQHDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHDesCan.UseAscender = true;
            celCQHDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCQHDesCal = new PdfPCell(new Phrase((datosNut.CQHCantDesayuno * 90).ToString(), fontDato));
            celCQHDesCal.BorderWidth = 0;
            celCQHDesCal.BorderWidthRight = 1;
            celCQHDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHDesCal.UseAscender = true;
            celCQHDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCQHComCan = new PdfPCell(new Phrase(datosNut.CQHCantComida.ToString(), fontDato));
            celCQHComCan.BorderWidth = 0;
            celCQHComCan.BorderWidthRight = 1;
            celCQHComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHComCan.UseAscender = true;
            celCQHComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCQHComCal = new PdfPCell(new Phrase((datosNut.CQHCantComida * 90).ToString(), fontDato));
            celCQHComCal.BorderWidth = 0;
            celCQHComCal.BorderWidthRight = 1;
            celCQHComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHComCal.UseAscender = true;
            celCQHComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCQHCenCan = new PdfPCell(new Phrase(datosNut.CQHCantCena.ToString(), fontDato));
            celCQHCenCan.BorderWidth = 0;
            celCQHCenCan.BorderWidthRight = 1;
            celCQHCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHCenCan.UseAscender = true;
            celCQHCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celCQHCenCal = new PdfPCell(new Phrase((datosNut.CQHCantCena * 90).ToString(), fontDato));
            celCQHCenCal.BorderWidth = 0;
            celCQHCenCal.BorderWidthRight = 1;
            celCQHCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celCQHCenCal.UseAscender = true;
            celCQHCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            // FRUTAS
            PdfPCell celFrutas = new PdfPCell(new Phrase("FRUTAS", fonEiqueta));
            celFrutas.BorderWidth = 0;
            celFrutas.BorderWidthLeft = 1;
            celFrutas.BorderWidthRight = 1;
            celFrutas.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutas.UseAscender = true;
            celFrutas.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celFrutasDesCan = new PdfPCell(new Phrase(datosNut.frutaCantDesayuno.ToString(), fontDato));
            celFrutasDesCan.BorderWidth = 0;
            celFrutasDesCan.BorderWidthRight = 1;
            celFrutasDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasDesCan.UseAscender = true;
            celFrutasDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrutasDesCal = new PdfPCell(new Phrase((datosNut.frutaCantDesayuno * 60).ToString(), fontDato));
            celFrutasDesCal.BorderWidth = 0;
            celFrutasDesCal.BorderWidthRight = 1;
            celFrutasDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasDesCal.UseAscender = true;
            celFrutasDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrutasComCan = new PdfPCell(new Phrase(datosNut.frutaCantComida.ToString(), fontDato));
            celFrutasComCan.BorderWidth = 0;
            celFrutasComCan.BorderWidthRight = 1;
            celFrutasComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasComCan.UseAscender = true;
            celFrutasComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrutasComCal = new PdfPCell(new Phrase((datosNut.frutaCantComida * 60).ToString(), fontDato));
            celFrutasComCal.BorderWidth = 0;
            celFrutasComCal.BorderWidthRight = 1;
            celFrutasComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasComCal.UseAscender = true;
            celFrutasComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrutasCenCan = new PdfPCell(new Phrase(datosNut.frutaCantCena.ToString(), fontDato));
            celFrutasCenCan.BorderWidth = 0;
            celFrutasCenCan.BorderWidthRight = 1;
            celFrutasCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasCenCan.UseAscender = true;
            celFrutasCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celFrutasCenCal = new PdfPCell(new Phrase((datosNut.frutaCantCena * 60).ToString(), fontDato));
            celFrutasCenCal.BorderWidth = 0;
            celFrutasCenCal.BorderWidthRight = 1;
            celFrutasCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celFrutasCenCal.UseAscender = true;
            celFrutasCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            // VERDURAS
            PdfPCell celVerduras = new PdfPCell(new Phrase("VERDURAS", fonEiqueta));
            celVerduras.BorderWidth = 0;
            celVerduras.BorderWidthLeft = 1;
            celVerduras.BorderWidthRight = 1;
            celVerduras.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerduras.UseAscender = true;
            celVerduras.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celVerdurasDesCan = new PdfPCell(new Phrase(datosNut.verduraCantDesayuno.ToString(), fontDato));
            celVerdurasDesCan.BorderWidth = 0;
            celVerdurasDesCan.BorderWidthRight = 1;
            celVerdurasDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasDesCan.UseAscender = true;
            celVerdurasDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celVerdurasDesCal = new PdfPCell(new Phrase((datosNut.verduraCantDesayuno * 25).ToString(), fontDato));
            celVerdurasDesCal.BorderWidth = 0;
            celVerdurasDesCal.BorderWidthRight = 1;
            celVerdurasDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasDesCal.UseAscender = true;
            celVerdurasDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celVerdurasComCan = new PdfPCell(new Phrase(datosNut.verdurasCantComida.ToString(), fontDato));
            celVerdurasComCan.BorderWidth = 0;
            celVerdurasComCan.BorderWidthRight = 1;
            celVerdurasComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasComCan.UseAscender = true;
            celVerdurasComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celVerdurasComCal = new PdfPCell(new Phrase((datosNut.verdurasCantComida * 25).ToString(), fontDato));
            celVerdurasComCal.BorderWidth = 0;
            celVerdurasComCal.BorderWidthRight = 1;
            celVerdurasComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasComCal.UseAscender = true;
            celVerdurasComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celVerdurasCenCan = new PdfPCell(new Phrase(datosNut.verduraCantCena.ToString(), fontDato));
            celVerdurasCenCan.BorderWidth = 0;
            celVerdurasCenCan.BorderWidthRight = 1;
            celVerdurasCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasCenCan.UseAscender = true;
            celVerdurasCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celVerdurasCenCal = new PdfPCell(new Phrase((datosNut.verduraCantCena * 25).ToString(), fontDato));
            celVerdurasCenCal.BorderWidth = 0;
            celVerdurasCenCal.BorderWidthRight = 1;
            celVerdurasCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celVerdurasCenCal.UseAscender = true;
            celVerdurasCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            // AZUCARES
            PdfPCell celAzucares = new PdfPCell(new Phrase("AZUCARES", fonEiqueta));
            celAzucares.BorderWidth = 0;
            celAzucares.BorderWidthLeft = 1;
            celAzucares.BorderWidthRight = 1;
            celAzucares.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucares.UseAscender = true;
            celAzucares.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celAzucaresDesCan = new PdfPCell(new Phrase(datosNut.azucaresCantDesayuno.ToString(), fontDato));
            celAzucaresDesCan.BorderWidth = 0;
            celAzucaresDesCan.BorderWidthRight = 1;
            celAzucaresDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresDesCan.UseAscender = true;
            celAzucaresDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celAzucaresDesCal = new PdfPCell(new Phrase((datosNut.azucaresCantDesayuno * 45).ToString(), fontDato));
            celAzucaresDesCal.BorderWidth = 0;
            celAzucaresDesCal.BorderWidthRight = 1;
            celAzucaresDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresDesCal.UseAscender = true;
            celAzucaresDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celAzucaresComCan = new PdfPCell(new Phrase(datosNut.azucaresCantComida.ToString(), fontDato));
            celAzucaresComCan.BorderWidth = 0;
            celAzucaresComCan.BorderWidthRight = 1;
            celAzucaresComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresComCan.UseAscender = true;
            celAzucaresComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celAzucaresComCal = new PdfPCell(new Phrase((datosNut.azucaresCantComida * 45).ToString(), fontDato));
            celAzucaresComCal.BorderWidth = 0;
            celAzucaresComCal.BorderWidthRight = 1;
            celAzucaresComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresComCal.UseAscender = true;
            celAzucaresComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celAzucaresCenCan = new PdfPCell(new Phrase(datosNut.azucaresCantCena.ToString(), fontDato));
            celAzucaresCenCan.BorderWidth = 0;
            celAzucaresCenCan.BorderWidthRight = 1;
            celAzucaresCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresCenCan.UseAscender = true;
            celAzucaresCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celAzucaresCenCal = new PdfPCell(new Phrase((datosNut.azucaresCantCena * 45).ToString(), fontDato));
            celAzucaresCenCal.BorderWidth = 0;
            celAzucaresCenCal.BorderWidthRight = 1;
            celAzucaresCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celAzucaresCenCal.UseAscender = true;
            celAzucaresCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            // GRASAS
            PdfPCell celGrasas = new PdfPCell(new Phrase("GRASAS", fonEiqueta));
            celGrasas.BorderWidth = 0;
            celGrasas.BorderWidthLeft = 1;
            celGrasas.BorderWidthRight = 1;
            celGrasas.BorderWidthBottom = 1;
            celGrasas.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasas.UseAscender = true;
            celGrasas.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celGrasasDesCan = new PdfPCell(new Phrase(datosNut.grasaCantDesayuno.ToString(), fontDato));
            celGrasasDesCan.BorderWidth = 0;
            celGrasasDesCan.BorderWidthRight = 1;
            celGrasasDesCan.BorderWidthBottom = 1;
            celGrasasDesCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasDesCan.UseAscender = true;
            celGrasasDesCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celGrasasDesCal = new PdfPCell(new Phrase((datosNut.grasaCantDesayuno * 40).ToString(), fontDato));
            celGrasasDesCal.BorderWidth = 0;
            celGrasasDesCal.BorderWidthRight = 1;
            celGrasasDesCal.BorderWidthBottom = 1;
            celGrasasDesCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasDesCal.UseAscender = true;
            celGrasasDesCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celGrasasComCan = new PdfPCell(new Phrase(datosNut.GrasaCantComida.ToString(), fontDato));
            celGrasasComCan.BorderWidth = 0;
            celGrasasComCan.BorderWidthRight = 1;
            celGrasasComCan.BorderWidthBottom = 1;
            celGrasasComCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasComCan.UseAscender = true;
            celGrasasComCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celGrasasComCal = new PdfPCell(new Phrase((datosNut.GrasaCantComida * 40).ToString(), fontDato));
            celGrasasComCal.BorderWidth = 0;
            celGrasasComCal.BorderWidthRight = 1;
            celGrasasComCal.BorderWidthBottom = 1;
            celGrasasComCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasComCal.UseAscender = true;
            celGrasasComCal.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celGrasasCenCan = new PdfPCell(new Phrase(datosNut.grasaCantCena.ToString(), fontDato));
            celGrasasCenCan.BorderWidth = 0;
            celGrasasCenCan.BorderWidthRight = 1;
            celGrasasCenCan.BorderWidthBottom = 1;
            celGrasasCenCan.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasCenCan.UseAscender = true;
            celGrasasCenCan.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celGrasasCenCal = new PdfPCell(new Phrase((datosNut.grasaCantCena * 40).ToString(), fontDato));
            celGrasasCenCal.BorderWidth = 0;
            celGrasasCenCal.BorderWidthRight = 1;
            celGrasasCenCal.BorderWidthBottom = 1;
            celGrasasCenCal.VerticalAlignment = Element.ALIGN_MIDDLE;
            celGrasasCenCal.UseAscender = true;
            celGrasasCenCal.HorizontalAlignment = Element.ALIGN_CENTER;

            //Totales
            PdfPCell celTotalCel1 = new PdfPCell(new Phrase("", fonEiqueta));
            celTotalCel1.BorderWidth = 0;
            celTotalCel1.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel1.UseAscender = true;
            celTotalCel1.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell celTotalCel2 = new PdfPCell(new Phrase("Total", fonEiqueta));
            celTotalCel2.BorderWidth = 0;
            celTotalCel2.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel2.UseAscender = true;
            celTotalCel2.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTotalCel3 = new PdfPCell(new Phrase(((datosNut.frijolCantDesayuno * 120) + (datosNut.toritillaCantDesayuno * 70) + (datosNut.CQHCantDesayuno * 90) + (datosNut.frutaCantDesayuno * 60) + (datosNut.verduraCantDesayuno * 25) + (datosNut.azucaresCantDesayuno * 45) + (datosNut.grasaCantDesayuno * 40)).ToString(), fontDato));
            celTotalCel3.BorderWidth = 0;
            celTotalCel3.BorderWidthBottom = 1;
            celTotalCel3.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel3.UseAscender = true;
            celTotalCel3.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTotalCel4 = new PdfPCell(new Phrase("Total", fontDato));
            celTotalCel4.BorderWidth = 0;
            celTotalCel4.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel4.UseAscender = true;
            celTotalCel4.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTotalCel5 = new PdfPCell(new Phrase(((datosNut.frijolCantComida * 120) + (datosNut.tortillaCantComida * 70) + (datosNut.CQHCantComida * 90) + (datosNut.frutaCantComida * 60) + (datosNut.verdurasCantComida* 25) + (datosNut.azucaresCantComida * 45) + (datosNut.GrasaCantComida * 40)).ToString(), fontDato));
            celTotalCel5.BorderWidth = 0;
            celTotalCel5.BorderWidthBottom = 1;
            celTotalCel5.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel5.UseAscender = true;
            celTotalCel5.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTotalCel6 = new PdfPCell(new Phrase("Total", fontDato));
            celTotalCel6.BorderWidth = 0;
            celTotalCel6.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel6.UseAscender = true;
            celTotalCel6.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell celTotalCel7 = new PdfPCell(new Phrase(((datosNut.frijolCantCena * 120) + (datosNut.torillaCantCena * 70) + (datosNut.CQHCantCena * 90) + (datosNut.frutaCantCena * 60) + (datosNut.verduraCantCena * 25) + (datosNut.azucaresCantCena * 45) + (datosNut.grasaCantCena * 40)).ToString(), fontDato));
            celTotalCel7.BorderWidth = 0;
            celTotalCel7.BorderWidthBottom = 1;
            celTotalCel7.VerticalAlignment = Element.ALIGN_MIDDLE;
            celTotalCel7.UseAscender = true;
            celTotalCel7.HorizontalAlignment = Element.ALIGN_CENTER;

            tblDatosRecordatorio.AddCell(celAlimento);
            tblDatosRecordatorio.AddCell(celDesayunoRec);
            tblDatosRecordatorio.AddCell(celComidaRec);
            tblDatosRecordatorio.AddCell(celCenaRec);

            tblDatosRecordatorio.AddCell(celDesCan);
            tblDatosRecordatorio.AddCell(celDesCaL);
            tblDatosRecordatorio.AddCell(celComCan);
            tblDatosRecordatorio.AddCell(celComCaL);
            tblDatosRecordatorio.AddCell(celCenCan);
            tblDatosRecordatorio.AddCell(celCenCaL);

            tblDatosRecordatorio.AddCell(celFrijol);
            tblDatosRecordatorio.AddCell(celFrijolDesCan);
            tblDatosRecordatorio.AddCell(celFrijolDesCal);
            tblDatosRecordatorio.AddCell(celFrijolComCan);
            tblDatosRecordatorio.AddCell(celFrijolComCal);
            tblDatosRecordatorio.AddCell(celFrijolCenCan);
            tblDatosRecordatorio.AddCell(celFrijolCenCal);

            tblDatosRecordatorio.AddCell(celTortilla);
            tblDatosRecordatorio.AddCell(celTortillaDesCan);
            tblDatosRecordatorio.AddCell(celTortillaDesCal);
            tblDatosRecordatorio.AddCell(celTortillaComCan);
            tblDatosRecordatorio.AddCell(celTortillaComCal);
            tblDatosRecordatorio.AddCell(celTortillaCenCan);
            tblDatosRecordatorio.AddCell(celTortillaCenCal);

            tblDatosRecordatorio.AddCell(celCQH);
            tblDatosRecordatorio.AddCell(celCQHDesCan);
            tblDatosRecordatorio.AddCell(celCQHDesCal);
            tblDatosRecordatorio.AddCell(celCQHComCan);
            tblDatosRecordatorio.AddCell(celCQHComCal);
            tblDatosRecordatorio.AddCell(celCQHCenCan);
            tblDatosRecordatorio.AddCell(celCQHCenCal);

            tblDatosRecordatorio.AddCell(celFrutas);
            tblDatosRecordatorio.AddCell(celFrutasDesCan);
            tblDatosRecordatorio.AddCell(celFrutasDesCal);
            tblDatosRecordatorio.AddCell(celFrutasComCan);
            tblDatosRecordatorio.AddCell(celFrutasComCal);
            tblDatosRecordatorio.AddCell(celFrutasCenCan);
            tblDatosRecordatorio.AddCell(celFrutasCenCal);

            tblDatosRecordatorio.AddCell(celVerduras);
            tblDatosRecordatorio.AddCell(celVerdurasDesCan);
            tblDatosRecordatorio.AddCell(celVerdurasDesCal);
            tblDatosRecordatorio.AddCell(celVerdurasComCan);
            tblDatosRecordatorio.AddCell(celVerdurasComCal);
            tblDatosRecordatorio.AddCell(celVerdurasCenCan);
            tblDatosRecordatorio.AddCell(celVerdurasCenCal);

            tblDatosRecordatorio.AddCell(celAzucares);
            tblDatosRecordatorio.AddCell(celAzucaresDesCan);
            tblDatosRecordatorio.AddCell(celAzucaresDesCal);
            tblDatosRecordatorio.AddCell(celAzucaresComCan);
            tblDatosRecordatorio.AddCell(celAzucaresComCal);
            tblDatosRecordatorio.AddCell(celAzucaresCenCan);
            tblDatosRecordatorio.AddCell(celAzucaresCenCal);

            tblDatosRecordatorio.AddCell(celGrasas);
            tblDatosRecordatorio.AddCell(celGrasasDesCan);
            tblDatosRecordatorio.AddCell(celGrasasDesCal);
            tblDatosRecordatorio.AddCell(celGrasasComCan);
            tblDatosRecordatorio.AddCell(celGrasasComCal);
            tblDatosRecordatorio.AddCell(celGrasasCenCan);
            tblDatosRecordatorio.AddCell(celGrasasCenCal);

            tblDatosRecordatorio.AddCell(celTotalCel1);
            tblDatosRecordatorio.AddCell(celTotalCel2);
            tblDatosRecordatorio.AddCell(celTotalCel3);
            tblDatosRecordatorio.AddCell(celTotalCel4);
            tblDatosRecordatorio.AddCell(celTotalCel5);
            tblDatosRecordatorio.AddCell(celTotalCel6);
            tblDatosRecordatorio.AddCell(celTotalCel7);

            docRepNut.Add(tblDatosRecordatorio);

            #endregion

            #region Analsisis y Observaciones
            Paragraph AnaObs = new Paragraph()
            {
                Alignment = Element.ALIGN_LEFT
            };
            AnaObs.Add(new Phrase("Análisis del valor nutritivo de la dieta: ", fonEiqueta));
            AnaObs.Add(Chunk.TABBING);
            AnaObs.Add(new Phrase("Calorías totales de la dieta: ", fontDato));
            AnaObs.Add(Chunk.TABBING);
            AnaObs.Add(new Phrase(((datosNut.frijolCantDesayuno * 120) + (datosNut.toritillaCantDesayuno * 70) + (datosNut.CQHCantDesayuno * 90) + (datosNut.frutaCantDesayuno * 60) + (datosNut.verduraCantDesayuno * 25) + (datosNut.azucaresCantDesayuno * 45) + (datosNut.grasaCantDesayuno * 40) + (datosNut.frijolCantComida * 120) + (datosNut.tortillaCantComida * 70) + (datosNut.CQHCantComida * 90) + (datosNut.frutaCantComida * 60) + (datosNut.verdurasCantComida * 25) + (datosNut.azucaresCantComida * 45) + (datosNut.GrasaCantComida * 40) + (datosNut.frijolCantCena * 120) + (datosNut.torillaCantCena * 70) + (datosNut.CQHCantCena * 90) + (datosNut.frutaCantCena * 60) + (datosNut.verduraCantCena * 25) + (datosNut.azucaresCantCena * 45) + (datosNut.grasaCantCena * 40)).ToString() + " KCAL.", fontDato));
            AnaObs.Add(Chunk.TABBING);
            AnaObs.Add(new Phrase("Calorías ideales: 2000 KCAL", fontDato));
            AnaObs.Add(Chunk.NEWLINE); 
            AnaObs.Add(new Phrase("Observaciones:", fonEiqueta));
            AnaObs.Add(Chunk.TABBING);
            AnaObs.Add(new Phrase(datosNut.Observaciones, fontDato));
            AnaObs.Add(Chunk.NEWLINE); AnaObs.Add(Chunk.NEWLINE);
            AnaObs.Add(new Phrase("Nombre y firma del evaluado: ", fontDato));
            AnaObs.Add(Chunk.TABBING);
            AnaObs.Add(new Phrase(datos.evaluado, fontFirma));

            docRepNut.Add(AnaObs);

            #endregion

            docRepNut.Close();
            byte[] bytesStream = msRepNut.ToArray();
            msRepNut = new MemoryStream();
            msRepNut.Write(bytesStream, 0, bytesStream.Length);
            msRepNut.Position = 0;

            return new FileStreamResult(msRepNut, "application/pdf");
        }

    }

    public class HeaderFooterEGO : PdfPageEventHelper
    {
        private string _Folio;
        private string _Realizo;
        private string _CedRea;
        private string _Superviso;
        private string _CedSup;
        private string _Titulo;

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
            string imageizq = @"C:\inetpub\wwwroot\fotoUser\gobedohor.png";
            //var imageizq = "/fotoUsergobedohor.png";
            iTextSharp.text.Image jpgSupIzq = iTextSharp.text.Image.GetInstance(imageizq);
            jpgSupIzq.ScaleToFit(80f, 80f);

            PdfPCell clLogoSupIzq = new PdfPCell();
            clLogoSupIzq.BorderWidth = 0;
            clLogoSupIzq.VerticalAlignment = Element.ALIGN_BOTTOM;
            clLogoSupIzq.AddElement(jpgSupIzq);

            string imageder = @"C:\inetpub\wwwroot\fotoUser\nuevoCeccc.png";
            //var imageder = "/fotoUser/nuevoCeccc.png";
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

            PdfPCell cf1 = new PdfPCell(new Phrase("Folio", fontFooter));
            cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1.Border = PdfPCell.NO_BORDER;
            cf1.BorderWidthTop = 0.75f;
            footer.AddCell(cf1);

            PdfPCell cf2 = new PdfPCell(new Phrase(_Realizo, fontFooter));
            cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2.Border = PdfPCell.NO_BORDER;
            cf2.BorderWidthTop = 0.75f;
            footer.AddCell(cf2);

            PdfPCell cf3 = new PdfPCell(new Phrase(_Superviso, fontFooter));
            cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3.Border = PdfPCell.NO_BORDER;
            cf3.BorderWidthTop = 0.75f;
            footer.AddCell(cf3);

            PdfPCell cf1b = new PdfPCell(new Phrase(_Folio, fontFooter));
            cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1b);

            PdfPCell cf2b = new PdfPCell(new Phrase(_CedRea, fontFooter));
            cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2b);

            PdfPCell cf3b = new PdfPCell(new Phrase(_CedSup, fontFooter));
            cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3b);

            PdfPCell cf1c = new PdfPCell(new Phrase("", fontFooter));
            cf1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1c);

            PdfPCell cf2c = new PdfPCell(new Phrase("Realizó", fontFooterTitulo));
            cf2c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2c);

            PdfPCell cf3c = new PdfPCell(new Phrase("Responsable Sanitario", fontFooterTitulo));
            cf3c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3c);

            PdfPCell texto = new PdfPCell(new Phrase("Este documento es confidencial no tendrá ningún valor jurídico si presenta tachaduras o enmendaduras.", fontFooter));
            texto.Colspan = 3;
            texto.Border = PdfPCell.NO_BORDER;
            texto.HorizontalAlignment = Element.ALIGN_CENTER;
            footer.AddCell(texto);

            footer.WriteSelectedRows(0, -1, 20, 60, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterEGO getMultilineFooter(string Folio, string Realizo, string CedRea, string Superviso, string CedSup, string Titulo)
        {
            HeaderFooterEGO result = new HeaderFooterEGO();

            result.folio = Folio;
            result.realizo = Realizo;
            result.cedrea = CedRea;
            result.superviso = Superviso;
            result.cedsup = CedSup;
            result.titulo = Titulo;

            return result;
        }
    }

    public class HeaderFooterNutricion : PdfPageEventHelper
    {
        private string _Folio;
        private string _Realizo;
        private string _CedRea;
        private string _Superviso;
        private string _CedSup;
        private string _Titulo;

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

            PdfPCell cf1 = new PdfPCell(new Phrase("Folio", fontFooter));
            cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1.Border = PdfPCell.NO_BORDER;
            cf1.BorderWidthTop = 0.75f;
            footer.AddCell(cf1);

            PdfPCell cf2 = new PdfPCell(new Phrase(_Realizo, fontFooter));
            cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2.Border = PdfPCell.NO_BORDER;
            cf2.BorderWidthTop = 0.75f;
            footer.AddCell(cf2);

            PdfPCell cf3 = new PdfPCell(new Phrase(_Superviso, fontFooter));
            cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3.Border = PdfPCell.NO_BORDER;
            cf3.BorderWidthTop = 0.75f;
            footer.AddCell(cf3);

            PdfPCell cf1b = new PdfPCell(new Phrase(_Folio, fontFooter));
            cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1b);

            PdfPCell cf2b = new PdfPCell(new Phrase("CED. PROF: " + _CedRea, fontFooter));
            cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2b);

            PdfPCell cf3b = new PdfPCell(new Phrase("CED. PROF: " + _CedSup, fontFooter));
            cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3b);

            PdfPCell cf1c = new PdfPCell(new Phrase("", fontFooter));
            cf1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1c);

            PdfPCell cf2c = new PdfPCell(new Phrase("REALIZO", fontFooter));
            cf2c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2c);

            PdfPCell cf3c = new PdfPCell(new Phrase("SUPERVISO", fontFooter));
            cf3c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3c);

            footer.WriteSelectedRows(0, -1, 20, 60, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterNutricion getMultilineFooter(string Folio, string Realizo, string CedRea, string Superviso, string CedSup, string Titulo)
        {
            HeaderFooterNutricion result = new HeaderFooterNutricion();

            result.folio = Folio;
            result.realizo = Realizo;
            result.cedrea = CedRea;
            result.superviso = Superviso;
            result.cedsup = CedSup;
            result.titulo = Titulo;

            return result;
        }
    }

    public class HeaderFooterTX : PdfPageEventHelper
    {
        private string _FolioTx;
        private string _RealizoTx;
        private string _CedReaTx;
        private string _SupervisoTx;
        private string _CedSupTx;
        private string _TituloTx;

        public string folioTx
        {
            get { return _FolioTx; }
            set { _FolioTx = value; }
        }

        public string realizoTx
        {
            get { return _RealizoTx; }
            set { _RealizoTx = value; }
        }

        public string cedreaTx
        {
            get { return _CedReaTx; }
            set { _CedReaTx = value; }
        }

        public string supervisoTx
        {
            get { return _SupervisoTx; }
            set { _SupervisoTx = value; }
        }

        public string cedsupTx
        {
            get { return _CedSupTx; }
            set { _CedSupTx = value; }
        }

        public string tituloTx
        {
            get { return _TituloTx; }
            set { _TituloTx = value; }
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

            Chunk chkTit = new Chunk("CENTRO ESTATAL DE CONTROL DE CONFIANZA CERTIFICADO DEL ESTADO DE CHIAPAS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph = new Paragraph();
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Add(chkTit);

            Chunk chkTit2 = new Chunk("DIRECCION MEDICA Y TOXICOLOGICA", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph2 = new Paragraph();
            paragraph2.Alignment = Element.ALIGN_CENTER;
            paragraph2.Add(chkTit2);

            Chunk chkSub = new Chunk(_TituloTx, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Alignment = Element.ALIGN_CENTER;
            paragraph1.Add(chkSub);

            PdfPCell clTitulo = new PdfPCell();
            clTitulo.BorderWidth = 0;
            clTitulo.AddElement(paragraph);

            PdfPCell clTitulo2 = new PdfPCell();
            clTitulo2.BorderWidth = 0;
            clTitulo2.AddElement(paragraph2);

            PdfPCell clSubTit = new PdfPCell();
            clSubTit.BorderWidth = 0;
            clSubTit.AddElement(paragraph1);

            PdfPTable tblTitulo = new PdfPTable(1);
            tblTitulo.WidthPercentage = 100;
            tblTitulo.AddCell(clTitulo);
            tblTitulo.AddCell(clTitulo2);
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

            PdfPCell cf1 = new PdfPCell(new Phrase("Folio", fontFooter));
            cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1.Border = PdfPCell.NO_BORDER;
            cf1.BorderWidthTop = 0.75f;
            footer.AddCell(cf1);

            PdfPCell cf2 = new PdfPCell(new Phrase(_RealizoTx, fontFooter));
            cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2.Border = PdfPCell.NO_BORDER;
            cf2.BorderWidthTop = 0.75f;
            footer.AddCell(cf2);

            PdfPCell cf3 = new PdfPCell(new Phrase(_SupervisoTx, fontFooter));
            cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3.Border = PdfPCell.NO_BORDER;
            cf3.BorderWidthTop = 0.75f;
            footer.AddCell(cf3);

            PdfPCell cf1b = new PdfPCell(new Phrase(_FolioTx, fontFooter));
            cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1b);

            PdfPCell cf2b = new PdfPCell(new Phrase(_CedReaTx, fontFooter));
            cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2b);

            PdfPCell cf3b = new PdfPCell(new Phrase(_CedSupTx, fontFooter));
            cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3b);

            PdfPCell cf1c = new PdfPCell(new Phrase("", fontFooter));
            cf1c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1c);

            PdfPCell cf2c = new PdfPCell(new Phrase("Supervisor Ocular", fontFooterTitulo));
            cf2c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2c);

            PdfPCell cf3c = new PdfPCell(new Phrase("Responsable Sanitario", fontFooterTitulo));
            cf3c.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3c.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3c);

            PdfPCell texto = new PdfPCell(new Phrase("ESTE FORMATO ES DE CARACTER RESERVADO", fontFooter));
            texto.Colspan = 3;
            texto.Border = PdfPCell.NO_BORDER;
            texto.HorizontalAlignment = Element.ALIGN_CENTER;
            footer.AddCell(texto);

            footer.WriteSelectedRows(0, -1, 20, 60, writer.DirectContent);
            //                                  60 margen inferior

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterTX getMultilineFooter(string Folio, string Realizo, string CedRea, string Superviso, string CedSup, string Titulo)
        {
            HeaderFooterTX result = new HeaderFooterTX();

            result.folioTx = Folio;
            result.realizoTx = Realizo;
            result.cedreaTx = CedRea;
            result.supervisoTx = Superviso;
            result.cedsupTx = CedSup;
            result.tituloTx = Titulo;

            return result;
        }
    }

}
