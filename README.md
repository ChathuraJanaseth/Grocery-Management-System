# Grocery Management System (GMS) 🛒

A professional and user-friendly desktop application developed using **C# Windows Forms (WinForms)** and **Microsoft SQL Server**. This system is specifically designed to handle fast customer billing and secure store administration for grocery stores and supermarkets.

## 🚀 Key Features & System Modules

This project consists of 5 main interfaces, structured for maximum efficiency:

*   **🔒 Secure Login (`LoginForm.cs`):** Password-protected authentication system to ensure only authorized administrators can access sensitive features.
*   **🎛️ Main Dashboard (`MainForm.cs`):** A centralized and intuitive navigation hub to access all the features of the system.
*   **🧾 Fast Billing Interface (`Billing.cs`):** 
    * Quick item search and filtering.
    * Automatic calculation of discounts, total amount, received amount, and balance.
    * Add-to-cart functionality and F10 shortcut for fast bill printing.
*   **📦 Item & Discount Management (`ItemManagement.cs`):** A dedicated admin panel to easily add new grocery items, update prices, and configure special discounts.
*   **📊 Sales Reporting (`SalesReport.cs`):** Allows administrators to view, track, and analyze monthly sales data to monitor business growth.

*(Note: This system focuses strictly on efficient billing, discount calculations, and sales tracking. It does not include complex warehouse stock/inventory management).*

## 🛠️ Technologies Used

*   **Frontend:** C# / .NET Framework (Windows Forms)
*   **Database:** Microsoft SQL Server
*   **IDE:** Visual Studio

## ⚙️ How to Setup and Run Locally

1.  **Clone the Repository:** 
```bash
    git clone [https://github.com/ChathuraJanaseth/Grocery-Management-System.git](https://github.com/ChathuraJanaseth/Grocery-Management-System.git)
    ```
2.  **Open the Project:** Open the `.sln` file using Visual Studio.
3.  **Database Setup:** 
    * Open SQL Server Management Studio (SSMS).
    * Execute the provided SQL script to create the database, tables, and dummy data.
4.  **Configuration:** Update the SQL Connection String in your code or `App.config` file to match your local SQL Server instance.
5.  **Build & Run:** Press `Start` in Visual Studio to run the application.

---
*Developed as a comprehensive showcase of C# WinForms and SQL Database integration.*
