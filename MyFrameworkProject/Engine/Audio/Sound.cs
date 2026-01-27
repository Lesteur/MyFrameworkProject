using System;

using Microsoft.Xna.Framework.Audio;

namespace MyFrameworkProject.Engine.Audio
{
    /// <summary>
    /// Represents a sound effect that can be played, paused, and stopped.
    /// Wraps MonoGame's SoundEffect and SoundEffectInstance for advanced playback control.
    /// Supports volume, pitch, and pan adjustments, as well as looping capabilities.
    /// </summary>
    public sealed class Sound : IDisposable
    {
        #region Fields - Audio

        /// <summary>
        /// The underlying MonoGame sound effect asset.
        /// </summary>
        private readonly SoundEffect _soundEffect;

        /// <summary>
        /// The sound effect instance used for playback control.
        /// Allows for volume, pitch, pan, and playback state manipulation.
        /// </summary>
        private SoundEffectInstance _instance;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether the sound instance has been disposed.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Properties - Audio Settings

        /// <summary>
        /// Gets the volume of the sound (0.0 to 1.0).
        /// </summary>
        public float Volume => _instance?.Volume ?? 0f;

        /// <summary>
        /// Gets the pitch adjustment of the sound (-1.0 to 1.0).
        /// 0.0 is normal pitch, -1.0 is one octave down, 1.0 is one octave up.
        /// </summary>
        public float Pitch => _instance?.Pitch ?? 0f;

        /// <summary>
        /// Gets the pan of the sound (-1.0 to 1.0).
        /// -1.0 is full left, 0.0 is center, 1.0 is full right.
        /// </summary>
        public float Pan => _instance?.Pan ?? 0f;

        /// <summary>
        /// Gets whether the sound should loop when it reaches the end.
        /// </summary>
        public bool IsLooped => _instance?.IsLooped ?? false;

        #endregion

        #region Properties - Playback State

        /// <summary>
        /// Gets whether the sound is currently playing.
        /// </summary>
        public bool IsPlaying => _instance?.State == SoundState.Playing;

        /// <summary>
        /// Gets whether the sound is currently paused.
        /// </summary>
        public bool IsPaused => _instance?.State == SoundState.Paused;

        /// <summary>
        /// Gets whether the sound is currently stopped.
        /// </summary>
        public bool IsStopped => _instance?.State == SoundState.Stopped;

        /// <summary>
        /// Gets the current playback state of the sound.
        /// </summary>
        public SoundState State => _instance?.State ?? SoundState.Stopped;

        #endregion

        #region Properties - Native

        /// <summary>
        /// Gets the underlying MonoGame SoundEffect.
        /// </summary>
        public SoundEffect NativeSoundEffect => _soundEffect;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class with the specified SoundEffect.
        /// </summary>
        /// <param name="soundEffect">The MonoGame SoundEffect to wrap.</param>
        public Sound(SoundEffect soundEffect)
        {
            _soundEffect = soundEffect ?? throw new ArgumentNullException(nameof(soundEffect));
            _instance = _soundEffect.CreateInstance();
        }

        #endregion

        #region Public Methods - Audio Control

        /// <summary>
        /// Sets the volume of the sound (0.0 to 1.0).
        /// </summary>
        /// <param name="value">The volume value to set.</param>
        public void SetVolume(float value)
        {
            if (_instance != null)
                _instance.Volume = Math.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Sets the pitch adjustment of the sound (-1.0 to 1.0).
        /// 0.0 is normal pitch, -1.0 is one octave down, 1.0 is one octave up.
        /// </summary>
        /// <param name="value">The pitch value to set.</param>
        public void SetPitch(float value)
        {
            if (_instance != null)
                _instance.Pitch = Math.Clamp(value, -1f, 1f);
        }

        /// <summary>
        /// Sets the pan of the sound (-1.0 to 1.0).
        /// -1.0 is full left, 0.0 is center, 1.0 is full right.
        /// </summary>
        /// <param name="value">The pan value to set.</param>
        public void SetPan(float value)
        {
            if (_instance != null)
                _instance.Pan = Math.Clamp(value, -1f, 1f);
        }

        /// <summary>
        /// Sets whether the sound should loop when it reaches the end.
        /// </summary>
        /// <param name="value">True to loop the sound, false otherwise.</param>
        public void SetLooped(bool value)
        {
            if (_instance != null)
                _instance.IsLooped = value;
        }

        #endregion

        #region Public Methods - Playback Control

        /// <summary>
        /// Plays the sound from the beginning.
        /// If the sound is already playing, it will restart from the beginning.
        /// </summary>
        public void Play()
        {
            if (_disposed)
                return;

            _instance?.Play();
        }

        /// <summary>
        /// Plays the sound with the specified volume, pitch, and pan settings.
        /// This is a one-shot play method that doesn't use the sound instance.
        /// </summary>
        /// <param name="volume">The volume (0.0 to 1.0).</param>
        /// <param name="pitch">The pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">The pan (-1.0 to 1.0).</param>
        public void Play(float volume, float pitch, float pan)
        {
            if (_disposed)
                return;

            _soundEffect?.Play(
                Math.Clamp(volume, 0f, 1f),
                Math.Clamp(pitch, -1f, 1f),
                Math.Clamp(pan, -1f, 1f)
            );
        }

        /// <summary>
        /// Pauses the sound playback at the current position.
        /// Call <see cref="Resume"/> to continue playback from where it was paused.
        /// </summary>
        public void Pause()
        {
            if (_disposed)
                return;

            _instance?.Pause();
        }

        /// <summary>
        /// Resumes the sound playback from the paused position.
        /// Has no effect if the sound is not paused.
        /// </summary>
        public void Resume()
        {
            if (_disposed || !IsPaused)
                return;

            _instance?.Resume();
        }

        /// <summary>
        /// Stops the sound playback immediately and resets the position to the beginning.
        /// </summary>
        public void Stop()
        {
            if (_disposed)
                return;

            _instance?.Stop();
        }

        /// <summary>
        /// Stops the sound playback with the specified behavior.
        /// </summary>
        /// <param name="immediate">True to stop immediately, false to let the sound finish naturally.</param>
        public void Stop(bool immediate)
        {
            if (_disposed)
                return;

            _instance?.Stop(immediate);
        }

        #endregion

        #region Public Methods - Configuration

        /// <summary>
        /// Sets all audio parameters at once for convenience.
        /// </summary>
        /// <param name="volume">The volume (0.0 to 1.0).</param>
        /// <param name="pitch">The pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">The pan (-1.0 to 1.0).</param>
        public void SetParameters(float volume, float pitch, float pan)
        {
            SetVolume(volume);
            SetPitch(pitch);
            SetPan(pan);
        }

        /// <summary>
        /// Resets all audio parameters to their default values.
        /// Volume = 1.0, Pitch = 0.0, Pan = 0.0, IsLooped = false.
        /// </summary>
        public void ResetParameters()
        {
            SetVolume(1f);
            SetPitch(0f);
            SetPan(0f);
            SetLooped(false);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the sound.
        /// Disposes the sound effect instance but not the underlying SoundEffect (managed by ContentManager).
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _instance?.Stop(true);
            _instance?.Dispose();
            _instance = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer to ensure resources are released if Dispose is not called.
        /// </summary>
        ~Sound()
        {
            Dispose();
        }

        #endregion
    }
}
