using SalesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SalesApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            FillItem();
            ResetSalesMaster();
            ResetSalesItem();
            SetGridSalesMaster();
        }

        private void SetGridSalesMaster()
        {
            dgvSales.DataSource = null;
            dgvSales.DataSource = GetSalesMaster();
        }

        public List<SalesMasterVM> GetSalesMaster()
        {
            var listSalesMaster = new List<SalesMasterVM>();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                SqlCommand cmd = new SqlCommand(@"SELECT [SaleId],[CustomerName],[Phone],[Address] FROM [SalesMaster] ORDER BY [SaleId]", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var oSalesMaster = new SalesMasterVM();
                    oSalesMaster.SaleId = dataReader.GetInt32(0);
                    oSalesMaster.CustomerName = dataReader.GetString(1);
                    oSalesMaster.Phone = dataReader.GetString(2);
                    oSalesMaster.Address = dataReader.GetString(3);
                    listSalesMaster.Add(oSalesMaster);
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return listSalesMaster;
        }

        public SalesMasterVM GetSalesMaster(int salesId)
        {
            var oSalesMaster = new SalesMasterVM();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                SqlCommand cmd = new SqlCommand(@"SELECT [SaleId],[CustomerName],[Phone],[Address] FROM [SalesMaster] WHERE [SaleId]=" + salesId, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    oSalesMaster.SaleId = dataReader.GetInt32(0);
                    oSalesMaster.CustomerName = dataReader.GetString(1);
                    oSalesMaster.Phone = dataReader.GetString(2);
                    oSalesMaster.Address = dataReader.GetString(3);
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return oSalesMaster;
        }

        public List<SalesItemVM> GetSalesItem(int saleMasterId)
        {
            var listSalesItem = new List<SalesItemVM>();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                SqlCommand cmd = new SqlCommand(@"SELECT si.[SaleItemId]
                                                    ,si.[SaleMasterId]
                                                    ,si.[ItemId]
                                                    ,si.[UnitPrice]
                                                    ,si.[Quantity]
                                                    ,si.[Vat]
                                                    ,si.[Amount]
                                                    ,i.ItemName
                                                    FROM [SalesItem] si
                                                    LEFT JOIN [Item] i ON i.ItemId=si.ItemId
                                                    WHERE si.[SaleMasterId]=" + saleMasterId, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var oSaleItem = new SalesItemVM();
                    oSaleItem.SaleItemId = dataReader.GetInt32(0);
                    oSaleItem.SaleMasterId = dataReader.GetInt32(1);
                    oSaleItem.ItemId = dataReader.GetInt32(2);
                    oSaleItem.UnitPrice = dataReader.GetDecimal(3);
                    oSaleItem.Quantity = dataReader.GetDecimal(4);
                    oSaleItem.Vat = dataReader.GetDecimal(5);
                    oSaleItem.Amount = dataReader.GetDecimal(6);
                    oSaleItem.ItemName = dataReader.GetString(7);
                    listSalesItem.Add(oSaleItem);
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return listSalesItem;
        }
        public List<ItemVM> GetItem()
        {
            var listItem = new List<ItemVM>();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                SqlCommand cmd = new SqlCommand("SELECT ItemId, ItemName FROM Item ORDER BY ItemName", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                
                while (dataReader.Read())
                {
                    ItemVM oItem = new ItemVM();
                    oItem.ItemId = dataReader.GetInt32(0);
                    oItem.ItemName = dataReader.GetString(1);
                    listItem.Add(oItem);
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return listItem;
        }

        public void FillItem()
        {
            try
            {
                cbItem.Items.Clear();
                cbItem.DataSource = GetItem();
                cbItem.ValueMember = "ItemId";
                cbItem.DisplayMember = "ItemName";
                cbItem.SelectedIndex = 0;
            }
            catch
            {
            }
            finally
            {
            }
        }

        public ItemVM GetItemById(int id)
        {
            var oItem = new ItemVM();
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                SqlCommand cmd = new SqlCommand("SELECT ItemId, ItemName, UnitPrice, Vat FROM Item WHERE ItemId = " + id, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    oItem.ItemId = dataReader.GetInt32(0);
                    oItem.ItemName = dataReader.GetString(1);
                    oItem.UnitPrice = dataReader.GetDecimal(2);
                    oItem.Vat = dataReader.GetDecimal(3);
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return oItem;
        }
        private void cbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (ItemVM)cbItem.SelectedItem;
            var oItem = GetItemById(item.ItemId);
            if (oItem != null)
            {
                txtUnitPrice.Text = oItem.UnitPrice.ToString();
                txtVat.Text = oItem.Vat.ToString();
                txtAmount.Text = CalculateAmount(Convert.ToDecimal(oItem.UnitPrice), Convert.ToDecimal(txtQty.Text.Trim()), Convert.ToDecimal(oItem.Vat)).ToString();
            }
        }

        private decimal CalculateAmount(decimal unitPrice, decimal quantity, decimal vat)
        {
            decimal unitPriceVated = unitPrice + (unitPrice * (vat / 100));
            return unitPriceVated * quantity;
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            txtAmount.Text = CalculateAmount(Convert.ToDecimal(txtUnitPrice.Text.Trim()), Convert.ToDecimal(txtQty.Text.Trim()), Convert.ToDecimal(txtVat.Text.Trim())).ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SalesItemVM oSIVM = new SalesItemVM();
            oSIVM.Amount = Convert.ToDecimal(txtAmount.Text.Trim());
            oSIVM.UnitPrice = Convert.ToDecimal(txtUnitPrice.Text.Trim());
            oSIVM.Quantity = Convert.ToDecimal(txtQty.Text.Trim());
            oSIVM.Vat = Convert.ToDecimal(txtVat.Text.Trim());
            if (oSIVM.UnitPrice > 0)
            {
                var oItem = (ItemVM)cbItem.SelectedItem;
                oSIVM.ItemId = oItem.ItemId;
                oSIVM.SaleItemId = (int)DateTime.UtcNow.Subtract(new DateTime(2020, 1, 1)).TotalSeconds;
                oSIVM.ItemName = oItem.ItemName.Trim();
                var listSI = dgvItem.DataSource == null ? new List<SalesItemVM>() : (List<SalesItemVM>)dgvItem.DataSource;
                listSI.Add(oSIVM);
                dgvItem.DataSource = null;
                dgvItem.DataSource = listSI;
                ResetSalesItem();
            }
            else 
            {
                MessageBox.Show("Select a item to sell.");
            }
        }
        public SalesMasterVM AddSalesMaster(SalesMasterVM model)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                #region ADD
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                cmd = new SqlCommand("INSERT INTO SalesMaster(Address,CustomerName,Phone) VALUES('" + model.Address + "','" + model.CustomerName + "','" + model.Phone + "')", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                #endregion
                #region Get Sales-Master-ID
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                cmd = new SqlCommand("SELECT MAX(SaleId) FROM SalesMaster", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                model.SaleId = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                con.Close();
                #endregion
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return model;
        }

        public SalesMasterVM UpdateSalesMaster(SalesMasterVM model)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                #region ADD
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                cmd = new SqlCommand("UPDATE SalesMaster SET Address='" + model.Address + "',CustomerName='" + model.CustomerName + "',Phone='" + model.Phone + "' WHERE SaleId=" + model.SaleId, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                #endregion
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return model;
        }

        public void RemoveSalesItem(int saleId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                #region ADD
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                cmd = new SqlCommand("DELETE SalesItem WHERE [SaleMasterId]=" + saleId, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                #endregion
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
        }

        public void RemoveSalesMaster(int saleId)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            try
            {
                #region ADD
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                cmd = new SqlCommand("DELETE SalesMaster WHERE SaleId=" + saleId, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                #endregion
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
        }

        public void AddSalesItem(List<SalesItemVM> salesItems, int saleMasterId)
        {
            foreach (SalesItemVM item in salesItems)
            {
                item.SaleMasterId = saleMasterId;
                SqlConnection con = null;
                SqlCommand cmd = null;
                try
                {
                    #region ADD
                    con = new SqlConnection(ConfigurationManager.ConnectionStrings["SalesConnStr"].ToString());
                    cmd = new SqlCommand(@"INSERT INTO [dbo].[SalesItem]
                                               ([SaleMasterId]
                                               ,[ItemId]
                                               ,[UnitPrice]
                                               ,[Quantity]
                                               ,[Vat]
                                               ,[Amount])
                                            VALUES
                                               ('"+ item.SaleMasterId + "'" +
                                               ",'"+item.ItemId+"'" +
                                               ",'"+item.UnitPrice+"'" +
                                               ",'"+item.Quantity+"'" +
                                               ",'"+item.Vat+"'" +
                                               ",'"+item.Amount+"')", con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    #endregion
                }
                catch
                {
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region Sales Master
            var SaleId = Convert.ToInt32(lblSaleId.Text.Trim());
            #region Set Sales-Master Data
            var oSalesMaster = new SalesMasterVM();
            oSalesMaster.SaleId = SaleId;
            oSalesMaster.Address = txtAddress.Text.Trim();
            oSalesMaster.CustomerName = txtCustomer.Text.Trim();
            oSalesMaster.Phone = txtPhone.Text.Trim();
            #endregion
            if (SaleId > 0)
            {
                #region UPDATE
                oSalesMaster = UpdateSalesMaster(oSalesMaster);
                #endregion
            }
            else
            {
                #region ADD
                oSalesMaster = AddSalesMaster(oSalesMaster);
                #endregion
            }
            #endregion
            #region Sales Items
            RemoveSalesItem(oSalesMaster.SaleId);
            var listSI = dgvItem.DataSource == null ? new List<SalesItemVM>() : (List<SalesItemVM>)dgvItem.DataSource;
            AddSalesItem(listSI, oSalesMaster.SaleId);
            #endregion
            ResetSalesItem();
            dgvItem.DataSource = null;
            ResetSalesMaster();
            SetGridSalesMaster();
            MessageBox.Show("Saved successfully.", "Message");
        }

        private void dgvItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItem.Columns[e.ColumnIndex].Name == "btnRemove")
            {
                var SaleItemId = Convert.ToInt32(dgvItem.Rows[e.RowIndex].Cells["SaleItemId"].Value);
                var listSalesItem = dgvItem.DataSource == null ? new List<SalesItemVM>() : (List<SalesItemVM>)dgvItem.DataSource;
                var oSalesItem = listSalesItem.Where(x => x.SaleItemId == SaleItemId).FirstOrDefault();
                if (oSalesItem != null) 
                {
                    listSalesItem.Remove(oSalesItem);
                    dgvItem.DataSource = null;
                    dgvItem.DataSource = listSalesItem;
                }
            }
        }

        private void ResetSalesItem() 
        {
            txtAmount.Text = "0";
            txtQty.Text = "0";
            txtUnitPrice.Text = "0";
            txtVat.Text = "0";
        }

        private void ResetSalesMaster()
        {
            lblSaleId.Text = "0";
            txtAddress.Text = "";
            txtCustomer.Text = "";
            txtPhone.Text = "";
            btnSubmit.Text = "ADD";
        }
        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSales.Columns[e.ColumnIndex].Name == "btnReports")
            {
               CrystalDetailsReport cdr = new CrystalDetailsReport();
                cdr.Show();
            }
            else if (dgvSales.Columns[e.ColumnIndex].Name == "btnDelete")
            {
                DialogResult dialogResult = MessageBox.Show("Are your sure to delete?", "Confirm!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    var SaleId = Convert.ToInt32(dgvSales.Rows[e.RowIndex].Cells["SaleId"].Value);
                    #region Sale Item
                    RemoveSalesItem(SaleId);
                    #endregion
                    #region Sale Master
                    RemoveSalesMaster(SaleId);
                    #endregion
                    ResetSalesMaster();
                    dgvItem.DataSource = null;
                    ResetSalesItem();
                    GetSalesMaster();
                    MessageBox.Show("Deleted successfully.", "Message");
                }
            }
            else if (dgvSales.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                var SaleId = Convert.ToInt32(dgvSales.Rows[e.RowIndex].Cells["SaleId"].Value);
                lblSaleId.Text = SaleId.ToString();
                #region Sale Master
                var oSM = GetSalesMaster(SaleId);
                if (oSM != null)
                {
                    txtAddress.Text = oSM.Address;
                    txtCustomer.Text = oSM.CustomerName;
                    txtPhone.Text = oSM.Phone;
                }
                #endregion
                #region Sale Item
                var listSI = GetSalesItem(SaleId);
                dgvItem.DataSource = null;
                dgvItem.DataSource = listSI;
                #endregion
                btnSubmit.Text = "UPDATE";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetSalesMaster();
            dgvItem.DataSource = null;
            ResetSalesItem();
            GetSalesMaster();
        }
        private void btnSCReport_Click(object sender, EventArgs e)
        {
            CrystalReportForm cf = new CrystalReportForm();
            cf.Show();
        }
    }
}