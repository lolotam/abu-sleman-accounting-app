using System;
using System.Drawing;
using System.Windows.Forms;

public class SimpleAccountingApp : Form
{
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;
    private Label lblStatus;

    public SimpleAccountingApp()
    {
        // Form settings
        this.Text = "Abu Sleman Accounting System";
        this.Size = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.White;

        // Create title label
        Label lblTitle = new Label();
        lblTitle.Text = "Abu Sleman Accounting System";
        lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(0, 150, 136); // Teal color
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Size = new Size(350, 30);
        lblTitle.Location = new Point(25, 20);

        // Create subtitle label
        Label lblSubtitle = new Label();
        lblSubtitle.Text = "Financial Management System";
        lblSubtitle.Font = new Font("Arial", 10);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitle.Size = new Size(350, 20);
        lblSubtitle.Location = new Point(25, 50);

        // Create username label
        Label lblUsername = new Label();
        lblUsername.Text = "Username:";
        lblUsername.Size = new Size(100, 20);
        lblUsername.Location = new Point(50, 90);

        // Create username textbox
        txtUsername = new TextBox();
        txtUsername.Size = new Size(200, 20);
        txtUsername.Location = new Point(150, 90);

        // Create password label
        Label lblPassword = new Label();
        lblPassword.Text = "Password:";
        lblPassword.Size = new Size(100, 20);
        lblPassword.Location = new Point(50, 120);

        // Create password textbox
        txtPassword = new TextBox();
        txtPassword.PasswordChar = '*';
        txtPassword.Size = new Size(200, 20);
        txtPassword.Location = new Point(150, 120);

        // Create login button
        btnLogin = new Button();
        btnLogin.Text = "LOGIN";
        btnLogin.Size = new Size(100, 30);
        btnLogin.Location = new Point(150, 160);
        btnLogin.BackColor = Color.FromArgb(0, 150, 136); // Teal color
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Click += new EventHandler(BtnLogin_Click);

        // Create status label
        lblStatus = new Label();
        lblStatus.Text = "";
        lblStatus.ForeColor = Color.Red;
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        lblStatus.Size = new Size(300, 20);
        lblStatus.Location = new Point(50, 200);

        // Create version label
        Label lblVersion = new Label();
        lblVersion.Text = "Version 1.0.0";
        lblVersion.Font = new Font("Arial", 8);
        lblVersion.ForeColor = Color.Gray;
        lblVersion.TextAlign = ContentAlignment.MiddleCenter;
        lblVersion.Size = new Size(350, 20);
        lblVersion.Location = new Point(25, 230);

        // Add controls to form
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblSubtitle);
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(lblStatus);
        this.Controls.Add(lblVersion);
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        // Check credentials
        if (txtUsername.Text == "admin" && txtPassword.Text == "admin123")
        {
            lblStatus.Text = "Login successful!";
            lblStatus.ForeColor = Color.Green;
            
            // Open main form
            MessageBox.Show("Welcome to Abu Sleman Accounting System!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Hide login form and show main form
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
            this.Close();
        }
        else
        {
            lblStatus.Text = "Invalid username or password!";
            lblStatus.ForeColor = Color.Red;
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new SimpleAccountingApp());
    }
}

public class MainForm : Form
{
    private Panel sidePanel;
    private Panel contentPanel;
    private Label lblTitle;

