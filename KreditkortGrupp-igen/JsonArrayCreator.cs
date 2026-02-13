using System.Text.Json;
using System.Text.Json.Serialization;

namespace KreditkortGrupp_igen;

public class JsonArrayCreator
{
    public List<Name> CreateNameList(string firstnamePath, string lastnamePath, int numberOfNames)
    {
        var jsonFirst = File.ReadAllText(firstnamePath);
        var jsonLast = File.ReadAllText(lastnamePath);
        var deserializeFirst = JsonSerializer.Deserialize<List<FirstName>>(jsonFirst);
        var deserializeLast = JsonSerializer.Deserialize<List<LastName>>(jsonLast);
        
        return MockNameGenerator(deserializeFirst, deserializeLast, numberOfNames);
    }

    public List<Name> MockNameGenerator(List<FirstName> firstNames, List<LastName> lastNames, int numberOfNames)
    {
        var random = new Random();
        
        return Enumerable.Range(0, numberOfNames)
            .Select(_ => new Name
            {
                FirstName = firstNames[random.Next(firstNames.Count)],
                LastName = lastNames[random.Next(lastNames.Count)]
            })
            .ToList();
    }
}

public class FirstName
{
    [JsonPropertyName("first_name")]
    public string FirstNameName { get; set; }
}

public class LastName
{
    [JsonPropertyName("last_name")]
    public string LastNameName { get; set; }
}

public class Name
{
    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }

    public override string ToString()
    {
        return $"{FirstName.FirstNameName} {LastName.LastNameName}";
    }
}
