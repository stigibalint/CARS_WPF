using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        private string connectionString = "Server=localhost;Database=classicmodels;Uid=root;";
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ObservableCollection<OrderProduct> OrderProducts { get; set; }

        public MainWindow()
        {
            InitializeComponent();

    
            Products = new ObservableCollection<Product>();
            Customers = new ObservableCollection<Customer>();
            Orders = new ObservableCollection<Order>();
            Countries = new ObservableCollection<string>();
            OrderProducts = new ObservableCollection<OrderProduct>();
            this.DataContext = this;
            LoadProducts();
            LoadCountries();
            LoadOrders();
        }

        private void LoadProducts()
        {
            Products.Clear();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT productCode, productName FROM products ORDER BY productName";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Products.Add(new Product
                            {
                                ProductCode = reader.GetString("productCode"),
                                ProductName = reader.GetString("productName")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a termékek betöltésekor: " + ex.Message);
            }
            lstProducts.ItemsSource = null; 
            lstProducts.ItemsSource = Products;
        }

        private void CheckProductOrders(string productCode)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM orderdetails WHERE productCode = @code";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", productCode);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            txtOrderCount.Text = $"Rendelések száma: {count}";
                        }
                        else
                        {
                            txtOrderCount.Text = "Nincs rendelés";
                            MessageBox.Show("A termékre nincs rendelés!", "Információ",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba: " + ex.Message);
            }
        }

        private void LoadCountries()
        {
            Countries.Clear();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT country FROM customers ORDER BY country";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Countries.Add(reader.GetString("country"));
                        }
                    }
                }
                cmbCountries.ItemsSource = Countries;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az országok betöltésekor: " + ex.Message);
            }
        }

        private void LoadCustomersByCountry(string country)
        {
            Customers.Clear();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT customerName, phone, city FROM customers WHERE country = @country ORDER BY customerName";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@country", country);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Customers.Add(new Customer
                                {
                                    CustomerName = reader.GetString("customerName"),
                                    Phone = reader.GetString("phone"),
                                    City = reader.GetString("city")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az ügyfelek betöltésekor: " + ex.Message);
            }
            lstCustomers.ItemsSource = null;
            lstCustomers.ItemsSource = Customers;
        }


        private void LoadOrders()
        {
            Orders.Clear();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT orderNumber, orderDate, status, customerNumber FROM orders ORDER BY orderDate DESC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders.Add(new Order
                            {
                                OrderNumber = reader.GetInt32("orderNumber"),
                                OrderDate = reader.GetDateTime("orderDate"),
                                Status = reader.GetString("status"),
                                CustomerNumber = reader.GetInt32("customerNumber")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a rendelések betöltésekor: " + ex.Message);
            }
            dgOrders.ItemsSource = null;
            dgOrders.ItemsSource = Orders;
        }
        private void LoadOrderProducts(int orderNumber)
        {
            OrderProducts.Clear();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT p.productName, p.buyPrice
                            FROM orderdetails od
                            JOIN products p ON od.productCode = p.productCode
                            WHERE od.orderNumber = @orderNum
                            ORDER BY p.productName";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderNum", orderNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OrderProducts.Add(new OrderProduct
                                {
                                    ProductName = reader.GetString("productName"),
                                    BuyPrice = reader.GetDecimal("buyPrice")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a termékek betöltésekor: " + ex.Message);
            }
            lstOrderProducts.ItemsSource = null;
            lstOrderProducts.ItemsSource = OrderProducts;
        }

        private void lstProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstProducts.SelectedItem != null)
            {
                Product product = lstProducts.SelectedItem as Product;
                CheckProductOrders(product.ProductCode);
            }
        }

        private void cmbCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCountries.SelectedItem != null)
            {
                string country = cmbCountries.SelectedItem.ToString();
                LoadCustomersByCountry(country);
            }
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                Order order = dgOrders.SelectedItem as Order;
                LoadOrderProducts(order.OrderNumber);
            }
        }
    }
}