using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Consultas
{
    public class NutriModel
    {
        public int idhistorico { get; set; }
        public string evaluado { get; set; }
        public string puesto { get; set; }
        public int edad { get; set; }
        public string sexo { get; set; }
        public string cevaluacion { get; set; }
        public string f_registro { get; set; }
        public decimal pesoactual { get; set; }
        public decimal talla { get; set; }
        public decimal pesoideal { get; set; }
        public decimal imc { get; set; }
        public string tensionarterial { get; set; }
        public int cardiaca { get; set; }
        public int respiratoria { get; set; }
        public decimal temperatura { get; set; }
        public int hayNutricion { get; set; }
        public string supervisor { get; set; }
    }
}
