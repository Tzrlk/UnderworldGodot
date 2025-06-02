namespace Underworld
{

    /// <summary>
    /// Trap that does an acrobat skill check when player is standing on a tile. If check is failed the ground below the player will collapse if the floor texture matches a trap owner value
    /// </summary>
    public class a_hack_trap_floorcollapse : trap
    {
        public static void Activate(uwObject trapObj, int triggerX, int triggerY, uwObject[] objList)
        {

            var playerTile = UWTileMap.current_tilemap.Tiles[playerdat.tileX_depreciated, playerdat.tileY_depreciated];
            if (playerTile.floorTexture == trapObj.owner)
            {
                var playerWeight = playerdat.WeightCarried;

                int checkvalue = 0;
                if (playerWeight / 12 > 20)
                {
                    checkvalue = playerWeight / 12;
                }

                var skillcheckresult = playerdat.SkillCheck(playerdat.Acrobat, checkvalue);
                if (skillcheckresult < 0)
                {
                    var si_floorheight = playerTile.floorHeight - 1;
                    var newTexture = (int)((trapObj.ypos << 3) | (int)trapObj.xpos);

                    if (si_floorheight < 0)
                    {
                        si_floorheight = 0;//new height
                    }
                    
                    TileInfo.ChangeTile(
                        StartTileX: playerdat.tileX_depreciated,
                        StartTileY: playerdat.tileY_depreciated,
                        newHeight: si_floorheight,
                        newFloorTexture: newTexture
                       );                   

                }
            }

        }
    }//end class
}//end namespace