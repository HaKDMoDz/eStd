using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Creek.Extensions
{
    public class HotKeysManager : NativeWindow
    {
        private Form owner;
        private List<HotKeysData> registerHotKeys;

        /// <summary>
        /// Occurs when [hot key press].
        /// </summary>
        public event EventHandler<HotKeysEventArgs> HotKeyPress;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeysManager"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public HotKeysManager(Form owner) {
            this.AssignHandle(owner.Handle);
            this.owner = owner;
            this.registerHotKeys = new List<HotKeysData>();
            owner.HandleCreated += new EventHandler(owner_HandleCreated);
        }

        /// <summary>
        /// Handles the HandleCreated event of the owner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void owner_HandleCreated(object sender, EventArgs e)
        {
            this.AssignHandle(owner.Handle);
        }

        /// <summary>
        /// Adds the hot key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        public void AddHotKey(Keys key, HotKeyModifiers modifiers) {
            if (this.registerHotKeys.Contains(
                new HotKeysData { Key = key, Modifiers = modifiers },
                new HotKeyEqualityComparer())) {
                    return;
            }

            RegisterHotKeys(key, modifiers);



        }

        /// <summary>
        /// Removes the hot key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        public void RemoveHotKey(Keys key, HotKeyModifiers modifiers) {
            if (registerHotKeys.Contains(new HotKeysData { Key = key, Modifiers = modifiers }, new HotKeyEqualityComparer()))
            {
                var data = registerHotKeys.Find(hk => hk.Key == key && hk.Modifiers == modifiers);
                this.UnregisterHotKeys(data);
                registerHotKeys.Remove(data);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="HotKeysManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~HotKeysManager() { 
            this.registerHotKeys.ForEach(hk=>this.UnregisterHotKeys(hk));
        }

        /// <summary>
        /// Registers the hot keys.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        private void RegisterHotKeys(Keys key, HotKeyModifiers modifiers) {
            var atomName = string.Empty;
            IntPtr atomId;

            atomName = key.ToString() + modifiers.ToString() + Environment.TickCount.ToString();
            atomName = atomName.Substring(0, Math.Min(atomName.Length, 255));

            atomId = GlobalAddAtom(atomName);
            if (atomId == IntPtr.Zero) {
                throw new Exception("Unable to save shortcut atom !");
            }

            if (!RegisterHotKey(this.owner.Handle, atomId.ToInt32(), (int)modifiers, (int)key)) {
                GlobalDeleteAtom(atomId);
                throw new Exception("Unable to save shortcut !");
            }

            this.registerHotKeys.Add(new HotKeysData(key, modifiers, atomId));
        }

        /// <summary>
        /// Unregisters the hot keys.
        /// </summary>
        /// <param name="data">The data.</param>
        private void UnregisterHotKeys(HotKeysData data) {
            UnregisterHotKey(this.owner.Handle, data.AtomID.ToInt32());
            GlobalDeleteAtom(data.AtomID);
        }

        /// <summary>
        /// Invokes the default window procedure associated with this window.
        /// </summary>
        /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/> that is associated with the current Windows message.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY) {
                IntPtr wParam = m.WParam;

                var data = registerHotKeys.Where(hk => hk.AtomID.Equals(wParam));
                if (data.Any())
                {
                    this.OnHotKeysPress(this, new HotKeysEventArgs(data.First().Key, data.First().Modifiers));
                }
            }


        }

        /// <summary>
        /// Called when [hot keys press].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="HotKeysEventArgs"/> instance containing the event data.</param>
        public virtual void OnHotKeysPress(object sender, HotKeysEventArgs e) {
            if (this.HotKeyPress != null) {
                this.HotKeyPress(sender, e);
            }
        }

        #region HotKeysData
        private struct HotKeysData
        {
            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>
            /// The key.
            /// </value>
            public Keys Key;

            /// <summary>
            /// Gets or sets the modifiers.
            /// </summary>
            /// <value>
            /// The modifiers.
            /// </value>
            public HotKeyModifiers Modifiers;

            /// <summary>
            /// Gets or sets the atom ID.
            /// </summary>
            /// <value>
            /// The atom ID.
            /// </value>
            public IntPtr AtomID;

            public static HotKeysData Empty = new HotKeysData();

            /// <summary>
            /// Initializes a new instance of the <see cref="HotKeysData"/> struct.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="modifiers">The modifiers.</param>
            /// <param name="atomId">The atom id.</param>
            public HotKeysData(Keys key, HotKeyModifiers modifiers, IntPtr atomId)
            {
                this.Key = key;
                this.Modifiers = modifiers;
                this.AtomID = atomId;
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return Modifiers.ToString() + "+" + Key.ToString();
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                return this.AtomID.Equals(obj);
            }

        }
        #endregion

        #region HotKeyEqualityComparer
        private class HotKeyEqualityComparer : IEqualityComparer<HotKeysData> {

            /// <summary>
            /// Equalses the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns></returns>
            public bool Equals(HotKeysData x, HotKeysData y)
            {
                return x.Key == y.Key
                    && x.Modifiers == y.Modifiers;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(HotKeysData obj)
            {
                return obj.GetHashCode();
            }
        }
        #endregion

        #region P/Invoke
        private const int WM_HOTKEY = 0x312;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GlobalDeleteAtom(IntPtr nAtom);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

    }
}
