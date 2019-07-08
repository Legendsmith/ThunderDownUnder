using BepInEx;
using UnityEngine;
using RoR2;


namespace SolidIceWall
{
    public class SolidIceWallLoader

    {
        public void Awake()
        {
            Chat.AddMessage("Loaded SolidIcewall!");
            GameObject pillarprefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            pillarprefab.layer = 11;
        }
    }
}
