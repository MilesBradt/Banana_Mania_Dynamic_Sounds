using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Flash2;
using Framework.UI;
using UnityEngine;
using static Flash2.Chara;

namespace DynamicSounds
{
    class SpeedToPitch
    {
        public void ChangePitch(float pitch, CriAtomExPlayer audio, CriAtomExPlayback playback)
        {
            if (pitch >= 1100f)
            {
                audio.SetPitch(1100f);
                audio.Update(playback);
            }
            else
            {
                audio.SetPitch(pitch);
                audio.Update(playback);
            }
        }

        public void SetSparkVol(float vol, CriAtomExPlayer audio, CriAtomExPlayback playback)
        {
            if (vol >= 0.6f)
            {
                audio.SetVolume(0.6f);
                audio.Update(playback);
            }
            else
            {
                audio.SetVolume(vol);
                audio.Update(playback);
            }
        }
    }

    class ImpactAudio
    {
        public void PlaySfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);

            if (sfx != "timer")
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.2f);
                audio.Update(playback);
            }
            else
            {
                audio.Start();
            }

        }
    }

    class Impact : ImpactAudio
    {
        public void CalcImpact(CriAtomExPlayer audio, CriAtomExAcb acb, float impact)
        {
            if (impact <= 6.5)
            {
                PlaySfx(audio, acb, "jump");
            }
            else if (impact > 6.5 && impact <= 9.5)
            {

                PlaySfx(audio, acb, "1up");
            }
            else
            {
                PlaySfx(audio, acb, "timer");
            }
        }

        public void SoftImpact(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlaySfx(audio, acb, "ballbound");
        }
    }

    class MonkeeAudio
    {
        public void PlaySfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx, float timer, bool isGoal)
        {
            audio.SetCue(acb, sfx);

            if (audio.GetStatus() == CriAtomExPlayer.Status.Playing && timer >= 0.2f && !isGoal)
            {
                audio.Stop();
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
            else if (audio.GetStatus() == CriAtomExPlayer.Status.Playing && timer <= 0.2f || isGoal)
            {
                // do nothing
            }
            else
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }

        public void PlayOtto(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void PlayFallout(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if (audio.GetStatus() != CriAtomExPlayer.Status.Playing)
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }

        public void PlayGoal(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void PlayGoalFly(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            audio.Start();
        }

        public void PlayStartSfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {
            audio.SetCue(acb, sfx);
            if (audio.GetStatus() != CriAtomExPlayer.Status.Playing)
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(1.0f);
                audio.Update(playback);
            }
        }
    }

    class Monkee : MonkeeAudio
    {
        public void CalcImpact(CriAtomExPlayer audio, CriAtomExAcb acb, float impact, Player player, float timer, bool isGoal)
        {
            if (impact <= 6.5)
            {
                PlaySfx(audio, acb, "goal_fly", timer, isGoal); ;
            }
            else if (impact > 6.5 && impact <= 9.5)
            {

                PlaySfx(audio, acb, "hanauta", timer, isGoal);
            }
            else
            {
                if (player.charaKind.ToString().ToLower() == "yanyan")
                {
                    PlaySfx(audio, acb, "yabai_long", timer, isGoal);
                }
                else
                {
                    PlaySfx(audio, acb, "thankyou", timer, isGoal);
                }
            }
        }

        public void CalcBanana(CriAtomExPlayer audio, CriAtomExAcb acb, int banana, float timer, bool isGoal)
        {
            if (banana == 1)
            {
                PlaySfx(audio, acb, "fallout", timer, isGoal);
            }
            else if (banana == 10)
            {
                PlaySfx(audio, acb, "timeover", timer, isGoal);
            }
        }

        public void StartMap(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlayStartSfx(audio, acb, "start");
        }

        public void Unbalance(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlayOtto(audio, acb, "happy_long");
        }

        public void Scream(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlayFallout(audio, acb, "happy");
        }

        public void Goal(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlayGoal(audio, acb, "continue_unselect");
        }

        public void GoalFly(CriAtomExPlayer audio, CriAtomExAcb acb)
        {
            PlayGoalFly(audio, acb, "yabai_long");
        }
    }

    class BananaAudio
    {
        public void PlaySfx(CriAtomExPlayer audio, CriAtomExAcb acb, string sfx)
        {

            audio.SetCue(acb, sfx);

            if (sfx == "timer")
            {
                CriAtomExPlayback playback = audio.Start();
                audio.SetVolume(0.9f);
                audio.Update(playback);
            }
            else
            {
                audio.Start();
            }
        }
    }

    class Banana : BananaAudio
    {
        public void CalcCollected(CriAtomExPlayer audio, CriAtomExAcb acb, int banana)
        {
            if (banana == 1)
            {
                PlaySfx(audio, acb, "bananaget");
            }
            else if (banana == 10)
            {
                PlaySfx(audio, acb, "timer");
            }
        }
    }

    internal class SoundController : MonoBehaviour
    {
        public SoundController(IntPtr value) : base(value) { }

        private Player _player;
        private PlayerMotion _motion;
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

        private string[] _monkeeArray;
        private string[] _guestArray;
        private string[] _consoleArray;
        private string[] _dlcArray;
        private string _monkeeType;

        private int _collideInt = 0;
        private int _dropInt = 0;
        private int _softInt = 0;
        private int _harvestedBananas;
        private int _timer;

        private float _groundTime;
        private float _bufferTime;
        private float _intensity;

        private bool _isStart;
        private bool _isOffBalance;
        private bool _isFallout;
        private bool _isGoal;
        private bool _isGoalFly;
        private bool _isBumped;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();

            _motion = new PlayerMotion();
            _speedToPitch = new SpeedToPitch();
            _impact = new Impact();
            _monkee = new Monkee();
            _banana = new Banana();

            _boundArray = new List<float>();
            _collideArray = new List<float>();
            _dropArray = new List<float>();
            _softArray = new List<float>();

            _monkeeArray = new string[] { "aiai", "baby", "doctor", "gongon", "jam", "jet", "meemee", "yanyan" };
            _guestArray = new string[] { "beat", "kiryu", "sonic", "tails" };
            _consoleArray = new string[] { "dreamcast", "gamegear", "segasaturn" };
            _dlcArray = new string[] { "suezo", "hellokitty", "morgana" };

            _monkeeType = _player.charaKind.ToString().ToLower();

            if (_monkeeArray.Contains(_monkeeType))
            {
                string monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Monkeys\vo_" + _monkeeType + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                string bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas.acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }
            else if (_consoleArray.Contains(_monkeeType))
            {
                string monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Consoles\vo_" + _monkeeType + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                string bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas.acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }
            else if (_guestArray.Contains(_monkeeType))
            {
                string monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Guests\vo_" + _monkeeType + ".acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                string bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas_" + _monkeeType + ".acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }
            else if (_dlcArray.Contains(_monkeeType))
            {
                string monkeePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\DLC\vo_muted.acb");
                _monkeeAcb = CriAtomExAcb.LoadAcbFile(null, monkeePath, null);

                string bananaPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\Bananas\bananas_muted.acb");
                _bananaAcb = CriAtomExAcb.LoadAcbFile(null, bananaPath, null);
            }

            // Reads acb file from current directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\ballroll_default.acb");
            string sparkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\spark_default.acb");

            if (_monkeeType == "sonic" || _monkeeType == "tails")
            {
                string impactPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\impacts_sonic.acb");
                _impactAcb = CriAtomExAcb.LoadAcbFile(null, impactPath, null);
            }
            else
            {
                string impactPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Sounds\impacts.acb");
                _impactAcb = CriAtomExAcb.LoadAcbFile(null, impactPath, null);
            }

            // Create players for each sfx
            _ballroll = new CriAtomExPlayer();
            _spark = new CriAtomExPlayer();
            _impactPlayer = new CriAtomExPlayer();
            _bananaPlayer = new CriAtomExPlayer();
            _monkeePlayer = new CriAtomExPlayer();

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

            _timer = MainGame.mainGameStage.m_GameTimer;

            _isStart = false;
            _isFallout = false;
            _isGoal = false;
            _isGoalFly = false;
            _isBumped = false;
        }

        private void Update()
        {
            int mainBananas = MainGame.mainGameStage.m_HarvestedBananaCount;
            var playerState = _player.m_PlayerMotion.m_State;
            var playerBalance = _player.m_PlayerMotion.m_UnbalanceState;
            var bumperAmount = _player.m_MainGameStage.m_Bumpers._size;
            var bumper = _player.m_MainGameStage.getNearestBumper(_player.m_PhysicsBall.m_Pos);

            _bufferTime += Time.deltaTime;

            if (_consoleArray.Contains(_monkeeType) || _guestArray.Contains(_monkeeType) || _monkeeType == "jam" || _monkeeType == "jet")
            {

                if (_player.m_MainGameStage.m_ReadyGoSequence.isFinished && !_isStart)
                {
                    _monkee.StartMap(_monkeePlayer, _monkeeAcb);
                    _isStart = true;
                }

                if (playerBalance != PlayerMotion.UnbalanceState.NONE && !_isOffBalance)
                {
                    _monkee.Unbalance(_monkeePlayer, _monkeeAcb);
                    _isOffBalance = true;
                }
                else if (playerBalance == PlayerMotion.UnbalanceState.NONE && _isOffBalance)
                {
                    _isOffBalance = false;
                }

                if (_player.IsFallOut() && !_isFallout)
                {
                    _monkee.Scream(_monkeePlayer, _monkeeAcb);
                    _isFallout = true;
                }

                if (playerState == PlayerMotion.State.GOAL && !_isGoal)
                {
                    _monkee.Goal(_monkeePlayer, _monkeeAcb);
                    _isGoal = true;
                }

                if (playerState == PlayerMotion.State.GOAL_FLY && !_isGoalFly)
                {
                    _monkee.GoalFly(_monkeePlayer, _monkeeAcb);
                    _isGoalFly = true;
                }
            }

            if (_harvestedBananas != mainBananas)
            {
                int collected = mainBananas - _harvestedBananas;
                _harvestedBananas = mainBananas;
                _banana.CalcCollected(_bananaPlayer, _bananaAcb, collected);
                _monkee.CalcBanana(_monkeePlayer, _monkeeAcb, collected, _bufferTime, _isGoal);
                _bufferTime = 0;
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
                    _speedToPitch.ChangePitch(ballrollPitch, _ballroll, _ballrollPlayback);
                }
                else
                {
                    _ballrollPlayback = _ballroll.Start();
                    _speedToPitch.ChangePitch(ballrollPitch, _ballroll, _ballrollPlayback);
                }

            }

            if (_player.IsOnGround() && rounded > 40f && _groundTime >= 0.3f)
            {
                if (_spark.GetStatus() != CriAtomExPlayer.Status.Stop)
                {
                    _speedToPitch.SetSparkVol(sparkVol, _spark, _sparkPlayback);
                    _speedToPitch.ChangePitch(sparkPitch, _spark, _sparkPlayback);

                }
                else
                {
                    _sparkPlayback = _spark.Start();
                    _speedToPitch.SetSparkVol(sparkVol, _spark, _sparkPlayback);
                    _speedToPitch.ChangePitch(sparkPitch, _spark, _sparkPlayback);
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

            if (_player.m_BoundTimer > 0 && bumperAmount != 0)
            {
                _collideArray.Clear();
                _dropArray.Clear();
                _softArray.Clear();
                _collideInt = 0;
                _dropInt = 0;
                _softInt = 0;

                if (bumper.m_state == Bumper.State.HIT)
                {
                    _isBumped = true;
                    Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                    int i = 0;
                    float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                    _intensity = Math.Abs(vectorDot);

                    _boundArray.Insert(i++, _intensity);
                } 
                else
                {
                    Vector3 normal = _player.m_PhysicsBall.m_Pos.normalized;

                    int i = 0;
                    float vectorDot = Vector3.Dot(normal, _player.m_relativeVelo);
                    _intensity = Math.Abs(vectorDot);

                    _boundArray.Insert(i++, _intensity);
                }

            }
            else if (_player.m_BoundTimer > 0 && bumperAmount == 0)
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
            
            if (_player.m_BoundTimer <= 0 && _boundArray.Any() && !_isBumped)
            {
                int timePast = _timer - MainGame.mainGameStage.m_GameTimer;
                if (timePast <= 10)
                {
                    _impact.CalcImpact(_impactPlayer, _impactAcb, 10f);
                    _boundArray.Clear();
                    _bufferTime = 0;
                }
                else
                {
                    float maxIntensity = _boundArray.Max();
                    _impact.CalcImpact(_impactPlayer, _impactAcb, maxIntensity);
                    _monkee.CalcImpact(_monkeePlayer, _monkeeAcb, maxIntensity, _player, _bufferTime, _isGoal);
                    _boundArray.Clear();
                    _bufferTime = 0;
                }
            }
            else if (_player.m_BoundTimer <= 0 && _boundArray.Any() && _isBumped)
            {
                float maxIntensity = _boundArray.Max();
                _monkee.CalcImpact(_monkeePlayer, _monkeeAcb, maxIntensity, _player, _bufferTime, _isGoal);
                _boundArray.Clear();
                _bufferTime = 0;
                _isBumped = false;
            }
            else if (_collideArray.Any() && soft == -1 && _player.m_PhysicsBall.m_CollisionSphere.isHit && _intensity < 6f)
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

                    _impact.SoftImpact(_impactPlayer, _impactAcb);
                }
                else if (maxDrop <= 0.02)
                {
                    _collideArray.Clear();
                    _dropArray.Clear();
                    _softArray.Clear();
                    _collideInt = 0;
                    _dropInt = 0;
                    _softInt = 0;
                    _impact.SoftImpact(_impactPlayer, _impactAcb);
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
            _ballroll.Dispose();

            _spark.Stop();
            _spark.Dispose();

            _impactPlayer.Stop();
            _impactPlayer.Dispose();

            _monkeePlayer.Stop();
            _monkeePlayer.Dispose();

            _bananaPlayer.Stop();
            _bananaPlayer.Dispose();
        }
    }
}

