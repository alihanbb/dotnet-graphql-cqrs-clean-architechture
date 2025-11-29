namespace GraphQl.Infrastructure.Search;

public class ElasticsearchSettings
{
    public string Url { get; set; } = "http://localhost:9200";
    public string ProductIndex { get; set; } = "products";
}
