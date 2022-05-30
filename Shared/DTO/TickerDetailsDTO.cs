using System.Text.Json.Serialization;

namespace Shared.DTO;

public class TickerDetailsDTO
{
    [JsonPropertyName("results")] 
    public TickersDetailsResultsDTO Results { get; set; }
    [JsonPropertyName("request_id")] 
    public string RequestId { get; set; }
    
    [JsonPropertyName("status")] 
    public string Status { get; set; }
}

public class TickersDetailsResultsDTO
{
    [JsonPropertyName("active")] public bool Active { get; set; }
    [JsonPropertyName("address")] public AddressDTO Address { get; set; }
    [JsonPropertyName("branding")] public BrandingDTO Branding { get; set; }
    [JsonPropertyName("cik")] public string Cik { get; set; }
    [JsonPropertyName("composite_figi")] public string CompositeFigi { get; set; }
    [JsonPropertyName("currency_name")] public string CurrencyName { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("homepage_url")] public Uri HomepageUrl { get; set; }
    [JsonPropertyName("list_date")] public DateTimeOffset ListDate { get; set; }
    [JsonPropertyName("locale")] public string Locale { get; set; }
    [JsonPropertyName("market")] public string Market { get; set; }
    [JsonPropertyName("market_cap")] public double MarketCap { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("phone_number")] public string PhoneNumber { get; set; }
    [JsonPropertyName("primary_exchange")] public string PrimaryExchange { get; set; }
    [JsonPropertyName("share_class_figi")] public string ShareClassFigi { get; set; }

    [JsonPropertyName("share_class_shares_outstanding")]
    public long ShareClassSharesOutstanding { get; set; }

    [JsonPropertyName("sic_code")] public string SicCode { get; set; }
    [JsonPropertyName("sic_description")] public string SicDescription { get; set; }
    [JsonPropertyName("ticker")] public string Ticker { get; set; }
    [JsonPropertyName("ticker_root")] public string TickerRoot { get; set; }
    [JsonPropertyName("total_employees")] public long TotalEmployees { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("weighted_shares_outstanding")]
    public long WeightedSharesOutstanding { get; set; }
}

public class AddressDTO
{
    [JsonPropertyName("address1")] public string Address1 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; }
    [JsonPropertyName("postal_code")] public string PostalCode { get; set; }
    [JsonPropertyName("state")] public string State { get; set; }
}

public class BrandingDTO
{
    [JsonPropertyName("icon_url")] public Uri IconUrl { get; set; }
    [JsonPropertyName("logo_url")] public Uri LogoUrl { get; set; }
}