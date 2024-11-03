using DryIoc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MathSymbolConverter.Utilities
{
    public class KeyboardListener : IDisposable
    {
        public KeyboardListener()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            hookedLowLevelKeyboardProc = (InterceptKeys.LowLevelKeyboardProc)LowLevelKeyboardProc;
            hookId = InterceptKeys.SetHook(hookedLowLevelKeyboardProc);
            hookedKeyboardCallbackAsync = new KeyboardCallbackAsync(KeyboardListener_KeyboardCallbackAsync);
        }

        private Dispatcher dispatcher;

        ~KeyboardListener()
        {
            Dispose();
        }

        public event RawKeyEventHandler KeyDown;

        public event RawKeyEventHandler KeyUp;

        #region Inner workings
        private IntPtr hookId = IntPtr.Zero;

        private delegate void KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character);

        string _chars = "";

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN || wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP)
                {
                    _chars = InterceptKeys.VKCodeToString((uint)Marshal.ReadInt32(lParam));

                    Task.Run(() => hookedKeyboardCallbackAsync((InterceptKeys.KeyEvent)wParam.ToUInt32(), Marshal.ReadInt32(lParam), _chars));
                    if (_chars.Equals("\t"))
                    {
                        return (IntPtr)1;
                    }
                }
            }

            return InterceptKeys.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private KeyboardCallbackAsync hookedKeyboardCallbackAsync;
        private InterceptKeys.LowLevelKeyboardProc hookedLowLevelKeyboardProc;
        void KeyboardListener_KeyboardCallbackAsync(InterceptKeys.KeyEvent keyEvent, int vkCode, string character)
        {
            switch (keyEvent)
            {
                // KeyDown events
                case InterceptKeys.KeyEvent.WM_KEYDOWN:
                    if (KeyDown != null)
                        dispatcher.BeginInvoke(new RawKeyEventHandler(KeyDown), this, new RawKeyEventArgs(vkCode, character));
                    break;

                // KeyUp events
                case InterceptKeys.KeyEvent.WM_KEYUP:
                    if (KeyUp != null)
                        dispatcher.BeginInvoke(new RawKeyEventHandler(KeyUp), this, new RawKeyEventArgs(vkCode, character));
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            InterceptKeys.UnhookWindowsHookEx(hookId);
        }

        #endregion
    }

    public class RawKeyEventArgs : EventArgs
    {
        public int VKCode;
        public int ASCII;
        public string Character;

        public override string ToString()
        {
            return Character;
        }

        public RawKeyEventArgs(int VKCode, string Character)
        {
            this.VKCode = VKCode;
            this.Character = Character;
            ASCII = char.Parse(Character);
        }
    }

    public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs args);

    #region WINAPI Helper class
    internal static class InterceptKeys
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam);
        public static int WH_KEYBOARD_LL = 13;

        public enum KeyEvent : int
        {
            /// <summary>
            /// Key down
            /// </summary>
            WM_KEYDOWN = 256,

            /// <summary>
            /// Key up
            /// </summary>
            WM_KEYUP = 257,

            /// <summary>
            /// System key up
            /// </summary>
            WM_SYSKEYUP = 261,

            /// <summary>
            /// System key down
            /// </summary>
            WM_SYSKEYDOWN = 260
        }

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #region Convert VKCode to string
        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern UInt32 GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        const int IMC_GETOPENSTATUS = 0x5;          // Windows 10 and Under
        const int IMC_GETCONVERSIONMODE = 0x1;      // Windows 11
        private const int WM_IME_CONTROL = 643;

        public static string VKCodeToString(uint VKCode)
        {
            //if (VKCode != 220)
            //{
            //    IntPtr currentHWnd = GetForegroundWindow();

            //    uint fromId = GetCurrentThreadId();
            //    uint toId = GetWindowThreadProcessId(currentHWnd, out _);

            //    AttachThreadInput(fromId, toId, true);

            //    IntPtr handle = GetFocus();

            //    IntPtr hIME = ImmGetDefaultIMEWnd(handle);
            //    int immStatus = SendMessage(hIME, WM_IME_CONTROL, (IntPtr)IMC_GETCONVERSIONMODE, (IntPtr)0).ToInt32();
            //    if (immStatus != 0)
            //    {
            //        AttachThreadInput(fromId, toId, false);
            //        return "\0";
            //    }
            //    AttachThreadInput(fromId, toId, false);
            //}
            //return ((char)VKCode).ToString();

            return ((char)VKCode).ToString();

        }
        #endregion
    }
    #endregion
}
