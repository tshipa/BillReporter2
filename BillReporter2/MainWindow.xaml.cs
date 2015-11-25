using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Reports;
using Syncfusion.ReportWriter;
using System.Data.SqlClient;
using System.Data.OleDb;
using BillReporter2.Settings_Views;
using Syncfusion.Windows.Shared;
using BillReporter2.Dashboard_Views;


namespace BillReporter2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext listDb = new DataClasses1DataContext();
      

        private string department = "";
        private string auth = "";

        public MainWindow()
        {
           
           InitializeComponent();        
           populateDataGrid("Bills");
           _mainFrame.Navigate(new EmailSettingsPage());
           monthly.Navigate(new Monthly());
           string theme=listDb.Themes.FirstOrDefault().theme1;
           SkinStorage.SetVisualStyle(Application.Current.MainWindow, theme);
          
                   
        }

        

        private void updateContacts_Click(object sender, RoutedEventArgs e)
        {
            Contacts wp = new Contacts();
            wp.Show();

        }

        
        private void sendBills_Click(object sender, RoutedEventArgs e)
        {
            SendMail send_form = new SendMail();
            send_form.Show();

        }

        private void newMonthUpload_Click(object sender, RoutedEventArgs e)
        {
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Text Files|*txt*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                var infile = File.ReadAllLines(filename);
                string line = (from a in infile select a).First();
                string[] lines = line.Split('e');
                string[] stringParts=lines[1].Substring(1,11).Split('/');
                
                DateTime thisDat = new DateTime(int.Parse(stringParts[0]), int.Parse(stringParts[1])-1, int.Parse(stringParts[2]));
                
                    if ((from a in listDb.Bills where a.Date.Value.Year == thisDat.Year & a.Date.Value.Month == thisDat.Month select a.Date).Count() == 0)
                    {

                        readText.IsBusy = true;

                        int uploaded = 0;
                        BackgroundWorker bgWorker = new BackgroundWorker();
                        bgWorker.DoWork += (s, ea) =>
                        {
                            uploaded = upload(filename);
                            ea.Result = uploaded;

                        };
                        bgWorker.RunWorkerCompleted += (s, ea) =>
                        {

                            readText.IsBusy = false;
                            lsDb.ItemsSource = null;
                            lsDb.ItemsSource = (from a in listDb.Bills select new { a.Date, a.Day, a.Location, a.Number, a.Duration, a.Time, a.Authorization, a.Department }).ToList();
                            MessageBox.Show(uploaded.ToString() + " New Rows Have Been Uploaded");

                        };
                        bgWorker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Month Already Uploaded !");
                    }
                

            }
           

        }
      
       

        private int toDatabase(String line)
        {
            
                int inline = 0;

                if (line.Length > 13)
                {


                    if (line.Substring(0, 14) == "Authorization:")
                    {
                        string[] partsA = line.Trim().Split(':');
                        auth = partsA[1];
                    }
                }
                if (line.Length > 14)
                {
                    if (line.Substring(0, 14) == "Authorization ")
                    {
                        string[] partsA = line.Trim().Split(':');
                        department = partsA[1];

                    }
                }

                string[] parts = line.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                Bill Row = new Bill();
                if (parts.Length == 10 && line.Contains("OUT"))
                {

                    string[] date_parts = parts[0].Split('/');
                    Row.Date = new DateTime(int.Parse(date_parts[0]), int.Parse(date_parts[1]), int.Parse(date_parts[2]));
                    Row.Day = parts[1];
                    Row.Time = parts[2];
                    Row.Type = parts[3];
                    Row.Number = parts[4];
                    Row.Location = parts[5] + " " + parts[6] + " " + parts[7];
                    Row.Duration = parts[8];
                    Row.Charge = parts[9];
                    Row.Authorization = auth;
                    Row.Department = department;
                    listDb.Bills.InsertOnSubmit(Row);
                    inline = 1;
                }

                if (parts.Length == 9 && line.Contains("OUT"))
                {

                    string[] date_parts = parts[0].Split('/');
                    Row.Date = new DateTime(int.Parse(date_parts[0]), int.Parse(date_parts[1]), int.Parse(date_parts[2]));
                    Row.Day = parts[1];
                    Row.Time = parts[2];
                    Row.Type = parts[3];
                    Row.Number = parts[4];
                    Row.Location = parts[5] + " " + parts[6];
                    Row.Duration = parts[7];
                    Row.Charge = parts[8];
                    Row.Department = department;
                    Row.Authorization = auth;
                    listDb.Bills.InsertOnSubmit(Row);
                    inline = 1;

                }
                if (parts.Length == 8 && line.Contains("OUT"))
                {

                    string[] date_parts = parts[0].Split('/');
                    Row.Date = new DateTime(int.Parse(date_parts[0]), int.Parse(date_parts[1]), int.Parse(date_parts[2]));
                    Row.Day = parts[1];
                    Row.Time = parts[2];
                    Row.Type = parts[3];
                    Row.Number = parts[4];
                    Row.Location = parts[5];
                    Row.Duration = parts[6];
                    Row.Charge = parts[7];
                    Row.Authorization = auth;
                    Row.Department = department;
                    listDb.Bills.InsertOnSubmit(Row);
                    inline = 1;

                
                
            }

                return inline;

        }


        private int upload(string filename)
        {
            int i = 0;
            
                using (StreamReader sr = File.OpenText(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        i = i + toDatabase(sr.ReadLine());
                    }
                }
                listDb.SubmitChanges();
            
                                
            return i;
        }

        private void populateDataGrid(string type)
        {
            if (type.Equals("Contacts"))
            {
                lsDb.ItemsSource = (from a in listDb.GetTable<Contact>() select new { a.Name, a.Email }).ToList();
            }
            else
            {
                lsDb.ItemsSource = (from a in listDb.GetTable<Bill>() select new { a.Date, a.Day, a.Location, a.Number, a.Duration, a.Time, a.Authorization, a.Department }).ToList();
            }
        }

        private void viewChange_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedView = (sender as ComboBox).SelectedItem as string;
            populateDataGrid(selectedView);
        }

        

        private void emailSet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _mainFrame.Navigate(new EmailSettingsPage());
        }

        private void themeSet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _mainFrame.Navigate(new ThemeSettingsPage());
        }

       
    }
}
