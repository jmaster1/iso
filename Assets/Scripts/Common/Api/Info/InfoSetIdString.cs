using Common.Lang;

namespace Common.Api.Info
{
    /// <summary>
    /// InfoSetIdGeneric extension with id of type string
    /// </summary>
    public class InfoSetIdString<T> : InfoSetIdGeneric<T, string> where T : IIdAware<string>
    {
    }
}