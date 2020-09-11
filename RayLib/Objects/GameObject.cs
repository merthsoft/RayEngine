using RayLib.Defs;
using System;
using System.Collections.Generic;

namespace RayLib.Objects
{
    public abstract class GameObject
    {
        public static GameVector StandardPlaneEffect(GameVector pv)
            => -pv.GetPerpendicularVector();

        public Def Def { get; set; }

        public GameVector Location { get; set; } = GameVector.Zero;

        private GameVector direction = (0, 0);
        public GameVector Direction
        { 
            get => direction;
            set
            {
                direction = value;
                PlaneCache = null;
            }
        }

        private double fieldOfView = 0.66;
        public double FieldOfView
        {
            get => fieldOfView;
            set
            {
                fieldOfView = value;
                PlaneCache = null;
            }
        }

        private Func<GameVector, GameVector> planeEffect = StandardPlaneEffect;
        public Func<GameVector, GameVector> PlaneEffect
        { 
            get => planeEffect;
            set
            {
                planeEffect = value;
                PlaneCache = null;
            }
        }

        public GameVector? PlaneCache = null;
        public GameVector Plane => PlaneCache ??= PlaneEffect(Direction) * FieldOfView;

        public abstract bool Blocking { get; }
        public abstract bool BlocksView { get; }

        public GameObject(Def def)
            => Def = def;

        public abstract void Initialize();

        public abstract RayTexture GetTexture(double viewAngle);

        public GameObject SetLocation(GameVector v)
        {
            Location = v;
            return this;
        }

        public GameObject SetDirection(GameVector v)
        {
            Direction = v;
            return this;
        }

        public GameObject SetFieldOfView(double fieldOfView)
        {
            FieldOfView = fieldOfView;
            return this;
        }

        public static RayTexture GetTextureFromAngle(IList<RayTexture> textures, double viewAngle)
            => textures.Count switch
             {
                 0 => RayTexture.Empty,
                 1 => textures[0],
                 2 => textures[viewAngle.CardinalDirection2IndexDegrees()],
                 4 => textures[viewAngle.CardinalDirection4IndexDegrees()],
                 8 => textures[viewAngle.CardinalDirection8IndexDegrees()],
                 _ => textures[0],
             }
        ;
    }
}
