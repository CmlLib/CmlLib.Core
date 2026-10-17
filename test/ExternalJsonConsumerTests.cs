using CmlLib.Core.ExternalJsonConsumer;

namespace CmlLib.Core.Test;

public class ExternalJsonConsumerTests
{
    [Fact]
    public void public_models_round_trip_with_an_external_generated_context()
    {
        PublicModelJsonConsumer.VerifyRoundTrips();
    }
}
