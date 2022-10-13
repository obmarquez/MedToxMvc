using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class interrogatorioModel
    {
        public int idhistorico { get; set; }
        public string pa_tension { get; set; }
        public string pa_frec_card { get; set; }
        public string pa_frec_resp { get; set; }
        public string pa_temperatura { get; set; }
        public string pa_peso { get; set; }
        public string pa_masa { get; set; }
        public bool bElectro { get; set; }
        public bool bOpto { get; set; }
        public bool bPlanto { get; set; }
        public string pCintura { get; set; }
        public string cHabitus { get; set; }
        public string pa_cabeza { get; set; }
        public string pa_cuello { get; set; }
        public string pa_torax { get; set; }
        public string pa_abdomen { get; set; }
        public string pa_genito_uri { get; set; }
        public string pa_muscular { get; set; }
        public string pa_neurologia { get; set; }
        public string cObservaInt { get; set; }
        public string pa_electro { get; set; }
        public string cOptometria { get; set; }
        public string cPlantoscopia { get; set; }
        public string pa_talla { get; set; }
        public bool nutricion { get; set; }
    }
}
