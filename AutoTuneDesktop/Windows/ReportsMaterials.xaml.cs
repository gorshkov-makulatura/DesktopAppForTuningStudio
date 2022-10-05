using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace AutoTuneDesktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReportsMaterials.xaml
    /// </summary>
    public partial class ReportsMaterials : Window
    {
        public ObservableCollection<KeyValuePair> list { get;set; }
        public AutoTuneEntities db = new AutoTuneEntities();
        public class KeyValuePair
        {

            public string Key { get; set; }
            public int Value { get; set; }
            
            public KeyValuePair(string key,int value)
            {
                Key = key;
                Value = value;
            }
        }
        public ReportsMaterials()
        {
            InitializeComponent();
            DataContext = this;
            LoadPieChartData();
        }

        private void LoadPieChartData()
        {
            list = new ObservableCollection<KeyValuePair>();
        }

        private void applyClick(object sender, RoutedEventArgs e)
        {
            list.Clear();
            foreach(var m in db.Materials.OrderBy(x=>x.Name))
            {
                int count = 0;
                foreach (var sto in db.ServicesToOrders.Where(x=>x.IDMaterial == m.ID))
                {
                    if (sto.Orders.EndTime > fromPicker.SelectedDate && sto.Orders.EndTime < toPicker.SelectedDate)
                    {
                        count += (int)sto.Quantity;
                    }
                }
                if(count > 0)
                    list.Add(new KeyValuePair(m.Name, count));
            }
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void createDocClick(object sender, RoutedEventArgs e)
        {
            if(list.Count != 0)
            {
                var template = Properties.Resources.templateOtchet;
                MemoryStream memoryStream = new MemoryStream(template);
                using (DocX document = DocX.Load(memoryStream))
                {
                    var table = document.Tables[0];
                    var row = table.InsertRow();
                    var c = new BarChart();
                    var s1 = new Series("Материалы");
                    s1.Bind(list, "Key", "Value");
                    c.AddSeries(s1);

                    foreach (var v in list)
                    {
                        row.Cells[0].Paragraphs[0].Append(v.Key);
                        row.Cells[1].Paragraphs[0].Append(v.Value.ToString());
                        row = table.InsertRow();
                    }
                    document.ReplaceText("От:", "От:" + fromPicker.SelectedDate.Value.Date.ToShortDateString());
                    document.ReplaceText("До:", "До:" + toPicker.SelectedDate.Value.Date.ToShortDateString());

                    document.InsertChart(c);
                    try
                    {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        DialogResult result = fbd.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            document.SaveAs(fbd.SelectedPath + "/" + "Materials " + DateTime.Now.ToShortDateString());
                            Messages.ShowInfo("Успешно");
                        }
                    }
                    catch (Exception ex)
                    {
                        Messages.ShowError(ex.ToString());
                    }

                }
            }
        }
    }
}
