using Unity.VisualScripting;
using UnityEngine;

public class ParticleManager
{
    public static ParticleSystem SpawnParticle(ParticleSystem particle, Vector3 pos, Transform parent, float size = 1f)
    {
        var p = Object.Instantiate(particle, pos, Quaternion.identity, parent);
        p.AddComponent<ParticleDestroy>();
        p.transform.localScale *= size;
        return p;
    }

    public static ParticleSystem SpawnParticle(ParticleSystem particle, Vector3 pos, float size = 1f)
    {
        return SpawnParticle(particle, pos, null, size);
    }

    public static ParticleSystem SpawnParticle(ParticleSystem particle, Transform transform, float size = 1f)
    {
        return SpawnParticle(particle, transform.position, transform, size);
    }
}