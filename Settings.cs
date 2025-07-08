namespace SharpBotTopOnline
{
    public class Settings
    {
        public const string TOKEN_BM = ""; // token access token
        public const string TOKEN_BOT = ""; // discord bot token
        public const int SERVER_ID_SQ_1 = 30985204;
        public const int SERVER_ID_SQ_2 = 31020814;
        //public const int SERVER_ID_SQ_3 = 31020814;


        private static List<ulong> admins_ids = [721996501999550485, 
            333565316372365313, 
            209329835762253824, 
            250745445838487562, 
            284011575071866880, 
            528240938762502166, 
            753980811107106846];

        public static List<ulong> ADMIN_IDS { get => admins_ids; set => admins_ids = value; }
    }
}