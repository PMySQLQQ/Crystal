using System.Collections.Generic;
using Shared;

namespace Client.MirObjects
{
    public class DamageInfoPool
    {
       
        private static DamageInfoPool _instance;
        public static DamageInfoPool Instance => _instance ?? (_instance = new DamageInfoPool());

       
        private readonly Queue<DamageInfo> _pool = new Queue<DamageInfo>();
        private readonly int _maxPoolSize = 50; // 最大池大小

       
        public DamageInfo Get(int damageValue, DamageType type)
        {
            DamageInfo info;
            if (_pool.Count > 0)
            {
                info = _pool.Dequeue();
         
                info.DamageValue = damageValue;
                info.Type = type;
                info.StartTime = System.DateTime.Now;
                info.AppearDelay = System.TimeSpan.FromMilliseconds(500);
                info.ShowDelay = System.TimeSpan.FromSeconds(1);
                info.HideDelay = System.TimeSpan.FromMilliseconds(250);
                info.DrawY = 0;
                info.Opacity = 0;
                info.Visible = true;
            }
            else
            {
             
                info = new DamageInfo(damageValue, type);
            }
            return info;
        }


        public void Return(DamageInfo info)
        {
            if (info == null) return;
            
        
            if (_pool.Count < _maxPoolSize)
            {
                _pool.Enqueue(info);
            }
        }

    
        public void Clear()
        {
            _pool.Clear();
        }


        public int Count => _pool.Count;
    }
}