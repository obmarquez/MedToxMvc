using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Consultas
{
    public class DatosGeneralesImpresion
    {
        public string codigoevaluado { get; set; }
        public string fechaIngreso { get; set; }
        public string evaluado { get; set; }
        public string rfc { get; set; }
        public string curp { get; set; }
        public string edad { get; set; }
        public string sexo { get; set; }
        public string desc_dependencia { get; set; }
        public string desc_subdep { get; set; }
        public string comision { get; set; }
        public string cevaluacion { get; set; }
        public string puesto { get; set; }
        public string funcion { get; set; }
        public string medico { get; set; }
        public string cedMed { get; set; }
        public string supervisor { get; set; }
        public string cedSup { get; set; }
        public string director { get; set; }
        public string cedDir { get; set; }
        public string alias { get; set; }
        public string ac_edocivil { get; set; }
        public string obsest { get; set; }
        public string domicilio { get; set; }
        public string telmovil { get; set; }
        public string cAdscripcion { get; set; }
        public string ac_religion { get; set; }
        public string folio { get; set; }
        public string origen { get; set; }
        public string fregistro { get; set; }
        public byte[] Picture { get; set; }
    }
}
