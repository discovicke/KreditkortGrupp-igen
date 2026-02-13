

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

using KreditkortGrupp_igen;

var firstnamePath = "MOCK_DATA_first_name.json";
var lastnamePath = "MOCK_DATA-last_name.json";

var arrayCreator = new JsonArrayCreator();
var nameList = arrayCreator.CreateNameList(firstnamePath, lastnamePath, 100);

foreach (var name in nameList)
{
    Console.WriteLine(name.ToString());
}