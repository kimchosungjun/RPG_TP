using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraTableClassGroup
{
    public class CameraTableClasses { }
    public class CameraShakeTableData
    {
        public int cameraID;
        public float startDelay;
        public float shakeTime;
        public float shakeMagnitudeX;
        public float shakeMagnitudeY;
        public float shakeSpeed;
        public float damping;
        public int maxShakeCount;
        public int localSetting;
    }
}
