using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Utils
{
    class myUtils
    {
        public static readonly string CHARSNUMS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //internal static bool myIsNumeric(string item)
        //{
        //    int result;
        //    if(Int32.TryParse(item, out result))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        internal static bool myIsNumeric(object ObjectToTest)
        {
            if (ObjectToTest == null)
            {
                return false;

            }
            else
            {
                double OutValue;
                return double.TryParse(ObjectToTest.ToString().Trim(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out OutValue);
            }
        }

        //converts a string input numerical value (hex or decimal) to an integer.
        internal static int StringToInt(string stringToConvert, out bool success)
        {
            int returnvalue;
            System.Globalization.NumberStyles style;
            style = System.Globalization.NumberStyles.Number;        //default style
            if (stringToConvert.Contains("0x"))
            {
                style = System.Globalization.NumberStyles.AllowHexSpecifier; //hex style
                stringToConvert = stringToConvert.Replace("0x", "");
            }

            success = int.TryParse(stringToConvert, style, System.Globalization.NumberFormatInfo.CurrentInfo, out returnvalue);
            return (returnvalue);
        }

        internal static byte[] IntToByteArray(int inputval)
        {
            byte[] returnval = new byte[4];
            returnval[0] = (byte)(inputval & 0xFF);
            returnval[1] = (byte)((inputval & 0xFF00) >> 8);
            returnval[2] = (byte)((inputval & 0xFF0000) >> 16);
            returnval[3] = (byte)((inputval & 0xFF000000) >> 24);
            return returnval;
        }

        internal static int ByteArrayToInt(byte[] byteArray, int index)
        {
            int returnval = 0;

            if (byteArray.Length - index > 4)
            {
                return -1;     //FAIL
            }

            if (byteArray.Length - index > 0)
            {
                returnval = byteArray[index];
            }

            if (byteArray.Length - index > 1)
            {
                returnval += byteArray[index + 1] << 8;
            }
            if (byteArray.Length - index > 2)
            {
                returnval += byteArray[index + 2] << 16;
            }
            if (byteArray.Length - index > 3)
            {
                returnval += byteArray[index + 3] << 24;
            }
            return returnval;
        }

        internal static string ByteArrayToString(byte[] inputarray)
        {
            if (inputarray == null)
            {
                return ("");
            }

            string s = System.Text.ASCIIEncoding.ASCII.GetString(inputarray);
            return s;
        }

        internal static byte[] StringToByteArray(string inputstring)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(inputstring);
        }

        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        /// <param name="length">optional parameter for the number of bytes to get from the byte array.  Use null to get the length automatically.</param>
        /// <returns></returns>
        internal static string ByteArrayToHexString(byte[] bytes, int? length, string ByteSeparatorText)
        {
            int len;
            string hexString = "";

            if (length == null) len = bytes.Length;
            else len = (int)length;
            if (ByteSeparatorText == null) ByteSeparatorText = "";

            for (int i = 0; i < len; i++)
            {
                hexString = hexString + ByteSeparatorText + bytes[i].ToString("X2");
            }
            return hexString;
        }

        internal static string FXD(string Input, int N)
        {
            if (Input.Length <= N)
            {
                return Input.PadLeft(N, '0');
            }
            else
            {
                return Input.Substring(Input.Length - N, N);
            }
        }

        internal static string Dec2Hex(int dec)
        {
            return dec.ToString("X2");
        }

        internal static int Hex2Dec(string hexstring)
        {
            if (hexstring.Length > 8 || hexstring.Length <= 0)
            {
                throw new ArgumentException("hex must be at most 8 characters in length");
            }
            int newInt;
            if (!int.TryParse(hexstring, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out newInt))
            {
                throw new ArgumentException("Hex2Dec: TryParse Failed");
            }

            return newInt;

        }

        internal static byte[] HexStringToByteArray(string hexstring)
        {
            byte[] bytes;
            if (hexstring.Length == 0)
            {
                return null;
            }

            try
            {
                //Ensure there is an even number of characters, and add a leading 0 if this is not the case:
                if (hexstring.Length % 2 != 0) hexstring = "0" + hexstring;

                bytes = new byte[hexstring.Length / 2];

                for (int i = 0; i < hexstring.Length / 2; i++)
                {
                    bytes[i] = HexToByte(hexstring.Substring(i * 2, 2));
                }
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HexStringToByteArray exception " + ex.Message);
                return null;        //Parse failed for some reason.
            }
        }

        /// <summary>
        /// Accepts a string of hexadecimal bytes and converts the bytes to their corresponding ASCII characters
        /// </summary>
        /// <param name="hexstring"></param>
        /// <returns></returns>
        internal static string HexStringToASCIIString(string hexstring)
        {
            return ByteArrayToString(HexStringToByteArray(hexstring));
        }

        /// <summary>
        /// Accepts an ASCII string and converts each character to its ASCII code displayed in hexadecimal form
        /// </summary>
        /// <param name="ASCIIString"></param>
        /// <returns></returns>
        internal static string ASCIIStringToHexString(string ASCIIString)
        {
            return ByteArrayToHexString(StringToByteArray(ASCIIString), null, null);
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        internal static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }
            //byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            byte newByte;

            if (!byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out newByte))
            {

                throw new ArgumentException("HexToByte: TryParse Failed");
            }

            return newByte;
        }

        internal static int myMakeNumeric(string item)
        {
            int result;
            if (Int32.TryParse(item, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        //Simple Start / Stop Event:
        //static System.Threading.ManualResetEvent ResetEvent = new ManualResetEvent(false);
        //ResetEvent.Reset();
        //result = ResetEvent.WaitOne(3000);       //Wait til ResetEvent.Set() is called by another thread
        //ResetEvent.Set();

        //Array.Resize
        
        //Start a function as a thread:
        //System.Threading.Thread thread1;
        //thread1.threadstart(Function);

        //System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
        //bw.DoWork += new System.ComponentModel.DoWorkEventHandler(Function);
        //private void Function(object sender, DoWorkEventArgs e)

        ///Great primer on threading:
        ///http://www.albahari.com/threading/


        //How to throw an exception:
        //throw new System.ArgumentException("Total Flash Size out of range", "TotalFlashSize");

        //Notes:
        //StringBuilder is in System.Text

        static public void HandleException(string moduleName, Exception e)
        {
            // Purpose    : Provides a central mechanism for exception handling.
            //            : Displays a message box that describes the exception.

            // Accepts    : moduleName - the module where the exception occurred.
            //            : e - the exception

            string Message;
            string Caption;

            try
            {
                // Create an error message.
                Message = "Exception: " + e.Message + Environment.NewLine + "Module: " + moduleName + Environment.NewLine + "Method: " + e.TargetSite.Name;

                // Specify a caption.
                Caption = "Unexpected Exception";

                // Display the message in a message box.
                MessageBox.Show(Message, Caption, MessageBoxButtons.OK);
                Debug.Write(Message);
            }
            finally { }

        }

        ///// <summary>
        ///// Replacement for VB InputBox, returns user input string.  Requires InputBoxDialog.cs
        ///// </summary>
        ///// <param name="prompt"></param>
        ///// <param name="title"></param>
        ///// <param name="defaultValue"></param>
        ///// <returns></returns>
        public static string InputBox(string prompt, string title, string defaultValue)
        {
            InputBoxDialog ib = new InputBoxDialog();
            ib.FormPrompt = prompt;
            ib.FormCaption = title;
            ib.DefaultValue = defaultValue;
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        } // method: InputBox
    }
}
