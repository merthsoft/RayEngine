using RayLib.Defs;
using System;

namespace RayLib.Objects
{
    public abstract class Actor : GameObject
    {
        protected static readonly Random Random = new Random();
        
        public virtual int TextureIndex { get; set; }
        public ActorDef ActorDef => (Def as ActorDef)!;

        public int ActTimer { get; set; }

        public override bool Blocking => ActorDef.Blocking;
        public override bool BlocksView => ActorDef.BlocksView;
        public override RenderStyle RenderStyle => ActorDef.RenderStyle;

        public State<ActionParameters>? CurrentState { get; set; }

        protected Actor()
            : base(ActorDef.Empty) { }

        public Actor(ActorDef def) 
            : base(def) { }

        public virtual void Act(ActionParameters parameters)
        {
            ActTimer--;
            if (ActTimer <= 0)
                ActTimer = 0;

            if (CurrentState == null)
                return;

            if (CurrentState.Action(parameters))
            {
                CurrentState = CurrentState.Advance();
                ActTimer = CurrentState.GetTimer();
            }
        }

        public static bool InteractingWith(ActionParameters parameters)
        {
            var player = parameters.Player;
            var t = parameters.Actor;
            if (!player.Interacting)
                return false;

            var interactionCell = player.Direction + player.Location;
            return interactionCell.Floor() == t.Location.Floor();
        }

        public static bool TimerUp(ActionParameters parameters)
            => parameters.Actor.ActTimer == 0;
        
        public override RayTexture GetTexture(double viewAngle)
            => GetTextureFromAngle(ActorDef.Textures[TextureIndex], viewAngle);
    }

    public abstract class Actor<TData> : Actor
    {
        public new State<ActionParameters, TData>? CurrentState { get => base.CurrentState as State<ActionParameters, TData>; set => base.CurrentState = value; }

        protected Actor() { }

        protected Actor(ActorDef def) : base(def) { }
    }
}
