using System;
using Dirt;
using Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public static class GameManager
    {
        public static GameOverReason LastGameOverReason { get; set; } 
        public static int MaxDirt { get; set; } = 50;
    }

}