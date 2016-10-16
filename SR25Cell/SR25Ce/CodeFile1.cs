using System;
using System.Collections.Generic;
using System.Collections;

namespace SR25Ce
{
    public class NutList : IDisposable
    {
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Food { get; set; }

        public NutList(string v, string u, string f)
        {
            Value = v;
            Unit = u;
            Food = f;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }


    public static class Comm
    {
        const string F0 = "{0:F0}";
        const string F1 = "{0:F1}";
        const string F2 = "{0:F2}";
        const string F3 = "{0:F3}";

        public static string[] StrFormat = { F0, F1, F2, F3 };

        public static Sr25DataSet ds { get; set; }

        //public static Sr25DataSet.Fd_GrpDataTable fg;

        //public static Sr25DataSet.FOOD_DESDataTable ft;

        //public static Sr25DataSet.ndataDataTable data;

        //public static Sr25DataSet.NUT_DEFDataTable nt;

        public static void sett(SR25Ce.Sr25DataSet d)
        {
            ds = d;
            //fg = ds.Fd_Grp;
            //ft = ds.FOOD_DES;
            //data = ds.ndata;
            //nt = ds.NUT_DEF;
       

        }

     
    }

    
}