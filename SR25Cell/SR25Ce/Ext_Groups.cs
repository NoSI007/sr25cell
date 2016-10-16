using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SR25Ce
{
    public class Ext_Groups: IDisposable 
    {
        public int FdGrp_CD { get; set; }
        public string FdGrp_Desc { get; set; }

        public Ext_Groups(int gc, string gd)
        {
            FdGrp_CD = gc;
            FdGrp_Desc = gd;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
