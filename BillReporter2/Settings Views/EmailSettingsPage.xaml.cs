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

namespace BillReporter2.Settings_Views
{
    /// <summary>
    /// Interaction logic for EmailSettingsPage.xaml
    /// </summary>
    public partial class EmailSettingsPage : Page
    {
        DataClasses1DataContext listDb = new DataClasses1DataContext();
        public EmailSettingsPage()
        {
            InitializeComponent();
            settingsLoad();
        }

        public void settingsLoad()
        {

            SettingDB settings = listDb.SettingDBs.FirstOrDefault();
            emailLbl.Text = settings.Email;
            pass.Password = settings.Password;
            smtp.Text = settings.SMTP;

        }

         private void updateSettings_Click(object sender, RoutedEventArgs e)
        {   
            var oldSettings = (
                from set in listDb.SettingDBs
                where set.Id == 1
                select set).FirstOrDefault();



            oldSettings.Email = emailLbl.Text;
            oldSettings.SMTP = smtp.Text;
            oldSettings.Password = pass.Password;


            // Submit the changes to the database.
            try
            {
                listDb.SubmitChanges();
                settingsLoad();
                MessageBox.Show("Update Made Successfully !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                settingsLoad();
            }

        
    }
    }
}
