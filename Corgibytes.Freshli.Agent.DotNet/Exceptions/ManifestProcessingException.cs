using System.Runtime.Serialization;

namespace Corgibytes.Freshli.Agent.DotNet.Exceptions;

[Serializable]
public class ManifestProcessingException : Exception
{
    public string? Details { get; set; }

    public ManifestProcessingException()
    {
    }

    protected ManifestProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ManifestProcessingException(string? message) : base(message)
    {
    }

    public ManifestProcessingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ManifestProcessingException(string? message, string? details) :
        base(message) => Details = details;
}
