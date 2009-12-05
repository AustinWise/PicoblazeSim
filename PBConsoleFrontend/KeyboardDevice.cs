using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Austin.PicoblazeSim;
using System.Windows.Forms;

namespace Austin.PBConsoleFrontend
{
    class KeyboardDevice : IHardwareDevice, IInterruptor
    {
        private Dictionary<Keys, byte> consoleKeyToScanCodes = new Dictionary<Keys, byte>();
        private Keys lastKeyPressed;

        public KeyboardDevice()
        {
            loadKeys();
        }

        public void OnKeyPress(Keys key)
        {
            lastKeyPressed = key;
            this.Interrupt(this, EventArgs.Empty);
        }

        private byte getKey()
        {
            var press = lastKeyPressed;
            if (consoleKeyToScanCodes.ContainsKey(lastKeyPressed))
                return consoleKeyToScanCodes[lastKeyPressed];
            else
                return 0;
        }

        public IDictionary<byte, Func<byte>> InputDevices
        {
            get
            {
                var dic = new Dictionary<byte, Func<byte>>();
                dic.Add(0x28, new Func<byte>(getKey));
                return dic;
            }
        }

        public IDictionary<byte, Action<byte>> OutputDevices
        {
            get { return new Dictionary<byte, Action<byte>>(); }
        }

        public event EventHandler Interrupt;

        private void loadKeys()
        {
            foreach (var line in Properties.Resources.KeysToScan.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] splitLine = line.Split(',');
                consoleKeyToScanCodes.Add((Keys)Enum.Parse(typeof(Keys), splitLine[0]), byte.Parse(splitLine[1], System.Globalization.NumberStyles.HexNumber));
            }
        }
    }
}
