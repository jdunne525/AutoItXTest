using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace KeyboardHookHandler
{
    class KeyboardMouseHook
    {
        /// <summary>
        /// This is a keyboard and mouse hook which provides an event handler for every keyboard or mouse event.
        /// Base code used for this class:
        /// http://blogs.msdn.com/b/toub/archive/2006/05/03/589423.aspx
        /// and this one:
        /// http://blogs.msdn.com/b/toub/archive/2006/05/03/589468.aspx
        /// Also some from BabySmash source code by Scott Hanselman.
        /// http://babysmash.codeplex.com/
        /// </summary>
        public enum ModifierEvent
        {
            None,
            LCtrlDown,
            LCtrlUp,
            LShiftDown,
            LShiftUp,
            LAltDown,
            LAltUp,
            LWindowsDown,
            LWindowsUp,
            RCtrlDown,
            RCtrlUp,
            RShiftDown,
            RShiftUp,
            RAltDown,
            RAltUp,
            RWindowsDown,
            RWindowsUp,
            MouseLButtonDown,
            MouseLButtonUp,
            MouseMove,
            MouseWheelDown,
            MouseWheelUp,
            MouseRButtonDown,
            MouseRButtonUp,
        }

        public enum KeyModifiers
        {
            None = 0x00,
            LCtrl = 0x01,
            LShift = 0x02,
            LAlt = 0x04,
            LWindows = 0x08,
            RCtrl = 0x10,
            RShift = 0x20,
            RAlt = 0x40,
            RWindows = 0x80,
        }

        public KeyModifiers Modifiers;

        #region Delegates

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void KeyboardEventHandler(AllKeys KeyCode, ModifierEvent e, System.Drawing.Point pt);            //This defines a delegate (pointer) called "EventHandler" which accepts a string parameter
        KeyboardEventHandler myEventKeyOrMouse;

        #endregion

        #region All Keys
        // Summary:
        //     Specifies key codes and modifiers.
        [Flags]
        public enum AllKeys
        {
            // Summary:
            //     The bitmask to extract modifiers from a key value.
            Modifiers = -65536,
            //
            // Summary:
            //     No key pressed.
            None = 0,
            //
            // Summary:
            //     The left mouse button.
            LButton = 1,
            //
            // Summary:
            //     The right mouse button.
            RButton = 2,
            //
            // Summary:
            //     The CANCEL key.
            Cancel = 3,
            //
            // Summary:
            //     The middle mouse button (three-button mouse).
            MButton = 4,
            //
            // Summary:
            //     The first x mouse button (five-button mouse).
            XButton1 = 5,
            //
            // Summary:
            //     The second x mouse button (five-button mouse).
            XButton2 = 6,
            //
            // Summary:
            //     The BACKSPACE key.
            BackSpace = 8,
            //
            // Summary:
            //     The TAB key.
            Tab = 9,
            //
            // Summary:
            //     The LINEFEED key.
            LineFeed = 10,
            //
            // Summary:
            //     The CLEAR key.
            Clear = 12,
            //
            // Summary:
            //     The ENTER key.
            Enter = 13,
            //
            // Summary:
            //     The RETURN key.
            Return = 13,
            //
            // Summary:
            //     The SHIFT key.
            ShiftKey = 16,
            //
            // Summary:
            //     The CTRL key.
            ControlKey = 17,
            //
            // Summary:
            //     The ALT key.
            AltKey = 18,
            //
            // Summary:
            //     The PAUSE key.
            Pause = 19,
            //
            // Summary:
            //     The CAPS LOCK key.
            CapsLock = 20,
            //
            // Summary:
            //     The IME Kana mode key.
            KanaMode = 21,
            //
            // Summary:
            //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
            HanguelMode = 21,
            //
            // Summary:
            //     The IME Hangul mode key.
            HangulMode = 21,
            //
            // Summary:
            //     The IME Junja mode key.
            JunjaMode = 23,
            //
            // Summary:
            //     The IME final mode key.
            FinalMode = 24,
            //
            // Summary:
            //     The IME Kanji mode key.
            KanjiMode = 25,
            //
            // Summary:
            //     The IME Hanja mode key.
            HanjaMode = 25,
            //
            // Summary:
            //     The ESC key.
            Escape = 27,
            //
            // Summary:
            //     The IME convert key.
            IMEConvert = 28,
            //
            // Summary:
            //     The IME nonconvert key.
            IMENonconvert = 29,
            //
            // Summary:
            //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
            IMEAceept = 30,
            //
            // Summary:
            //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
            IMEAccept = 30,
            //
            // Summary:
            //     The IME mode change key.
            IMEModeChange = 31,
            //
            // Summary:
            //     The SPACEBAR key.
            Space = 32,
            //
            // Summary:
            //     The PAGE UP key.
            PageUp = 33,
            //
            // Summary:
            //     The PAGE DOWN key.
            PageDown = 34,
            //
            // Summary:
            //     The END key.
            End = 35,
            //
            // Summary:
            //     The HOME key.
            Home = 36,
            //
            // Summary:
            //     The LEFT ARROW key.
            Left = 37,
            //
            // Summary:
            //     The UP ARROW key.
            Up = 38,
            //
            // Summary:
            //     The RIGHT ARROW key.
            Right = 39,
            //
            // Summary:
            //     The DOWN ARROW key.
            Down = 40,
            //
            // Summary:
            //     The SELECT key.
            Select = 41,
            //
            // Summary:
            //     The PRINT key.
            Print = 42,
            //
            // Summary:
            //     The EXECUTE key.
            Execute = 43,
            //
            // Summary:
            //     The PRINT SCREEN key.
            PrintScreen = 44,
            //
            // Summary:
            //     The INS key.
            Insert = 45,
            //
            // Summary:
            //     The DEL key.
            Del = 46,
            //
            // Summary:
            //     The HELP key.
            Help = 47,
            //
            // Summary:
            //     The 0 key.
            D0 = 48,
            //
            // Summary:
            //     The 1 key.
            D1 = 49,
            //
            // Summary:
            //     The 2 key.
            D2 = 50,
            //
            // Summary:
            //     The 3 key.
            D3 = 51,
            //
            // Summary:
            //     The 4 key.
            D4 = 52,
            //
            // Summary:
            //     The 5 key.
            D5 = 53,
            //
            // Summary:
            //     The 6 key.
            D6 = 54,
            //
            // Summary:
            //     The 7 key.
            D7 = 55,
            //
            // Summary:
            //     The 8 key.
            D8 = 56,
            //
            // Summary:
            //     The 9 key.
            D9 = 57,
            //
            // Summary:
            //     The A key.
            A = 65,
            //
            // Summary:
            //     The B key.
            B = 66,
            //
            // Summary:
            //     The C key.
            C = 67,
            //
            // Summary:
            //     The D key.
            D = 68,
            //
            // Summary:
            //     The E key.
            E = 69,
            //
            // Summary:
            //     The F key.
            F = 70,
            //
            // Summary:
            //     The G key.
            G = 71,
            //
            // Summary:
            //     The H key.
            H = 72,
            //
            // Summary:
            //     The I key.
            I = 73,
            //
            // Summary:
            //     The J key.
            J = 74,
            //
            // Summary:
            //     The K key.
            K = 75,
            //
            // Summary:
            //     The L key.
            L = 76,
            //
            // Summary:
            //     The M key.
            M = 77,
            //
            // Summary:
            //     The N key.
            N = 78,
            //
            // Summary:
            //     The O key.
            O = 79,
            //
            // Summary:
            //     The P key.
            P = 80,
            //
            // Summary:
            //     The Q key.
            Q = 81,
            //
            // Summary:
            //     The R key.
            R = 82,
            //
            // Summary:
            //     The S key.
            S = 83,
            //
            // Summary:
            //     The T key.
            T = 84,
            //
            // Summary:
            //     The U key.
            U = 85,
            //
            // Summary:
            //     The V key.
            V = 86,
            //
            // Summary:
            //     The W key.
            W = 87,
            //
            // Summary:
            //     The X key.
            X = 88,
            //
            // Summary:
            //     The Y key.
            Y = 89,
            //
            // Summary:
            //     The Z key.
            Z = 90,
            //
            // Summary:
            //     The left Windows logo key (Microsoft Natural Keyboard).
            LWin = 91,
            //
            // Summary:
            //     The right Windows logo key (Microsoft Natural Keyboard).
            RWin = 92,
            //
            // Summary:
            //     The application key (Microsoft Natural Keyboard).
            Apps = 93,
            //
            // Summary:
            //     The computer sleep key.
            Sleep = 95,
            //
            // Summary:
            //     The 0 key on the numeric keypad.
            NumPad0 = 96,
            //
            // Summary:
            //     The 1 key on the numeric keypad.
            NumPad1 = 97,
            //
            // Summary:
            //     The 2 key on the numeric keypad.
            NumPad2 = 98,
            //
            // Summary:
            //     The 3 key on the numeric keypad.
            NumPad3 = 99,
            //
            // Summary:
            //     The 4 key on the numeric keypad.
            NumPad4 = 100,
            //
            // Summary:
            //     The 5 key on the numeric keypad.
            NumPad5 = 101,
            //
            // Summary:
            //     The 6 key on the numeric keypad.
            NumPad6 = 102,
            //
            // Summary:
            //     The 7 key on the numeric keypad.
            NumPad7 = 103,
            //
            // Summary:
            //     The 8 key on the numeric keypad.
            NumPad8 = 104,
            //
            // Summary:
            //     The 9 key on the numeric keypad.
            NumPad9 = 105,
            //
            // Summary:
            //     The multiply key.
            NumpadMult = 106,
            //
            // Summary:
            //     The add key.
            NumpadAdd = 107,
            //
            // Summary:
            //     The separator key.
            Separator = 108,
            //
            // Summary:
            //     The subtract key.
            NumpadSub = 109,
            //
            // Summary:
            //     The decimal key.
            NumpadDot = 110,
            //
            // Summary:
            //     The divide key.
            NumpadDiv = 111,
            //
            // Summary:
            //     The F1 key.
            F1 = 112,
            //
            // Summary:
            //     The F2 key.
            F2 = 113,
            //
            // Summary:
            //     The F3 key.
            F3 = 114,
            //
            // Summary:
            //     The F4 key.
            F4 = 115,
            //
            // Summary:
            //     The F5 key.
            F5 = 116,
            //
            // Summary:
            //     The F6 key.
            F6 = 117,
            //
            // Summary:
            //     The F7 key.
            F7 = 118,
            //
            // Summary:
            //     The F8 key.
            F8 = 119,
            //
            // Summary:
            //     The F9 key.
            F9 = 120,
            //
            // Summary:
            //     The F10 key.
            F10 = 121,
            //
            // Summary:
            //     The F11 key.
            F11 = 122,
            //
            // Summary:
            //     The F12 key.
            F12 = 123,
            //
            // Summary:
            //     The F13 key.
            F13 = 124,
            //
            // Summary:
            //     The F14 key.
            F14 = 125,
            //
            // Summary:
            //     The F15 key.
            F15 = 126,
            //
            // Summary:
            //     The F16 key.
            F16 = 127,
            //
            // Summary:
            //     The F17 key.
            F17 = 128,
            //
            // Summary:
            //     The F18 key.
            F18 = 129,
            //
            // Summary:
            //     The F19 key.
            F19 = 130,
            //
            // Summary:
            //     The F20 key.
            F20 = 131,
            //
            // Summary:
            //     The F21 key.
            F21 = 132,
            //
            // Summary:
            //     The F22 key.
            F22 = 133,
            //
            // Summary:
            //     The F23 key.
            F23 = 134,
            //
            // Summary:
            //     The F24 key.
            F24 = 135,
            //
            // Summary:
            //     The NUM LOCK key.
            NumLock = 144,
            //
            // Summary:
            //     The SCROLL LOCK key.
            ScrollLock = 145,
            //
            // Summary:
            //     The left SHIFT key.
            LShiftKey = 160,
            //
            // Summary:
            //     The right SHIFT key.
            RShiftKey = 161,
            //
            // Summary:
            //     The left CTRL key.
            LControlKey = 162,
            //
            // Summary:
            //     The right CTRL key.
            RControlKey = 163,
            //
            // Summary:
            //     The left ALT key.
            LAltKey = 164,
            //
            // Summary:
            //     The right ALT key.
            RAltKey = 165,
            //
            // Summary:
            //     The browser back key (Windows 2000 or later).
            BrowserBack = 166,
            //
            // Summary:
            //     The browser forward key (Windows 2000 or later).
            BrowserForward = 167,
            //
            // Summary:
            //     The browser refresh key (Windows 2000 or later).
            BrowserRefresh = 168,
            //
            // Summary:
            //     The browser stop key (Windows 2000 or later).
            BrowserStop = 169,
            //
            // Summary:
            //     The browser search key (Windows 2000 or later).
            BrowserSearch = 170,
            //
            // Summary:
            //     The browser favorites key (Windows 2000 or later).
            BrowserFavorites = 171,
            //
            // Summary:
            //     The browser home key (Windows 2000 or later).
            BrowserHome = 172,
            //
            // Summary:
            //     The volume mute key (Windows 2000 or later).
            VolumeMute = 173,
            //
            // Summary:
            //     The volume down key (Windows 2000 or later).
            VolumeDown = 174,
            //
            // Summary:
            //     The volume up key (Windows 2000 or later).
            VolumeUp = 175,
            //
            // Summary:
            //     The media next track key (Windows 2000 or later).
            MediaNextTrack = 176,
            //
            // Summary:
            //     The media previous track key (Windows 2000 or later).
            MediaPreviousTrack = 177,
            //
            // Summary:
            //     The media Stop key (Windows 2000 or later).
            MediaStop = 178,
            //
            // Summary:
            //     The media play pause key (Windows 2000 or later).
            MediaPlayPause = 179,
            //
            // Summary:
            //     The launch mail key (Windows 2000 or later).
            LaunchMail = 180,
            //
            // Summary:
            //     The select media key (Windows 2000 or later).
            SelectMedia = 181,
            //
            // Summary:
            //     The start application one key (Windows 2000 or later).
            LaunchApplication1 = 182,
            //
            // Summary:
            //     The start application two key (Windows 2000 or later).
            LaunchApplication2 = 183,
            //
            // Summary:
            //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
            Semicolon = 186,
            //
            // Summary:
            //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
            Plus = 187,
            //
            // Summary:
            //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
            Comma = 188,
            //
            // Summary:
            //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
            Minus = 189,
            //
            // Summary:
            //     The OEM period key on any country/region keyboard (Windows 2000 or later).
            Period = 190,
            //
            // Summary:
            //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
            Question = 191,
            //
            // Summary:
            //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
            Tilde = 192,
            //
            // Summary:
            //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
            OpenBrackets = 219,
            //
            // Summary:
            //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
            BackSlash = 220,
            //
            // Summary:
            //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
            CloseBrackets = 221,
            //
            // Summary:
            //     The OEM singled/double quote key on a US standard keyboard (Windows 2000
            //     or later).
            Quotes = 222,
            //
            // Summary:
            //     The OEM 8 key.
            Oem8 = 223,
            //
            // Summary:
            //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows
            //     2000 or later).
            Backslash = 226,
            //
            // Summary:
            //     The PROCESS KEY key.
            ProcessKey = 229,
            //
            // Summary:
            //     Used to pass Unicode characters as if they were keystrokes. The Packet key
            //     value is the low word of a 32-bit virtual-key value used for non-keyboard
            //     input methods.
            Packet = 231,
            //
            // Summary:
            //     The ATTN key.
            Attn = 246,
            //
            // Summary:
            //     The CRSEL key.
            Crsel = 247,
            //
            // Summary:
            //     The EXSEL key.
            Exsel = 248,
            //
            // Summary:
            //     The ERASE EOF key.
            EraseEof = 249,
            //
            // Summary:
            //     The PLAY key.
            Play = 250,
            //
            // Summary:
            //     The ZOOM key.
            Zoom = 251,
            //
            // Summary:
            //     A constant reserved for future use.
            NoName = 252,
            //
            // Summary:
            //     The PA1 key.
            Pa1 = 253,
            //
            // Summary:
            //     The CLEAR key.
            OemClear = 254,
            //
            // Summary:
            //     The bitmask to extract a key code from a key value.
            KeyCode = 65535,
            //
            // Summary:
            //     The SHIFT modifier key.
            Shift = 0x10000,
            //
            // Summary:
            //     The CTRL modifier key.
            Control = 0x20000,
            //
            // Summary:
            //     The ALT modifier key.
            Alt = 0x40000,
        }
        #endregion

        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        public const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        public const int WH_MOUSE = 7;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        public const int WH_KEYBOARD = 2;

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        public const int WM_MOUSEMOVE = 0x200;

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        public const int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        public const int WM_LBUTTONUP = 0x202;

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        public const int WM_RBUTTONUP = 0x205;

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        public const int WM_MBUTTONUP = 0x208;

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        public const int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        public const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        public const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        public const int WM_KEYUP = 0x101;

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        public const int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        public const int WM_SYSKEYUP = 0x105;

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CAPITAL = 0x14;
        public const byte VK_NUMLOCK = 0x90;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        private IntPtr _KeyboardhookID;
        private IntPtr _MousehookID;
        private readonly LowLevelKeyboardProc _Keyboardproc;
        private readonly LowLevelMouseProc _Mouseproc;


        private IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public string KeyCodeToString(AllKeys KeyCode)
        {
            string Character = "";

            switch ((int)KeyCode)
            {
                case 0x20:
                    Character = " ";
                    break;
                case 0xDB:
                    Character = "[";
                    break;
                case 0xDD:
                    Character = "]";
                    break;
                case 0xDC:
                    Character = "\\";
                    break;
                case 0xBA:
                    Character = ";";
                    break;
                case 0xDE:
                    Character = "'";
                    break;
                case 0xBF:
                    Character = "/";
                    break;
                case 0xBE:
                    Character = ".";
                    break;
                case 0xBC:
                    Character = ",";
                    break;
                case 0xBD:
                    Character = "-";
                    break;
                case 0xBB:
                    Character = "=";
                    break;
                case 0xC0:
                    Character = "`";
                    break;
                case 0x31:
                    Character = "1";
                    break;
                case 0x32:
                    Character = "2";
                    break;
                case 0x33:
                    Character = "3";
                    break;
                case 0x34:
                    Character = "4";
                    break;
                case 0x35:
                    Character = "5";
                    break;
                case 0x36:
                    Character = "6";
                    break;
                case 0x37:
                    Character = "7";
                    break;
                case 0x38:
                    Character = "8";
                    break;
                case 0x39:
                    Character = "9";
                    break;
                case 0x30:
                    Character = "0";
                    break;
                default:
                    Character = KeyCode.ToString();
                    break;
            }

            if (Character.Length > 1)
            {
                Character = "{" + Character + "}";
            }
            return Character;
        }

        public KeyboardMouseHook(KeyboardEventHandler eventKeyDown)
        {
            //Create an event using a delegate to fire whenever data is received by the hook
            myEventKeyOrMouse = eventKeyDown;
            _Keyboardproc = KeyboardHookCallback;
            _KeyboardhookID = SetKeyboardHook(_Keyboardproc);

            _Mouseproc = MouseHookCallback;
            _MousehookID = SetMouseHook(_Mouseproc);

            Modifiers = KeyModifiers.None;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                                                   IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //See http://msdn.microsoft.com/en-us/library/windows/desktop/ms644970%28v=vs.85%29.aspx

            //WM_LBUTTONDOWN = 0x0201,
            //WM_LBUTTONUP = 0x0202,
            //WM_MOUSEMOVE = 0x0200,
            //WM_MOUSEWHEEL = 0x020A,
            //WM_RBUTTONDOWN = 0x0204,
            //WM_RBUTTONUP = 0x0205

                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                switch ((MouseMessages)wParam) 
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        //Console.WriteLine("Mouse click at:" + hookStruct.pt.x + ", " + hookStruct.pt.y);
                        myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseLButtonDown, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        break;
                    case MouseMessages.WM_LBUTTONUP:
                        myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseLButtonUp, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        //For now IGNORE mousemove as it will generate MANY events...
                        //myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseMove, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        break;
                    case MouseMessages.WM_MOUSEWHEEL:

                        //The high 16 its of mouseData contains the movement data of the wheel.  One wheel click is defined as WHEEL_DELTA, which is 120.
                        //If the mouse wheel is moved DOWN, this is identified by the value -120
                        if (hookStruct.mouseData == 0x00780000)
                        {
                            myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseWheelUp, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        }
                        else if (hookStruct.mouseData == 0xff880000)
                        {
                            myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseWheelDown, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        }
                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseRButtonDown, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        break;
                    case MouseMessages.WM_RBUTTONUP:
                        myEventKeyOrMouse(AllKeys.None, ModifierEvent.MouseRButtonUp, new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y));
                        break;

                }


            }
            return CallNextHookEx(_MousehookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //int wParam;
            if (nCode >= 0)
            {
                //bool Alt = (Control.ModifierKeys & Keys.Alt) != 0;
                //bool Control = (Control.ModifierKeys & Keys.Control) != 0;

                //Prevent ALT-TAB and CTRL-ESC by eating TAB and ESC. Also kill Windows Keys.
                int vkCode = Marshal.ReadInt32(lParam);
                AllKeys key = (AllKeys)vkCode;

                //At this point we have captured every key possible using a keyboard hook.
                //We can now do whatever we want with this information including CONSUME the keys.
                //It appears I'm getting 2 events, one for press, another for hold?

                //CONSUME the key (make it so windows never sees the key):
                //return (IntPtr)1; //handled

                //Pass the key on down (allow the key to be "pressed"):
                //return InterceptKeys.CallNextHookEx(_hookID, nCode, wParam, lParam);

                if ((int)wParam == WM_KEYDOWN || (int)wParam == WM_SYSKEYDOWN)
                {
                    if (key == AllKeys.LControlKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.LCtrl) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.LCtrl);
                            myEventKeyOrMouse(key, ModifierEvent.LCtrlDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LAltKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.LAlt) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.LAlt);
                            myEventKeyOrMouse(key, ModifierEvent.LAltDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LShiftKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.LShift) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.LShift);
                            myEventKeyOrMouse(key, ModifierEvent.LShiftDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LWin)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.LWindows) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.LWindows);
                            myEventKeyOrMouse(key, ModifierEvent.LWindowsDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RControlKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.RCtrl) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)(int)KeyModifiers.RCtrl);
                            myEventKeyOrMouse(key, ModifierEvent.RCtrlDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RAltKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.RAlt) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.RAlt);
                            myEventKeyOrMouse(key, ModifierEvent.RAltDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RShiftKey)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.RShift) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.RShift);
                            myEventKeyOrMouse(key, ModifierEvent.RShiftDown, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RWin)
                    {
                        if (!(((int)Modifiers & (int)KeyModifiers.RWindows) != 0))
                        {
                            Modifiers = (KeyModifiers)((int)Modifiers | (int)KeyModifiers.RWindows);
                            myEventKeyOrMouse(key, ModifierEvent.RWindowsDown, System.Drawing.Point.Empty);
                        }
                    }
                    else
                    {
                        //non modifier key
                        myEventKeyOrMouse(key, ModifierEvent.None, System.Drawing.Point.Empty);
                    }
                }

                if ((int)wParam == WM_KEYUP)
                {
                    if (key == AllKeys.LControlKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.LCtrl) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.LCtrl;
                            myEventKeyOrMouse(key, ModifierEvent.LCtrlUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LAltKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.LAlt) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.LAlt;
                            myEventKeyOrMouse(key, ModifierEvent.LAltUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LShiftKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.LShift) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.LShift;
                            myEventKeyOrMouse(key, ModifierEvent.LShiftUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.LWin)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.LWindows) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.LWindows;
                            myEventKeyOrMouse(key, ModifierEvent.LWindowsUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RControlKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.RCtrl) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.RCtrl;
                            myEventKeyOrMouse(key, ModifierEvent.RCtrlUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RAltKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.RAlt) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.RAlt;
                            myEventKeyOrMouse(key, ModifierEvent.RAltUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RShiftKey)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.RShift) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.RShift;
                            myEventKeyOrMouse(key, ModifierEvent.RShiftUp, System.Drawing.Point.Empty);
                        }
                    }
                    else if (key == AllKeys.RWin)
                    {
                        if ((((int)Modifiers & (int)KeyModifiers.RWindows) != 0))
                        {
                            Modifiers -= (int)KeyModifiers.RWindows;
                            myEventKeyOrMouse(key, ModifierEvent.RWindowsUp, System.Drawing.Point.Empty);
                        }
                    }
                }

                //if (key == Keys.LWin || key == Keys.RWin) return (IntPtr)1; //handled

                //Write to the console what key was pressed:
                //Debug.WriteLine(vkCode.ToString() + " " + key);


            }
            return CallNextHookEx(_KeyboardhookID, nCode, wParam, lParam);
        }

    }
}