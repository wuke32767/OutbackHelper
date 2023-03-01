using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.OutbackHelper
{
    public class CompleteAreaTrigger : Trigger
    {
        private Level level;

        public CompleteAreaTrigger(EntityData data, Vector2 offset) : base(data, offset) {}

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            level.CompleteArea(true, false);
            player.StateMachine.State = 11;
            RemoveSelf();
        }
    }
}
