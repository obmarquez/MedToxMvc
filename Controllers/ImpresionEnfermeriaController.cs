using iTextSharp.text;
using iTextSharp.text.pdf;
using MedToxMVC.Data;
using MedToxMVC.Models.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Controllers
{
    public class ImpresionEnfermeriaController : Controller
    {
        float[] widthsTitulosGenerales = new float[] { 1f };
        private DBOperaciones repo;

        public ImpresionEnfermeriaController()
        {
            repo = new DBOperaciones();
        }

        //[Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult testAudit(int IdH)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_Test_enfermeria", new { @idhistorico = IdH, @test = 1 }).FirstOrDefault();
            var datosC3 = repo.Get<ConsultasModel>("sp_general_obtener_certificacion_acreditacion").FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datos.folio.ToString();
            string elEvaluado = datos.evaluado.ToString();
            string elCodigo = datos.codigoevaluado.ToString();
            string elTitulo = "Test Audit";

            pwRep.PageEvent = HeaderFooterEnfermeria.getMultilineFooter(elFolio, elEvaluado, elCodigo, elTitulo);

            docRep.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);

            string respuestaPregunta1 = string.Empty;
            string respuestaPregunta2 = string.Empty;
            string respuestaPregunta3 = string.Empty;
            string respuestaPregunta4 = string.Empty;
            string respuestaPregunta5 = string.Empty;
            string respuestaPregunta6 = string.Empty;
            string respuestaPregunta7 = string.Empty;
            string respuestaPregunta8 = string.Empty;
            string respuestaPregunta9 = string.Empty;
            string respuestaPregunta10 = string.Empty;

            #region respuestas de Número a Texto
            switch (datos.pregunta1)
            {
                case 0: 
                    respuestaPregunta1 = "Ninguna";                        
                    break;
                case 1:
                    respuestaPregunta1 = "Una o menos veces al mes";
                    break;
                case 2:
                    respuestaPregunta1 = "De 2 a 4 veces al mes";
                    break;
                case 3:
                    respuestaPregunta1 = "De 2 a 3 veces a la semana";
                    break;
                case 4:
                    respuestaPregunta1 = "4 o más veces a la semana";
                    break;
            }

            switch (datos.pregunta2)
            {
                case 0:
                    respuestaPregunta2 = "1 ó 2";
                    break;
                case 1:
                    respuestaPregunta2 = "3 ó 4";
                    break;
                case 2:
                    respuestaPregunta2 = "5 ó 6";
                    break;
                case 3:
                    respuestaPregunta2 = "7, 8 ó 9";
                    break;
                case 4:
                    respuestaPregunta2 = "10 ó más";
                    break;
            }

            switch (datos.pregunta3)
            {
                case 0:
                    respuestaPregunta3 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta3 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta3 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta3 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta3 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta4)
            {
                case 0:
                    respuestaPregunta4 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta4 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta4 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta4 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta4 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta5)
            {
                case 0:
                    respuestaPregunta5 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta5 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta5 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta5 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta5 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta6)
            {
                case 0:
                    respuestaPregunta6 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta6 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta6 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta6 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta6 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta7)
            {
                case 0:
                    respuestaPregunta7 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta7 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta7 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta7 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta7 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta8)
            {
                case 0:
                    respuestaPregunta8 = "Nunca";
                    break;
                case 1:
                    respuestaPregunta8 = "Menos de una vez al mes";
                    break;
                case 2:
                    respuestaPregunta8 = "Mensualmente";
                    break;
                case 3:
                    respuestaPregunta8 = "Semanalmente";
                    break;
                case 4:
                    respuestaPregunta8 = "A diario o casi diario";
                    break;
            }

            switch (datos.pregunta9)
            {
                case 0:
                    respuestaPregunta9 = "No";
                    break;
                case 1:
                    respuestaPregunta9 = "Sí, pero no el curso del último año";
                    break;
                case 2:
                    respuestaPregunta9 = "Sí, el último año";
                    break;
            }

            switch (datos.pregunta10)
            {
                case 0:
                    respuestaPregunta10 = "No";
                    break;
                case 1:
                    respuestaPregunta10 = "Sí, pero no el curso del último año";
                    break;
                case 2:
                    respuestaPregunta10 = "Sí, el último año";
                    break;
            }
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

            PdfPCell celEmi_b = new PdfPCell(new Phrase("2021", fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/29", fonEiqueta));
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

            #region Fecha
            Paragraph certificacion = new Paragraph();
            certificacion.Alignment = Element.ALIGN_LEFT;
            certificacion.Add(Chunk.TABBING);
            certificacion.Add(new Phrase("Certificación No. ", fonEiqueta));
            certificacion.Add(new Phrase(datosC3.certifica, fontDato));
            certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING);
            certificacion.Add(new Phrase("Acreditación No. ", fonEiqueta));
            certificacion.Add(new Phrase(datosC3.acredita, fontDato));
            docRep.Add(certificacion);

            Paragraph fecha = new Paragraph();
            fecha.Alignment = Element.ALIGN_RIGHT;
            fecha.Add(new Phrase("Tuxtla Gutiérrez, Chiapas a ", fontDato));
            fecha.Add(new Phrase(datos.fecha, fontDato));
            docRep.Add(fecha);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 20f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos Personales", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);

            #endregion

            #region Datos Personales
            Paragraph losDatos = new Paragraph();
            losDatos.Alignment = Element.ALIGN_LEFT;
            losDatos.Add(new Phrase("Nombre: ", fonEiqueta)); 
            losDatos.Add(Chunk.TABBING); 
            losDatos.Add(new Phrase(datos.evaluado, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("RFC: ", fonEiqueta));
            losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.rfc, fontDato));
            losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Edad: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.edad.ToString(), fontDato));
            losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Sexo: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.sexo, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Dependencia: ", fonEiqueta));
            losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.dependencia, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Lugar de evaluación: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Tuxtla Gutiérrez, Chiapas.", fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Tipo de evaluación: ", fonEiqueta)); losDatos.Add(Chunk.TABBING); losDatos.Add(new Phrase(datos.evaluacion, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Puesto: ", fonEiqueta)); losDatos.Add(Chunk.TABBING); losDatos.Add(new Phrase(datos.puesto, fontDato));
            losDatos.Add(Chunk.NEWLINE); losDatos.Add(Chunk.NEWLINE);

            docRep.Add(losDatos);
            #endregion

            #region Test Audit
            Paragraph Audit = new Paragraph();
            Audit.Alignment = Element.ALIGN_LEFT;
            Audit.Add(new Phrase("1.- ¿Con qué frecuencia consume alguna bebida alcohólica?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta1, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("2.- ¿Cuántas copas, latas o vasos de bebidas alcohólicas suele realizar en un día de consumo normal?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta2, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("3.- ¿Con qué frecuencia toma 6 o más bebidas alcohólicas en un solo día?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta3, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("4.- ¿Con qué frecuencia en el curso del último año ha sido incapaz de parar de beber una vez que ha iniciado la ingesta?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta4, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("5.- ¿Con qué frecuencia en el último año no pudo hacer lo que se esperaba de usted porque había bebido?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta5, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("6.- ¿Con qué frecuencia en el curso del último año ha necesitado beber en ayunas para recuperarse despúes de haber bebido mucho en el día anterior?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta6, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("7.- ¿Con qué frecuencia en el curso del último año ha tenido remordimientos o sentimientos de culpa despúes de haber bebido?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta7, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("8.- ¿Con qué frecuencia en el curso del último año no ha podido recordar lo que sucedió la noche anterior porque había estado bebiendo?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta8, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("9.- ¿Usted o alguna otra persona ha resultado herido porque usted ha bebido?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta9, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            Audit.Add(new Phrase("10.- ¿Algún familiar, amigo, médico o profesional de la salud ha mostrado preocupación por su consumo de bebidas alcohólicas o le han sugerido dejar de tomar?", fonEiqueta)); Audit.Add(Chunk.NEWLINE);
            Audit.Add(Chunk.TABBING);
            Audit.Add(new Phrase(respuestaPregunta10, fontDato));
            Audit.Add(Chunk.NEWLINE); 

            docRep.Add(Audit);
            #endregion

            docRep.Close();

            byte[] bytesStream = msRep.ToArray();

            msRep = new MemoryStream();

            msRep.Write(bytesStream, 0, bytesStream.Length);

            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        public IActionResult testNicotina(int IdH)
        {
            var datosNico = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_Test_enfermeria", new { @idhistorico = IdH, @test = 2 }).FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datosNico.folio.ToString();
            string elEvaluado = datosNico.evaluado.ToString();
            string elCodigo = datosNico.codigoevaluado.ToString();
            string elTitulo = "Test de Fagerström";

            pwRep.PageEvent = HeaderFooterEnfermeria.getMultilineFooter(elFolio, elEvaluado, elCodigo, elTitulo);

            docRep.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);

            string respuesta1 = string.Empty;
            string respuesta2 = string.Empty;
            string respuesta3 = string.Empty;
            string respuesta4 = string.Empty;
            string respuesta5 = string.Empty;
            string respuesta6 = string.Empty;

            #region respuestas
            if(datosNico.p7==0)
            {
                respuesta1 = "-";
            }
            else
            {
                switch (datosNico.p1)
                {
                    case 0:
                        respuesta1 = "Más de 60 minutos";
                        break;
                    case 1:
                        respuesta1 = "Entre 6 y 30 minutos";
                        break;
                    case 2:
                        respuesta1 = "Hasta 5 minutos";
                        break;
                    case 3:
                        respuesta1 = "Entre 6 y 30 minutos";
                        break;
                    default:
                        respuesta1 = "No definido";
                        break;
                }
            }

            if (datosNico.p7 == 0)
            {
                respuesta2 = "-";
            }
            else
            {
                if (datosNico.p2 == 0)
                    respuesta2 = "No";
                else
                    respuesta2 = "Sí";
            }

            if (datosNico.p7 == 0)
            {
                respuesta3 = "-";
            }
            else
            {
                if (datosNico.p3 == 0)
                    respuesta3 = "Cualquier otro";
                else
                    respuesta3 = "El primero de la mañana";
            }

            if (datosNico.p7 == 0)
            {
                respuesta4 = "-";
            }
            else
            {
                switch (datosNico.p4)
                {
                    case 0:
                        respuesta4 = "10 ó menos";
                        break;
                    case 1:
                        respuesta4 = "11 - 20";
                        break;
                    case 2:
                        respuesta4 = "21 - 30";
                        break;
                    case 3:
                        respuesta4 = "31 ó más";
                        break;
                    default:
                        respuesta4 = "No definido";
                        break;
                }
            }

            if (datosNico.p7 == 0)
            {
                respuesta5 = "-";
            }
            else
            {
                if (datosNico.p5 == 0)
                    respuesta5 = "No";
                else
                    respuesta5 = "Sí";
            }

            if (datosNico.p7 == 0)
            {
                respuesta6 = "-";
            }
            else
            {
                if (datosNico.p6 == 0)
                    respuesta6 = "No";
                else
                    respuesta6 = "Sí";
            }
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

            PdfPCell celEmi_b = new PdfPCell(new Phrase("2021", fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPTable tblEmiRevCpd = new PdfPTable(2);
            tblEmiRevCpd.WidthPercentage = 100;
            float[] witdhs = new float[] { 50f, 50f};
            tblEmiRevCpd.SetWidths(witdhs);

            tblEmiRevCpd.AddCell(clRev);
            tblEmiRevCpd.AddCell(celEmi);

            tblEmiRevCpd.AddCell(celEmi_b);
            tblEmiRevCpd.AddCell(celRev_b);

            docRep.Add(tblEmiRevCpd);
            #endregion

            #region fecha
            Paragraph fecha = new Paragraph();
            fecha.Alignment = Element.ALIGN_RIGHT;
            fecha.Add(new Phrase("Fecha: ", fonEiqueta));
            fecha.Add(Chunk.TABBING);
            fecha.Add(new Phrase(datosNico.fecha, fontDato));
            fecha.Add(Chunk.NEWLINE);

            docRep.Add(fecha);
            #endregion

            #region Evaluado
            Paragraph evaluado = new Paragraph();
            evaluado.Alignment = Element.ALIGN_LEFT;
            evaluado.Add(new Phrase("Nombre: ", fonEiqueta));
            evaluado.Add(Chunk.TABBING);
            evaluado.Add(new Phrase(datosNico.evaluado, fontDato));
            evaluado.Add(Chunk.TABBING); evaluado.Add(Chunk.TABBING); evaluado.Add(Chunk.TABBING); evaluado.Add(Chunk.TABBING);
            evaluado.Add(new Phrase("Edad: ", fonEiqueta));
            evaluado.Add(Chunk.TABBING);
            evaluado.Add(new Phrase(datosNico.edad.ToString(), fontDato));
            evaluado.Add(Chunk.TABBING); evaluado.Add(Chunk.TABBING); evaluado.Add(Chunk.TABBING);
            evaluado.Add(new Phrase("Sexo: ", fonEiqueta));
            evaluado.Add(Chunk.TABBING);
            evaluado.Add(new Phrase(datosNico.sexo, fontDato));
            evaluado.Add(Chunk.NEWLINE); evaluado.Add(Chunk.NEWLINE);

            docRep.Add(evaluado);
            #endregion

            #region Nicotina
            Paragraph Nicotina = new Paragraph();
            Nicotina.Alignment = Element.ALIGN_LEFT;
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(new Phrase("1.- ¿Cuánto tiempo pasa entre que se levanta y fuma su primer cigarrillo?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta1, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            Nicotina.Add(new Phrase("2.- ¿Encuentra díficil no fumar en lugares donde está prohibido, como la biblioteca o el cine?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta2, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            Nicotina.Add(new Phrase("3.- ¿Qué cigarrillo le molesta más dejar de fumar?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta3, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            Nicotina.Add(new Phrase("4.- ¿Cuántos cigarrillos fuma cada día?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta4, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            Nicotina.Add(new Phrase("5.- ¿Fuma con más frecuencia durante las primeras horas después de levantarse que durante el resto del día?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta5, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            Nicotina.Add(new Phrase("6.- ¿Fuma aunque esté tan enfermo que tenga que guardar cama la mayor parte del día?", fonEiqueta));
            Nicotina.Add(Chunk.NEWLINE);
            Nicotina.Add(Chunk.TABBING);
            Nicotina.Add(new Phrase(respuesta6, fontDato));
            Nicotina.Add(Chunk.NEWLINE); Nicotina.Add(Chunk.NEWLINE);

            docRep.Add(Nicotina);
            #endregion

            #region total
            Paragraph total = new Paragraph();
            total.Alignment = Element.ALIGN_RIGHT;
            total.Add(new Phrase("Puntuación total: ", fonEiqueta));
            total.Add(Chunk.TABBING);
            total.Add(new Phrase((datosNico.p1 + datosNico.p2 + datosNico.p3 + datosNico.p4 + datosNico.p5 + datosNico.p6).ToString(), fonEiqueta));
            total.Add(Chunk.NEWLINE);

            docRep.Add(total);
            #endregion

            docRep.Close();

            byte[] bytesStream = msRep.ToArray();

            msRep = new MemoryStream();

            msRep.Write(bytesStream, 0, bytesStream.Length);

            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }

        public IActionResult textMed(int IdH)
        {
            var datos = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_Test_enfermeria", new { @idhistorico = IdH, @test = 3 }).FirstOrDefault();
            var datosC3 = repo.Get<ConsultasModel>("sp_general_obtener_certificacion_acreditacion").FirstOrDefault();

            MemoryStream msRep = new MemoryStream();

            Document docRep = new Document(PageSize.LETTER, 30f, 20f, 70f, 40f);
            PdfWriter pwRep = PdfWriter.GetInstance(docRep, msRep);

            string elFolio = datos.folio.ToString();
            string elEvaluado = datos.evaluado.ToString();
            string elCodigo = datos.codigoevaluado.ToString();
            string elTitulo = "Custionario de Ingesta de Medicamentos";

            pwRep.PageEvent = HeaderFooterEnfermeria.getMultilineFooter(elFolio, elEvaluado, elCodigo, elTitulo);

            docRep.Open();

            var fonEiqueta = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
            var fontDato = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);

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

            PdfPCell celEmi_b = new PdfPCell(new Phrase("2021", fonEiqueta));
            celEmi_b.BorderWidth = 0;
            celEmi_b.VerticalAlignment = Element.ALIGN_TOP;
            celEmi_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celEmi_b.BorderWidthBottom = 0.75f;

            PdfPCell celRev_b = new PdfPCell(new Phrase("1.1", fonEiqueta));
            celRev_b.BorderWidth = 0;
            celRev_b.VerticalAlignment = Element.ALIGN_TOP;
            celRev_b.HorizontalAlignment = Element.ALIGN_CENTER;
            celRev_b.BorderWidthBottom = 0.75f;

            PdfPCell celCod_b = new PdfPCell(new Phrase("CECCC/DMT/31", fonEiqueta));
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

            #region Fecha
            Paragraph certificacion = new Paragraph();
            certificacion.Alignment = Element.ALIGN_LEFT;
            certificacion.Add(Chunk.TABBING);
            certificacion.Add(new Phrase("Certificación No. ", fonEiqueta));
            certificacion.Add(new Phrase(datosC3.certifica, fontDato));
            certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING); certificacion.Add(Chunk.TABBING);
            certificacion.Add(new Phrase("Acreditación No. ", fonEiqueta));
            certificacion.Add(new Phrase(datosC3.acredita, fontDato));
            certificacion.Add(Chunk.NEWLINE);
            docRep.Add(certificacion);

            Paragraph fecha = new Paragraph();
            fecha.Alignment = Element.ALIGN_RIGHT;
            fecha.Add(new Phrase("Tuxtla Gutiérrez, Chiapas a ", fontDato));
            fecha.Add(new Phrase(datos.fecha, fontDato));
            docRep.Add(fecha);
            #endregion

            #region Titulo Datos personales
            PdfPTable Datospersonales = new PdfPTable(1);
            Datospersonales.TotalWidth = 560f;
            Datospersonales.LockedWidth = true;

            Datospersonales.SetWidths(widthsTitulosGenerales);
            Datospersonales.HorizontalAlignment = 0;
            Datospersonales.SpacingBefore = 10f;
            Datospersonales.SpacingAfter = 10f;

            PdfPCell cellTituloTituloFamiliar = new PdfPCell(new Phrase("Datos Personales", new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLDITALIC)));
            cellTituloTituloFamiliar.HorizontalAlignment = 1;
            cellTituloTituloFamiliar.BackgroundColor = new iTextSharp.text.BaseColor(234, 236, 238);
            cellTituloTituloFamiliar.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cellTituloTituloFamiliar.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            Datospersonales.AddCell(cellTituloTituloFamiliar);

            docRep.Add(Datospersonales);
            #endregion

            #region Datos Personales
            Paragraph losDatos = new Paragraph();
            losDatos.Alignment = Element.ALIGN_LEFT;
            losDatos.Add(new Phrase("Nombre: ", fonEiqueta));
            losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.evaluado, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("RFC: ", fonEiqueta));
            losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.rfc, fontDato));
            losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Edad: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.edad.ToString(), fontDato));
            losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Sexo: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.sexo, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Dependencia: ", fonEiqueta));
            losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase(datos.dependencia, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Lugar de evaluación: ", fonEiqueta)); losDatos.Add(Chunk.TABBING);
            losDatos.Add(new Phrase("Tuxtla Gutiérrez, Chiapas.", fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Tipo de evaluación: ", fonEiqueta)); losDatos.Add(Chunk.TABBING); losDatos.Add(new Phrase(datos.evaluacion, fontDato));
            losDatos.Add(Chunk.NEWLINE);
            losDatos.Add(new Phrase("Puesto: ", fonEiqueta)); losDatos.Add(Chunk.TABBING); losDatos.Add(new Phrase(datos.puesto, fontDato));
            losDatos.Add(Chunk.NEWLINE); 

            docRep.Add(losDatos);
            #endregion

            #region Medicamentos
            Paragraph med = new Paragraph();
            med.Alignment = Element.ALIGN_LEFT;
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase("1.- ¿Actualmente padece alguna enfermedad?", fonEiqueta)); med.Add(Chunk.TABBING); med.Add(new Phrase(datos.padeceenfermedad, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("2.- ¿Cuál?", fonEiqueta)); med.Add(Chunk.TABBING); med.Add(new Phrase(datos.enfermedad, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("3.- ¿Actualmente se encuentra tomando algún medicamento?", fonEiqueta)); med.Add(Chunk.TABBING); med.Add(new Phrase(datos.tomamedicamento, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("4.- ¿Cuenta con receta médica?", fonEiqueta)); med.Add(Chunk.TABBING); med.Add(new Phrase(datos.cReceta, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("5.- Si su respuesta anterior fue afirmativa indique el nombre del medicamento", fonEiqueta));
            med.Add(Chunk.NEWLINE); 
            med.Add(new Phrase(datos.medicamento, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("6.- ¿Cuántas pastillas o cápsulas toma al día?", fonEiqueta));
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase(datos.cantidad, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("7.- ¿Cuánto tiempo lleva consumiendo el medicamento?", fonEiqueta));
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase(datos.tiempo, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("8.- ¿Ha probado o consumido algún tipo de droga en el último año?", fonEiqueta)); med.Add(Chunk.TABBING); med.Add(new Phrase(datos.consumiodroga, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("9.- Si su respuesta anterior fue afirmativa indique cuál fue", fonEiqueta));
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase(datos.droga, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("10.- ¿Cuántas veces la consumió?", fonEiqueta));
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase(datos.frecuenciadroga, fontDato));
            med.Add(Chunk.NEWLINE); med.Add(Chunk.NEWLINE);

            med.Add(new Phrase("11.- ¿En qué cantidad la consumió?", fonEiqueta));
            med.Add(Chunk.NEWLINE);
            med.Add(new Phrase(datos.cantidaddroga, fontDato));
            med.Add(Chunk.NEWLINE);

            docRep.Add(med);
            #endregion

            docRep.Close();

            byte[] bytesStream = msRep.ToArray();

            msRep = new MemoryStream();

            msRep.Write(bytesStream, 0, bytesStream.Length);

            msRep.Position = 0;

            return new FileStreamResult(msRep, "application/pdf");
        }
    }

    public class HeaderFooterEnfermeria : PdfPageEventHelper
    {
        private string _Folio;
        private string _Codigo;
        private string _Evaluado;
        private string _Titulo;

        public string folio
        {
            get { return _Folio; }
            set { _Folio = value; }
        }

        public string codigo
        {
            get { return _Codigo; }
            set { _Codigo = value; }
        }

        public string evaluado
        {
            get { return _Evaluado; }
            set { _Evaluado = value; }
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
            PdfPTable footer = new PdfPTable(3);
            footer.TotalWidth = page.Width - 40;

            PdfPCell cf1 = new PdfPCell(new Phrase("Folio", fontFooter));
            cf1.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1.Border = PdfPCell.NO_BORDER;
            cf1.BorderWidthTop = 0.75f;
            footer.AddCell(cf1);

            PdfPCell cf2 = new PdfPCell(new Phrase(_Evaluado, fontFooter));
            cf2.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2.Border = PdfPCell.NO_BORDER;
            cf2.BorderWidthTop = 0.75f;
            footer.AddCell(cf2);

            PdfPCell cf3 = new PdfPCell(new Phrase("Codigo Evaluado", fontFooter));
            cf3.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3.Border = PdfPCell.NO_BORDER;
            cf3.BorderWidthTop = 0.75f;
            footer.AddCell(cf3);

            PdfPCell cf1b = new PdfPCell(new Phrase(_Folio, fontFooter));
            cf1b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf1b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf1b);

            PdfPCell cf2b = new PdfPCell(new Phrase("Nombre y firma del Evaluado", fontFooter));
            cf2b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf2b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf2b);

            PdfPCell cf3b = new PdfPCell(new Phrase(_Codigo, fontFooter));
            cf3b.HorizontalAlignment = Element.ALIGN_CENTER;
            cf3b.Border = PdfPCell.NO_BORDER;
            footer.AddCell(cf3b);

            PdfPCell texto = new PdfPCell(new Phrase("Este documento es confidencial no tendrá ningún jurídico si presenta tachaduras o enmendaduras.", fontFooter));
            texto.Colspan = 3;
            texto.Border = PdfPCell.NO_BORDER;
            texto.HorizontalAlignment = Element.ALIGN_CENTER;
            footer.AddCell(texto);

            footer.WriteSelectedRows(0, -1, 20, 50, writer.DirectContent);

            iTextSharp.text.Rectangle rect = writer.GetBoxSize("footer");
        }

        public static HeaderFooterEnfermeria getMultilineFooter(string Folio, string Evaluado, string Codigo, string Titulo)
        {
            HeaderFooterEnfermeria result = new HeaderFooterEnfermeria();

            result.folio = Folio;
            result.codigo = Codigo;
            result.evaluado = Evaluado;
            result.titulo = Titulo;

            return result;
        }
    }
}
