using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class pTatuajeModel
    {
        public int idhistorico { get; set; }
        public int idTatuajeevaluado { get; set; }
        public string cUbicacion { get; set; }
        public string cDescripcion { get; set; }
        public byte[] imgTatuaje { get; set; }
    }
}
