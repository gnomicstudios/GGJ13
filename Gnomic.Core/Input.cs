using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#if USE_PCGAMEPAD
using DI = Microsoft.DirectX.DirectInput;
#endif

namespace Gnomic
{
    public enum MouseButton
    {
        Left = 0,
        Middle,
        Right
    }
    public enum GamePadButton
    {
        DPadUp                  = 0,
        DPadDown,
        DPadLeft,
        DPadRight,
        Start,
        Back,
        LeftStick,
        RightStick,
        LeftShoulder,
        RightShoulder,
        BigButton               = 11,
        A,
        B,
        X,
        Y,
        LeftThumbstickLeft      = 21,
        RightTrigger,
        LeftTrigger,
        RightThumbstickUp,
        RightThumbstickDown,
        RightThumbstickRight,
        RightThumbstickLeft,
        LeftThumbstickUp,
        LeftThumbstickDown,
        LeftThumbstickRight
    }
    public enum ButtonGeneric
    {
        #region Keyboard

        // Summary:
        //     Reserved
        None = 0,
        //
        // Summary:
        //     BACKSPACE key
        Back = 8,
        //
        // Summary:
        //     TAB key
        Tab = 9,
        //
        // Summary:
        //     ENTER key
        Enter = 13,
        //
        // Summary:
        //     PAUSE key
        Pause = 19,
        //
        // Summary:
        //     CAPS LOCK key
        CapsLock = 20,
        //
        // Summary:
        //     ESC key
        Escape = 27,
        //
        // Summary:
        //     SPACEBAR
        Space = 32,
        //
        // Summary:
        //     PAGE UP key
        PageUp = 33,
        //
        // Summary:
        //     PAGE DOWN key
        PageDown = 34,
        //
        // Summary:
        //     END key
        End = 35,
        //
        // Summary:
        //     HOME key
        Home = 36,
        //
        // Summary:
        //     LEFT ARROW key
        Left = 37,
        //
        // Summary:
        //     UP ARROW key
        Up = 38,
        //
        // Summary:
        //     RIGHT ARROW key
        Right = 39,
        //
        // Summary:
        //     DOWN ARROW key
        Down = 40,
        //
        // Summary:
        //     SELECT key
        Select = 41,
        //
        // Summary:
        //     PRINT key
        Print = 42,
        //
        // Summary:
        //     EXECUTE key
        Execute = 43,
        //
        // Summary:
        //     PRINT SCREEN key
        PrintScreen = 44,
        //
        // Summary:
        //     INS key
        Insert = 45,
        //
        // Summary:
        //     DEL key
        Delete = 46,
        //
        // Summary:
        //     HELP key
        Help = 47,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D0 = 48,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D1 = 49,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D2 = 50,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D3 = 51,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D4 = 52,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D5 = 53,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D6 = 54,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D7 = 55,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D8 = 56,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D9 = 57,
        //
        // Summary:
        //     A key
        A = 65,
        //
        // Summary:
        //     B key
        B = 66,
        //
        // Summary:
        //     C key
        C = 67,
        //
        // Summary:
        //     D key
        D = 68,
        //
        // Summary:
        //     E key
        E = 69,
        //
        // Summary:
        //     F key
        F = 70,
        //
        // Summary:
        //     G key
        G = 71,
        //
        // Summary:
        //     H key
        H = 72,
        //
        // Summary:
        //     I key
        I = 73,
        //
        // Summary:
        //     J key
        J = 74,
        //
        // Summary:
        //     K key
        K = 75,
        //
        // Summary:
        //     L key
        L = 76,
        //
        // Summary:
        //     M key
        M = 77,
        //
        // Summary:
        //     N key
        N = 78,
        //
        // Summary:
        //     O key
        O = 79,
        //
        // Summary:
        //     P key
        P = 80,
        //
        // Summary:
        //     Q key
        Q = 81,
        //
        // Summary:
        //     R key
        R = 82,
        //
        // Summary:
        //     S key
        S = 83,
        //
        // Summary:
        //     T key
        T = 84,
        //
        // Summary:
        //     U key
        U = 85,
        //
        // Summary:
        //     V key
        V = 86,
        //
        // Summary:
        //     W key
        W = 87,
        //
        // Summary:
        //     X key
        X = 88,
        //
        // Summary:
        //     Y key
        Y = 89,
        //
        // Summary:
        //     Z key
        Z = 90,
        //
        // Summary:
        //     Left Windows key
        LeftWindows = 91,
        //
        // Summary:
        //     Right Windows key
        RightWindows = 92,
        //
        // Summary:
        //     Applications key
        Apps = 93,
        //
        // Summary:
        //     Computer Sleep key
        Sleep = 95,
        //
        // Summary:
        //     Numeric keypad 0 key
        NumPad0 = 96,
        //
        // Summary:
        //     Numeric keypad 1 key
        NumPad1 = 97,
        //
        // Summary:
        //     Numeric keypad 2 key
        NumPad2 = 98,
        //
        // Summary:
        //     Numeric keypad 3 key
        NumPad3 = 99,
        //
        // Summary:
        //     Numeric keypad 4 key
        NumPad4 = 100,
        //
        // Summary:
        //     Numeric keypad 5 key
        NumPad5 = 101,
        //
        // Summary:
        //     Numeric keypad 6 key
        NumPad6 = 102,
        //
        // Summary:
        //     Numeric keypad 7 key
        NumPad7 = 103,
        //
        // Summary:
        //     Numeric keypad 8 key
        NumPad8 = 104,
        //
        // Summary:
        //     Numeric keypad 9 key
        NumPad9 = 105,
        //
        // Summary:
        //     Multiply key
        Multiply = 106,
        //
        // Summary:
        //     Add key
        Add = 107,
        //
        // Summary:
        //     Separator key
        Separator = 108,
        //
        // Summary:
        //     Subtract key
        Subtract = 109,
        //
        // Summary:
        //     Decimal key
        Decimal = 110,
        //
        // Summary:
        //     Divide key
        Divide = 111,
        //
        // Summary:
        //     F1 key
        F1 = 112,
        //
        // Summary:
        //     F2 key
        F2 = 113,
        //
        // Summary:
        //     F3 key
        F3 = 114,
        //
        // Summary:
        //     F4 key
        F4 = 115,
        //
        // Summary:
        //     F5 key
        F5 = 116,
        //
        // Summary:
        //     F6 key
        F6 = 117,
        //
        // Summary:
        //     F7 key
        F7 = 118,
        //
        // Summary:
        //     F8 key
        F8 = 119,
        //
        // Summary:
        //     F9 key
        F9 = 120,
        //
        // Summary:
        //     F10 key
        F10 = 121,
        //
        // Summary:
        //     F11 key
        F11 = 122,
        //
        // Summary:
        //     F12 key
        F12 = 123,
        //
        // Summary:
        //     F13 key
        F13 = 124,
        //
        // Summary:
        //     F14 key
        F14 = 125,
        //
        // Summary:
        //     F15 key
        F15 = 126,
        //
        // Summary:
        //     F16 key
        F16 = 127,
        //
        // Summary:
        //     F17 key
        F17 = 128,
        //
        // Summary:
        //     F18 key
        F18 = 129,
        //
        // Summary:
        //     F19 key
        F19 = 130,
        //
        // Summary:
        //     F20 key
        F20 = 131,
        //
        // Summary:
        //     F21 key
        F21 = 132,
        //
        // Summary:
        //     F22 key
        F22 = 133,
        //
        // Summary:
        //     F23 key
        F23 = 134,
        //
        // Summary:
        //     F24 key
        F24 = 135,
        //
        // Summary:
        //     NUM LOCK key
        NumLock = 144,
        //
        // Summary:
        //     SCROLL LOCK key
        Scroll = 145,
        //
        // Summary:
        //     Left SHIFT key
        LeftShift = 160,
        //
        // Summary:
        //     Right SHIFT key
        RightShift = 161,
        //
        // Summary:
        //     Left CONTROL key
        LeftControl = 162,
        //
        // Summary:
        //     Right CONTROL key
        RightControl = 163,
        //
        // Summary:
        //     Left ALT key
        LeftAlt = 164,
        //
        // Summary:
        //     Right ALT key
        RightAlt = 165,
        //
        // Summary:
        //     Windows 2000/XP: Browser Back key
        BrowserBack = 166,
        //
        // Summary:
        //     Windows 2000/XP: Browser Forward key
        BrowserForward = 167,
        //
        // Summary:
        //     Windows 2000/XP: Browser Refresh key
        BrowserRefresh = 168,
        //
        // Summary:
        //     Windows 2000/XP: Browser Stop key
        BrowserStop = 169,
        //
        // Summary:
        //     Windows 2000/XP: Browser Search key
        BrowserSearch = 170,
        //
        // Summary:
        //     Windows 2000/XP: Browser Favorites key
        BrowserFavorites = 171,
        //
        // Summary:
        //     Windows 2000/XP: Browser Start and Home key
        BrowserHome = 172,
        //
        // Summary:
        //     Windows 2000/XP: Volume Mute key
        VolumeMute = 173,
        //
        // Summary:
        //     Windows 2000/XP: Volume Down key
        VolumeDown = 174,
        //
        // Summary:
        //     Windows 2000/XP: Volume Up key
        VolumeUp = 175,
        //
        // Summary:
        //     Windows 2000/XP: Next Track key
        MediaNextTrack = 176,
        //
        // Summary:
        //     Windows 2000/XP: Previous Track key
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     Windows 2000/XP: Stop Media key
        MediaStop = 178,
        //
        // Summary:
        //     Windows 2000/XP: Play/Pause Media key
        MediaPlayPause = 179,
        //
        // Summary:
        //     Windows 2000/XP: Start Mail key
        LaunchMail = 180,
        //
        // Summary:
        //     Windows 2000/XP: Select Media key
        SelectMedia = 181,
        //
        // Summary:
        //     Windows 2000/XP: Start Application 1 key
        LaunchApplication1 = 182,
        //
        // Summary:
        //     Windows 2000/XP: Start Application 2 key
        LaunchApplication2 = 183,
        //
        // Summary:
        //     Windows 2000/XP: The OEM Semicolon key on a US standard keyboard
        OemSemicolon = 186,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '+' key
        OemPlus = 187,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the ',' key
        OemComma = 188,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '-' key
        OemMinus = 189,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '.' key
        OemPeriod = 190,
        //
        // Summary:
        //     Windows 2000/XP: The OEM question mark key on a US standard keyboard
        OemQuestion = 191,
        //
        // Summary:
        //     Windows 2000/XP: The OEM tilde key on a US standard keyboard
        OemTilde = 192,
        //
        // Summary:
        //     Green ChatPad key
        ChatPadGreen = 202,
        //
        // Summary:
        //     Orange ChatPad key
        ChatPadOrange = 203,
        //
        // Summary:
        //     Windows 2000/XP: The OEM open bracket key on a US standard keyboard
        OemOpenBrackets = 219,
        //
        // Summary:
        //     Windows 2000/XP: The OEM pipe key on a US standard keyboard
        OemPipe = 220,
        //
        // Summary:
        //     Windows 2000/XP: The OEM close bracket key on a US standard keyboard
        OemCloseBrackets = 221,
        //
        // Summary:
        //     Windows 2000/XP: The OEM singled/double quote key on a US standard keyboard
        OemQuotes = 222,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        Oem8 = 223,
        //
        // Summary:
        //     Windows 2000/XP: The OEM angle bracket or backslash key on the RT 102 key
        //     keyboard
        OemBackslash = 226,
        //
        // Summary:
        //     Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        ProcessKey = 229,
        //
        // Summary:
        //     Attn key
        Attn = 246,
        //
        // Summary:
        //     CrSel key
        Crsel = 247,
        //
        // Summary:
        //     ExSel key
        Exsel = 248,
        //
        // Summary:
        //     Erase EOF key
        EraseEof = 249,
        //
        // Summary:
        //     Play key
        Play = 250,
        //
        // Summary:
        //     Zoom key
        Zoom = 251,
        //
        // Summary:
        //     PA1 key
        Pa1 = 253,
        //
        // Summary:
        //     CLEAR key
        OemClear = 254,
#endregion // Keyboard

