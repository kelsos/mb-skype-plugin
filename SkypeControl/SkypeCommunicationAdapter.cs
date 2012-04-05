using System;
using System.Diagnostics;

namespace MusicBeePlugin.SkypeControl
{
    class SkypeCommunicationAdapter
    {
        private readonly SkypeClient _mySkypeClient;
        private static SkypeCommunicationAdapter _instance;

        private SkypeCommunicationAdapter()
        {
            _mySkypeClient = new SkypeClient();
            _mySkypeClient.SkypeAttach += mySkypeClient_OnSkypeAttach;
            _mySkypeClient.SkypeResponse += MySkypeClientSkypeResponse;
        }


        public event SkypeAttachHandler SkypeAttach;
        public event SkypeResponseHandler SkypeResponse;

        public static SkypeCommunicationAdapter GetInstance()
        {
            return _instance ?? (_instance = new SkypeCommunicationAdapter());
        }

        private void mySkypeClient_OnSkypeAttach(object theSender, SkypeAttachEventArgs theEventArgs)
        {
            if (SkypeAttach != null)
                SkypeAttach(this, theEventArgs);
        }

        void MySkypeClientSkypeResponse(object theSender, SkypeResponseEventArgs theEventArgs)
        {
            if (SkypeResponse != null)
                SkypeResponse(this, theEventArgs);
            Debug.WriteLine("RESP:" + theEventArgs.Response);
        }

        public bool Connect()
        {
            return _mySkypeClient.Connect();
        }

        public void Disconnect()
        {
            _mySkypeClient.Disconnect();
        }

        public bool SendMessage(string message)
        {
            return _mySkypeClient.SendMessage(message);
        }
    }
}
