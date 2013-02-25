using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Hotkeys;         //Hotkey handling class allowing global hotkeys to be registered
using KeyboardHookHandler;   //Keyboard hook class providing an event handler to keep track of ALL keyboard events
using Utils;                 //static utility class

namespace AutoitxTest
{
    public partial class frmMain : Form
    {
        KeyboardMouseHook myKeyboardMouseHook;
        HotkeyHandler myHotkeyHandler;
        IniFile ini;
        private NotifyIcon myTrayIcon;
        private ContextMenu myTrayIconContextMenu;
        public bool AppRunning;

        public frmMain()
        {
            InitializeComponent();

            AppRunning = true;

            //Pull in some settings from an ini file:
            ini = new IniFile(Application.StartupPath + "\\AutoItX.ini");
            bool success;
            int SendKeyDelay = myUtils.StringToInt(ini.IniReadValue("AUTOITXSETTINGS", "SendKeyDelay"), out success);
            if (!success) SendKeyDelay = 5;     //load default value
            int WinWaitDelay = myUtils.StringToInt(ini.IniReadValue("AUTOITXSETTINGS", "WinWaitDelay"), out success);
            if (!success) WinWaitDelay = 250;

            ///Alters the the length of the brief pause in between sent keystrokes.
            ///Time in milliseconds to pause (default=5). Sometimes a value of 0 does not work; use 1 instead.
            AutoItX.AU3_AutoItSetOption("SendKeyDelay", SendKeyDelay);

            ///Alters the method that is used to match window titles during search operations.
            ///1 = Match the title from the start (default)
            ///2 = Match any substring in the title
            ///3 = Exact title match
            ///4 = Advanced mode, see Window Titles & Text (Advanced)
            AutoItX.AU3_AutoItSetOption("WinTitleMatchMode", 2);

            ///WinWaitDelay Alters how long a script should briefly pause after a successful window-related operation. 
            ///Time in milliseconds to pause (default=250). 
            AutoItX.AU3_AutoItSetOption("WinWaitDelay", WinWaitDelay);

            ///WinSearchChildren Allows the window search routines to search child windows as well as top-level windows.
            ///0 = Only search top-level windows (default)
            ///1 = Search top-level and child windows 
            AutoItX.AU3_AutoItSetOption("WinSearchChildren", 0);


            //When using the COM library (rater than dllimport):
            //AutoItX3Lib.AutoItX3Class Au3Class = new AutoItX3Lib.AutoItX3Class();
            //Au3Class.

            //Simple keyboard hook set up with a callback to eventKeyPress on keyboard events:
            myKeyboardMouseHook = new KeyboardMouseHook(eventKeyPress);

            //Simple hotkey handler to allow registering hotkeys each with their own callbacks 
            //(also hotkeys pressed are CONSUMED, meaning windows will not pass them down to other applications)
            //Or in other words, if this application registers a given hotkey, that hotkey will ONLY be seen by this
            //application.
            //Very important: be sure to override WndProc(ref Message m) and add a function call to myHotkeyHandler.WndProc(ref m)
            myHotkeyHandler = new HotkeyHandler(this.Handle);

            myHotkeyHandler.HotkeySet(HotkeyHandler.KeyModifiers.Control | HotkeyHandler.KeyModifiers.Alt, Keys.A, eventHotkeyCtrlAltA);
            myHotkeyHandler.HotkeySet(HotkeyHandler.KeyModifiers.None, Keys.B, eventHotkeyB);

            myHotkeyHandler.HotkeySet(HotkeyHandler.KeyModifiers.Control, Keys.B, AutoItXScriptRun);


            //Set up a tray icon with a context menu
            SetupTrayIcon();

            //Comment this out to cause the form to start hidden (only show the tray icon):
            this.Show();
        }

        #region Form Items

        /// <summary>
        /// Override WndProc to add a message handler for the hotkey handler:
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (myHotkeyHandler != null)
            {
                myHotkeyHandler.WndProc(ref m);
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppRunning = false;   
            myTrayIcon.Dispose();
        }

        /// <summary>
        /// Allows the application to exit when the form is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


        /// <summary>
        /// Runs a MethodInvoker delegate on the UI thread from whichever thread we are currently calling from.
        /// </summary>
        /// <param name="ivk"></param>
        public void UI(MethodInvoker ivk)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(ivk);
            else
                ivk();
        }

