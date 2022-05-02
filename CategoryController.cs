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
    class CategoryController
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        private static void listCategories()
        {
            logger.Info("Listing all cateogries");
            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description} - {item.CategoryId}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void createCategory()
        {
            logger.Info("Creating A category");

            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                var db = new NWConsole_48_JPKContext();
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    // TODO: save category to db
                    db.Categories.Add(category);
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

        private static void listRelatedProducts()
        {
            logger.Info("Listing A category and its products");

            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            //Category category = db.Category.FirstOrDefault(c => c.CategoryId == id);
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                Console.WriteLine(p.ProductName);
            }
        }

        private static void listAllCategoriesAndThierProducts()
        {
            logger.Info("Listing All Categories and All Products");

            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
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

        private static void updateCategory()
        {

            logger.Info("Updating Category");

            var db = new NWConsole_48_JPKContext();
            Console.WriteLine("Enter Category Name To Search For");
            string categoryName = Console.ReadLine();
            Category category = db.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            category.CategoryName = change("Category Name", category.CategoryName);
            category.Description = change("Description", category.Description);


            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Categories.Update(category);
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

        private static void DisplayAllCategoriesAndAllActiveProducts()
        {

            logger.Info("Displaying all category and all active products");

            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    if (!p.Discontinued)
                    {
                        Console.WriteLine($"\t{p.ProductName}");
                    }
                }
            }
        }

        private static void DisplayCategoryAndActiveProducts()
        {
            logger.Info("Displaying a category and all its active products");

            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            //Category category = db.Category.FirstOrDefault(c => c.CategoryId == id);
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                if (!p.Discontinued)
                {
                    Console.WriteLine(p.ProductName);
                }
            }
        }

        private static void deleteCategory()
        {
            logger.Info("Deleting A category and its products");

            var db = new NWConsole_48_JPKContext();
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category which you would like to delete");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            //Category category = db.Category.FirstOrDefault(c => c.CategoryId == id);
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                db.Products.Remove(p);
            }
            db.Categories.Remove(category);
            db.SaveChanges();
        }

        public static void manageCategoryWorkflows()
        {
            logger.Info("Entering Category Workflow");

            try
            {

                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Update Category");
                    Console.WriteLine("4) Display Category and related products");
                    Console.WriteLine("5) Display All Categories and their related products");
                    Console.WriteLine("6) Display All Categories and Active Products");
                    Console.WriteLine("7) Delete Category");
                    Console.WriteLine("\"back\" to go to main menu");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");

                    if (choice == "1")
                    {
                        listCategories();
                    }
                    else if (choice == "2")
                    {
                        createCategory();
                    }
                    else if (choice == "3")
                    {
                        updateCategory();
                    }
                    else if (choice == "4")
                    {
                        listRelatedProducts();
                    }
                    else if (choice == "5")
                    {
                        listAllCategoriesAndThierProducts();
                    }
                    else if (choice == "6")
                    {
                        DisplayAllCategoriesAndAllActiveProducts();
                    }
                    else if (choice == "7")
                    {
                        deleteCategory();
                    }

                } while (choice.ToLower() != "back");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }

    }
}