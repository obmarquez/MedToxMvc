using MedToxMVC.Data;
using MedToxMVC.Helper;
using MedToxMVC.Models.Consultas;
using MedToxMVC.Models.Medicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Controllers
{
    [Authorize]

    public class MedicoController : Controller
    {
        private DBOperaciones repo;

        public MedicoController()
        {
            repo = new DBOperaciones();
        }

        [Authorize(Roles = "Administrador, Medico")]
        public IActionResult IndexMedico()
        {
            var userMedico = SessionHelper.GetName(User);

            return View(repo.Getdosparam1<MedicoModel>("sp_medicos_medico_obtener_evaluaciones_a_realizar", new { @usuarioMedico = userMedico }).ToList());
        }

        public IActionResult HistoriaMedica(int idhistorico)
        {
            //Para combo estado civil
            List<SelectListItem> eCivil = new List<SelectListItem>();
            eCivil.Add(new SelectListItem { Text = "Casado", Value = "Casado" });
            eCivil.Add(new SelectListItem { Text = "Divorciado", Value = "Divorciado" });
            eCivil.Add(new SelectListItem { Text = "Soltero", Value = "Soltero" });
            eCivil.Add(new SelectListItem { Text = "Union libre", Value = "Union libre" });
            eCivil.Add(new SelectListItem { Text = "Viudo", Value = "Viudo" });
            ViewBag.ecivil = eCivil;

            //Para combo preferencia sexual
            List<SelectListItem> preferencia = new List<SelectListItem>();
            preferencia.Add(new SelectListItem { Text = "Heterosexual", Value = "Heterosexual" });
            preferencia.Add(new SelectListItem { Text = "Homosexual", Value = "Homosexual" });
            preferencia.Add(new SelectListItem { Text = "Bisexual", Value = "Bisexual" });
            ViewBag.preferencia = preferencia;

            //Para combo Audit
            List<SelectListItem> comboAudit = new List<SelectListItem>();
            comboAudit.Add(new SelectListItem { Text = "", Value = "" });
            comboAudit.Add(new SelectListItem { Text = "I", Value = "I" });
            comboAudit.Add(new SelectListItem { Text = "II", Value = "II" });
            comboAudit.Add(new SelectListItem { Text = "III", Value = "III" });
            comboAudit.Add(new SelectListItem { Text = "IV", Value = "IV" });
            ViewBag.cAudit = comboAudit;

            ViewBag.losSupMed = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_usuarios", new { @opcion = 3 }).ToList();                    

            //Todos los estados
            ViewBag.losEstados = repo.Getdosparam1<ConsultasModel>("sp_general_obtener_estados", null).ToList();

            //Pestaña de Ficha de Identificacion
            ViewBag.ficha = repo.Getdosparam1<fichaIdentificacion>("sp_medicos_historia_clinica_obtener_ficha_identificacion", new { @idhistorico = idhistorico }).FirstOrDefault();

            #region Ubicacion y medicamentos
            //Pestaña Ficha, Ubicación y Medicamentos
            ViewBag.ubi_med = repo.Getdosparam1<UbicacionMedicamentoModel>("sp_medicos_historia_clinica_obtener_ubicacionmedicamentos", new { @idhistorico = idhistorico }).FirstOrDefault();

            //Todos los municipio involucrados en la pesta de Ubicacion y Medicamentps
            ViewBag.municipio_nacio = (repo.Getdosparam1<ConsultasModel>("sp_medicos_historia_clinica_obtener_municipios", new { @idEstado = ViewBag.ubi_med.nEstadonacio, @opcion = 2 }).ToList());
            ViewBag.municipio_trabaja = (repo.Getdosparam1<ConsultasModel>("sp_medicos_historia_clinica_obtener_municipios", new { @idEstado = ViewBag.ubi_med.nEstadotrabaja, @opcion = 1 }).ToList());
            ViewBag.municipio_vive = (repo.Getdosparam1<ConsultasModel>("sp_medicos_historia_clinica_obtener_municipios", new { @idEstado = ViewBag.ubi_med.nEstadovive, @opcion = 1 }).ToList());
            #endregion

            #region Antecedentes heredofamiliarea - no patologicos
            //Pestaña Antecedentes heredofamiliares - no patologicos
            //Para combo familiar
            List<SelectListItem> familiar = new List<SelectListItem>();
            familiar.Add(new SelectListItem { Text = "", Value = "" });
            familiar.Add(new SelectListItem { Text = "Abuelos", Value = "Abuelos" });
            familiar.Add(new SelectListItem { Text = "Hermanos", Value = "Hermanos" });
            familiar.Add(new SelectListItem { Text = "Hijos", Value = "Hijos" });
            familiar.Add(new SelectListItem { Text = "Madre", Value = "Madre" });
            familiar.Add(new SelectListItem { Text = "Padre", Value = "Padre" });
            familiar.Add(new SelectListItem { Text = "Primos", Value = "Primos" });
            familiar.Add(new SelectListItem { Text = "Tios", Value = "Tios" });
            ViewBag.familiares = familiar;

            //Obtener datos de pestaña antecedentes heredofamiliarea - no patológicos
            //Para combo higiene y alimentación
            List<SelectListItem> hig_ali = new List<SelectListItem>();
            hig_ali.Add(new SelectListItem { Text = "Buena", Value = "Buena" });
            hig_ali.Add(new SelectListItem { Text = "Regular", Value = "Regular" });
            hig_ali.Add(new SelectListItem { Text = "Mala", Value = "Mala" });
            ViewBag.hig_ali = hig_ali;

            //Para combo Inmunización
            List<SelectListItem> inmunizacion = new List<SelectListItem>();
            inmunizacion.Add(new SelectListItem { Text = "Completa", Value = "Completa" });
            inmunizacion.Add(new SelectListItem { Text = "Incompleta", Value = "Incompleta" });
            ViewBag.inmunizacion = inmunizacion;            

            ViewBag.antHerNoPat = repo.Getdosparam1<antHeredofamiliarNoPatologico>("sp_medicos_historia_clinica_obtener_antecedentes_heredo_nopatologicos", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region Antecedentes patologicos
            ViewBag.antecentespatologico = repo.Getdosparam1<antecedentePatologicoModel>("sp_medicos_historia_clinica_obtener_antecedentes_patologicos", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region Ginecologicos
            ViewBag.ginecologico = repo.Getdosparam1<ginecoModel>("sp_medicos_historia_clinica_obtener_antecedentes_ginecologicos", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region Andregnico
            ViewBag.andregonico = repo.Getdosparam1<androgenicoModel>("sp_medicos_historia_clinica_obtener_antecedentes_androgenico", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region Anamnesis
            ViewBag.anamnesis = repo.Getdosparam1<anamnesisModel>("sp_medicos_historia_clinica_obtener_anamnesis", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region interrogatorio
            ViewBag.interrogatorio = repo.Getdosparam1<interrogatorioModel>("sp_medicos_historia_clinica_obtener_interrogatorio", new { @idhistorico = idhistorico }).FirstOrDefault();
            #endregion

            #region Observaciones
            ViewBag.ObservacionesPublicas = repo.Getdosparam1<ConsultasModel>("sp_general_observacionpublica_area", new { @idHistorico = idhistorico, @idArea = 2, @accion = 1, @ido = 0 }).ToList();
            ViewBag.ObservacionCustodia = repo.Getdosparam1<ConsultasModel>("sp_general_observacionCustodia", new { @idHistorico = idhistorico }).ToList();
            #endregion

            #region cie10
            //ViewData["cie10"] = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_cie10", null).ToList();
            var loscie10 = repo.Getdosparam1<ConsultasModel>("sp_medicos_obtener_cie10", null).ToList();
            var losci = new SelectList(loscie10, "clave", "dxCie10");
            ViewData["cie10"] = losci;
            #endregion

            ViewBag.sIdH = idhistorico;

            return View();
        }

        public JsonResult GetMunicpios(int id, int opcion)
        {
            //return Json(repo.Getdosparam1<ConsultasModel>("sp_general_obtener_municipios", new { @id = id }).ToList());
            return Json(repo.Getdosparam1<ConsultasModel>("sp_medicos_historia_clinica_obtener_municipios", new { @idEstado = id, @opcion = opcion }).ToList());
        }

        public IActionResult AddUpdAntecedentesHerdofamiliares(int p_idh, bool p_dm_sn, string p_dm_quien, string p_cDiabetes, bool p_has_sn, string p_has_quien, string p_cHipertension, bool p_ep_sn, string p_ep_quien, string p_cNeurologicos, bool p_tb_sn, string p_tb_quien, string p_cTuberculosis, bool p_as_sn, string p_as_qiien, string p_cAsma, bool p_ca_sn, string p_ca_quien, string p_cCancer, bool p_card_sn, string p_card_quien, string p_cCardiopatias, bool p_hepa_sn, string p_hepa_quien, string p_cHepatopatias, bool p_nefr_sn, string p_nefr_quien, string p_cNefropatias, bool p_bHematologicos, string p_cHema_quien, string p_cHematologicos, string p_cHorario, string p_cFuncion, bool p_np_ejercicio, string p_np_higiene, string p_np_habitacion, bool p_np_arma, string p_cArma, bool p_np_vehiculo, string p_cVehiculo, bool p_np_agua, bool p_np_drenaje, bool p_np_gas, bool p_np_hacinamiento, bool p_np_electr, bool p_np_zoonosis, string p_chigiene2, string p_np_alimento, string p_np_inmunizac, string p_cOcupacion, string p_cObserva, string p_cFisicos, string p_cQuimicos, string p_cMecanico, string p_cBiologico, string p_cPsicosocial)
        {
            antHeredofamiliarNoPatologico antHerNoPat = new antHeredofamiliarNoPatologico();
            antHerNoPat.idhistorico = p_idh;
            antHerNoPat.dm_sn = p_dm_sn;
            antHerNoPat.dm_quien = p_dm_quien;
            antHerNoPat.cDiabetes = p_cDiabetes;
            antHerNoPat.has_sn = p_has_sn;
            antHerNoPat.has_quien = p_has_quien;
            antHerNoPat.cHipertension = p_cHipertension;
            antHerNoPat.ep_sn = p_ep_sn;
            antHerNoPat.ep_quien = p_ep_quien;
            antHerNoPat.cNeurologicos = p_cNeurologicos;
            antHerNoPat.tb_sn = p_tb_sn;
            antHerNoPat.tb_quien = p_tb_quien;
            antHerNoPat.cTuberculosis = p_cTuberculosis;
            antHerNoPat.as_sn = p_as_sn;
            antHerNoPat.as_qiien = p_as_qiien;
            antHerNoPat.cAsma = p_cAsma;
            antHerNoPat.ca_sn = p_ca_sn;
            antHerNoPat.ca_quien = p_ca_quien;
            antHerNoPat.cCancer = p_cCancer;
            antHerNoPat.card_sn = p_card_sn;
            antHerNoPat.card_quien = p_card_quien;
            antHerNoPat.cCardiopatias = p_cCardiopatias;
            antHerNoPat.hepa_sn = p_hepa_sn;
            antHerNoPat.hepa_quien = p_hepa_quien;
            antHerNoPat.cHepatopatias = p_cHepatopatias;
            antHerNoPat.nefr_sn = p_nefr_sn;
            antHerNoPat.nefr_quien = p_nefr_quien;
            antHerNoPat.cNefropatias = p_cNefropatias;
            antHerNoPat.bHematologicos = p_bHematologicos;
            antHerNoPat.cHema_quien = p_cHema_quien;
            antHerNoPat.cHematologicos = p_cHematologicos;
            antHerNoPat.cHorario = p_cHorario;
            antHerNoPat.cFuncion = p_cFuncion;
            antHerNoPat.np_ejercicio = p_np_ejercicio;
            antHerNoPat.np_higiene = p_np_higiene;
            antHerNoPat.np_habitacion = p_np_habitacion;
            antHerNoPat.np_arma = p_np_arma;
            antHerNoPat.cArma = p_cArma;
            antHerNoPat.np_vehiculo = p_np_vehiculo;
            antHerNoPat.cVehiculo = p_cVehiculo;
            antHerNoPat.np_agua = p_np_agua;
            antHerNoPat.np_drenaje = p_np_drenaje;
            antHerNoPat.np_gas = p_np_gas;
            antHerNoPat.np_hacinamiento = p_np_hacinamiento;
            antHerNoPat.np_electr = p_np_electr;
            antHerNoPat.np_zoonosis = p_np_zoonosis;
            antHerNoPat.chigiene2 = p_chigiene2;
            antHerNoPat.np_alimento = p_np_alimento;
            antHerNoPat.np_inmunizac = p_np_inmunizac;
            antHerNoPat.cOcupacion = p_cOcupacion;
            antHerNoPat.cObserva = p_cObserva;
            antHerNoPat.cFisicos = p_cFisicos;
            antHerNoPat.cQuimicos = p_cQuimicos;
            antHerNoPat.cMecanico = p_cMecanico;
            antHerNoPat.cBiologico = p_cBiologico;
            antHerNoPat.cPsicosocial = p_cPsicosocial;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_clinica", antHerNoPat);
            return Json(resultado);
        }

        public IActionResult AddUpdClinica2(int p_idhistorico, string p_pt_congenita, string p_pt_infancia, string p_pt_neurologica, string p_pt_quirurgica, string p_pt_trauma, string p_pt_alergico, string p_pt_transfusion, string p_pt_intoxica, string p_pt_hospiltal, string p_pt_cronodeg, string p_cOservapatologicos, bool p_np_tabaco, string p_np_cigarros, string p_np_anios, string p_cit, string p_caudit, bool p_np_alcohol, string p_np_bebida, string p_np_frec_bebida, bool  p_np_toxico, string p_np_cual_toxico, string p_np_tiempo, string p_cObservatox)
        {
            antecedentePatologicoModel clinica2 = new antecedentePatologicoModel();
            clinica2.idhistorico = p_idhistorico;
            clinica2.pt_congenita = p_pt_congenita;
            clinica2.pt_infancia = p_pt_infancia;
            clinica2.pt_neurologica = p_pt_neurologica;
            clinica2.pt_quirurgica = p_pt_quirurgica;
            clinica2.pt_trauma = p_pt_trauma;
            clinica2.pt_alergico = p_pt_alergico;
            clinica2.pt_transfusion = p_pt_transfusion;
            clinica2.pt_intoxica = p_pt_intoxica;
            clinica2.pt_hospiltal = p_pt_hospiltal;
            clinica2.pt_cronodeg = p_pt_cronodeg;
            clinica2.cOservapatologicos = p_cOservapatologicos;
            clinica2.np_tabaco = p_np_tabaco;
            clinica2.np_cigarros = p_np_cigarros;
            clinica2.np_anios = p_np_anios;
            clinica2.cit = p_cit;
            clinica2.caudit = p_caudit;
            clinica2.np_alcohol = p_np_alcohol;
            clinica2.np_bebida = p_np_bebida;
            clinica2.np_frec_bebida = p_np_frec_bebida;
            clinica2.np_toxico = p_np_toxico;
            clinica2.np_cual_toxico = p_np_cual_toxico;
            clinica2.np_tiempo = p_np_tiempo;
            clinica2.cObservatox = p_cObservatox;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_clinica2", clinica2);
            return Json(resultado);
        }
    
        public IActionResult AddUpdClinica3(int p_idh, string p_gn_mena, string p_gn_ritmo, string p_gn_fum, string p_gn_ivsa, string p_gn_fup, string p_gn_gesta, string p_gn_parto, string p_gn_cesarea, string p_gn_aborto, string p_gn_fpp, string p_gn_complicac, string p_gn_anticon, string p_gn_docma, string p_gn_docacu, string p_gn_numpar, string p_gn_prefiere, string p_cObservagineco, string p_cEts)
        {
            ginecoModel clinica3 = new ginecoModel();
            clinica3.idhistorico = p_idh;
            clinica3.gn_mena = p_gn_mena;
            clinica3.gn_ritmo = p_gn_ritmo;
            clinica3.gn_fum = p_gn_fum;
            clinica3.gn_ivsa = p_gn_ivsa;
            clinica3.gn_fup = p_gn_fup;
            clinica3.gn_gesta = p_gn_gesta;
            clinica3.gn_parto = p_gn_parto;
            clinica3.gn_aborto = p_gn_aborto;
            clinica3.gn_cesarea = p_gn_cesarea;
            clinica3.gn_aborto = p_gn_aborto;
            clinica3.gn_fpp = p_gn_fpp;
            clinica3.gn_complicac = p_gn_complicac;
            clinica3.gn_anticon = p_gn_anticon;
            clinica3.gn_docma = p_gn_docma;
            clinica3.gn_docacu = p_gn_docacu;
            clinica3.gn_numpar = p_gn_numpar;
            clinica3.gn_prefiere = p_gn_prefiere;
            clinica3.cObservagineco = p_cObservagineco;
            clinica3.cEts = p_cEts;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_clinica3", clinica3);
            return Json(resultado);
        }

        public IActionResult AddUpdClinica4(int p_idh, string p_an_pubertad, string p_an_barba, string p_an_ivisa, string p_an_parejas, string p_an_preferencia, string p_cEtsandro, string p_cOBservaandro)
        {
            androgenicoModel clinica4 = new androgenicoModel();
            clinica4.idhistorico = p_idh;
            clinica4.an_pubertad = p_an_pubertad;
            clinica4.an_barba = p_an_barba;
            clinica4.an_ivisa = p_an_ivisa;
            clinica4.an_parejas = p_an_parejas;
            clinica4.an_preferencia = p_an_preferencia;
            clinica4.cEtsandro = p_cEtsandro;
            clinica4.cOBservaandro = p_cOBservaandro;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_clinica4", clinica4);
            return Json(resultado);
        }

        public IActionResult AddUpdAnamnesis(int p_idh, bool p_bVariacion, bool p_bApetito, bool p_bSed, bool p_bFiebre, bool p_bEscalofrio, bool p_bDiaforesis, bool p_bAdinamia, bool p_bMalestar, bool p_bPrurito, bool p_bLesiones, bool p_bAlteraciones, bool p_bHalitosis, bool p_bDisfagia, bool p_bReflujo, bool p_bAnorexia, bool p_bHiporexia, bool p_bOdinofagia, bool p_bPolipdipsia, bool p_bNauseas, bool p_bVomito, bool p_bDispepsia, bool p_bRectorragia, bool p_bMelena, bool p_bPirosis, bool p_bHematemesis, bool p_bAcolia, bool p_bMeteorismo, bool p_bTenesmo, string p_cObservadigestivo, bool p_bDolor, bool p_bDisnea, bool p_bHemoptisis, bool p_bSibilancias, bool p_bCianosis, bool p_bTos, bool p_bExpectoracion, bool p_bOrtopnea, string p_cObservarespiratorio, bool p_bPrecordial, bool p_bEdema, bool p_bDisneacardiovascular, bool p_bPalpitacion, bool p_bSincope, bool p_bClaudicacion, string p_cObservacardiovascular, bool p_bLumbar, bool p_bDisuria, bool p_bPolaquiuria, bool p_bIncontinencia, bool p_bPoliuria, bool p_bOliguria, bool p_bNicturia, bool p_bHematuria, bool p_bTenesmourinario, bool p_bAnuria, string p_cObservaurinario, bool p_bHipermenorrea, bool p_bHipomenorrea, bool p_bAmenorrea, bool p_bDispareunia, bool p_bMetrorragia, bool p_bLeucorrea, bool p_bDismenorrea, string p_cObservagenital, bool p_bCefalea, bool p_bConvulsiones, bool p_bObnubilacion, bool p_bMarcha, bool p_bMemoria, bool p_bEquilibrio, bool p_bLenguaje, bool p_bVigilia, bool p_bSensibilidad, bool p_bParalisis, string p_cObservanervioso, bool p_bBocio, bool p_bLeargia, bool p_bIntolerancia, bool p_bBochornos, string p_cObservaendocrino, bool p_bDiplopia, bool p_bOcular, bool p_bFotobia, bool p_bAmaurosis, bool p_bFotopsias, bool p_bMiodesopsias, bool p_bEscozor, bool p_bLeganas, string p_cObservaoftamologico, bool p_bOtalgia, bool p_bOtorrea, bool p_bOtorragia, bool p_bHipoacusia, bool p_bEpistaxis, bool p_bRinorrea, bool p_bOdinofagiaotorrino, bool p_bFonacion, string p_cObservaotorrino, bool p_bFuerza, bool p_bDeformidades, bool p_bMialgias, bool p_bArtralgias, bool p_bRigidez, bool p_bEdemalocomotor, string p_cObservalocomotor)
        {
            anamnesisModel anam = new anamnesisModel();
            anam.idhistorico = p_idh;
            anam.bVariacion = p_bVariacion;             anam.bApetito = p_bApetito;                             anam.bSed = p_bSed;                                     anam.bFiebre = p_bFiebre;           anam.bEscalofrio = p_bEscalofrio;
            anam.bDiaforesis = p_bDiaforesis;           anam.bAdinamia = p_bAdinamia;                           anam.bMalestar = p_bMalestar;
            anam.bPrurito = p_bPrurito;                 anam.bLesiones = p_bLesiones;                           anam.bAlteraciones = p_bAlteraciones;
            anam.bHalitosis = p_bHalitosis;             anam.bDisfagia = p_bDisfagia;                           anam.bReflujo = p_bReflujo;                             anam.bAnorexia = p_bAnorexia;       anam.bHiporexia = p_bHiporexia;
            anam.bOdinofagia = p_bOdinofagia;           anam.bPolipdipsia = p_bPolipdipsia;                     anam.bNauseas = p_bNauseas;                             anam.bVomito = p_bVomito;           anam.bDispepsia = p_bDispepsia;
            anam.bRectorragia = p_bRectorragia;         anam.bMelena = p_bMelena;                               anam.bPirosis = p_bPirosis;                             anam.bHematemesis = p_bHematemesis; anam.bAcolia = p_bAcolia;
            anam.bMeteorismo = p_bMeteorismo;           anam.bTenesmo = p_bTenesmo;                             anam.cObservadigestivo = p_cObservadigestivo;
            anam.bDolor = p_bDolor;                     anam.bDisnea = p_bDisnea;                               anam.bHemoptisis = p_bHemoptisis;                       anam.bSibilancias = p_bSibilancias; anam.bCianosis = p_bCianosis;
            anam.bTos = p_bTos;                         anam.bExpectoracion = p_bExpectoracion;                 anam.bOrtopnea = p_bOrtopnea;                           anam.cObservarespiratorio = p_cObservarespiratorio;
            anam.bPrecordial = p_bPrecordial;           anam.bEdema = p_bEdema;                                 anam.bDisneacardiovascular = p_bDisneacardiovascular;   anam.bPalpitacion = p_bPalpitacion; anam.bSincope = p_bSincope;
            anam.bClaudicacion = p_bClaudicacion;       anam.cObservacardiovascular = p_cObservacardiovascular;
            anam.bLumbar = p_bLumbar;                   anam.bDisuria = p_bDisuria;                             anam.bPolaquiuria = p_bPolaquiuria;                     anam.bIncontinencia = p_bIncontinencia;
            anam.bPoliuria = p_bPoliuria;               anam.bOliguria = p_bOliguria;                           anam.bNicturia = p_bNicturia;                           anam.bHematuria = p_bHematuria;
            anam.bTenesmourinario = p_bTenesmourinario; anam.bAnuria = p_bAnuria;                               anam.cObservaurinario = p_cObservaurinario;
            anam.bHipermenorrea = p_bHipermenorrea;     anam.bHipomenorrea = p_bHipomenorrea;                   anam.bAmenorrea = p_bAmenorrea;                         anam.bDispareunia = p_bDispareunia; anam.bMetrorragia = p_bMetrorragia;
            anam.bLeucorrea = p_bLeucorrea;             anam.bDismenorrea = p_bDismenorrea;                     anam.cObservagenital = p_cObservagenital;
            anam.bCefalea = p_bCefalea;                 anam.bConvulsiones = p_bConvulsiones;                   anam.bObnubilacion = p_bObnubilacion;                   anam.bMarcha = p_bMarcha;           anam.bMemoria = p_bMemoria;
            anam.bEquilibrio = p_bEquilibrio;           anam.bLenguaje = p_bLenguaje;                           anam.bVigilia = p_bVigilia;                             anam.bSensibilidad = p_bSensibilidad;
            anam.bParalisis = p_bParalisis;             anam.cObservanervioso = p_cObservanervioso;
            anam.bBocio = p_bBocio;                     anam.bLeargia = p_bLeargia;                             anam.bIntolerancia = p_bIntolerancia;                   anam.bBochornos = p_bBochornos;
            anam.cObservaendocrino = p_cObservaendocrino;
            anam.bDiplopia = p_bDiplopia;               anam.bOcular = p_bOcular;                               anam.bFotobia = p_bFotobia;                             anam.bAmaurosis = p_bAmaurosis;     anam.bFotopsias = p_bFotopsias;
            anam.bMiodesopsias = p_bMiodesopsias;       anam.bEscozor = p_bEscozor;                             anam.bLeganas = p_bLeganas;                             anam.cObservaoftamologico = p_cObservaoftamologico;
            anam.bOtalgia = p_bOtalgia;                 anam.bOtorrea = p_bOtorrea;                             anam.bOtorragia = p_bOtorragia;                         anam.bHipoacusia = p_bHipoacusia;   anam.bEpistaxis = p_bEpistaxis;
            anam.bRinorrea = p_bRinorrea;               anam.bOdinofagiaotorrino = p_bOdinofagiaotorrino;       anam.bFonacion = p_bFonacion;                           anam.cObservaotorrino = p_cObservaotorrino;
            anam.bFuerza = p_bFuerza;                   anam.bDeformidades = p_bDeformidades;                   anam.bMialgias = p_bMialgias;                           anam.bArtralgias = p_bArtralgias;   anam.bRigidez = p_bRigidez;
            anam.bEdemalocomotor = p_bEdemalocomotor;   anam.cObservalocomotor = p_cObservalocomotor;
            anam.cUsuario = SessionHelper.GetName(User);

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_anamnesis", anam);
            return Json(resultado);
        }

        public IActionResult AddUpdClinica5(int p_idh, string p_pa_tension, string p_pa_frec_card, string p_pa_frec_resp, string p_pa_temperatura, string p_pa_peso, string p_pa_masa, string p_pCintura, string p_pa_talla, bool p_bElectro, bool p_bOpto, bool p_bPlanto, bool p_nutricion, string p_cHabitus, string p_pa_cabeza, string p_pa_cuello, string p_pa_torax, string p_pa_abdomen, string p_pa_genito_uri, string p_pa_muscular, string p_pa_neurologia, string p_cObservaInt, string p_pa_electro, string p_cOptometria, string p_cPlantoscopia)
        {
            interrogatorioModel interroga = new interrogatorioModel();
            interroga.idhistorico = p_idh;
            interroga.pa_tension = p_pa_tension;
            interroga.pa_frec_card = p_pa_frec_card;
            interroga.pa_frec_resp = p_pa_frec_resp;
            interroga.pa_temperatura = p_pa_temperatura;
            interroga.pa_peso = p_pa_peso;
            interroga.pa_masa = p_pa_masa;
            interroga.bElectro = p_bElectro;
            interroga.bOpto = p_bOpto;
            interroga.bPlanto = p_bPlanto;
            interroga.pCintura = p_pCintura;
            interroga.cHabitus = p_cHabitus;
            interroga.pa_cabeza = p_pa_cabeza;
            interroga.pa_cuello = p_pa_cuello;
            interroga.pa_torax = p_pa_torax;
            interroga.pa_abdomen = p_pa_abdomen;
            interroga.pa_genito_uri = p_pa_genito_uri;
            interroga.pa_muscular = p_pa_muscular;
            interroga.pa_neurologia = p_pa_neurologia;
            interroga.cObservaInt = p_cObservaInt;
            interroga.pa_electro = p_pa_electro;
            interroga.cOptometria = p_cOptometria;
            interroga.cPlantoscopia = p_cPlantoscopia;
            interroga.pa_talla = p_pa_talla;
            interroga.nutricion = p_nutricion;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_upd_clinica5", interroga);
            return Json(resultado);
        }

        public IActionResult AddCie10(int p_idh, string p_cClave, bool p_principal)
        {
            pToxIdeModel pTox = new pToxIdeModel();
            pTox.idhistorico = p_idh;
            pTox.cClave = p_cClave;
            pTox.principal = p_principal;

            string resultado = "Ok";
            repo.Getdosparam2("sp_medicos_historia_clinica_add_cie10", pTox);
            return Json(resultado);
        }

        public IActionResult ReporteIntegral(int idhistorico)
        {
            ViewBag.idhistorico = idhistorico;

            //Para combo Dx
            List<SelectListItem> comboDx = new List<SelectListItem>();
            comboDx.Add(new SelectListItem { Text = "Riesgo bajo", Value = "Riesgo bajo" });
            comboDx.Add(new SelectListItem { Text = "Riesgo medio", Value = "Riesgo medio" });
            comboDx.Add(new SelectListItem { Text = "Riesgo alto", Value = "Riesgo alto" });
            comboDx.Add(new SelectListItem { Text = "No cubre el perfil", Value = "No cubre el perfil" });
            comboDx.Add(new SelectListItem { Text = "No presentó", Value = "No presentó" });
            ViewBag.diagnostico = comboDx;

            return View(repo.Getdosparam1<reporteIntegralModel>("sp_medicos_reporte_integral_obtener_clinica_rint", new { @idhistorico = idhistorico }).FirstOrDefault());
        }

        public IActionResult GrabarActualizarReporteIntegral(reporteIntegralModel repInt)
        {
            repo.Getdosparam2("sp_medicos_reporte_integral_agregar_actualizar_clinica_rint", repInt);

            return Redirect(Url.Action("IndexMedico", "Medico"));
        }
    }
}