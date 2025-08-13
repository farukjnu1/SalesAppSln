using SalesApp.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesApp
{
    public partial class CrystalDetailsReport : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-KQM4O6M;Initial Catalog=GPSDB;Integrated Security=True");
        public CrystalDetailsReport()
        {
            InitializeComponent();
        }

        private void CrystalDetailsReport_Load(object sender, EventArgs e)
        {
            con.Open();
            string q = "select * from SalesItem";
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds, "SalesItem");
            CrystalReport1 cr1 = new CrystalReport1();
            cr1.SetDataSource(ds);
            crystalReportViewer1.ReportSource = cr1;
            con.Close();
            crystalReportViewer1.Refresh();
            con.Close();
        }
    }
}
