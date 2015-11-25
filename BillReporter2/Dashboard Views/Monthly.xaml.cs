using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using BillReporter2.Dashboard_Views;
using BillReporter2;
using Syncfusion.UI.Xaml.Gauges;


namespace BillReporter2.Dashboard_Views
{
    /// <summary>
    /// Interaction logic for Monthly.xaml
    /// </summary>
    /// 
   
    public partial class Monthly : Page
    {
        public DataClasses1DataContext db = new DataClasses1DataContext();
        public Game game2 = new Game(0);
        public double perc = 0;
        
        public Monthly()
        {
            InitializeComponent();
            
            
            this.myGauge2.DataContext = game2;
            
            //Start the timer
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2500);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            DateTime lastDate = (from a in db.GetTable<Bill>() select a).OrderByDescending(x => x.Date).FirstOrDefault().Date.Value;
            //listofIDs.Select(int.Parse).ToList()
            List<double> thisMonths = convList((from a in db.GetTable<Bill>() where a.Date.Value.Month == lastDate.Month  select a.Charge).ToList<string>());
            List<double> lastMonths =convList((from a in db.GetTable<Bill>() where a.Date.Value.Month == lastDate.Month-1  select a.Charge).ToList<string>());
            perc = ((thisMonths.Sum()-lastMonths.Sum())/thisMonths.Sum())*100;
        }
        public List<double> convList(List<string> inList)
        {
            List<double> outL=new List<double>();
            foreach(string a in inList)
            {
                outL.Add(double.Parse(a));
            }
            return outL;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            game2.Score = perc;
        }
            
            
            
        }

       
    }
    public class Model
    {
        public string Country
        {
            get;
            set;
        }
        public string SocialNetwork
        {
            get;
            set;
        }
        public double Status
        {
            get;
            set;
        }
        public double Year2012
        {
            get;
            set;
        }
        public double Year2014
        {
            get;
            set;
        }
        public double Year2015
        {
            get;
            set;
        }
        public double YData4
        {
            get;
            set;
        }

    }
   

    
    public class Game : INotifyPropertyChanged
    {
        private double score;

        public double Score
        {
            get { return score; }
            set
            {
                score = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                }
            }
        }


        public Game(double scr)
        {
            this.Score = scr;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
   
