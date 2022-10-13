using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class reporteIntegralModel
    {
        public int idhistorico { get; set; }
        public string desc1 { get; set; }
        public string f_evaluacion { get; set; }
        //public string folio { get; set; }
        public string diagnostico { get; set; }
        public bool bCompromiso { get; set; }
        public bool bConfirmatorio { get; set; }
    }
}
