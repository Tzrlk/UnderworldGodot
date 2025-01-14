using System.Diagnostics;

namespace Underworld
{
    /// <summary>
    /// Run SCD.ARK Function Calls
    /// </summary>
    public partial class scd : UWClass
    {
        public static int RunSCDFunction(byte[] currentblock, int eventOffset)
        {
            Debug.Print($"Running SCD function {currentblock[eventOffset + 4]} at {eventOffset}");
            switch (currentblock[eventOffset + 4])
            {
                case 0:
                    return 0;//does nothing
                case 1:
                    //Set goal and gtarg
                    return 0;
                case 2://move npcs
                    {
                        return MoveNPCs(
                            currentblock: currentblock,
                            eventOffset: eventOffset);
                    }
                case 3://Kill NPCs
                    {
                        return KillNPCs(
                            currentblock: currentblock,
                            eventOffset: eventOffset);
                    }
                case 4: // Change quest
                    {
                        return ChangeQuest(
                             currentblock: currentblock,
                             eventOffset: eventOffset);
                    }
                case 5: //run trigger
                    {
                        return RunTrigger(
                             currentblock: currentblock,
                             eventOffset: eventOffset);
                    }
                case 6:// does nothing
                    return 0;
                case 7:// Run extended commands
                    return RunSCDFunction_extended(
                             currentblock: currentblock,
                             eventOffset: eventOffset);//runs extra commands defined by eventrow[5]
                case 8://set attitude
                    return 0;
                case 9://perform variable operation
                    scd_variableoperation(
                             currentblock: currentblock,
                             eventOffset: eventOffset);
                    return 0;
                case 10:// run a block of events.
                    {
                        return RunBlock(
                            currentblock: currentblock,
                            eventOffset: eventOffset);
                    }
                case 11://remove object from tile
                    return 0;
            }
            return 0;
        }


        /// <summary>
        /// Additional functions that are called when eventrow[4] == 7. Function defined by eventrow[5]
        /// </summary>
        /// <param name="currentblock"></param>
        /// <param name="eventOffset"></param>
        /// <returns></returns>
        static int RunSCDFunction_extended(byte[] currentblock, int eventOffset)
        {
            Debug.Print($"Running Extended SCD function {currentblock[eventOffset + 5]} at {eventOffset}");
            switch (currentblock[eventOffset + 5])
            {
                case 0:
                    return 0;//does nothing
                case 1:
                    changevariable_by_npcXYHome(
                        currentblock: currentblock, 
                        eventOffset: eventOffset);
                    return 0;//variable operation involving NPC XY Home
                case 2://Maybe move an NPC
                    return 0;
                case 3: //Change a tile
                    return 0;
                case 4://maybe close doors
                    return 0;
                case 5:// does nothing
                    return 0;
                case 6: // Hp Change on npc
                    return 0;
                case 7://maybe move npcs
                    return 0;
            }
            return 0;
        }

        static void RunCodeOnObjects_SCD(CallBacks.UWObjectCallBackWithParams methodToCall, int mode, int filter, bool loopAll, byte[] currentblock, int eventOffset)
        {     
            var paramsArray = MakeParamsArray(currentblock, eventOffset, 0);
            switch(mode)
            {
                case 0:
                    CallBacks.RunCodeOnNPCS_WhoAmI(
                        methodToCall: methodToCall, 
                        whoami: filter, 
                        paramsArray: paramsArray, 
                        loopAll: loopAll);
                    break;
                case 1:
                    CallBacks.RunCodeOnRace(
                        methodToCall: methodToCall, 
                        race: filter, 
                        paramsArray: paramsArray, 
                        loopAll: loopAll);
                    break;
                case 2:
                case 3:
                    break;
            }
        }


        /// <summary>
        /// Creates an int[] of params starting at the offset specified.
        /// </summary>
        /// <param name="currentblock"></param>
        /// <param name="eventOffset"></param>
        /// <param name="paramOffset"></param>
        /// <returns></returns>
        static int[] MakeParamsArray(byte[] currentblock, int eventOffset, int paramOffset)
        {
            int[] paramsArray = new int[16-paramOffset];
            for (int i= 0; i<16-paramOffset;i++)
            {
                paramsArray[i] = currentblock[eventOffset+i];
            }
            return paramsArray;
        }

    }//end class
}//end namespace