        #region Mouse
        MouseLeft           = 300,
        MouseMiddle,
        MouseRight,
        #endregion

        #region Gamepad0
        Pad0DPadUp              = 400,
        Pad0DPadDown,
        Pad0DPadLeft,
        Pad0DPadRight,
        Pad0Start,
        Pad0Back,
        Pad0LeftStick,
        Pad0RightStick,
        Pad0LeftShoulder,
        Pad0RightShoulder,
        Pad0BigButton           = 411,
        Pad0A,
        Pad0B,
        Pad0X,
        Pad0Y,
        Pad0LeftThumbstickLeft  = 421,
        Pad0RightTrigger,
        Pad0LeftTrigger,
        Pad0RightThumbstickUp,
        Pad0RightThumbstickDown,
        Pad0RightThumbstickRight,
        Pad0RightThumbstickLeft,
        Pad0LeftThumbstickUp,
        Pad0LeftThumbstickDown,
        Pad0LeftThumbstickRight,
        #endregion

        #region Gamepad1
        Pad1DPadUp              = 500,
        Pad1DPadDown,
        Pad1DPadLeft,
        Pad1DPadRight,
        Pad1Start,
        Pad1Back,
        Pad1LeftStick,
        Pad1RightStick,
        Pad1LeftShoulder,
        Pad1RightShoulder,
        Pad1BigButton           = 511,
        Pad1A,
        Pad1B,
        Pad1X,
        Pad1Y,
        Pad1LeftThumbstickLeft  = 521,
        Pad1RightTrigger,
        Pad1LeftTrigger,
        Pad1RightThumbstickUp,
        Pad1RightThumbstickDown,
        Pad1RightThumbstickRight,
        Pad1RightThumbstickLeft,
        Pad1LeftThumbstickUp,
        Pad1LeftThumbstickDown,
        Pad1LeftThumbstickRight,
        #endregion
        
