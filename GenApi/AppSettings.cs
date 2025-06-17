namespace GenApi;

public class AppSettings
{
    public OpenSearchSettings? OpenSearch { get; set; }
}

public class OpenSearchSettings
{
    public string NodeUris { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
