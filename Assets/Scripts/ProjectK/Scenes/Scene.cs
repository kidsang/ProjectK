using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.ProjectK.Base;
using Assets.Scripts.ProjectK.Entites;

namespace Assets.Scripts.ProjectK.Scenes
{
    public class Scene : DisposableBehaviour
    {
        private GameObject sceneRoot;
        private ResourceLoader loader;

        internal void Init()
        {
            sceneRoot = gameObject;
            loader = new ResourceLoader();

            // todo test:
            SceneEntityManager.Create(loader, 1);
        }

        protected override void OnDispose()
        {
            loader.Dispose();
            DestroyObject(sceneRoot);

            base.OnDispose();
        }
    }
}
