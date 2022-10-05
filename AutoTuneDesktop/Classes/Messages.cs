using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace AutoTuneDesktop.Classes
{
    public class Messages
    {
        public static void ShowError(string Error)
        {
            MessageBox.Show(Error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowInfo(string Info)
        {
            MessageBox.Show(Info, "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static bool ShowQuestion(string Question)
        {
            MessageBoxResult d = System.Windows.MessageBox.Show(Question, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (d == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

    }
}
