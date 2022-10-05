using AutoTuneDesktop.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ReportsSales.xaml
    /// </summary>
    public partial class ReportsSales : Window
    {
        public ObservableCollection<KeyValuePair> list { get; set; }
        public AutoTuneEntities db = new AutoTuneEntities();
        public class KeyValuePair
        {

            public string Key { get; set; }
            public int Value { get; set; }

            public KeyValuePair(string key, int value)
            {
                Key = key;
                Value = value;
            }
        }
        public int sent;
        public ReportsSales(int sentID)
        {
            InitializeComponent();
            DataContext = this;
            list = new ObservableCollection<KeyValuePair>();
            sent = sentID;
        }
        private void applyClick(object sender, RoutedEventArgs e)
        {
            list.Clear();
            foreach(var o in db.ServicesToOrders.Where(x=>x.IDMaterial == sent && x.Orders.DateRegister > fromPicker.SelectedDate && x.Orders.DateRegister < toPicker.SelectedDate).OrderBy(x=>x.Orders.DateRegister))
            {
                if(o.Orders.Payments.PaymentStatusID == 1)
                    list.Add(new KeyValuePair(o.Orders.DateRegister.Value.ToShortDateString(), (int)o.Quantity));
            }
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
