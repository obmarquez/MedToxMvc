using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class antHeredofamiliarNoPatologico
    {
        public int idhistorico { get; set; }
        public bool dm_sn { get; set; }
        public string dm_quien { get; set; }
        public string cDiabetes { get; set; }
        public bool has_sn { get; set; }
        public string has_quien { get; set; }
        public string cHipertension { get; set; }
        public bool ep_sn { get; set; }
        public string ep_quien { get; set; }
        public string cNeurologicos { get; set; }
        public bool tb_sn { get; set; }
        public string tb_quien { get; set; }
        public string cTuberculosis { get; set; }
        public bool as_sn { get; set; }
        public string as_qiien { get; set; }
        public string cAsma { get; set; }
        public bool ca_sn { get; set; }
        public string ca_quien { get; set; }
        public string cCancer { get; set; }
        public bool card_sn { get; set; }
        public string card_quien { get; set; }
        public string cCardiopatias { get; set; }
        public bool hepa_sn { get; set; }
        public string hepa_quien { get; set; }
        public string cHepatopatias { get; set; }
        public bool nefr_sn { get; set; }
        public string nefr_quien { get; set; }
        public string cNefropatias { get; set; }
        public bool bHematologicos { get; set; }
        public string cHema_quien { get; set; }
        public string cHematologicos { get; set; }
        public string cHorario { get; set; }
        public string cFuncion { get; set; }
        public bool np_ejercicio { get; set; }  //Check del ejercicio
        public string np_higiene { get; set; } //Frecuencia del ejercicio
        public string np_habitacion { get; set; } //tipo del ejercicio
        public bool np_arma { get; set; } //Check del arma
        public string cArma { get; set; }  //Tipo de arma
        public bool np_vehiculo { get; set; } //Check del vehiculo
        public string cVehiculo { get; set; } //Tipo vehiculo
        public bool np_agua { get; set; }
        public bool np_drenaje { get; set; }
        public bool np_gas { get; set; }
        public bool np_hacinamiento { get; set; }
        public bool np_electr { get; set; }
        public bool np_zoonosis { get; set; }
        public string chigiene2 { get; set; }
        public string np_alimento { get; set; }
        public string np_inmunizac { get; set; }
        public string cOcupacion { get; set; }
        public string cObserva { get; set; }
        public string cFisicos { get; set; }
        public string cQuimicos { get; set; }
        public string cMecanico { get; set; }
        public string cBiologico { get; set; }
        public string cPsicosocial { get; set; }
        public bool np_tabaco { get; set; } //Check fuma en antecedentes patologicos
        public string np_cigarros { get; set; } //Caja de texto para indicar cantidad de cigarrillos
        public string np_años { get; set; } //Caja de texto cantidad de años fumando
        public string cit { get; set; } //Indice tabaquico
        public bool np_alcohol { get; set; }
        public string np_bebida { get; set; }
        public string np_frec_bebida { get; set; }
        public string cAudit { get; set; }
        public bool np_toxico { get; set; }
        public string np_cual_toxico { get; set; }
        public string np_tiempo { get; set; }
        public string cObservatox { get; set; }
    }
}
