using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MyFrameworkProject.Engine.Audio
{
    /// <summary>
    /// Central audio management system that handles sound effects and background music playback.
    /// Provides global volume control, sound pooling, and music management.
    /// Supports playing multiple sound effects simultaneously with individual control.
    /// </summary>
    public sealed class AudioManager : IDisposable
    {
        #region Fields - Sound Collections

        /// <summary>
        /// Dictionary of all loaded sounds, indexed by their unique identifier.
        /// </summary>
        private readonly Dictionary<string, Sound> _sounds;

        /// <summary>
        /// List of currently playing sound instances for tracking and cleanup.
        /// </summary>
        private readonly List<Sound> _activeSounds;

        #endregion

        #region Fields - Music Management

        /// <summary>
        /// The currently playing background music track.
        /// </summary>
        private Song _currentMusic;

        /// <summary>
        /// The name/identifier of the currently playing music track.
        /// </summary>
        private string _currentMusicName;

        #endregion

        #region Fields - Volume Control

        /// <summary>
        /// The master volume for all sound effects (0.0 to 1.0).
        /// </summary>
        private float _masterSoundVolume = 1f;

        /// <summary>
        /// The master volume for background music (0.0 to 1.0).
        /// </summary>
        private float _masterMusicVolume = 1f;

        /// <summary>
        /// Indicates whether all audio is muted.
        /// </summary>
        private bool _isMuted = false;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether the audio manager has been disposed.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Properties - Volume

        /// <summary>
        /// Gets or sets the master volume for all sound effects (0.0 to 1.0).
        /// </summary>
        public float MasterSoundVolume
        {
            get => _masterSoundVolume;
            set => _masterSoundVolume = Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets the master volume for background music (0.0 to 1.0).
        /// Updates the current playing music volume immediately.
        /// </summary>
        public float MasterMusicVolume
        {
            get => _masterMusicVolume;
            set
            {
                _masterMusicVolume = Math.Clamp(value, 0f, 1f);
                UpdateMusicVolume();
            }
        }

        /// <summary>
        /// Gets or sets whether all audio is muted.
        /// </summary>
        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                UpdateMusicVolume();
            }
        }

        #endregion

        #region Properties - Music State

        /// <summary>
        /// Gets whether music is currently playing.
        /// </summary>
        public bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        /// <summary>
        /// Gets whether music is currently paused.
        /// </summary>
        public bool IsMusicPaused => MediaPlayer.State == MediaState.Paused;

        /// <summary>
        /// Gets whether music is currently stopped.
        /// </summary>
        public bool IsMusicStopped => MediaPlayer.State == MediaState.Stopped;

        /// <summary>
        /// Gets the name of the currently playing or loaded music track.
        /// </summary>
        public string CurrentMusicName => _currentMusicName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioManager"/> class.
        /// Sets up the audio system with default volume settings.
        /// </summary>
        public AudioManager()
        {
            _sounds = [];
            _activeSounds = [];

            Core.Logger.Info("AudioManager initialized");
        }

        #endregion

        #region Public Methods - Sound Loading

        /// <summary>
        /// Loads a sound effect and stores it with the specified identifier.
        /// </summary>
        /// <param name="name">The unique identifier for the sound.</param>
        /// <param name="soundEffect">The MonoGame SoundEffect to load.</param>
        public void LoadSound(string name, SoundEffect soundEffect)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Sound name cannot be null or empty.", nameof(name));

            if (soundEffect == null)
                throw new ArgumentNullException(nameof(soundEffect));

            if (_sounds.ContainsKey(name))
            {
                Core.Logger.Warning($"Sound '{name}' already loaded. Replacing existing sound.");
                _sounds[name].Dispose();
            }

            _sounds[name] = new Sound(soundEffect);
            Core.Logger.Info($"Sound '{name}' loaded successfully");
        }

        /// <summary>
        /// Unloads a sound effect and releases its resources.
        /// </summary>
        /// <param name="name">The unique identifier of the sound to unload.</param>
        public void UnloadSound(string name)
        {
            if (_sounds.TryGetValue(name, out Sound sound))
            {
                sound.Dispose();
                _sounds.Remove(name);
                Core.Logger.Info($"Sound '{name}' unloaded");
            }
        }

        /// <summary>
        /// Checks if a sound with the specified name is loaded.
        /// </summary>
        /// <param name="name">The unique identifier of the sound.</param>
        /// <returns>True if the sound is loaded, false otherwise.</returns>
        public bool IsSoundLoaded(string name)
        {
            return _sounds.ContainsKey(name);
        }

        #endregion

        #region Public Methods - Sound Playback

        /// <summary>
        /// Plays a sound effect by its identifier.
        /// Creates a new instance for playback to allow multiple simultaneous plays.
        /// </summary>
        /// <param name="name">The unique identifier of the sound to play.</param>
        /// <param name="volume">The volume override (0.0 to 1.0), or -1 to use default.</param>
        /// <param name="pitch">The pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">The pan (-1.0 to 1.0).</param>
        public void PlaySound(string name, float volume = -1f, float pitch = 0f, float pan = 0f)
        {
            if (_isMuted || _disposed)
                return;

            if (!_sounds.TryGetValue(name, out Sound sound))
            {
                Core.Logger.Warning($"Cannot play sound '{name}': Sound not loaded");
                return;
            }

            float finalVolume = volume >= 0f ? volume * _masterSoundVolume : _masterSoundVolume;
            sound.Play(finalVolume, pitch, pan);
        }

        /// <summary>
        /// Plays a looping sound effect that can be controlled later.
        /// Returns the Sound instance for advanced control.
        /// </summary>
        /// <param name="name">The unique identifier of the sound to play.</param>
        /// <param name="volume">The volume (0.0 to 1.0).</param>
        /// <returns>The Sound instance being played, or null if not found.</returns>
        public Sound PlaySoundLooped(string name, float volume = 1f)
        {
            if (_isMuted || _disposed)
                return null;

            if (!_sounds.TryGetValue(name, out Sound sound))
            {
                Core.Logger.Warning($"Cannot play looped sound '{name}': Sound not loaded");
                return null;
            }

            // Create a new instance for independent control
            var instance = new Sound(sound.NativeSoundEffect)
            {
                IsLooped = true,
                Volume = volume * _masterSoundVolume
            };

            instance.Play();
            _activeSounds.Add(instance);

            return instance;
        }

        /// <summary>
        /// Gets a loaded sound instance for manual control.
        /// </summary>
        /// <param name="name">The unique identifier of the sound.</param>
        /// <returns>The Sound instance, or null if not found.</returns>
        public Sound GetSound(string name)
        {
            return _sounds.TryGetValue(name, out Sound sound) ? sound : null;
        }

        /// <summary>
        /// Stops all currently active sounds.
        /// </summary>
        public void StopAllSounds()
        {
            foreach (var sound in _activeSounds)
            {
                sound.Stop(true);
                sound.Dispose();
            }

            _activeSounds.Clear();
            Core.Logger.Info("All sounds stopped");
        }

        #endregion

        #region Public Methods - Music Playback

        /// <summary>
        /// Plays background music from the specified Song.
        /// Stops any currently playing music before starting the new track.
        /// </summary>
        /// <param name="name">The unique identifier for the music track.</param>
        /// <param name="song">The MonoGame Song to play.</param>
        /// <param name="isRepeating">Whether the music should loop.</param>
        public void PlayMusic(string name, Song song, bool isRepeating = true)
        {
            ArgumentNullException.ThrowIfNull(song);

            if (_disposed)
                return;

            StopMusic();

            _currentMusic = song;
            _currentMusicName = name;
            MediaPlayer.IsRepeating = isRepeating;
            UpdateMusicVolume();
            MediaPlayer.Play(song);

            Core.Logger.Info($"Music '{name}' started playing");
        }

        /// <summary>
        /// Pauses the currently playing music.
        /// </summary>
        public void PauseMusic()
        {
            if (IsMusicPlaying)
            {
                MediaPlayer.Pause();
                Core.Logger.Info("Music paused");
            }
        }

        /// <summary>
        /// Resumes the paused music.
        /// </summary>
        public void ResumeMusic()
        {
            if (IsMusicPaused)
            {
                MediaPlayer.Resume();
                Core.Logger.Info("Music resumed");
            }
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (!IsMusicStopped)
            {
                MediaPlayer.Stop();
                _currentMusic = null;
                _currentMusicName = null;
                Core.Logger.Info("Music stopped");
            }
        }

        #endregion

        #region Public Methods - Update

        /// <summary>
        /// Updates the audio manager.
        /// Cleans up finished sound instances to prevent memory leaks.
        /// Should be called once per frame.
        /// </summary>
        public void Update()
        {
            if (_disposed)
                return;

            // Clean up finished sounds
            for (int i = _activeSounds.Count - 1; i >= 0; i--)
            {
                if (_activeSounds[i].IsStopped)
                {
                    _activeSounds[i].Dispose();
                    _activeSounds.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Private Methods - Helpers

        /// <summary>
        /// Updates the music volume based on master volume and mute state.
        /// </summary>
        private void UpdateMusicVolume()
        {
            MediaPlayer.Volume = _isMuted ? 0f : _masterMusicVolume;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the audio manager.
        /// Stops all sounds and music, and disposes all loaded sound instances.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            StopMusic();
            StopAllSounds();

            foreach (var sound in _sounds.Values)
            {
                sound.Dispose();
            }

            _sounds.Clear();
            _disposed = true;

            Core.Logger.Info("AudioManager disposed");
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer to ensure resources are released if Dispose is not called.
        /// </summary>
        ~AudioManager()
        {
            Dispose();
        }

        #endregion
    }
}