        #region Gamepad2
        Pad2DPadUp              = 600,
        Pad2DPadDown,
        Pad2DPadLeft,
        Pad2DPadRight,
        Pad2Start,
        Pad2Back,
        Pad2LeftStick,
        Pad2RightStick,
        Pad2LeftShoulder,
        Pad2RightShoulder,
        Pad2BigButton           = 611,
        Pad2A,
        Pad2B,
        Pad2X,
        Pad2Y,
        Pad2LeftThumbstickLeft  = 621,
        Pad2RightTrigger,
        Pad2LeftTrigger,
        Pad2RightThumbstickUp,
        Pad2RightThumbstickDown,
        Pad2RightThumbstickRight,
        Pad2RightThumbstickLeft,
        Pad2LeftThumbstickUp,
        Pad2LeftThumbstickDown,
        Pad2LeftThumbstickRight,
        #endregion
        
        #region Gamepad3
        Pad3DPadUp              = 700,
        Pad3DPadDown,
        Pad3DPadLeft,
        Pad3DPadRight,
        Pad3Start,
        Pad3Back,
        Pad3LeftStick,
        Pad3RightStick,
        Pad3LeftShoulder,
        Pad3RightShoulder,
        Pad3BigButton           = 711,
        Pad3A,
        Pad3B,
        Pad3X,
        Pad3Y,
        Pad3LeftThumbstickLeft  = 721,
        Pad3RightTrigger,
        Pad3LeftTrigger,
        Pad3RightThumbstickUp,
        Pad3RightThumbstickDown,
        Pad3RightThumbstickRight,
        Pad3RightThumbstickLeft,
        Pad3LeftThumbstickUp,
        Pad3LeftThumbstickDown,
        Pad3LeftThumbstickRight,
        #endregion
        
        #region Gamepad4
        Pad4DPadUp              = 800,
        Pad4DPadDown,
        Pad4DPadLeft,
        Pad4DPadRight,
        Pad4Start,
        Pad4Back,
        Pad4LeftStick,
        Pad4RightStick,
        Pad4LeftShoulder,
        Pad4RightShoulder,
        Pad4BigButton           = 811,
        Pad4A,
        Pad4B,
        Pad4X,
        Pad4Y,
        Pad4LeftThumbstickLeft  = 821,
        Pad4RightTrigger,
        Pad4LeftTrigger,
        Pad4RightThumbstickUp,
        Pad4RightThumbstickDown,
        Pad4RightThumbstickRight,
        Pad4RightThumbstickLeft,
        Pad4LeftThumbstickUp,
        Pad4LeftThumbstickDown,
        Pad4LeftThumbstickRight,
        #endregion
        
        #region Gamepad5
        Pad5DPadUp              = 900,
        Pad5DPadDown,
        Pad5DPadLeft,
        Pad5DPadRight,
        Pad5Start,
        Pad5Back,
        Pad5LeftStick,
        Pad5RightStick,
        Pad5LeftShoulder,
        Pad5RightShoulder,
        Pad5BigButton           = 911,
        Pad5A,
        Pad5B,
        Pad5X,
        Pad5Y,
        Pad5LeftThumbstickLeft  = 921,
        Pad5RightTrigger,
        Pad5LeftTrigger,
        Pad5RightThumbstickUp,
        Pad5RightThumbstickDown,
        Pad5RightThumbstickRight,
        Pad5RightThumbstickLeft,
        Pad5LeftThumbstickUp,
        Pad5LeftThumbstickDown,
        Pad5LeftThumbstickRight,
        #endregion
        
        #region Gamepad6
        Pad6DPadUp              = 1000,
        Pad6DPadDown,
        Pad6DPadLeft,
        Pad6DPadRight,
        Pad6Start,
        Pad6Back,
        Pad6LeftStick,
        Pad6RightStick,
        Pad6LeftShoulder,
        Pad6RightShoulder,
        Pad6BigButton           = 1011,
        Pad6A,
        Pad6B,
        Pad6X,
        Pad6Y,
        Pad6LeftThumbstickLeft  = 1021,
        Pad6RightTrigger,
        Pad6LeftTrigger,
        Pad6RightThumbstickUp,
        Pad6RightThumbstickDown,
        Pad6RightThumbstickRight,
        Pad6RightThumbstickLeft,
        Pad6LeftThumbstickUp,
        Pad6LeftThumbstickDown,
        Pad6LeftThumbstickRight,
        #endregion
        
        #region Gamepad7
        Pad7DPadUp              = 1100,
        Pad7DPadDown,
        Pad7DPadLeft,
        Pad7DPadRight,
        Pad7Start,
        Pad7Back,
        Pad7LeftStick,
        Pad7RightStick,
        Pad7LeftShoulder,
        Pad7RightShoulder,
        Pad7BigButton           = 1111,
        Pad7A,
        Pad7B,
        Pad7X,
        Pad7Y,
        Pad7LeftThumbstickLeft  = 1121,
        Pad7RightTrigger,
        Pad7LeftTrigger,
        Pad7RightThumbstickUp,
        Pad7RightThumbstickDown,
        Pad7RightThumbstickRight,
        Pad7RightThumbstickLeft,
        Pad7LeftThumbstickUp,
        Pad7LeftThumbstickDown,
        Pad7LeftThumbstickRight
        #endregion

    }

