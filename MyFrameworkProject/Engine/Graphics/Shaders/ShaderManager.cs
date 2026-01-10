using System;
using System.Collections.Generic;

using MyFrameworkProject.Engine.Core;

namespace MyFrameworkProject.Engine.Graphics.Shaders
{
    /// <summary>
    /// Manages shader effects for the entire application, supporting both global shaders
    /// that apply to all entities and entity-specific shaders.
    /// Provides automatic shader lifecycle management and efficient parameter updates.
    /// </summary>
    public sealed class ShaderManager : IDisposable
    {
        #region Fields - Shader Collections

        /// <summary>
        /// Collection of global shaders that apply to all world-space entities during rendering.
        /// Shaders are applied in the order they were added.
        /// </summary>
        private readonly List<ShaderEffect> _globalWorldShaders;

        /// <summary>
        /// Collection of global shaders that apply to all UI-space elements during rendering.
        /// Shaders are applied in the order they were added.
        /// </summary>
        private readonly List<ShaderEffect> _globalUIShaders;

        /// <summary>
        /// Named shader library for easy retrieval and reuse.
        /// Maps shader names to their corresponding ShaderEffect instances.
        /// </summary>
        private readonly Dictionary<string, ShaderEffect> _namedShaders;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether the shader manager has been disposed.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Properties - Global Shader Access

        /// <summary>
        /// Gets the read-only list of global world shaders.
        /// Used by the Renderer to apply shaders during world rendering.
        /// </summary>
        public IReadOnlyList<ShaderEffect> GlobalWorldShaders => _globalWorldShaders;

        /// <summary>
        /// Gets the read-only list of global UI shaders.
        /// Used by the Renderer to apply shaders during UI rendering.
        /// </summary>
        public IReadOnlyList<ShaderEffect> GlobalUIShaders => _globalUIShaders;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderManager"/> class.
        /// </summary>
        public ShaderManager()
        {
            _globalWorldShaders = [];
            _globalUIShaders = [];
            _namedShaders = [];

            Logger.Info("ShaderManager initialized");
        }

        #endregion

        #region Public Methods - Named Shader Management

        /// <summary>
        /// Registers a shader effect with a unique name for easy retrieval.
        /// Named shaders can be applied to entities or used as global shaders.
        /// </summary>
        /// <param name="name">The unique identifier for the shader.</param>
        /// <param name="shader">The shader effect to register.</param>
        /// <returns>True if the shader was registered successfully, false if the name already exists.</returns>
        public bool RegisterShader(string name, ShaderEffect shader)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.Warning("Cannot register shader with null or empty name");
                return false;
            }

            if (shader == null)
            {
                Logger.Warning($"Cannot register null shader with name '{name}'");
                return false;
            }

            if (_namedShaders.ContainsKey(name))
            {
                Logger.Warning($"Shader with name '{name}' is already registered");
                return false;
            }

