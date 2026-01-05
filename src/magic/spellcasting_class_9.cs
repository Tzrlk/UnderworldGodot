namespace Underworld
{
    public partial class SpellCasting {
        /// <summary>
        /// Applies curse object damage
        /// Assumes it can only affect the player character.
        /// </summary>
        /// <param name="minorclass"></param>
        /// <param name="CastOnEquip">Has the cursed item just been equiped</param>
        public static void CastClass9_Curse(int minorclass, bool CastOnEquip)
        {
            var dmg = Rng.DiceRoll(minorclass, 8);
            if (playerdat.play_hp - dmg >= 3)
            {
                playerdat.play_hp -= dmg;
            }
            else
            {
                playerdat.play_hp = 3;
            }
            if (CastOnEquip)
            {
                if (GameConfig.GameSelected == Game.Uw2)
                {
                    uimanager.AddToMessageScroll(GameStrings.GetString(1, 362));
                }
            }
            else
            {
                if (GameConfig.GameSelected == Game.Uw2)
                {
                    uimanager.FlashColour(0x30, uimanager.Cuts3DWin, 0.1f);
                }
                else
                {
                    uimanager.FlashColour(0xA8, uimanager.Cuts3DWin, 0.1f);
                }
            }
        }
    }//end class
}//end namespace