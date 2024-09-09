using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace vb6callgraph
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
    }

    public class Order2
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public string Dimensions { get; set; }
    }

    public class LinqGen
    {
        public static void LinqMain(params string[] param)
        {
            List<Customer> customers = new List<Customer>
            {
                new Customer { CustomerID = 1, Name = "Alice" },
                new Customer { CustomerID = 2, Name = "Bob" }
            };

            List<Order2> orders = new List<Order2>
            {
                new Order2 { OrderID = 1, CustomerID = 1, ProductID = 1, OrderDate = DateTime.Now },
                new Order2 { OrderID = 2, CustomerID = 2, ProductID = 4, OrderDate = DateTime.Now },
                new Order2 { OrderID = 3, CustomerID = 2, ProductID = 3, OrderDate = DateTime.Now },
                new Order2 { OrderID = 4, CustomerID = 1, ProductID = 2, OrderDate = DateTime.Now }
            };

            List<Product> products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Laptop", Price = 1000m, Category = "Electronics", Manufacturer = "Company A", ReleaseDate = DateTime.Now, Stock = 50, Description = "High-end laptop", Color = "Silver", Weight = 1.5, Dimensions = "30x20x2 cm" },
                new Product { ProductID = 2, ProductName = "Phone", Price = 500m, Category = "Electronics", Manufacturer = "Company B", ReleaseDate = DateTime.Now, Stock = 100, Description = "Smartphone", Color = "Black", Weight = 0.2, Dimensions = "15x7x0.8 cm" },
                new Product { ProductID = 3, ProductName = "ALaptop", Price = 100m, Category = "Electronics", Manufacturer = "Company A", ReleaseDate = DateTime.Now, Stock = 50, Description = "High-end laptop", Color = "Silver", Weight = 1.5, Dimensions = "30x20x2 cm" },
                new Product { ProductID = 4, ProductName = "IPhone", Price = 250m, Category = "Electronics", Manufacturer = "Company B", ReleaseDate = DateTime.Now, Stock = 100, Description = "Smartphone", Color = "Black", Weight = 0.2, Dimensions = "15x7x0.8 cm" }
            };

            var selectedFields = new List<string>(param);
            var whereConditions = new List<Func<Product, bool>>
            {
                p => p.Price >= 0m,
                p => p.Stock > 0
            };
            var orderByFields = new List<(string Field, bool Descending)>
            {
                ("ProductName", false),
                ("Price", true)
            };

            var query = customers
                .Join(orders, customer => customer.CustomerID, order => order.CustomerID, (customer, order) => new { customer, order })
                .Join(products, co => co.order.ProductID, product => product.ProductID, (co, product) => new { co.customer, co.order, product });

            foreach (var condition in whereConditions)
            {
                query = query.Where(x => condition(x.product));
            }

            IOrderedEnumerable<dynamic> orderedQuery = null;
            foreach (var (field, descending) in orderByFields)
            {
//#if true
                var parameter = Expression.Parameter(typeof(object), "x");
                var productProperty = Expression.Property(Expression.Property(Expression.Convert(parameter, query.First().GetType()), "product"), field);
                var lambda = Expression.Lambda<Func<dynamic, object>>(Expression.Convert(productProperty, typeof(object)), parameter);
//#else
                var parameter2 = Expression.Parameter(typeof(object), "x");
                var productProperty2 = Expression.Property(Expression.Property(Expression.Convert(parameter2, query.GetType().GenericTypeArguments[0]), "product"), field);
                var lambda2 = Expression.Lambda<Func<dynamic, object>>(Expression.Convert(productProperty2, typeof(object)), parameter2);
//#endif

                if (orderedQuery == null)
                {
                    orderedQuery = descending
                        ? query.OrderByDescending(lambda.Compile())
                        : query.OrderBy(lambda.Compile());
                }
                else
                {
                    orderedQuery = descending
                        ? orderedQuery.ThenByDescending(lambda.Compile())
                        : orderedQuery.ThenBy(lambda.Compile());
                }
            }

            var result = orderedQuery.Select(x => new
            {
                CustomerName = x.customer.Name,
                ProductDetails = selectedFields.ToDictionary(field => field, field => x.product.GetType().GetProperty(field).GetValue(x.product, null)),
                OrderDate = x.order.OrderDate
            });

            foreach (var item in result)
            {
                Console.WriteLine($"Customer: {item.CustomerName}, Order Date: {item.OrderDate}");
                foreach (var detail in item.ProductDetails)
                {
                    Console.WriteLine($"{detail.Key}: {detail.Value}");
                }
            }
        }
    }
}