    public MainForm()
    {
        // Form settings
        this.Text = "Abu Sleman Accounting System";
        this.Size = new Size(1024, 768);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;

        // Create side panel
        sidePanel = new Panel();
        sidePanel.Size = new Size(200, this.ClientSize.Height);
        sidePanel.Location = new Point(0, 0);
        sidePanel.BackColor = Color.FromArgb(0, 121, 107); // Dark Teal
        sidePanel.Dock = DockStyle.Left;

        // Create content panel
        contentPanel = new Panel();
        contentPanel.Size = new Size(this.ClientSize.Width - sidePanel.Width, this.ClientSize.Height);
        contentPanel.Location = new Point(sidePanel.Width, 0);
        contentPanel.BackColor = Color.White;
        contentPanel.Dock = DockStyle.Fill;

        // Create title label
        lblTitle = new Label();
        lblTitle.Text = "Dashboard";
        lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(0, 150, 136); // Teal color
        lblTitle.Size = new Size(300, 30);
        lblTitle.Location = new Point(20, 20);
        contentPanel.Controls.Add(lblTitle);

        // Create menu buttons
        string[] menuItems = { "Dashboard", "Products", "Customers", "Suppliers", "Sales", "Purchases", "Accounting", "Reports", "Settings" };
        for (int i = 0; i < menuItems.Length; i++)
        {
            Button btnMenu = new Button();
            btnMenu.Text = menuItems[i];
            btnMenu.Size = new Size(200, 40);
            btnMenu.Location = new Point(0, 100 + (i * 45));
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.ForeColor = Color.White;
            btnMenu.TextAlign = ContentAlignment.MiddleLeft;
            btnMenu.Padding = new Padding(20, 0, 0, 0);
            btnMenu.BackColor = Color.FromArgb(0, 121, 107); // Dark Teal
            btnMenu.Tag = menuItems[i];
            btnMenu.Click += new EventHandler(BtnMenu_Click);
            sidePanel.Controls.Add(btnMenu);
        }

        // Create app title in side panel
        Label lblAppTitle = new Label();
        lblAppTitle.Text = "Abu Sleman";
        lblAppTitle.Font = new Font("Arial", 14, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.Size = new Size(200, 30);
        lblAppTitle.Location = new Point(20, 20);
        sidePanel.Controls.Add(lblAppTitle);

        Label lblAppSubtitle = new Label();
        lblAppSubtitle.Text = "Accounting System";
        lblAppSubtitle.Font = new Font("Arial", 10);
        lblAppSubtitle.ForeColor = Color.FromArgb(200, 255, 255, 255);
        lblAppSubtitle.Size = new Size(200, 20);
        lblAppSubtitle.Location = new Point(20, 50);
        sidePanel.Controls.Add(lblAppSubtitle);

        // Create dashboard content
        CreateDashboardContent();

        // Add panels to form
        this.Controls.Add(sidePanel);
        this.Controls.Add(contentPanel);
    }

    private void BtnMenu_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        lblTitle.Text = btn.Tag.ToString();

        // Clear content panel except title
        foreach (Control ctrl in contentPanel.Controls.ToArray())
        {
            if (ctrl != lblTitle)
                contentPanel.Controls.Remove(ctrl);
        }

        // Show appropriate content
        if (btn.Tag.ToString() == "Dashboard")
        {
            CreateDashboardContent();
        }
        else
        {
            Label lblComingSoon = new Label();
            lblComingSoon.Text = $"{btn.Tag.ToString()} module is coming soon...";
            lblComingSoon.Font = new Font("Arial", 14);
            lblComingSoon.ForeColor = Color.Gray;
            lblComingSoon.Size = new Size(400, 30);
            lblComingSoon.Location = new Point(20, 70);
            contentPanel.Controls.Add(lblComingSoon);
        }
    }

