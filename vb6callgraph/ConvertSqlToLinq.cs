using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace vb6callgraph
{
    public class ConvertSqlToLinq
    {
        public static void Test()
        {
            List<Person> people = new List<Person>
        {
            new Person { Id = 1, Name = "Alice", Age = 30 },
            new Person { Id = 2, Name = "Bob", Age = 25 },
            new Person { Id = 3, Name = "Charlie", Age = 35 }
        };

            List<Order> orders = new List<Order>
        {
            new Order { OrderId = 1, PersonId = 1, Amount = 100 },
            new Order { OrderId = 2, PersonId = 2, Amount = 200 },
            new Order { OrderId = 3, PersonId = 3, Amount = 300 }
        };

            string sqlSelectAllPeople = "SELECT * FROM people";
            string sqlSelectWherePeople = "SELECT * FROM people WHERE Age > 30";
            string sqlSelectColumnsPeople = "SELECT Id, Name FROM people WHERE Age > 30";
            string sqlSelectAllOrders = "SELECT * FROM orders";
            string sqlSelectWhereOrders = "SELECT * FROM orders WHERE Amount > 150";

            Console.WriteLine(DoConvertionSqlToLinq(sqlSelectAllPeople, people));
            Console.WriteLine(DoConvertionSqlToLinq(sqlSelectWherePeople, people));
            Console.WriteLine(DoConvertionSqlToLinq(sqlSelectColumnsPeople, people));
            Console.WriteLine(DoConvertionSqlToLinq(sqlSelectAllOrders, orders));
            Console.WriteLine(DoConvertionSqlToLinq(sqlSelectWhereOrders, orders));
        }

        public static string DoConvertionSqlToLinq(string sql, object table)
        {
            sql = Regex.Replace(sql, @"\s+", " ").Trim();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return ConvertSelectToLinq(sql, table);
            }
            else if (sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                return ConvertInsertToLinq(sql);
            }
            else if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                return ConvertUpdateToLinq(sql);
            }
            else if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                return ConvertDeleteToLinq(sql);
            }
            return string.Empty;
        }

        public static string ConvertSelectToLinq(string sql, object table)
        {
            var match = Regex.Match(sql, @"SELECT (.+) FROM (\w+)( WHERE (.+))?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string columns = match.Groups[1].Value.Trim();
                string tableName = match.Groups[2].Value;
                string condition = match.Groups[4].Value;

                string linqQuery = tableName;
                if (!string.IsNullOrEmpty(condition))
                {
                    linqQuery += $".Where({ConvertConditionToLinq(condition)})";
                }

                if (columns == "*")
                {
                    var properties = table.GetType().GetGenericArguments()[0].GetProperties().Select(p => p.Name);
                    linqQuery += $".Select(p => new {{ {string.Join(", ", properties.Select(c => $"{c} = p.{c}"))} }})";
                }
                else
                {
                    var columnList = columns.Split(',').Select(c => c.Trim());
                    linqQuery += $".Select(p => new {{ {string.Join(", ", columnList.Select(c => $"{c} = p.{c}"))} }})";
                }

                return linqQuery;
            }
            return string.Empty;
        }

        public static string ConvertInsertToLinq(string sql)
        {
            var match = Regex.Match(sql, @"INSERT INTO (\w+) \((.+)\) VALUES \((.+)\)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string tableName = match.Groups[1].Value;
                string[] columns = match.Groups[2].Value.Split(',').Select(c => c.Trim()).ToArray();
                string[] values = match.Groups[3].Value.Split(',').Select(v => v.Trim()).ToArray();
                return $"{tableName}.Add(new {tableName.Substring(0, tableName.Length - 1)} {{ {string.Join(", ", columns.Zip(values, (c, v) => $"{c} = {v}"))} }})";
            }
            return string.Empty;
        }

        public static string ConvertUpdateToLinq(string sql)
        {
            var match = Regex.Match(sql, @"UPDATE (\w+) SET (.+) WHERE (.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string tableName = match.Groups[1].Value;
                string setClause = match.Groups[2].Value;
                string condition = match.Groups[3].Value;
                return $"{tableName}.Where({ConvertConditionToLinq(condition)}).ToList().ForEach(p => {{ {ConvertSetClauseToLinq(setClause)} }});";
            }
            return string.Empty;
        }

        public static string ConvertDeleteToLinq(string sql)
        {
            var match = Regex.Match(sql, @"DELETE FROM (\w+) WHERE (.+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string tableName = match.Groups[1].Value;
                string condition = match.Groups[2].Value;
                return $"{tableName}.RemoveAll({ConvertConditionToLinq(condition)});";
            }
            return string.Empty;
        }

        public static string ConvertConditionToLinq(string condition)
        {
            return $"p => p.{condition.Replace(" ", "")}";
        }

        public static string ConvertSetClauseToLinq(string setClause)
        {
            return string.Join("; ", setClause.Split(',').Select(s => $"p.{s.Trim()}"));
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
    }
    public class T
    {
        public int T1 { get; set; }
        public int T2 { get; set; }
        public decimal T3 { get; set; }
    }
}
