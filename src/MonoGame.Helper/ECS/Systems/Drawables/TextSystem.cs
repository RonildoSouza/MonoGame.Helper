﻿using MonoGame.Helper.ECS.Components.Drawables;
using MonoGame.Helper.ECS.Systems.Attributes;
using MonoGame.Helper.Extensions;
using System.Collections.Generic;

namespace MonoGame.Helper.ECS.Systems.Drawables
{
    [RequiredComponent(typeof(TextSystem), typeof(TextComponent))]
    public class TextSystem : DrawableSystem<TextComponent>
    {
        protected override void DrawEntities(ref IReadOnlyList<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var textComponent = entity.GetComponent<TextComponent>();

                Scene.SpriteBatch.DrawString(entity, textComponent);
            }
        }
    }
}
