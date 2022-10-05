using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Xceed.Words.NET;

namespace AutoTuneDesktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Users bindingActiveUser { get; set; }
        AutoTuneEntities db = new AutoTuneEntities();
        public Admin(Users activeUser)
        {
            InitializeComponent();
            DataContext = this;
            bindingActiveUser = activeUser;

            Visibility(bindingActiveUser.RoleID);
            rolesBoxUpdate();
            SuppliesBoxesUpdate();
            statusBoxUpdate();
            loadStatusOrdersBox();
            materialTypeBoxUpdate();
            UpdateUsers();
            UpdateSuppliers();
            UpdateServices();
            UpdateMaterials();
            UpdateSupplies();
            UpdateOrders();
            UpdateMaterialTypes();
            loadMechanicStatusesBox();
        }
        public void UpdateUsers()
        {
            var usersList = db.Users.ToList();
            if (searchBox.Text != "")
            {
                //string local = searchBox.Text.ToLower();
                usersList = usersList.Where(x => x.FirstName.Contains(searchBox.Text)
                || x.SecondName.Contains(searchBox.Text)
                || x.PhoneNumber.Contains(searchBox.Text)
                || x.FirstName.Contains(searchBox.Text)
                || x.Email.Contains(searchBox.Text)
                ).ToList();
            }
            if (rolesBox.SelectedItem != null)
            {
                if (rolesBox.SelectedItem.ToString() != "Все")
                {
                    usersList = usersList.Where(x => x.Roles.Name == rolesBox.SelectedItem.ToString()).ToList();
                }
            }
            if (statusBox.SelectedItem != null)
            {
                if (statusBox.SelectedItem.ToString() != "Все")
                {
                    if (statusBox.SelectedItem.ToString() == "Активен")
                    {
                        usersList = usersList.Where(x => x.IsActive == true).ToList();
                    }
                    else
                    {
                        usersList = usersList.Where(x => x.IsActive == false).ToList();
                    }
                }
            }

            usersGrid.ItemsSource = usersList;
        }
        public void UpdateSuppliers()
        {
            var suppliersList = db.Suppliers.ToList();
            if (searchBoxSuppliers.Text != "")
            {
                suppliersList = suppliersList.Where(x => x.Name.Contains(searchBoxSuppliers.Text)
                || x.Address.Contains(searchBoxSuppliers.Text)
                || x.Email.Contains(searchBoxSuppliers.Text)
                || x.Phone.Contains(searchBoxSuppliers.Text)
                || x.WebSite.Contains(searchBoxSuppliers.Text)
                ).ToList();
            }
            suppliersGrid.ItemsSource = suppliersList;
        }
        private void UpdateServices()
        {
            var servicesList = db.TypeOfServices.ToList();
            if (searchBoxServices.Text != "")
            {
                servicesList = servicesList.Where(x => x.Name.Contains(searchBoxServices.Text)).ToList();
            }
            servicesGrid.ItemsSource = servicesList;
            servicesGrid.SelectedIndex = 1;
        }
        private void UpdateOrders()
        {
            var status = statusOrdersBox.SelectedItem.ToString();
            var ordersList = db.Orders.ToList();
            if (searchBoxOrders.Text != "")
            {
                ordersList = ordersList.Where(x => x.Users.SecondName?.Contains(searchBoxOrders.Text) == true ||
                x.Users1?.SecondName?.Contains(searchBoxOrders.Text) == true ||
                x.Users2?.SecondName?.Contains(searchBoxOrders.Text) == true ||
                x.Cars?.Number?.Contains(searchBoxOrders.Text) == true
                ).ToList();
            }
            if(status != "Все")
            {
                ordersList = ordersList.Where(x => x.OrderStatuses.Name == status).ToList();
            }
            ordersGrid.ItemsSource = ordersList;
            ordersGrid.SelectedIndex = 0;

        }
        private void UpdateMaterials()
        {
            var materialsList = db.Materials.ToList();
            if (materialsTypeBox.SelectedItem != null)
            {
                if (materialsTypeBox.SelectedItem.ToString() != "Все")
                {
                    materialsList = materialsList.Where(x => x.MaterialsType.Name == materialsTypeBox.SelectedItem.ToString()).ToList();
                }
            }
            if (searchBoxMaterials.Text != "")
                materialsList = materialsList.Where(x => x.Name.Contains(searchBoxMaterials.Text)).ToList();
            materialsGrid.ItemsSource = materialsList;
            materialsGrid.SelectedIndex = 0;
        }
        private void UpdateSupplies()
        {
            var suppliesList = db.Supplies.ToList();
            if (suppliersBox.SelectedItem != null)
            {
                if (suppliersBox.SelectedItem.ToString() != "Все")
                {
                    suppliesList = suppliesList.Where(x => x.Suppliers.Name == suppliersBox.SelectedItem.ToString()).ToList();
                }
            }
            if (statusSuppliesBox.SelectedItem != null)
            {
                if (statusSuppliesBox.SelectedItem.ToString() != "Все")
                {
                    suppliesList = suppliesList.Where(x => x.SupplyStatuses.Name == statusSuppliesBox.SelectedItem.ToString()).ToList();
                }
            }
            suppliesGrid.ItemsSource = suppliesList;
        }
        public void rolesBoxUpdate()
        {
            rolesBox.Items.Add("Все");
            foreach (Roles r in db.Roles)
            {
                rolesBox.Items.Add(r.Name);
            }
            rolesBox.SelectedIndex = 0;
        }
        public void statusBoxUpdate()
        {
            statusBox.Items.Add("Все");
            statusBox.Items.Add("Активен");
            statusBox.Items.Add("Неактивен");
            statusBox.SelectedIndex = 0;
        }
        public void materialTypeBoxUpdate()
        {
            materialsTypeBox.Items.Add("Все");
            foreach (MaterialsType m in db.MaterialsType)
            {
                materialsTypeBox.Items.Add(m.Name);
            }
            materialsTypeBox.SelectedIndex = 0;
        }
        public void SuppliesBoxesUpdate()
        {
            suppliersBox.Items.Add("Все");
            foreach (Suppliers s in db.Suppliers)
            {
                suppliersBox.Items.Add(s.Name);
            }

            statusSuppliesBox.Items.Add("Все");
            foreach (SupplyStatuses s in db.SupplyStatuses)
            {
                statusSuppliesBox.Items.Add(s.Name);
            }
            suppliersBox.SelectedIndex = 0;
            statusSuppliesBox.SelectedIndex = 0;
        }

        private void rolesBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUsers();
        }

        private void statusBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUsers();
        }

        private void editMenuClick(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem is Users u)
            {
                if(bindingActiveUser.Roles.Name == "Менеджер")
                {
                    if (u.Roles.Name == "Менеджер" || u.Roles.Name == "Администратор")
                    {
                        Messages.ShowInfo("Вы не можете изменить данные о сотруднике!");
                        return;
                    }
                }
                AddEditUser ade = new AddEditUser(u.ID,bindingActiveUser.ID);
                ade.ShowDialog();
                db = new AutoTuneEntities();
                UpdateUsers();
            }
        }

        private void addUserClick(object sender, RoutedEventArgs e)
        {
            Users newUser = new Users();
            AddEditUser ade = new AddEditUser(newUser.ID, bindingActiveUser.RoleID);
            ade.ShowDialog();
            db = new AutoTuneEntities();
            UpdateUsers();
        }

        private void searchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUsers();
        }

        private void searchBoxSuppliersTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSuppliers();
        }
        private void searchBoxServicesTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }
        private void searchBoxMaterialsTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMaterials();
        }
        private void materialsTypeBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMaterials();
        }
        private void suppliersBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSupplies();
        }
        private void statusSuppliesBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSupplies();
        }
        public void loadStatusOrdersBox()
        {
            statusOrdersBox.Items.Add("Все");
            foreach(var s in db.OrderStatuses)
            {
                statusOrdersBox.Items.Add(s.Name);
            }
            statusOrdersBox.SelectedIndex = 0;
        }

        private void editSupplierMenuClick(object sender, RoutedEventArgs e)
        {
            if (suppliersGrid.SelectedItem is Suppliers s)
            {
                AddEditSupplier ads = new AddEditSupplier(s.ID);
                ads.ShowDialog();
                db = new AutoTuneEntities();
                UpdateSuppliers();
            }
        }
        private void editServicesMenuClick(object sender, RoutedEventArgs e)
        {
            if (servicesGrid.SelectedItem is TypeOfServices s)
            {
                AddEditService ads = new AddEditService(s.ID);
                ads.ShowDialog();
                db = new AutoTuneEntities();
                UpdateServices();
            }
        }

        private void addSupplierClick(object sender, RoutedEventArgs e)
        {
            AddEditSupplier ads = new AddEditSupplier(0);
            ads.ShowDialog();
            db = new AutoTuneEntities();
            UpdateSuppliers();
        }

        private void addServiceClick(object sender, RoutedEventArgs e)
        {
            AddEditService ads = new AddEditService(0);
            ads.ShowDialog();
            db = new AutoTuneEntities();
            UpdateServices();
        }

        private void carsMenuClick(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem is Users u)
            {
                MenuCars adc = new MenuCars(u.ID);
                adc.ShowDialog();
                db = new AutoTuneEntities();
                UpdateUsers();
            }
        }

        private void editMaterialClick(object sender, RoutedEventArgs e)
        {
            if (materialsGrid.SelectedItem is Materials mt)
            {
                AddEditMaterial adm = new AddEditMaterial(mt.ID);
                adm.ShowDialog();
                db = new AutoTuneEntities();
                UpdateMaterials();
            }
        }


        private void addSupplyClick(object sender, RoutedEventArgs e)
        {
            AddEditSupply aes = new AddEditSupply(0);
            aes.ShowDialog();
            db = new AutoTuneEntities();
            UpdateSupplies();
        }

        private void editSupplyClick(object sender, RoutedEventArgs e)
        {
            if (suppliesGrid.SelectedItem is Supplies mt)
            {
                if (bindingActiveUser.Roles.Name == "Администратор")
                {
                    AddEditSupply ads = new AddEditSupply(mt.ID);
                    ads.ShowDialog();
                    db = new AutoTuneEntities();
                    UpdateSupplies();
                    UpdateMaterials();
                }
                else
                {
                    if (mt.SupplyStatuses.Name == "Доставлен")
                        Messages.ShowError("Вы не можете изменить поставки, которые уже заверешены");
                    else
                    {
                        AddEditSupply ads = new AddEditSupply(mt.ID);
                        ads.ShowDialog();
                        db = new AutoTuneEntities();
                        UpdateSupplies();
                        UpdateMaterials();
                    }
                }     
            }

        }
        private void acceptSupplyClick(object sender, RoutedEventArgs e)
        {
            if (suppliesGrid.SelectedItem is Supplies supply)
            {
                if (supply.SupplyStatuses.Name == "В обработке")
                {
                    if(Messages.ShowQuestion("Вы уверены что хотите изменить статус поставки, после изменения статуса на 'Доставлен', внести изменения будет невозможно"))
                    {
                        foreach (var mts in db.MaterialsToSupplies.Where(x => x.IDSupply == supply.ID))
                        {
                            Materials material = db.Materials.Where(x => x.ID == mts.IDMaterial).SingleOrDefault();
                            material.Quantity += mts.Quantity;
                            MaterialsType type = db.MaterialsType.Where(x => x.ID == material.IDMaterialType).SingleOrDefault();
                            material.RetailPrice = mts.TradePrice + (mts.TradePrice * type.ProcentRetailPrice / 100);
                            material.StockStatus = true;
                        }
                        supply.IdStatusSupply = 1;
                        try
                        {
                            db.SaveChanges();
                            Messages.ShowInfo("Успешно!");
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowError(ex.ToString());
                        }
                    }    
                }
                else
                {
                    supply.IdStatusSupply = 3;
                    if(Messages.ShowQuestion("Хотите ли вы вернуть кол-во материалла до принятия поставки?") == true)
                    {
                        foreach (var mts in db.MaterialsToSupplies.Where(x => x.IDSupply == supply.ID))
                        {
                            Materials material = db.Materials.Where(x => x.ID == mts.IDMaterial).SingleOrDefault();
                            material.Quantity -= mts.Quantity;
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно!");
                    }
                    catch (Exception ex)
                    {
                        Messages.ShowError(ex.ToString());
                    }
                }
                db = new AutoTuneEntities();
                UpdateSupplies();
                UpdateMaterials();

            }
        }

        private void editOrderClick(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem is Orders mt)
            {
                if(bindingActiveUser.Roles.Name == "Администратор")
                {
                    AddEditOrder aeo = new AddEditOrder(mt.ID, bindingActiveUser.ID);
                    aeo.ShowDialog();
                    db = new AutoTuneEntities();
                    UpdateOrders();
                    return;
                }

                if(mt.Payments.PaymentStatuses.Name == "Не оплачен")
                {
                    AddEditOrder aeo = new AddEditOrder(mt.ID, bindingActiveUser.ID);
                    aeo.ShowDialog();
                    db = new AutoTuneEntities();
                    UpdateOrders();
                }
                else
                {
                    Messages.ShowError("Нельзя изменить заказ, который уже оплачен");
                }

            }
        }

        private void addOrderClick(object sender, RoutedEventArgs e)
        {
            AddEditOrder aeo = new AddEditOrder(0,bindingActiveUser.ID);
            aeo.ShowDialog();
            db = new AutoTuneEntities();
            UpdateOrders();
        }
        public void UpdateMaterialTypes()
        {
            typesGrid.ItemsSource = db.MaterialsType.ToList();
            typesGrid.SelectedIndex = 0;
        }

        private void editTypeClick(object sender, RoutedEventArgs e)
        {
            if (typesGrid.SelectedItem is MaterialsType mt)
            {
                AddEditTypeMaterial aetm = new AddEditTypeMaterial(mt.ID);
                aetm.ShowDialog();
                db = new AutoTuneEntities();
                UpdateMaterialTypes();
            }
        }

        private void addTypeClick(object sender, RoutedEventArgs e)
        {
            AddEditTypeMaterial aetm = new AddEditTypeMaterial(0);
            aetm.ShowDialog();
            db = new AutoTuneEntities();
            UpdateMaterialTypes();
        }

        private void payClick(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem is Orders mt)
            {
                if(mt.Payments.PaymentStatuses.Name == "Оплачен")
                {
                    if(Messages.ShowQuestion("Вы уверены что хотите изменить статус оплаты на 'Не оплачен'?") == true)
                        mt.Payments.PaymentStatuses = db.PaymentStatuses.Where(x => x.Name == "Не оплачен").FirstOrDefault();
                }
                else
                {
                    if (mt.OrderStatuses.Name == "Выполнен")
                    {
                        if (Messages.ShowQuestion("Вы уверены что хотите изменить статус оплаты на 'Оплачен'?") == true)
                            mt.Payments.PaymentStatuses = db.PaymentStatuses.Where(x => x.Name == "Оплачен").FirstOrDefault();
                    }
                    else
                        Messages.ShowError("Нельзя изменить статус оплаты на 'Оплачено', если заказ еще не выполнен");
                }
            }
            db.SaveChanges();
            db = new AutoTuneEntities();
            UpdateOrders();
        }

        private void mechanicBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateOrders();
        }

        private void searchBoxOrdersTextChanged(object sender, TextChangedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateOrders();
        }

        private void refreshUsersClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateUsers();
        }

        private void refreshSuppliersClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateSuppliers();
        }

        private void refreshServicesClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateServices();
        }

        private void refreshMaterialsClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateMaterials();
        }

        private void refreshSuppliesClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateSupplies();
        }

        private void refreshOrdersClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateOrders();
        }

        private void refreshTypesClick(object sender, RoutedEventArgs e)
        {
            db = new AutoTuneEntities();
            UpdateMaterialTypes();
        }

        private void addMaterialClick(object sender, RoutedEventArgs e)
        {
            AddEditMaterial aem = new AddEditMaterial(0);
            aem.ShowDialog();
            db = new AutoTuneEntities();
            UpdateMaterials();
        }

        private void mechanicStatusesBoxSelected(object sender, RoutedEventArgs e)
        {
            loadMechanicOrders();
        }
        private void loadMechanicStatusesBox()
        {
            mechanicStatusesBox.Items.Add("Все");
            foreach(var v in db.OrderStatuses)
            {
                mechanicStatusesBox.Items.Add(v.Name);
            }
            mechanicStatusesBox.SelectedIndex = 0;
        }
        private void loadMechanicOrders()
        {
            var list = db.Orders.Where(x => x.Users2.ID == bindingActiveUser.ID).ToList();
            if(mechanicStatusesBox.SelectedItem.ToString() != "Все")
            {
                list = list.Where(x => x.OrderStatuses.Name == mechanicStatusesBox.SelectedItem.ToString()).ToList();
            }
            mechanicOrders.ItemsSource = list;
            mechanicOrders.SelectedItem = 0;
        }
        private void Visibility(int sentID)
        {
            if(sentID == 2)
            {
                tab.SelectedIndex = 2;
                tabUsers.Visibility = System.Windows.Visibility.Collapsed;

                tabSuppliers.Visibility = System.Windows.Visibility.Collapsed;

                panelServices.Visibility = System.Windows.Visibility.Collapsed;
                contextServices.Visibility = System.Windows.Visibility.Collapsed;

                panelMaterials.Visibility = System.Windows.Visibility.Collapsed;
                contextMaterials.Visibility = System.Windows.Visibility.Collapsed;

                tabSupplies.Visibility = System.Windows.Visibility.Collapsed;

                tabOrders.Visibility = System.Windows.Visibility.Collapsed;

                tabTypes.Visibility = System.Windows.Visibility.Collapsed;

                reportMaterials.Visibility = System.Windows.Visibility.Collapsed;

            }
            if(sentID == 1)
            {
                tabYourOrders.Visibility = System.Windows.Visibility.Collapsed;
            }
            if(sentID == 3)
            {
                tabYourOrders.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void changeStatusOrderClick(object sender, RoutedEventArgs e)
        {
           if(mechanicOrders.SelectedItem is Orders or)
           {
                if (or.OrderStatuses.Name == "Выполнен")
                {
                    Messages.ShowError("Нельзя изменить статус заказа, если он уже выполнен");
                }
                if (or.OrderStatuses.Name == "В работе")
                {
                    if (Messages.ShowQuestion("Перед тем как Изменить статус заказа на 'Выполнен', вы убедились в том, что вы верно заполнели услуги?  ") == true)
                    {
                        or.OrderStatuses = db.OrderStatuses.Where(x => x.Name == "Выполнен").FirstOrDefault();
                        or.EndTime = DateTime.Now;
                        foreach(var v in db.ServicesToOrders.Where(x=>x.IDOrder == or.ID).ToList())
                        {

                            if(v.IDMaterial != null)
                            {
                                Materials materials = db.Materials.Where(x => x.ID == v.IDMaterial).FirstOrDefault();
                                materials.Quantity -= v.Quantity;
                            }

                        }
                    }
                }
                if (or.OrderStatuses.Name == "Ожидание")
                {

                    if (Messages.ShowQuestion("Изменить статус заказа на 'В работе' ? ") == true)
                    {
                        or.StartTime = DateTime.Now;
                        or.OrderStatuses = db.OrderStatuses.Where(x => x.Name == "В работе").FirstOrDefault();
                    }

                }
                db.SaveChanges();
                db = new AutoTuneEntities();
                loadMechanicOrders();
           }
        }

        private void logoutClick(object sender, RoutedEventArgs e)
        {
            Authorization a = new Authorization();
            a.Show();
            this.Close();
        }


        private void changeDetalisClick(object sender, RoutedEventArgs e)
        {
            if (mechanicOrders.SelectedItem is Orders or)
            {
                if(or.OrderStatuses.Name != "Выполнен")
                {
                    AddEditOrder aeo = new AddEditOrder(or.ID, bindingActiveUser.ID);
                    aeo.ShowDialog();
                    db = new AutoTuneEntities();
                    loadMechanicOrders();
                }
                else
                {
                    Messages.ShowError("Нельзя изменить детали к закакзу, если он уже выполнен!");
                }

            }
        }

        private void reportMaterialsClick(object sender, RoutedEventArgs e)
        {
            ReportsMaterials rm = new ReportsMaterials();
            rm.ShowDialog();
        }

        private void checkClick(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem is Orders mt)
            {
                if (mt.Payments.PaymentStatuses.Name == "Оплачен")
                {
                    var template = Properties.Resources.template;
                    MemoryStream memoryStream = new MemoryStream(template);
                    using (DocX document = DocX.Load(memoryStream))
                    {
                        var table = document.Tables[0];
                        var row = table.InsertRow();
                        foreach (ServicesToOrders sto in db.ServicesToOrders.Where(x => x.IDOrder == mt.ID))
                        {
                            row.Cells[0].Paragraphs[0].Append(sto.TypeOfServices.Name);
                            row.Cells[1].Paragraphs[0].Append(sto.TypeOfServices.Cost.ToString());
                            row.Cells[2].Paragraphs[0].Append("1");

                            if (sto.Materials != null)
                            {
                                row = table.InsertRow();
                                row.Cells[0].Paragraphs[0].Append(sto.Materials.Name);
                                row.Cells[1].Paragraphs[0].Append(sto.Materials.RetailPrice.ToString());
                                row.Cells[2].Paragraphs[0].Append(sto.Quantity.ToString());
                            }
                            row = table.InsertRow();

                        }
                        row = table.InsertRow();
                        row.Cells[0].Paragraphs[0].Append("Итого");
                        row.Cells[1].Paragraphs[0].Append(mt.TotalCost.ToString());
                        document.ReplaceText("Товарный чек 1", "Товарный чек " + mt.ID.ToString());
                        document.ReplaceText("14.05.2022", DateTime.Now.ToShortDateString());
                        document.ReplaceText("Работы выполнил:", "Работу выполнил: " + mt.Users2.SecondName + " " + mt.Users2.SecondName[0] + "." + mt.Users2.LastName[0] + ".");
                        document.ReplaceText("Менеджер:", "Менеджер: " + mt.Users1.SecondName + " " + mt.Users1.SecondName[0] + "." + mt.Users1.LastName[0] + ".");
                        try
                        {
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            DialogResult result = fbd.ShowDialog();
                            if(result == System.Windows.Forms.DialogResult.OK)
                            {
                                document.SaveAs(fbd.SelectedPath + "/" + "check" + mt.ID);
                                Messages.ShowInfo("Успешно");
                            }
                        }
                        catch(Exception)
                        {

                        }

                    }

                }
            }
        }

        private void statusOrdersBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOrders();
        }

        private void reportSalesClick(object sender, RoutedEventArgs e)
        {
            if (materialsGrid.SelectedItem is Materials mt)
            {
                ReportsSales rs = new ReportsSales(mt.ID);
                rs.ShowDialog();
            }
        }
    }
}
