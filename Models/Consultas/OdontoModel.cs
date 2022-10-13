using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Consultas
{
    public class OdontoModel
    {
        public int idhistorico { get; set; }
        public string evaluado { get; set; }
        public string fechaAlta { get; set; }
        public string dependencia { get; set; }
        public string codigoevaluado { get; set; }
        public string folio { get; set; }
        public string sexo { get; set; }
        public int hayOdo { get; set; }
        public int hayTat { get; set; }
        public string descripcion { get; set; }
        public byte[] imgTatuajeRecuperado { get; set; }
        public string odontologa { get; set; }
        public string medico { get; set; }
        public string evaluacion { get; set; }
    }
}
