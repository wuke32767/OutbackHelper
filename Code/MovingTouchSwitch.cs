using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.OutbackHelper
{
    [Tracked(false)]
    public class MovingTouchSwitch : TouchSwitch
    {
        private Vector2 start;

        private int nodeIndex = 0;

        private Vector2[] touchSwitchNodes;

        private bool isMoving = false;

        private PlayerCollider playerCollider;
        private SeekerCollider seekerCollider;
        private HoldableCollider holdableCollider;

        private const float travelSpeed = 0.05f;

        private Sprite icon;

        private Level level
        {
            get
            {
                return (Level)base.Scene;
            }
        }

        public MovingTouchSwitch(Vector2[] nodes, EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            nodes = data.NodesOffset(offset);
            touchSwitchNodes = nodes;

            playerCollider = Get<PlayerCollider>();
            playerCollider.OnCollide = new Action<Player>(OnPlayer);

            seekerCollider = Get<SeekerCollider>();
            seekerCollider.OnCollide = new Action<Seeker>(OnSeeker);

            holdableCollider = Get<HoldableCollider>();
            holdableCollider.OnCollide = new Action<Holdable>(OnHoldable);

            icon = Get<Sprite>();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = scene as Level;

            if (level == null)
            {
                start = Position;
            }
        }

        private void OnPlayer(Player player)
        {
            TriggerNextState();
        }

        private void OnSeeker(Seeker seeker)
        {
            if (SceneAs<Level>().InsideCamera(Position, 10f))
            {
                TriggerNextState();
            }
        }

        private void OnHoldable(Holdable holdable)
        {
            TriggerNextState();
        }

        public void TriggerNextState()
        {
            if (!Switch.Activated)
            {
                Add(new Coroutine(TriggeredSwitch(), true));
            }
        }

        private IEnumerator TriggeredSwitch()
        {
            if (isMoving)
            {
                yield break;
            }

            isMoving = true;
            level.Shake(0.1f);

            if (nodeIndex < touchSwitchNodes.Length)
            {
                Audio.Play("event:/game/general/crystalheart_bounce", Position);

                for (int i = 0; i < 16; i++)
                {
                    float angle = Calc.Random.NextFloat(2 * (float)Math.PI);
                    level.Particles.Emit(P_FireWhite, Position + Calc.AngleToVector(angle, 6f), angle);
                }

                Vector2 targetPosition = touchSwitchNodes[nodeIndex];

                Add(new Coroutine(DrawPathParticles(Center, targetPosition + new Vector2(7f, 7f)), true));

                Tween.Position(this, targetPosition, 0.8f, Ease.SineOut);

                while (Position != targetPosition)
                {
                    icon.Color = new Color(1f, 0.5f, 0.5f);
                    yield return null;
                }

                for (int i = 0; i < 32; i++)
                {
                    float angle = Calc.Random.NextFloat(2 * (float)Math.PI);
                    level.Particles.Emit(P_FireWhite, Position + Calc.AngleToVector(angle, 6f), angle);
                }

                Audio.Play("event:/game/04_cliffside/greenbooster_dash", Position);
                nodeIndex++;
            }
            else
            {
                if (nodeIndex >= touchSwitchNodes.Length)
                {
                    TurnOn();
                }
            }

            isMoving = false;
            yield break;
        }

        private IEnumerator DrawPathParticles(Vector2 start, Vector2 end)
        {
            Vector2 currentPosition = start;
            while (currentPosition != end)
            {
                currentPosition = new Vector2(Calc.LerpSnap(currentPosition.X, end.X, 0.5f, 5f),
                    Calc.LerpSnap(currentPosition.Y, end.Y, 0.5f, 5f));

                if (nodeIndex + 1 == touchSwitchNodes.Length)
                {
                    level.Particles.Emit(P_Fire, currentPosition);
                }
                else
                {
                    level.Particles.Emit(P_FireWhite, currentPosition);
                }
                yield return null;
            }
            yield break;
        }
    }
}
