﻿namespace CmlLib.Core.VersionMetadata;

public class MojangVersionMetadata : JsonVersionMetadata
{
    private HttpClient _httpClient;
    
    public string Url { get; }

    public MojangVersionMetadata(JsonVersionMetadataModel model, HttpClient httpClient) : base(model)
    {
        _httpClient = httpClient;
        IsSaved = false;

        if (string.IsNullOrEmpty(model.Url))
            throw new ArgumentNullException(nameof(model.Url));

        Url = model.Url;
    }

    protected override async ValueTask<Stream> GetVersionJsonStream()
    {
        return await _httpClient.GetStreamAsync(Url)
            .ConfigureAwait(false);
    }
}