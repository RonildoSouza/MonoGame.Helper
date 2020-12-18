﻿using MonoGame.Helper.ECS;
using MonoGame.Helper.Samples.Systems.SceneGraph;

namespace MonoGame.Helper.Samples.Scenes
{
    class SceneGraphScene : Scene
    {
        public override void Initialize()
        {
            AddSystem<CharacterMovementSystem>();
            AddSystem<EquipmentMovimentSystem>();

            base.Initialize();
        }
    }
}
