using System;
using MusicBeePlugin.SkypeControl;

namespace MusicBeePlugin.Events
{
    public sealed class EventDispatcher
    {
        private static readonly EventDispatcher ClassInstance = new EventDispatcher();

        private EventDispatcher()
        {
            // Private constructor to make the singleton inaccessible.
        }

        public event EventHandler InitiateConnection;
        public event EventHandler InitiateDisconnect;
        public event EventHandler<MessageEventArgs> MessageDataAvailable;
        public event SkypeAttachHandler SkypeAttach;
        public event SkypeResponseHandler SkypeResponse;

        public void OnSkypeAttach(SkypeAttachEventArgs eventargs)
        {
            SkypeAttachHandler handler = SkypeAttach;
            if (handler != null) handler(this, eventargs);
        }

        public void OnSkypeResponse(SkypeResponseEventArgs eventargs)
        {
            SkypeResponseHandler handler = SkypeResponse;
            if (handler != null) handler(this, eventargs);
        }

        public void OnInitiateConnection(Object sender, EventArgs e)
        {
            if (InitiateConnection != null) InitiateConnection(sender, e);
        }

        public void OnInitiateDisconnect(Object sender, EventArgs e)
        {
            if (InitiateDisconnect != null) InitiateDisconnect(sender, e);
        }

        public void OnMessageDataAvailable(Object sender, MessageEventArgs e)
        {
            if (MessageDataAvailable != null) MessageDataAvailable(sender, e);
        }

        public static EventDispatcher Instance
        {
            get { return ClassInstance; }
        }

    }
}
