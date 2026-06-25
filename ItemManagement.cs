using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GMS
{
    public partial class ItemManagement : Form
    {
        private string connectionString = "Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True";
        private Timer searchTimer;
        private TextBox txtSearch;
        private Label lblTitle;
        private DataGridView dgvItems;
        private Button btnAddNew, btnEdit, btnDelete, btnRefresh, btnBack;
        private Panel panel1, panel2;
        private Label lblSearchHint;

        public ItemManagement()
        {
            InitializeCustomComponents();
            SetupSearchTimer();
            LoadItems();
            
            // Enable keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += ItemManagement_KeyDown;
        }

        private void InitializeCustomComponents()
        {
            // Form setup
            this.Text = "ABC GROCERY - Item Management";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 230, 230);

            // Title Panel
            panel1 = new Panel();
            panel1.BackColor = Color.FromArgb(41, 128, 185);
            panel1.Dock = DockStyle.Top;
            panel1.Height = 80;

            lblTitle = new Label();
            lblTitle.Text = "ITEM MANAGEMENT";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            panel1.Controls.Add(lblTitle);

            // Control Panel
            panel2 = new Panel();
            panel2.BackColor = Color.White;
            panel2.Dock = DockStyle.Top;
            panel2.Height = 70;
            panel2.Padding = new Padding(20, 10, 20, 10);

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
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += BtnBack_Click;

            // Search Box with placeholder text implementation
            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.Size = new Size(300, 35);
            txtSearch.Location = new Point(170, 15); // Adjusted for back button
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Search items...";
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Add hint label for better placeholder effect
            lblSearchHint = new Label();
            lblSearchHint.Text = "Search items...";
            lblSearchHint.Font = new Font("Segoe UI", 11);
            lblSearchHint.ForeColor = Color.Gray;
            lblSearchHint.BackColor = Color.White;
            lblSearchHint.Location = new Point(175, 22); // Adjusted for back button
            lblSearchHint.Size = new Size(150, 20);
            lblSearchHint.Click += LblSearchHint_Click;

            // Buttons (Adjusted positions for back button)
            btnAddNew = new Button();
            btnAddNew.Text = "Add New";
            btnAddNew.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddNew.Size = new Size(120, 35);
            btnAddNew.Location = new Point(490, 15); // Adjusted for back button
            btnAddNew.BackColor = Color.FromArgb(46, 204, 113);
            btnAddNew.ForeColor = Color.White;
            btnAddNew.FlatStyle = FlatStyle.Flat;
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Cursor = Cursors.Hand;
            btnAddNew.Click += BtnAddNew_Click;

            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit.Size = new Size(100, 35);
            btnEdit.Location = new Point(620, 15); // Adjusted for back button
            btnEdit.BackColor = Color.FromArgb(52, 152, 219);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "Delete";
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.Size = new Size(100, 35);
            btnDelete.Location = new Point(730, 15); // Adjusted for back button
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(840, 15); // Adjusted for back button
            btnRefresh.BackColor = Color.FromArgb(155, 89, 182);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += BtnRefresh_Click;

            panel2.Controls.AddRange(new Control[] { btnBack, txtSearch, lblSearchHint, btnAddNew, btnEdit, btnDelete, btnRefresh });

            // DataGridView
            dgvItems = new DataGridView();
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.Font = new Font("Segoe UI", 10);
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.ReadOnly = true;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvItems.DefaultCellStyle.Padding = new Padding(5);
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;
            dgvItems.RowTemplate.Height = 35;
            dgvItems.CellDoubleClick += DgvItems_CellDoubleClick;

            // Create columns
            var column1 = new DataGridViewTextBoxColumn();
            column1.Name = "ItemID";
            column1.HeaderText = "ITEM ID";
            column1.DataPropertyName = "ItemID";
            column1.Width = 80;

            var column2 = new DataGridViewTextBoxColumn();
            column2.Name = "ItemName";
            column2.HeaderText = "PRODUCT NAME";
            column2.DataPropertyName = "ItemName";
            column2.Width = 250;

            var column3 = new DataGridViewTextBoxColumn();
            column3.Name = "Price";
            column3.HeaderText = "PRICE (Rs.)";
            column3.DataPropertyName = "Price";
            column3.DefaultCellStyle.Format = "N2";
            column3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column3.Width = 120;

            var column4 = new DataGridViewTextBoxColumn();
            column4.Name = "DiscountPercentage";
            column4.HeaderText = "DISCOUNT (%)";
            column4.DataPropertyName = "DiscountPercentage";
            column4.DefaultCellStyle.Format = "N1";
            column4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column4.Width = 120;

            var column5 = new DataGridViewTextBoxColumn();
            column5.Name = "FinalPrice";
            column5.HeaderText = "FINAL PRICE (Rs.)";
            column5.DefaultCellStyle.Format = "N2";
            column5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column5.Width = 140;

            dgvItems.Columns.AddRange(new DataGridViewColumn[] { column1, column2, column3, column4, column5 });

            // Add panels to form
            this.Controls.Add(dgvItems);
            this.Controls.Add(panel2);
            this.Controls.Add(panel1);
        }

        // Back button click event
        private void BtnBack_Click(object sender, EventArgs e)
        {
            ReturnToMainForm();
        }

        // Keyboard shortcut handler
        private void ItemManagement_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    ReturnToMainForm();
                    break;
                    
                case Keys.F5:
                    LoadItems();
                    txtSearch.Text = "Search items...";
                    txtSearch.ForeColor = Color.Gray;
                    lblSearchHint.Visible = true;
                    break;
                    
                case Keys.Insert:
                case Keys.F3:
                    BtnAddNew_Click(sender, e);
                    break;
                    
                case Keys.F6:
                    if (dgvItems.SelectedRows.Count > 0)
                    {
                        BtnEdit_Click(sender, e);
                    }
                    break;
                    
                case Keys.Delete:
                    if (dgvItems.SelectedRows.Count > 0)
                    {
                        BtnDelete_Click(sender, e);
                    }
                    break;
                    
                case Keys.Escape:
                    txtSearch.Text = "Search items...";
                    txtSearch.ForeColor = Color.Gray;
                    lblSearchHint.Visible = true;
                    dgvItems.ClearSelection();
                    break;
            }
        }

        // Method to return to MainForm with session check
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

        private void TxtSearch_GotFocus(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search items...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
            lblSearchHint.Visible = false;
        }

        private void TxtSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search items...";
                txtSearch.ForeColor = Color.Gray;
                lblSearchHint.Visible = true;
            }
        }

        private void LblSearchHint_Click(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void SetupSearchTimer()
        {
            searchTimer = new Timer();
            searchTimer.Interval = 300; // 300ms delay
            searchTimer.Tick += SearchTimer_Tick;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Only search if it's not the placeholder text
            if (txtSearch.Text != "Search items...")
            {
                // Reset and start timer
                searchTimer.Stop();
                searchTimer.Start();
            }
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            SearchItems(txtSearch.Text.Trim());
        }

        private void LoadItems()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("GetAllItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Calculate final price for each item
                        DataTable dtWithFinalPrice = new DataTable();
                        dtWithFinalPrice.Columns.Add("ItemID", typeof(int));
                        dtWithFinalPrice.Columns.Add("ItemName", typeof(string));
                        dtWithFinalPrice.Columns.Add("Price", typeof(decimal));
                        dtWithFinalPrice.Columns.Add("DiscountPercentage", typeof(decimal));
                        dtWithFinalPrice.Columns.Add("FinalPrice", typeof(decimal));

                        foreach (DataRow row in dt.Rows)
                        {
                            decimal price = Convert.ToDecimal(row["Price"]);
                            decimal discount = Convert.ToDecimal(row["DiscountPercentage"]);
                            decimal finalPrice = price - (price * discount / 100);

                            dtWithFinalPrice.Rows.Add(
                                row["ItemID"],
                                row["ItemName"],
                                price,
                                discount,
                                finalPrice
                            );
                        }

                        dgvItems.DataSource = dtWithFinalPrice;

                        // Format the grid
                        dgvItems.Columns["ItemID"].Width = 80;
                        dgvItems.Columns["ItemName"].Width = 250;
                        dgvItems.Columns["Price"].Width = 120;
                        dgvItems.Columns["DiscountPercentage"].Width = 120;
                        dgvItems.Columns["FinalPrice"].Width = 140;

                        UpdateStatusLabel();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchItems(string searchTerm)
        {
            // Don't search if it's empty or placeholder
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Search items...")
            {
                LoadItems(); // Load all items
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SearchItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Calculate final price for each item
                        DataTable dtWithFinalPrice = new DataTable();
                        dtWithFinalPrice.Columns.Add("ItemID", typeof(int));
                        dtWithFinalPrice.Columns.Add("ItemName", typeof(string));
                        dtWithFinalPrice.Columns.Add("Price", typeof(decimal));
                        dtWithFinalPrice.Columns.Add("DiscountPercentage", typeof(decimal));
                        dtWithFinalPrice.Columns.Add("FinalPrice", typeof(decimal));

                        foreach (DataRow row in dt.Rows)
                        {
                            decimal price = Convert.ToDecimal(row["Price"]);
                            decimal discount = Convert.ToDecimal(row["DiscountPercentage"]);
                            decimal finalPrice = price - (price * discount / 100);

                            dtWithFinalPrice.Rows.Add(
                                row["ItemID"],
                                row["ItemName"],
                                price,
                                discount,
                                finalPrice
                            );
                        }

                        dgvItems.DataSource = dtWithFinalPrice;
                        UpdateStatusLabel();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching items: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusLabel()
        {
            int itemCount = dgvItems.Rows.Count;
            lblTitle.Text = $"ITEM MANAGEMENT - {itemCount} Item(s)";
        }

        private void BtnAddNew_Click(object sender, EventArgs e)
        {
            using (ItemDetailForm detailForm = new ItemDetailForm())
            {
                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadItems(); // Refresh the list
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int itemId = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["ItemID"].Value);
            
            using (ItemDetailForm detailForm = new ItemDetailForm(itemId))
            {
                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadItems(); // Refresh the list
                }
            }
        }

        private void DgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int itemId = Convert.ToInt32(dgvItems.Rows[e.RowIndex].Cells["ItemID"].Value);
                
                using (ItemDetailForm detailForm = new ItemDetailForm(itemId))
                {
                    if (detailForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadItems(); // Refresh the list
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int itemId = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["ItemID"].Value);
            string itemName = dgvItems.SelectedRows[0].Cells["ItemName"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete '{itemName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DeleteItem", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ItemID", itemId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Item deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadItems(); // Refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting item: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadItems();
            txtSearch.Text = "Search items...";
            txtSearch.ForeColor = Color.Gray;
            lblSearchHint.Visible = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            searchTimer?.Dispose();
        }
    }

    public class ItemDetailForm : Form
    {
        private TextBox txtItemName;
        private TextBox txtPrice;
        private TextBox txtDiscount;
        private Button btnSave;
        private Button btnCancel;
        private Label lblItemName;
        private Label lblPrice;
        private Label lblDiscount;
        private Panel panel1;
        private int? itemId = null;

        public ItemDetailForm(int? existingItemId = null)
        {
            itemId = existingItemId;
            InitializeComponents();
            if (itemId.HasValue)
                LoadItemData();
        }

        private void InitializeComponents()
        {
            this.Text = itemId.HasValue ? "Edit Item" : "Add New Item";
            this.Size = new Size(450, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title Panel
            panel1 = new Panel();
            panel1.BackColor = Color.FromArgb(41, 128, 185);
            panel1.Dock = DockStyle.Top;
            panel1.Height = 50;

            Label lblTitle = new Label();
            lblTitle.Text = itemId.HasValue ? "EDIT ITEM" : "ADD NEW ITEM";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            panel1.Controls.Add(lblTitle);

            // Form controls
            int yPos = 70;
            int labelWidth = 120;
            int textBoxWidth = 250;

            lblItemName = new Label();
            lblItemName.Text = "Item Name:";
            lblItemName.Font = new Font("Segoe UI", 11);
            lblItemName.Location = new Point(30, yPos);
            lblItemName.Size = new Size(labelWidth, 25);
            lblItemName.TextAlign = ContentAlignment.MiddleRight;

            txtItemName = new TextBox();
            txtItemName.Font = new Font("Segoe UI", 11);
            txtItemName.Location = new Point(160, yPos);
            txtItemName.Size = new Size(textBoxWidth, 30);
            txtItemName.BorderStyle = BorderStyle.FixedSingle;

            yPos += 50;

            lblPrice = new Label();
            lblPrice.Text = "Price (Rs.):";
            lblPrice.Font = new Font("Segoe UI", 11);
            lblPrice.Location = new Point(30, yPos);
            lblPrice.Size = new Size(labelWidth, 25);
            lblPrice.TextAlign = ContentAlignment.MiddleRight;

            txtPrice = new TextBox();
            txtPrice.Font = new Font("Segoe UI", 11);
            txtPrice.Location = new Point(160, yPos);
            txtPrice.Size = new Size(textBoxWidth, 30);
            txtPrice.BorderStyle = BorderStyle.FixedSingle;

            yPos += 50;

            lblDiscount = new Label();
            lblDiscount.Text = "Discount (%):";
            lblDiscount.Font = new Font("Segoe UI", 11);
            lblDiscount.Location = new Point(30, yPos);
            lblDiscount.Size = new Size(labelWidth, 25);
            lblDiscount.TextAlign = ContentAlignment.MiddleRight;

            txtDiscount = new TextBox();
            txtDiscount.Font = new Font("Segoe UI", 11);
            txtDiscount.Location = new Point(160, yPos);
            txtDiscount.Size = new Size(textBoxWidth, 30);
            txtDiscount.BorderStyle = BorderStyle.FixedSingle;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSave.Size = new Size(120, 40);
            btnSave.Location = new Point(160, 220);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Font = new Font("Segoe UI", 11);
            btnCancel.Size = new Size(120, 40);
            btnCancel.Location = new Point(290, 220);
            btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                panel1,
                lblItemName, txtItemName,
                lblPrice, txtPrice,
                lblDiscount, txtDiscount,
                btnSave, btnCancel
            });
        }

        private void LoadItemData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("GetItemByID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemID", itemId.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtItemName.Text = reader["ItemName"].ToString();
                                txtPrice.Text = reader["Price"].ToString();
                                txtDiscount.Text = reader["DiscountPercentage"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading item: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True"))
                {
                    conn.Open();

                    if (itemId.HasValue)
                    {
                        // Update existing item
                        using (SqlCommand cmd = new SqlCommand("UpdateItem", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ItemID", itemId.Value);
                            cmd.Parameters.AddWithValue("@ItemName", txtItemName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                            cmd.Parameters.AddWithValue("@DiscountPercentage", decimal.Parse(txtDiscount.Text));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Add new item
                        using (SqlCommand cmd = new SqlCommand("AddItem", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ItemName", txtItemName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                            cmd.Parameters.AddWithValue("@DiscountPercentage", decimal.Parse(txtDiscount.Text));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                MessageBox.Show("Please enter item name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtItemName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Please enter a valid discount percentage (0-100).", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiscount.Focus();
                return false;
            }

            return true;
        }
    }
}