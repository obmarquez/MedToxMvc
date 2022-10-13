using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Medicos
{
    public class anamnesisModel
    {
        public int idhistorico { get; set; }
        //Sintomas generales
        public bool bVariacion { get; set; }
        public bool bApetito { get; set; }
        public bool bSed { get; set; }
        public bool bFiebre { get; set; }
        public bool bEscalofrio { get; set; }
        public bool bDiaforesis { get; set; }
        public bool bAdinamia { get; set; }
        public bool bMalestar { get; set; }

        //Piel y faneras
        public bool bPrurito { get; set; }
        public bool bLesiones { get; set; }
        public bool bAlteraciones { get; set; }

        //Aparato digestivo
        public bool bHalitosis { get; set; }
        public bool bDisfagia { get; set; }
        public bool bReflujo { get; set; }
        public bool bAnorexia { get; set; }
        public bool bHiporexia { get; set; }
        public bool bOdinofagia { get; set; }
        public bool bPolipdipsia { get; set; }
        public bool bNauseas { get; set; }
        public bool bVomito { get; set; }
        public bool bDispepsia { get; set; }
        public bool bRectorragia { get; set; }
        public bool bMelena { get; set; }
        public bool bPirosis { get; set; }
        public bool bHematemesis { get; set; }
        public bool bAcolia { get; set; }
        public bool bMeteorismo { get; set; }
        public bool bTenesmo { get; set; }
        public string cObservadigestivo { get; set; }

        //Aparato respiratorio
        public bool bDolor { get; set; }
        public bool bDisnea { get; set; }
        public bool bHemoptisis { get; set; }
        public bool bSibilancias { get; set; }
        public bool bCianosis { get; set; }
        public bool bTos { get; set; }
        public bool bExpectoracion { get; set; }
        public bool bOrtopnea { get; set; }
        public string cObservarespiratorio { get; set; }

        //Aparato cardiovascular
        public bool bPrecordial { get; set; }
        public bool bEdema { get; set; }
        public bool bDisneacardiovascular { get; set; }
        public bool bPalpitacion { get; set; }
        public bool bSincope { get; set; }
        public bool bClaudicacion { get; set; }
        public string cObservacardiovascular { get; set; }

        //Aparato urinario
        public bool bLumbar { get; set; }
        public bool bDisuria { get; set; }
        public bool bPolaquiuria { get; set; }
        public bool bIncontinencia { get; set; }
        public bool bPoliuria { get; set; }
        public bool bOliguria { get; set; }
        public bool bNicturia { get; set; }
        public bool bHematuria { get; set; }
        public bool bTenesmourinario { get; set; }
        public bool bAnuria { get; set; }
        public string cObservaurinario { get; set; }

        //Aparato Genital
        public bool bHipermenorrea { get; set; }
        public bool bHipomenorrea { get; set; }
        public bool bAmenorrea { get; set; }
        public bool bDispareunia { get; set; }
        public bool bMetrorragia { get; set; }
        public bool bLeucorrea { get; set; }
        public bool bDismenorrea { get; set; }
        public string cObservagenital { get; set; }

        //Sistema nervioso
        public bool bCefalea { get; set; }
        public bool bConvulsiones { get; set; }
        public bool bObnubilacion { get; set; }
        public bool bMarcha { get; set; }
        public bool bMemoria { get; set; }
        public bool bEquilibrio { get; set; }
        public bool bLenguaje { get; set; }
        public bool bVigilia { get; set; }
        public bool bSensibilidad { get; set; }
        public bool bParalisis { get; set; }
        public string cObservanervioso { get; set; }

        //Endocrino
        public bool bBocio { get; set; }
        public bool bLeargia { get; set; }
        public bool bIntolerancia { get; set; }
        public bool bBochornos { get; set; }
        public string cObservaendocrino { get; set; }

        //Oftmalogico
        public bool bDiplopia { get; set; }
        public bool bOcular { get; set; }
        public bool bFotobia { get; set; }
        public bool bAmaurosis { get; set; }
        public bool bFotopsias { get; set; }
        public bool bMiodesopsias { get; set; }
        public bool bEscozor { get; set; }
        public bool bLeganas { get; set; }
        public string cObservaoftamologico { get; set; }

        //Otoriino
        public bool bOtalgia { get; set; }
        public bool bOtorrea { get; set; }
        public bool bOtorragia { get; set; }
        public bool bHipoacusia { get; set; }
        public bool bEpistaxis { get; set; }
        public bool bRinorrea { get; set; }
        public bool bOdinofagiaotorrino { get; set; }
        public bool bFonacion { get; set; }
        public string cObservaotorrino { get; set; }

        //Locomotor
        public bool bFuerza { get; set; }
        public bool bDeformidades { get; set; }
        public bool bMialgias { get; set; }
        public bool bArtralgias { get; set; }
        public bool bRigidez { get; set; }
        public bool bEdemalocomotor { get; set; }
        public string cObservalocomotor { get; set; }

        public string cUsuario { get; set; }
    }
}
