using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class fichaIdentificacion
    {
        public int idhistorico { get; set; }
        public string evaluado { get; set; }
        public int edad { get; set; }
        public string sexo { get; set; }
        public string telefono { get; set; }
        public string dependencia { get; set; }
        public string puesto { get; set; }
        public string categoria { get; set; }
        public string funcioninstitucional { get; set; }
        public string funciondeclarada { get; set; }
        public string evaluacion { get; set; }
        public string folio { get; set; }
        public string supervisor { get; set; }
        public string alias { get; set; }
    }
}
