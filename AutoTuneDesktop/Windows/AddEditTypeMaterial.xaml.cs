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
    /// Логика взаимодействия для AddEditTypeMaterial.xaml
    /// </summary>
    public partial class AddEditTypeMaterial : Window
    {
        AutoTuneEntities db = new AutoTuneEntities();
        public MaterialsType bindingType { get; set; }
        public AddEditTypeMaterial(int SentID)
        {
            InitializeComponent();
            DataContext = this;
            if (SentID == 0)
                bindingType = new MaterialsType();
            else
                bindingType = db.MaterialsType.Where(x => x.ID == SentID).SingleOrDefault();
        }

        private void saveService(object sender, RoutedEventArgs e)
        {
            if(Validation())
            {
                if(bindingType.ID == 0)
                {
                    db.MaterialsType.Add(bindingType);
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
                    Messages.ShowInfo("Успешно!");
                }
                else
                {
                    if(db.ChangeTracker.HasChanges())
                    {
                        try
                        {
                            db.SaveChanges();
                            this.Close();
                        }
                        catch(Exception ex)
                        {
                            Messages.ShowError(ex.ToString());
                        }
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

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool Validation()
        {
            int number;
            bool check = int.TryParse(percentBox.Text, out number);
            if (check && nameBox.Text != "")
            {
                if (number < 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
