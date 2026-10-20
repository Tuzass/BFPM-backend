using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Entities;

namespace API;

public class Utils
{
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false)
        }
    };
    
    public static string ChangeToJson(Change localChange)
    {
        return JsonSerializer.Serialize(localChange, JsonOptions);
    }

    public static Change? JsonToChange(string localChangeJSON)
    {
        return JsonSerializer.Deserialize<Change>(localChangeJSON, JsonOptions);
    }
}
