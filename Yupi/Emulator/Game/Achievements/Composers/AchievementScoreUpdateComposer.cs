/**
     Because i love chocolat...                                      
                                    88 88  
                                    "" 88  
                                       88  
8b       d8 88       88 8b,dPPYba,  88 88  
`8b     d8' 88       88 88P'    "8a 88 88  
 `8b   d8'  88       88 88       d8 88 ""  
  `8b,d8'   "8a,   ,a88 88b,   ,a8" 88 aa  
    Y88'     `"YbbdP'Y8 88`YbbdP"'  88 88  
    d8'                 88                 
   d8'                  88     
   
   Private Habbo Hotel Emulating System
   @author Claudio A. Santoro W.
   @author Kessiler R.
   @version dev-beta
   @license MIT
   @copyright Sulake Corporation Oy
   @observation All Rights of Habbo, Habbo Hotel, and all Habbo contents and it's names, is copyright from Sulake
   Corporation Oy. Yupi! has nothing linked with Sulake. 
   This Emulator is Only for DEVELOPMENT uses. If you're selling this you're violating Sulakes Copyright.
*/

using Yupi.Emulator.Messages;
using Yupi.Emulator.Messages.Buffers;

namespace Yupi.Emulator.Game.Achievements.Composers
{
    /// <summary>
    ///     Class AchievementScoreUpdateComposer.
    /// </summary>
    internal class AchievementScoreUpdateComposer
    {
        /// <summary>
        ///     Composes the specified score.
        /// </summary>
        /// <param name="score">The score.</param>
        /// <returns>SimpleServerMessageBuffer.</returns>
        internal static SimpleServerMessageBuffer Compose(uint score)
        {
            SimpleServerMessageBuffer simpleServerMessageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("AchievementPointsMessageComposer"));

            simpleServerMessageBuffer.AppendInteger(score);

            return simpleServerMessageBuffer;
        }
    }
}