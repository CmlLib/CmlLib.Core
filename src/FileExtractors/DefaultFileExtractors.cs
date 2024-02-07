using CmlLib.Core.Java;
using CmlLib.Core.Rules;
using CmlLib.Core.Version;

namespace CmlLib.Core.FileExtractors;

public class DefaultFileExtractors
{
    public static DefaultFileExtractors CreateDefault(
        HttpClient httpClient,
        IRulesEvaluator rulesEvaluator,
        IJavaPathResolver javaPathResolver)
    {
        var extractors = new DefaultFileExtractors();
        extractors.Library = new LibraryFileExtractor(JsonVersionParserOptions.ClientSide, rulesEvaluator);
        extractors.Asset = new AssetFileExtractor(httpClient);
        extractors.Client = new ClientFileExtractor();
        extractors.Java = new JavaFileExtractor(httpClient, javaPathResolver);
        extractors.Log = new LogFileExtractor();
        return extractors;
    }

    public LibraryFileExtractor? Library { get; set; }
    public AssetFileExtractor? Asset { get; set; }
    public ClientFileExtractor? Client { get; set; }
    public JavaFileExtractor? Java { get; set; }
    public LogFileExtractor? Log { get; set; }
    public IEnumerable<IFileExtractor> ExtraExtractors { get; set; } = 
        Enumerable.Empty<IFileExtractor>();

    public FileExtractorCollection ToExtractorCollection()
    {
        var extractors = new FileExtractorCollection();
        if (Library != null)
            extractors.Add(Library);
        if (Asset != null)
            extractors.Add(Asset);
        if (Client != null)
            extractors.Add(Client);
        if (Log != null)
            extractors.Add(Log);
        if (Java != null)
            extractors.Add(Java);
        extractors.AddRange(ExtraExtractors);
        return extractors;
    }
}