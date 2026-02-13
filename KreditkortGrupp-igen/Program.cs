using KreditkortGrupp_igen;


var firstnamePath = "MOCK_DATA_first_name.json";
var lastnamePath = "MOCK_DATA-last_name.json";

var arrayCreator = new JsonArrayCreator();

string[] menu = [
    "1. Generate mock data?",
    "2. List People?",
    "3. List Cards?",
    "4. List Transactions",
    "5. Exit"
];

bool run = true;

Console.WriteLine("|=== What do you want to do? ===|\n");
while (run)
{
    foreach(var alternativ in menu)
    {
        Console.WriteLine(alternativ);
    }
    Console.Write("\nChoice: ");
    string? val = Console.ReadLine();
    switch (val)
    {
        case "1":
            Console.WriteLine("Number of people to generate (default 100 000: ");
            var numberOfPeople = int.TryParse(Console.ReadLine(), out int result) ? result : 100000;
            var nameList = arrayCreator.CreateNameList(firstnamePath, lastnamePath, result);
            PrintNames(nameList);
            break;

        case "2":

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