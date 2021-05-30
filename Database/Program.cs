using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog= CheckersDB;AttachDbFilename=C:\Checkers\CheckersDB.mdf;Integrated Security=True";
            using (var ctx = new GameContext(connectionString))
            {
                
                ctx.Database.Initialize(force: true);
                Console.WriteLine("database created..");
                Console.ReadLine();
            }
        }
    }
}
