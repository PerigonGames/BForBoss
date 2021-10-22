namespace BForBoss
{
    public static class DreamloData
    {
        public static string Host = "http://dreamlo.com/lb/";

        // If anyone get's to read this, NEVER PUT SECRET ON FRONT END. ONLY ON BACKEND
        // THIS IS JUST DATA WE DON'T CARE ABOUT
        public static string Secret
        {
            get
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                return "4GsDJUyAWEeeeSfAUlimgwX8RwpzhjOUm2mBqGemFh7A";
#endif
                return "HP6OVeNT206-R5KUiyCaGAwbb9BGNVgUeUWZvSMf-rag";
            }
        }

        public static string Public
        {
            get
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                return "614aafc28f40bb0e28687778";
#endif
                return "61726da08f40bba8b4ee9863";
            }
        } 
    }
}
