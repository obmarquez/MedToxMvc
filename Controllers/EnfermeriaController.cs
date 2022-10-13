using MedToxMVC.Data;
using MedToxMVC.Helper;
using MedToxMVC.Models.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedToxMVC.Models.EnfermeriaModel;

namespace MedToxMVC.Controllers
{
    [Authorize]
    public class EnfermeriaController : Controller
    {
        private DBOperaciones repo;

        public EnfermeriaController()
        {
            repo = new DBOperaciones();
        }

        [Authorize(Roles = "Administrador, Enfermeria")]
        public IActionResult IndexEnfermeria(string fecha = "")
        {
            if (fecha == "")
            {
                return View();
            }
            else
            {
                ViewBag.fechapasar = fecha;

                #region Nicotina
                List<SelectListItem> opcion0 = new List<SelectListItem>();
                opcion0.Add(new SelectListItem { Text = "No", Value = "0" });
                opcion0.Add(new SelectListItem { Text = "Sí", Value = "1" });
                ViewBag.vcmbP00 = opcion0;

                List<SelectListItem> opcion1 = new List<SelectListItem>();
                opcion1.Add(new SelectListItem { Text = "más de 60 minutos", Value = "0" });
                opcion1.Add(new SelectListItem { Text = "31 a 60 minutos", Value = "1" });
                opcion1.Add(new SelectListItem { Text = "entre 6 y 30 minutos", Value = "2" });
                opcion1.Add(new SelectListItem { Text = "hasta 5 minutos", Value = "3" });
                ViewBag.vcmbP01 = opcion1;

                List<SelectListItem> opcion2 = new List<SelectListItem>();
                opcion2.Add(new SelectListItem { Text = "No", Value = "0" });
                opcion2.Add(new SelectListItem { Text = "Si", Value = "1" });
                ViewBag.vcmbP02 = opcion2;

                List<SelectListItem> opcion3 = new List<SelectListItem>();
                opcion3.Add(new SelectListItem { Text = "Cualquier otro", Value = "0" });
                opcion3.Add(new SelectListItem { Text = "El primero de la mañana", Value = "1" });
                ViewBag.vcmbP03 = opcion3;

                List<SelectListItem> opcion4 = new List<SelectListItem>();
                opcion4.Add(new SelectListItem { Text = "10 o menos", Value = "0" });
                opcion4.Add(new SelectListItem { Text = "11 - 20", Value = "1" });
                opcion4.Add(new SelectListItem { Text = "21 - 30", Value = "2" });
                opcion4.Add(new SelectListItem { Text = "31 o más", Value = "3" });
                ViewBag.vcmbP04 = opcion4;

                List<SelectListItem> opcion5 = new List<SelectListItem>();
                opcion5.Add(new SelectListItem { Text = "No", Value = "0" });
                opcion5.Add(new SelectListItem { Text = "Sí", Value = "1" });
                ViewBag.vcmbP05 = opcion5;

                List<SelectListItem> opcion6 = new List<SelectListItem>();
                opcion6.Add(new SelectListItem { Text = "No", Value = "0" });
                opcion6.Add(new SelectListItem { Text = "Sí", Value = "1" });
                ViewBag.vcmbP06 = opcion6;
                #endregion

                #region Audit
                List<SelectListItem> p1 = new List<SelectListItem>();
                p1.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p1.Add(new SelectListItem { Text = "Una ó menos veces al mes", Value = "1" });
                p1.Add(new SelectListItem { Text = "De 2 a 4 veces al mes", Value = "2" });
                p1.Add(new SelectListItem { Text = "De 2 a 3 veces a la semana", Value = "3" });
                p1.Add(new SelectListItem { Text = "4 ó más veces a la semana", Value = "4" });
                ViewBag.vP1 = p1;

                List<SelectListItem> p2 = new List<SelectListItem>();
                p2.Add(new SelectListItem { Text = "1 ó 2", Value = "0" });
                p2.Add(new SelectListItem { Text = "3 ó 4", Value = "1" });
                p2.Add(new SelectListItem { Text = "5 ó 6", Value = "2" });
                p2.Add(new SelectListItem { Text = "7, 8 ó 9", Value = "3" });
                p2.Add(new SelectListItem { Text = "10 ó más", Value = "4" });
                ViewBag.vP2 = p2;

                List<SelectListItem> p3 = new List<SelectListItem>();
                p3.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p3.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p3.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p3.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p3.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP3 = p3;

                List<SelectListItem> p4 = new List<SelectListItem>();
                p4.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p4.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p4.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p4.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p4.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP4 = p4;

                List<SelectListItem> p5 = new List<SelectListItem>();
                p5.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p5.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p5.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p5.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p5.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP5 = p5;

                List<SelectListItem> p6 = new List<SelectListItem>();
                p6.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p6.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p6.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p6.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p6.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP6 = p6;

                List<SelectListItem> p7 = new List<SelectListItem>();
                p7.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p7.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p7.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p7.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p7.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP7 = p7;

                List<SelectListItem> p8 = new List<SelectListItem>();
                p8.Add(new SelectListItem { Text = "Nunca", Value = "0" });
                p8.Add(new SelectListItem { Text = "Más de una vez al mes", Value = "1" });
                p8.Add(new SelectListItem { Text = "Mensualmente", Value = "2" });
                p8.Add(new SelectListItem { Text = "Semanalmente", Value = "3" });
                p8.Add(new SelectListItem { Text = "A diario o casi diario", Value = "4" });
                ViewBag.vP8 = p8;

                List<SelectListItem> p9 = new List<SelectListItem>();
                p9.Add(new SelectListItem { Text = "N0", Value = "0" });
                p9.Add(new SelectListItem { Text = "Si, pero no el curso del último año", Value = "2" });
                p9.Add(new SelectListItem { Text = "Sí, el último año", Value = "4" });
                ViewBag.vP9 = p9;

                List<SelectListItem> p10 = new List<SelectListItem>();
                p10.Add(new SelectListItem { Text = "N0", Value = "0" });
                p10.Add(new SelectListItem { Text = "Si, pero no el curso del último año", Value = "2" });
                p10.Add(new SelectListItem { Text = "Sí, el último año", Value = "4" });
                ViewBag.vP10 = p10;
                #endregion

                #region Medicamentos
                List<SelectListItem> m1 = new List<SelectListItem>();
                m1.Add(new SelectListItem { Text = "No", Value = "No" });
                m1.Add(new SelectListItem { Text = "Sí", Value = "Sí" });
                ViewBag.vM1 = m1;

                List<SelectListItem> m4 = new List<SelectListItem>();
                m4.Add(new SelectListItem { Text = "No", Value = "No" });
                m4.Add(new SelectListItem { Text = "Sí", Value = "Sí" });
                m4.Add(new SelectListItem { Text = "Automedicado", Value = "Automedicado" });
                ViewBag.vM4 = m4;
                #endregion

                return View(repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = fecha }).ToList());
            }
        }

        public IActionResult NuevoFagerstrom(int idhistorico, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int accion)
        {
            FagerstromModel Nicotina = new FagerstromModel();
            Nicotina.idhistorico = idhistorico;
            Nicotina.p1 = p1;
            Nicotina.p2 = p2;
            Nicotina.p3 = p3;
            Nicotina.p4 = p4;
            Nicotina.p5 = p5;
            Nicotina.p6 = p6;
            Nicotina.p7 = p7;
            Nicotina.usuario = SessionHelper.GetName(User);
            Nicotina.accion = accion;

            string resultado = "Ok";

            repo.Getdosparam2("sp_medicos_agrega_actualiza_fagerstrom", Nicotina);
            return Json(resultado);
        }

        public IActionResult AddUpdTestAudit(int idHistorico, int pregunta1, int pregunta2, int pregunta3, int pregunta4, int pregunta5, int pregunta6, int pregunta7, int pregunta8, int pregunta9, int pregunta10, int accion)
        {
            tTestAuditModel Audit = new tTestAuditModel();

            Audit.idHistorico = idHistorico;
            Audit.pregunta1 = pregunta1;
            Audit.pregunta2 = pregunta2;
            Audit.pregunta3 = pregunta3;
            Audit.pregunta4 = pregunta4;
            Audit.pregunta5 = pregunta5;
            Audit.pregunta6 = pregunta6;
            Audit.pregunta7 = pregunta7;
            Audit.pregunta8 = pregunta8;
            Audit.pregunta9 = pregunta9;
            Audit.pregunta10 = pregunta10;
            Audit.accion = accion;

            string resultado = "Ok";

            repo.Getdosparam2("sp_medicos_agrega_actualiza_test_audit", Audit);
            return Json(resultado);
        }

        public IActionResult AddUpdMedicamentos(int idHistorico, string padeceenfermedad, string enfermedad, string tomamedicamento, string medicamento, string cantidad, string tiempo, string consumiodroga, string droga, string frecuenciadroga, string cantidaddroga, string usuario, string cReceta, int accion)
        {
            tMedicamentosModel Medical = new tMedicamentosModel();

            Medical.idHistorico = idHistorico;
            Medical.padeceenfermedad = padeceenfermedad;
            Medical.enfermedad = enfermedad;
            Medical.tomamedicamento = tomamedicamento;
            Medical.medicamento = medicamento;
            Medical.cantidad = cantidad;
            Medical.tiempo = tiempo;
            Medical.consumiodroga = consumiodroga;
            Medical.droga = droga;
            Medical.frecuenciadroga = frecuenciadroga;
            Medical.cantidaddroga = cantidaddroga;
            Medical.usuario = SessionHelper.GetName(User);
            Medical.cReceta = cReceta;
            Medical.accion = accion;

            string resultado = "Ok";

            repo.Getdosparam2("sp_medicos_agrega_actualiza_medicamentos", Medical);
            return Json(resultado);
        }

        public JsonResult ObtenerDatosTest(int idhistorico, int test)
        {
            return Json(repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_Test_enfermeria", new { @idhistorico = idhistorico, @test = test }).FirstOrDefault());
        }

    }
}
