using UnityEngine;

internal abstract class CometFactory
{
    internal abstract IComet Generate(ParticleSystem particles, PranksterScript script);
}

internal class DaredevilCometFactory : CometFactory
{
    internal override IComet Generate(ParticleSystem particles, PranksterScript script)
    {
        script.Log("A Daredevil Comet has arrived! Be sure not to strike while it is active.");

        DaredevilComet comet = new DaredevilComet(particles, script);

        ParticleSystem.MainModule m = particles.main;
        m.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0.8f));
        particles.Play();

        return comet;
    }
}

internal class CosmicCometFactory : CometFactory
{
    internal override IComet Generate(ParticleSystem particles, PranksterScript script)
    {
        script.Log("A Cosmic Comet has arrived! Be sure to avoid the shadow.");

        CosmicComet comet = new CosmicComet(particles, script);

        ParticleSystem.MainModule m = particles.main;
        m.startColor = new ParticleSystem.MinMaxGradient(new Color(.36f, .2f, .92f, 1f), new Color(.36f, .2f, .92f, 0.8f));
        particles.Play();

        return comet;
    }
}

internal class PurpleCometFactory : CometFactory
{
    internal override IComet Generate(ParticleSystem particles, PranksterScript script)
    {
        script.Log("A Purple Comet has arrived! Be sure to collect all the pink highlights.");

        PurpleComet comet = new PurpleComet(particles, script);

        ParticleSystem.MainModule m = particles.main;
        m.startColor = new ParticleSystem.MinMaxGradient(new Color(.9f, .12f, .94f, 1f), new Color(.9f, .12f, .94f, 0.8f));
        particles.Play();

        return comet;
    }
}