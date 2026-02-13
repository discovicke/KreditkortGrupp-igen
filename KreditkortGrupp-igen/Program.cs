using KreditkortGrupp_igen;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

using var connection = new SqliteConnection("Data Source=kreditkort.db");
connection.Open();
using var command = connection.CreateCommand();
command.CommandText =
    @"
CREATE TABLE IF NOT EXISTS People (
    id INTEGER PRIMARY KEY, 
    Name TEXT NOT NULL, 
    CreditId INTEGER, 
    FOREIGN KEY (CreditId) REFERENCES CreditCards(id))";
command.ExecuteNonQuery();
command.CommandText = @"
CREATE TABLE IF NOT EXISTS CreditCards 
(id INTEGER PRIMARY KEY, 
CardNumber TEXT NOT NULL)";
command.ExecuteNonQuery();

var firstnamePath = "MOCK_DATA_first_name.json";
var lastnamePath = "MOCK_DATA-last_name.json";

var arrayCreator = new JsonArrayCreator();

string[] menu =
[
    "1. Generate mock data?",
    "2. List People?",
    "3. List Cards?",
    "4. List Transactions",
    "5. Exit"
];

bool run = true;
var nameList = new List<Name>();

Console.WriteLine("|=== What do you want to do? ===|\n");
while (run)
{
    foreach (var alternativ in menu)
    {
        Console.WriteLine(alternativ);
    }

    Console.Write("\nChoice: ");
    string? val = Console.ReadLine();
    switch (val)
    {
        case "1":
        {
            Console.WriteLine("Number of people to generate (default 100 000: ");
            var numberOfPeople = int.TryParse(Console.ReadLine(), out int result) ? result : 100000;
            Console.WriteLine("Generating data...");
            var sw = Stopwatch.StartNew();
            nameList = arrayCreator.CreateNameList(firstnamePath, lastnamePath, result);

            using var transaction = connection.BeginTransaction();
            command.Transaction = transaction;
            command.CommandText = @"INSERT INTO People (Name) VALUES (@Name)";

            var nameParam = command.CreateParameter();
            nameParam.ParameterName = "@Name";
            command.Parameters.Add(nameParam);

            foreach (var name in nameList)
            {
                nameParam.Value = name.ToString();
                command.ExecuteNonQuery();
            }

            transaction.Commit();


            sw.Stop();
            double seconds = sw.Elapsed.TotalSeconds;

            Console.WriteLine($"Data generated in {seconds:F1} seconds. \nPress any key to return to main menu");
            Console.ReadKey();
            break;
        }

        case "2":
            PrintNames(nameList);
            Console.WriteLine("Data generated. \nPress any key to return to main menu");
            Console.ReadKey();
            break;

        case "3":

            break;

        case "4":

            break;

        case "5":
            Console.WriteLine("Terminating the program");
            run = false;
            break;
    }
}

void PrintNames(List<Name> names)
{
    foreach (var name in names)
    {
        Console.WriteLine(name);
    }
}