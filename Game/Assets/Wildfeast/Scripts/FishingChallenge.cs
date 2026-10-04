using UnityEngine;
namespace Wildfeast
{
    // Pure simulation: holding accelerates the catch zone, fish use species-specific motion.
    public sealed class FishingChallenge
    {
        public float Zone {get;private set;}=.5f;
        public float Fish {get;private set;}=.5f;
        public float Progress {get;private set;}=.2f;
        public float Width {get;}
        public bool Tracking=>Mathf.Abs(Zone-Fish)<=Width*.5f;
        float clock,velocity; readonly int species; readonly float duration;
        public FishingChallenge(int species,bool relaxed,float duration=5)
        {this.species=species;this.duration=duration;Width=relaxed?.29f:.21f;}
        public void Tick(float dt,bool hold)
        {
            dt=Mathf.Clamp(dt,0,.05f);clock+=dt;
            velocity=Mathf.MoveTowards(velocity,hold?.55f:-.45f,dt*2.4f);
            Zone=Mathf.Clamp(Zone+velocity*dt,Width/2,1-Width/2);
            Fish=Mathf.Clamp(.5f+Mathf.Sin(clock*(.85f+species*.11f))*.25f+Mathf.Sin(clock*2.15f+species)*.09f,.08f,.92f);
            Progress=Mathf.Clamp01(Progress+dt*(Tracking?1/duration:-.075f));
        }
    }
}
