using System;
using UnityEngine;

namespace Wildfeast
{
    // One action owns its windup, one contact and recovery. Inputs are never queued.
    public sealed class ToolActionClock
    {
        public bool Busy { get; private set; }
        public int Tool { get; private set; }
        public float Progress => Busy ? Mathf.Clamp01(elapsed/duration) : 0;
        float elapsed, duration;
        bool contacted;
        Action impact;
        public static float Duration(int tool)=>tool==7||tool==9?.8f:tool==8?.7f:tool==2?.75f:.65f;
        public bool Begin(int tool,Action contact)
        {
            if(Busy)return false;
            Tool=tool;duration=Duration(tool);elapsed=0;contacted=false;impact=contact;Busy=true;return true;
        }
        public void Tick(float delta)
        {
            if(!Busy)return;
            elapsed+=Mathf.Max(0,delta);
            if(!contacted&&elapsed>=duration*.46f){contacted=true;var action=impact;impact=null;action?.Invoke();}
            if(elapsed>=duration){Busy=false;impact=null;}
        }
        public void Cancel(){Busy=false;impact=null;}
    }
}
