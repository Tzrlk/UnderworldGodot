using System.Diagnostics;

namespace Underworld
{
    public class trap : UWClass
    {
        public static int ObjectThatStartedChain = 0;

        public static void ActivateTrap(int character, uwObject trapObj, uwObject ObjectUsed, int triggerX, int triggerY, uwObject[] objList)
        {
            // if (trapIndex == 0)
            // {
            //     Debug.Print("Trap is at index 0. Do not fire");
            //     return;
            // }
            // var trapObj = objList[trapIndex];
            if (trapObj == null)
            {
                Debug.Print("Null trap");
                return;
            }
            else
            {
                Debug.Print($"Running trap {trapObj.a_name} i:{trapObj.index} q:{trapObj.quality}");
            }
            var triggerNextIndex = trapObj.link; //default object to trigger next. This may change due to the results of a check_variable_trap
            bool implemented = false;
            if (trapObj.majorclass == 6)
            {
                switch (trapObj.minorclass)
                {
                    case 0: // class 6-0 traps
                        {
                            switch (trapObj.classindex)
                            {
                                case 0://damage traps
                                    {
                                        implemented = true;
                                        a_damage_trap.Activate(
                                                trapObj: trapObj,
                                                objList: objList);
                                        break;
                                    }
                                case 1: // a teleport trap
                                    {
                                        implemented = true;
                                        a_teleport_trap.Activate(
                                                trapObj: trapObj,
                                                objList: objList);
                                        break;
                                    }
                                case 2://arrow trap
                                    {
                                        implemented = true;//-ish
                                        an_arrow_trap.Activate(
                                            trapObj: trapObj,
                                            triggerX: triggerX,
                                            triggerY: triggerY,
                                            objList: objList);
                                        break;
                                    }
                                case 3:// Do and hack traps
                                    {
                                        implemented = hack_trap.ActivateHackTrap(
                                                trapObj: trapObj,
                                                ObjectUsed: ObjectUsed,
                                                triggerX: triggerX,
                                                triggerY: triggerY,
                                                objList: objList,
                                                character: character,
                                                ref triggerNextIndex);
                                        break;
                                    }
                                case 4: // pit trap 6-0-4 in uw1, special effects in uw2
                                    {
                                        if (_RES != GAME_UW2)
                                        {//uw1 pit trap
                                            implemented = true;//to continue the chain
                                        }
                                        else
                                        {
                                            implemented = true;
                                            a_specialeffect_trap.Activate(trapObj);
                                        }
                                        break;
                                    }
                                case 5: //change terrain trap
                                    {
                                        implemented = true;
                                        a_change_terrain_trap.Activate(
                                            triggerX: triggerX,
                                            triggerY: triggerY,
                                            trapObj: trapObj);
                                        break;
                                    }
                                case 6: // spell trap
                                    {
                                        implemented = true;
                                        a_spell_trap.Activate(
                                            character: character,
                                            trapObj: trapObj,
                                            objList: UWTileMap.current_tilemap.LevelObjects);
                                        break;
                                    }
                                case 7: //create object 6-0-7
                                    {
                                        implemented = true;
                                        a_create_object_trap.Activate(
                                            triggerX: triggerX,
                                            triggerY: triggerY,
                                            trapObj: trapObj,
                                            objList: objList);
                                        triggerNextIndex = 0;//always stop on create object trap
                                        break;
                                    }
                                case 8://door trap
                                    {
                                        implemented = true;
                                        a_door_trap.Activate(
                                            triggerX: triggerX,
                                            triggerY: triggerY,
                                            trapObj: trapObj,
                                            objList: objList);
                                        break;
                                    }
                                case 0xB: //6-0-B, delete object
                                    {
                                        implemented = true;
                                        a_delete_object_trap.Activate(
                                            trapObj: trapObj,
                                            objList: objList);
                                        triggerNextIndex = 0; ;//always stop on delete object trap
                                        break;
                                    }
                                case 0xC: // 6-0-C, inventory trap
                                    {
                                        triggerNextIndex = an_inventory_trap.Activate(trapObj, objList);
                                        implemented = true;
                                        break;
                                    }
                                case 0xD://set variable trap
                                    {
                                        implemented = true;
                                        a_set_variable_trap.Activate(
                                            trapObj: trapObj);
                                        break;
                                    }
                                case 0xE://check variable trap
                                    {
                                        implemented = true;
                                        triggerNextIndex = a_check_variable_trap.Activate(
                                            trapObj: trapObj,
                                            objList: objList);
                                        break;
                                    }
                                case 0xF://nulltrap/combination trap
                                    {
                                        implemented = true;//null trap does nothing.
                                        break;
                                    }
                            }
                            break;
                        }
                    case 1://class 6-1
                        {
                            switch (trapObj.classindex)
                            {
                                case 0: //6-1-0 Text String Trap
                                    {
                                        implemented = true;
                                        a_text_string_trap.Activate(
                                            trapObj: trapObj,
                                            objList: objList);
                                        break;
                                    }
                                case 1:
                                    {//6-1-1 experience trap
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            an_experience_trap.Activate(trapObj: trapObj);
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        //6-1-2 jump trap
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            a_jump_trap.Activate(trapObj: trapObj);
                                        }
                                        break;
                                    }
                                case 3:// a change_from_trap.
                                    {
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            a_change_from_trap.Activate(
                                                trapObj: trapObj,
                                                triggerX: triggerX,
                                                triggerY: triggerY);
                                        }
                                        break;
                                    }
                                case 4:
                                    {//A change_to_trap. Does nothing but continues execution.
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                        }
                                        break;
                                    }
                                case 5://oscillator (uw2)
                                    {
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            an_oscillator_trap.Activate(
                                                trapObj: trapObj,
                                                triggerX: triggerX,
                                                triggerY: triggerY
                                                );

                                        }
                                        break;
                                    }
                                case 6://proximity trap (uw2)
                                    {
                                        if (_RES == GAME_UW2)
                                        {
                                            //Debug.Print("Skipping proximity trap for testing of chains");
                                            implemented = true;
                                            triggerNextIndex = a_proximity_trap.Activate(
                                                trapObj: trapObj,
                                                triggerX: triggerX,
                                                triggerY: triggerY,
                                                character: character);
                                        }

                                        break;
                                    }
                                case 7://Pit trap (UW2) In UW1 Pit trap is 6,0,4 and does nothing.
                                    {
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            a_pit_trap.Activate(
                                                trapObj: trapObj,
                                                triggerX: triggerX,
                                                triggerY: triggerY);
                                        }
                                        break;
                                    }
                                case 8: //Bridge trap
                                    {
                                        if (_RES == GAME_UW2)
                                        {
                                            implemented = true;
                                            a_bridge_trap.Activate(
                                                trapObj: trapObj,
                                                triggerX: triggerX,
                                                triggerY: triggerY);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
            else
            {
                //non trap class.
                return;
            }

            if (!implemented)
            {
                Debug.Print($"Unknown/unimplemented Trap Class {trapObj.majorclass} {trapObj.minorclass} {trapObj.classindex} {trapObj.a_name} i:{trapObj.index} Q:{trapObj.quality}");
            }
            else
            {
                if (triggerNextIndex != 0)
                {
                    var triggerNextObject = objList[triggerNextIndex];
                    if (triggerNextObject != null)
                    {
                        if (triggerNextObject.majorclass == 6)//only trigger next a trap/trigger.
                        {
                            switch (triggerNextObject.minorclass)
                            {
                                case 0:
                                case 1://traps
                                    ActivateTrap(
                                        character: character,
                                        trapObj: triggerNextObject,
                                        triggerX: triggerX,
                                        triggerY: triggerY,
                                        ObjectUsed: ObjectUsed,
                                        objList: objList); //am i right re-using the original trigger?
                                    break;
                                case 2:
                                case 3://triggers
                                    trigger.RunTrigger(character: character,
                                        ObjectUsed: ObjectUsed,
                                        TriggerObject: triggerNextObject,
                                        triggerType: (int)triggerObjectDat.triggertypes.ALL,
                                        objList: objList);
                                    break;

                            }
                        }
                    }
                }
            }
        }
    }//end class
}//end namespace