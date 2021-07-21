using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PranksterScript : ModuleScript
{
    [SerializeField]
    private ParticleSystem Particles;
    [SerializeField]
    private RectTransform Vignette;
    [SerializeField]
    internal RectTransform CosmicOrb;
    [SerializeField]
    internal Material PurpleMat;

    private Queue<Vector2> MousePath = new Queue<Vector2>(new Vector2[] { new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2() });

    private IComet currentComet = null;

    private static readonly CometFactory[] factories = new CometFactory[] { new DaredevilCometFactory(), new CosmicCometFactory(), new PurpleCometFactory() };

    internal Action OnModuleStrikeEvent { get; set; }

    internal Action OnCosmicMouseoverEvent { get; set; }

    private void Start()
    {
        OnModuleStrikeEvent = () => { };
        OnCosmicMouseoverEvent = () => { };
        Vignette.anchorMin = new Vector2(1, 0);
        Vignette.anchorMax = new Vector2(0, 1);
        Vignette.pivot = new Vector2(0.5f, 0.5f);
        CosmicOrb.gameObject.SetActive(false);
        Particles.Stop();
        Get<KMNeedyModule>().Assign(onNeedyActivation: NewComet, onTimerExpired: CometDone, onActivate: () => { PlaySound("Startup"); });
        Get<KMBombInfo>().OnBombSolved += () => { PlaySound("Solved"); };
        GetChild<Canvas>().gameObject.SetActive(false);
    }

    private void CometDone()
    {
        GetChild<Canvas>().gameObject.SetActive(false);

        currentComet.Destroy();
    }

    private void NewComet()
    {
        PlaySound("AlarmSound");
        GetChild<Canvas>().gameObject.SetActive(true);

        currentComet = factories.PickRandom().Generate(Particles, this);
    }

    public override void OnModuleStrike(string moduleId)
    {
        base.OnModuleStrike(moduleId);

        OnModuleStrikeEvent();
    }

    private void FixedUpdate()
    {
        CosmicOrb.transform.localPosition = MousePath.Dequeue();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetChild<Canvas>().transform as RectTransform, Input.mousePosition, GetChild<Canvas>().worldCamera, out pos);
        MousePath.Enqueue(pos);
    }

    public void OnCosmic()
    {
        OnCosmicMouseoverEvent();
    }
}
