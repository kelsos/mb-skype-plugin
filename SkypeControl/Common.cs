using System;

namespace MusicBeePlugin.SkypeControl
{
    public class SkypeAttachEventArgs:EventArgs
    {
        public SkypeAttachStatus AttachStatus;

        public SkypeAttachEventArgs(SkypeAttachStatus attacheStatus)
        {
            AttachStatus = attacheStatus;
        }
    }

    public delegate void SkypeAttachHandler(object sender, SkypeAttachEventArgs eventArgs);

    public class SkypeResponseEventArgs:EventArgs
    {
        public string Response;

        public  SkypeResponseEventArgs(string response)
        {
            Response = response;
        }
    }

    public delegate void SkypeResponseHandler(object sender, SkypeResponseEventArgs eventArgs);

    public enum SkypeAttachStatus:uint
    {
        Success = 0,
        PendingAuthorization = 1,
        Refused = 2,
        NotAvailable = 3,
        Available = 0x8001
    }
    
    internal class Constants
    {
        public const string SkypeControlAPIDiscover = "SkypeControlAPIDiscover";
        public const string SkypeControlAPIAttach = "SkypeControlAPIAttach";
    }
}