    private void CreateDashboardContent()
    {
        // Create summary cards
        string[] cardTitles = { "Sales", "Purchases", "Expenses", "Profit" };
        string[] cardValues = { "$25,000", "$15,000", "$3,000", "$7,000" };
        Color[] cardColors = { 
            Color.FromArgb(3, 169, 244),   // Light Blue
            Color.FromArgb(156, 39, 176),  // Purple
            Color.FromArgb(233, 30, 99),   // Pink
            Color.FromArgb(0, 150, 136)    // Teal
        };

        for (int i = 0; i < cardTitles.Length; i++)
        {
            Panel card = new Panel();
            card.Size = new Size(200, 100);
            card.Location = new Point(20 + (i * 220), 70);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.None;

            // Add shadow effect
            card.Paint += (sender, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            Label lblCardTitle = new Label();
            lblCardTitle.Text = cardTitles[i];
            lblCardTitle.Font = new Font("Arial", 12);
            lblCardTitle.ForeColor = Color.Gray;
            lblCardTitle.Size = new Size(180, 20);
            lblCardTitle.Location = new Point(15, 15);
            card.Controls.Add(lblCardTitle);

            Label lblCardValue = new Label();
            lblCardValue.Text = cardValues[i];
            lblCardValue.Font = new Font("Arial", 16, FontStyle.Bold);
            lblCardValue.ForeColor = cardColors[i];
            lblCardValue.Size = new Size(180, 30);
            lblCardValue.Location = new Point(15, 45);
            card.Controls.Add(lblCardValue);

            Panel colorBar = new Panel();
            colorBar.Size = new Size(5, 100);
            colorBar.Location = new Point(0, 0);
            colorBar.BackColor = cardColors[i];
            card.Controls.Add(colorBar);

            contentPanel.Controls.Add(card);
        }

        // Create recent transactions panel
        Panel recentPanel = new Panel();
        recentPanel.Size = new Size(contentPanel.Width - 40, 300);
        recentPanel.Location = new Point(20, 190);
        recentPanel.BackColor = Color.White;
        recentPanel.BorderStyle = BorderStyle.None;
        recentPanel.Paint += (sender, e) => {
            ControlPaint.DrawBorder(e.Graphics, recentPanel.ClientRectangle,
                Color.LightGray, 1, ButtonBorderStyle.Solid,
                Color.LightGray, 1, ButtonBorderStyle.Solid,
                Color.LightGray, 1, ButtonBorderStyle.Solid,
                Color.LightGray, 1, ButtonBorderStyle.Solid);
        };

        Label lblRecentTitle = new Label();
        lblRecentTitle.Text = "Recent Transactions";
        lblRecentTitle.Font = new Font("Arial", 14, FontStyle.Bold);
        lblRecentTitle.ForeColor = Color.FromArgb(0, 150, 136);
        lblRecentTitle.Size = new Size(200, 25);
        lblRecentTitle.Location = new Point(15, 15);
        recentPanel.Controls.Add(lblRecentTitle);

        string[] customers = { "Ahmed Ali", "Fatima Mohamed", "Omar Khaled", "Layla Ibrahim" };
        string[] types = { "Sales Invoice", "Purchase Invoice", "Sales Return", "Payment" };
        string[] amounts = { "$1,250", "$3,750", "$350", "$2,000" };
        string[] dates = { "15/05/2023", "14/05/2023", "12/05/2023", "10/05/2023" };

        Label headerType = new Label();
        headerType.Text = "Type";
        headerType.Font = new Font("Arial", 10, FontStyle.Bold);
        headerType.Size = new Size(150, 20);
        headerType.Location = new Point(15, 50);
        recentPanel.Controls.Add(headerType);

        Label headerCustomer = new Label();
        headerCustomer.Text = "Customer/Supplier";
        headerCustomer.Font = new Font("Arial", 10, FontStyle.Bold);
        headerCustomer.Size = new Size(150, 20);
        headerCustomer.Location = new Point(170, 50);
        recentPanel.Controls.Add(headerCustomer);

        Label headerAmount = new Label();
        headerAmount.Text = "Amount";
        headerAmount.Font = new Font("Arial", 10, FontStyle.Bold);
        headerAmount.Size = new Size(100, 20);
        headerAmount.Location = new Point(325, 50);
        recentPanel.Controls.Add(headerAmount);

        Label headerDate = new Label();
        headerDate.Text = "Date";
        headerDate.Font = new Font("Arial", 10, FontStyle.Bold);
        headerDate.Size = new Size(100, 20);
        headerDate.Location = new Point(430, 50);
        recentPanel.Controls.Add(headerDate);

        Panel headerLine = new Panel();
        headerLine.Size = new Size(recentPanel.Width - 30, 1);
        headerLine.Location = new Point(15, 75);
        headerLine.BackColor = Color.LightGray;
        recentPanel.Controls.Add(headerLine);

        for (int i = 0; i < 4; i++)
        {
            Label lblType = new Label();
            lblType.Text = types[i];
            lblType.Font = new Font("Arial", 10);
            lblType.Size = new Size(150, 20);
            lblType.Location = new Point(15, 85 + (i * 30));
            recentPanel.Controls.Add(lblType);

            Label lblCustomer = new Label();
            lblCustomer.Text = customers[i];
            lblCustomer.Font = new Font("Arial", 10);
            lblCustomer.Size = new Size(150, 20);
            lblCustomer.Location = new Point(170, 85 + (i * 30));
            recentPanel.Controls.Add(lblCustomer);

            Label lblAmount = new Label();
            lblAmount.Text = amounts[i];
            lblAmount.Font = new Font("Arial", 10);
            lblAmount.Size = new Size(100, 20);
            lblAmount.Location = new Point(325, 85 + (i * 30));
            recentPanel.Controls.Add(lblAmount);

            Label lblDate = new Label();
            lblDate.Text = dates[i];
            lblDate.Font = new Font("Arial", 10);
            lblDate.Size = new Size(100, 20);
            lblDate.Location = new Point(430, 85 + (i * 30));
            recentPanel.Controls.Add(lblDate);

            Panel rowLine = new Panel();
            rowLine.Size = new Size(recentPanel.Width - 30, 1);
            rowLine.Location = new Point(15, 110 + (i * 30));
            rowLine.BackColor = Color.WhiteSmoke;
            recentPanel.Controls.Add(rowLine);
        }

        contentPanel.Controls.Add(recentPanel);
    }
}