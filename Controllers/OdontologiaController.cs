using MedToxMVC.Data;
using MedToxMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedToxMVC.Models.Consultas;
using MedToxMVC.Models.Odontologias;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MedToxMVC.Models;

namespace MedToxMVC.Controllers
{
    [Authorize]
    public class OdontologiaController : Controller
    {
        static string path = @"C:\inetpub\wwwroot\fotoDental\";
        //static string path = @"C:\Dental\";

        //private long m_limagefilelenght2 = 0;
        //private byte[] m_barrImg2;


        private DBOperaciones repo;

        public OdontologiaController()
        {
            repo = new DBOperaciones();
        }

        [Authorize(Roles = "Administrador, Odontologia")]
        public IActionResult IndexOdontologia()
        {
            var userOdonto = SessionHelper.GetName(User);
            
            return View(repo.Getdosparam1<OdontoModel>("sp_medicos_odontologia_obtener_evaluaciones_a_realizar", new { @usuarioOdontologo = userOdonto }).ToList());
        }

        public IActionResult AddUpdOdontologia(int opcion, int idhistorico, string fechaAceptacion)
        {
            ViewBag.accion = opcion;
            ViewBag.idhistorico = idhistorico;
            ViewBag.fechaaceptacion = fechaAceptacion;

            ViewBag.ObservacionesPublicas = repo.Getdosparam1<ConsultasModel>("sp_general_observacionpublica_area", new { @idHistorico = idhistorico, @idArea = 2, @accion = 1, @ido = 0 }).ToList();
            ViewBag.ObservacionCustodia = repo.Getdosparam1<ConsultasModel>("sp_general_observacionCustodia", new { @idHistorico = idhistorico }).ToList();

            //Para combo habitos alimenticios
            List<SelectListItem> oclusion = new List<SelectListItem>();
            oclusion.Add(new SelectListItem { Text = "No presenta", Value = "No presenta" });
            oclusion.Add(new SelectListItem { Text = "Clase I", Value = "Clase I" });
            oclusion.Add(new SelectListItem { Text = "Clase II", Value = "Clase II" });
            oclusion.Add(new SelectListItem { Text = "Clase III", Value = "Clase III" });
            ViewBag.tipoOclusion = oclusion;

            if(opcion == 1)
            {
                return View();
            }
            else
            {
                return View(repo.Getdosparam1<OdontologiasModel>("sp_medicos_odontologia_obtener_evaluacion_idhistorico", new { @idhistorico = idhistorico }).FirstOrDefault());
            }
            
        }

        public IActionResult GrabarActualizarOdontologia(OdontologiasModel OdonObjeto)
        {
            var respuesta = repo.Getdosparam2("sp_medicos_odontologia_agregar_actualizar", OdonObjeto);

            if (respuesta.Count() == 0)
            {
                return Redirect(@Url.Action("IndexOdontologia", "Mensajes"));
            }
            else
            {
                return Redirect(@Url.Action("IndexOdontologia", "Odontologia"));
            }
            //return View();
        }

        [HttpPost]
        public IActionResult CrearFoto(string imageData, string idhistorico, string descripcion)
        {
            //--------------------------------------------------------------------------------------Este codigo crea la foto en una carpeta en el servidor
            ////string fileNameWitPath = path + DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";
            ///
            //------------------------------------archivo con extensión png
            //string fileNameWitPath = path + idhistorico + ".png";

            //------------------------------------archivo con extensión jpg
            //string fileNameWitPath = path + idhistorico + ".jpg";
            //using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
            //{
            //    using (BinaryWriter bw = new BinaryWriter(fs))
            //    {
            //        byte[] data = Convert.FromBase64String(imageData);
            //        bw.Write(data);
            //        bw.Close();
            //    }
            //}
            //--------------------------------------------------------------------------------------------------------------------------------------------

            //--------------------------------------------------------------------------------------Este codigo intenta ingresar la imagen en la base de datos de una vez

            byte[] data = Convert.FromBase64String(imageData);
            //FileInfo fimage = new FileInfo(idhistorico + ".jpg");
            //m_limagefilelenght2 = data.Length;
            //m_barrImg2 = new byte[Convert.ToInt32(data)];
            //FileStream fsimagen = new FileStream(idhistorico+".jpg", FileMode.Open, FileAccess.Read, FileShare.Read);

            TatuajeDental tatDen = new TatuajeDental();
            tatDen.idhistorico = Convert.ToInt32(idhistorico);
            tatDen.imgTatuaje = data;
            tatDen.cUsuario = SessionHelper.GetName(User);
            tatDen.descripcion = descripcion;

            repo.Getdosparam2("sp_medicos_odontologia_agregar_foto_dental", tatDen);

            return Redirect(@Url.Action("IndexOdontologia", "Odontologia"));
        }

        [Authorize(Roles = "Administrador, Odontologia")]
        public IActionResult IndexAsociar(string fecha = "")
        {
            if(fecha=="")
            {
                return View();
            }
            else
            {
                var losOdontologos = repo.Getdosparam1<Usuarios>("sp_medicos_obtener_usuarios", new { @opcion = 2 }).ToList();
                var losOdontos = new SelectList(losOdontologos, "Nombre", "NombreUsuario");
                ViewData["odontologos"] = losOdontos;

                return View(repo.Getdosparam1<ConsultasModel>("sp_medicos_odontologia_asociacion", new { @fecha = fecha }).ToList());
            }
        }

        public IActionResult js_asociar_odontologo(int p_idhistorico, string p_idodontologo)
        {
            string resp = "Error";

            var respuesta = repo.Getdosparam2("sp_medicos_asociar_areas", new { @area = 1, @idhistorico = p_idhistorico, @evaluador = p_idodontologo });

            if (respuesta.Count() == 0)
            {
                resp = "Ok";
            }

            return Json(resp);
        }
    }
}
