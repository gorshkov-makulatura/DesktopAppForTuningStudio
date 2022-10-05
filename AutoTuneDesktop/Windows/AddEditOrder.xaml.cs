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
    /// Логика взаимодействия для AddEditOrder.xaml
    /// </summary>
    public partial class AddEditOrder : Window
    {
        public AutoTuneEntities db = new AutoTuneEntities();
        public Orders ord { get; set; }
        List<ServicesToOrders> listSto = new List<ServicesToOrders>();
        public int OrderID;
        public AddEditOrder(int SentID,int activeUserID)
        {
            InitializeComponent();
            DataContext = this;
            OrderID = SentID;
            Users active = db.Users.Where(x => x.ID == activeUserID).SingleOrDefault();
            if(active.RoleID == 2)
            {
                notForAMechanic.Visibility = Visibility.Collapsed;
            }
            loadClients();
            loadManagers();
            loadCars();
            loadMechanics();
            loadMaterials();
            loadServices();
            loadMethodsBox();
            loadStatuses();
            if (SentID == 0)
            {
                ord = new Orders();
                ord.OrderStatuses = db.OrderStatuses.Where(x => x.Name == "Ожидание").FirstOrDefault();
                statusBox.IsEnabled = false;
                quantityBox.Text = "1";
                quantityBox.IsEnabled = false;
            }
            else
            {
                ord = db.Orders.Where(x => x.ID == SentID).SingleOrDefault();
                listSto = ord.ServicesToOrders.ToList();
            }
            ord.Users1 = db.Users.Where(x => x.ID == active.ID).SingleOrDefault();
            
            loadSTO();
        }
        public void loadSTO()
        {
            servicesToOrdersGrid.ItemsSource = null;
            servicesToOrdersGrid.ItemsSource = listSto;
        }
        public void loadClients()
        {
            clientsBox.ItemsSource = db.Users.Where(x => x.RoleID == 4).OrderBy(x => x.SecondName).ToList();
        }
        public void loadManagers()
        {
            managerBox.ItemsSource = db.Users.Where(x => x.RoleID == 3 || x.RoleID == 1).OrderBy(x => x.SecondName).ToList();
        }
        public void loadMechanics()
        {
            mechanicBox.ItemsSource = db.Users.Where(x => x.RoleID == 2).OrderBy(x=>x.SecondName).ToList();
        }
        public void loadStatuses()
        {
            statusBox.ItemsSource = db.OrderStatuses.ToList();
        }
        public void loadCars()
        {
            if(clientsBox.SelectedIndex != -1)
            {
                Users client = (Users)clientsBox.SelectedItem;
                carBox.ItemsSource = db.Cars.Where(x => x.UserID == client.ID).ToList();
            }
        }
        public void loadServices()
        {
            servicesBox.ItemsSource = db.TypeOfServices.OrderBy(x => x.Name).ToList();
        }
        public void loadMaterials()
        {
            if(servicesBox.SelectedIndex != -1)
            {
                TypeOfServices sto = (TypeOfServices)servicesBox.SelectedItem;
                materialsBox.ItemsSource = db.Materials.Where(x => x.StockStatus == true && x.Quantity > 0 && x.IDMaterialType == sto.IdMaterialType).OrderBy(x=>x.Name).ToList();
            }
            
        }
        private void addServicesToOrdersClick(object sender, RoutedEventArgs e)
        {
            bool checkCopies = false;

            ServicesToOrders sto = new ServicesToOrders();
            if (servicesBox.SelectedItem is TypeOfServices mt)
            {
                if (mt.IdMaterialType != null)
                {
                    if (materialsBox.SelectedIndex != -1)
                    {
                        Materials material = (Materials)materialsBox.SelectedItem;

                        foreach(var v in listSto)
                        {
                                if (v.IDMaterial == material.ID && v.IDService == mt.ID)
                                    checkCopies = true;
                        }
                        if (quantityBox.Text != "")
                        {
                            int number;
                            bool check = int.TryParse(quantityBox.Text, out number);
                            if (number > 0)
                            {
                                sto.Quantity = Convert.ToInt32(quantityBox.Text);
                            }
                            else
                            {
                                Messages.ShowError("Кол-во материала не может быть меньше или равно 0");
                                return;
                            }
                        }
                        else
                        {
                            Messages.ShowError("Вы отсавили поле кол-во материала пустым!");
                            return;
                        }
                        if(sto.Quantity <= material.Quantity)
                        {
                            sto.IDMaterial = material.ID;
                            sto.Materials = db.Materials.Where(x => x.ID == material.ID).SingleOrDefault();
                        }
                        else
                        {
                            Messages.ShowError("Вы не можете добавить материла больше, чем есть на складе!");
                            return;
                        }
                    }
                    else
                    {
                        Messages.ShowError("Вы оставили материал незаполненным");
                        return;
                    }
                }
                else
                {
                    foreach (var v in listSto)
                    {
                        if (v.IDService == mt.ID)
                            checkCopies = true;
                    }
                }
                if (checkCopies == true)
                {
                    Messages.ShowError("Вы не можете добавить в таблицу, одинаковый материал И услугу");
                    return;
                }
                sto.IDOrder = OrderID;
                sto.IDService = mt.ID;
                
                sto.TypeOfServices = db.TypeOfServices.Where(x => x.ID == mt.ID).SingleOrDefault();
                listSto.Add(sto);
                loadSTO();
            }   
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void loadMethodsBox()
        {
            methodsBox.ItemsSource = db.PaymentMethods.ToList();
        }

        private void saveOrder(object sender, RoutedEventArgs e)
        {
            materialsBox.ItemsSource = db.Materials.ToList();
            if (Validation())
            {
                decimal cost = 0;
                if (OrderID == 0)
                {
                    Payments newPay = new Payments();
                    newPay.PaymentStatusID = 2;
                    if (methodsBox.SelectedItem is PaymentMethods mt)
                        newPay.PaymentMethodID = mt.ID;
                    db.Payments.Add(newPay);
                    ord.DateRegister = DateTime.Now;
                    ord.PaymentID = newPay.ID;
                    ord.OrderStatuses.Name = "Ожидание";
                    db.Orders.Add(ord);
                }
                List<Nullable<int>> ids = new List<Nullable<int>>();
                foreach (var sto in listSto)
                {
                    Nullable<int> id = sto.IDMaterial;
                    TypeOfServices type = db.TypeOfServices.Where(x => x.ID == sto.IDService).SingleOrDefault();
                    if (sto.IDMaterial != null)
                    {
                        Materials mat = db.Materials.Where(x => x.ID == sto.IDMaterial).SingleOrDefault();
                        if (sto.Quantity != null)
                        {
                            cost += (decimal)mat.RetailPrice * (int)sto.Quantity;
                        }
                        else
                            cost += (decimal)mat.RetailPrice;
                    }
                    cost += type.Cost;
                    sto.Orders = ord;
                    ids.Add(id);
                }
                ord.TotalCost = cost;
                foreach (var sto in listSto)
                {
                    if (sto.ID != 0)
                        db.ServicesToOrders.Remove(sto);
                }
                try
                {
                    db.SaveChanges();
                    Messages.ShowInfo("Успешно!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    Messages.ShowError(ex.ToString());
                }
                int circle = 0;
                foreach (var sto in listSto)
                {
                    sto.IDMaterial = ids[circle];
                    db.ServicesToOrders.Add(sto);
                    circle++;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Messages.ShowError(ex.ToString());
                }
                this.Close();
            }
            else
                Messages.ShowInfo("Не удалось сохранить изменения, вы оставили некоторые поля пустыми");
            
        }

        private void clientsBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCars();
        }

        private void servicesBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadMaterials();
        }
        private bool Validation()
        {
            if (clientsBox.SelectedIndex == -1 || managerBox.SelectedIndex == -1 || mechanicBox.SelectedIndex == -1 || carBox.SelectedIndex == -1 || methodsBox.SelectedIndex == -1 || statusBox.SelectedIndex == -1)
                return false;
            else
                return true;
        }

        private void deleteClick(object sender, RoutedEventArgs e)
        {
            if(servicesToOrdersGrid.SelectedItem is ServicesToOrders sto)
            {
                if(sto.ID != 0)
                {
                    db.ServicesToOrders.Remove(sto);
                }
                listSto.Remove(sto);
            }
            loadSTO();
        }

    }
}
