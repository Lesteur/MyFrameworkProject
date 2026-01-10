using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics.Shaders
{
    /// <summary>
    /// Encapsulates a MonoGame shader effect with automatic parameter management.
    /// Provides a simplified interface for defining and applying visual effects.
    /// </summary>
    public sealed class ShaderEffect : IDisposable
    {
        #region Fields - Effect

        /// <summary>
        /// The underlying MonoGame effect containing the compiled shader code.
        /// </summary>
        private readonly Effect _nativeEffect;

        /// <summary>
        /// Cache of frequently used parameters to avoid repeated lookups.
        /// </summary>
        private readonly Dictionary<string, EffectParameter> _parameterCache;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether the effect has been disposed.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Properties - Effect

        /// <summary>
        /// Gets the native MonoGame effect for direct access if needed.
        /// </summary>
        public Effect NativeEffect => _nativeEffect;

        /// <summary>
        /// Gets or sets the current technique used by the shader.
        /// </summary>
        public EffectTechnique CurrentTechnique
        {
            get => _nativeEffect?.CurrentTechnique;
            set
            {
                if (_nativeEffect != null)
                    _nativeEffect.CurrentTechnique = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderEffect"/> class with the specified effect.
        /// </summary>
        /// <param name="effect">The MonoGame effect to encapsulate.</param>
        public ShaderEffect(Effect effect)
        {
            _nativeEffect = effect ?? throw new ArgumentNullException(nameof(effect));
            _parameterCache = new Dictionary<string, EffectParameter>();
        }

        #endregion

        #region Public Methods - Parameter Setting

        /// <summary>
        /// Sets the value of a float parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string parameterName, float value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of an int parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string parameterName, int value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Vector2 parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string parameterName, Vector2 value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Vector3 parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string parameterName, Vector3 value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Vector4 parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string parameterName, Vector4 value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Color parameter in the shader (converted to Vector4).
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The color to set.</param>
        public void SetParameter(string parameterName, Color value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value.ToVector4());
        }

        /// <summary>
        /// Sets the value of a Matrix parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The matrix to set.</param>
        public void SetParameter(string parameterName, Matrix value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Texture2D parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The texture to set.</param>
        public void SetParameter(string parameterName, Texture2D value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a float array parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The float array to set.</param>
        public void SetParameter(string parameterName, float[] value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        /// <summary>
        /// Sets the value of a Vector2 array parameter in the shader.
        /// </summary>
        /// <param name="parameterName">The name of the parameter in the shader.</param>
        /// <param name="value">The Vector2 array to set.</param>
        public void SetParameter(string parameterName, Vector2[] value)
        {
            var param = GetParameter(parameterName);
            param?.SetValue(value);
        }

        #endregion

        #region Public Methods - Technique Selection

        /// <summary>
        /// Selects a technique by name.
        /// </summary>
        /// <param name="techniqueName">The name of the technique to use.</param>
        /// <returns>True if the technique was found and selected, otherwise false.</returns>
        public bool SetTechnique(string techniqueName)
        {
            if (string.IsNullOrEmpty(techniqueName))
                return false;

            var technique = _nativeEffect.Techniques[techniqueName];
            if (technique != null)
            {
                CurrentTechnique = technique;
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods - Parameter Management

        /// <summary>
        /// Gets an effect parameter, using the cache if available.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The effect parameter, or null if it does not exist.</returns>
        private EffectParameter GetParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                return null;

            // Check the cache first
            if (_parameterCache.TryGetValue(parameterName, out var cachedParam))
                return cachedParam;

            // Search for the parameter in the effect
            var param = _nativeEffect.Parameters[parameterName];
            if (param != null)
            {
                _parameterCache[parameterName] = param;
            }

            return param;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the shader effect.
        /// Note: The native effect is NOT disposed as it is managed by the ContentManager.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _parameterCache.Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer to ensure resources are released if Dispose is not called.
        /// </summary>
        ~ShaderEffect()
        {
            Dispose();
        }

        #endregion
    }
}