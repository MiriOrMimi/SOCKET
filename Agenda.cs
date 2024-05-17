using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SOCKET
{
    internal class Agenda
    {
        public List<Contatto> Contatti { get; set; }
        public Agenda()
        {
            Contatti = new List<Contatto> { };
            LeggiAgenda();
        }

        public void AggiungiContatto(string nome, string ip, string porta)
        {
            Contatti.Add(new Contatto(nome, ip, porta));
            StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Files/agenda.txt"), true);
            sw.WriteLine(nome + "," + ip + "," + porta);
            sw.Close();
        }
        public void LeggiAgenda()
        {
            StreamReader sr = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/agenda.txt"));
            string line = "";

            while (line != null)
            {
                line = sr.ReadLine();
                if (line != null)
                {
                    string[] info = line.Split(",");
                    Contatti.Add(new Contatto(info[0], info[1], info[2]) { });
                }

            }
            sr.Close();
        }
    }

    internal class Contatto
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string Porta { get; set; }
        public Contatto(string n, string ip, string porta)
        {
            Nome = n;
            Indirizzo = ip;
            Porta = porta;
        }
    }

}
