using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GMS
{
    public partial class SalesReport : Form
    {
        private string connectionString = "Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True";
        
        private DataGridView dgvSalesReport;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnSearch;
        private Button btnExport;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnBack;
        private Label lblTotalSales;
        private Label lblTotalSavings;
        private Label lblTotalBills;

        public SalesReport()
        {
            SetupForm();
        }

        private void SetupForm()
        {
            // Form Properties
            this.Text = "ABC GROCERY - Sales Report System";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 230, 230);
            this.Font = new Font("Segoe UI", 9);
            this.KeyPreview = true;
            this.KeyDown += SalesReport_KeyDown;

            // Create main container
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            mainPanel.Padding = new Padding(10);

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = Color.FromArgb(0, 123, 255);
            headerPanel.Padding = new Padding(20, 10, 20, 10);

            // Back Button
            btnBack = new Button();
            btnBack.Text = "← Back to Main (F2)";
            btnBack.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBack.Size = new Size(140, 35);
            btnBack.Location = new Point(20, 15);
            btnBack.BackColor = Color.FromArgb(108, 117, 125);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += BtnBack_Click;

            Label lblTitle = new Label();
            lblTitle.Text = "ABC GROCERY";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(180, 15);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Professional Sales Report System";
            lblSubtitle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.White;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(185, 55);

            Label lblTagline = new Label();
            lblTagline.Text = "Fast • Reliable • Best Prices in Town";
            lblTagline.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblTagline.ForeColor = Color.LightYellow;
            lblTagline.AutoSize = true;
            lblTagline.Location = new Point(185, 75);

            headerPanel.Controls.Add(btnBack);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTagline);

            // Filter Panel
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 70;
            filterPanel.BackColor = Color.FromArgb(248, 249, 250);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Padding = new Padding(20);

            Label lblFilter = new Label();
            lblFilter.Text = "Select date range to filter reports...";
            lblFilter.Location = new Point(20, 15);
            lblFilter.Size = new Size(200, 20);
            lblFilter.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblFilter.ForeColor = Color.Gray;

            Label lblStart = new Label();
            lblStart.Text = "From Date:";
            lblStart.Location = new Point(20, 40);
            lblStart.Size = new Size(70, 20);
            lblStart.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dtpStartDate = new DateTimePicker();
            dtpStartDate.Location = new Point(95, 37);
            dtpStartDate.Size = new Size(120, 25);
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Font = new Font("Segoe UI", 9);

            Label lblEnd = new Label();
            lblEnd.Text = "To Date:";
            lblEnd.Location = new Point(230, 40);
            lblEnd.Size = new Size(60, 20);
            lblEnd.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dtpEndDate = new DateTimePicker();
            dtpEndDate.Location = new Point(295, 37);
            dtpEndDate.Size = new Size(120, 25);
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Font = new Font("Segoe UI", 9);

            btnSearch = new Button();
            btnSearch.Text = "Search Report";
            btnSearch.Location = new Point(430, 35);
            btnSearch.Size = new Size(120, 30);
            btnSearch.BackColor = Color.FromArgb(40, 167, 69);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSearch.Click += BtnSearch_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(560, 35);
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.BackColor = Color.FromArgb(23, 162, 184);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnRefresh.Click += BtnRefresh_Click;

            filterPanel.Controls.Add(lblFilter);
            filterPanel.Controls.Add(lblStart);
            filterPanel.Controls.Add(dtpStartDate);
            filterPanel.Controls.Add(lblEnd);
            filterPanel.Controls.Add(dtpEndDate);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnRefresh);

            // Summary Panel
            Panel summaryPanel = new Panel();
            summaryPanel.Dock = DockStyle.Top;
            summaryPanel.Height = 80;
            summaryPanel.BackColor = Color.White;
            summaryPanel.BorderStyle = BorderStyle.FixedSingle;
            summaryPanel.Padding = new Padding(20, 10, 20, 10);

            // Total Amount Panel
            Panel salesPanel = new Panel();
            salesPanel.Location = new Point(20, 10);
            salesPanel.Size = new Size(150, 60);
            salesPanel.BorderStyle = BorderStyle.FixedSingle;
            salesPanel.BackColor = Color.FromArgb(248, 249, 250);

            Label lblSalesText = new Label();
            lblSalesText.Text = "TOTAL AMOUNT";
            lblSalesText.Location = new Point(5, 5);
            lblSalesText.Size = new Size(140, 20);
            lblSalesText.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblSalesText.TextAlign = ContentAlignment.MiddleCenter;

            lblTotalSales = new Label();
            lblTotalSales.Text = "Rs. 0.00";
            lblTotalSales.Location = new Point(5, 25);
            lblTotalSales.Size = new Size(140, 30);
            lblTotalSales.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTotalSales.ForeColor = Color.FromArgb(40, 167, 69);
            lblTotalSales.TextAlign = ContentAlignment.MiddleCenter;

            salesPanel.Controls.Add(lblSalesText);
            salesPanel.Controls.Add(lblTotalSales);

            // Savings Panel
            Panel savingsPanel = new Panel();
            savingsPanel.Location = new Point(180, 10);
            savingsPanel.Size = new Size(150, 60);
            savingsPanel.BorderStyle = BorderStyle.FixedSingle;
            savingsPanel.BackColor = Color.FromArgb(248, 249, 250);

            Label lblSavingsText = new Label();
            lblSavingsText.Text = "YOU SAVE";
            lblSavingsText.Location = new Point(5, 5);
            lblSavingsText.Size = new Size(140, 20);
            lblSavingsText.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblSavingsText.TextAlign = ContentAlignment.MiddleCenter;

            lblTotalSavings = new Label();
            lblTotalSavings.Text = "Rs. 0.00";
            lblTotalSavings.Location = new Point(5, 25);
            lblTotalSavings.Size = new Size(140, 30);
            lblTotalSavings.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTotalSavings.ForeColor = Color.FromArgb(255, 193, 7);
            lblTotalSavings.TextAlign = ContentAlignment.MiddleCenter;

            savingsPanel.Controls.Add(lblSavingsText);
            savingsPanel.Controls.Add(lblTotalSavings);

            // Bills Panel
            Panel billsPanel = new Panel();
            billsPanel.Location = new Point(340, 10);
            billsPanel.Size = new Size(150, 60);
            billsPanel.BorderStyle = BorderStyle.FixedSingle;
            billsPanel.BackColor = Color.FromArgb(248, 249, 250);

            Label lblBillsText = new Label();
            lblBillsText.Text = "TOTAL BILLS";
            lblBillsText.Location = new Point(5, 5);
            lblBillsText.Size = new Size(140, 20);
            lblBillsText.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblBillsText.TextAlign = ContentAlignment.MiddleCenter;

            lblTotalBills = new Label();
            lblTotalBills.Text = "0";
            lblTotalBills.Location = new Point(5, 25);
            lblTotalBills.Size = new Size(140, 30);
            lblTotalBills.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTotalBills.ForeColor = Color.FromArgb(220, 53, 69);
            lblTotalBills.TextAlign = ContentAlignment.MiddleCenter;

            billsPanel.Controls.Add(lblBillsText);
            billsPanel.Controls.Add(lblTotalBills);

            summaryPanel.Controls.Add(salesPanel);
            summaryPanel.Controls.Add(savingsPanel);
            summaryPanel.Controls.Add(billsPanel);

            // Data Grid View
            dgvSalesReport = new DataGridView();
            dgvSalesReport.Dock = DockStyle.Fill;
            dgvSalesReport.ReadOnly = true;
            dgvSalesReport.AllowUserToAddRows = false;
            dgvSalesReport.AllowUserToDeleteRows = false;
            dgvSalesReport.BackgroundColor = Color.White;
            dgvSalesReport.BorderStyle = BorderStyle.FixedSingle;
            dgvSalesReport.Font = new Font("Segoe UI", 9);

            // Button Panel
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 70;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonPanel.Padding = new Padding(20);

            btnExport = new Button();
            btnExport.Text = "EXPORT TO CSV";
            btnExport.Size = new Size(150, 40);
            btnExport.Location = new Point(800, 15);
            btnExport.BackColor = Color.FromArgb(40, 167, 69);
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnExport.Click += BtnExport_Click;

            btnDelete = new Button();
            btnDelete.Text = "DELETE DATA";
            btnDelete.Size = new Size(150, 40);
            btnDelete.Location = new Point(970, 15);
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.Add(btnExport);
            buttonPanel.Controls.Add(btnDelete);

            // Add controls to main panel
            mainPanel.Controls.Add(dgvSalesReport);
            mainPanel.Controls.Add(summaryPanel);
            mainPanel.Controls.Add(filterPanel);
            mainPanel.Controls.Add(headerPanel);

            // Add to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);

            // Initialize and load data
            InitializeData();
        }

        // Back Button Logic with session check
        private void BtnBack_Click(object sender, EventArgs e)
        {
            ReturnToMainForm();
        }

        private void SalesReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                ReturnToMainForm();
            }
        }

        private void ReturnToMainForm()
        {
            // Check if user session is still valid
            if (ApplicationContext.UserID == 0)
            {
                MessageBox.Show("Session expired. Please login again.", "Session Expired", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Hide();
            }
            else
            {
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
        }

        private void InitializeData()
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Today;
            FormatDataGridView();
            LoadSalesReport();
        }

        private void FormatDataGridView()
        {
            dgvSalesReport.AutoGenerateColumns = false;
            dgvSalesReport.Columns.Clear();

            // Add columns
            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "BillID",
                HeaderText = "BILL ID",
                Width = 80
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "BillDate",
                HeaderText = "DATE & TIME",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "CustomerName",
                HeaderText = "CUSTOMER NAME",
                Width = 180
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "CustomerPhone",
                HeaderText = "PHONE",
                Width = 120
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TotalAmount",
                HeaderText = "TOTAL AMOUNT (Rs.)",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle() 
                { 
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TotalSavings",
                HeaderText = "SAVINGS (Rs.)",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle() 
                { 
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "PaymentMethod",
                HeaderText = "PAYMENT METHOD",
                Width = 130
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TotalItems",
                HeaderText = "ITEMS COUNT",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvSalesReport.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "TotalQuantity",
                HeaderText = "TOTAL QTY",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            // Style the DataGridView
            dgvSalesReport.EnableHeadersVisualStyles = false;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 58, 64);
            dgvSalesReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSalesReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dgvSalesReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgvSalesReport.RowHeadersVisible = false;
        }

        private void LoadSalesReport()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetSalesReport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", dtpStartDate.Value.Date);
                        cmd.Parameters.AddWithValue("@EndDate", dtpEndDate.Value.Date);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvSalesReport.DataSource = dt;
                        CalculateSummary(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales report: {ex.Message}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateSummary(DataTable dt)
        {
            decimal totalSales = 0;
            decimal totalSavings = 0;
            int totalBills = dt.Rows.Count;

            foreach (DataRow row in dt.Rows)
            {
                totalSales += Convert.ToDecimal(row["TotalAmount"]);
                totalSavings += Convert.ToDecimal(row["TotalSavings"]);
            }

            lblTotalSales.Text = $"Rs. {totalSales:N2}";
            lblTotalSavings.Text = $"Rs. {totalSavings:N2}";
            lblTotalBills.Text = totalBills.ToString("N0");
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (dtpStartDate.Value > dtpEndDate.Value)
            {
                MessageBox.Show("Start date cannot be greater than end date!", 
                    "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            LoadSalesReport();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Today;
            LoadSalesReport();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteSalesData();
        }

        private void ExportToCSV()
        {
            try
            {
                if (dgvSalesReport.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export!", "Export", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV Files|*.csv";
                saveDialog.FileName = $"ABC_GROCERY_Sales_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                saveDialog.Title = "Export Sales Report to CSV";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        // Write header information
                        writer.WriteLine("ABC GROCERY - SALES REPORT");
                        writer.WriteLine($"Report Period: {dtpStartDate.Value:dd/MM/yyyy} to {dtpEndDate.Value:dd/MM/yyyy}");
                        writer.WriteLine($"Exported On: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        writer.WriteLine("All amounts in Sri Lankan Rupees (LKR)");
                        writer.WriteLine();

                        // Write column headers
                        List<string> headers = new List<string>();
                        foreach (DataGridViewColumn col in dgvSalesReport.Columns)
                        {
                            headers.Add($"\"{col.HeaderText}\"");
                        }
                        writer.WriteLine(string.Join(",", headers));

                        // Write data rows
                        foreach (DataGridViewRow row in dgvSalesReport.Rows)
                        {
                            if (row.IsNewRow) continue;

                            List<string> cells = new List<string>();
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string value = cell.Value?.ToString() ?? "";
                                if (cell.OwningColumn.DataPropertyName == "BillDate" && DateTime.TryParse(value, out DateTime dateValue))
                                {
                                    value = dateValue.ToString("dd/MM/yyyy HH:mm");
                                }
                                value = value.Replace("\"", "\"\"");
                                cells.Add($"\"{value}\"");
                            }
                            writer.WriteLine(string.Join(",", cells));
                        }

                        // Write summary section
                        writer.WriteLine();
                        writer.WriteLine("SUMMARY REPORT");
                        writer.WriteLine($"Total Bills,\"{lblTotalBills.Text}\"");
                        writer.WriteLine($"Total Sales (LKR),\"{lblTotalSales.Text}\"");
                        writer.WriteLine($"Total Customer Savings (LKR),\"{lblTotalSavings.Text}\"");
                        writer.WriteLine();
                        writer.WriteLine("Generated by ABC GROCERY Professional Billing System");
                    }

                    MessageBox.Show($"Sales report exported successfully!\n\nFile: {saveDialog.FileName}", 
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSalesData()
        {
            DialogResult result = MessageBox.Show(
                "⚠️  W A R N I N G  ⚠️\n\n" +
                "This will PERMANENTLY DELETE all sales data for the selected period!\n\n" +
                "• All bill records\n" +
                "• All bill detail records\n" +
                "• All sales history\n\n" +
                "This action CANNOT be undone!\n\n" +
                "Are you absolutely sure you want to continue?",
                "CONFIRM DATA DELETION",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteSalesData", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@StartDate", dtpStartDate.Value.Date);
                            cmd.Parameters.AddWithValue("@EndDate", dtpEndDate.Value.Date);
                            cmd.Parameters.AddWithValue("@DeleteBillDetails", 1);

                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }

                    MessageBox.Show($"Sales data deleted successfully!\n\nPeriod: {dtpStartDate.Value:dd/MM/yyyy} to {dtpEndDate.Value:dd/MM/yyyy}", 
                        "Delete Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadSalesReport();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Delete failed: {ex.Message}", "Delete Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}