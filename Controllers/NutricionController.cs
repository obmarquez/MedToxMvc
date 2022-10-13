using MedToxMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedToxMVC.Models.Consultas;
using MedToxMVC.Models.Nutricion;
using Microsoft.AspNetCore.Authorization;

namespace MedToxMVC.Controllers
{
    [Authorize]

    public class NutricionController : Controller
    {
        private DBOperaciones repo;

        public NutricionController()
        {
            repo = new DBOperaciones();
        }
        [Authorize(Roles = "Administrador, Nutricion")]
        public IActionResult Index()
        {
            //return View(repo.Getdosparam1<ConsultasModel>("sp_medicos_entrada_diaria", new { @fecha = fecha }).ToList());
            return View(repo.Getdosparam1<MedToxMVC.Models.Consultas.NutriModel>("sp_medicos_nutricion_obtener_evaluaciones_a_realizar", new { @idhistorico = 0 }).ToList());
        }

        public IActionResult AddUpdNutricion(int opcion, int idhistorico)
        {
            ViewBag.accion = opcion;
            ViewBag.idhistorico = idhistorico;

            //Para combo habitos alimenticios
            List<SelectListItem> habito = new List<SelectListItem>();
            habito.Add(new SelectListItem { Text = "Casa", Value = "Casa" });
            habito.Add(new SelectListItem { Text = "Calle", Value = "Calle" });
            habito.Add(new SelectListItem { Text = "Trabajo", Value = "Trabajo" });
            ViewBag.habitoAlimentario = habito;

            //Para combo respuesta si/no
            List<SelectListItem> respuesta = new List<SelectListItem>();
            respuesta.Add(new SelectListItem { Text = "No", Value = "No" });
            respuesta.Add(new SelectListItem { Text = "Si", Value = "Si" });
            ViewBag.respuestasino = respuesta;

            ViewBag.datosEnfermeria = repo.Getdosparam1<NutriModel>("sp_medicos_nutricion_obtener_datos_enfermeria", new { @idhistorico = idhistorico }).FirstOrDefault();

            if (opcion == 1)
            {
                return View();
            }
            else
            {
                return View(repo.Getdosparam1<NutricionModel>("sp_medicos_nutricion_obtener_evaluaciones_a_realizar", new { @idhistorico = idhistorico }).FirstOrDefault());
            }
        }

        public IActionResult GrabarActualizar(NutricionModel NutriciOscar)
        {
            var respuesta = repo.Getdosparam2("sp_medicos_nutricion_agrega_actualiza", NutriciOscar);
            if(respuesta.Count() == 0)
            {
                return Redirect(@Url.Action("IndexNutricion", "Mensajes"));
            }
            else
            {
                return Redirect(@Url.Action("Index","Nutricion"));
            }
        }

        public IActionResult obtenerDetallesMes(string mesDetalle = "")
        {
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem { Text = "Enero", Value = "Enero" });
            meses.Add(new SelectListItem { Text = "Febrero", Value = "Febrero" });
            meses.Add(new SelectListItem { Text = "Marzo", Value = "Marzo" });
            meses.Add(new SelectListItem { Text = "Abril", Value = "Abril" });
            meses.Add(new SelectListItem { Text = "Mayo", Value = "Mayo" });
            meses.Add(new SelectListItem { Text = "Junio", Value = "Junio" });
            meses.Add(new SelectListItem { Text = "Julio", Value = "Julio" });
            meses.Add(new SelectListItem { Text = "Agosto", Value = "Agosto" });
            meses.Add(new SelectListItem { Text = "Septiembre", Value = "Septiembre" });
            meses.Add(new SelectListItem { Text = "Octubre", Value = "Octubre" });
            meses.Add(new SelectListItem { Text = "Noviembre", Value = "Noviembre" });
            meses.Add(new SelectListItem { Text = "Diciembre", Value = "Diciembre" });
            ViewBag.mesesillos = meses;

            if (mesDetalle == "")
                return View();
            else
                return View(repo.Getdosparam1<NutriDetalleMes>("sp_medicos_nutricion_detallado_mensual", new { @mes = mesDetalle }).ToList());
        }
    }
}
