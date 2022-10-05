using AutoTuneDesktop.Classes;
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
    /// Логика взаимодействия для AddEditSupply.xaml
    /// </summary>
    public partial class AddEditSupply : Window
    {
        public AutoTuneEntities db = new AutoTuneEntities();
        public Supplies sup { get; set; }
        List<MaterialsToSupplies> listMts = new List<MaterialsToSupplies>();
        public int IDSupply;
        public AddEditSupply(int sentID)
        {
            InitializeComponent();
            DataContext = this;
            IDSupply = sentID;
            loadSuppliers();
            loadMaterials();
            if (sentID == 0)
                sup = new Supplies();
            else
            {
                sup = db.Supplies.Where(x => x.ID == sentID).SingleOrDefault();
                listMts = sup.MaterialsToSupplies.ToList();
            }
            loadMTS();
        }
        public void loadMTS()
        {
            materialsToSuppliesGrid.ItemsSource = null;
            materialsToSuppliesGrid.ItemsSource = listMts;
        }
        public void loadSuppliers()
        {
            suppliersBox.ItemsSource = db.Suppliers.ToList();
        }
        public void loadMaterials()
        {
            materialBox.ItemsSource = db.Materials.ToList();
            materialBox.SelectedIndex = 0;
        }

        private void addNewMaterialClick(object sender, RoutedEventArgs e)
        {
            AddEditMaterial adm = new AddEditMaterial(0);
            adm.ShowDialog();
            loadMaterials();
        }

        private void addMaterialClick(object sender, RoutedEventArgs e)
        {
            if (ValidationForAdd())
            {
                Materials material = (Materials)materialBox.SelectedItem;
                MaterialsToSupplies newM = new MaterialsToSupplies();
                newM.Materials = db.Materials.Where(x => x.ID == material.ID).SingleOrDefault();
                newM.IDMaterial = material.ID;
                newM.IDSupply = IDSupply;
                newM.TradePrice = Convert.ToDecimal(tradePriceBox.Text.Replace(".", ","));
                newM.Quantity = Convert.ToInt32(quantityBox.Text);
                listMts.Add(newM);
                loadMTS();
            }
            else
                Messages.ShowError("Не удалось добавить материал, проверьте все ли поля заполнены");

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveSupply(object sender, RoutedEventArgs e)
        {
            if (Validation())
            {
                decimal cost = 0;
                if (IDSupply == 0)
                {
                        sup.IdStatusSupply = 3;
                        sup.SupplyDate = DateTime.Now;
                        db.Supplies.Add(sup);
                }
                foreach (var mts in listMts)
                {
                    cost += (int)mts.Quantity * (decimal)mts.TradePrice;
                    mts.Supplies = sup;
                }
                sup.Cost = cost;
                //sup.MaterialsToSupplies.Clear();
                foreach(var m in listMts)
                {
                    if(m.ID != 0)
                        db.MaterialsToSupplies.Remove(m);
                }
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

                //db.MaterialsToSupplies.RemoveRange(sup.MaterialsToSupplies);
                foreach(var m in listMts)
                {
                    db.MaterialsToSupplies.Add(m);
                }
                //sup.MaterialsToSupplies = listMts;
                try
                {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    Messages.ShowError(ex.ToString());
                }
            }
            else
                Messages.ShowError("Не удалось сохранить изменения, некоторые поля остались пустыми!");
        }
        private bool Validation()
        {
            if (suppliersBox.SelectedIndex == -1)
                return false;
            else
                return true;
        }
        private bool ValidationForAdd()
        {
            int number;
            bool check = int.TryParse(quantityBox.Text, out number);
            decimal number2;
            bool check2 = decimal.TryParse(tradePriceBox.Text.Replace(".", ","), out number2);
            if (check && check2){
                if (number > 0 && number2 >= 0)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {
            if(materialsToSuppliesGrid.SelectedItem is MaterialsToSupplies mts)
            {
                if(mts.ID != 0)
                {
                    db.MaterialsToSupplies.Remove(mts);
                }
                listMts.Remove(mts);
            }
            loadMTS();
        }
    }
}