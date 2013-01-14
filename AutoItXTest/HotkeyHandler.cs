using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Utils;

namespace Hotkeys
{
    class HotkeyHandler
    {
        const int WM_HOTKEY = 0x0312;

        /// <summary>
        /// This code comes from http://www.codekeep.net/snippets/694d10f6-3aaa-4f6b-bb38-366c77fb5ec0.aspx
        /// This method of setting a hotkey CONSUMES the key, meaning it does not get passed down to Windows.
        /// </summary>

        public delegate void HotkeyEventHandler(Keys KeyCode, KeyModifiers KeyMods);            //This defines a delegate (pointer) called "EventHandler" which accepts various parameters

        /// <summary>
        /// Call with this.Handle.
        /// </summary>
        /// <param name="hWnd">Use this.Handle</param>
        public HotkeyHandler(IntPtr hWnd)
        {
            myID = 0x2500;      //Randomly chosen ID to start with.
            myhWnd = hWnd;
            HotkeyPairs = new HotkeyPair[0];
        }

        ~HotkeyHandler()
        {
            //Unregister all hotkeys on destruction of this class.
            //bool bcheck = UnregisterHotKey(Handle, HOTKEY_ID);
        }

        int myID;
        IntPtr myhWnd;

        struct HotkeyPair
        {
            public KeyModifiers KeyMods;
            public Keys KeyCode;
            public int ID;
            public HotkeyEventHandler eventHotkey;
            //TODO: Add an event handler here to clean up the interface (Override WndProc() in this file instead)
        }

        HotkeyPair[] HotkeyPairs;

        //API Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(
            IntPtr hWnd, // handle to window    
            int id, // hot key identifier    
            KeyModifiers fsModifiers, // key-modifier options    
            Keys vk    // virtual-key code    
            );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd, // handle to window    
            int id      // hot key identifier    
            );
        public enum KeyModifiers        //enum to call 3rd parameter of RegisterHotKey easily
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        /*
public enum Modifiers
{
    None = 0x00,
    LCtrl = 0x01,
    LShift = 0x02,
    LAlt = 0x04,
    LWindows = 0x08,
    RCtrl = 0x10,
    RShift = 0x20,
    RAlt = 0x40,
    RWindows = 0x80
}
*/

        /// <summary>
        /// Set a hotkey.  (Catch the hotkey in WndProc(Message msg) by checking msg.Msg == WM_HOTKEY)
        /// </summary>
        /// <param name="KeyMods">Modifier keys (Ctrl, Alt, Shift, Windows key)</param>
        /// <param name="KeyCode">Keycode</param>
        /// <returns></returns>
        public bool HotkeySet(KeyModifiers KeyMods, Keys KeyCode, HotkeyEventHandler eventHandler)
        {
            //Remove the key first:
            HotkeyUnset(KeyMods, KeyCode);

            //Add the new key pair to our hotkeypairs array:
            Array.Resize(ref HotkeyPairs, HotkeyPairs.Length+1);

            myID++;         //increment the ID value (this only has to be a unique value per hotkey pair) so we can use an index to remove keypairs.
            HotkeyPairs[HotkeyPairs.Length - 1].ID = myID;
            HotkeyPairs[HotkeyPairs.Length - 1].KeyMods = KeyMods;
            HotkeyPairs[HotkeyPairs.Length - 1].KeyCode = KeyCode;
            HotkeyPairs[HotkeyPairs.Length - 1].eventHotkey = eventHandler;
            //Register the new key:
            return (RegisterHotKey(myhWnd, myID++, KeyMods, KeyCode));
        }

        /// <summary>
        /// Delete a hotkey.
        /// </summary>
        /// <param name="KeyMods">Modifier keys (Ctrl, Alt, Shift)</param>
        /// <param name="KeyCode">Keycode</param>
        /// <returns></returns>
        public bool HotkeyUnset(KeyModifiers KeyMods, Keys KeyCode)
        {
            foreach (HotkeyPair pair in HotkeyPairs)
            {
                if (pair.KeyCode == KeyCode && pair.KeyMods == KeyMods)
                {
                    //We found a matching hotkey.  Remove it.
                    //Remove the PAIR also!
                    return UnregisterHotKey(myhWnd, pair.ID);
                }
            }
            return false;
        }

        /// <summary>
        /// Unregister all hotkeys
        /// </summary>
        public void RemoveAllHotkeys()
        {
            //Unregister all hotkeys:
            foreach (HotkeyPair pair in HotkeyPairs)
            {
                UnregisterHotKey(myhWnd, pair.ID);
            }
        }

        //Call this from within WndProc in the main form.
        public void WndProc(ref Message msg)
        {
            // Listen for operating system messages.
            if (msg.Msg == WM_HOTKEY)
            {
                byte[] LParam;
                Keys KeyCode;
                HotkeyHandler.KeyModifiers KeyMods;
                //bool CtrlKeyPressed = false;
                //bool ShiftKeyPressed = false;
                //bool AltKeyPressed = false;
                //bool WinKeyPressed = false;


                LParam = myUtils.IntToByteArray(msg.LParam.ToInt32());
                KeyCode = (Keys)(LParam[2] + LParam[3] * 256);        //I'm guessing this is a 16bit keycode?
                KeyMods = (HotkeyHandler.KeyModifiers)LParam[0];

                /*
                if (((int)KeyMods & (int)Hotkeys.KeyModifiers.Control) != 0) CtrlKeyPressed = true;
                if (((int)KeyMods & (int)Hotkeys.KeyModifiers.Shift) != 0) ShiftKeyPressed = true;
                if (((int)KeyMods & (int)Hotkeys.KeyModifiers.Alt) != 0) AltKeyPressed = true;
                if (((int)KeyMods & (int)Hotkeys.KeyModifiers.Windows) != 0) WinKeyPressed = true;

                string Mods = "";
                if (CtrlKeyPressed) Mods += "Ctrl ";
                if (ShiftKeyPressed) Mods += "Shift ";
                if (AltKeyPressed) Mods += "Alt ";
                if (WinKeyPressed) Mods += "Win ";
                */

                //MessageBox.Show("Hotkey pressed 0x" + myUtils.Dec2Hex(msg.LParam.ToInt32()) + " hotkey: " + Mods + KeyCode.ToString());
                //myHotkeyEvent(KeyCode, KeyMods);

                //Can this be done by ID instead?
                foreach (HotkeyPair pair in HotkeyPairs)
                {
                    if (pair.KeyCode == KeyCode && pair.KeyMods == KeyMods)
                    {
                        //We found the matching hotkey, so call it's corresponding event handler:
                        pair.eventHotkey(KeyCode, KeyMods);
                        return;
                    }
                }

                //This shouldn't happen
                Console.WriteLine("Couldn't find event handler for the hotkey pressed!");
            }
        }
    }
}
