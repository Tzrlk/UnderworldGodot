using System;

namespace Underworld
{
    /// <summary>
    /// Base class
    /// </summary>
    [Obsolete("No need to use this as a base class.")]
    public class UWClass
    {
        public const byte GAME_UWDEMO = 0;//"UW0";
        public const byte GAME_UW1 = 1; // "UW1";
        public const byte GAME_UW2 = 2; // "UW2";

        /// <summary>
        /// Use to track what game is currently active.
        /// </summary>
        [Obsolete("Use GameConfig.GameToLoad instead")]
        public static byte _RES
	        => (byte)GameConfig.GameSelected;

        [Obsolete("Use GameConfig.GamePath instead")]
        public static string BasePath
	        => GameConfig.GamePath;

    }//end class
}//end namespace