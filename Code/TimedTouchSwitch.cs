using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;
using System.Reflection;

namespace Celeste.Mod.OutbackHelper
{
	[Tracked(false)]
	public class TimedTouchSwitch : TouchSwitch
	{
        private Vector2 start;

        private float startDisappearTime;
        private float currentDisappearTime;

        private PlayerCollider playerCollider;
        private SeekerCollider seekerCollider;
        private HoldableCollider holdableCollider;
        
        private Sprite icon;
        private MTexture border;

        private bool emittedFire = false;
        private List<Entity> switchGates;

        private enum DisappearTimes
        {
            Fast = 5,
            Medium = 10,
            Slow = 15
        }

        private Level level
		{
			get
			{
				return (Level)Scene;
			}
		}
		
		public TimedTouchSwitch(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			startDisappearTime = (float)data.Enum("startDisappearTime", DisappearTimes.Slow);
            icon = new Sprite(GFX.Game, "collectables/outback/timedtouchswitch/idle");
            Remove(Get<Sprite>());
            Add(icon);
            border = GFX.Game["collectables/outback/timedtouchswitch/cont"];
            icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), new int[]
            {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7   
            });
            icon.Play("spin", false, false);
            icon.CenterOrigin();

            playerCollider = Get<PlayerCollider>();
			playerCollider.OnCollide = new Action<Player>(OnPlayer);

			seekerCollider = Get<SeekerCollider>();
			seekerCollider.OnCollide = new Action<Seeker>(OnSeeker);

			holdableCollider = Get<HoldableCollider>();
			holdableCollider.OnCollide = new Action<Holdable>(OnHoldable);
		}
		
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = scene as Level;
			if (level != null)
			{
				start = Position;
				currentDisappearTime = Math.Max(0, startDisappearTime);
				icon.Color = Color.White;
                icon.Rate = 0;
                Add(new Coroutine(FadeOut(), true));
			}
		}
		
		public override void Awake(Scene scene)
		{
			switchGates = scene.Entities.OfType<SwitchGate>().ToList<Entity>();
			base.Awake(scene);
		}

		private void OnPlayer(Player player)
		{
			TryTurnOn();
		}
		
		private void OnSeeker(Seeker seeker)
		{
			if (SceneAs<Level>().InsideCamera(Position, 10f))
			{
				TryTurnOn();
			}
		}

		private void OnHoldable(Holdable holdable)
		{
			TryTurnOn();
		}

		public void TryTurnOn()
		{
			if (currentDisappearTime > 0f)
			{
				TurnOn();
			}
		}
		private IEnumerator FadeOut()
		{
            // If player not spawned or moved
            Player player;
            while ((player = Scene.Tracker.GetEntity<Player>()) == null || player.JustRespawned)
            {
                yield return null;
            }
            while (!Switch.Activated && currentDisappearTime > 0)
			{
                icon.SetAnimationFrame(8 - (int)Math.Floor(8 * currentDisappearTime / startDisappearTime));
                currentDisappearTime -= Engine.DeltaTime;
				if (currentDisappearTime <= 0f)
				{
					Add(new Coroutine(TurnRed(icon, new Color(1f, 0.2f, 0.2f) * 0.1f), true));
                    icon.Stop();
					if (!emittedFire)
					{
						for (int i = 0; i < 32; i++)
						{
							float angle = Calc.Random.NextFloat(2 * (float)Math.PI);
							level.Particles.Emit(P_FireWhite, Position + Calc.AngleToVector(angle, 6f), angle);
						}
						Audio.Play("event:/game/04_cliffside/arrowblock_break", Position);
						emittedFire = true;
					}
					for (int i = 0; i < switchGates.Count; i++)
					{
						SwitchGate switchGateEntity = (SwitchGate)switchGates[i];
						Sprite switchGateIcon = switchGateEntity.Get<Sprite>();
						Add(new Coroutine(TurnRed(switchGateIcon, new Color(1f, 0.2f, 0.2f)), true));
						switchGateEntity = null;
						switchGateIcon = null;
					}
				}
				yield return null;
			}
            if (Switch.Activated)
            {
                icon.Rate = 50;
            }
		}

		private IEnumerator TurnRed(Sprite sprite, Color targetColor)
		{
			Color startColor = sprite.Color;
			float rate = 1f;
			while (rate > 0)
			{
				sprite.Color = Color.Lerp(startColor, targetColor, 1f - rate);
				rate -= Engine.DeltaTime * 4f;
				rate = Math.Max(0, rate);
				yield return null;
			}
			sprite.Color = targetColor;
		}
    }
}
