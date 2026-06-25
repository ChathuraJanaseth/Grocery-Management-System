using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;

namespace GMS
{
    public partial class Billing : Form
    {
        string connString = "Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True";
        DataTable cartItems = new DataTable();
        int lastBillID = 0;
        decimal totalSavings = 0;
        decimal finalTotalAmount = 0;
        decimal receivedAmount = 0;
        decimal balanceAmount = 0;

        // Controls
        TextBox txtSearch, txtReceivedAmount, txtCustomerName, txtCustomerPhone;
        DataGridView dgvItems, dgvCart;
        Label lblTotal, lblSavings, lblBalance;
        Button btnPrint, btnClear, btnBack;
        Panel pnlCenter, pnlBottom, pnlTop, pnlSearch;
        PrintDocument printDoc;
        ComboBox cmbPaymentMethod;

        public Billing()
        {
            this.KeyPreview = true;
            this.Load += Billing_Load;
            this.KeyDown += Billing_KeyDown;
        }

        private void Billing_Load(object sender, EventArgs e)
        {
            InitializeForm();
            SetupForm();
            SetupCart();
            LoadItems();
            txtSearch.Focus();
        }

        private void InitializeForm()
        {
            this.Text = "ABC GROCERY - Professional Billing System";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(230, 230, 230);
            this.Font = new Font("Segoe UI", 9);
        }

        private void SetupForm()
        {
            // Top Panel
            pnlTop = new Panel 
            { 
                Dock = DockStyle.Top, 
                Height = 100, 
                BackColor = Color.FromArgb(13, 71, 161) 
            };
            
            // Back Button
            btnBack = new Button
            {
                Text = "← Back to Main (F2)",
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 15)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += btnBack_Click;
            
            // Main Title
            Label lblTitle = new Label 
            { 
                Text = "ABC GROCERY", 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI", 24, FontStyle.Bold), 
                AutoSize = true,
                Location = new Point(180, 15)
            };
            
            // Tagline
            Label lblTag = new Label 
            { 
                Text = "Fast • Reliable • Best Prices in Town", 
                ForeColor = Color.FromArgb(200, 200, 255), 
                Font = new Font("Segoe UI", 11, FontStyle.Italic), 
                AutoSize = true,
                Location = new Point(185, 60)
            };

            pnlTop.Controls.AddRange(new Control[] { btnBack, lblTitle, lblTag });
            this.Controls.Add(pnlTop);

            // Search Panel
            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            txtSearch = new TextBox 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Text = "Type item name to search...",
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.GotFocus += (s, ev) => 
            { 
                if (txtSearch.Text == "Type item name to search...") 
                {
                    txtSearch.Text = ""; 
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, ev) => 
            { 
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) 
                {
                    txtSearch.Text = "Type item name to search..."; 
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += (s, ev) => LoadItems(txtSearch.Text);

            pnlSearch.Controls.Add(txtSearch);
            this.Controls.Add(pnlSearch);

            // Bottom Panel
            SetupBottomPanel();

            // Center Panel with Grids
            SetupDataGrids();

            // Print Document
            printDoc = new PrintDocument();
            printDoc.PrintPage += printDoc_PrintPage;
        }

        private void SetupDataGrids()
        {
            // Calculate available height for center panel
            int availableHeight = this.ClientSize.Height - pnlTop.Height - pnlSearch.Height - pnlBottom.Height;
            
            // Center Panel
            pnlCenter = new Panel
            {
                Location = new Point(0, pnlTop.Height + pnlSearch.Height),
                Size = new Size(this.ClientSize.Width, availableHeight),
                BackColor = Color.White,
                Padding = new Padding(15)
            };
            this.Controls.Add(pnlCenter);

            // Split container for better layout
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SplitterDistance = pnlCenter.Width / 2 - 10,
                SplitterWidth = 8,
                Panel1 = { BackColor = Color.Transparent },
                Panel2 = { BackColor = Color.Transparent }
            };

            // Left Panel for Items
            Panel pnlLeft = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Items Grid
            dgvItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            SetupItemsGridStyle();
            pnlLeft.Controls.Add(dgvItems);

            // Right Panel for Cart
            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Cart Grid
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            SetupCartGridStyle();
            pnlRight.Controls.Add(dgvCart);

            splitContainer.Panel1.Controls.Add(pnlLeft);
            splitContainer.Panel2.Controls.Add(pnlRight);
            pnlCenter.Controls.Add(splitContainer);
        }

        private void SetupItemsGridStyle()
        {
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.ReadOnly = true;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.RowHeadersVisible = false;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.FixedSingle;
            dgvItems.GridColor = Color.LightGray;
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ScrollBars = ScrollBars.Vertical;

            // Header style
            dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(3)
            };

            // Row style
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvItems.DefaultCellStyle.Padding = new Padding(3);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Set row height for better visibility
            dgvItems.RowTemplate.Height = 35;

            dgvItems.CellContentClick += dgvItems_CellContentClick;
        }

