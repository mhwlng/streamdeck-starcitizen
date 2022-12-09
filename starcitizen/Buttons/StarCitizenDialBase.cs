using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using BarRaider.SdTools;

namespace starcitizen.Buttons
{
    public abstract class StarCitizenDialBase : EncoderBase
    {
        protected StarCitizenDialBase(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "aa");
        }

        public override void Dispose()
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "bb");
        }

        
        public override void OnTick()
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "dd");

            //var deviceInfo = Connection.DeviceInfo();

        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }


    }
}