            _namedShaders[name] = shader;
            Logger.Info($"Shader '{name}' registered successfully");
            return true;
        }

        /// <summary>
        /// Retrieves a registered shader effect by name.
        /// </summary>
        /// <param name="name">The name of the shader to retrieve.</param>
        /// <returns>The shader effect if found, otherwise null.</returns>
        public ShaderEffect GetShader(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            _namedShaders.TryGetValue(name, out var shader);
            return shader;
        }

        /// <summary>
        /// Unregisters and optionally disposes a shader effect by name.
        /// </summary>
        /// <param name="name">The name of the shader to unregister.</param>
        /// <param name="dispose">Whether to dispose the shader effect. Default is false as effects are managed by ContentManager.</param>
        /// <returns>True if the shader was unregistered, false if it was not found.</returns>
        public bool UnregisterShader(string name, bool dispose = false)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (_namedShaders.TryGetValue(name, out var shader))
            {
                _namedShaders.Remove(name);

                if (dispose)
                    shader.Dispose();

                Logger.Info($"Shader '{name}' unregistered");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all named shaders and optionally disposes them.
        /// </summary>
        /// <param name="dispose">Whether to dispose all shader effects. Default is false as effects are managed by ContentManager.</param>
        public void ClearAllNamedShaders(bool dispose = false)
        {
            if (dispose)
            {
                foreach (var shader in _namedShaders.Values)
                {
                    shader.Dispose();
                }
            }

            _namedShaders.Clear();
            Logger.Info("All named shaders cleared");
        }

        #endregion

        #region Public Methods - Global World Shader Management

        /// <summary>
        /// Adds a global shader that will be applied to all world-space entities.
        /// Global shaders are applied in the order they are added.
        /// </summary>
        /// <param name="shader">The shader effect to add globally.</param>
        public void AddGlobalWorldShader(ShaderEffect shader)
        {
            if (shader == null)
            {
                Logger.Warning("Cannot add null global world shader");
                return;
            }

            _globalWorldShaders.Add(shader);
            Logger.Info($"Global world shader added (total: {_globalWorldShaders.Count})");
        }

        /// <summary>
        /// Adds a global shader by name that will be applied to all world-space entities.
        /// </summary>
        /// <param name="shaderName">The name of the registered shader to add globally.</param>
        /// <returns>True if the shader was found and added, false otherwise.</returns>
        public bool AddGlobalWorldShader(string shaderName)
        {
            var shader = GetShader(shaderName);
            if (shader == null)
            {
                Logger.Warning($"Cannot add global world shader: '{shaderName}' not found");
                return false;
            }

            AddGlobalWorldShader(shader);
            return true;
        }

        /// <summary>
        /// Removes a global world shader.
        /// </summary>
        /// <param name="shader">The shader to remove.</param>
        /// <returns>True if the shader was removed, false if it was not found.</returns>
        public bool RemoveGlobalWorldShader(ShaderEffect shader)
        {
            return _globalWorldShaders.Remove(shader);
        }

        /// <summary>
        /// Clears all global world shaders.
        /// </summary>
        public void ClearGlobalWorldShaders()
        {
            _globalWorldShaders.Clear();
            Logger.Info("All global world shaders cleared");
        }

        #endregion

        #region Public Methods - Global UI Shader Management

        /// <summary>
        /// Adds a global shader that will be applied to all UI-space elements.
        /// Global shaders are applied in the order they are added.
        /// </summary>
        /// <param name="shader">The shader effect to add globally.</param>
        public void AddGlobalUIShader(ShaderEffect shader)
        {
            if (shader == null)
            {
                Logger.Warning("Cannot add null global UI shader");
                return;
            }

            _globalUIShaders.Add(shader);
            Logger.Info($"Global UI shader added (total: {_globalUIShaders.Count})");
        }

        /// <summary>
        /// Adds a global shader by name that will be applied to all UI-space elements.
        /// </summary>
        /// <param name="shaderName">The name of the registered shader to add globally.</param>
        /// <returns>True if the shader was found and added, false otherwise.</returns>
        public bool AddGlobalUIShader(string shaderName)
        {
            var shader = GetShader(shaderName);
            if (shader == null)
            {
                Logger.Warning($"Cannot add global UI shader: '{shaderName}' not found");
                return false;
            }

            AddGlobalUIShader(shader);
            return true;
        }

        /// <summary>
        /// Removes a global UI shader.
        /// </summary>
        /// <param name="shader">The shader to remove.</param>
        /// <returns>True if the shader was removed, false if it was not found.</returns>
        public bool RemoveGlobalUIShader(ShaderEffect shader)
        {
            return _globalUIShaders.Remove(shader);
        }

        /// <summary>
        /// Clears all global UI shaders.
        /// </summary>
        public void ClearGlobalUIShaders()
        {
            _globalUIShaders.Clear();
            Logger.Info("All global UI shaders cleared");
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the shader manager.
        /// Note: ShaderEffect instances are NOT disposed as they are managed by ContentManager.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _globalWorldShaders.Clear();
            _globalUIShaders.Clear();
            _namedShaders.Clear();

            _disposed = true;
            Logger.Info("ShaderManager disposed");
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer to ensure resources are released if Dispose is not called.
        /// </summary>
        ~ShaderManager()
        {
            Dispose();
        }

        #endregion
    }
}