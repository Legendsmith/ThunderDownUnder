using BepInEx;
using UnityEngine;
using RoR2;


namespace ThunderDownUnder.SolidIceWall
{
    [BepInPlugin("com.ThunderDownUnder.SolidIcewall", "SolidIceWall", "1.1.0")]
    public class SolidIceWallLoader : BaseUnityPlugin
    
    {
        public void Awake()
        {
            Chat.AddMessage("Loaded SolidIcewall!");
            GameObject pillarprefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            pillarprefab.layer = 11;
        }
    }
}
