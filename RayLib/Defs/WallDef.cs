﻿namespace RayLib.Defs
{
    public record WallDef : Def
    {
        public override string DefName => nameof(WallDef);

        public static new WallDef Empty { get; } = new WallDef("<EMPTY>", RayTexture.Empty, RayTexture.Empty);

        public RayTexture NorthSouthTexture { get; } = null!;
        public RayTexture EastWestTexture { get; } = null!;

        public WallDef(string name, RayTexture northSouthTexture, RayTexture eastWestTexture) 
            : base(name, northSouthTexture.Size)
            => (NorthSouthTexture, EastWestTexture)
             = (northSouthTexture, eastWestTexture);
    }
}
