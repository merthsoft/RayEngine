using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayEngine.Actors
{
    public class Door : Actor
    {
        private List<RayTexture> Textures { get; } = new();

        public WallDef BackWall { get; set; } = WallDef.Empty;

        public Door()
            => BackWall = WallDef.Empty;

        public Door(ActorDef def)
            : base(def)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var textures in ActorDef.Textures)
                foreach (var texture in textures)
                    Textures.Add(Direction == GameVector.East || Direction == GameVector.West
                        ? texture.Zip(BackWall.NorthSouthTexture) // Seems backwards at first but it makes sense
                        : texture.Zip(BackWall.EastWestTexture));
        }

        public override void Act(Map map, Player player)
        {
            base.Act(map, player);
            if (ActTimer == 0)
            {
                TextureIndex = (TextureIndex + 1) % Textures.Count;
                ActTimer = 50;
            }
        }

        public override RayTexture GetTexture(double viewAngle)
            => Textures[TextureIndex];
    }
}
