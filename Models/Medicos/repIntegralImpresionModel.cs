using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class repIntegralImpresionModel
    {
        public int idhistorico { get; set; }
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
        public string lugarEval { get; set; }
        public string cevaluacion { get; set; }
        public string puesto { get; set; }
        public string categoria { get; set; }
        public string funcion { get; set; }
        public string funDeclara { get; set; }
        public string FOLIO { get; set; }
        public string sintesis { get; set; }
        public string dx { get; set; }
        public string medico { get; set; }
        public string cedMed { get; set; }
        public string supervisor { get; set; }
        public string cedSup { get; set; }
        public string director { get; set; }
        public string cedDir { get; set; }
        public byte[] Picture { get; set; }
    }
}
