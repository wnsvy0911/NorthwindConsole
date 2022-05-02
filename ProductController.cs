using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace NorthwindConsole
{
    class ProductController
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        private static void listProducts()
        {
            logger.Info("Listing all products");
            Console.WriteLine("Would you like to see\n - 1) All Products\n - 2) Discontinued Products\n - 3) Active Products");
            String choice = Console.ReadLine();
            var db = new NWConsole_48_JPKContext();
            var query = db.Products.OrderBy(p => p.ProductName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (choice == "1")
            {
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.ProductName} - {item.UnitPrice}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (choice == "2")
            {
                foreach (var item in query)
                {
                    if (item.Discontinued)
                    {
                        Console.WriteLine($"{item.ProductName} - {item.UnitPrice} - ( Discontinued )");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (choice == "3")
            {
                foreach (var item in query)
                {
                    if (!item.Discontinued)
                    {
                        Console.WriteLine($"{item.ProductName} - {item.UnitPrice}");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine("Bad Choice");
            }
        }

        private static void createProduct()
        {
            logger.Info("Creating a product");
            Product product = new Product();
            Console.WriteLine("Enter Product Name:");
            product.ProductName = Console.ReadLine();
            Console.WriteLine("Enter the Product Supplier Id:");
            product.SupplierId = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Product Category Id:");
            product.CategoryId = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Product Quantity Per Unit:");
            product.QuantityPerUnit = Console.ReadLine();
            Console.WriteLine("Enter the Product Unit Price:");
            product.UnitPrice = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Product Units In Stock:");
            product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Enter the Product Units On Order:");
            product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Enter the Product Reorder Level:");
            product.ReorderLevel = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Enter the Product Discontinued:");
            product.Discontinued = bool.Parse(Console.ReadLine());
            product.Category = null;
            product.OrderDetails = null;
            product.Supplier = null;

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                var db = new NWConsole_48_JPKContext();
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Products.Add(product);
                    db.SaveChanges();
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }

        private static void getProduct()
        {
            logger.Info("Getting a single product");

            var db = new NWConsole_48_JPKContext();
            Console.WriteLine("Enter Product Name To Search For");
            string productName = Console.ReadLine();
            Product product = db.Products.FirstOrDefault(b => b.ProductName == productName);
            Console.WriteLine($"{product.ProductName}\n - Product Id: {product.ProductId}\n - Supplier Id: {product.SupplierId}\n - Category Id: {product.CategoryId}\n - Quantity Per Unit: {product.QuantityPerUnit}\n - Unit Price: {product.UnitPrice}\n - Units In Stock: {product.UnitsInStock}\n - Units On Order: {product.UnitsOnOrder}\n - Reorder Level: {product.ReorderLevel}\n - Discontinued: {product.Discontinued}\n");
        }

        private static String change(String prompt, String currentValue)
        {
            Console.WriteLine("Would you like to change the " + prompt + " field? y/n?");
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("Please Enter A New Value ( Current Value: " + currentValue + " )");
                string newValue = Console.ReadLine();
                return newValue;
            }
            else
            {
                return currentValue;
            }
        }
        private static void updateProduct()
        {
            logger.Info("Updating a product");
            var db = new NWConsole_48_JPKContext();
            Console.WriteLine("Enter Product Name To Search For");
            string productName = Console.ReadLine();
            Product product = db.Products.FirstOrDefault(b => b.ProductName == productName);
            product.ProductName = change("Product Name", product.ProductName);
            product.SupplierId = Int32.Parse(change("Supplier Id", Convert.ToString(product.SupplierId)));
            product.CategoryId = Int32.Parse(change("Category Id", Convert.ToString(product.CategoryId)));
            product.QuantityPerUnit = change("Quantity Per Unit", product.QuantityPerUnit);
            product.UnitPrice = decimal.Parse(change("Unit Price", Convert.ToString(product.UnitPrice)));
            product.UnitsInStock = Int16.Parse(change("Units in stock", Convert.ToString(product.UnitsInStock)));
            product.UnitsOnOrder = Int16.Parse(change("Units On Order", Convert.ToString(product.UnitsOnOrder)));
            product.ReorderLevel = Int16.Parse(change("Reorder Level", Convert.ToString(product.ReorderLevel)));
            product.Discontinued = bool.Parse(change("Discountinued", Convert.ToString(product.Discontinued)));

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Products.Update(product);
                    db.SaveChanges();
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }

        private static void deleteProduct()
        {
            logger.Info("Delete a single product");

            var db = new NWConsole_48_JPKContext();
            Console.WriteLine("Enter Product Name To Search For And Delete");
            string productName = Console.ReadLine();
            Product product = db.Products.FirstOrDefault(b => b.ProductName == productName);
            db.Products.Remove(product);
            db.SaveChanges();
        }

        public static void manageProductWorkflows()
        {
            logger.Info("Entering Category Workflow");

            try
            {

                string choice;
                do
                {
                    Console.WriteLine("1) Display Products");
                    Console.WriteLine("2) Add Product");
                    Console.WriteLine("3) Get Product");
                    Console.WriteLine("4) Update Product");
                    Console.WriteLine("5) Delete Product");
                    Console.WriteLine("\"b\" to Go Back to Main Menu");


                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option \"{choice}\" selected");

                    if (choice == "1")
                    {
                        listProducts();
                    }
                    else if (choice == "2")
                    {
                        createProduct();
                    }
                    else if (choice == "3")
                    {
                        getProduct();
                    }
                    else if (choice == "4")
                    {
                        updateProduct();
                    }
                    else if (choice == "5")
                    {
                        deleteProduct();
                    }

                } while (choice.ToLower() != "b");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }
    }
}