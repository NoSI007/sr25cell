using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;


[assembly: CLSCompliant(true)]//4FxCop
namespace SR25Ce
{
    /// <summary>
    /// Interaction logic for inFood.xaml
    /// </summary>
    public sealed partial class inFood : Window, IDisposable
    {
        Sr25DataSet ds;
        public int Ndb_No { get; set; }
        public string Foo { get; set; }

        private Collection<NutList> _vlist = new Collection<NutList>();

        public Collection<NutList> ValueList 
        { 
            get { return _vlist; } 
            set { _vlist = value;} 
        }

        public inFood()
        {
            InitializeComponent();
            Loaded += inFood_Loaded;
        }

        void inFood_Loaded(object sender, RoutedEventArgs e)
        {
            ds = Comm.ds;

            if (ds.ndata.Rows.Count == 0)
                LoadData();
            //LINQ..
            //var nutqry = from d in ds.ndata
            //             where d.NDB_No == Ndb_No
            //             select new { Nutr_No = d.Nutr_No, Nutr_Val = d.Nutr_Val };

            //Lambda
            var nutqry = ds.ndata.Where(n => n.NDB_No == Ndb_No);

            foreach (var r in nutqry)
            {
                AddRow(r.Nutr_No, r.Nutr_Val);
            }

            NutV.DataContext = ValueList;
            Title = string.Format("{0} Records Found", ValueList.Count);
            Cfood.Text = this.Foo;
        }

        private void LoadData()
        {
            this.Title = "Loading data . . . . . . . . . . . .";
            Sr25DataSetTableAdapters.ndataTableAdapter nda = new Sr25DataSetTableAdapters.ndataTableAdapter();
            nda.Fill(ds.ndata);
            nda.Dispose();
        }

        private void AddRow(short p1, float p2)
        {

            Sr25DataSet.NUT_DEFRow ndr = ds.NUT_DEF.Where(n => n.Nutr_No == p1).First();

            //ListViewItem lbi = new ListViewItem();
            
            //StackPanel sp = new StackPanel();
            //sp.Height = 22;
            //sp.Orientation = Orientation.Horizontal;
            
            //Label  val = new Label ();
            //val.Padding = new Thickness(0);
            //val.Width = 80;
            //val.Height = 25;
            //val.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //val.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            int dec = int.Parse(ndr.Decimal);

            //val.Content = string.Format(Comm.StrFormat[dec], p2);
            string aval =  string.Format(Comm.StrFormat[dec], p2);
            //Label  uni = new Label();
            //uni.Padding = new Thickness(0);
            //uni.Height = 22;
            //uni.Width = 30;
            //uni.Content  = ndr.Units;
            //uni.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            //Label  nname = new Label ();
            //nname.Padding = new Thickness(0);
            //nname.Height = 22;
            //nname.Content = ndr.NutrDesc;
            //nname.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //sp.Children.Add(val);
            //sp.Children.Add(uni);
            //sp.Children.Add(nname);
            


            //lbi.Content = sp;
            //lbi.Height = 22;
            //lbi.FontWeight = FontWeights.Bold;
            //lbi.Padding = new Thickness(0);

            //NutV.Items.Add(lbi);

            ValueList.Add(new NutList(aval,ndr.Units,ndr.NutrDesc));

        }

        



        public void Dispose()
        {
            foreach (NutList n in ValueList)
            {
                n.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }
    }
}
