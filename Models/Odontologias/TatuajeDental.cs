using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Models.Odontologias
{
    public class TatuajeDental
    {
        public int idhistorico { get; set; }
        public  byte[] imgTatuaje { get; set; }
        public string cUsuario { get; set; }
        public string descripcion { get; set; }
    }
}
