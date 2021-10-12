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
        private CriAtomExPlayback _ballrollPlayback;
        private CriAtomExPlayback _sparkPlayback;

        private float startTime;

        internal void Awake()
        {
            _player = FindObjectOfType<Player>();

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ballroll_default.acb");
            string sparkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"spark_default.acb");

            // Create new sound player
            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            
            // Load specific sounds from sound bank
            CriAtomExAcb ballrollAcb = CriAtomExAcb.LoadAcbFile(null, path, null);
            _ballroll.SetCue(ballrollAcb, "1up");
            _ballrollPlayback = _ballroll.Start();
            _ballroll.Update(_ballrollPlayback);

            CriAtomExAcb sparkAcb = CriAtomExAcb.LoadAcbFile(null, sparkPath, null);
            _spark.SetCue(sparkAcb, "1up");
            _sparkPlayback = _spark.Start();
            _spark.Update(_sparkPlayback);

        }

        internal void Update()
        {
            // Calculated speed based off square magnitude, multiplied and rounded to X.XX
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            // My best guess on the linear equations they used in the GC games based on audio
            var ballrollPitch = (float)(10.14f * rounded) - 574.23f;
            var sparkPitch = (float)(11.673f * rounded) - 844.251f;
            var sparkVol = (float)(0.004f * rounded) - 0.008f;

            // Flags to mute sound, prevents audio popping
            if (_player.IsOnGround() == false || rounded <= 4f || GameManager.IsPause())
            {
                _ballroll.SetVolume(0.0f);
                _ballroll.Update(_ballrollPlayback);

                _spark.SetVolume(0.0f);
                _spark.Update(_sparkPlayback);

                startTime = 0;
            }

            // Flags for enabling sound
            if (_player.IsOnGround() && rounded > 4f)
            {
                // Counts how long enabled flag is lasting
                startTime += Time.deltaTime;
   
                // If on the ground for more than 0.4 seconds, play sounds
                if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Playing && startTime >= 0.4f)
                {
                    // Max threshold for pitch shifting
                    if (ballrollPitch >= 1100f)
                    {
                        _ballroll.SetVolume(1f);
                        _ballroll.SetPitch(1100f);
                        _ballroll.Update(_ballrollPlayback);
                    }
                    else
                    {
                        _ballroll.SetVolume(1f);
                        _ballroll.SetPitch(ballrollPitch);
                        _ballroll.Update(_ballrollPlayback);
                    }
                }
                else if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Stop && startTime >= 0.4f)
                {
                    _ballrollPlayback = _ballroll.Start();
                    _ballroll.SetVolume(1f);
                    _ballroll.SetPitch(ballrollPitch);
                    _ballroll.Update(_ballrollPlayback);
                }
            }
            
            // Same thing just for spark sounds
            if (_player.IsOnGround() && rounded > 40f && _ballroll.GetStatus() == CriAtomExPlayer.Status.Playing && startTime >= 0.4f)
            {
                if (_spark.GetStatus() == CriAtomExPlayer.Status.Playing)
                {
                    if (sparkPitch >= 1100f)
                    {
                        _spark.SetPitch(1100f);
                        _spark.SetVolume(0.7f);
                        _spark.Update(_sparkPlayback);
                    }
                    else
                    {
                        _spark.SetPitch(sparkPitch);
                        _spark.SetVolume(sparkVol);
                        _spark.Update(_sparkPlayback);
                    }
                }
                else if (_spark.GetStatus() == CriAtomExPlayer.Status.Stop && startTime >= 0.4f)
                {
                    _sparkPlayback = _spark.Start();
                    _spark.SetPitch(sparkPitch);
                    _spark.SetVolume(sparkVol);
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