using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        // These are ugly, I don't want to keep them
        private List<float> _boundArray;
        private List<float> _collideArray;
        private List<float> _dropArray;
        private List<float> _softArray;
        private int _collideInt = 0;
        private int _dropInt = 0;
        private int _softInt = 0;
        private float _groundTime;
        private float _intensity;

        private void Awake()
        {

            _player = FindObjectOfType<Player>();

            _boundArray = new List<float>();
            _collideArray = new List<float>();
            _dropArray = new List<float>();
            _softArray = new List<float>();

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
            _ballroll.SetFadeOutTime(200);
            _ballroll.SetFadeInTime(100);
            _ballrollPlayback = _ballroll.Start();
            _ballroll.Update(_ballrollPlayback);

            CriAtomExAcb sparkAcb = CriAtomExAcb.LoadAcbFile(null, sparkPath, null);
            _spark.SetCue(sparkAcb, "1up");
            _spark.AttachFader();
            _spark.SetFadeOutTime(200);
            _spark.SetFadeInTime(100);
            _sparkPlayback = _spark.Start();
            _spark.Update(_sparkPlayback);

            _bonkAcb = CriAtomExAcb.LoadAcbFile(null, bonkPath, null);

        }


        private void Update()
        {
            // Calculated speed based off square magnitude, multiplied and rounded to X.XX
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            float soft = _player.m_PhysicsBall.m_CollisionSphere.m_IntentPos.sqrMagnitude.CompareTo(_player.m_PhysicsBall.m_Pos.sqrMagnitude);

            if (soft != -1)
            {
                Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                float drop = Math.Abs(_player.m_PhysicsBall.m_Pos.y - _player.m_PhysicsBall.m_OldPos.y);
                _intensity = Math.Abs(vectorDot);

                _collideArray.Insert(_collideInt++, soft);
                _dropArray.Insert(_dropInt++, drop);
                _softArray.Insert(_softInt++, _intensity);
            }

            if (_player.m_BoundTimer > 0)
            {
                _collideArray.Clear();
                _dropArray.Clear();
                _softArray.Clear();
                _collideInt = 0;
                _dropInt = 0;
                _softInt = 0;

                Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                int i = 0;
                float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                _intensity = Math.Abs(vectorDot);

                _boundArray.Insert(i++, _intensity);
            }
            else if (_player.m_BoundTimer <= 0 && _boundArray.Any())
            {
                float maxIntensity = _boundArray.Max();

                if (maxIntensity <= 6)
                {
                    _bonks.SetCue(_bonkAcb, "jump");
                    _bonks.SetVolume(1.2f);
                    _bonks.Update(_bonks.Start());
                    _bonks.Start();
                }
                else if (maxIntensity > 6 && maxIntensity <= 8.5)
                {
                    _bonks.SetCue(_bonkAcb, "1up");
                    _bonks.SetVolume(1.1f);
                    _bonks.Update(_bonks.Start());
                }
                else
                {
                    _bonks.SetCue(_bonkAcb, "timer");
                    _bonks.SetVolume(0.9f);
                    _bonks.Update(_bonks.Start());
                }
                _boundArray.Clear();
            }
            else if (_collideArray.Any() && soft == -1 && _intensity > 1.5f)
            {
                float maxDrop = _dropArray.Max();

                if (maxDrop >= 0.045f)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();
                    _collideInt = 0;
                    _dropInt = 0;
                    _softInt = 0;
                    _bonks.SetCue(_bonkAcb, "ballbound");
                    _bonks.SetVolume(1.2f);
                    _bonks.Update(_bonks.Start());
                }
                else if (maxDrop <= 0.02)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();
                    _collideInt = 0;
                    _dropInt = 0;
                    _softInt = 0;
                    _bonks.SetCue(_bonkAcb, "ballbound");
                    _bonks.SetVolume(1.2f);
                    _bonks.Update(_bonks.Start());
                }
                else
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();
                    _collideInt = 0;
                    _dropInt = 0;
                    _softInt = 0;
                }

            }

            // My best guess on the linear equations they used in the GC games based on audio
            var ballrollPitch = (float)(10.14f * rounded) - 574.23f;
            var sparkPitch = (float)(11.673f * rounded) - 844.251f;
            var sparkVol = (float)(0.004f * rounded) - 0.011f;

            if (!_player.IsOnGround())
            {
                _groundTime = 0;
            }

            // Flags to mute sound, prevents audio popping
            if (!_player.IsOnGround() || rounded <= 3f || GameManager.IsPause())
            {
                _ballroll.SetVolume(0.0f);
                _ballroll.Update(_ballrollPlayback);

                _spark.SetVolume(0.0f);
                _spark.Update(_sparkPlayback);

            }

            if (!_player.IsOnGround() || rounded <= 40f || GameManager.IsPause())
            {
                _spark.SetVolume(0.0f);
                _spark.Update(_sparkPlayback);
            }


            // Flags for enabling sound
            if (_player.IsOnGround() && rounded > 3f)
            {
                // Counts how long enabled flag is lasting
                _groundTime += Time.deltaTime;

                // If on the ground for more than 0.4 seconds, play sounds
                if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Playing && _groundTime >= 0.3f)
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
                else if (_ballroll.GetStatus() == CriAtomExPlayer.Status.Stop && _groundTime >= 0.3f)
                {
                    _ballrollPlayback = _ballroll.Start();
                    _ballroll.SetVolume(1f);
                    _ballroll.SetPitch(ballrollPitch);
                    _ballroll.Update(_ballrollPlayback);
                }
            }

            // Same thing just for spark sounds
            if (_player.IsOnGround() && rounded > 40f && _groundTime >= 0.3f)
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
                else if (_spark.GetStatus() == CriAtomExPlayer.Status.Stop && _groundTime >= 0.5f)
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

