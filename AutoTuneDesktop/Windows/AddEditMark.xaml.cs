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
    /// Логика взаимодействия для AddEditMark.xaml
    /// </summary>
    public partial class AddEditMark : Window
    {
        public AutoTuneEntities db = new AutoTuneEntities();
        public Marks bindingMark { get; set; }
        public AddEditMark(int sentID)
        {
            InitializeComponent();
            DataContext = this;
            if (sentID != 0)
                bindingMark = db.Marks.Where(x => x.ID == sentID).SingleOrDefault();
            else
                bindingMark = new Marks();
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveMark(object sender, RoutedEventArgs e)
        {
            if (Validation() == true)
            {
                if (bindingMark.ID == 0)
                {
                    try
                    {
                        db.Marks.Add(bindingMark);
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно!");
                        this.Close();
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
                    }
                    else
                    {
                        Messages.ShowInfo("Вы не внесли какие-либо изменения");
                    }
                }
            }
            else
                Messages.ShowError("Не удалось сохранить изменения, вы оставили поле пустым!");
        }
        public bool Validation()
        {
            if (nameBox.Text == "")
                return false;
            return true;
        }
    }
}