    public enum MenuInputs
    {
        Forward,
        Backwards,
        Up,
        Down,
        Left,
        Right,
        Start,
        Info,
        Info2,
        Exit
    }

    #region PC Gamepad
    //0        DPadUp = 1,
    //1        DPadDown = 2,
    //2        DPadLeft = 4,
    //3        DPadRight = 8,
    //4        Start = 16,
    //5        Back = 32,
    //6        LeftStick = 64,
    //7        RightStick = 128,
    //8        LeftShoulder = 256,
    //9        RightShoulder = 512,
    //10        BigButton = 2048,
    //11        A = 4096,
    //12        B = 8192,
    //13        X = 16384,
    //14        Y = 32768,
    //15        LeftThumbstickLeft = 2097152,
    //16        RightTrigger = 4194304,
    //17        LeftTrigger = 8388608,
    //18        RightThumbstickUp = 16777216,
    //19        RightThumbstickDown = 33554432,
    //20        RightThumbstickRight = 67108864,
    //22        RightThumbstickLeft = 134217728,
    //23        LeftThumbstickUp = 268435456,
    //24        LeftThumbstickDown = 536870912,
    //25        LeftThumbstickRight = 1073741824,

#if USE_PCGAMEPAD

    /// <summary>
    /// Encapsulates a DirectInput device for use with XNA
    /// </summary>
    public class PCGamepad
    {
        protected DI.Device device;
        protected DI.DeviceCaps caps;
        const float center = 32767.5f;

        /// <summary>
        /// Maps the first 12 pc gamepad buttons to the matching 
        /// bit positions of Microsoft.Xna.Framework.Input.Buttons
        /// </summary>
        public int[] ButtonMappings = new int[] { 14, 12, 13, 15, 8, 9, 23, 22, 4, 5, 7, 8 };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gamepadInstanceGuid">The DirectInput device Guid</param>
        public PCGamepad(Guid gamepadInstanceGuid)
        {
            device = new DI.Device(gamepadInstanceGuid);
            device.SetDataFormat(DI.DeviceDataFormat.Joystick);
            device.Acquire();
            caps = device.Caps;
        }

        /// <summary>
        /// Converts the current state of this device to an XNA GamePadState
        /// </summary>
        /// <returns>The GamePadState for this DirectInput device</returns>
        public GamePadState GetState()
        {
            // Get the JoystickState state
            DI.JoystickState joyState = device.CurrentJoystickState;
            
            // Point Of View = DPad
            int pov = joyState.GetPointOfView()[0];
            
            byte[] btns = joyState.GetButtons();

            // Map the DirectInput buttons to XNA Buttons enum values
            Buttons padBtns = (Buttons)0;
            int btnCount = Math.Min(caps.NumberButtons, 12);
            for (int i = 0; i < btnCount; i++)
            {
                if (btns[i] != 0)
                    padBtns |= (Buttons)(1 << ButtonMappings[i]);
            }

            // Put it all together. 
            GamePadState gs = new GamePadState(
                new GamePadThumbSticks(
                    caps.NumberAxes <= 0 ? Vector2.Zero : 
                        new Vector2((joyState.X - center) / center, -(joyState.Y - center) / center),
                    caps.NumberAxes <= 2 ? Vector2.Zero :
                        new Vector2((joyState.Z - center) / center, -(joyState.Rz - center) / center)),
                new GamePadTriggers(
                    (0 != (int)(padBtns & Buttons.LeftTrigger)) ? 1.0f : 0.0f,
                    (0 != (int)(padBtns & Buttons.RightTrigger)) ? 1.0f : 0.0f),
                new GamePadButtons(padBtns),
                (pov < 0) ? new GamePadDPad() : new GamePadDPad(
                    (pov > 27000 || pov < 9000) ? ButtonState.Pressed : ButtonState.Released,
                    (9000 < pov && pov < 27000) ? ButtonState.Pressed : ButtonState.Released,
                    (18000 < pov) ? ButtonState.Pressed : ButtonState.Released,
                    (0 < pov && pov < 18000) ? ButtonState.Pressed : ButtonState.Released));

            return gs;
        }
    }
#endif
    #endregion

    public static class Input
    {
        private static GraphicsDevice graphicsDevice;
        private static MouseState currentMouseState;
        private static MouseState lastMouseState;
        private static int zeroPointX = 640;
        private static int zeroPointY = 512;

        private static KeyboardState currentKeyState;
        private static KeyboardState lastKeyState;
        private static GamePadState[] lastPadState = new GamePadState[8];

        public static GamePadState[] CurrentPadState = new GamePadState[8];
        public static List<int> UpdatePadIds = new List<int>(new int[] { 0, 1, 2, 3});
        public static float[] PadVibrate = new float[4];
        public static float PadVibrateFalloff = 1.2f;
        public static bool VibrationEnabled;

        public static float TriggerThreshold = 0.4f;
        public static float StickDirectionThreshold = 0.75f;

        public static Action<int> PadDisconnected = null;

        public static int lastPlayerId = -1;
        public static PlayerIndex LastPlayerIndex
        {
            get { return (PlayerIndex)Math.Min(3, Math.Max(0, lastPlayerId)); }
        }

        public static Dictionary<int, ButtonGeneric[]> ButtonMappings = new Dictionary<int, ButtonGeneric[]>();

#if USE_PCGAMEPAD
        static List<PCGamepad> pcPads = new List<PCGamepad>();
#endif

        static bool captureMouse = false;
        public static bool CaptureMouse
        {
            get { return captureMouse; }
            set 
            { 
                captureMouse = value;
                // Update the zero point
                if (captureMouse && graphicsDevice != null)
                {
                    zeroPointX = graphicsDevice.Viewport.Width / 2;
                    zeroPointY = graphicsDevice.Viewport.Height / 2;

                    Microsoft.Xna.Framework.Input.Mouse.SetPosition(zeroPointX, zeroPointY);
                }
            }
        }

        static bool enabled = true;
        public static bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        static int mouseDX;
        public static int MouseDX
        {
            get { return mouseDX; }
        }

        static int mouseDY;
        public static int MouseDY
        {
            get { return mouseDY; }
        }

        static int mouseDW;
        public static int MouseDW
        {
            get { return mouseDW; }
        }

        public static int MouseX
        {
            get { return currentMouseState.X; }
        }

