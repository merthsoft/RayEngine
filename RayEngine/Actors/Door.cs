using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayEngine.Actors
{
    public class Door : Actor
    {
        private List<RayTexture[]> Textures { get; } = new();

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
                    Textures.Add(new[]
                    {
                        texture.Zip(BackWall.EastWestTexture),
                        texture.Zip(BackWall.NorthSouthTexture),
                    });
        }

        public override void Act(Map map, Player player)
        { 

        }

        public override RayTexture GetTexture(double viewAngle)
            => GetTextureFromAngle(Textures[TextureIndex], viewAngle);
    }
}