        private void SetupCartGridStyle()
        {
            dgvCart.AllowUserToAddRows = false;
            dgvCart.AllowUserToDeleteRows = false;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.MultiSelect = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.FixedSingle;
            dgvCart.GridColor = Color.LightGray;
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ScrollBars = ScrollBars.Vertical;

            // Header style
            dgvCart.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(3)
            };

            // Row style
            dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 249);
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvCart.DefaultCellStyle.Padding = new Padding(3);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 255, 220);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Set row height for better visibility
            dgvCart.RowTemplate.Height = 35;

            dgvCart.CellContentClick += dgvCart_CellContentClick;
            dgvCart.CellValueChanged += dgvCart_CellValueChanged;
            dgvCart.CellBeginEdit += (s, e) => 
            {
                // Only allow editing of Qty column
                if (e.ColumnIndex != dgvCart.Columns["Qty"].Index)
                    e.Cancel = true;
            };
        }

        private void SetupBottomPanel()
        {
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 180,
                BackColor = Color.FromArgb(34, 34, 34),
                Padding = new Padding(25)
            };

            // Clear Cart Button
            btnClear = new Button
            {
                Text = "🔄 New Customer\r\n(Esc)",
                Size = new Size(140, 80),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 20)
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, ev) => { 
                ClearAll();
            };

            // Customer Information Panel
            Panel pnlCustomerInfo = new Panel
            {
                Size = new Size(250, 140),
                BackColor = Color.FromArgb(50, 50, 50),
                Location = new Point(180, 15)
            };

            // Customer Name
            Label lblCustomerName = new Label
            {
                Text = "Customer Name:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            txtCustomerName = new TextBox
            {
                Size = new Size(230, 25),
                Location = new Point(10, 30),
                Font = new Font("Segoe UI", 9),
                Text = "Walk-in Customer"
            };

            // Customer Phone
            Label lblCustomerPhone = new Label
            {
                Text = "Phone Number:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(10, 60)
            };

            txtCustomerPhone = new TextBox
            {
                Size = new Size(230, 25),
                Location = new Point(10, 80),
                Font = new Font("Segoe UI", 9),
                Text = "Not Provided"
            };

            pnlCustomerInfo.Controls.AddRange(new Control[] { lblCustomerName, txtCustomerName, lblCustomerPhone, txtCustomerPhone });

            // Payment Information Panel
            Panel pnlPayment = new Panel
            {
                Size = new Size(300, 140),
                BackColor = Color.FromArgb(40, 40, 40),
                Location = new Point(450, 15)
            };

            // Total Amount
            Label lblTotalText = new Label
            {
                Text = "TOTAL AMOUNT",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(20, 10)
            };

            lblTotal = new Label
            {
                Text = "Rs. 0.00",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                AutoSize = true,
                Location = new Point(20, 35)
            };

            // Received Amount
            Label lblReceivedText = new Label
            {
                Text = "RECEIVED AMOUNT",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(20, 65)
            };

            txtReceivedAmount = new TextBox
            {
                Size = new Size(120, 25),
                Location = new Point(20, 85),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "0.00",
                TextAlign = HorizontalAlignment.Right
            };
            txtReceivedAmount.TextChanged += txtReceivedAmount_TextChanged;
            txtReceivedAmount.KeyPress += txtReceivedAmount_KeyPress;

            // Balance Amount
            Label lblBalanceText = new Label
            {
                Text = "BALANCE",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(150, 65)
            };

            lblBalance = new Label
            {
                Text = "Rs. 0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(150, 85)
            };

            // Payment Method
            Label lblPaymentMethod = new Label
            {
                Text = "PAYMENT METHOD",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(20, 115)
            };

            cmbPaymentMethod = new ComboBox
            {
                Size = new Size(120, 25),
                Location = new Point(20, 135),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPaymentMethod.Items.AddRange(new object[] { "CASH", "CARD", "DIGITAL PAYMENT" });
            cmbPaymentMethod.SelectedIndex = 0;

            pnlPayment.Controls.AddRange(new Control[] { 
                lblTotalText, lblTotal, lblReceivedText, txtReceivedAmount, 
                lblBalanceText, lblBalance, lblPaymentMethod, cmbPaymentMethod 
            });

            // Print Button
            btnPrint = new Button
            {
                Text = "🖨️ PRINT BILL\r\n(F10)",
                Size = new Size(150, 80),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(770, 20)
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += btnPrint_Click;

            // Savings Label
            lblSavings = new Label
            {
                Text = "You Save: Rs. 0.00",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(770, 110)
            };

            pnlBottom.Controls.AddRange(new Control[] { 
                btnClear, pnlCustomerInfo, pnlPayment, btnPrint, lblSavings 
            });
            this.Controls.Add(pnlBottom);
        }

        private void ClearAll()
        {
            cartItems.Clear();
            txtCustomerName.Text = "Walk-in Customer";
            txtCustomerPhone.Text = "Not Provided";
            txtReceivedAmount.Text = "0.00";
            cmbPaymentMethod.SelectedIndex = 0;
            CalculateTotal();
            CalculateSavings();
            CalculateBalance();
        }

        private void txtReceivedAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void txtReceivedAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, decimal point, and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void CalculateBalance()
        {
            if (decimal.TryParse(txtReceivedAmount.Text, out receivedAmount))
            {
                balanceAmount = receivedAmount - finalTotalAmount;
                lblBalance.Text = "Rs. " + balanceAmount.ToString("N2");
                
                // Change color based on balance
                if (balanceAmount >= 0)
                {
                    lblBalance.ForeColor = Color.FromArgb(46, 204, 113); // Green
                }
                else
                {
                    lblBalance.ForeColor = Color.FromArgb(220, 53, 69); // Red
                }
            }
            else
            {
                lblBalance.Text = "Rs. 0.00";
                lblBalance.ForeColor = Color.FromArgb(255, 193, 7); // Yellow
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if (pnlCenter != null && pnlTop != null && pnlSearch != null && pnlBottom != null)
            {
                int availableHeight = this.ClientSize.Height - pnlTop.Height - pnlSearch.Height - pnlBottom.Height;
                pnlCenter.Location = new Point(0, pnlTop.Height + pnlSearch.Height);
                pnlCenter.Size = new Size(this.ClientSize.Width, availableHeight);
            }
        }

        private void SetupCart()
        {
            if (cartItems.Columns.Count == 0)
            {
                cartItems.Columns.Add("ItemID", typeof(int));
                cartItems.Columns.Add("ItemName", typeof(string));
                cartItems.Columns.Add("Qty", typeof(int));
                cartItems.Columns.Add("Price", typeof(decimal));
                cartItems.Columns.Add("Discount", typeof(decimal));
                cartItems.Columns.Add("SubTotal", typeof(decimal));
                cartItems.Columns.Add("OriginalTotal", typeof(decimal)); // For savings calculation
            }
            else
            {
                cartItems.Rows.Clear();
            }

            dgvCart.DataSource = cartItems;
            FormatCartGrid();
        }

        private void FormatCartGrid()
        {
            dgvCart.Columns.Clear();

            if (cartItems.Columns.Count > 0)
            {
                // Add columns with proper settings
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "ItemID",
                    DataPropertyName = "ItemID",
                    Visible = false
                });

                dgvCart.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "ItemName",
                    HeaderText = "PRODUCT NAME",
                    DataPropertyName = "ItemName",
                    Width = 250,
                    ReadOnly = true
                });

                DataGridViewTextBoxColumn qtyCol = new DataGridViewTextBoxColumn 
                { 
                    Name = "Qty",
                    HeaderText = "QTY",
                    DataPropertyName = "Qty",
                    Width = 60
                };
                qtyCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCart.Columns.Add(qtyCol);

                DataGridViewTextBoxColumn priceCol = new DataGridViewTextBoxColumn 
                { 
                    Name = "Price",
                    HeaderText = "PRICE (Rs.)",
                    DataPropertyName = "Price",
                    Width = 90,
                    Visible = false // Hidden as per requirement
                };
                priceCol.DefaultCellStyle.Format = "N2";
                priceCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCart.Columns.Add(priceCol);

                DataGridViewTextBoxColumn discountCol = new DataGridViewTextBoxColumn 
                { 
                    Name = "Discount",
                    HeaderText = "DISCOUNT (Rs.)",
                    DataPropertyName = "Discount",
                    Width = 90,
                    Visible = false // Hidden as per requirement
                };
                discountCol.DefaultCellStyle.Format = "N2";
                discountCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCart.Columns.Add(discountCol);

                DataGridViewTextBoxColumn subTotalCol = new DataGridViewTextBoxColumn 
                { 
                    Name = "SubTotal",
                    HeaderText = "SUB TOTAL (Rs.)",
                    DataPropertyName = "SubTotal",
                    Width = 100,
                    Visible = false // Hidden as per requirement
                };
                subTotalCol.DefaultCellStyle.Format = "N2";
                subTotalCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCart.Columns.Add(subTotalCol);

                // Original Total column (hidden - for savings calculation)
                dgvCart.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "OriginalTotal",
                    DataPropertyName = "OriginalTotal",
                    Visible = false
                });

                // Make Qty column editable
                dgvCart.Columns["Qty"].ReadOnly = false;

                // Add Remove button column
                DataGridViewButtonColumn removeBtn = new DataGridViewButtonColumn
                {
                    HeaderText = "ACTION",
                    Text = "🗑️ Remove",
                    UseColumnTextForButtonValue = true,
                    Name = "RemoveBtn",
                    Width = 90,
                    FlatStyle = FlatStyle.Flat
                };
                removeBtn.DefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69);
                removeBtn.DefaultCellStyle.ForeColor = Color.White;
                removeBtn.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                removeBtn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCart.Columns.Add(removeBtn);
            }
        }

        private void LoadItems(string search = "")
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    
                    string query = @"
