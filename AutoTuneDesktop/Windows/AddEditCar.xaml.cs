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
    /// Логика взаимодействия для AddEditCar.xaml
    /// </summary>
    public partial class AddEditCar : Window
    {
        AutoTuneEntities db = new AutoTuneEntities();
        public Cars bindingCar { get; set; }
        public int userSentID;
        public AddEditCar(int sentID, int userID)
        {
            InitializeComponent();
            LoadComboBoxes();
            userSentID = sentID;
            DataContext = this;
            if (sentID != 0)
                bindingCar = db.Cars.Where(x => x.ID == sentID).SingleOrDefault();
            else
            {
                bindingCar = new Cars();
                bindingCar.UserID = userID;
            }
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveService(object sender, RoutedEventArgs e)
        {
            if(Validation() == true)
            {
                if(bindingCar.ID == 0)
                {
                    try
                    {
                        db.Cars.Add(bindingCar);
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно");
                        this.Close();
                    }
                    catch(Exception)
                    {
                        Messages.ShowError("Не удалось сохранить изменения");
                    }
                }
                else
                {
                    if(db.ChangeTracker.HasChanges())
                    {
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно");
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
        public void LoadComboBoxes()
        {
            marksComboBox.ItemsSource = db.Marks.ToList();
        }
        public bool Validation()
        {
            if (marksComboBox.SelectedIndex == -1 || numberBox.Text.Contains("_"))
                return false;
            else
                return true;
        }
    }
}
