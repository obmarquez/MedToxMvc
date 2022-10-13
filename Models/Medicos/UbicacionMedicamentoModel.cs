using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class UbicacionMedicamentoModel
    {
        public int idhistorico { get; set; }
        //public string curp { get; set; }
        //public int idevaluacion { get; set; }
        public DateTime f_registro { get; set; }
        public string folio { get; set; }
        public string ac_religion { get; set; }
        public string ac_edocivil { get; set; }
        public string ac_mun_vive { get; set; }
        public string ac_mun_nacio { get; set; }
        public string ac_medicamento { get; set; }
        public string ac_motivo { get; set; }
        public string ac_tuso { get; set; }
        public string ac_prescrito { get; set; }
        public int nEstadotrabaja { get; set; }
        public int nEstadonacio { get; set; }
        public int nEstadovive { get; set; }
        public string cmunicipiovive2 { get; set; }
        public DateTime fModificacion { get; set; }
        public string alias { get; set; }
        public string supervisor { get; set; }
    }
}