        public static int MouseY
        {
            get { return currentMouseState.Y; }
        }

#if USE_PCGAMEPAD
        static Input()
        {
            RefreshPCGamepads();
        }
#endif


        /// <summary>
        /// Add any connected PC Gamepads to our list
        /// </summary>
        public static void RefreshPCGamepads()
        {
            // PC Gamepads occupy IDs 4 - 7.
            for (int i = UpdatePadIds.Count - 1; i >= 0; --i)
            {
                if (UpdatePadIds[i] >= 4)
                    UpdatePadIds.RemoveAt(i);
            }

#if USE_PCGAMEPAD
            // We only ask for DI.DeviceType.Joystick here. If we also ask
            // for gamepads, we get the XBox 360 controllers, which we are
            // already handling using XNA.
            DI.DeviceList joystickInstanceList =
                DI.Manager.GetDevices(
                    DI.DeviceType.Joystick,
                    DI.EnumDevicesFlags.AttachedOnly);

            // Add any PC gamepads to our list of pads to update
            int id = 4;
            pcPads.Clear();
            foreach (DI.DeviceInstance deviceInstance in joystickInstanceList)
            {
                if (id >= 8)
                    break;

                PCGamepad gamepad = new PCGamepad(deviceInstance.InstanceGuid);
                pcPads.Add(gamepad);
                UpdatePadIds.Add(id);
                id++;
            }
#endif
        }


        public static void Initialize(GraphicsDevice device)
        {
            graphicsDevice = device;

            SetDefaultGenericMappings();

            // Causes mouse zero point to be initialized
            CaptureMouse = captureMouse;
        }


        public static void Update(float dt)
        {
            if (!enabled)
                return;

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

#if WINDOWS_PHONE || WINRT
            TouchTwinStick.Update(dt);
            lastPadState[0] = CurrentPadState[0];
            CurrentPadState[0] = TouchTwinStick.GetGamePadState();

#elif WINDOWS_PHONE
            if (VibrationEnabled && PadVibrate[0] > 0.0f)
            {
                Microsoft.Devices.VibrateController.Default.Start(
                    TimeSpan.FromSeconds(Math.Min(5.0f, PadVibrate[0] / (2.0f * PadVibrateFalloff))));

                PadVibrate[0] = 0.0f;
            }
#else
            lastKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();

#if NETFX_CORE
            // TODO SharpDX's .Net Core implementation does not have SetVibration() yet
#else
            for (int i = 0; i < 4; i++)
            {
                
                if (PadVibrate[i] > 0f)
                {
                    GamePad.SetVibration((PlayerIndex)i, PadVibrate[i], PadVibrate[i]);
                    PadVibrate[i] -= dt * PadVibrateFalloff;
                    if (PadVibrate[i] < 0f)
                    {
                        PadVibrate[i] = 0f;
                        GamePad.SetVibration((PlayerIndex)i, 0f, 0f);
                    }
                }
            }
#endif


            for (int i = 0; i < UpdatePadIds.Count; i++)
            {
                int id = UpdatePadIds[i];
                lastPadState[id] = CurrentPadState[id];

#if NETFX_CORE
                // TODO SharpDX .Net Core's GamePad has no SetVibration()
#else
                if (id < 4)
                {
                    PlayerIndex pId = (PlayerIndex)id;
                    CurrentPadState[id] = GamePad.GetState(pId, GamePadDeadZone.Circular);

                    // Handle vibration
                    if (CurrentPadState[id].IsConnected && PadVibrate[id] > 0f)
                    {
                        GamePad.SetVibration(pId, PadVibrate[id], PadVibrate[id]);
                        PadVibrate[id] -= dt;
                        if (PadVibrate[id] < 0f)
                        {
                            PadVibrate[id] = 0f;
                            GamePad.SetVibration(pId, 0f, 0f);
                        }
                    }
                }
#endif
#if USE_PCGAMEPAD
                else
                {
                    int pcPadId = id - 4;
                    if (pcPads[pcPadId] != null)
                    {
                        try
                        {

                            CurrentPadState[id] = pcPads[pcPadId].GetState();
                        }
                        catch
                        {
                            pcPads[pcPadId] = null;
                            UpdatePadIds.RemoveAt(i);
							if (PadDisconnected != null)
            				{
                            	PadDisconnected(i);
							}
                            i--;
                        }
                    }
                }
#endif
            }

            if (PadDisconnected != null)
            {
                for (int i = 0; i < UpdatePadIds.Count; i++)
                {
                    if (i < 4)
                    {
                        if (lastPadState[i].IsConnected &&
                            !CurrentPadState[i].IsConnected)
                        {
                            PadDisconnected(i);
                        }
                    }

                }
            }
#endif

            if (captureMouse)
            {
                mouseDX = currentMouseState.X - zeroPointX;
                mouseDY = currentMouseState.Y - zeroPointY;

                Mouse.SetPosition(zeroPointX, zeroPointY);
            }
            else
            {
                mouseDX = currentMouseState.X - lastMouseState.X;
                mouseDY = currentMouseState.Y - lastMouseState.Y;
            }
            mouseDW = currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
        }

        public static bool KeyDown(Keys k)
        {
            return currentKeyState.IsKeyDown(k);
        }
        public static bool KeyJustDown(Keys k)
        {
            return currentKeyState.IsKeyDown(k) && !lastKeyState.IsKeyDown(k);
        }
        public static bool KeyJustUp(Keys k)
        {
            return !currentKeyState.IsKeyDown(k) && lastKeyState.IsKeyDown(k);
        }

