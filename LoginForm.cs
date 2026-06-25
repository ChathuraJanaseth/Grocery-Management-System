using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GMS
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Data Source=CHATHURA\\SQLEXPRESS;Initial Catalog=GroceryDB;Integrated Security=True;TrustServerCertificate=True";
        
        private TextBox txtUsername, txtPassword;
        private Button btnLogin, btnExit;
        private Label lblTitle, lblUsername, lblPassword, lblError;
        private CheckBox chkShowPassword;
        private Panel panelMain;
        
        public LoginForm()
        {
            SetupForm();
        }
        
        private void SetupForm()
        {
            // Form Properties
            this.Text = "ABC GROCERY - Login";
            this.Size = new Size(350, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 230, 230);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;
            
            // Main Panel
            panelMain = new Panel();
            panelMain.Size = new Size(350, 400);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point((this.ClientSize.Width - 350) / 2, (this.ClientSize.Height - 400) / 2);
            panelMain.BackColor = Color.White;
            panelMain.BorderStyle = BorderStyle.FixedSingle;
            
            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "ABC GROCERY";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(13, 71, 161);
            lblTitle.Size = new Size(300, 50);
            lblTitle.Location = new Point(25, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            
            // Subtitle
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Professional Billing System";
            lblSubtitle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Size = new Size(300, 30);
            lblSubtitle.Location = new Point(25, 80);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            
            // Username Label
            lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(64, 64, 64);
            lblUsername.Size = new Size(100, 25);
            lblUsername.Location = new Point(40, 140);
            
            // Username TextBox
            txtUsername = new TextBox();
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.Size = new Size(250, 35);
            txtUsername.Location = new Point(40, 170);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.MaxLength = 50;
            txtUsername.KeyDown += TxtUsername_KeyDown;
            
            // Password Label
            lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblPassword.Size = new Size(100, 25);
            lblPassword.Location = new Point(40, 220);
            
            // Password TextBox
            txtPassword = new TextBox();
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.Size = new Size(250, 35);
            txtPassword.Location = new Point(40, 250);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '•';
            txtPassword.MaxLength = 50;
            txtPassword.KeyDown += TxtPassword_KeyDown;
            
            // Show Password Checkbox
            chkShowPassword = new CheckBox();
            chkShowPassword.Text = "Show Password";
            chkShowPassword.Font = new Font("Segoe UI", 9);
            chkShowPassword.Size = new Size(120, 25);
            chkShowPassword.Location = new Point(40, 290);
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            
            // Error Label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblError.ForeColor = Color.Red;
            lblError.Size = new Size(250, 30);
            lblError.Location = new Point(40, 320);
            lblError.TextAlign = ContentAlignment.MiddleLeft;
            
            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.Size = new Size(250, 40);
            btnLogin.Location = new Point(40, 360);
            btnLogin.BackColor = Color.FromArgb(40, 167, 69);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            
            // Exit Button
            btnExit = new Button();
            btnExit.Text = "EXIT";
            btnExit.Font = new Font("Segoe UI", 10);
            btnExit.Size = new Size(100, 30);
            btnExit.Location = new Point(190, 410);
            btnExit.BackColor = Color.FromArgb(108, 117, 125);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Cursor = Cursors.Hand;
            btnExit.Click += BtnExit_Click;
            
            // Add controls to panel
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblSubtitle);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(chkShowPassword);
            panelMain.Controls.Add(lblError);
            panelMain.Controls.Add(btnLogin);
            
            // Add panel to form
            this.Controls.Add(panelMain);
            this.Controls.Add(btnExit);
            
            // Focus on username field
            txtUsername.Focus();
        }
        
        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        
        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                BtnExit_Click(sender, e);
            }
        }
        
        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '•';
        }
        
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            
            // Validation
            if (string.IsNullOrEmpty(username))
            {
                lblError.Text = "Please enter username!";
                txtUsername.Focus();
                return;
            }
            
            if (string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter password!";
                txtPassword.Focus();
                return;
            }
            
            // Authenticate user
            if (AuthenticateUser(username, password))
            {
                lblError.Text = "";
                lblError.ForeColor = Color.Green;
                lblError.Text = "Login successful!";
                
                // Open MainForm
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                lblError.ForeColor = Color.Red;
                lblError.Text = "Invalid username or password!";
                txtPassword.Text = "";
                txtPassword.Focus();
                
                // Shake animation for wrong credentials
                ShakeForm();
            }
        }
        
        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                // First, check if Users table exists
                if (!CheckUsersTableExists())
                {
                    // If table doesn't exist, create default admin user
                    CreateDefaultUsersTable();
                    return CheckDefaultAdmin(username, password);
                }
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Query to check user credentials
                    string query = @"
                    SELECT UserID, Username, UserRole 
                    FROM Users 
                    WHERE Username = @Username AND Password = @Password AND IsActive = 1";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Store user info in application context
                                ApplicationContext.UserID = Convert.ToInt32(reader["UserID"]);
                                ApplicationContext.Username = reader["Username"].ToString();
                                ApplicationContext.UserRole = reader["UserRole"].ToString();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Authentication error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return false;
        }
        
        private bool CheckUsersTableExists()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    string query = @"
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                               WHERE TABLE_NAME = 'Users' AND TABLE_SCHEMA = 'dbo')
                    SELECT 1 ELSE SELECT 0";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        
        private void CreateDefaultUsersTable()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Create Users table
                    string createTableQuery = @"
                    CREATE TABLE Users (
                        UserID INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(50) NOT NULL UNIQUE,
                        Password NVARCHAR(100) NOT NULL,
                        UserRole NVARCHAR(20) NOT NULL DEFAULT 'Admin',
                        FullName NVARCHAR(100),
                        Email NVARCHAR(100),
                        Phone NVARCHAR(20),
                        IsActive BIT DEFAULT 1,
                        CreatedDate DATETIME DEFAULT GETDATE(),
                        LastLogin DATETIME
                    )";
                    
                    using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    
                    // Insert default admin user
                    string insertAdminQuery = @"
                    INSERT INTO Users (Username, Password, UserRole, FullName, Email, Phone)
                    VALUES ('admin', 'admin123', 'Admin', 'System Administrator', 'admin@abcgrocery.lk', '0771234567')";
                    
                    using (SqlCommand cmd = new SqlCommand(insertAdminQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating users table: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private bool CheckDefaultAdmin(string username, string password)
        {
            // Default credentials (for initial setup)
            if (username.ToLower() == "admin" && password == "admin123")
            {
                ApplicationContext.UserID = 1;
                ApplicationContext.Username = "admin";
                ApplicationContext.UserRole = "Admin";
                return true;
            }
            return false;
        }
        
        private void ShakeForm()
        {
            Point originalLocation = this.Location;
            int shakeOffset = 10;
            Random rnd = new Random();
            
            for (int i = 0; i < 10; i++)
            {
                int x = originalLocation.X + rnd.Next(-shakeOffset, shakeOffset);
                int y = originalLocation.Y + rnd.Next(-shakeOffset, shakeOffset);
                this.Location = new Point(x, y);
                System.Threading.Thread.Sleep(20);
            }
            
            this.Location = originalLocation;
        }
        
        private void BtnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit?", 
                "Exit Application", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // If user is closing the login form without logging in
            if (ApplicationContext.UserID == 0)
            {
                DialogResult result = MessageBox.Show(
                    "Do you want to exit the application?", 
                    "Exit", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
    
    // Application Context class to store user session
    public static class ApplicationContext
    {
        public static int UserID { get; set; }
        public static string Username { get; set; }
        public static string UserRole { get; set; }
        
        static ApplicationContext()
        {
            UserID = 0;
            Username = "";
            UserRole = "";
        }
    }
}