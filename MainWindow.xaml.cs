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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MenuCB.IsEnabled = true;
            MenuCB.Items.Add(new ComboBoxItem() { Content = "Feedback" });
            
          
            //if (MenuCB.SelectedIndex < 0)
            //{
            //    MenuCB.Text = "Menu";
            //}
            //else
            //{
            //    MenuCB.Text = MenuCB.SelectedItem as string;
            //}
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuCB.SelectedItem==FeedbackGB)
            {
                FeedbackGB.Visibility=Visibility.Hidden;
            }

        }

        private void MenuCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
