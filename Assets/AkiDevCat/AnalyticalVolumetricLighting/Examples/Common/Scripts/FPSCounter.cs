using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AkiDevCat.AVL.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace AkiDevCat.AVL.Examples
{
    [ExecuteInEditMode]
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private Volume _volume;
        [SerializeField] private int framesPerUpdate = 10;
        [SerializeField] private int queueSize = 30;

        private Queue<float> frameTimeQueue = new();
        
        private int counter = 0;
        private float time = 0.0f;
        private float lastTime = 0.0f;

        private void Update()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Application.isMobilePlatform ? Screen.currentResolution.refreshRate : -1;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_volume.sharedProfile.TryGet<AVLVolumeComponent>(out var cmp))
                    cmp.active = !cmp.active;
            }
            
            time += Time.deltaTime;
            
            if (++counter != framesPerUpdate)
            {
                return;
            }

            if (frameTimeQueue.Count >= queueSize)
                frameTimeQueue.Dequeue();
            
            frameTimeQueue.Enqueue(time / framesPerUpdate);
            lastTime = time / framesPerUpdate;

            counter = 0;
            time = 0.0f;
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 300, 200);
            GUI.Box(rect, "Performance Overview");

            StringBuilder result = new();

            result.AppendFormat("{0:0.0000} . . . Frame Time\n", lastTime);
            result.AppendFormat("{0:0.0000} . . . Frames Per Second\n", lastTime > 0.0f ? 1.0f / lastTime : 0.0f);

            float maxTime = float.MinValue, minTime = float.MaxValue;

            foreach (var t in frameTimeQueue)
            {
                maxTime = Mathf.Max(maxTime, t);
                minTime = Mathf.Min(minTime, t);
            }
            
            var graphMatrix = new char[(queueSize + 1) * 8];
            for (var y = 0; y < 8; y++)
                for (var x = 0; x < queueSize + 1; x++)
                {
                    if (x == queueSize)
                        graphMatrix[x + y * (queueSize + 1)] = '\n';
                    else
                        graphMatrix[x + y * (queueSize + 1)] = '#';
                }

            int i = 0;
            foreach (var t in frameTimeQueue)
            {
                var nt = (t - minTime) / (maxTime - minTime);

                for (var y = 0; y < 8; y++)
                {
                    if (nt < y / 8.0f)
                        graphMatrix[i + (7 - y) * (queueSize + 1)] = '0';
                    else
                        graphMatrix[i + (7 - y) * (queueSize + 1)] = '1';
                }
                
                i++;
            }

            result.Append(new string(graphMatrix));
            
            GUI.Label(new Rect(20, 30, 300 - 20 - 10, 200 - 30 - 10), result.ToString());
        }
    }
}