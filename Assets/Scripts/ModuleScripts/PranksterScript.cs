using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    internal RectTransform TPCursor;
    [SerializeField]
    internal Material PurpleMat;

    private Queue<Vector2> MousePath = new Queue<Vector2>(new Vector2[] { new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2(), new Vector2() });

    private IComet currentComet = null;

    private static readonly CometFactory[] factories = new CometFactory[] { new DaredevilCometFactory(), new CosmicCometFactory(), new PurpleCometFactory() };
    
    internal Action OnModuleStrikeEvent { get; set; }

    internal bool Dangerous { get; set; }

    private bool TPMove { get; set; }

    private void Start()
    {
        OnModuleStrikeEvent = () => { };
        Dangerous = false;
        TPMove = true;
        Vignette.anchorMin = new Vector2(1, 0);
        Vignette.anchorMax = new Vector2(0, 1);
        Vignette.pivot = new Vector2(0.5f, 0.5f);
        CosmicOrb.gameObject.SetActive(false);
        TPCursor.gameObject.SetActive(false);
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
        if (TwitchPlaysActive)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetChild<Canvas>().transform as RectTransform, TPCursor.position, GetChild<Canvas>().worldCamera, out pos);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetChild<Canvas>().transform as RectTransform, Input.mousePosition, GetChild<Canvas>().worldCamera, out pos);
        MousePath.Enqueue(pos);
    }

    public void OnCosmic()
    {
        if(Dangerous)
        {
            Strike("You ran into the cosmic shadow!");
            currentComet.Destroy();
        }
    }

    //twitch plays
    IEnumerator MoveCursor(Vector3 target)
    {
        while (!TPMove) yield return null;
        TPMove = false;
        Vector3 startPos = TPCursor.localPosition;
        float t = 0f;
        while (!CursorInPosition(target))
        {
            TPCursor.localPosition = Vector3.Lerp(startPos, target, t);
            t += Time.deltaTime * 3f;
            yield return null;
        }
        TPMove = true;
    }

    bool CursorInPosition(Vector3 target)
    {
        if (!(Vector3.Distance(TPCursor.localPosition, target) < 0.0001f))
            return false;
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} show [Highlights this module] | !{0} move <u/d/l/r> [Moves the cursor in the specified direction] | Moves may be chained, for ex: !{0} move udlr | On Twitch Plays a fake cursor will be placed on the screen until the module deactivates and the speed of the shadow will be slower";
    internal bool TwitchPlaysActive;
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*move\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                char[] dirs = { 'u', 'd', 'l', 'r' };
                for (int i = 0; i < parameters[1].Length; i++)
                {
                    if (!dirs.Contains(parameters[1].ToLower()[i]))
                    {
                        yield return "sendtochaterror!f The specified direction '" + parameters[1][i] + "' is invalid!";
                        yield break;
                    }
                }
                for (int i = 0; i < parameters[1].Length; i++)
                {
                    while (!TPMove) { yield return "trycancel Halted movement of the cursor due to a request to cancel!"; }
                    Vector3 temp;
                    if (parameters[1].ToLower()[i].Equals('u'))
                    {
                        temp = new Vector3(TPCursor.localPosition.x, TPCursor.localPosition.y + 100);
                        if (temp.y > 540f)
                            temp = new Vector3(TPCursor.localPosition.x, 540f);
                    }
                    else if (parameters[1].ToLower()[i].Equals('d'))
                    {
                        temp = new Vector3(TPCursor.localPosition.x, TPCursor.localPosition.y - 100);
                        if (temp.y < -540f)
                            temp = new Vector3(TPCursor.localPosition.x, -540f);
                    }
                    else if (parameters[1].ToLower()[i].Equals('l'))
                    {
                        temp = new Vector3(TPCursor.localPosition.x - 100, TPCursor.localPosition.y);
                        if ((temp.x - 50) < -960f)
                            temp = new Vector3(-960f, TPCursor.localPosition.y);
                    }
                    else
                    {
                        temp = new Vector3(TPCursor.localPosition.x + 100, TPCursor.localPosition.y);
                        if ((temp.x + 50) > 960f)
                            temp = new Vector3(960f, TPCursor.localPosition.y);
                    }
                    StartCoroutine(MoveCursor(temp));
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify at least one direction to move in!";
            }
        }
    }
}