        public static bool MouseDown(MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return currentMouseState.LeftButton == ButtonState.Pressed; 
                case MouseButton.Right:
                    return currentMouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return currentMouseState.MiddleButton == ButtonState.Pressed; 
            }
            return false;
        }
        public static bool MouseJustDown(MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return currentMouseState.LeftButton == ButtonState.Pressed &&
                           lastMouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return currentMouseState.RightButton == ButtonState.Pressed &&
                           lastMouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle:
                    return currentMouseState.MiddleButton == ButtonState.Pressed &&
                           lastMouseState.MiddleButton == ButtonState.Released;
            }
            return false;
        }
        public static bool MouseJustUp(MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return currentMouseState.LeftButton == ButtonState.Released &&
                           lastMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return currentMouseState.RightButton == ButtonState.Released &&
                           lastMouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return currentMouseState.MiddleButton == ButtonState.Released &&
                           lastMouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool PadDown(int padID, GamePadButton b)
        {
            return PadDown(ref CurrentPadState[padID], b);
        }
        public static bool PadJustDown(int padID, GamePadButton b)
        {
            return PadDown(ref CurrentPadState[padID], b) && 
                   !PadDown(ref lastPadState[padID], b);
        }
        public static bool PadJustUp(int padID, GamePadButton b)
        {
            return !PadDown(ref CurrentPadState[padID], b) &&
                   PadDown(ref lastPadState[padID], b);
        }
        private static bool PadDown(ref GamePadState padState, GamePadButton b)
        {
            if ((int)b < 21)
            {
                return padState.IsButtonDown((Buttons)(1 << (int)b));
            }

            switch (b)
            {
                case GamePadButton.LeftTrigger:
                    return padState.Triggers.Left > TriggerThreshold;
                case GamePadButton.RightTrigger:
                    return padState.Triggers.Right > TriggerThreshold;
                case GamePadButton.LeftThumbstickDown:
                    return padState.ThumbSticks.Left.Y < -StickDirectionThreshold;
                case GamePadButton.LeftThumbstickUp:
                    return padState.ThumbSticks.Left.Y > StickDirectionThreshold;
                case GamePadButton.LeftThumbstickLeft:
                    return padState.ThumbSticks.Left.X < -StickDirectionThreshold;
                case GamePadButton.LeftThumbstickRight:
                    return padState.ThumbSticks.Left.X > StickDirectionThreshold;
                case GamePadButton.RightThumbstickDown:
                    return padState.ThumbSticks.Right.Y < -StickDirectionThreshold;
                case GamePadButton.RightThumbstickUp:
                    return padState.ThumbSticks.Right.Y > StickDirectionThreshold;
                case GamePadButton.RightThumbstickLeft:
                    return padState.ThumbSticks.Right.X < -StickDirectionThreshold;
                case GamePadButton.RightThumbstickRight:
                    return padState.ThumbSticks.Right.X > StickDirectionThreshold;
                default:
                    return false;
            }
        }

        public static Vector2 PadThumbStickLeft(int padID)
        {
            return CurrentPadState[padID].ThumbSticks.Left;
        }
        public static Vector2 PadThumbStickRight(int padID)
        {
            return CurrentPadState[padID].ThumbSticks.Right;
        }

        public static bool ButtonDownGeneric(ButtonGeneric b)
        {
            int btnID = (int)b;
            if (btnID >= 400)
            {
                // Calculate controller Id
                int padId = (btnID - 400) / 100;
                if (PadDown(padId, (GamePadButton)(btnID % 100)))
                {
                    lastPlayerId = padId;
                    return true;
                }
            }
            else if (btnID < 300)
            {
                if (KeyDown((Keys)btnID))
                {
                    lastPlayerId = 8; //Keyboard;
                    return true;
                }
            }
            else // if (btnID < 400)
            {
                if (MouseDown((MouseButton)(btnID - 300)))
                {
                    lastPlayerId = 10; //Mouse;
                    return true;
                }
            }
            return false;
        }

        public static bool ButtonJustDownGeneric(ButtonGeneric b)
        {
            int btnID = (int)b;
            if (btnID < 300)
            {
                if (KeyJustDown((Keys)btnID))
                {
                    lastPlayerId = 8; //Keyboard;
                    return true;
                }
            }
            else if (btnID < 400)
            {
                if (MouseJustDown((MouseButton)(btnID - 300)))
                {
                    lastPlayerId = 10; //Mouse;
                    return true;
                }
            }
            else 
            {
                int padId = (btnID - 400) / 100;
                if (PadJustDown(padId, (GamePadButton)(btnID % 100)))
                {
                    lastPlayerId = padId;
                    return true;
                }
            }

            return false;
        }
        public static bool ButtonJustUpGeneric(ButtonGeneric b)
        {
            int btnID = (int)b;
            if (btnID < 300)
            {
                if (KeyJustUp((Keys)btnID))
                {
                    lastPlayerId = 8; //Keyboard;
                    return true;
                }
            }
            else if (btnID < 400)
            {
                if (MouseJustUp((MouseButton)(btnID - 300)))
                {
                    lastPlayerId = 10; //Mouse;
                    return true;
                }
            }
            else
            {
                lastPlayerId = (btnID - 400) / 100;
                return PadJustUp(lastPlayerId, (GamePadButton)(btnID % 100));
            }

            return false;
        }

        public static ButtonGeneric PadButtonToGeneric(int playerID, GamePadButton btn)
        {
            return (ButtonGeneric)(400 + 100 * playerID + (int)btn);
        }
        public static ButtonGeneric PadButtonToGeneric(PlayerIndex playerID, GamePadButton btn)
        {
            return (ButtonGeneric)(400 + 100 * (int)playerID + (int)btn);
        }

        public static bool ButtonDownMapped(int btnKey)
        {
            ButtonGeneric[] buttons = ButtonMappings[btnKey];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ButtonDownGeneric(buttons[i]))
                    return true;
            }
            return false;
        }
        public static bool ButtonJustDownMapped(int btnKey)
        {
            ButtonGeneric[] buttons = ButtonMappings[btnKey];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ButtonJustDownGeneric(buttons[i]))
                    return true;
            }
            return false;
        }
        public static bool ButtonJustUpMapped(int btnKey)
        {
            ButtonGeneric[] buttons = ButtonMappings[btnKey];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ButtonJustUpGeneric(buttons[i]))
                    return true;
            }
            return false;
        }


        public static void ClearMappings() { ButtonMappings.Clear(); }
        public static void SetDefaultGenericMappings()
        {
            ButtonMappings.Clear();

            // FORWARD
            List<ButtonGeneric> fwdBtns = new List<ButtonGeneric>();
            fwdBtns.Add(ButtonGeneric.Space);
            fwdBtns.Add(ButtonGeneric.Enter);
            fwdBtns.Add(ButtonGeneric.Pad0A);
            fwdBtns.Add(ButtonGeneric.Pad1A);
            fwdBtns.Add(ButtonGeneric.Pad2A);
            fwdBtns.Add(ButtonGeneric.Pad3A);
#if !XBOX360
            fwdBtns.Add(ButtonGeneric.Pad4A);
            fwdBtns.Add(ButtonGeneric.Pad5A);
            fwdBtns.Add(ButtonGeneric.Pad6A);
            fwdBtns.Add(ButtonGeneric.Pad7A);
#endif
            ButtonMappings.Add((int)MenuInputs.Forward, fwdBtns.ToArray());

            // BACK
            List<ButtonGeneric> backBtns = new List<ButtonGeneric>();
            
            backBtns.Add(ButtonGeneric.Back);
            backBtns.Add(ButtonGeneric.Escape);
            backBtns.Add(ButtonGeneric.Pad0B);
            backBtns.Add(ButtonGeneric.Pad1B);
            backBtns.Add(ButtonGeneric.Pad2B);
            backBtns.Add(ButtonGeneric.Pad3B);
            backBtns.Add(ButtonGeneric.Pad0Back);
            backBtns.Add(ButtonGeneric.Pad1Back);
            backBtns.Add(ButtonGeneric.Pad2Back);
            backBtns.Add(ButtonGeneric.Pad3Back);
#if !XBOX360
            backBtns.Add(ButtonGeneric.Pad4B);
            backBtns.Add(ButtonGeneric.Pad5B);
            backBtns.Add(ButtonGeneric.Pad6B);
            backBtns.Add(ButtonGeneric.Pad7B);
#endif
            ButtonMappings.Add((int)MenuInputs.Backwards, backBtns.ToArray());

            // LEFT
            List<ButtonGeneric> leftBtns = new List<ButtonGeneric>();
            leftBtns.Add(ButtonGeneric.Left);
            leftBtns.Add(ButtonGeneric.Pad0DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad1DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad2DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad3DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad0LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad1LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad2LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad3LeftThumbstickLeft);
#if !XBOX360
            leftBtns.Add(ButtonGeneric.Pad4DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad5DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad6DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad7DPadLeft);
            leftBtns.Add(ButtonGeneric.Pad4LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad5LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad6LeftThumbstickLeft);
            leftBtns.Add(ButtonGeneric.Pad7LeftThumbstickLeft);
#endif
            ButtonMappings.Add((int)MenuInputs.Left, leftBtns.ToArray());

            // RIGHT
            List<ButtonGeneric> rightBtns = new List<ButtonGeneric>();
            rightBtns.Add(ButtonGeneric.Right);
            rightBtns.Add(ButtonGeneric.Pad0DPadRight);
            rightBtns.Add(ButtonGeneric.Pad1DPadRight);
            rightBtns.Add(ButtonGeneric.Pad2DPadRight);
            rightBtns.Add(ButtonGeneric.Pad3DPadRight);
            rightBtns.Add(ButtonGeneric.Pad0LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad1LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad2LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad3LeftThumbstickRight);
#if !XBOX360
            rightBtns.Add(ButtonGeneric.Pad4DPadRight);
            rightBtns.Add(ButtonGeneric.Pad5DPadRight);
            rightBtns.Add(ButtonGeneric.Pad6DPadRight);
            rightBtns.Add(ButtonGeneric.Pad7DPadRight);
            rightBtns.Add(ButtonGeneric.Pad4LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad5LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad6LeftThumbstickRight);
            rightBtns.Add(ButtonGeneric.Pad7LeftThumbstickRight);
#endif
            ButtonMappings.Add((int)MenuInputs.Right, rightBtns.ToArray());

            // UP
            List<ButtonGeneric> upBtns = new List<ButtonGeneric>();
            upBtns.Add(ButtonGeneric.Up);
            upBtns.Add(ButtonGeneric.Pad0DPadUp);
            upBtns.Add(ButtonGeneric.Pad1DPadUp);
            upBtns.Add(ButtonGeneric.Pad2DPadUp);
            upBtns.Add(ButtonGeneric.Pad3DPadUp);
            upBtns.Add(ButtonGeneric.Pad0LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad1LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad2LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad3LeftThumbstickUp);
#if !XBOX360
            upBtns.Add(ButtonGeneric.Pad4DPadUp);
            upBtns.Add(ButtonGeneric.Pad5DPadUp);
            upBtns.Add(ButtonGeneric.Pad6DPadUp);
            upBtns.Add(ButtonGeneric.Pad7DPadUp);
            upBtns.Add(ButtonGeneric.Pad4LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad5LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad6LeftThumbstickUp);
            upBtns.Add(ButtonGeneric.Pad7LeftThumbstickUp);
#endif
            ButtonMappings.Add((int)MenuInputs.Up, upBtns.ToArray());

            // DOWN
            List<ButtonGeneric> downBtns = new List<ButtonGeneric>();
            downBtns.Add(ButtonGeneric.Down);
            downBtns.Add(ButtonGeneric.Pad0DPadDown);
            downBtns.Add(ButtonGeneric.Pad1DPadDown);
            downBtns.Add(ButtonGeneric.Pad2DPadDown);
            downBtns.Add(ButtonGeneric.Pad3DPadDown);
            downBtns.Add(ButtonGeneric.Pad0LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad1LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad2LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad3LeftThumbstickDown);
#if !XBOX360
            downBtns.Add(ButtonGeneric.Pad4DPadDown);
            downBtns.Add(ButtonGeneric.Pad5DPadDown);
            downBtns.Add(ButtonGeneric.Pad6DPadDown);
            downBtns.Add(ButtonGeneric.Pad7DPadDown);
            downBtns.Add(ButtonGeneric.Pad4LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad5LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad6LeftThumbstickDown);
            downBtns.Add(ButtonGeneric.Pad7LeftThumbstickDown);
#endif
            ButtonMappings.Add((int)MenuInputs.Down, downBtns.ToArray());

            // START
            List<ButtonGeneric> startBtns = new List<ButtonGeneric>();
            startBtns.Add(ButtonGeneric.Enter);
            startBtns.Add(ButtonGeneric.Pad0Start);
            startBtns.Add(ButtonGeneric.Pad1Start);
            startBtns.Add(ButtonGeneric.Pad2Start);
            startBtns.Add(ButtonGeneric.Pad3Start);
#if !XBOX360
            startBtns.Add(ButtonGeneric.Pad4Start);
            startBtns.Add(ButtonGeneric.Pad5Start);
            startBtns.Add(ButtonGeneric.Pad6Start);
            startBtns.Add(ButtonGeneric.Pad7Start);
#endif
            ButtonMappings.Add((int)MenuInputs.Start, startBtns.ToArray());
            
            // INFO
            List<ButtonGeneric> infoBtns = new List<ButtonGeneric>();
            infoBtns.Add(ButtonGeneric.LeftShift);
            infoBtns.Add(ButtonGeneric.Pad0Y);
            infoBtns.Add(ButtonGeneric.Pad1Y);
            infoBtns.Add(ButtonGeneric.Pad2Y);
            infoBtns.Add(ButtonGeneric.Pad3Y);
#if !XBOX360
            infoBtns.Add(ButtonGeneric.Pad4Y);
            infoBtns.Add(ButtonGeneric.Pad5Y);
            infoBtns.Add(ButtonGeneric.Pad6Y);
            infoBtns.Add(ButtonGeneric.Pad7Y);
#endif
            ButtonMappings.Add((int)MenuInputs.Info, infoBtns.ToArray());

            // INFO2
            List<ButtonGeneric> info2Btns = new List<ButtonGeneric>();
            info2Btns.Add(ButtonGeneric.LeftControl);
            info2Btns.Add(ButtonGeneric.Pad0X);
            info2Btns.Add(ButtonGeneric.Pad1X);
            info2Btns.Add(ButtonGeneric.Pad2X);
            info2Btns.Add(ButtonGeneric.Pad3X);
#if !XBOX360
            info2Btns.Add(ButtonGeneric.Pad4X);
            info2Btns.Add(ButtonGeneric.Pad5X);
            info2Btns.Add(ButtonGeneric.Pad6X);
            info2Btns.Add(ButtonGeneric.Pad7X);
#endif
            ButtonMappings.Add((int)MenuInputs.Info2, info2Btns.ToArray());

            // EXIT
            List<ButtonGeneric> exitBtns = new List<ButtonGeneric>();
            exitBtns.Add(ButtonGeneric.Escape);
            exitBtns.Add(ButtonGeneric.Pad0Back);
            exitBtns.Add(ButtonGeneric.Pad1Back);
            exitBtns.Add(ButtonGeneric.Pad2Back);
            exitBtns.Add(ButtonGeneric.Pad3Back);
#if !XBOX360
            exitBtns.Add(ButtonGeneric.Pad4Back);
            exitBtns.Add(ButtonGeneric.Pad5Back);
            exitBtns.Add(ButtonGeneric.Pad6Back);
            exitBtns.Add(ButtonGeneric.Pad7Back);
#endif
            ButtonMappings.Add((int)MenuInputs.Exit, exitBtns.ToArray());
        }
    }


