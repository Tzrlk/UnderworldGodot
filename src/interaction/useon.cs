using System;

namespace Underworld
{
    /// <summary>
    /// Class for interactions involving the use verb when holding another object
    /// </summary>
    public class useon : UWClass
    {
        public static useon CurrentItemBeingUsed;
        public int index = 0;

        public bool WorldObject;

        public uwObject itemBeingUsed;

        public useon(uwObject obj, bool _worldobject)
        {
            index = obj.index;
            WorldObject = _worldobject;
            itemBeingUsed = obj;
            //change the mouse cursor
            uimanager.instance.mousecursor.SetCursorToObject(obj.item_id);
        }

        public string GeneralUseOnString
        {
            get
            {
                return $"Use {GameStrings.GetObjectNounUW(itemBeingUsed.item_id, 0)} on?";
            }
        }

        public static bool UseOn(uwObject ObjectUsed, useon srcObject, bool WorldObject)
        {

            var ObjInHand = srcObject.itemBeingUsed;
            if (ObjectUsed == null) { return false; }
            trap.ObjectThatStartedChain = ObjectUsed.index;
            bool result = false;
            // if (index <= targetobjList.GetUpperBound(0))
            // {
            //     var targetObj = targetobjList[index];
            switch (ObjInHand.majorclass)
            {
                case 2:
                    {
                        //result = UseOnMajorClass2(obj, objList, WorldObject);
                        break;
                    }
                case 3:
                    {
                        result = UseOnMajorClass3(ObjInHand, ObjectUsed, WorldObject);
                        break;
                    }
                case 4:
                    {
                        result = UseOnMajorClass4(ObjInHand, ObjectUsed, WorldObject);
                        break;
                    }
                case 5:
                    {
                        //result = UseOnMajorClass5(obj, objList, WorldObject);
                        break;
                    }
            }
            uwObject[] targetobjList;
            if (WorldObject)
            {
                targetobjList = UWTileMap.current_tilemap.LevelObjects;
            }
            else
            {
                targetobjList = playerdat.InventoryObjects;
            }
            //Check for use trigger on this action and try activate if so.
            trigger.TriggerObjectLink
            (
                character: 0,
                ObjectUsed: ObjectUsed,
                triggerType: (int)triggerObjectDat.triggertypes.USE,
                triggerX: ObjectUsed.tileX,
                triggerY: ObjectUsed.tileY,
                objList: targetobjList
            );

            //Clear the cursor icons
            CurrentItemBeingUsed = null;
            // playerdat.ObjectInHand = -1;
            uimanager.instance.mousecursor.SetCursorToCursor();

            return false;
        }

        public static bool UseOnMajorClass3(uwObject objInHand, uwObject targetObject, bool WorldObject)
        {
            switch (objInHand.minorclass)
            {
                case 1:
                    {//some misc items                    
                        switch (objInHand.classindex)
                        {
                            case 7://anvil
                                return anvil.UseOn(objInHand, targetObject, WorldObject);
                            case 8://pole
                                return pole.UseOn(objInHand, targetObject, WorldObject);
                        }
                        break;
                    }
                case 2:
                    {
                        if (_RES != GAME_UW2)
                        {
                            switch (objInHand.classindex)
                            {
                                case 7://key of infinity
                                    return key_of_infinity.UseOn(objInHand, targetObject, WorldObject);
                            }
                        }
                        break;
                    }
            }
            return false;
        }

        public static bool UseOnMajorClass4(uwObject objInHand, uwObject targetObject, bool WorldObject)
        {
            switch (objInHand.minorclass)
            {
                case 0: //keys up to 0xE
                    {
                        if (_RES == GAME_UW2)
                        {
                            switch (objInHand.classindex)
                            {
                                case 0:
                                case 1:
                                    //LOCKPICK and curious implement.
                                    return lockpick.UseOn(objInHand, targetObject);
                                case > 1 and < 0xE://keys
                                    return doorkey.UseOn(objInHand, targetObject);
                            }
                        }
                        else
                        {
                            switch (objInHand.classindex)
                            {
                                case 0:
                                case > 1 and < 0xE://keys
                                    return doorkey.UseOn(objInHand, targetObject);
                                case 1://lockpick
                                    return lockpick.UseOn(objInHand, targetObject);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        if (_RES == GAME_UW2)
                        {
                            switch (objInHand.classindex)
                            {
                                case >= 8 and <= 0xF:
                                    return smallblackrockgem.UseOn(objInHand, targetObject, WorldObject);
                            }
                        }
                        break;
                    }
                case 2:// misc usables
                    {
                        switch (objInHand.classindex)
                        {
                            case 0x7:   //spike in uw1
                                {
                                    if (_RES != GAME_UW2)
                                    {
                                        return spike.UseOn(objInHand, targetObject, WorldObject);
                                    }
                                    break;
                                }
                            case 0x8://rock hammer
                                {
                                    return rockhammer.UseOn(objInHand, targetObject, WorldObject);
                                }
                            case 0xD://oil flask
                                {
                                    return oilflask.UseOn(objInHand, targetObject);
                                }
                        }
                        break;
                    }
            }
            return false;
        }

    } //end class
} //end namespace