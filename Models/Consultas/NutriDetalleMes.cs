using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Consultas
{
    public class NutriDetalleMes
    {
        public int idhistorico { get; set; }
        public string evaluado { get; set; }
        public string fatencion { get; set; }
        public string sexo { get; set; }
        public string edad { get; set; }
        public string dependencia { get; set; }
        public string padecimiento { get; set; }
    }
}
