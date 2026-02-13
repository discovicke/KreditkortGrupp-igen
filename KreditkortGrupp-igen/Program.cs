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
    HasCard BOOLEAN DEFAULT FALSE)
    ";
command.ExecuteNonQuery();
command.CommandText = @"
CREATE TABLE IF NOT EXISTS CreditCards 
(id INTEGER PRIMARY KEY, 
CardNumber TEXT NOT NULL,
PersonId INTEGER, 
FOREIGN KEY (PersonId) REFERENCES People(id))";
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
            
            command.Transaction = transaction;
            
            command.CommandText = @"INSERT INTO CreditCards (PersonId, CardNumber) VALUES (@PersonId, @CardNumber)";

            var dataToAdd = CardToPerson(param);

            foreach (var person in dataToAdd)
            {
                var personIdParam = command.CreateParameter();
                personIdParam.ParameterName = "@PersonId";
                personIdParam.Value = person.Key;
                command.Parameters.Add(personIdParam);
                
                var cardNumberParam = command.CreateParameter();
                cardNumberParam.ParameterName = "@CardNumber";
                cardNumberParam.Value = person.Value;
                command.Parameters.Add(cardNumberParam);
                
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

Dictionary<long, string> CardToPerson(string cardNumber)
{
    var listOfUsers = command.CommandText = @"
        SELECT 
            Users.Id 
        FROM
            Users
        WHERE HasCard = false";
    command.ExecuteReader();
    
    foreach (user in listOfUsers)
    {
        var bucket = user % 100;
        var toBeInserted = new Dictionary<long, string>();
        
        if (bucket < 70)
        {
            //get number and assign to var cardNumberToAdd
            toBeInserted.Add(user, cardNumberToAdd);
        } 
        else if (user < 90)
        {   
            var numberOfCards = 2;
            
            foreach (card in numberOfCards)
            {
                //get number and assign to var cardNumberToAdd
                toBeInserted.Add(user, cardNumberToAdd);
            }
        }
        var rand = new Random();
        var numberOfCards =  rand.Next(3, 11);
            
        foreach (card in numberOfCards)
        {
            //get number and assign to var cardNumberToAdd
            toBeInserted.Add(user, cardNumberToAdd);
        }

        return toBeInserted;
    }
    
    
    /*
     * Plocka ut alla användare ur databsen --> lista
     * Fördela användare till 3 typer
     * Insert typ 1 = 1 kort
     * 2 = 2
     * 3 = 3-10 (random)
     * IF redan har nummer = inget händer
     *
     * För varje person i databasen ska det finnas ett kort
     * OM person.id STARTS WITH = 1 > 6
     * THEN CardNumber.PersonId = person.id på ETT KORT
     * OM person.id START WITH = 7 > 8
     * THEN CardNumber.PersonId = person.id på TVÅ KORT
     * OM person.id START WITH = 9
     * THEN CardNumber.PersonId = person.id på TRE KORT
     */
}