        /// <summary>
        /// Runs a MethodInvoker delegate on the UI thread from whichever thread we are currently calling from and BLOCKS until it is complete
        /// </summary>
        /// <param name="ivk"></param>
        public void UIBlockingInvoke(MethodInvoker ivk)
        {
            bool result;
            System.Threading.ManualResetEvent UIAsyncComplete = new System.Threading.ManualResetEvent(false);
            UIAsyncComplete.Reset();
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    try
                    {
                        ivk();
                    }
                    finally
                    {
                        UIAsyncComplete.Set();
                    }
                }));

                while (AppRunning)
                {
                    //Check AppRunning...
                    //Don't call WaitOne(int32)!!!  It was added in .NET framework 2.0 SERVICE PACK 2!!  
                    //Instead call WaitOne(int32, false).  This works in .NET framework 2.0 RTM.
                    result = UIAsyncComplete.WaitOne(500, false);      //timeout after 500mS to check if AppRunning is still true
                    if (result)
                    {
                        break;  //Exit when UIAsyncComplete has been set OR if AppRunning becomes false.
                    }
                }
            }
            else
            {
                ivk();
            }
        }

        #endregion

        #region Tray Icon

        public void SetupTrayIcon()
        {
            myTrayIconContextMenu = new ContextMenu();
            myTrayIconContextMenu.MenuItems.Add(0,
                new MenuItem("Show", new System.EventHandler(Show_Click)));
            myTrayIconContextMenu.MenuItems.Add(1,
                new MenuItem("Hide", new System.EventHandler(Hide_Click)));
            myTrayIconContextMenu.MenuItems.Add(2,
                new MenuItem("Exit", new System.EventHandler(Exit_Click)));

            myTrayIcon = new NotifyIcon();
            myTrayIcon.Text = "Right click for context menu";
            myTrayIcon.Visible = true;

            //Add the icon to the project, then change the properties for the Build action to Embedded resource
            myTrayIcon.Icon = new Icon(GetType(), "favicon.ico");
            myTrayIcon.ContextMenu = myTrayIconContextMenu;
            
        }

        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Close();
        }
        protected void Hide_Click(Object sender, System.EventArgs e)
        {
            Hide();
        }
        protected void Show_Click(Object sender, System.EventArgs e)
        {
            Show();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            AutoItXScript();
        }
        private void AutoItXScript()
        {

            //Lame solution:  I couldn't figure out how to use Notepad.exe from the default Windows path with AutoItX.  I guess I should just use C# to run.
            //AutoItX.AU3_Run("c:\\windows\\Notepad.exe", "c:\\windows", AutoItX.SW_SHOWDEFAULT);

            //Much better:
            System.Diagnostics.Process.Start("Notepad.exe");

            //Note I'm using WinTitleMatchMode = 2, any substring in the title
            if (AutoItX.AU3_WinWait("Notepad", "", 1000) == 1)
            {
                AutoItX.AU3_WinActivate("Notepad", "");
                AutoItX.AU3_Send("Test", 0);

                AutoItX.AU3_Send("{SHIFTDOWN}{Left}{Left}{Left}{Left}{SHIFTUP}", 0);
            }

            //Put a tooltip just below the mouse:
            AutoItX.AU3_ToolTip("test tooltip", AutoItX.AU3_MouseGetPosX(), AutoItX.AU3_MouseGetPosY() + 15);
            AutoItX.AU3_Sleep(1000);
            AutoItX.AU3_ToolTip("", AutoItX.AU3_MouseGetPosX(), AutoItX.AU3_MouseGetPosY());
        }

        void AutoItXScriptRun(Keys KeyCode, HotkeyHandler.KeyModifiers KeyMods)
        {
            AutoItXScript();
        }
        void eventHotkeyCtrlAltA(Keys KeyCode, HotkeyHandler.KeyModifiers KeyMods)
        {
            //MessageBox.Show("Hotkey Ctrl Alt A pressed!");
            lblHotkey.Text = "Hotkey Ctrl Alt A pressed!";


        }

        void eventHotkeyB(Keys KeyCode, HotkeyHandler.KeyModifiers KeyMods)
        {
            //MessageBox.Show("Hotkey B pressed!");
            lblHotkey.Text = "Hotkey B pressed!";
        }

        void eventKeyPress(KeyboardMouseHook.AllKeys KeyCode, KeyboardMouseHook.ModifierEvent e, Point pt)
        {
            /*
            //Some useful information can also be gotten from the windows forms Control interface:
            //Control.MousePosition
            //Control.MouseButtons
             */

            string Mods = "";
            //These can be used to retrive the static state of the modifier keys:
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.LCtrl) != 0) Mods += "LCtrl ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.LAlt) != 0) Mods += "LAlt ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.LShift) != 0) Mods += "LShift ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.LWindows) != 0) Mods += "LWin ";

            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.RCtrl) != 0) Mods += "RCtrl ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.RAlt) != 0) Mods += "RAlt ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.RShift) != 0) Mods += "RShift ";
            if (((int)myKeyboardMouseHook.Modifiers & (int)KeyboardMouseHook.KeyModifiers.RWindows) != 0) Mods += "RWin ";

            if (KeyCode == KeyboardMouseHook.AllKeys.None)
            {
                //First handle MOUSE events:

                Debug.WriteLine("Mouse event: " + e.ToString() + " " + pt.ToString());
            }
            else if (e != KeyboardMouseHook.ModifierEvent.None)
            {
                //To retrieve Modifier keydown and keyup Events, check the ModifierEvent argument e:
                if (e.ToString().Contains("Down"))
                {
                    Debug.WriteLine("Key pressed: 0x" + myUtils.Dec2Hex((int)KeyCode) + " " + KeyCode.ToString() + " Down");
                }
                else
                {
                    Debug.WriteLine("Key pressed: 0x" + myUtils.Dec2Hex((int)KeyCode) + " " + KeyCode.ToString() + " Up");
                }
            }
            else
            {
                string Character = myKeyboardMouseHook.KeyCodeToString(KeyCode);

                //All non-modifier keys will appear here:
                Debug.WriteLine("Key pressed: 0x" + myUtils.Dec2Hex((int)KeyCode) + " " + Mods + KeyCode.ToString());
                lblKeys.Text += Character;
            }
        }
    }
}
