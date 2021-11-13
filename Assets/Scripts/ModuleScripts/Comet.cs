using System;
using System.Collections;
using UnityEngine;
using System.Linq;

internal interface IComet
{
    void Destroy();
}

internal class DaredevilComet : IComet
{
    private ParticleSystem particles;

    private readonly PranksterScript script;

    private readonly Action action;

    public DaredevilComet(ParticleSystem particles, PranksterScript script)
    {
        this.particles = particles;
        this.script = script;
        action = () => { for(int i = 0; i < KeepCoding.Game.Mission.GeneratorSetting.NumStrikes; i++) script.Get<KMGameCommands>().CauseStrike("A Daredevil Comet was active!"); };
        script.OnModuleStrikeEvent += action;
    }

    public void Destroy()
    {
        script.OnModuleStrikeEvent -= action;
        particles.Stop();
        script.Log("The Daredevil Comet has gone. You are safer for now.");

        script.Solve();
    }
}

internal class CosmicComet : IComet
{
    private ParticleSystem particles;

    private readonly PranksterScript script;

    public CosmicComet(ParticleSystem particles, PranksterScript script)
    {
        this.particles = particles;
        this.script = script;

        script.CosmicOrb.gameObject.SetActive(true);
        if (script.TwitchPlaysActive)
        {
            script.TPCursor.gameObject.SetActive(true);
            script.TPCursor.localPosition = new Vector3(0, 0);
        }
        script.StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        CanvasRenderer r = script.CosmicOrb.GetComponent<CanvasRenderer>();
        Color c = r.GetColor();
        int safeZone = 5;
        if (script.TwitchPlaysActive)
            safeZone = 20;
        for(int i = 0; i < safeZone; i++)
        {
            c.a = 1f;
            r.SetColor(c);
            yield return new WaitForSeconds(0.2f);
            c.a = .5f;
            r.SetColor(c);
            yield return new WaitForSeconds(0.2f);
        }
        c.a = 1f;
        r.SetColor(c);

        script.Dangerous = true;
    }

    public void Destroy()
    {
        particles.Stop();
        script.Log("The Cosmic Comet has gone. You are safer for now.");

        script.CosmicOrb.gameObject.SetActive(false);
        script.Dangerous = false;

        script.Solve();
    }
}

internal class PurpleComet : IComet
{
    private ParticleSystem particles;

    private readonly PranksterScript script;

    private int collected = 0;
    private readonly int needed = 0;

    public PurpleComet(ParticleSystem particles, PranksterScript script)
    {
        this.particles = particles;
        this.script = script;

        KMBombModule[] modules = UnityEngine.Object.FindObjectsOfType<KMBombModule>().Where(t => t != null).ToArray().Shuffle().Take(3).ToArray();
        needed = modules.Count();

        if(needed == 0)
            script.Log("Selected 0 modules.");
        else
            script.Log("Selected {0} module{2} being: {1}", needed, modules.Select(m => m.ModuleDisplayName).Join(", "), needed == 1 ? ", that" : "s, those");

        for(int i = 0; i < modules.Length; i++)
        {
            KMBombModule mod = modules[i];

            mod.GetComponent<KMSelectable>().Highlight.GetComponent<MeshRenderer>().enabled = true;
            Material m = mod.GetComponent<KMSelectable>().Highlight.GetComponent<MeshRenderer>().material;
            mod.GetComponent<KMSelectable>().Highlight.GetComponent<MeshRenderer>().material = script.PurpleMat;
            Action a = mod.GetComponent<KMSelectable>().OnHighlight;
            mod.GetComponent<KMSelectable>().OnHighlight += () =>
            {
                collected++;
                mod.GetComponent<KMSelectable>().Highlight.GetComponent<MeshRenderer>().enabled = false;
                mod.GetComponent<KMSelectable>().Highlight.GetComponent<MeshRenderer>().material = m;
                mod.GetComponent<KMSelectable>().OnHighlight = a;
            };
        }
    }

    public void Destroy()
    {
        particles.Stop();

        if(collected >= needed)
        {
            script.Log("The Purple Comet has gone. You are safer for now.");
        }
        else
        {
            script.Strike("You failed to collect all the pink highlights! Strike.");
        }

        KMBombModule[] modules = UnityEngine.Object.FindObjectsOfType<KMBombModule>().Where(t => t != null).ToArray();
        for(int i = 0; i < modules.Length; i++)
        {
            KMBombModule mod = modules[i];

            if(mod.GetComponent<KMSelectable>() == null)
                continue;
            if(mod.GetComponent<KMSelectable>().OnHighlight != null)
                mod.GetComponent<KMSelectable>().OnHighlight();
            if(mod.GetComponent<KMSelectable>().OnHighlightEnded != null)
                mod.GetComponent<KMSelectable>().OnHighlightEnded();
        }

        script.Solve();
    }
}