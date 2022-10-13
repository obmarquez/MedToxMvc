using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.EnfermeriaModel
{
    public class tMedicamentosModel
    {
        public int idHistorico { get; set; }
        public string padeceenfermedad { get; set; }
        public string enfermedad { get; set; }
        public string tomamedicamento { get; set; }
        public string medicamento { get; set; }
        public string cantidad { get; set; }
        public string tiempo { get; set; }
        public string consumiodroga { get; set; }
        public string droga { get; set; }
        public string frecuenciadroga { get; set; }
        public string cantidaddroga { get; set; }
        public string usuario { get; set; }
        public string cReceta { get; set; }
        public int accion { get; set; }
    }
}
