namespace CmlLib.Core.Auth
{
    public class MLoginResponse
    {
        public MLoginResponse(MLoginResult result, MSession? session, string? errormsg, string? rawresponse)
        {
            Result = result;
            Session = session;
            ErrorMessage = errormsg;
            RawResponse = rawresponse;
        }

        public MLoginResult Result { get; private set; }
        public MSession? Session { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? RawResponse { get; private set; }

        public bool IsSuccess => Result == MLoginResult.Success;
    }
}
