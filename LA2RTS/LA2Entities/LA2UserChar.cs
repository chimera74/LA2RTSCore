namespace LA2RTS.LA2Entities
{

    // Character in LA2 that we can control
    public class LA2UserChar : LA2Char
    {
        public RTSClient client;

        public enum ClientStatus
        {
            Off = 0,
            CharSelect = 1,
            InGame = 2
        }

        public ClientStatus Status;

        public bool CanCryst;//: Boolean; Может кристализовать предметы наш герой или нет?
        public int Charges;//: Cardinal; для гладов зарядки
        public int WeightPenalty;//: Cardinal;
        public int WeapPenalty;//: Cardinal;
        public int ArmorPenalty;//: Cardinal;
        public int DeathPenalty;//: Cardinal;
        public int Souls;//: Cardinal;

        public LA2UserChar(RTSClient rtsClient)
        {
            client = rtsClient;
        }
        
    }
}
