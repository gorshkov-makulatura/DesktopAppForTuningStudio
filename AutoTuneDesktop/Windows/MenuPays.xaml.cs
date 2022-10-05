using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
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
using System.Windows.Shapes;

namespace AutoTuneDesktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для MenuPays.xaml
    /// </summary>
    public partial class MenuPays : Window
    {
        public AutoTuneEntities db = new AutoTuneEntities();
        public Payments pay { get; set; }
        public MenuPays(int sentID)
        {
            InitializeComponent();
            DataContext = this;
            pay = db.Payments.Where(x => x.ID == sentID).SingleOrDefault();
        }

        private void savePay(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                try
                {
                    db.SaveChanges();
                    Messages.ShowInfo("Успешно!");
                    this.Close();
                }
                catch(Exception ex)
                {
                    Messages.ShowError(ex.ToString());
                }
            }
        }
        private bool Validation()
        {
            if (methodsBox.SelectedIndex == -1 || statusesBox.SelectedIndex == -1)
                return false;
            else
                return true;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
