﻿using Microsoft.Xna.Framework;
using MonoGame.Helper.ECS.Components.Drawables;
using MonoGame.Helper.ECS.Systems;
using MonoGame.Helper.Extensions;
using MonoGame.Helper.Physic.Components;

namespace Collision.Systems
{
    public class BorderControllerSystem : MonoGame.Helper.ECS.System, IInitializable
    {
        public void Initialize()
        {
            var horizontalBorderTexture = Scene.GameCore.GraphicsDevice.CreateTextureRectangle(Scene.ScreenWidth, 25, Color.Maroon);
            var verticalBorderTexture = Scene.GameCore.GraphicsDevice.CreateTextureRectangle(25, Scene.ScreenHeight, Color.Maroon);

            var horizontalBorderSpriteComponent = new SpriteComponent(horizontalBorderTexture);
            var verticalBorderSpriteComponent = new SpriteComponent(verticalBorderTexture);

            Scene.CreateEntity("left-border")
                .SetPosition(verticalBorderSpriteComponent.Origin.X, verticalBorderSpriteComponent.Origin.Y)
                .AddComponent(verticalBorderSpriteComponent)
                .AddComponent<BodyComponent>();

            Scene.CreateEntity("up-border")
                .SetPosition(horizontalBorderSpriteComponent.Origin.X, horizontalBorderSpriteComponent.Origin.Y)
                .AddComponent(horizontalBorderSpriteComponent)
                .AddComponent<BodyComponent>();

            Scene.CreateEntity("right-border")
                .SetPosition(Scene.ScreenWidth - verticalBorderSpriteComponent.Origin.X, verticalBorderSpriteComponent.Origin.Y)
                .AddComponent(verticalBorderSpriteComponent)
                .AddComponent<BodyComponent>();

            Scene.CreateEntity("down-border")
                .SetPosition(horizontalBorderSpriteComponent.Origin.X, Scene.ScreenHeight - horizontalBorderSpriteComponent.Origin.Y)
                .AddComponent(horizontalBorderSpriteComponent)
                .AddComponent<BodyComponent>();
        }
    }
}