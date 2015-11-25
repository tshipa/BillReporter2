using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using Syncfusion.ReportWriter;
using Syncfusion.Windows.Reports;
using Syncfusion.Windows.Shared;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace BillReporter2
{
    
    public partial class SendMail : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public SendMail()
        {
            InitializeComponent();
            string theme = db.Themes.FirstOrDefault().theme1;
            SkinStorage.SetVisualStyle(Application.Current.MainWindow, theme);
            DateTime lastDate = (DateTime)db.Bills.Max(r => r.Date);
            DateTime newDate = lastDate;
            PickedMonthtxt.Text = newDate.ToString("MMM-yy");
        }



        private string toPDF2(string auth1,string reportPath, string month)
        {
            
                //string reportPath = templatetxt.Text; ;
                WriterFormat format;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = @path + "\\Telephone Bill"+" "+auth1+".pdf";

                format = WriterFormat.PDF;
                ReportWriter wrp = new ReportWriter(reportPath);

                wrp.SetParameters(this.GetParameter(auth1, month));
                wrp.Save(fileName, format);

                return fileName;
            
           
        }
        private IEnumerable<Syncfusion.Windows.Reports.ReportParameter> GetParameter(string auth, string month)
        {
            List<ReportParameter> parameters = new List<ReportParameter>();
            ReportParameter param = new ReportParameter();
            param.Labels.Add("Authorzation");
            param.Values.Add(auth);
            param.Name = "AuthParameter";
            parameters.Add(param);

            param = new ReportParameter();
            param.Labels.Add("Bill Month");
            param.Values.Add(month);
            param.Name = "ReportingMonth";
            parameters.Add(param);

            return parameters;
        }



       

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool sent(string path, string body, string mailTo, string pickedMonth,string pass,string smtp,string email)
        {
            //ListDbDataContext db = new ListDbDataContext();
            List<Contact> contacts = (from c in db.Contacts select c).ToList();
            List<Bill> AllBills = (from b in db.Bills select b).ToList();
            string toEmail="";
            //string path = templatetxt.Text;
            foreach (Contact row in contacts)
            {
                try{
               
                    Bill bill = AllBills.Where(b => b.Authorization.Replace(" ",String.Empty).Equals(row.Authorization.Replace(" ",String.Empty))).FirstOrDefault();
                    //MessageBox.Show(toPDF2(bill.Authorization, path));
                    if(!mailTo.Equals("Mail List")){toEmail=mailTo;}
                    else{sendTo.Text=row.Email;}
                    sendFromSMTP(bill, path,body,toEmail, pickedMonth,smtp,email,pass);
                   
                    string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "result.txt");
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Email :" + row.Email + "<br/>" + Environment.NewLine + "Result:" + "Sent Successfully" +
                           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                    
                 }
                catch (Exception ex)
                {
                   
                    string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "result.txt");

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Email :" + row.Email + "<br/>" + Environment.NewLine + "Result:" + ex.Message+
                           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                   
                }


               

            };
            
            //this.Close();
            return true;
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

            sendingMail.IsBusy = true;
            string path = templatetxt.Text;
            string body = mailText.Text;
            string mailTo = sendTo.Text;
            string pickedMonth = PickedMonthtxt.Text;
            //ListDbDataContext db=new ListDbDataContext();
            SettingDB set = db.SettingDBs.FirstOrDefault();
            //int uploaded = 0;
            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (s, ea) =>
            {
               
                ea.Result = sent(path,body,mailTo,pickedMonth,set.Password,set.SMTP,set.Email);

            };
            bgWorker.RunWorkerCompleted += (s, ea) =>
            {

                sendingMail.IsBusy = false;
                Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "result.txt"));
               

            };
            bgWorker.RunWorkerAsync();
            
        }
    
    private void sendFromSMTP(Bill bill, string reportPath,string body,string toEmail, string pickedMonth, string smtp, string email, string pass)
{
    
           //Bill bill = (from a in db.GetTable<Bill>() select a).FirstOrDefault();
          System.Net.Mail.MailMessage mail = new MailMessage();
          System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient(smtp);

           mail.From = new MailAddress(email);
           mail.To.Add(toEmail);
           mail.Subject = "Telephone Bill for"+" "+ bill.Authorization;
           mail.Body = body;
           
           System.Net.Mail.Attachment attachment;
           string fileAttach = toPDF2(bill.Authorization,reportPath, pickedMonth);
           attachment = new System.Net.Mail.Attachment(fileAttach);
         
           mail.Attachments.Add(attachment);
           
           SmtpServer.Credentials = new System.Net.NetworkCredential(email, pass);
           

           SmtpServer.Send(mail);
           attachment.Dispose();
           File.Delete(fileAttach);
           //MessageBox.Show("mail Send");
         
           
           

}
   
      

        private void monthPickbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "RDL File|*rdl*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                templatetxt.Text = filename;
            }
            else
            {
                MessageBox.Show("No file Chosen !");
            }
        }

    }

}
