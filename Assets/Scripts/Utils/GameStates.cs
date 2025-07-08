namespace Utils
{
    public static class GameStates
    {
        public static GameOverReason LastGameOverReason { get; set; } 
        public static int MaxDirt { get; set; } = 20;
        public static bool isPlayerCaught { get; set; } = false;
    }

}