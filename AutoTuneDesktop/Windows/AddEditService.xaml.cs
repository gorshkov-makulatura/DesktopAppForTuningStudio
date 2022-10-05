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
    /// Логика взаимодействия для AddEditService.xaml
    /// </summary>
    public partial class AddEditService : Window
    {
        public TypeOfServices bindingService { get; set; }
        public AutoTuneEntities db = new AutoTuneEntities();
        public AddEditService(int serviceID)
        {
            InitializeComponent();
            DataContext = this;
            if (serviceID == 0)
                bindingService = new TypeOfServices();
            else
                bindingService = db.TypeOfServices.Where(x => x.ID == serviceID).SingleOrDefault();
            loadComboBoxes();
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveService(object sender, RoutedEventArgs e)
        {
            if (Validation() == true)
            {
                if(bindingService.ID == 0)
                {
                    try
                    {
                        db.TypeOfServices.Add(bindingService);
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
                    if(db.ChangeTracker.HasChanges())
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
                Messages.ShowError("Не удалось сохранить изменения, проверьте правильно ли вы заполнили поля");
            }
        }
        private void loadComboBoxes()
        {
            servicesMaterialsTypeComboBox.ItemsSource = db.MaterialsType.ToList();
        }
        private bool Validation()
        {
            decimal number;
            bool check = decimal.TryParse(costBox.Text.Replace(".",","), out number);
            if (check)
            {
                if(nameBox.Text == "" || number < 0 )
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void deselectClick(object sender, RoutedEventArgs e)
        {
            servicesMaterialsTypeComboBox.SelectedIndex = -1;
        }
    }
}
