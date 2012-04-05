using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin.SkypeControl
{
    
    class ApiFilter:IMessageFilter
    {
        private IntPtr _mySkypeHandle = IntPtr.Zero;

        public event SkypeAttachHandler SkypeAttach;
        public event SkypeResponseHandler SkypeResponse;
        public bool PreFilterMessage(ref Message m)
        {
            UInt32 aMsg = (UInt32)m.Msg;

            if (aMsg == _umSkypeControlAPIAttach)
            {
                SkypeAttachStatus anAttachStatus = (SkypeAttachStatus)m.LParam;

                if (anAttachStatus == SkypeAttachStatus.Success)
                    _mySkypeHandle = m.WParam;

                if (SkypeAttach != null)
                    SkypeAttach(this, new SkypeAttachEventArgs(anAttachStatus));

                m.Result = new IntPtr(1);
                return true;
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
                    return true;
                }
            }
            return false;
        }
    }
}
