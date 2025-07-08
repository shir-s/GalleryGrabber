
using Utils;


namespace Managers
{
    public static class GameManager
    {
        public static GameOverReason LastGameOverReason { get; set; } 
        public static int MaxDirt { get; set; } = 40;
        public static bool isPlayerCaught { get; set; } = false;
    }

}