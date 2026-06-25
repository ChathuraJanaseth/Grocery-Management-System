using System;
using System.Drawing;
using System.Windows.Forms;

namespace GMS
{
    public partial class MainForm : Form
    {
        private Button btnBilling, btnItemManagement, btnSalesReport, btnLogout, btnExit;
        private Label lblTitle, lblWelcome, lblUserInfo;
        private Panel panelMain;
        
        public MainForm()
        {
            SetupForm();
            DisplayUserInfo();
        }
        
        private void SetupForm()
        {
            // Form Properties
            this.Text = "ABC GROCERY - Main Menu";
            this.Size = new Size(730, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 230, 230);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.FormClosing += MainForm_FormClosing;
            
            // Main Panel
            panelMain = new Panel();
            panelMain.Dock = DockStyle.Fill;
            panelMain.BackColor = Color.White;
            //panelMain.Padding = new Padding(20);
            
            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 120;
            headerPanel.BackColor = Color.FromArgb(13, 71, 161);
            headerPanel.Padding = new Padding(20);
            
            // Title
            lblTitle = new Label();
            lblTitle.Text = "ABC GROCERY";
            lblTitle.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            
            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Text = "Professional Billing System";
            lblWelcome.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblWelcome.ForeColor = Color.FromArgb(200, 200, 255);
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(23, 70);
            
            // User Info Label
            lblUserInfo = new Label();
            lblUserInfo.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblUserInfo.ForeColor = Color.LightYellow;
            lblUserInfo.AutoSize = true;
            lblUserInfo.Location = new Point(500, 70);
            
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblUserInfo);
            
            // Content Panel
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.FromArgb(230, 230, 230);
            contentPanel.Padding = new Padding(50, 50, 50, 50);
            
            // Billing Button
            btnBilling = new Button();
            btnBilling.Text = "💰 BILLING SYSTEM\n(F1)";
            btnBilling.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnBilling.Size = new Size(300, 120);
            btnBilling.Location = new Point(200, 50);
            btnBilling.BackColor = Color.FromArgb(40, 167, 69);
            btnBilling.ForeColor = Color.White;
            btnBilling.FlatStyle = FlatStyle.Flat;
            btnBilling.FlatAppearance.BorderSize = 0;
            btnBilling.Cursor = Cursors.Hand;
            btnBilling.TextAlign = ContentAlignment.MiddleCenter;
            btnBilling.Click += BtnBilling_Click;
            
            // Item Management Button
            btnItemManagement = new Button();
            btnItemManagement.Text = "📦 ITEM MANAGEMENT\n(F3)";
            btnItemManagement.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnItemManagement.Size = new Size(300, 120);
            btnItemManagement.Location = new Point(200, 200);
            btnItemManagement.BackColor = Color.FromArgb(52, 152, 219);
            btnItemManagement.ForeColor = Color.White;
            btnItemManagement.FlatStyle = FlatStyle.Flat;
            btnItemManagement.FlatAppearance.BorderSize = 0;
            btnItemManagement.Cursor = Cursors.Hand;
            btnItemManagement.TextAlign = ContentAlignment.MiddleCenter;
            btnItemManagement.Click += BtnItemManagement_Click;
            
            // Sales Report Button
            btnSalesReport = new Button();
            btnSalesReport.Text = "📊 SALES REPORTS\n(F5)";
            btnSalesReport.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSalesReport.Size = new Size(300, 120);
            btnSalesReport.Location = new Point(200, 350);
            btnSalesReport.BackColor = Color.FromArgb(155, 89, 182);
            btnSalesReport.ForeColor = Color.White;
            btnSalesReport.FlatStyle = FlatStyle.Flat;
            btnSalesReport.FlatAppearance.BorderSize = 0;
            btnSalesReport.Cursor = Cursors.Hand;
            btnSalesReport.TextAlign = ContentAlignment.MiddleCenter;
            btnSalesReport.Click += BtnSalesReport_Click;
            
            contentPanel.Controls.Add(btnBilling);
            contentPanel.Controls.Add(btnItemManagement);
            contentPanel.Controls.Add(btnSalesReport);
            
            // Footer Panel
            Panel footerPanel = new Panel();
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 70;
            footerPanel.BackColor = Color.FromArgb(34, 34, 34);
            footerPanel.BorderStyle = BorderStyle.FixedSingle;
            footerPanel.Padding = new Padding(20);
            
            // Logout Button
            btnLogout = new Button();
            btnLogout.Text = "🔓 LOGOUT (F12)";
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.Size = new Size(150, 40);
            btnLogout.Location = new Point(370, 15);
            btnLogout.BackColor = Color.FromArgb(255, 193, 7);
            btnLogout.ForeColor = Color.Black;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += BtnLogout_Click;
            
            // Exit Button
            btnExit = new Button();
            btnExit.Text = "❌ EXIT (Esc)";
            btnExit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnExit.Size = new Size(150, 40);
            btnExit.Location = new Point(540, 15);
            btnExit.BackColor = Color.FromArgb(220, 53, 69);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Cursor = Cursors.Hand;
            btnExit.Click += BtnExit_Click;
            
            footerPanel.Controls.Add(btnLogout);
            footerPanel.Controls.Add(btnExit);
            
            // Add panels to main panel
            panelMain.Controls.Add(contentPanel);
            panelMain.Controls.Add(headerPanel);
            panelMain.Controls.Add(footerPanel);
            
            // Add main panel to form
            this.Controls.Add(panelMain);
        }
        
        private void DisplayUserInfo()
        {
            lblUserInfo.Text = $"Logged in as: {ApplicationContext.Username} ({ApplicationContext.UserRole})";
            
            // If no user is logged in (shouldn't happen), return to login
            if (ApplicationContext.UserID == 0)
            {
                MessageBox.Show("Session expired. Please login again.", "Session Expired", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ReturnToLogin();
            }
        }
        
        private void BtnBilling_Click(object sender, EventArgs e)
        {
            OpenBillingForm();
        }
        
        private void BtnItemManagement_Click(object sender, EventArgs e)
        {
            OpenItemManagementForm();
        }
        
        private void BtnSalesReport_Click(object sender, EventArgs e)
        {
            OpenSalesReportForm();
        }
        
        private void OpenBillingForm()
        {
            Billing billingForm = new Billing();
            billingForm.Show();
            this.Hide();
        }
        
        private void OpenItemManagementForm()
        {
            ItemManagement itemForm = new ItemManagement();
            itemForm.Show();
            this.Hide();
        }
        
        private void OpenSalesReportForm()
        {
            SalesReport reportForm = new SalesReport();
            reportForm.Show();
            this.Hide();
        }
        
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }
        
        private void BtnExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }
        
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    OpenBillingForm();
                    break;
                    
                case Keys.F3:
                    OpenItemManagementForm();
                    break;
                    
                case Keys.F5:
                    OpenSalesReportForm();
                    break;
                    
                case Keys.F12:
                    Logout();
                    break;
                    
                case Keys.Escape:
                    ExitApplication();
                    break;
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user is closing the main form
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "Do you want to exit the application?", 
                    "Exit", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        
        private void Logout()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?", 
                "Logout", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // Clear user session
                ApplicationContext.UserID = 0;
                ApplicationContext.Username = "";
                ApplicationContext.UserRole = "";
                
                ReturnToLogin();
            }
        }
        
        private void ReturnToLogin()
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }
        
        private void ExitApplication()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?", 
                "Exit Application", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}