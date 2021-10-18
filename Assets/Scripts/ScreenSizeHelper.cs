using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ymir.ScreenHelper
{
    public class ScreenSizeHelper
    {
        private Vector3 screenCenter;
        public Vector3 ScreenCenter { get { return screenCenter; } }

        public void InitializeScreenCenter()
        {
            screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        }

        public void AdjustForScreenResolutionChange()
        {
            if (screenCenter.x != Screen.width / 2 || screenCenter.y != Screen.height / 2)
            {
                screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
            }
        }

    }
}