namespace System.IO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class InputListener
    {
        public delegate void KeyStateChangedDelegate(Keys key);

        /// <summary>
        /// An event that gets invokes when a key is pressed down
        /// </summary>
        public event KeyStateChangedDelegate KeyDown;

        /// <summary>
        /// An event that gets invokes when a key that was previously pressed down is now released
        /// </summary>
        public event KeyStateChangedDelegate KeyUp;

        private List<Keys> pressedKeys = new List<Keys>();

        /// <summary>
        /// An array of all currently pressed keys
        /// </summary>
        public Keys[] PressedKeys
        {
            get { return this.pressedKeys.ToArray(); }
        }

        public InputListener()
        {
            this.KeyDown += this.InputListener_KeyDown;
            this.KeyUp += this.InputListener_KeyUp;
        }

        /// <summary>
        /// Starts the listening for pressed keys
        /// </summary>
        public void Start()
        {
            Thread th = new Thread(this.Run);
            th.IsBackground = true;
            th.Priority = ThreadPriority.BelowNormal;
            th.Name = "InputListener Thread";
            th.Start();
        }

        private void Run()
        {
            while (true)
            {
                this.Tick();
                Thread.Sleep(50);
            }
        }

        internal void InputListener_KeyUp(Keys key)
        {
            try
            {
                this.pressedKeys.Remove(key);
            }
            catch (Exception)
            {
            }
        }

        private void InputListener_KeyDown(Keys key)
        {
            this.pressedKeys.Add(key);
        }

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int vKey);

        private static bool IsKeyPushedDown(Keys vKey)
        {
            return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
        }

        internal void Tick()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (IsKeyPushedDown(key))
                {
                    if (!this.pressedKeys.Contains(key))
                    {
                        KeyStateChangedDelegate temp = this.KeyDown;
                        if (temp != null)
                        {
                            temp(key);
                        }
                    }
                }
                else
                {
                    if (this.pressedKeys.Contains(key))
                    {
                        KeyStateChangedDelegate temp = this.KeyUp;
                        if (temp != null)
                        {
                            temp(key);
                        }
                    }
                }
            }
        }
    }
}
