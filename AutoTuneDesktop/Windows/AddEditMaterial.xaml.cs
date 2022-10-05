using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Логика взаимодействия для AddEditMaterial.xaml
    /// </summary>
    public partial class AddEditMaterial : Window
    {
        public Materials bindingMaterial { get; set; }
        AutoTuneEntities db = new AutoTuneEntities();
        public AddEditMaterial(int sentID)
        {
            InitializeComponent();
            DataContext = this;
            loadComboBoxes();
            if (sentID != 0)
                bindingMaterial = db.Materials.Where(x => x.ID == sentID).SingleOrDefault();
            else
                bindingMaterial = new Materials();
        }

        private void changePhotoClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите изображение";
            op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
            if (op.ShowDialog() == true)
            {
                bindingMaterial.Photo = File.ReadAllBytes(op.FileName);
                DataContext = null;
                DataContext = this;
            }
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (Validation() == true)
            {
                if (bindingMaterial.ID == 0)
                {
                    try
                    {
                        bindingMaterial.Quantity = 0;
                        db.Materials.Add(bindingMaterial);
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно");
                        this.Close();
                    }
                    catch (Exception)
                    {
                        Messages.ShowError("Не удалось сохранить материал");
                    }
                }
                else
                {
                    if (db.ChangeTracker.HasChanges())
                    {
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно");
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
        public void loadComboBoxes()
        {
            materialsTypeBox.ItemsSource = db.MaterialsType.ToList();

        }
        public bool Validation()
        {
            int number;
            bool check = int.TryParse(quantityBox.Text, out number);
            decimal number2;
            bool check2 = decimal.TryParse(retailPriceBox.Text.Replace(".",","), out number2);
            if (check && check2)
            {
                if (nameBox.Text == "" || materialsTypeBox.SelectedIndex == -1)
                {
                    return false;
                }
                else
                {
                    if (number >= 0 && number2 >= 0)
                        return true;
                    else
                        return false;
                }
            }
            else
                return false;
        }
    }
}
