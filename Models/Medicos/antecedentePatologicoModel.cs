using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class antecedentePatologicoModel //Clinica2
    {
        public int idhistorico { get; set; }
        public string pt_congenita { get; set; }
        public string pt_infancia{ get; set; }
        public string pt_neurologica { get; set; }
        public string pt_quirurgica { get; set; }
        public string pt_trauma { get; set; }
        public string pt_alergico { get; set; }
        public string pt_transfusion { get; set; }
        public string pt_intoxica { get; set; }
        public string pt_hospiltal { get; set; }
        public string pt_cronodeg { get; set; }
        public string cOservapatologicos { get; set; }
        //---------------------------------------------------Campos restantes ubicados en tabla Clinica
        public bool np_tabaco { get; set; }
        public string np_cigarros { get; set; }
        public string np_anios { get; set; }
        public string cit { get; set; }
        public string caudit { get; set; }
        public bool np_alcohol { get; set; }
        public string np_bebida { get; set; }
        public string np_frec_bebida { get; set; }
        public bool np_toxico { get; set; }
        public string np_cual_toxico { get; set; }
        public string np_tiempo { get; set; }
        public string cObservatox { get; set; }
    }
}
