namespace Underworld
{
    public partial class ConversationVM {
        public static void do_decline(uwObject talker)
        {
            for (int i=0;i<uimanager.NoOfTradeSlots;i++)
            {
                uimanager.SetNPCTradeSlot(i,-1,false);
            }
        }
    }//end class
}//end namespace