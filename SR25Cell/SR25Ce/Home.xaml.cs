using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SR25Ce
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Home : Window
    {
        Sr25DataSet ds = null;
        int NDB_No;
        string foo_Desc;
        BackgroundWorker bg = new BackgroundWorker();

        public Home()
        {
            InitializeComponent();
            ds = (Sr25DataSet)this.FindResource("sr25DataSet");
            this.Loaded += Home_Loaded;
            bg.DoWork += bg_DoWork;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
        }

         
      

       

        #region WinInit

        void Home_Loaded(object sender, RoutedEventArgs e)
        {
            Loaddata();
            Comm.sett(ds);
            ComboSetUp();
        }

        //loads groups and ndef files.
        private void Loaddata()
        {
            //DateTime s = DateTime.Now;
            Listing.Visibility = System.Windows.Visibility.Hidden;
            Detail.Visibility = System.Windows.Visibility.Hidden;

            bg.RunWorkerAsync(); // start loading the Bigger datatable.
            // Load data into the table Fd_Grp. You can modify this code as needed.
            SR25Ce.Sr25DataSetTableAdapters.Fd_GrpTableAdapter gda = new SR25Ce.Sr25DataSetTableAdapters.Fd_GrpTableAdapter();
            gda.Fill(ds.Fd_Grp);
            gda.Dispose();

            System.Windows.Data.CollectionViewSource fd_GrpViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fd_GrpViewSource")));
            fd_GrpViewSource.View.MoveCurrentToFirst();
            // Load data into the table NUT_DEF. You can modify this code as needed.
            SR25Ce.Sr25DataSetTableAdapters.NUT_DEFTableAdapter nda = new SR25Ce.Sr25DataSetTableAdapters.NUT_DEFTableAdapter();
            nda.Fill(ds.NUT_DEF);
            System.Windows.Data.CollectionViewSource nUT_DEFViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("nUT_DEFViewSource")));
            nUT_DEFViewSource.View.MoveCurrentToFirst();
            nda.Dispose();

            SR25Ce.Sr25DataSetTableAdapters.FOOD_DESTableAdapter des = new Sr25DataSetTableAdapters.FOOD_DESTableAdapter();
            des.Fill(ds.FOOD_DES);


            des.Dispose();
     
            //DateTime en = DateTime.Now;
            //TimeSpan sp = en.Subtract(s);
            //string ss = string.Format("Time taken is {0} min {1} sec {2} mil.", sp.Minutes, sp.Seconds, sp.Milliseconds);
            //this.Title = ss;
        }
        private void ComboSetUp()
        {
            if (Group.SelectedItem == null)
                return;

            System.Data.DataRowView rv0 = (System.Data.DataRowView)Group.SelectedItem;
            Sr25DataSet.Fd_GrpRow sg = (Sr25DataSet.Fd_GrpRow)rv0.Row;

            //Lambda version
            var lfdes = ds.FOOD_DES.Where(f => f.FdGrp_Cd == sg.FdGrp_CD);


            // LINQ Version
            //var fdes = from f in ds.FOOD_DES
            //           where f.FdGrp_Cd == sg.FdGrp_CD
            //           orderby f.Desc
            //           select new { NDB_No = f.NDB_No, Desc = f.Desc };

            inGroup.DataContext = lfdes;
        }
        #endregion WinInit

        #region EventHandlers

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (GetFoodCode() == true)
            {
                inFood inf = new inFood();
                inf.Ndb_No = NDB_No;
                inf.Foo = foo_Desc;
                inf.Topmost = true;
                inf.ShowDialog();
                inf.Dispose();
            }
        }


        private void Listing_Click(object sender, RoutedEventArgs e)
        {
            List2 wpfForm = new List2();
            wpfForm.Topmost = true;
            wpfForm.ShowDialog();
            wpfForm.Dispose();

            
        }

        private void Group_Selected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboSetUp();
        }

 

        private bool GetFoodCode()
        {
            if (inGroup.SelectedValue == null || string.IsNullOrWhiteSpace(inGroup.Text) == true)
            {
                MessageBox.Show("ComboBoxes selections Not made!!!");
                return false;
            }
            NDB_No = (int)inGroup.SelectedValue;
            foo_Desc = inGroup.Text;
            return true;
        }



        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Listing.Visibility = System.Windows.Visibility.Visible;
            Detail.Visibility = System.Windows.Visibility.Visible;
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Loadndata();
        }



        #endregion EventHandlers

        #region ndataLoad
        private void Loadndata()
        {
            if (ds.ndata.Rows.Count == 0)
            {
                SR25Ce.Sr25DataSetTableAdapters.ndataTableAdapter da = new Sr25DataSetTableAdapters.ndataTableAdapter();
                da.Fill(ds.ndata);
                da.Dispose();
            }
        }
        #endregion ndataLoad

    }
}
