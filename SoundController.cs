using System;
using System.Collections.Generic;
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
        private CriAtomExPlayer _bonks;
        private CriAtomExAcb _bonkAcb;

        private CriAtomExPlayback _ballrollPlayback;
        private CriAtomExPlayback _sparkPlayback;

        private float startTime;


        private void Awake()
        {
            _player = FindObjectOfType<Player>();

            
            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\ballroll_default.acb");
            string sparkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\spark_default.acb");
            string bonkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\bonks.acb");
            
            // Create new sound player
            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            _bonks = new CriAtomExPlayer();
            
            // Load specific sounds from sound bank
            CriAtomExAcb ballrollAcb = CriAtomExAcb.LoadAcbFile(null, path, null);
            _ballroll.SetCue(ballrollAcb, "1up");
            _ballroll.AttachFader();
            _ballroll.SetFadeOutTime(500);
            _ballroll.SetFadeInTime(200);
            _ballrollPlayback = _ballroll.Start();
            _ballroll.Update(_ballrollPlayback);

            CriAtomExAcb sparkAcb = CriAtomExAcb.LoadAcbFile(null, sparkPath, null);
            _spark.SetCue(sparkAcb, "1up");
            _spark.AttachFader();
            _spark.SetFadeOutTime(500);
            _spark.SetFadeInTime(200);
            _sparkPlayback = _spark.Start();
            _spark.Update(_sparkPlayback);

            _bonkAcb = CriAtomExAcb.LoadAcbFile(null, bonkPath, null);

        }


        private void Update()
        {

            /*Console.WriteLine(Player.ClashSpeedSoft);
            Console.WriteLine(Player.ClashSpeedNormal);
            Console.WriteLine(Player.ClashSpeedSoft);*/
            

            // Calculated speed based off square magnitude, multiplied and rounded to X.XX
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);
            var hit = _player.m_PhysicsBall.m_CollisionSphere.isHit;


            if (_player.m_BoundTimer == 0.1f)
            {
                Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;
                var vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                var intensity = Math.Abs(vectorDot);
                Console.WriteLine("IMPACT: " + Math.Abs(vectorDot));

                    if (intensity <= 7)
                    {
                        _bonks.SetCue(_bonkAcb, "jump");
                        _bonks.SetVolume(1.3f);
                        _bonks.Update(_bonks.Start());
                        _bonks.Start();
                    }
                    else if (intensity > 7 && intensity <= 15.5)
                    {
                        _bonks.SetCue(_bonkAcb, "1up");
                        _bonks.SetVolume(1.3f);
                        _bonks.Update(_bonks.Start());
                    }
                    else
                    {
                        _bonks.SetCue(_bonkAcb, "timer");
                        _bonks.SetVolume(1.0f);
                        _bonks.Update(_bonks.Start());
                }
                
            }

            // My best guess on the linear equations they used in the GC games based on audio
            var ballrollPitch = (float)(10.14f * rounded) - 574.23f;
            var sparkPitch = (float)(11.673f * rounded) - 844.251f;
            var sparkVol = (float)(0.004f * rounded) - 0.011f;

            // Flags to mute sound, prevents audio popping
            if (_player.IsOnGround() == false || rounded <= 3f || GameManager.IsPause())
            {
                _ballroll.Stop();
                _ballroll.Update(_ballrollPlayback);

                _spark.Stop();
                _spark.Update(_sparkPlayback);

                startTime = 0;
            }

            // Flags for enabling sound
            if (_player.IsOnGround() && rounded > 3f)
            {
                // Counts how long enabled flag is lasting
                startTime += Time.deltaTime;

                // If on the ground for more than 0.4 seconds, play sounds
                if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Playing && startTime >= 0.45f)
                {
                    // Max threshold for pitch shifting
                    if (ballrollPitch >= 1100f)
                    {
                        _ballroll.SetPitch(1100f);
                        _ballroll.Update(_ballrollPlayback);
                    }
                    else
                    {
                        _ballroll.SetPitch(ballrollPitch);
                        _ballroll.Update(_ballrollPlayback);
                    }
                }
                else if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Stop && startTime >= 0.45f)
                {
                    _ballrollPlayback = _ballroll.Start();
                    _ballroll.SetPitch(ballrollPitch);
                    _ballroll.Update(_ballrollPlayback);
                }
            }
            
            // Same thing just for spark sounds
            if (_player.IsOnGround() && rounded > 40f && _ballroll.GetStatus() == CriAtomExPlayer.Status.Playing && startTime >= 0.5f)
            {
                if (_spark.GetStatus() == CriAtomExPlayer.Status.Playing)
                {
                    if (sparkPitch >= 1100f)
                    {
                        _spark.SetPitch(1100f);
                        _spark.SetVolume(0.6f);
                        _spark.Update(_sparkPlayback);
                    }
                    else
                    {
                        _spark.SetPitch(sparkPitch);
                        _spark.SetVolume(sparkVol);
                        _spark.Update(_sparkPlayback);
                    }
                }
                else if (_spark.GetStatus() == CriAtomExPlayer.Status.Stop && startTime >= 0.5f)
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