using System.Text.Json.Serialization;

namespace Shared.DTO;

public class AggregatesResultDTO
{
    [JsonPropertyName("v")]
    public double V { get; set; }

    [JsonPropertyName("vw")]
    public double Vw { get; set; }

    [JsonPropertyName("o")]
    public double O { get; set; }

    [JsonPropertyName("c")]
    public double C { get; set; }

    [JsonPropertyName("h")]
    public double H { get; set; }

    [JsonPropertyName("l")]
    public double L { get; set; }

    [JsonPropertyName("t")]
    public double T { get; set; }

    [JsonPropertyName("n")]
    public double N { get; set; }
}

public class AggregatesDTO
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; }

    [JsonPropertyName("queryCount")]
    public int QueryCount { get; set; }

    [JsonPropertyName("resultsCount")]
    public int ResultsCount { get; set; }

    [JsonPropertyName("adjusted")]
    public bool Adjusted { get; set; }

    [JsonPropertyName("results")]
    public List<AggregatesResultDTO> Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}