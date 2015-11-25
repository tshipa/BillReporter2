using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using Syncfusion.Windows.Shared;

namespace BillReporter2
{
    /// <summary>
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public Contacts()
        {
            InitializeComponent();
            string theme = db.Themes.FirstOrDefault().theme1;
            SkinStorage.SetVisualStyle(Application.Current.MainWindow, theme);
            
            dataGrid1.ItemsSource = (from a in db.Contacts select a).ToList();
        }

        private void oneBtn_Click(object sender, RoutedEventArgs e)
        {
            
            Contact newContact = new Contact();
            newContact.Email = emailTxt.Text;
            newContact.Authorization = authTxt.Text;
            newContact.Name = nameTxt.Text;
            db.Contacts.InsertOnSubmit(newContact);
            db.SubmitChanges();
            dataGrid1.Items.Clear();
            dataGrid1.ItemsSource = (from a in db.Contacts select a).ToList();
        }

      

        private void namyBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCSV();
        }

        public void AddCSV()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "CSV Files|*csv*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                List<Contact> contacts = new List<Contact>();

                int i = 0;
                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        String line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] lines = line.Split(',');
                            // contactsUploaded.Items.Add(lines[0] +" |"+lines[1]);
                            Contact c = new Contact();
                            c.Id = i++;
                            c.Authorization = lines[2];
                            c.Email = lines[1];
                            c.Name = lines[0];
                            contacts.Add(c);

                        }
                    }
                    // listDb.SubmitChanges();
                    //ListDbDataContext db = new ListDbDataContext();
                    db.Contacts.InsertAllOnSubmit(contacts);

                    db.SubmitChanges();
                    dataGrid1.ItemsSource = (from a in db.Contacts select a).ToList();
                    //saveContactsBtn.Visibility = Visibility.Visible;
                    //browseBtn.Visibility = Visibility.Hidden;

                }
                catch (Exception ea)
                {
                    MessageBox.Show("Error: " + ea.Message);

                }


            }
        }
    }
}
