using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace BillReporter2.Settings_Views
{
    /// <summary>
    /// Interaction logic for ThemeSettingsPage.xaml
    /// </summary>
    public partial class ThemeSettingsPage : Page
    {
        public ThemeSettingsPage()
        {
            InitializeComponent();
        }

        private void updateTheme_Click(object sender, RoutedEventArgs e)
        {


          applyTheme();

        }
        private int applyTheme()
        {
            int done = 1;
            DataClasses1DataContext db = new DataClasses1DataContext();
            var oldTheme= (
               from set in db.Themes
               where set.id == 1
               select set).FirstOrDefault();



            

            if (blend.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "Blend");

                oldTheme.theme1 = "Blend";

                return done;
            }
            else if (metro.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "Metro");
                oldTheme.theme1 = "Metro";
                db.SubmitChanges();
                return done;
            }
            else if (orange.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "SyncOrange");
                oldTheme.theme1 = "SyncOrange";
                db.SubmitChanges();
                return done;
            }
            else if (transparent.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "Transparent");
                oldTheme.theme1 = "Transparent";
                db.SubmitChanges();
                return done;
            }
            else if (blue.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "Office2010Blue");
                oldTheme.theme1 = "Office2010Blue";
                db.SubmitChanges();
                return done;
            }
            else if (silver.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "Office2010Silver");
                oldTheme.theme1 = "Office2010Silver";
                db.SubmitChanges();
                return done;
            }
            else if (red.IsChecked == true)
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "ShinyRed");
                oldTheme.theme1 = "ShinyRed";
                db.SubmitChanges();
                return done;
            }
            else
            {
                SkinStorage.SetVisualStyle(Application.Current.MainWindow, "VS2010");
                oldTheme.theme1 = "VS2010";
                db.SubmitChanges();
                return done;

            }

            
        }
    }
}
