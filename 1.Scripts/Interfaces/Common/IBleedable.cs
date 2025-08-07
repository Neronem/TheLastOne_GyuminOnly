using System.Collections;

namespace _1.Scripts.Interfaces.Common
{
    public interface IBleedable
    {
        public void OnBleed(int totalTick, float tickInterval, int damagePerTick);
    }
}