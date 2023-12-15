using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicerTeleporters
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class NicerTeleportersBase : BaseUnityPlugin
    {
        public const string modGUID = "MV.NicerTeleporters";
        public const string modName = "NicerTeleporters";
        public const string modVersion = "1.0.0";

    

        private readonly Harmony harmony = new Harmony(modGUID);
        private static NicerTeleportersBase instance;
        private ManualLogSource mls;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            harmony.PatchAll();

            mls.LogInfo("Patched NicerTeleporters.");
        }


    }
}
