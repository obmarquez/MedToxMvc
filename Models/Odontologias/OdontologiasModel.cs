using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Odontologias
{
    public class OdontologiasModel
    {
        public int idhistorico { get; set; }
        public string f_registro { get; set; }
        public string od_atm { get; set; }
        public string od_labios { get; set; }
        public string od_paladar { get; set; }
        public string od_carrillos { get; set; }
        public string od_istmo { get; set; }
        public string od_lengua { get; set; }
        public string od_piso_boca { get; set; }
        public string od_encia { get; set; }
        public string od_ausentes { get; set; }
        public string od_perdidos { get; set; }
        public string od_obturados { get; set; }
        public string od_reemplezados { get; set; }
        public string od_tipooclusion { get; set; }
        public string od_observa { get; set; }
        public string recomendacion { get; set; }
        public string diagnostico { get; set; }
        public string od_diente { get; set; }
        public int  accion { get; set; }
    }
}
