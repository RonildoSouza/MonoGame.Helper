﻿using Curupira2D.ECS.Components.Drawables;
using Curupira2D.ECS.Components.Physics;
using Curupira2D.ECS.Systems.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TiledLib;
using TiledLib.Layer;
using TiledLib.Objects;

namespace Curupira2D.ECS.Systems.Drawables
{
    [RequiredComponent(typeof(TiledMapSystem), typeof(TiledMapComponent))]
    public class TiledMapSystem : DrawableSystem<TiledMapComponent>, ILoadable
    {
        public void LoadContent()
        {
            var entities = Scene.GetEntities(_ => MatchActiveEntitiesAndComponents(_));

            for (int i = 0; i < entities.Count; i++)
            {
                var tiledMapEntity = entities[i];
                var tiledMapComponent = tiledMapEntity.GetComponent<TiledMapComponent>();

                foreach (var objectLayer in tiledMapComponent.Map.Layers.OfType<ObjectLayer>().Where(_ => _.Visible))
                {
                    foreach (var baseObject in objectLayer.Objects)
                    {
                        CreateCollisionEntities(baseObject);

                        // Gets entity with the same name as the point object and sets the position
                        if (baseObject is PointObject pointObject)
                        {
                            var entity = Scene
                                .GetEntities(_ => _.UniqueId == pointObject.Name || _.UniqueId == pointObject.Properties.GetValue(TiledMapSystemConstants.Properties.EntityUniqueId))
                                .FirstOrDefault();

                            if (entity != null && entity.Transform.Position == default)
                                entity.SetPosition((float)pointObject.X, (float)pointObject.Y);
                        }
                    }
                }
            }
        }

        protected override void DrawEntities(ref IReadOnlyList<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var tiledMapEntity = entities[i];
                var tiledMapComponent = tiledMapEntity.GetComponent<TiledMapComponent>();
                var layers = tiledMapComponent.Map.Layers
                    .OfType<TileLayer>()
                    .OrderBy(_ =>
                    {
                        var propertyValueOrder = _.Properties.GetValue(TiledMapSystemConstants.Properties.Order);
                        return string.IsNullOrEmpty(propertyValueOrder) ? _.Id : int.Parse(propertyValueOrder);
                    })
                    .Where(_ => _.Visible);

                foreach (var layer in layers)
                {
                    for (int y = 0, j = 0; y < layer.Height; y++)
                    {
                        for (int x = 0; x < layer.Width; x++, j++)
                        {
                            var gid = layer.Data[j];
                            if (gid == 0)
                                continue;

                            var tileset = tiledMapComponent.Map.Tilesets.Single(_ => Utils.GetId(gid) >= _.FirstGid && _.FirstGid + _.TileCount > Utils.GetId(gid));
                            var tile = tileset[gid];

                            GetTileOrientation(tile, out var tileSpriteEffect, out var tileRotation);

                            var tilePosX = x * tile.Width + (int)(tile.Width * 0.5f);
                            var tilePosY = (int)Scene.InvertPositionY(y * tile.Height + (int)(tile.Height * 0.5f));

                            Scene.SpriteBatch.Draw(
                                tiledMapComponent.Texture,
                                new Rectangle(tilePosX, tilePosY, tile.Width, tile.Height),
                                new Rectangle(tile.Left, tile.Top, tile.Width, tile.Height),
                                Color.White,
                                MathHelper.ToRadians(tileRotation),
                                tiledMapComponent.Origin,
                                tileSpriteEffect | tiledMapComponent.SpriteEffect,
                                tiledMapComponent.LayerDepth);
                        }
                    }
                }
            }
        }

        void CreateCollisionEntities(BaseObject baseObject)
        {
            var entityUniqueId = baseObject.Properties.GetValue(TiledMapSystemConstants.Properties.EntityUniqueId);
            var entityGroup = baseObject.Properties.GetValue(TiledMapSystemConstants.Properties.EntityGroup);

            // Creates ellipse type collision entity
            if (baseObject is EllipseObject ellipseObject)
            {
                var posX = (float)ellipseObject.X + (float)ellipseObject.Width * 0.5f;
                var posY = Scene.InvertPositionY((float)ellipseObject.Y + (float)ellipseObject.Height * 0.5f);

                Scene.CreateEntity(entityUniqueId ?? $"{nameof(EllipseObject)}_{ellipseObject.Id}", entityGroup)
                    .SetPosition(posX, posY)
                    .AddComponent(new BodyComponent((float)ellipseObject.Width, (float)ellipseObject.Height, EntityType.Static, EntityShape.Ellipse));
            }

            // Creates rectangle type collision entity
            if (baseObject is RectangleObject rectangleObject)
            {
                var posX = (float)rectangleObject.X + (float)rectangleObject.Width * 0.5f;
                var posY = Scene.InvertPositionY((float)rectangleObject.Y + (float)rectangleObject.Height * 0.5f);

                Scene.CreateEntity(entityUniqueId ?? $"{nameof(RectangleObject)}_{rectangleObject.Id}", entityGroup)
                    .SetPosition(posX, posY)
                    .AddComponent(new BodyComponent((float)rectangleObject.Width, (float)rectangleObject.Height, EntityType.Static, EntityShape.Rectangle));
            }

            // Creates polygon type collision entity
            if (baseObject is PolygonObject polygonObject)
            {
                Scene.CreateEntity(entityUniqueId ?? $"{nameof(PolygonObject)}_{polygonObject.Id}", entityGroup)
                    .SetPosition((float)polygonObject.X, Scene.InvertPositionY((float)polygonObject.Y))
                    .AddComponent(new BodyComponent((float)polygonObject.Width, (float)polygonObject.Height, EntityType.Static, EntityShape.Polygon)
                    {
                        Vertices = polygonObject.Polygon.Select(_ => new Vector2((float)_.X, (float)_.Y))
                    });
            }
        }

        void GetTileOrientation(Tile tile, out SpriteEffects tileSpriteEffect, out float tileRotation)
        {
            tileSpriteEffect = SpriteEffects.None;
            tileRotation = 0f;

            switch (tile.Orientation)
            {
                case TileOrientation.FlippedH:
                    tileSpriteEffect = SpriteEffects.FlipHorizontally;
                    break;
                case TileOrientation.FlippedV:
                    tileSpriteEffect = SpriteEffects.FlipVertically;
                    break;
                case TileOrientation.FlippedAD:
                    tileSpriteEffect = SpriteEffects.FlipVertically;
                    tileRotation = 90f;
                    break;
                case TileOrientation.Rotate90CW:
                    tileRotation = 90f;
                    break;
                case TileOrientation.Rotate180CCW:
                    tileRotation = 180f;
                    break;
                case TileOrientation.Rotate270CCW:
                    tileRotation = 270f;
                    break;
            }
        }
    }
}
