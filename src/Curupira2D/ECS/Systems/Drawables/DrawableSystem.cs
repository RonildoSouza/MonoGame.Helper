﻿using Curupira2D.ECS.Components.Drawables;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Curupira2D.ECS.Systems.Drawables
{
    public abstract class DrawableSystem<TDrawableComponent> : System, IRenderable
        where TDrawableComponent : DrawableComponent
    {
        public void Draw()
        {
            DrawEntitiesInUICamera();
            DrawEntitiesInCamera();
        }

        protected abstract void DrawEntities(ref IReadOnlyList<Entity> entities);

        void DrawEntitiesInUICamera()
        {
            Scene.SpriteBatch.Begin(
                sortMode: SpriteSortMode.BackToFront,
                rasterizerState: RasterizerState.CullClockwise,
                effect: Scene.UICamera2D.SpriteBatchEffect);

            var withoutUsingCameraEntities = Scene.GetEntities(_ =>
            {
                var drawableComponent = _.GetComponent<TDrawableComponent>();
                return MatchActiveEntitiesAndComponents(_) && drawableComponent.DrawInUICamera;
            });

            DrawEntities(ref withoutUsingCameraEntities);

            Scene.SpriteBatch.End();
        }

        void DrawEntitiesInCamera()
        {
            Scene.SpriteBatch.Begin(
                sortMode: SpriteSortMode.BackToFront,
                rasterizerState: RasterizerState.CullClockwise,
                effect: Scene.Camera2D.SpriteBatchEffect);

            var usingCameraEntities = Scene.GetEntities(_ =>
            {
                var drawableComponent = _.GetComponent<TDrawableComponent>();
                return MatchActiveEntitiesAndComponents(_) && !drawableComponent.DrawInUICamera;
            });

            DrawEntities(ref usingCameraEntities);

            Scene.SpriteBatch.End();
        }
    }
}
