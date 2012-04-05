using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace MusicBeePlugin.SkypeControl
{

    internal sealed partial class SkypeClient : NativeWindow
    {

        private readonly UInt32 _umSkypeControlAPIDiscover;
        private readonly UInt32 _umSkypeControlAPIAttach;


        private IntPtr _mySkypeHandle = IntPtr.Zero;

        public event SkypeAttachHandler SkypeAttach;
        public event SkypeResponseHandler SkypeResponse;

        public SkypeClient()
        {
            _umSkypeControlAPIDiscover = Platform.RegisterWindowMessage(Constants.SkypeControlAPIDiscover);
            _umSkypeControlAPIAttach = Platform.RegisterWindowMessage(Constants.SkypeControlAPIAttach);
            CreateParams createParams = new CreateParams {Height = 0, Width = 0};
            CreateHandle(createParams);
        }

        public bool Connect()
        {
            IntPtr result;
            IntPtr aRetVal = Platform.SendMessageTimeout(Platform.HWND_BROADCAST, _umSkypeControlAPIDiscover, Handle, IntPtr.Zero, Platform.SendMessageTimeoutFlags.SMTO_NORMAL, 100, out result);

            return (aRetVal != IntPtr.Zero);
        }

        public void Disconnect()
        {
            SendMessage(String.Empty);
            _mySkypeHandle = IntPtr.Zero;
        }

        public bool SendMessage(string messageData)
        {
            Platform.CopyDataStruct aCDS = new Platform.CopyDataStruct {ID = "1", Data = messageData};

            aCDS.Length = aCDS.Data.Length + 1;

            IntPtr result;
            IntPtr aRetVal = Platform.SendMessageTimeout(_mySkypeHandle, Platform.WM_COPYDATA, Handle, ref aCDS, Platform.SendMessageTimeoutFlags.SMTO_NORMAL, 100, out result);

            return (aRetVal != IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
           Debug.WriteLine(m.ToString());
            UInt32 aMsg = (UInt32)m.Msg;

            if (aMsg == _umSkypeControlAPIAttach)
            {
                SkypeAttachStatus anAttachStatus = (SkypeAttachStatus)m.LParam;

                if (anAttachStatus == SkypeAttachStatus.Success)
                    _mySkypeHandle = m.WParam;

                if (SkypeAttach != null)
                    SkypeAttach(this, new SkypeAttachEventArgs(anAttachStatus));

                m.Result = new IntPtr(1);
                return;
            }

            if (aMsg == Platform.WM_COPYDATA)
            {
                if (m.WParam == _mySkypeHandle)
                {
                    Platform.CopyDataStruct aCDS = (Platform.CopyDataStruct)m.GetLParam(typeof(Platform.CopyDataStruct));
                    string aResponse = aCDS.Data;

                    if (SkypeResponse != null)
                        SkypeResponse(this, new SkypeResponseEventArgs(aResponse));

                    m.Result = new IntPtr(1);
                    return;
                }
            }

            base.WndProc(ref m);
        }

    }
}