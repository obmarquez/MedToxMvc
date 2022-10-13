using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Nutricion
{
    public class NutricionModel
    {
        public int idhistorico { get; set; }
        public decimal pesoactual { get; set; }
        public decimal talla { get; set; }
        public decimal pesoideal { get; set; }
        public decimal imc { get; set; }
        public string tensionarterial { get; set; }
        public int cardiaca { get; set; }
        public int respiratoria { get; set; }
        public decimal temperatura { get; set; }
        public string Hadesayuno { get; set; }
        public string Hacomida { get; set; }
        public string Hacena { get; set; }
        public string Hcdesayuno { get; set; }
        public string Hccomida { get; set; }
        public string Hccena { get; set; }
        public bool bebidaalcohilica { get; set; }
        public int Baveces { get; set; }
        public string padecimiento { get; set; }
        public bool bebidaembotellada { get; set; }
        public int Beveces { get; set; }
        public int frijolCantDesayuno { get; set; }
        public int toritillaCantDesayuno { get; set; }
        public int CQHCantDesayuno { get; set; }
        public int frutaCantDesayuno { get; set; }
        public int verduraCantDesayuno { get; set; }
        public int azucaresCantDesayuno { get; set; }
        public int grasaCantDesayuno { get; set; }
        public int frijolCantComida { get; set; }
        public int tortillaCantComida { get; set; }
        public int CQHCantComida { get; set; }
        public int frutaCantComida { get; set; }
        public int verdurasCantComida { get; set; }
        public int azucaresCantComida { get; set; }
        public int GrasaCantComida { get; set; }
        public int frijolCantCena { get; set; }
        public int torillaCantCena { get; set; }
        public int CQHCantCena { get; set; }
        public int verduraCantCena { get; set; }
        public int frutaCantCena { get; set; }
        public int azucaresCantCena { get; set; }
        public int grasaCantCena { get; set; }
        public string Observaciones { get; set; }
        public int accion { get; set; }
        public string alimentoGratis { get; set; }
        public string realizaActividad { get; set; }
        public string actividadFisica { get; set; }
    }
}
