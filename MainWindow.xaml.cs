using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOCKET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Agenda agenda;
        public MainWindow()
        {
            InitializeComponent();
            agenda = new Agenda();
            cbxAgenda.Items.Add("FACCEZ");
            cbxAgenda.Items.Add("PANTERZ");
            cbxAgenda.Items.Add("MIRI");
            cbxAgenda.Items.Add("STRUMPA");
        }

        private void cbxAgenda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(cbxAgenda.SelectedItem as string);
        }

        private void btnAggiungiContatto_Click(object sender, RoutedEventArgs e)
        {
            if(txtIp.Visibility== Visibility.Visible && txtName.Visibility == Visibility.Visible)
            {
                agenda.AggiungiContatto(txtName.Text, txtIp.Text);
                cbxAgenda.Items.Add(agenda.Contatti[agenda.Contatti.Count - 1].Nome);
                txtIp.Text = "Nome contatto";
                txtName.Text = "Ip contatto";
                txtIp.Visibility = Visibility.Collapsed;
                txtName.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtIp.Visibility = Visibility.Visible;
                txtName.Visibility = Visibility.Visible;
            }
           
        }
    }
}
