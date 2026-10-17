using System.Text.Json;
using System.Text.Json.Serialization;
using CmlLib.Core.Java;
using CmlLib.Core.Version;
using CmlLib.Core.VersionMetadata;

namespace CmlLib.Core.ExternalJsonConsumer;

// Core의 InternalsVisibleTo 대상이 아닌 어셈블리에서 공개 모델의 JSON 계약을 검증한다.
public static class PublicModelJsonConsumer
{
    private static readonly DateTimeOffset ReleaseTime = new(2023, 6, 7, 12, 0, 0, TimeSpan.Zero);

    public static void VerifyRoundTrips()
    {
        VerifyJavaVersion();
        VerifyVersion();
        VerifyManifest();
    }

    private static void VerifyJavaVersion()
    {
        const string json = """
            {"component":"java-runtime","majorVersion":17}
            """;
        var version = JsonSerializer.Deserialize(
            json,
            ConsumerJsonContext.Default.JavaVersion)
            ?? throw new InvalidOperationException("JavaVersion was null.");

        Check(version.MajorVersion == "17", "Java majorVersion must accept a number.");

        var serialized = JsonSerializer.Serialize(
            version,
            ConsumerJsonContext.Default.JavaVersion);
        var restored = JsonSerializer.Deserialize(
            serialized,
            ConsumerJsonContext.Default.JavaVersion);

        Check(restored == version, "JavaVersion must round-trip.");
    }

    private static void VerifyVersion()
    {
        const string json = """
            {
                "id":"1.20",
                "complianceLevel":1,
                "minimumLauncherVersion":21,
                "type":2,
                "javaVersion":{"component":"java-runtime","majorVersion":17.0},
                "releaseTime":"2023-06-07T12:00:00Z",
                "time":"invalid-date"
            }
            """;
        var version = JsonSerializer.Deserialize(
            json,
            ConsumerJsonContext.Default.JsonVersionDTO)
            ?? throw new InvalidOperationException("JsonVersionDTO was null.");

        Check(version.ComplianceLevel == "1", "ComplianceLevel must accept a number.");
        Check(version.MinimumLauncherVersion == "21", "MinimumLauncherVersion must accept a number.");
        Check(version.Type == "2", "Type must accept a number.");
        Check(
            version.JavaVersion?.MajorVersion == "17.0",
            "Nested JavaVersion must preserve decimal text.");
        Check(
            version.Time == DateTimeOffset.MinValue,
            "Invalid dates must retain the fallback behavior.");

        var serialized = JsonSerializer.Serialize(
            version,
            ConsumerJsonContext.Default.JsonVersionDTO);
        var restored = JsonSerializer.Deserialize(
            serialized,
            ConsumerJsonContext.Default.JsonVersionDTO);

        Check(restored?.Id == "1.20", "JsonVersionDTO must round-trip.");
        Check(restored?.ReleaseTime == ReleaseTime, "ReleaseTime must round-trip.");
        Check(
            restored?.JavaVersion == version.JavaVersion,
            "Nested JavaVersion must round-trip.");
    }

    private static void VerifyManifest()
    {
        const string json = """
            {"versions":[{"id":"1.20","time":"invalid-date","releaseTime":"2023-06-07T12:00:00Z"}]}
            """;
        var manifest = JsonSerializer.Deserialize(
            json,
            ConsumerJsonContext.Default.JsonVersionManifestModel)
            ?? throw new InvalidOperationException("JsonVersionManifestModel was null.");

        var metadata = manifest.Versions.Single();
        Check(
            metadata.Time == DateTimeOffset.MinValue,
            "Manifest dates must retain the fallback behavior.");

        var serialized = JsonSerializer.Serialize(
            manifest,
            ConsumerJsonContext.Default.JsonVersionManifestModel);
        var restored = JsonSerializer.Deserialize(
            serialized,
            ConsumerJsonContext.Default.JsonVersionManifestModel);

        Check(
            restored?.Versions.Single().ReleaseTime == ReleaseTime,
            "Manifest dates must round-trip.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

[JsonSerializable(typeof(JavaVersion))]
[JsonSerializable(typeof(JsonVersionDTO))]
[JsonSerializable(typeof(JsonVersionManifestModel))]
internal partial class ConsumerJsonContext : JsonSerializerContext
{
}
