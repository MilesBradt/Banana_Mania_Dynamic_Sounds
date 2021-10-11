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
        private CriAtomExPlayback _playback;
        private CriAtomExPlayback _sparkPlayback;

        internal void Awake()
        {
            _player = FindObjectOfType<Player>();

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ballroll_default.acb");
            string sparkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"spark_default.acb");

            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            
            CriAtomExAcb acb = CriAtomExAcb.LoadAcbFile(null, path, null);
            _ballroll.SetCue(acb, "1up");
            _playback = _ballroll.Start();
            _ballroll.Update(_playback);

            CriAtomExAcb sparkAcb = CriAtomExAcb.LoadAcbFile(null, sparkPath, null);
            _spark.SetCue(sparkAcb, "1up");
            _sparkPlayback = _spark.Start();
            _spark.Update(_sparkPlayback);
           

        }

        internal void Update()
        {
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            // My best guess on the formulas they used in the GC games based on audio
            var pitch = (float)(10.14f * rounded) - 574.23f;
            var sparkPitch = (float)(14.32f * rounded) - 487.13f;

            if (_player.IsOnGround() == false || rounded <= 5f || GameManager.IsPause())
            {
                _ballroll.Stop();
                _spark.Stop();
            }

            if (_player.IsOnGround() && rounded > 5f)
            {
                if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Playing)
                {
                    if (pitch >= 1100f)
                    {
                        _ballroll.SetPitch(1100f);
                        _ballroll.Update(_playback);
                    }
                    else
                    {
                        _ballroll.SetPitch(pitch);
                        _ballroll.Update(_playback);
                    }
                }
                else if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Stop)
                {
                    _playback = _ballroll.Start();
                    _ballroll.SetPitch(pitch);
                    _ballroll.Update(_playback);
                }
            }
            
            if (_player.IsOnGround() && rounded > 40f && _ballroll.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                if (_spark.GetStatus() == CriAtomExPlayer.Status.Playing)
                {
                    if (pitch >= 1100f)
                    {
                        _spark.SetPitch(1100f);
                        _spark.Update(_sparkPlayback);
                    }
                    else
                    {
                        _spark.SetPitch(pitch);
                        _spark.Update(_sparkPlayback);
                    }
                }
                else if (_spark.GetStatus() == CriAtomExPlayer.Status.Stop)
                {
                    _sparkPlayback = _spark.Start();
                    _spark.SetPitch(pitch);
                    _spark.Update(_sparkPlayback);
                }
            }
        }



        private void OnDisable()
        {
            _ballroll.Stop();
            _spark.Stop();
        }
    }
}