//    /// <summary>
//    /// InputHelper. Demonstrates how to use both PC Gamepads 
//    /// and XBox controllers via the same code
//    /// </summary>
//    public static class InputHelper
//    {
//        // Current GamePadStates
//        public static GamePadState[] CurrentPadState = new GamePadState[8];

//        // Last GamePadStates
//        static GamePadState[] lastPadState = new GamePadState[8];

//        // The list of Pad Ids to update (0 - 3) for Xbox gamepads,
//        // (4 - 7) for PC gamepads
//        public static List<int> UpdatePadIds = new List<int>();
        
//#if !XBOX360
//        // Current list of connected PC gamepads
//        static List<PCGamepad> pcPads = new List<PCGamepad>();
//#endif

//        /// <summary>
//        /// Static constructor
//        /// </summary>
//        static InputHelper()
//        {
//            // By default, update all 4 potential XBox controllers
//            UpdatePadIds.AddRange(new int[] { 0, 1, 2, 3 });

//            RefreshPCGamepads();
//        }

//        /// <summary>
//        /// Add any connected PC Gamepads to our list
//        /// </summary>
//        public static void RefreshPCGamepads()
//        {
//            // PC Gamepads occupy IDs 4 - 7.
//            UpdatePadIds.RemoveAll(delegate(int i) { return i >= 4; });

