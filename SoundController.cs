using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Flash2;
using Framework.UI;
using UnityEngine;
using static Flash2.Chara;

namespace DynamicRoll
{
    
    class SpeedToPitch
    {
        public void setPitch(float pitch, CriAtomExPlayer audio, CriAtomExPlayback playback)
        {
            if (pitch >= 1100f)
            {
                audio.SetPitch(1100f);
                audio.Update(playback);
            } else
            {
                audio.SetPitch(pitch);
                audio.Update(playback);
            } 
        }

        public void setSparkVol(float vol, CriAtomExPlayer audio, CriAtomExPlayback playback)
        {
            if (vol >= 0.6f)
            {
                audio.SetVolume(0.6f);
                audio.Update(playback);
            } else
            {
                audio.SetVolume(vol);
                audio.Update(playback);
            }
        }
    }

    class ImpactAudio
    {
        public void playSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if(sfx != "timer")
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.2f);
                audio.Update(playback);
            } else
            {
                audio.Start();
            }
        }
    }

    class Impact : ImpactAudio
    {
        public void calcImpact (CriAtomExPlayer audio, CriAtomExAcb acb, float impact)
        {
            if (impact <= 6.5)
            {
                playSfx(audio, acb, "jump");
            }
            else if (impact > 6.5 && impact <= 9.5)
            {

                playSfx(audio, acb, "1up");
            }
            else
            {
                playSfx(audio, acb, "timer");
            }
        }

        public void softImpact (CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            playSfx(audio, acb, "ballbound");
        }
    }

    class MonkeeAudio
    {
        public void playSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if (audio.GetStatus() != CriAtomExPlayer.Status.Playing)
            {
                audio.Start();
            }
            
        }
    }

    class Monkee : MonkeeAudio
    {
        public void calcImpact(CriAtomExPlayer audio, CriAtomExAcb acb, float impact, Player player)
        {
            if (impact <= 6.5)
            {
                playSfx(audio, acb, "goal_fly");
            }
            else if (impact > 6.5 && impact <= 9.5)
            {

                playSfx(audio, acb, "hanauta");
            }
            else
            {
                if (player.charaKind.ToString().ToLower() == "yanyan")
                {
                    playSfx(audio, acb, "yabai_long");
                } else
                {
                    playSfx(audio, acb, "thankyou");
                }
            }
        }

        public void calcBanana(CriAtomExPlayer audio, CriAtomExAcb acb, int banana)
        {
            if (banana == 1)
            {
                playSfx(audio, acb, "fallout");
            }
            else if (banana == 10)
            {
                playSfx(audio, acb, "timeover");
            }
        }
    }

    class BananaAudio
    {
        public void playSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }
    }

    class Banana : BananaAudio
    {
        public void calcCollected(CriAtomExPlayer audio, CriAtomExAcb acb, int banana)
        {
            if (banana == 1)
            {
                playSfx(audio, acb, "1up");
            } else if (banana == 10)
            {
                playSfx(audio, acb, "timer");
            }
        }
    }


    internal class SoundController : MonoBehaviour
    {
        public SoundController(IntPtr value) : base(value) { }

        private Player _player;
        private SpeedToPitch _speedToPitch;
        private Impact _impact;
        private Monkee _monkee;
        private Banana _banana;

        private CriAtomExPlayer _ballroll;
        private CriAtomExPlayer _spark;
        private CriAtomExPlayer _impactPlayer;
        private CriAtomExPlayer _monkeePlayer;
        private CriAtomExPlayer _bananaPlayer;
        
        private CriAtomExAcb _impactAcb;
        private CriAtomExAcb _monkeeAcb;
        private CriAtomExAcb _bananaAcb;

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
        private int _harvestedBananas;
        private float _groundTime;
        private float _intensity;

        

        private void Awake()
        {
            _player = FindObjectOfType<Player>();

            _speedToPitch = new SpeedToPitch();
            _impact = new Impact();
            _monkee = new Monkee();
            _banana = new Banana();

            _boundArray = new List<float>();
            _collideArray = new List<float>();
            _dropArray = new List<float>();
            _softArray = new List<float>();

            string[] monkeeArray = new string[] { "aiai", "baby", "doctor", "gongon", "jam", "jet", "meemee", "yanyan" };

            string monkee = _player.charaKind.ToString().ToLower();

            if (monkeeArray.Contains(monkee)) 
            {
                string monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\vo_" + monkee + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);
            }

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\ballroll_default.acb");
            string sparkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\spark_default.acb");
            string impactPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\bonks.acb");
            string bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\bananas.acb");

            // Create players for each sfx
            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            _impactPlayer = new CriAtomExPlayer();
            _monkeePlayer = new CriAtomExPlayer();
            _bananaPlayer = new CriAtomExPlayer();

            // Load sound banks
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

            _impactAcb = CriAtomExAcb.LoadAcbFile(null, impactPath, null);
            _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
        }


        private void Update()
        {
            int mainBananas = MainGame.mainGameStage.m_HarvestedBananaCount;

            if (_harvestedBananas != mainBananas)
            {
                int collected = mainBananas - _harvestedBananas;
                Console.WriteLine(collected);
                _harvestedBananas = mainBananas;
                _banana.calcCollected(_bananaPlayer, _bananaAcb, collected);
                _monkee.calcBanana(_monkeePlayer, _monkeeAcb, collected);
            }

            // Calculated speed based off square magnitude, multiplied and rounded to X.XX
            var playerSpeed = _player.m_PhysicsBall.m_Velocity.sqrMagnitude * 1000f;
            var rounded = Math.Round(playerSpeed, 2);

            //// My best guess on the linear equations they used in the GC games based on audio
            var ballrollPitch = (float)(10.14f * rounded) - 574.23f;
            var sparkPitch = (float)(11.673f * rounded) - 844.251f;
            var sparkVol = (float)(0.004f * rounded) - 0.011f;

            // Time on ground
            if (_player.IsOnGround())
            {
                _groundTime += Time.deltaTime;
            }
            else if (!_player.IsOnGround())
            {
                _groundTime = 0;
            }

            // Flags for disabling sound
            if (!_player.IsOnGround() || rounded <= 40f || GameManager.IsPause())
            {
                _spark.Stop();
            }

            if (!_player.IsOnGround() || rounded <= 3f || GameManager.IsPause())
            {
                _ballroll.Stop();
            }

            // Flags for enabling sound
            if (_player.IsOnGround() && rounded > 3f && _groundTime >= 0.3f)
            {

                if (_ballroll.GetStatus() != CriAtomExPlayer.Status.Stop)
                {
                    _speedToPitch.setPitch(ballrollPitch, _ballroll, _ballrollPlayback);
                }
                else
                {
                    _ballrollPlayback = _ballroll.Start();
                    _speedToPitch.setPitch(ballrollPitch, _ballroll, _ballrollPlayback);
                }

            }

            if (_player.IsOnGround() && rounded > 40f && _groundTime >= 0.3f)
            {
                if (_spark.GetStatus() != CriAtomExPlayer.Status.Stop)
                {
                    _speedToPitch.setSparkVol(sparkVol, _spark, _sparkPlayback);
                    _speedToPitch.setPitch(sparkPitch, _spark, _sparkPlayback);

                }
                else
                {
                    _sparkPlayback = _spark.Start();
                    _speedToPitch.setSparkVol(sparkVol, _spark, _sparkPlayback);
                    _speedToPitch.setPitch(sparkPitch, _spark, _sparkPlayback);
                }
            }

            float soft = _player.m_PhysicsBall.m_CollisionSphere.m_IntentPos.sqrMagnitude.CompareTo(_player.m_PhysicsBall.m_Pos.sqrMagnitude);

            if (soft != -1)
            {
                Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;
                float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                float drop = Math.Abs(_player.m_PhysicsBall.m_Pos.y - _player.m_PhysicsBall.m_OldPos.y);
                float intensity = Math.Abs(vectorDot);

                _collideArray.Insert(_collideInt++, soft);
                _dropArray.Insert(_dropInt++, drop);
                _softArray.Insert(_softInt++, intensity);
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
                _impact.calcImpact(_impactPlayer, _impactAcb, maxIntensity);
                _monkee.calcImpact(_monkeePlayer, _monkeeAcb, maxIntensity, _player);
                _boundArray.Clear();
            }
            else if (_collideArray.Any() && soft == -1 && _intensity > 1.5f && _player.m_PhysicsBall.m_CollisionSphere.isHit && _intensity < 6f)
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

                    _impact.softImpact(_impactPlayer, _impactAcb);
                }
                else if (maxDrop <= 0.02)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();
                    _collideInt = 0;
                    _dropInt = 0;
                    _softInt = 0;
                    _impact.softImpact(_impactPlayer, _impactAcb);
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
        }

        private void OnDisable()
        {
            _ballroll.Stop();
            _spark.Stop();
            _impactPlayer.Stop();
            _monkeePlayer.Stop();
            _bananaPlayer.Stop();
    }
    }
}

