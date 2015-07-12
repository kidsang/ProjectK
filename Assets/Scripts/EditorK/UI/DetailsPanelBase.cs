using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProjectK.Base;

namespace EditorK.UI
{
    public class DetailsPanelBase : DisposableBehaviour
    {
        virtual public void OnEnable()
        {

        }

        virtual public void OnDisable()
        {
            EventManager.Instance.UnregisterAll(this);
        }

        virtual public void Refresh(InfoMap infos)
        {
        }
    }
}
