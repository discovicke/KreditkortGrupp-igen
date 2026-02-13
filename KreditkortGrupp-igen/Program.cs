
using KreditkortGrupp_igen;

var firstnamePath = "MOCK_DATA_first_name.json";
var lastnamePath = "MOCK_DATA-last_name.json";

var arrayCreator = new JsonArrayCreator();
var nameList = arrayCreator.CreateNameList(firstnamePath, lastnamePath, 100);

foreach (var name in nameList)
{
    Console.WriteLine(name.ToString());
}