

string[] menu = [
    "1. Generate mock data?",
    "2. List people?",
    "3. List Cards?",
    "4. Exit"
];

bool run = true;

Console.WriteLine("|=== What do you want to do? ===|\n");
while (run)
{
    foreach(var alternativ in menu)
    {
        Console.WriteLine(alternativ);
    }

        string? val = Console.ReadLine();
    switch (val)
    {
        case "1":

        break;

        case "2":

        break;

        case "3":

        break;

        case "4":
            Console.WriteLine("Terminating the program");
            run = false;
        break;
    }
}