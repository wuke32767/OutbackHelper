using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.OutbackHelper
{
    public class OutbackModule : EverestModule
    {
        public static SpriteBank SpriteBank;
    
        private static FieldInfo pufferPushRadius = typeof(Puffer).GetField("pushRadius", BindingFlags.Instance | BindingFlags.NonPublic);

        private const string ENTITY_PREFIX = "outback/";

        public override void Load()
        {
            Everest.Events.Level.OnLoadEntity += new Everest.Events.Level.LoadEntityHandler(this.OnLoadEntity);
            On.Celeste.Puffer.Explode += Puffer_Explode;
        }

        public override void LoadContent(bool firstLoad)
        {
            SpriteBank = new SpriteBank(GFX.Game, "Graphics/OutbackSprites.xml");
        }

        public override void Unload()
        {
            Everest.Events.Level.OnLoadEntity -= new Everest.Events.Level.LoadEntityHandler(this.OnLoadEntity);
            On.Celeste.Puffer.Explode -= Puffer_Explode;
        }

        private void Puffer_Explode(On.Celeste.Puffer.orig_Explode orig, Puffer self)
        {
            orig.Invoke(self);
            Collider collider = self.Collider;
            self.Collider = (Collider)pufferPushRadius.GetValue(self);
            turnOnSpecialTouchSwitches(self);
            self.Collider = collider;
        }

        private void turnOnSpecialTouchSwitches(Entity self)
        {
            foreach (Entity switchEntity in self.Scene.Tracker.GetEntities<MovingTouchSwitch>())
            {
                MovingTouchSwitch movingTouchSwitch = (MovingTouchSwitch)switchEntity;
                if (self.CollideCheck(movingTouchSwitch))
                {
                    movingTouchSwitch.TriggerNextState();
                }
            }
            foreach (Entity switchEntity in self.Scene.Tracker.GetEntities<TimedTouchSwitch>())
            {
                TimedTouchSwitch timedTouchSwitch = (TimedTouchSwitch)switchEntity;
                if (self.CollideCheck(timedTouchSwitch))
                {
                    timedTouchSwitch.TryTurnOn();
                }
            }
        }

        private bool OnLoadEntity(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            if (!entityData.Name.StartsWith(ENTITY_PREFIX))
            {
                return false;
            }
            else
            {
                switch (entityData.Name.Substring(ENTITY_PREFIX.Length))
                {
                    case "movingtouchswitch":
                        level.Add(new MovingTouchSwitch(entityData.NodesOffset(offset), entityData, offset));
                        break;
                    case "portal":
                        level.Add(new Portal(entityData.NodesOffset(offset), entityData, offset));
                        break;
                    case "timedtouchswitch":
                        level.Add(new TimedTouchSwitch(entityData, offset));
                        break;
                    case "destroycrystalstrigger":
                        level.Add(new DestroyCrystalsTrigger(entityData, offset));
                        break;
                    case "completeareatrigger":
                        level.Add(new CompleteAreaTrigger(entityData, offset));
                        break;
                }
            }
            return true;
        }
    }
}