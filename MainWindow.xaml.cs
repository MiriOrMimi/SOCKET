using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace SOCKET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Agenda agenda;
        int sourcePort = 50000;
        public string Mossa { get; set; }
        public string MossaAvversario { get; set; }
        public bool modPartita { get; set; }
        public int Punti { get; set; }
        Socket s = null;
        Thread ricezione;
        Thread partita;
        public MainWindow()
        {
            InitializeComponent();
            modPartita = false;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //accettiamo ipv4 e lavoriamo con udp
            IPAddress local_address = IPAddress.Any; //con any va a recuperare automaticamente l'ip della macchina
            IPEndPoint local_endpoint = new IPEndPoint(local_address, sourcePort); //endpoint
            s.Bind(local_endpoint);
            ricezione = new Thread(ricezioneSocket);
            ricezione.Start();
           
            agenda = new Agenda();
            foreach (Contatto c in agenda.Contatti)
            {
                cbxAgenda.Items.Add(c.Nome + " " + c.Indirizzo + ":" + c.Porta);

            }
        }

        private void GestisciPartita(object? obj)
        {
            int turno = 1;
            int puntiAvversario = 0;
            while (modPartita)
            {
                Dispatcher.Invoke(() => lblPunteggio.Content = $"{turno}/3");
                if (Mossa != String.Empty && MossaAvversario != String.Empty)
                {

                    if ((Mossa == "Sasso" && MossaAvversario == "Forbici") || (Mossa == "Forbici" && MossaAvversario == "Carta") || (Mossa == "Carta" && MossaAvversario == "Sasso"))
                    {
                        Punti++;
                    }
                    else if (Mossa != MossaAvversario)
                    {
                        puntiAvversario++;
                    }
                    Dispatcher.Invoke(() => lblLocal.Content = $"TU : {Mossa}");
                    Dispatcher.Invoke(() => lblAvversario.Content = $"{cbxAgenda.SelectedItem as string} : {MossaAvversario}");
                    Dispatcher.Invoke(() => imgCarta.IsEnabled = true);
                    Dispatcher.Invoke(() => imgSasso.IsEnabled = true);
                    Dispatcher.Invoke(() => imgForbici.IsEnabled = true);
                    Mossa = String.Empty;
                    MossaAvversario = String.Empty;
                    turno++;
                }
                if (turno == 4)
                {
                    if (Punti > puntiAvversario)
                    {
                        Dispatcher.Invoke(() => lblPunteggio.Content = $"HAI VINTO {Punti} : {puntiAvversario}");
                    }
                    else if (Punti < puntiAvversario)
                    {
                        Dispatcher.Invoke(() => lblPunteggio.Content = $"HAI PERSO  {Punti} : {puntiAvversario}");
                    }
                    else
                    {
                        Dispatcher.Invoke(() => lblPunteggio.Content = $"PAREGGIO  {Punti} : {puntiAvversario}");
                    }

                    Dispatcher.Invoke(() => cbxAgenda.IsEnabled = true);
                    Dispatcher.Invoke(() => btnAggiungiContatto.IsEnabled = true);
                    Dispatcher.Invoke(() => btnChatta.IsEnabled = true);
                    Dispatcher.Invoke(() => btnGioca.IsEnabled = true);
                    Dispatcher.Invoke(() => lblLocal.Content = $"");
                    Dispatcher.Invoke(() => lblAvversario.Content = $"");

                    modPartita = false;

                }
            }
        }

        private void cbxAgenda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGioca.Visibility = Visibility.Visible;
            btnChatta.Visibility = Visibility.Visible;
            lstMessaggi.Items.Clear();  //pulisco la list box dopo il cambio del contatto
            txtInvia.Text = String.Empty;



        }

        private void btnAggiungiContatto_Click(object sender, RoutedEventArgs e)
        {
            cbxAgenda.SelectedItem = null;
            btnChatta.Visibility = Visibility.Collapsed;
            btnGioca.Visibility = Visibility.Collapsed;
            lstMessaggi.Visibility = Visibility.Collapsed;
            imgCarta.Visibility = Visibility.Collapsed;
            imgForbici.Visibility = Visibility.Collapsed;
            imgSasso.Visibility = Visibility.Collapsed;
            btnInvia.Visibility = Visibility.Collapsed;
            txtInvia.Visibility = Visibility.Collapsed;
            cbxAgenda.IsEnabled = true;

            if (txtIp.Visibility == Visibility.Visible && txtName.Visibility == Visibility.Visible)
            {

                agenda.AggiungiContatto(txtName.Text, txtIp.Text, txtPorta.Text);
                Contatto c = agenda.Contatti[agenda.Contatti.Count - 1];
                cbxAgenda.Items.Add(c.Nome + " " + c.Indirizzo + ":" + c.Porta);
                txtIp.Text = "Ip contatto";
                txtName.Text = "Nome contatto";
                txtPorta.Text = "Porta contatto";
                txtIp.Visibility = Visibility.Collapsed;
                txtName.Visibility = Visibility.Collapsed;
                txtPorta.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtIp.Visibility = Visibility.Visible;
                txtName.Visibility = Visibility.Visible;
                txtPorta.Visibility = Visibility.Visible;
                cbxAgenda.IsEnabled = false;
            }

        }
        private void ricezioneSocket()
        {
            while (true)
            {
                int nBytes = 0;

                if ((nBytes = s.Available) > 0) //controllo se ho ricevuto qualcosa
                {
                    byte[] buffer = new byte[nBytes];  // metto i byte ricevuti nel buffer
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    s.ReceiveFrom(buffer, ref remoteEndPoint); //buffer dove salvare, chi è l'endpoint

                    string from = ((IPEndPoint)remoteEndPoint).Address.ToString(); // recupero l'indirizzo
                    string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes); //recupero il messaggio
                    if (!modPartita)
                    {
                        this.Dispatcher.Invoke(() => lstMessaggi.Items.Add($"{from}: {messaggio}"));
                        //lstMessaggi.Items.Add($"{from}: {messaggio}"); //aggiungo il messaggio alla lista
                    }
                    else
                    {
                        MossaAvversario = messaggio;
                    }

                }
            }

        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            string[] data = { agenda.Contatti[cbxAgenda.SelectedIndex].Indirizzo, agenda.Contatti[cbxAgenda.SelectedIndex].Porta };
            IPAddress remote_address = IPAddress.Parse(data[0]); ;
            int remote_port = int.Parse(data[1]); ;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, remote_port);
            byte[] messaggio = Encoding.UTF8.GetBytes(txtInvia.Text);  //codifichiamo in byte il nostro messaggio
            s.SendTo(messaggio, remote_endpoint);
            lstMessaggi.Items.Add($"tu: {txtInvia.Text}"); //aggiungo il messaggio alla lista
            txtInvia.Text = String.Empty;
        }

        private void btnChatta_Click(object sender, RoutedEventArgs e)
        {

            lblPunteggio.Content = $"";
            lstMessaggi.Visibility = Visibility.Visible;
            btnInvia.Visibility = Visibility.Visible;
            txtInvia.Visibility = Visibility.Visible;
            imgSasso.Visibility = Visibility.Collapsed;
            imgCarta.Visibility = Visibility.Collapsed;
            imgForbici.Visibility = Visibility.Collapsed;

        }

        private void btnGioca_Click(object sender, RoutedEventArgs e)
        {

            lblPunteggio.Content = $"";
            lstMessaggi.Visibility = Visibility.Collapsed;
            btnInvia.Visibility = Visibility.Collapsed;
            txtInvia.Visibility = Visibility.Collapsed;
            imgSasso.Visibility = Visibility.Visible;
            imgCarta.Visibility = Visibility.Visible;
            imgForbici.Visibility = Visibility.Visible;
            lblPunteggio.Visibility = Visibility.Visible;
            btnChatta.IsEnabled = false;
            btnGioca.IsEnabled = false;
            btnAggiungiContatto.IsEnabled = false;
            cbxAgenda.IsEnabled = false;
            modPartita = true;
            Mossa = String.Empty;
            MossaAvversario = String.Empty;
            Punti = 0;
            partita = new Thread(GestisciPartita);
            partita.Start();
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mossa = ((Image)sender).Name.Substring(3);
            imgCarta.IsEnabled = false;
            imgSasso.IsEnabled = false;
            imgForbici.IsEnabled = false;
            lblAvversario.Content = "";
            lblLocal.Content = "";

            string[] data = { agenda.Contatti[cbxAgenda.SelectedIndex].Indirizzo, agenda.Contatti[cbxAgenda.SelectedIndex].Porta };
            IPAddress remote_address = IPAddress.Parse(data[0]); ;
            int remote_port = int.Parse(data[1]); ;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, remote_port);
            byte[] messaggio = Encoding.UTF8.GetBytes(Mossa);  //codifichiamo in byte il nostro messaggio
            s.SendTo(messaggio, remote_endpoint);
        }





    }
}
