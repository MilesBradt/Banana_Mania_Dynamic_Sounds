using System;
using System.IO;
using System.Reflection;
using Flash2;
using Framework.UI;
using UnityEngine;

namespace DynamicRoll
{
    internal class SoundController : MonoBehaviour
    {
        public SoundController(IntPtr value) : base(value) { }

        private Player _player;
        private CriAtomExPlayer _ballroll;
        private CriAtomExPlayer _spark;

        internal void Awake()
        {
            _player = FindObjectOfType<Player>();

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"se_com.acb");

            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            
            CriAtomExAcb acb = CriAtomExAcb.LoadAcbFile(null, path, null);
            _ballroll.SetCue(acb, "ballroll");
            _spark.SetCue(acb, "1up");
            
        }

        internal void Update()
        {
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            // My best guess on the formulas they used in the GC games based on audio
            var pitch = (float)(14.73f * rounded) - 511.08f;
            var sparkPitch = (float)(14.32f * rounded) - 487.13f;

            /*Console.WriteLine(_player.IsOnGround());*/
            Console.WriteLine("rolling: " + _ballroll.GetStatus());
            Console.WriteLine("spark: " + _spark.GetStatus());

            if (_player.IsOnGround() == false || rounded < 4.6f)
            {
                _ballroll.Stop();
                _spark.Stop();
            }

            
            if ((_ballroll.GetStatus() == CriAtomExPlayer.Status.Playing || _ballroll.GetStatus() == CriAtomExPlayer.Status.Prep))
            {
                return;
            } 
            else if (_player.IsOnGround() && rounded >= 4.6f)
            { 
                _ballroll.SetPitch(pitch);
                _ballroll.Update(_ballroll.Start());
            } 

            if ((_spark.GetStatus() == CriAtomExPlayer.Status.Playing || _spark.GetStatus() == CriAtomExPlayer.Status.Prep))
            {
                return;
            } else if (_player.IsOnGround() && rounded >= 22.6f)
            {
                _spark.SetVolume(0.5f);
                _spark.Update(_spark.Start()); 
            }
        }
    }
}