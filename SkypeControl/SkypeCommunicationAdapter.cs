using System.Diagnostics;
using MusicBeePlugin.Events;

namespace MusicBeePlugin.SkypeControl
{
    class SkypeCommunicationAdapter
    {
        private static SkypeCommunicationAdapter _instance;
        public event SkypeResponseHandler SkypeResponse; 
        public event SkypeAttachHandler SkypeAttach;

        private SkypeCommunicationAdapter()
        {
           SkypeConnector.Start();
           EventDispatcher.Instance.SkypeAttach += HandleSkypeAttach;
           EventDispatcher.Instance.SkypeResponse += HandleSkypeResponse;
        }

        public static SkypeCommunicationAdapter GetInstance()
        {
            return _instance ?? (_instance = new SkypeCommunicationAdapter());
        }

        private void HandleSkypeAttach(object theSender, SkypeAttachEventArgs theEventArgs)
        {
            if (SkypeAttach != null)
                SkypeAttach(this, theEventArgs);
        }

        void HandleSkypeResponse(object theSender, SkypeResponseEventArgs theEventArgs)
        {
            if (SkypeResponse != null)
                SkypeResponse(this, theEventArgs);
#if DEBUG
            Debug.WriteLine("RESP:" + theEventArgs.Response);
#endif
        }

        public void Connect()
        {
            EventDispatcher.Instance.OnInitiateConnection(this,null);
        }

        public void Disconnect()
        {
            EventDispatcher.Instance.OnInitiateDisconnect(this, null);
        }

        public void SendMessage(string message)
        {
            EventDispatcher.Instance.OnMessageDataAvailable(this, new MessageEventArgs(message));
        }
    }
}
