using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SR25Ce
{
    /// <summary>
    /// Interaction logic for List2.xaml
    /// </summary>
    public sealed partial class List2 : Window, IDisposable 
    {
        Collection<Ext_Groups> _groups = new Collection<Ext_Groups>();

        Sr25DataSet ds = null;

        Ext_Groups sg;//Hack change too.
        Sr25DataSet.NUT_DEFRow ndr;

        BackgroundWorker bw = new BackgroundWorker();
        object qryres = null;

        public int RecCount { get; set; }

        public List2()
        {
            InitializeComponent();
            Loaded += List2_Loaded;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }



        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            DisplayTheData();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            Ok.IsEnabled = true;
            SetHeaders();
            dataTable1ListView.DataContext = qryres;
            Hidep();
        }

        void List2_Loaded(object sender, RoutedEventArgs e)
        {
            ds = Comm.ds;
            // Load data into the table Fd_Grp. You can modify this code as needed.
            if (ds == null)
            {
                MessageBox.Show("Null Data set");
                this.Close();
            }

            _groups.Add(new Ext_Groups(0, "All groups"));

            foreach (Sr25DataSet.Fd_GrpRow r in ds.Fd_Grp)
            {
                _groups.Add(new Ext_Groups(r.FdGrp_CD, r.FdGrp_Desc));
            }

            Group.DataContext = _groups;
            Nutdef.DataContext = ds.NUT_DEF.DefaultView;
       
        }

        private void FoodValueData()
        {
            //lazy loading must beware this data is not loaded at start of the program.
            if (ds.ndata.Rows.Count == 0)
            {
                SR25Ce.Sr25DataSetTableAdapters.ndataTableAdapter da = new Sr25DataSetTableAdapters.ndataTableAdapter();
                da.Fill(ds.ndata);
                da.Dispose();
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //Dispatcher.Invoke(new MakeVisibile(SetVisible), System.Windows.Threading.DispatcherPriority.Render, null);
            if (bw.IsBusy != true)
            {

                if( SetParam() == true)
                {
                    SetVisible();
                    Ok.IsEnabled = false;
                    bw.RunWorkerAsync();
                    this.UpdateLayout();
                }
            }

        }
        
        private void DisplayTheData()
        {

            if (ndr == null || sg == null)
                return;

            FoodValueData();

            
            // OLD Linq
            //var ndblist = (from f in ds.FOOD_DES
            //                where f.FdGrp_Cd == sg.Gcode
            //                select f).ToList<Sr25DataSet.FOOD_DESRow>();
            // New Lambda 
            var foodds = ds.FOOD_DES.Where(f => sg.FdGrp_CD == 0 ? (true) : (sg.FdGrp_CD == f.FdGrp_Cd));
            var dds = ds.ndata.Where(d => d.Nutr_No == ndr.Nutr_No);
            int dp = int.Parse(ndr.Decimal);
            //MessageBox.Show(string.Format("Foods = {0}    Data Records = {1}",foodds.Count(),dds.Count()));

            var qry = from sf in foodds
                        join fd in dds on sf.NDB_No equals fd.NDB_No
                        orderby fd.Nutr_Val descending
                        select new { Nutr_Val = string.Format(Comm.StrFormat[dp],fd.Nutr_Val), Desc = sf.Desc };

            qryres = qry.ToList();

            RecCount = qry.Count();
            
        }

        private void SetHeaders()
        {
            Nutrient.Content = string.Format(" Containing {0} ", ndr.NutrDesc);

            Units.Content = String.Format("in ( {0} ) ", ndr.Units);

            FdGrp.Content = string.Format("{0}", sg.FdGrp_Desc);
            Recf.Content = string.Format("{0} Records Found.", RecCount);
            dtgen.Content = string.Format("Content generated {0} - Local Time.",DateTime.Now.ToLocalTime());
        }

        private bool SetParam()
        {

            if (Group.SelectedItem == null || Nutdef.SelectedItem == null)
            {
                MessageBox.Show("The Comboboxes are not set!!!");
                return false;
            }


            //HACK: change to a list ....
            //System.Data.DataRowView rv0 = (System.Data.DataRowView)Group.SelectedItem;
            //sg = (Sr25DataSet.Fd_GrpRow)rv0.Row;
            if (sg == null)
            {
                sg = new Ext_Groups((int)Group.SelectedValue, Group.Text);
            }
            else
            {
                sg.FdGrp_Desc = Group.Name;
                sg.FdGrp_CD = (int)Group.SelectedValue;
            }

            System.Data.DataRowView rv1 = (System.Data.DataRowView)Nutdef.SelectedItem;

            ndr = (Sr25DataSet.NUT_DEFRow)rv1.Row;
            
            if (ndr == null)
                return false;

            return true;
        }


        public void SetVisible()
        {
            zprog.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hidep()
        {
            this.zprog.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Dispose()
        {
            bw.Dispose();
            foreach (Ext_Groups e in _groups)
            {
                e.Dispose();
            }
            
            GC.SuppressFinalize(this);
        }
    }
}
