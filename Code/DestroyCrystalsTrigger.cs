using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.OutbackHelper
{
    public class DestroyCrystalsTrigger : Trigger
    {
        private int destructionType = 0;

        private enum DestroyTypes
        {
            InTrigger,
            EveryCrystal
        }

        public DestroyCrystalsTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            destructionType = (int)data.Enum("destroyEveryCrystal", DestroyTypes.InTrigger);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            List<CrystalStaticSpinner> list = Scene.Entities.OfType<CrystalStaticSpinner>().ToList();
            if (list.Count > 0)
            {
                foreach (CrystalStaticSpinner crystalStaticSpinner in list)
                {
                    if (CollideCheck(crystalStaticSpinner) || destructionType == (int)DestroyTypes.EveryCrystal)
                    {
                        crystalStaticSpinner.Destroy(false);
                    }
                }
            }
            RemoveSelf();
        }
    }
}
