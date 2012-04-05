using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using MusicBeePlugin.Events;



namespace MusicBeePlugin.SkypeControl
{
    internal sealed class SkypeConnector : Form
    {
        private static SkypeConnector _mInstance;

        private UInt32 _umSkypeControlAPIDiscover;
        private UInt32 _umSkypeControlAPIAttach;

        private IntPtr _mySkypeHandle = IntPtr.Zero;

        private SkypeConnector()
        {
        }

        public static void Start()
        {
#if DEBUG
            CheckForIllegalCrossThreadCalls = false;
#endif
            Thread thread = new Thread(RunSkypeClient);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Name = "SkypeConnector";
            thread.Start();

        }

        public static void Stop()
        {
            if (_mInstance == null) return;
            _mInstance.Invoke(new MethodInvoker(_mInstance.EndConnector));
        }

        private void EndConnector()
        {
            Close();
        }

        protected override void SetVisibleCore(bool value)
        {
            //Prevents window getting visible
            if (_mInstance == null) CreateHandle();
            _mInstance = this;
            value = false;
            _umSkypeControlAPIDiscover = Platform.RegisterWindowMessage(Constants.SkypeControlAPIDiscover);
            _umSkypeControlAPIAttach = Platform.RegisterWindowMessage(Constants.SkypeControlAPIAttach);
            EventDispatcher.Instance.InitiateConnection += InstanceInitiateConnection;
            EventDispatcher.Instance.InitiateDisconnect += InstanceInitiateDisconnect;
            EventDispatcher.Instance.MessageDataAvailable += InstanceMessageDataAvailable;
            base.SetVisibleCore(value);
        }

        private void InstanceMessageDataAvailable(object sender, MessageEventArgs e)
        {
            SendMessage(e.Message);
        }

        private void InstanceInitiateDisconnect(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void InstanceInitiateConnection(object sender, EventArgs e)
        {
            Connect();
        }

        private static void RunSkypeClient()
        {
            Application.Run(new SkypeConnector());
        }

        private bool Connect()
        {
            IntPtr result;
            IntPtr aRetVal = Platform.SendMessageTimeout(Platform.HWND_BROADCAST, _umSkypeControlAPIDiscover, Handle,
                                                         IntPtr.Zero, Platform.SendMessageTimeoutFlags.SMTO_NORMAL, 100,
                                                         out result);

            return (aRetVal != IntPtr.Zero);
        }

        private void Disconnect()
        {
            SendMessage(String.Empty);
            _mySkypeHandle = IntPtr.Zero;
        }

        private bool SendMessage(string messageData)
        {
            Platform.CopyDataStruct aCDS = new Platform.CopyDataStruct {ID = "1", Data = messageData};

            aCDS.Length = aCDS.Data.Length + 1;

            IntPtr result;
            IntPtr aRetVal = Platform.SendMessageTimeout(_mySkypeHandle, Platform.WM_COPYDATA, Handle,
                                                         ref aCDS,
                                                         Platform.SendMessageTimeoutFlags.SMTO_NORMAL, 100, out result);

            return (aRetVal != IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
            Debug.WriteLine(m.ToString());
            UInt32 aMsg = (UInt32) m.Msg;

            if (aMsg == _umSkypeControlAPIAttach)
            {
                SkypeAttachStatus anAttachStatus = (SkypeAttachStatus) m.LParam;

                if (anAttachStatus == SkypeAttachStatus.Success)
                    _mySkypeHandle = m.WParam;

                EventDispatcher.Instance.OnSkypeAttach(new SkypeAttachEventArgs(anAttachStatus));

                m.Result = new IntPtr(1);
                return;
            }

            if (aMsg == Platform.WM_COPYDATA)
            {
                if (m.WParam == _mySkypeHandle)
                {
                    Platform.CopyDataStruct aCDS =
                        (Platform.CopyDataStruct) m.GetLParam(typeof (Platform.CopyDataStruct));
                    string aResponse = aCDS.Data;

                    EventDispatcher.Instance.OnSkypeResponse(new SkypeResponseEventArgs(aResponse));

                    m.Result = new IntPtr(1);
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}