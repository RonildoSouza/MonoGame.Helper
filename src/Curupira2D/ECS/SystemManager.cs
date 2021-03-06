﻿using Curupira2D.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Curupira2D.ECS
{
    internal sealed class SystemManager : IDisposable
    {
        readonly List<ILoadable> _loadableSystems = new List<ILoadable>();
        readonly List<IUpdatable> _updatableSystems = new List<IUpdatable>();
        readonly List<IRenderable> _renderableSystems = new List<IRenderable>();
        static readonly Lazy<SystemManager> _systemManager = new Lazy<SystemManager>(() => new SystemManager());

        SystemManager() { }

        public static SystemManager Instance => _systemManager.Value;

        public void LoadableSystemsIteration()
        {
            for (int i = 0; i < _loadableSystems.Count; i++)
                _loadableSystems[i].LoadContent();
        }

        public void UpdatableSystemsIteration()
        {
            for (int i = 0; i < _updatableSystems.Count; i++)
            {
                if (SystemIsValid(_updatableSystems[i]))
                    _updatableSystems[i].Update();
            }
        }

        public void RenderableSystemsIteration()
        {
            for (int i = 0; i < _renderableSystems.Count; i++)
            {
                if (SystemIsValid(_renderableSystems[i]))
                    _renderableSystems[i].Draw();
            }
        }

        public void Add<TSystem>(Scene scene, TSystem system) where TSystem : System
        {
            if (system is ILoadable && !_loadableSystems.Any(_ => _.GetType().Name == typeof(TSystem).Name))
                _loadableSystems.Add(system as ILoadable);

            if (system is IUpdatable && !_updatableSystems.Any(_ => _.GetType().Name == typeof(TSystem).Name))
                _updatableSystems.Add(system as IUpdatable);

            if (system is IRenderable && !_renderableSystems.Any(_ => _.GetType().Name == typeof(TSystem).Name))
                _renderableSystems.Add(system as IRenderable);

            system.SetScene(scene);
        }

        public void Add<TSystem>(Scene scene, params object[] args) where TSystem : System
        {
            var system = (TSystem)Activator.CreateInstance(typeof(TSystem), args);
            Add(scene, system);
        }

        public void Remove<TSystem>() where TSystem : System
        {
            _loadableSystems.RemoveAll(_ => _.GetType().Name == typeof(TSystem).Name);
            _updatableSystems.RemoveAll(_ => _.GetType().Name == typeof(TSystem).Name);
            _renderableSystems.RemoveAll(_ => _.GetType().Name == typeof(TSystem).Name);
        }

        public void RemoveAll()
        {
            _loadableSystems.RemoveAll(_ => true);
            _updatableSystems.RemoveAll(_ => true);
            _renderableSystems.RemoveAll(_ => true);
        }

        public void Dispose()
        {
            RemoveAll();
            _systemManager.Value.Dispose();

            GC.Collect();
        }

        bool SystemIsValid(ISystem system) => system.Scene != null && system.Scene.GameTime != null;
    }
}
