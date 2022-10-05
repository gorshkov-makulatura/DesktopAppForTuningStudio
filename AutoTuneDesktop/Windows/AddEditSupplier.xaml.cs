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
    /// Логика взаимодействия для AddEditSupplier.xaml
    /// </summary>
    public partial class AddEditSupplier : Window
    {
        public Suppliers bindingSuppliers { get; set; }
        public AutoTuneEntities db = new AutoTuneEntities();
        public AddEditSupplier(int bindingID)
        {
            InitializeComponent();
            DataContext = this;
            if (bindingID == 0)
                bindingSuppliers = new Suppliers();
            else
                bindingSuppliers = db.Suppliers.Where(x => x.ID == bindingID).FirstOrDefault();
        }

        private void saveSupplier(object sender, RoutedEventArgs e)
        {
            if(validation() == true)
            {
                if (bindingSuppliers.ID == 0)
                {
                    try
                    {
                        db.Suppliers.Add(bindingSuppliers);
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно!");
                    }
                    catch (Exception)
                    {
                        Messages.ShowError("Не удалось сохранить пользователя");
                    }
                }
                else
                {
                    if (db.ChangeTracker.HasChanges())
                    {
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно!");
                        this.Close();
                    }
                    else
                    {
                        Messages.ShowInfo("Вы не внесли какие-либо изменения");
                    }
                    
                }
            }
            else
            {
                Messages.ShowError("Не удалось сохранить изменения, у вас остались незаполненные поля");
            }
            
        }
        private bool validation()
        {
            if (nameBox.Text == "" || addressBox.Text == "" || phoneBox.Text.Contains("_") || emailBox.Text == "" || websiteBox.Text == "")
                return false;
            else
                return true;
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