//#if !XBOX360
//            // We only ask for DI.DeviceType.Joystick here. If we also ask
//            // for gamepads, we get the XBox 360 controllers, which we are
//            // already handling using XNA.
//            DI.DeviceList joystickInstanceList =
//                DI.Manager.GetDevices(
//                    DI.DeviceType.Joystick,
//                    DI.EnumDevicesFlags.AttachedOnly);

//            // Add any PC gamepads to our list of pads to update
//            int id = 4;
//            pcPads.Clear();
//            foreach (DI.DeviceInstance deviceInstance in joystickInstanceList)
//            {
//                if (id >= 8)
//                    break;

//                PCGamepad gamepad = new PCGamepad(deviceInstance.InstanceGuid);
//                pcPads.Add(gamepad);
//                UpdatePadIds.Add(id);
//                id++;
//            }
//#endif
//        }

//        /// <summary>
//        /// Call this each frame of your game
//        /// </summary>
//        public static void Update()
//        {
//            for (int i = 0; i < UpdatePadIds.Count; i++)
//            {
//                int id = UpdatePadIds[i];
//                lastPadState[id] = CurrentPadState[id];

//                if (id < 4)
//                {
//                    CurrentPadState[id] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.Circular);
//                }
//#if !XBOX360
//                else
//                {
//                    int pcPadId = id - 4;
//                    CurrentPadState[id] = pcPads[pcPadId].GetState();
//                }
//#endif
//            }
//        }
        
//        /// <summary>
//        /// Pad button state helper
//        /// </summary>
//        /// <param name="padID">The id of the pad to test. Matches PlayerIndex for (0 - 3)</param>
//        /// <param name="b">The button(s) to test</param>
//        /// <returns>True if the button(s) are down</returns>
//        public static bool IsPadDown(int padID, Buttons b)
//        {
//            return CurrentPadState[padID].IsButtonDown(b);
//        }

//        /// <summary>
//        /// Pad button just pressed helper
//        /// </summary>
//        /// <param name="padID">The id of the pad to test. Matches PlayerIndex for (0 - 3)</param>
//        /// <param name="b">The button(s) to test</param>
//        /// <returns>True if the button(s) are down but were up last frame</returns>
//        public static bool IsPadJustDown(int padID, Buttons b)
//        {
//            return CurrentPadState[padID].IsButtonDown(b) &&
//                   lastPadState[padID].IsButtonUp(b);
//        }

//        /// <summary>
//        /// Pad button just released helper
//        /// </summary>
//        /// <param name="padID">The id of the pad to test. Matches PlayerIndex for (0 - 3)</param>
//        /// <param name="b">The button(s) to test</param>
//        /// <returns>True if the button(s) are up but were down last frame</returns>
//        public static bool IsPadJustUp(int padID, Buttons b)
//        {
//            return CurrentPadState[padID].IsButtonUp(b) &&
//                   lastPadState[padID].IsButtonDown(b);
//        }
//    }
}