SELECT [ItemID]
      ,[ItemName]
      ,[Price]
      ,[DiscountPercentage]
FROM [GroceryDB].[dbo].[Items]";
                    
                    if (!string.IsNullOrEmpty(search) && search != "Type item name to search...")
                    {
                        query += " WHERE [ItemName] LIKE @SearchTerm";
                    }

                    query += " ORDER BY [ItemName]";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    if (!string.IsNullOrEmpty(search) && search != "Type item name to search...")
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + search + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    
                    int itemCount = dt.Rows.Count;
                    
                    if (itemCount == 0 && !string.IsNullOrEmpty(search) && search != "Type item name to search...")
                    {
                        MessageBox.Show("No items found matching your search.", "No Results", 
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Bind data to grid
                    dgvItems.DataSource = dt;
                    FormatItemsGrid();

                    // Ensure first row is visible
                    if (dgvItems.Rows.Count > 0)
                    {
                        dgvItems.Rows[0].Selected = true;
                        dgvItems.FirstDisplayedScrollingRowIndex = 0;
                    }

                }
                catch (SqlException sqlEx)
                {
                    string errorMessage = $"Database Error: {sqlEx.Message}\n\n";
                    errorMessage += "Please check:\n";
                    errorMessage += "1. SQL Server is running\n";
                    errorMessage += "2. Database 'GroceryDB' exists\n";
                    errorMessage += "3. Table 'Items' exists\n";
                    errorMessage += "4. Connection string is correct";

                    MessageBox.Show(errorMessage, "Database Connection Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormatItemsGrid()
        {
            dgvItems.Columns.Clear();

            if (dgvItems.DataSource != null)
            {
                // Add columns in correct order - ALL VISIBLE as requested
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "ItemID",
                    DataPropertyName = "ItemID",
                    HeaderText = "ITEM ID",
                    Width = 80,
                    ReadOnly = true
                });

                dgvItems.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "ItemName",
                    HeaderText = "PRODUCT NAME",
                    DataPropertyName = "ItemName",
                    Width = 250,
                    ReadOnly = true
                });

                // Price column - VISIBLE as requested (showing all columns)
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "Price",
                    HeaderText = "PRICE (Rs.)",
                    DataPropertyName = "Price",
                    Width = 100,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                // Discount column - VISIBLE as requested (showing all columns)
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    Name = "DiscountPercentage",
                    HeaderText = "DISCOUNT (%)",
                    DataPropertyName = "DiscountPercentage",
                    Width = 90,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "N1",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                // Final Price column (calculated) - VISIBLE as requested
                DataGridViewTextBoxColumn finalPriceCol = new DataGridViewTextBoxColumn
                {
                    Name = "FinalPrice",
                    HeaderText = "FINAL PRICE (Rs.)",
                    Width = 110,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                };
                dgvItems.Columns.Add(finalPriceCol);

                // Calculate final prices
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (row.Cells["Price"].Value != null && row.Cells["DiscountPercentage"].Value != null)
                    {
                        decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                        decimal discountPct = Convert.ToDecimal(row.Cells["DiscountPercentage"].Value);
                        decimal finalPrice = price - (price * discountPct / 100);
                        row.Cells["FinalPrice"].Value = finalPrice;
                    }
                }

                // Add "Add to Cart" button column
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn
                {
                    HeaderText = "ACTION",
                    Text = "➕ Add to Cart",
                    UseColumnTextForButtonValue = true,
                    Name = "AddBtn",
                    Width = 120,
                    FlatStyle = FlatStyle.Flat
                };
                btn.DefaultCellStyle.BackColor = Color.FromArgb(40, 167, 69);
                btn.DefaultCellStyle.ForeColor = Color.White;
                btn.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                btn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvItems.Columns.Add(btn);
            }
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvItems.Columns[e.ColumnIndex].Name == "AddBtn")
            {
                DataGridViewRow row = dgvItems.Rows[e.RowIndex];
                int id = Convert.ToInt32(row.Cells["ItemID"].Value);
                string name = row.Cells["ItemName"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                decimal discPct = Convert.ToDecimal(row.Cells["DiscountPercentage"].Value);
                decimal discAmount = price * discPct / 100;
                decimal originalTotal = price; // Original price without discount

                // Check if item already exists in cart
                foreach (DataRow cartRow in cartItems.Rows)
                {
                    if ((int)cartRow["ItemID"] == id)
                    {
                        int currentQty = (int)cartRow["Qty"];
                        cartRow["Qty"] = currentQty + 1;
                        cartRow["SubTotal"] = (price - discAmount) * (currentQty + 1);
                        cartRow["OriginalTotal"] = originalTotal * (currentQty + 1);
                        
                        CalculateTotal();
                        CalculateSavings();
                        dgvCart.Refresh();
                        return;
                    }
                }

                // Add new item to cart
                DataRow dr = cartItems.NewRow();
                dr["ItemID"] = id;
                dr["ItemName"] = name;
                dr["Qty"] = 1;
                dr["Price"] = price;
                dr["Discount"] = discAmount;
                dr["SubTotal"] = price - discAmount;
                dr["OriginalTotal"] = originalTotal; // Original price for savings calculation
                cartItems.Rows.Add(dr);
                
                CalculateTotal();
                CalculateSavings();
                dgvCart.Refresh();
            }
        }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCart.Columns[e.ColumnIndex].Name == "RemoveBtn")
            {
                cartItems.Rows.RemoveAt(e.RowIndex);
                CalculateTotal();
                CalculateSavings();
                dgvCart.Refresh();
            }
        }

        private void dgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCart.Columns["Qty"].Index)
            {
                var row = dgvCart.Rows[e.RowIndex];
                if (row.Cells["Qty"].Value != null && row.Cells["Price"].Value != null && row.Cells["Discount"].Value != null)
                {
                    int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                    if (qty < 1) 
                    {
                        qty = 1;
                        row.Cells["Qty"].Value = 1;
                    }
                    
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    decimal disc = Convert.ToDecimal(row.Cells["Discount"].Value);
                    decimal originalPrice = price + disc; // Calculate original price per item
                    
                    row.Cells["SubTotal"].Value = (price - disc) * qty;
                    row.Cells["OriginalTotal"].Value = originalPrice * qty;
                    
                    CalculateTotal();
                    CalculateSavings();
                }
            }
        }

        private void CalculateTotal()
        {
            finalTotalAmount = 0;
            foreach (DataRow r in cartItems.Rows)
                finalTotalAmount += Convert.ToDecimal(r["SubTotal"]);
            lblTotal.Text = "Rs. " + finalTotalAmount.ToString("N2");
            
            // Auto-set received amount to total for convenience
            txtReceivedAmount.Text = finalTotalAmount.ToString("N2");
            CalculateBalance();
        }

        private void CalculateSavings()
        {
            totalSavings = 0;
            decimal originalTotal = 0;
            decimal discountedTotal = 0;

            foreach (DataRow r in cartItems.Rows)
            {
                originalTotal += Convert.ToDecimal(r["OriginalTotal"]);
                discountedTotal += Convert.ToDecimal(r["SubTotal"]);
            }

            totalSavings = originalTotal - discountedTotal;
            lblSavings.Text = $"You Save: Rs. {totalSavings:N2}";
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (cartItems.Rows.Count == 0) 
            { 
                MessageBox.Show("Cart is empty! Add items before printing.", "Empty Cart", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return; 
            }

            if (receivedAmount < finalTotalAmount)
            {
                DialogResult result = MessageBox.Show(
                    $"Received amount (Rs. {receivedAmount:N2}) is less than total amount (Rs. {finalTotalAmount:N2}).\nDo you want to continue printing?",
                    "Insufficient Payment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;
            }

            try
            {
                lastBillID = SaveBillToDatabase(finalTotalAmount, totalSavings, receivedAmount, balanceAmount);
                
                // Show print preview
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = printDoc;
                previewDialog.WindowState = FormWindowState.Maximized;
                previewDialog.Text = $"ABC GROCERY - Bill #{lastBillID}";
                
                previewDialog.ShowDialog();

                // Clear cart after successful print preview
                ClearAll();
                LoadItems();
                
                MessageBox.Show($"Bill #{lastBillID} saved successfully!\nTotal Savings: Rs. {totalSavings:N2}", "Success", 
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing bill: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int SaveBillToDatabase(decimal total, decimal savings, decimal received, decimal balance)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Save to Bills table with payment details
                    SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Bills (TotalAmount, TotalSavings, ReceivedAmount, BalanceAmount, 
                                           PaymentMethod, CustomerName, CustomerPhone, BillDate) 
                          OUTPUT INSERTED.BillID 
                          VALUES (@t, @s, @r, @b, @pm, @cn, @cp, @d)", 
                        conn, tran);
                    cmd.Parameters.AddWithValue("@t", total);
                    cmd.Parameters.AddWithValue("@s", savings);
                    cmd.Parameters.AddWithValue("@r", received);
                    cmd.Parameters.AddWithValue("@b", balance);
                    cmd.Parameters.AddWithValue("@pm", cmbPaymentMethod.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@cn", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@cp", txtCustomerPhone.Text);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now);
                    int billID = (int)cmd.ExecuteScalar();

                    // Save bill details
                    foreach (DataRow r in cartItems.Rows)
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO BillDetails (BillID, ItemID, Quantity, Price, Discount, SubTotal) VALUES (@bid, @iid, @q, @p, @d, @s)", 
                            conn, tran);
                        cmd.Parameters.AddWithValue("@bid", billID);
                        cmd.Parameters.AddWithValue("@iid", r["ItemID"]);
                        cmd.Parameters.AddWithValue("@q", r["Qty"]);
                        cmd.Parameters.AddWithValue("@p", r["Price"]);
                        cmd.Parameters.AddWithValue("@d", r["Discount"]);
                        cmd.Parameters.AddWithValue("@s", r["SubTotal"]);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return billID;
                }
                catch (Exception ex)
                { 
                    tran.Rollback(); 
                    throw new Exception($"Database save error: {ex.Message}");
                }
            }
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font headerFont = new Font("Segoe UI", 15, FontStyle.Bold);
            Font subHeaderFont = new Font("Segoe UI", 8, FontStyle.Italic);
            Font normalFont = new Font("Segoe UI", 7);
            Font boldFont = new Font("Segoe UI", 7, FontStyle.Bold);
            Font totalFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font savingsFont = new Font("Segoe UI", 8, FontStyle.Bold);
            Font itemHeaderFont = new Font("Segoe UI", 7, FontStyle.Bold);
            Font paymentFont = new Font("Segoe UI", 8, FontStyle.Bold);

            float y = 5;
            float leftMargin = 0;
            float width = e.PageBounds.Width - 40;

            try
            {
                // Load and draw logo from PNG file
                Image logo = LoadLogoFromPNG();
                if (logo != null)
                {
                    float logoHeight = 60;
                    float logoWidth = logo.Width * (logoHeight / logo.Height);
                    if (logoWidth > 150) logoWidth = 150;
                    g.DrawImage(logo, leftMargin, y, logoWidth, logoHeight);
                }

                // Header Section with Logo
                float headerX = logo != null ? leftMargin + 70 : leftMargin;
                
                g.DrawString("ABC GROCERY", headerFont, Brushes.Black, headerX, y);
                y += 30;
                g.DrawString("Fast • Reliable • Best Prices in Town", subHeaderFont, Brushes.Black, headerX, y);
                y += 45;

                // Store Info
                g.DrawString("📍 123 Main Street, Colombo 05", normalFont, Brushes.Black, headerX, y);
                y += 15;
                g.DrawString("📞 Tel: 011-1234567, 077-1234567", normalFont, Brushes.Black, headerX, y);
                y += 15;
                g.DrawString("📧 Email: info@abcgrocery.lk", normalFont, Brushes.Black, headerX, y);
                y += 15;
                g.DrawString("🕒 Open: 7:00 AM - 10:00 PM (Daily)", normalFont, Brushes.Black, headerX, y);
                y += 25;

                // Business Registration Info
                g.DrawString("Business Reg No: AB123456789", normalFont, Brushes.Black, headerX, y);
                y += 15;
                g.DrawString("GST No: GST123456789", normalFont, Brushes.Black, headerX, y);
                y += 25;

                // Bill Info Section
                g.DrawString($"Bill Number: {lastBillID:0000}", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString($"Date: {DateTime.Now:dd/MM/yyyy}", boldFont, Brushes.Black, width - 80, y);
                y += 15;
                g.DrawString($"Time: {DateTime.Now:hh:mm tt}", boldFont, Brushes.Black, width - 80, y);
                y += 25;

                // Customer Information
                g.DrawString($"Customer: {txtCustomerName.Text}", boldFont, Brushes.Black, leftMargin, y);
                y += 15;
                g.DrawString($"Phone: {txtCustomerPhone.Text}", boldFont, Brushes.Black, leftMargin, y);
                y += 15;
                g.DrawString($"Cashier: System User", boldFont, Brushes.Black, leftMargin, y);
                y += 25;

                // Line separator
                using (Pen blackPen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(blackPen, leftMargin, y, width * 2 + leftMargin, y);
                }
                y += 10;

                // Column headers with better spacing
                float[] columnWidths = { 100, 40, 50, 50, 60 }; // Item, Qty, Price, Disc%, Disc Amt, Total
                float x = leftMargin;

                g.DrawString("Item", itemHeaderFont, Brushes.Black, x, y);
                x += columnWidths[0];
                g.DrawString("Qty", itemHeaderFont, Brushes.Black, x, y);
                x += columnWidths[1];
                g.DrawString("U.Price", itemHeaderFont, Brushes.Black, x, y);
                x += columnWidths[2];
                g.DrawString("Disc %", itemHeaderFont, Brushes.Black, x, y);
                x += columnWidths[3];
                //g.DrawString("Disc Amt", itemHeaderFont, Brushes.Black, x, y);
                //x += columnWidths[4];
                g.DrawString("Amount", itemHeaderFont, Brushes.Black, x, y);
                
                y += 25;
                
                // Line under headers
                using (Pen linePen = new Pen(Color.Black, 1))
                {
                    g.DrawLine(linePen, leftMargin, y, width * 2 + leftMargin, y);
                }
                y += 10;

                // Items List
                decimal finalTotal = 0;
                decimal totalDiscount = 0;
                int totalItems = 0;

                foreach (DataRow r in cartItems.Rows)
                {
                    string itemName = r["ItemName"].ToString();
                    int qty = Convert.ToInt32(r["Qty"]);
                    decimal price = Convert.ToDecimal(r["Price"]);
                    decimal discount = Convert.ToDecimal(r["Discount"]);
                    decimal subTotal = Convert.ToDecimal(r["SubTotal"]);
                    decimal discountPercentage = (discount / price) * 100;
                    
                    finalTotal += subTotal;
                    totalDiscount += discount * qty;
                    totalItems += qty;

                    // Wrap item name if too long
                    x = leftMargin;
                    string displayName = itemName;
                    if (itemName.Length > 20) 
                        displayName = itemName.Substring(0, 20) + "...";
                    
                    g.DrawString(displayName, normalFont, Brushes.Black, x, y);
                    x += columnWidths[0];
                    g.DrawString(qty.ToString(), normalFont, Brushes.Black, x, y);
                    x += columnWidths[1];
                    g.DrawString(price.ToString("N2"), normalFont, Brushes.Black, x, y);
                    x += columnWidths[2];
                    g.DrawString(discountPercentage.ToString("N1") + "%", normalFont, Brushes.Black, x, y);
                    x += columnWidths[3];
                    //g.DrawString((discount * qty).ToString("N2"), normalFont, Brushes.Black, x, y);
                    //x += columnWidths[4];
                    g.DrawString(subTotal.ToString("N2"), normalFont, Brushes.Black, x, y);
                    
                    y += 18;

                    // If item name was truncated, show full name on next line
                    if (itemName.Length > 20)
                    {
                        g.DrawString("  " + itemName, new Font("Segoe UI", 8), Brushes.Gray, leftMargin, y);
                        y += 15;
                    }

                    // Check if we need another page
                    if (y > e.PageBounds.Height - 200)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }

                y += 10;
                
                // Bottom line
                using (Pen bottomPen = new Pen(Color.Black, 2))
                {
                    g.DrawLine(bottomPen, leftMargin, y, width * 2 + leftMargin, y);
                }
                y += 15;

                // Summary Section - LINE BY LINE FORMAT
                x = leftMargin + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4];
                
                // Total Items
                g.DrawString("Total Items:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString(totalItems.ToString(), boldFont, Brushes.Black, width - 80, y);
                y += 20;

                // Subtotal (Original total without discount)
                decimal subtotalAmount = finalTotal + totalDiscount;
                g.DrawString("Sub Total:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString(subtotalAmount.ToString("N2"), boldFont, Brushes.Black, width - 80, y);
                y += 20;

                // Total Discount
                g.DrawString("Total Discount:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString("-" + totalDiscount.ToString("N2"), boldFont, Brushes.Green, width - 80, y);
                y += 20;

                // Final Total
                g.DrawString("TOTAL AMOUNT:", totalFont, Brushes.Black, leftMargin, y);
                g.DrawString(finalTotal.ToString("N2"), totalFont, Brushes.Black, width - 80, y);
                y += 35;

                // Payment Details Section
                g.DrawString("PAYMENT DETAILS", paymentFont, Brushes.Black, leftMargin, y);
                y += 20;

                // Payment Method
                g.DrawString("Payment Method:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString(cmbPaymentMethod.SelectedItem.ToString(), boldFont, Brushes.Black, leftMargin + 120, y);
                y += 20;

                // Received Amount
                g.DrawString("Received Amount:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString(receivedAmount.ToString("N2"), boldFont, Brushes.Black, leftMargin + 120, y);
                y += 20;

                // Balance Amount
                g.DrawString("Balance Amount:", boldFont, Brushes.Black, leftMargin, y);
                g.DrawString(balanceAmount.ToString("N2"), boldFont, 
                            balanceAmount >= 0 ? Brushes.Green : Brushes.Red, 
                            leftMargin + 120, y);
                y += 25;

              /*  // Savings Section - PROMINENTLY DISPLAYED IN LINE BY LINE
                g.DrawString("TOTAL SAVINGS:", savingsFont, Brushes.Green, x - 180, y);
                g.DrawString(totalSavings.ToString("N2"), savingsFont, Brushes.Green, x, y);
                y += 25;

                // Savings Percentage
                decimal savingsPercentage = subtotalAmount > 0 ? (totalSavings / subtotalAmount) * 100 : 0;
                g.DrawString("YOU SAVED:", new Font("Segoe UI", 10, FontStyle.Bold), Brushes.Green, x - 180, y);
                g.DrawString($"{savingsPercentage:N1}%", new Font("Segoe UI", 10, FontStyle.Bold), Brushes.Green, x, y);
                y += 30; */

                // Thank You Message
                g.DrawString("Thank you for shopping with us!", new Font("Segoe UI", 11, FontStyle.Bold), 
                            Brushes.Black, width / 2 - 100, y);
                y += 20;
                g.DrawString("Please visit again!", new Font("Segoe UI", 10, FontStyle.Italic), 
                            Brushes.Black, width / 2 - 50, y);
                y += 20;

                y += 12;
                g.DrawString($"Printed on: {DateTime.Now:dd/MM/yyyy hh:mm tt}", new Font("Segoe UI", 8), 
                            Brushes.Gray, width / 2 - 70, y);

                y += 20;
                // Footer
                g.DrawString("@Powerd by CJ", new Font("Segoe UI", 8),
                            Brushes.Gray, width / 2 - 20, y);

            }
            catch (Exception ex)
            {
                // If any error occurs during printing, show simple bill
                g.DrawString("Error generating detailed bill. Printing basic bill...", 
                            new Font("Segoe UI", 10, FontStyle.Bold), Brushes.Red, leftMargin, y);
                y += 30;
                g.DrawString($"Bill #: {lastBillID}", boldFont, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Total: Rs. {finalTotalAmount:N2}", boldFont, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Received: Rs. {receivedAmount:N2}", boldFont, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Balance: Rs. {balanceAmount:N2}", boldFont, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Total Savings: Rs. {totalSavings:N2}", boldFont, Brushes.Green, leftMargin, y);
            }
        }

        private Image LoadLogoFromPNG()
        {
            try
            {
                // Try to load PNG logo from various possible locations
                string[] possiblePaths = {
                    @"C:\ABC_Grocery_Logo.png",
                    @"D:\ABC_Grocery_Logo.png",
                    @"E:\ABC_Grocery_Logo.png",
                    Path.Combine(Application.StartupPath, "ABC_Grocery_Logo.png"),
                    Path.Combine(Application.StartupPath, "Resources", "ABC_Grocery_Logo.png"),
                    Path.Combine(Application.StartupPath, "Images", "ABC_Grocery_Logo.png"),
                    Path.Combine(Application.StartupPath, "Assets", "ABC_Grocery_Logo.png"),
                    Path.Combine(Application.StartupPath, "Logo", "ABC_Grocery_Logo.png"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ABC_Grocery_Logo.png"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ABC_Grocery_Logo.png"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ABC_Grocery_Logo.png"),
                    // Add your specific logo path here
                    @"C:\Users\janas\Downloads\Softwares\GMS\Resources\ABC_Grocery_Logo.png" // Replace with your actual logo path
                };

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            // Load the PNG image
                            Image logo = Image.FromFile(path);
                            return logo;
                        }
                        catch
                        {
                            // If loading fails, continue to next path
                            continue;
                        }
                    }
                }

                // If no PNG logo found, create a simple one programmatically
                return CreateSimpleLogo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logo loading error: {ex.Message}\nA default logo will be used.", "Logo Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return CreateSimpleLogo();
            }
        }

        private Image CreateSimpleLogo()
        {
            // Create a simple logo programmatically as fallback
            Bitmap logo = new Bitmap(150, 60);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.White);

                // Draw ABC Grocery text
                using (Font logoFont = new Font("Arial", 14, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(13, 71, 161)))
                {
                    g.DrawString("ABC", logoFont, textBrush, 10, 10);
                    g.DrawString("GROCERY", new Font("Arial", 10, FontStyle.Bold), textBrush, 10, 35);
                }

                // Draw a simple shopping cart icon
                using (Pen greenPen = new Pen(Color.FromArgb(46, 204, 113), 2))
                {
                    g.DrawRectangle(greenPen, 100, 15, 30, 20);
                    g.DrawEllipse(greenPen, 105, 35, 8, 8);
                    g.DrawEllipse(greenPen, 120, 35, 8, 8);
                }

                // Fill the rectangle
                g.FillRectangle(Brushes.LightGreen, 101, 16, 29, 19);
            }
            return logo;
        }

        // Navigate back to MainForm with session check
        private void btnBack_Click(object sender, EventArgs e)
        {
            ReturnToMainForm();
        }

        private void ReturnToMainForm()
        {
            MessageBox.Show("Session expired. Please login again.", "Session Expired",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }

        private void Billing_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter when dgvItems.Focused && dgvItems.CurrentRow != null:
                    dgvItems_CellContentClick(dgvItems, new DataGridViewCellEventArgs(dgvItems.Columns["AddBtn"].Index, dgvItems.CurrentRow.Index));
                    break;
                    
                case Keys.Delete when dgvCart.Focused && dgvCart.CurrentRow != null:
                    dgvCart_CellContentClick(dgvCart, new DataGridViewCellEventArgs(dgvCart.Columns["RemoveBtn"].Index, dgvCart.CurrentRow.Index));
                    break;
                    
                case Keys.F10:
                    btnPrint.PerformClick();
                    break;
                    
                case Keys.Escape:
                    ClearAll();
                    break;
                    
                case Keys.F2:
                    ReturnToMainForm();
                    break;

                case Keys.F5:
                    // Auto-calculate received amount as total
                    txtReceivedAmount.Text = finalTotalAmount.ToString("N2");
                    break;
            }
        }
    }
}