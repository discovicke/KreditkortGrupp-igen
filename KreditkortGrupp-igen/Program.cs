using KreditkortGrupp_igen;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

using var connection = new SqliteConnection("Data Source=kreditkort.db");
connection.Open();

// Create tables with fresh local commands to avoid accidentally reusing a command
using (var createPeople = connection.CreateCommand())
{
    createPeople.CommandText = @"
CREATE TABLE IF NOT EXISTS People (
    id INTEGER PRIMARY KEY, 
    Name TEXT NOT NULL,
    HasCard BOOLEAN DEFAULT FALSE)
    ";
    createPeople.ExecuteNonQuery();
}

using (var createCards = connection.CreateCommand())
{
    createCards.CommandText = @"
CREATE TABLE IF NOT EXISTS CreditCards 
(id INTEGER PRIMARY KEY, 
CardNumber TEXT NOT NULL,
PersonId INTEGER, 
FOREIGN KEY (PersonId) REFERENCES People(id))";
    createCards.ExecuteNonQuery();
}


var rnd = new Random();

var firstnamePath = "MOCK_DATA_first_name.json";
var lastnamePath = "MOCK_DATA-last_name.json";

var arrayCreator = new JsonArrayCreator();

string[] menu =
{
    "1. Generate mock data?",
    "2. List People?",
    "3. List Cards?",
    "4. List Transactions",
    "5. Exit"
};

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
            nameList = arrayCreator.CreateNameList(firstnamePath, lastnamePath, numberOfPeople);

            // Speed: adjust pragmas for bulk insert
            using (var p = connection.CreateCommand())
            {
                p.CommandText = "PRAGMA synchronous = OFF;";
                p.ExecuteNonQuery();
                p.CommandText = "PRAGMA journal_mode = MEMORY;";
                p.ExecuteNonQuery();
            }

            // Single transaction for all DB writes in this operation
            using (var transaction = connection.BeginTransaction())
            {
                // Insert People with a prepared statement
                using (var insertPersonCmd = connection.CreateCommand())
                {
                    insertPersonCmd.Transaction = transaction;
                    insertPersonCmd.CommandText = "INSERT INTO People (Name) VALUES (@Name)";
                    var nameParam = insertPersonCmd.CreateParameter();
                    nameParam.ParameterName = "@Name";
                    insertPersonCmd.Parameters.Add(nameParam);
                    insertPersonCmd.Prepare();

                    foreach (var name in nameList)
                    {
                        nameParam.Value = name.ToString();
                        insertPersonCmd.ExecuteNonQuery();
                    }
                }

                // Select IDs of users without cards
                var ids = new List<long>();
                using (var selectCmd = connection.CreateCommand())
                {
                    selectCmd.Transaction = transaction;
                    selectCmd.CommandText = "SELECT id FROM People WHERE HasCard = 0";
                    using var reader = selectCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ids.Add(reader.GetInt64(0));
                    }
                }

                // Prepare insert for cards
                using (var insertCardCmd = connection.CreateCommand())
                {
                    insertCardCmd.Transaction = transaction;
                    insertCardCmd.CommandText = "INSERT INTO CreditCards (PersonId, CardNumber) VALUES (@PersonId, @CardNumber)";
                    var personIdParam = insertCardCmd.CreateParameter();
                    personIdParam.ParameterName = "@PersonId";
                    insertCardCmd.Parameters.Add(personIdParam);
                    var cardNumberParam = insertCardCmd.CreateParameter();
                    cardNumberParam.ParameterName = "@CardNumber";
                    insertCardCmd.Parameters.Add(cardNumberParam);
                    insertCardCmd.Prepare();

                    // Assign cards per person according to bucket logic
                    foreach (var userId in ids)
                    {
                        var bucket = (int)(userId % 100);
                        int numberOfCards = 0;
                        if (bucket < 70)
                        {
                            numberOfCards = 1;
                        }
                        else if (bucket < 90)
                        {
                            numberOfCards = 2;
                        }
                        else
                        {
                            numberOfCards = rnd.Next(3, 11);
                        }

                        for (int i = 0; i < numberOfCards; i++)
                        {
                            personIdParam.Value = userId;
                            cardNumberParam.Value = CreditCard.GenerateCreditCardNumber(rnd);
                            insertCardCmd.ExecuteNonQuery();
                        }
                    }
                }

                // Optionally mark users as having cards
                using (var updateCmd = connection.CreateCommand())
                {
                    updateCmd.Transaction = transaction;
                    updateCmd.CommandText = "UPDATE People SET HasCard = 1 WHERE HasCard = 0";
                    updateCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            var elapsed = sw.Elapsed.TotalSeconds;
            Console.WriteLine($"Data generated in {elapsed:F1} seconds. \nPress any key to return to main menu");
            if (!Console.IsInputRedirected) Console.ReadKey();
            break;
        }

        case "2":
            PrintNames(nameList);
            Console.WriteLine("Data generated. \nPress any key to return to main menu");

            if (!Console.IsInputRedirected) Console.ReadKey();
            break;

        case "3":
            string card = CreditCard.GenerateCreditCardNumber(rnd);
            Console.WriteLine($"Generated card: {card}");

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
