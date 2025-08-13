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
using System.Configuration;
namespace SalesApp
{
    public partial class CrystalReportForm : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-KQM4O6M;Initial Catalog=GPSDB;Integrated Security=True");
        public CrystalReportForm()
        {
            InitializeComponent();
        }

        private void CrystalReportForm_Load(object sender, EventArgs e)
        {
            con.Open();
            string q = "select * from SalesMaster;";
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds, "SalesMaster");
            CrystalReport1 cr1 = new CrystalReport1();
            cr1.SetDataSource(ds);
            crystalReportViewer1.ReportSource = cr1;
            con.Close();
            crystalReportViewer1.Refresh();
            con.Close();
        }    
    }
}
