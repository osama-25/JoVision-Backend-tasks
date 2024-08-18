using System.Text.Json.Serialization;

namespace Task47.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FilterType
    {
        ByModificationDate,
        ByCreationDateDescending,
        ByCreationDateAscending,
        ByOwner
    }
}
