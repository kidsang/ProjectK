using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProjectK.Base
{
    public class PrefabResource : Resource
    {
        private GameObject gameObject;

        internal override void Load()
        {
            gameObject = Resources.Load<GameObject>(Url);
            if (gameObject == null)
            {
                loadFailed = true;
                Log.Error("资源加载错误! Url:", Url, "\nType:", GetType());
            }

            state = ResourceState.Complete;
        }

        protected override void OnDispose()
        {
            if (gameObject != null)
            {
                // 没办法直接Unload一个GameObject，也许只能调用Resources.UnloadUnusedAssets()
                //Resources.UnloadAsset(gameObject);
                gameObject = null;
            }

            base.OnDispose();
        }

        public GameObject Instantiate()
        {
            if (gameObject == null)
                return null;
            return (GameObject)GameObject.Instantiate(gameObject);
        }
    }
}
