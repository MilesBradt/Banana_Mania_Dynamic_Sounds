using System;
using System.Collections.Generic;
using Flash2;
using UnhollowerRuntimeLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DynamicRoll
{
    // Honestly I don't know C# well enough, this just works and I'm not touching it
    public static class Main
    {
        private static SoundController _dynamicRoll;
        private static CameraController _cameraController;

        public static List<Type> OnModLoad(Dictionary<string, object> settings)
        {
            return new List<Type> { typeof(SoundController) };
        }

        public static void OnModUpdate()
        {
            var player = Object.FindObjectOfType<Player>();
   
            if (player == null) 
            {
                Object.Destroy(_dynamicRoll);
                _dynamicRoll = null;
                return; 
            } 

            if (_dynamicRoll == null)
            {
                _cameraController = player.GetCameraController();
                _dynamicRoll = new SoundController(_cameraController.gameObject.AddComponent(Il2CppType.Of<SoundController>()).Pointer); 
            } 
        }
    }
}