using System;

namespace Eir.Common.IO
{
    public interface IFakeFile : IFileInfo
    {
        byte[] ContentAsBytes { get; set; }

        string ContentAsUtf8String { get; set; }

        string ContentAsHex { get; set; }

        string ContentAsBase64 { get; set; }

        new DateTime LastWriteTimeUtc { get; set; }

        /// <summary>
        /// The Locked property is an additional "external" lock mechanism in addition to any actual open streams.
        /// </summary>
        bool Locked { get; set; }
    }
}