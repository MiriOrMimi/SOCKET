using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCKET
{
    internal class Agenda
    {
        public List<Contatto> Contatti { get; set; }
        public Agenda()
        {
            Contatti = new List<Contatto> { };
        }

        public void AggiungiContatto(string nome, string ip)
        {
            Contatti.Add(new Contatto(nome, ip));
        }
    }

    internal class Contatto {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public Contatto(string n, string ip)
        {
            Nome = n;
            Indirizzo = ip;
        }
    }
    
}
