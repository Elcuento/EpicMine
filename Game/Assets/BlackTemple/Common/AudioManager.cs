using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackTemple.Common
{
    public class AudioManager : Singleton<AudioManager>
    {
        private const string MusicMutedKey = "MUSIC_MUTED";

        private const string MusicVolumeKey = "MUSIC_VOLUME";

        private const string SoundsMutedKey = "SOUNDS_MUTED";

        private const string SoundsVolumeKey = "SOUNDS_VOLUME";

        private readonly List<Audio> _musicTracks = new List<Audio>();

        private readonly List<Audio> _sounds = new List<Audio>();

        private readonly Dictionary<AudioClip, List<Audio>> _soundsPools = new Dictionary<AudioClip, List<Audio>>();

        private readonly Dictionary<Audio, Coroutine> _fadingCoroutines = new Dictionary<Audio, Coroutine>();

        private readonly List<Audio> _soundsToDestroy = new List<Audio>();

        private short _soundsCounter = 1;

        private float _previousTimescale;


        public bool IsMusicMuted { get; private set; }

        public bool IsSoundsMuted { get; private set; }

        public float MusicVolume { get; private set; }

        public float SoundsVolume { get; private set; }


        public void SetMusicMuted(bool muted)
        {
            IsMusicMuted = muted;

            foreach (var musicTrack in _musicTracks)
                musicTrack.AudioSource.mute = IsMusicMuted;

            PlayerPrefs.SetInt(MusicMutedKey, IsMusicMuted ? 1 : 0);
        }

        public void SetSoundsMuted(bool muted)
        {
            IsSoundsMuted = muted;

            foreach (var sound in _sounds)
                sound.AudioSource.mute = IsSoundsMuted;

            foreach (var pool in _soundsPools)
            {
                foreach (var sound in pool.Value)
                    sound.AudioSource.mute = IsSoundsMuted;
            }

            PlayerPrefs.SetInt(SoundsMutedKey, muted ? 1 : 0);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;

            foreach (var musicTrack in _musicTracks)
                musicTrack.AudioSource.volume = MusicVolume * musicTrack.OriginalVolume;

            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        }

        public void SetSoundsVolume(float volume)
        {
            SoundsVolume = volume;

            foreach (var sound in _sounds)
                sound.AudioSource.volume = SoundsVolume * sound.OriginalVolume;

            foreach (var pool in _soundsPools)
            {
                foreach (var sound in pool.Value)
                    sound.AudioSource.volume = SoundsVolume * sound.OriginalVolume;
            }

            PlayerPrefs.SetFloat(SoundsVolumeKey, volume);
        }

        /// <summary>
        ///     Play music on a specific track
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="track"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="fadeTime"></param>
        public void PlayMusic(AudioClip audioClip, int track = 0, bool loop = false, bool autoPausable = false, float volume = 1f, float fadeTime = 0f)
        {
            var music = _musicTracks.FirstOrDefault(t => t.Id == track);
            if (music != null)
            {
                music.AudioSource.Stop();
                music.AudioSource.clip = audioClip;
            }
            else
            {
                music = CreateNewAudio(track, audioClip);
                _musicTracks.Add(music);
            }

            music.AudioSource.gameObject.name = $"Music track #{music.Id}: {audioClip.name}";
            music.AudioSource.mute = IsMusicMuted;
            music.AudioSource.loop = loop;
            music.AudioSource.Play();
            music.SetOriginalVolume(volume).SetAutoPausable(autoPausable);

            if (fadeTime > 0f)
            {
                music.AudioSource.volume = 0f;
                FadeAudio(music, MusicVolume * volume, fadeTime);
            }
            else
            {
                music.AudioSource.volume = MusicVolume * volume;
            }
        }

        /// <summary>
        ///     Play music or continue, if it is already playing
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="track"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="fadeTime"></param>
        public void PlayOrContinueMusic(AudioClip audioClip, int track = 0, bool loop = false, bool autoPausable = false, float volume = 1f, float fadeTime = 0f)
        {
            var music = _musicTracks.FirstOrDefault(t => t.Id == track);
            if (music != null && music.AudioSource.clip == audioClip)
            {
                music.AudioSource.loop = loop;
                music.SetOriginalVolume(volume).SetAutoPausable(autoPausable);
                PauseMusic(false, track, fadeTime);
            }
            else
                PlayMusic(audioClip, track, loop, autoPausable, volume, fadeTime);
        }

        /// <summary>
        ///     Un- / Pause music on a specific track
        /// </summary>
        /// <param name="pause"></param>
        /// <param name="track"></param>
        /// <param name="fadeTime"></param>
        public void PauseMusic(bool pause, int track = 0, float fadeTime = 0f)
        {
            var music = _musicTracks.FirstOrDefault(t => t.Id == track);
            if (music == null)
                return;

            if (fadeTime > 0f)
            {
                if (pause)
                {
                    FadeAudio(music, 0f, fadeTime, () => { music.SetPause(true); });
                }
                else
                {
                    music.SetPause(false);
                    FadeAudio(music, MusicVolume * music.OriginalVolume, fadeTime);
                }
            }
            else
            {
                music.SetPause(pause);
            }
        }

        /// <summary>
        ///     Stop playing music on a specific track
        /// </summary>
        /// <param name="track"></param>
        /// <param name="fadeTime"></param>
        public void StopMusic(int track = 0, float fadeTime = 0f)
        {
            var music = _musicTracks.FirstOrDefault(t => t.Id == track);
            if (music == null)
                return;

            var clearMusic = new Action(() =>
            {
                music.Clear();
                music.AudioSource.gameObject.name = $"Music track #{track}";
            });

            if (fadeTime > 0f)
            {
                FadeAudio(music, 0f, fadeTime, clearMusic);
                return;
            }

            clearMusic();
        }

        /// <summary>
        ///     Stop playing music on all tracks
        /// </summary>
        /// <param name="fadeTime"></param>
        public void StopAllMusic(float fadeTime = 0f)
        {
            foreach (var music in _musicTracks)
                StopMusic(music.Id, fadeTime);
        }


        /// <summary>
        ///     Create pool of identical sounds
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="poolSize"></param>
        public void CreateSoundPool(AudioClip audioClip, int poolSize = 1)
        {
            List<Audio> pool;

            if (_soundsPools.TryGetValue(audioClip, out pool))
                return;

            pool = new List<Audio>();

            for (var i = 0; i < poolSize; i++)
            {
                var audioSource = CreateNewAudioSource(audioClip);
                var sound = new Audio(audioSource);
                sound.AudioSource.gameObject.SetActive(false);
                pool.Add(sound);
            }

            _soundsPools.Add(audioClip, pool);
        }

        /// <summary>
        ///     Destroy identical sounds pool
        /// </summary>
        /// <param name="audioClip"></param>
        public void DestroySoundPool(AudioClip audioClip)
        {
            List<Audio> pool;
            if (_soundsPools.TryGetValue(audioClip, out pool))
            {
                foreach (var sound in pool)
                    Destroy(sound.AudioSource.gameObject);

                _soundsPools.Remove(audioClip);
            }
        }

        /// <summary>
        ///     Destroy all identical sound pools
        /// </summary>
        public void DestroyAllSoundPools()
        {
            foreach (var pool in _soundsPools)
            {
                foreach (var sound in pool.Value)
                    Destroy(sound.AudioSource.gameObject);
            }

            _soundsPools.Clear();
        }

        /// <summary>
        ///     Play sound. Sound will be loaded from pool - if it is exists
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="fadeTime"></param>
        /// <returns></returns>
        public int PlaySound(AudioClip audioClip, bool loop = false, bool autoPausable = false, float volume = 1f, float fadeTime = 0f)
        {
            
            _soundsCounter++;
            var soundId = _soundsCounter;
            Audio sound = null;

            if (_soundsPools.TryGetValue(audioClip, out var pool))
            {
                foreach (var pooledSound in pool)
                {
                    if (pooledSound.AudioSource.gameObject.activeSelf)
                        continue;

                    pooledSound.AudioSource.gameObject.SetActive(true);
                    sound = pooledSound;
                    break;
                }

                if (sound != null)
                    pool.Remove(sound);
            }

            if (sound == null)
                sound = CreateNewAudio(soundId, audioClip);
            else
                sound.SetId(soundId);

            sound.AudioSource.gameObject.name = $"Sound: {audioClip.name}";
            sound.AudioSource.mute = IsSoundsMuted;
            sound.AudioSource.ignoreListenerPause = !autoPausable;
            sound.AudioSource.loop = loop;
            sound.AudioSource.Play();
            sound.SetOriginalVolume(volume).SetAutoPausable(autoPausable);
            _sounds.Add(sound);

            if (fadeTime > 0f)
            {
                if (!loop && fadeTime > audioClip.length)
                    fadeTime = audioClip.length;

                sound.AudioSource.volume = 0f;
                FadeAudio(sound, SoundsVolume * volume, fadeTime);
            }
            else
            {
                sound.AudioSource.volume = SoundsVolume * volume;
            }

            return soundId;
        }


        /// <summary>
        ///     Pause a specific sound
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pause"></param>
        /// <param name="fadeTime"></param>
        public void PauseSound(int id, bool pause, float fadeTime = 0f)
        {
            var sound = _sounds.FirstOrDefault(s => s.Id == id);
            if (sound == null)
                return;

            if (fadeTime > 0f)
            {
                var timeLeft = sound.AudioSource.clip.length - sound.AudioSource.time;
                if (!sound.AudioSource.loop && fadeTime > timeLeft)
                    fadeTime = timeLeft;

                if (pause)
                {
                    FadeAudio(sound, 0f, fadeTime, () => { sound.SetPause(true); });
                }
                else
                {
                    sound.SetPause(false);
                    FadeAudio(sound, MusicVolume * sound.OriginalVolume, fadeTime);
                }
            }
            else
            {
                sound.SetPause(pause);
            }
        }

        /// <summary>
        ///     Change specific sound volume
        /// </summary>
        /// <param name="id"></param>
        /// <param name="volume"></param>
        public void SetSoundVolume(int id, float volume)
        {
            var sound = _sounds.FirstOrDefault(s => s.Id == id);
            if (sound == null)
                return;

            sound.SetOriginalVolume(volume);
            sound.AudioSource.volume = SoundsVolume * sound.OriginalVolume;
        }
        /// <summary>
        ///     Check specific sound
        /// </summary>
        /// <param name="id"></param>

        public bool IsPlaying(int id)
        {
            var sound = _sounds.FirstOrDefault(s => s.Id == id);
            return sound != null && sound.AudioSource.isPlaying;
        }

        ///
        /// 
        /// <summary>
        ///     Stop specific sound
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fadeTime"></param>
        public void StopSound(int id, float fadeTime = 0f)
        {
            var sound = _sounds.FirstOrDefault(s => s.Id == id);
            if (sound == null)
                return;

            StopSound(sound, fadeTime);
        }

        /// <summary>
        ///     Stop all identical sounds
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="fadeTime"></param>
        public void StopSound(AudioClip audioClip, float fadeTime = 0f)
        {
            var sounds = _sounds.Where(s => s.AudioSource.clip == audioClip).ToList();

            foreach (var sound in sounds)
                StopSound(sound, fadeTime);
        }

        /// <summary>
        ///     Stop all sounds
        /// </summary>
        /// <param name="fadeTime"></param>
        public void StopAllSounds(float fadeTime = 0f)
        {
            foreach (var sound in _sounds)
                StopSound(sound.Id, fadeTime);
        }


        protected override void Awake()
        {
            base.Awake();

            IsMusicMuted = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
            IsSoundsMuted = PlayerPrefs.GetInt(SoundsMutedKey, 0) == 1;
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1);
            SoundsVolume = PlayerPrefs.GetFloat(SoundsVolumeKey, 1);
        }

        private void Update()
        {
            if (!_previousTimescale.Equals(Time.timeScale))
            {
                foreach (var musicTrack in _musicTracks)
                {
                    if (musicTrack.IsAutoPausable)
                        musicTrack.SetAutoPause(Time.timeScale <= 0f);
                }

                foreach (var sound in _sounds)
                {
                    if (sound.IsAutoPausable)
                        sound.SetAutoPause(Time.timeScale <= 0f);
                }

                _previousTimescale = Time.timeScale;
            }   
            

            foreach (var sound in _sounds)
            {
                if (sound.AudioSource.loop)
                    continue;

                if (sound.AudioSource.isPlaying)
                    continue;

                if (sound.IsPaused || sound.IsAutoPaused)
                    continue;

                _soundsToDestroy.Add(sound);
            }

            foreach (var sound in _soundsToDestroy)
                StopSound(sound.Id);

            _soundsToDestroy.Clear();
        }

        private void FadeAudio(Audio audioToFade, float volume, float time, Action onComplete = null)
        {
            Coroutine fadingCoroutine;
            if (_fadingCoroutines.TryGetValue(audioToFade, out fadingCoroutine))
            {
                if (fadingCoroutine != null)
                {
                    StopCoroutine(fadingCoroutine);
                    _fadingCoroutines.Remove(audioToFade);
                }
            }

            fadingCoroutine = StartCoroutine(
                FadeAudioCoroutine(
                    audioToFade,
                    volume,
                    time,
                    () =>
                    {
                        _fadingCoroutines.Remove(audioToFade);
                        onComplete?.Invoke();
                    }));

            _fadingCoroutines.Add(audioToFade, fadingCoroutine);
        }

        private IEnumerator FadeAudioCoroutine(Audio sound, float volume, float fadeTime, Action onComplete = null)
        {
            var startVolume = sound.AudioSource.volume;
            var startTime = Time.time;
            var timer = fadeTime;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                var newVolume = Mathf.Lerp(startVolume, volume, (Time.time - startTime) / fadeTime);
                sound.AudioSource.volume = newVolume;

                yield return null;
            }

            sound.AudioSource.volume = volume;
            onComplete?.Invoke();
        }

        private AudioSource CreateNewAudioSource(AudioClip audioClip)
        {
            var go = new GameObject($"Audio: { audioClip.name }");
            go.transform.SetParent(transform);
            var audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            return audioSource;
        }

        private Audio CreateNewAudio(int id, AudioClip audioClip)
        {
            var audioSource = CreateNewAudioSource(audioClip);
            var newAudio = new Audio(id, audioSource);
            return newAudio;
        }

        private void StopSound(Audio sound, float fadeTime = 0f)
        {
            if (fadeTime > 0f)
            {
                var timeLeft = sound.AudioSource.clip.length - sound.AudioSource.time;
                if (!sound.AudioSource.loop && fadeTime > timeLeft)
                    fadeTime = timeLeft;

                FadeAudio(sound, 0f, fadeTime, () => { DestroySound(sound); });
            }
            else
            {
                DestroySound(sound);
            }
        }

        private void DestroySound(Audio sound)
        {
            Coroutine fadingCoroutine;
            if (_fadingCoroutines.TryGetValue(sound, out fadingCoroutine))
            {
                StopCoroutine(fadingCoroutine);
                _fadingCoroutines.Remove(sound);
            }

            List<Audio> pool;
            if (_soundsPools.TryGetValue(sound.AudioSource.clip, out pool))
            {
                sound.Reset();
                sound.AudioSource.gameObject.SetActive(false);
                pool.Add(sound);
            }
            else
            {
                Destroy(sound.AudioSource.gameObject);
            }

            _sounds.Remove(sound);

            if (_sounds.Count <= 0)
                _soundsCounter = 0;
        }


        private class Audio
        {
            public AudioSource AudioSource { get; }

            public int Id { get; private set; }

            public bool IsPaused { get; private set; }

            public bool IsAutoPaused { get; private set; }

            public bool IsAutoPausable { get; private set; }

            public float OriginalVolume { get; private set; }

            public Audio(AudioSource audioSource)
            {
                AudioSource = audioSource;
            }

            public Audio(int id, AudioSource audioSource)
            {
                Id = id;
                AudioSource = audioSource;
            }

            public Audio SetOriginalVolume(float volume)
            {
                OriginalVolume = volume;
                return this;
            }

            public Audio SetAutoPausable(bool pausable)
            {
                IsAutoPausable = pausable;
                return this;
            }

            public void SetAutoPause(bool pause)
            {
                IsAutoPaused = pause;

                if (!pause && !IsPaused)
                    AudioSource.UnPause();
                else
                    AudioSource.Pause(); 
            }

            public void SetPause(bool pause)
            {
                IsPaused = pause;

                if (!pause && !IsAutoPaused)
                    AudioSource.UnPause();
                else
                    AudioSource.Pause();
            }

            public void SetId(int id)
            {
                Id = id;
            }

            public void Reset()
            {
                AudioSource.loop = false;
                AudioSource.volume = 1f;
                OriginalVolume = 1f;
            }

            public void Clear()
            {
                AudioSource.Stop();
                AudioSource.clip = null;
                Reset();
            }
        }
    }
}