using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillReporter2.Dashboard_Views
{
    public class ViewModel
    {
        
        public ObservableCollection<string> PaletteList
        {
            get;
            set;
        }
        public ObservableCollection<Model> CountryStatus
        {
            get;
            set;
        }
        public ObservableCollection<Model> ActiveUsers
        {
            get;
            set;
        }
        public ObservableCollection<Model> RegisteredUsers
        {
            get;
            set;
        }
        public ObservableCollection<Model> AnnualStatus
        {
            get;
            set;
        }

        public int[] getParts(string monthName)
        {
            string[] parts = new string[2];
            int[] dateParts = new int[2];
            parts = monthName.Split('-');
            dateParts[0] = getMonth(parts[0]);
            dateParts[1] = getYear(parts[1]);
            return dateParts;
        }
        public int getYear(string yearPart)
        {
            return int.Parse("20" + yearPart);
        }
        public int getMonth(string monthPart)
        {
            switch (monthPart)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                default:
                    return 12;

            }
        }



        public double sumCharges(List<string> charges)
        {
            double sums = 0;
            foreach (string charge in charges)
            {
                sums = double.Parse(charge) + sums;
            }
            return sums;
        }

        public ViewModel()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            
            this.PaletteList = new ObservableCollection<string>();

           

            PaletteList.Add((from a in db.GetTable<Bill>() select a).OrderByDescending(x => x.Date).FirstOrDefault().Date.Value.ToString("MMM-yy"));
            int monthCh = getParts((from a in db.GetTable<Bill>() select a).OrderByDescending(x => x.Date).FirstOrDefault().Date.Value.ToString("MMM-yy"))[0];

            this.AnnualStatus = new ObservableCollection<Model>();

            var dataPoints1 = new List<Model>();
            var bills1 = (from a in db.GetTable<Bill>() select a).ToList<Bill>();
            List<String> months1 = new List<string>();

            foreach (var bil in bills1)
            {
                if (!months1.Contains(bil.Department))
                {
                    List<string> charges = (from a in db.GetTable<Bill>() where a.Department == bil.Department && a.Date.Value.Month == monthCh select a.Charge).ToList<string>();
                    AnnualStatus.Add(new Model { SocialNetwork = bil.Department, Status = sumCharges(charges) });
                    months1.Add(bil.Department);
                    //PaletteList.Add(bil.Date.Value.ToString("MMM-yy"));
                }

            }


            //Active Users
            this.ActiveUsers = new ObservableCollection<Model>();
            var dataPoints = new List<Model>();
            var bills = (from a in db.GetTable<Bill>() select a).ToList<Bill>();
            List<String> months = new List<string>();

            foreach (var bil in bills)
            {
                if (!months.Contains(bil.Authorization))
                {
                    List<string> charges = (from a in db.GetTable<Bill>() where a.Authorization == bil.Authorization && a.Date.Value.Month == monthCh select a.Charge).ToList<string>();
                    dataPoints.Add(new Model { SocialNetwork = bil.Authorization, Status = sumCharges(charges) });
                    months.Add(bil.Authorization);
                    //PaletteList.Add(bil.Date.Value.ToString("MMM-yy"));
                }

            }

            foreach (Model m in dataPoints.OrderByDescending(x => x.Status).Take(10))
            {
                ActiveUsers.Add(m);
            }

        }
    }
}
