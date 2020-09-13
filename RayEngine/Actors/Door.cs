using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayEngine.Actors
{
    public class Door : Actor<Door.DoorState>
    {
        public record DoorState
        {
            public bool Blocking { get; set; }
            public bool BlocksView { get; set; }
            public int TextureIndex { get; set; }
        }

        protected static readonly State<ActionParameters, DoorState> closedState = new(InteractingWith, new() { Blocking = true, BlocksView = true, TextureIndex = 0, });
        protected static readonly State<ActionParameters, DoorState> openingState = new(15, TimerUp, new() { Blocking = true, BlocksView = false, TextureIndex = 1, });
        protected static readonly State<ActionParameters, DoorState> openState = new(150, a => TimerUp(a) || InteractingWith(a), new() { Blocking = false, BlocksView = false, TextureIndex = 2, });
        protected static readonly State<ActionParameters, DoorState> closingState = new(15, TimerUp, new() { Blocking = true, BlocksView = false, TextureIndex = 1, });

        private List<RayTexture> Textures { get; } = new();

        public WallDef BackWall { get; set; } = WallDef.Empty;

        public override bool Blocking => CurrentState!.Data.Blocking;
        public override bool BlocksView => CurrentState!.Data.BlocksView;
        public override int TextureIndex => CurrentState!.Data.TextureIndex;

        static Door()
        {
            closedState
                .Chain(openingState)
                .Chain(openState)
                .Chain(closingState)
                .Chain(closedState);
        }

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

            CurrentState = closedState;
        }

        public override RayTexture GetTexture(double viewAngle)
            => Textures[TextureIndex];
    }
}
