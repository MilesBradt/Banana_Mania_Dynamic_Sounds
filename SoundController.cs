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
        private CriAtomExPlayer _audio;

        internal void Awake()
        {
            _player = FindObjectOfType<Player>();

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"se_com.acb");

            _audio = new CriAtomExPlayer();
            
            CriAtomExAcb acb = CriAtomExAcb.LoadAcbFile(null, path, null);
            _audio.SetCue(acb, "ballroll");
            _audio.Start();
        }

        internal void Update()
        {
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            /*Console.WriteLine(rounded); */

            if (_audio.GetStatus() == CriAtomExPlayer.Status.Playing || _audio.GetStatus() == CriAtomExPlayer.Status.Prep)
            {
                return;
            }
            else
            {
                _audio.SetPitch(playerSpeed);
                _audio.SetPlaybackRatio(_player.m_PhysicsBall.m_Velocity.sqrMagnitude * 10f);
                _audio.Update(_audio.Start());
            }
        }
    }
}