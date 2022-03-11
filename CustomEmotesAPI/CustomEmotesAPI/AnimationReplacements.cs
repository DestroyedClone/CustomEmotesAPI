﻿using BepInEx;
using R2API.Utils;
using RoR2;
using R2API;
using R2API.MiscHelpers;
using System.Reflection;
using static R2API.SoundAPI;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;
using RiskOfOptions;
using TMPro;
using R2API.Networking.Interfaces;
using UnityEngine.Animations;
using UnityEngine.UI;
using EmotesAPI;
using UnityEngine.AddressableAssets;

internal static class AnimationReplacements
{
    internal static void RunAll()
    {
        CustomEmotesAPI.LoadResource("customemotespackage");
        CustomEmotesAPI.LoadResource("enfucker");
        ChangeAnims();
        On.RoR2.UI.HUD.Awake += (orig, self) =>
        {
            orig(self);
            GameObject g = GameObject.Instantiate(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/emotewheel.prefab"));
            foreach (var item in g.GetComponentsInChildren<TextMeshProUGUI>())
            {
                item.font = self.mainContainer.transform.Find("MainUIArea").Find("SpringCanvas").Find("UpperLeftCluster").Find("MoneyRoot").Find("ValueText").GetComponent<TextMeshProUGUI>().font;
                item.fontMaterial = self.mainContainer.transform.Find("MainUIArea").Find("SpringCanvas").Find("UpperLeftCluster").Find("MoneyRoot").Find("ValueText").GetComponent<TextMeshProUGUI>().fontMaterial;
                item.fontSharedMaterial = self.mainContainer.transform.Find("MainUIArea").Find("SpringCanvas").Find("UpperLeftCluster").Find("MoneyRoot").Find("ValueText").GetComponent<TextMeshProUGUI>().fontSharedMaterial;
            }
            g.transform.SetParent(self.mainContainer.transform);
            g.transform.localPosition = new Vector3(0, 0, 0);
            var s = g.AddComponent<EmoteWheel>();
            foreach (var item in g.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.name.StartsWith("Emote"))
                {
                    s.gameObjects.Add(item.gameObject);
                }
                if (item.gameObject.name.StartsWith("MousePos"))
                {
                    s.text = item.gameObject;
                }
                if (item.gameObject.name == "Center")
                {
                    s.joy = item.gameObject.GetComponent<Image>();
                }
            }
        };
    }
    internal static bool setup = false;
    internal static void ChangeAnims()
    {
        On.RoR2.SurvivorCatalog.Init += (orig) =>
        {
            orig();
            if (!setup)
            {
                setup = true;
                ApplyAnimationStuff(RoR2Content.Survivors.Croco, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/acrid.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Mage, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/artificer.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Captain, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/captain.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Engi, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/engi.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Loader, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/loader.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Merc, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/merc.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Toolbot, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/mult.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Treebot, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/rex.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Commando, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Huntress, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/huntressBetterMaybeFixed.prefab");
                ApplyAnimationStuff(RoR2Content.Survivors.Bandit2, "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/bandit.prefab");
                ApplyAnimationStuff(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Heretic/HereticBody.prefab").WaitForCompletion(), "@CustomEmotesAPI_customemotespackage:assets/animationreplacements/heretic.prefab", 3);
            }
            foreach (var item in SurvivorCatalog.allSurvivorDefs)
            {
                if (item.bodyPrefab.name == "EnforcerBody")
                {
                    var skele = Assets.Load<GameObject>("@CustomEmotesAPI_enfucker:assets/fbx/enfucker/enfucker.prefab");
                    skele.GetComponent<Animator>().runtimeAnimatorController = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab")).GetComponent<Animator>().runtimeAnimatorController;
                    CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                }
                //DebugClass.Log($"---{item.bodyPrefab.name}");
            }
            //bodyPrefab = survivorDef.displayPrefab;
            //animcontroller = Resources.Load<GameObject>(resource);
            //animcontroller.transform.parent = bodyPrefab.GetComponent<ModelLocator>().modelTransform;
            //animcontroller.transform.localPosition = Vector3.zero;
            //animcontroller.transform.localEulerAngles = Vector3.zero;
            //smr1 = animcontroller.GetComponentInChildren<SkinnedMeshRenderer>();
            //smr2 = bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<SkinnedMeshRenderer>();
            //test = animcontroller.AddComponent<BoneMapper>();
            //test.smr1 = smr1;
            //test.smr2 = smr2;
            //test.a1 = bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<Animator>();
            //test.a2 = animcontroller.GetComponentInChildren<Animator>();
            //test.h = bodyPrefab.GetComponentInChildren<HealthComponent>();
        };
    }
    internal static void ApplyAnimationStuff(SurvivorDef index, string resource, int pos = 0)
    {
        var survivorDef = index;
        ApplyAnimationStuff(survivorDef.bodyPrefab, resource, pos);
    }
    internal static void ApplyAnimationStuff(GameObject bodyPrefab, string resource, int pos = 0)
    {
        GameObject animcontroller = Assets.Load<GameObject>(resource);
        ApplyAnimationStuff(bodyPrefab, animcontroller, pos);
    }
    internal static void ApplyAnimationStuff(GameObject bodyPrefab, GameObject animcontroller, int pos = 0)
    {
        if (!animcontroller.GetComponentInChildren<Animator>().avatar.isHuman)
        {
            DebugClass.Log($"{animcontroller}'s avatar isn't humanoid, please fix it in unity!");
            return;
        }
        animcontroller.transform.parent = bodyPrefab.GetComponent<ModelLocator>().modelTransform;
        animcontroller.transform.localPosition = Vector3.zero;
        animcontroller.transform.localEulerAngles = Vector3.zero;
        animcontroller.transform.localScale = Vector3.one;
        SkinnedMeshRenderer smr1 = animcontroller.GetComponentsInChildren<SkinnedMeshRenderer>()[pos];
        SkinnedMeshRenderer smr2 = bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>()[pos];
        var test = animcontroller.AddComponent<BoneMapper>();
        test.smr1 = smr1;
        test.smr2 = smr2;
        for (int i = 0; i < smr1.bones.Length; i++)
        {
            if (smr1.bones[i].name != smr2.bones[i].name)
            {
                DebugClass.Log($"Fixing {bodyPrefab.name} bone order for emotes");
                List<Transform> trans = new List<Transform>();
                foreach (var item in smr2.bones)
                {
                    foreach (var item2 in smr1.bones)
                    {
                        if (item.name == item2.name)
                        {
                            trans.Add(item2);
                        }
                    }
                }
                smr1.bones = trans.ToArray();
                DebugClass.Log($"Done");
                break;
            }
        }
        test.a1 = bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<Animator>();
        test.a2 = animcontroller.GetComponentInChildren<Animator>();

        test.h = bodyPrefab.GetComponentInChildren<HealthComponent>();
        test.model = bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
    }
}
internal class CustomAnimationClip : MonoBehaviour
{
    internal AnimationClip clip, secondaryClip; //DONT SUPPORT MULTI CLIP ANIMATIONS TO SYNC
    internal bool looping;
    internal string wwiseEvent;
    //internal bool syncronizeAnimation;
    internal float syncTimer = 0f;
    internal int syncPlayerCount = 0;
    internal List<HumanBodyBones> soloIgnoredBones;
    internal List<HumanBodyBones> rootIgnoredBones;
    internal bool dimAudioWhenClose;
    internal bool stopOnAttack;
    internal bool stopOnMove;
    internal bool visibility;
    internal CustomAnimationClip(AnimationClip _clip, bool _loop/*, bool _shouldSyncronize = false*/, string _wwiseEventName = "", string _wwiseStopEvent = "", HumanBodyBones[] rootBonesToIgnore = null, HumanBodyBones[] soloBonesToIgnore = null, AnimationClip _secondaryClip = null, bool dimWhenClose = false, bool stopWhenMove = false, bool stopWhenAttack = false, bool visible = true)
    {
        if (rootBonesToIgnore == null)
            rootBonesToIgnore = new HumanBodyBones[0];
        if (soloBonesToIgnore == null)
            soloBonesToIgnore = new HumanBodyBones[0];
        clip = _clip;
        if (_secondaryClip)
            secondaryClip = _secondaryClip;
        looping = _loop;
        //syncronizeAnimation = _shouldSyncronize;
        dimAudioWhenClose = dimWhenClose;
        stopOnAttack = stopWhenAttack;
        stopOnMove = stopWhenMove;
        visibility = visible;
        //int count = 0;
        //float timer = 0;
        if (_wwiseEventName != "" && _wwiseStopEvent == "")
        {
            //DebugClass.Log($"Error #2: wwiseEventName is declared but wwiseStopEvent isn't skipping sound implementation for [{clip.name}]");
        }
        else if (_wwiseEventName == "" && _wwiseStopEvent != "")
        {
            //DebugClass.Log($"Error #3: wwiseStopEvent is declared but wwiseEventName isn't skipping sound implementation for [{clip.name}]");
        }
        else if (_wwiseEventName != "")
        {
            //if (!_shouldSyncronize)
            //{
            BoneMapper.stopEvents.Add(_wwiseStopEvent);
            //}
            wwiseEvent = _wwiseEventName;
        }
        //DebugClass.Log($"----------checking?");
        if (soloBonesToIgnore.Length != 0)
        {
            //Debug.Log($"---woah it's here-------{ignoredBones[0]}");
            soloIgnoredBones = new List<HumanBodyBones>(soloBonesToIgnore);
        }
        else
        {
            soloIgnoredBones = new List<HumanBodyBones>();
        }

        if (rootBonesToIgnore.Length != 0)
        {
            //Debug.Log($"---woah it's here-------{ignoredBones[0]}");
            rootIgnoredBones = new List<HumanBodyBones>(rootBonesToIgnore);
        }
        else
        {
            rootIgnoredBones = new List<HumanBodyBones>();
        }
    }
}
internal class BoneMapper : MonoBehaviour
{
    public static List<string> stopEvents = new List<string>();
    public SkinnedMeshRenderer smr1, smr2;
    public Animator a1, a2;
    public HealthComponent h;
    public List<BonePair> pairs = new List<BonePair>();
    public float timer = 0;
    public static float caramellCount = 0;
    public static float caramellTimer = 0;
    public GameObject model;
    List<string> ignore = new List<string>();
    bool twopart = false;
    public static Dictionary<string, CustomAnimationClip> animClips = new Dictionary<string, CustomAnimationClip>();
    public CustomAnimationClip currentClip = null;
    internal static float Current_MSX = 69;
    internal static List<BoneMapper> allMappers = new List<BoneMapper>();
    private bool local = false;
    internal static bool moving = false;
    internal static bool attacking = false;
    public void PlayAnim(string s)
    {
        a2.enabled = true;
        List<string> dontAnimateUs = new List<string>();
        if (s != "none")
        {
            if (!animClips.ContainsKey(s))
            {
                DebugClass.Log($"No animation bound to the name [{s}]");
                return;
            }
            currentClip = animClips[s];
            foreach (var item in currentClip.soloIgnoredBones)
            {
                if (a2.GetBoneTransform(item))
                    dontAnimateUs.Add(a2.GetBoneTransform(item).name);
            }
            foreach (var item in currentClip.rootIgnoredBones)
            {
                if (a2.GetBoneTransform(item))
                    dontAnimateUs.Add(a2.GetBoneTransform(item).name);
                foreach (var bone in a2.GetBoneTransform(item).GetComponentsInChildren<Transform>())
                {
                    dontAnimateUs.Add(bone.name);
                }
            }
        }
        for (int i = 0; i < smr2.bones.Length; i++)
        {
            try
            {
                if (smr2.bones[i].gameObject.GetComponent<ParentConstraint>() && !dontAnimateUs.Contains(smr2.bones[i].name))
                {
                    smr2.bones[i].gameObject.GetComponent<ParentConstraint>().constraintActive = true;
                }
                else if (dontAnimateUs.Contains(smr2.bones[i].name))
                {
                    smr2.bones[i].gameObject.GetComponent<ParentConstraint>().constraintActive = false;
                }
            }
            catch (Exception)
            {
            }
        }
        foreach (var item in stopEvents)
        {
            AkSoundEngine.PostEvent(item, gameObject);
        }
        if (s == "none")
        {
            a2.Play("none", -1, 0f);
            twopart = false;
            currentClip = null;
            return;
        }
        if (currentClip.wwiseEvent != "")
        {
            AkSoundEngine.PostEvent(currentClip.wwiseEvent, gameObject);
        }
        AnimatorOverrideController animController = new AnimatorOverrideController(a2.runtimeAnimatorController);
        if (currentClip.secondaryClip)
        {
            animController["Dab"] = currentClip.clip;
            animController["nobones"] = currentClip.secondaryClip;
            a2.runtimeAnimatorController = animController;
            a2.Play("PoopToLoop", -1, 0f);
        }
        else if (currentClip.looping)
        {
            animController["Floss"] = currentClip.clip;
            a2.runtimeAnimatorController = animController;
            a2.Play("Loop", -1, currentClip.syncTimer / currentClip.clip.length);
        }
        else
        {
            animController["Default Dance"] = currentClip.clip;
            a2.runtimeAnimatorController = animController;
            a2.Play("Poop", -1, currentClip.syncTimer / currentClip.clip.length);
        }
        twopart = false;
    }
    void AddIgnore(DynamicBone dynbone, Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            if (!dynbone.m_Exclusions.Contains(t.GetChild(i)))
            {
                ignore.Add(t.GetChild(i).name);
                AddIgnore(dynbone, t.GetChild(i));
            }
        }
    }
    void Start()
    {
        try
        {
            var body = NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody();
            if (body && Vector3.Distance(transform.parent.position, body.transform.position) < 4f)
            {
                local = true;
            }
        }
        catch (Exception)
        {
        }
        allMappers.Add(this);
        foreach (var item in model.GetComponents<DynamicBone>())
        {
            try
            {
                if (!item.m_Exclusions.Contains(item.m_Root))
                {
                    ignore.Add(item.m_Root.name);
                }
                AddIgnore(item, item.m_Root);
            }
            catch (Exception)
            {
            }
        }
        if (model.name.StartsWith("mdlLoader"))
        {
            Transform LClav = model.transform, RClav = model.transform;
            foreach (var item in model.GetComponentsInChildren<Transform>())
            {
                if (item.name == "clavicle.l")
                {
                    LClav = item;
                    ignore.Add(LClav.name);
                }
                if (item.name == "clavicle.r")
                {
                    RClav = item;
                    ignore.Add(RClav.name);
                }
            }
            foreach (var item in LClav.GetComponentsInChildren<Transform>())
            {
                ignore.Add(item.name);
            }
            foreach (var item in RClav.GetComponentsInChildren<Transform>())
            {
                ignore.Add(item.name);
            }
        }
        for (int i = 0; i < smr2.bones.Length; i++)
        {
            try
            {
                if (!ignore.Contains(smr2.bones[i].name))
                {
                    var s = new ConstraintSource();
                    s.sourceTransform = smr1.bones[i];
                    s.weight = 1;
                    smr2.bones[i].gameObject.AddComponent<ParentConstraint>().AddSource(s);
                }
            }
            catch (Exception)
            {
            }
        }
    }
    void Update()
    {
        if (local)
        {
            float closestDimmingSource = 20f;
            foreach (var item in allMappers)
            {
                try
                {
                    if (item && item.a2.enabled && item.currentClip.dimAudioWhenClose)
                    {
                        if (Vector3.Distance(item.transform.parent.position, transform.position) < closestDimmingSource)
                        {
                            closestDimmingSource = Vector3.Distance(item.transform.position, transform.position);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            if (closestDimmingSource < 20f)
            {
                Current_MSX = Mathf.Lerp(Current_MSX, (closestDimmingSource / 20f) * CustomEmotesAPI.Actual_MSX, Time.deltaTime * 3);
                AkSoundEngine.SetRTPCValue("Volume_MSX", Current_MSX);
            }
            else if (Current_MSX != CustomEmotesAPI.Actual_MSX)
            {
                Current_MSX = Mathf.Lerp(Current_MSX, CustomEmotesAPI.Actual_MSX, Time.deltaTime * 3);
                AkSoundEngine.SetRTPCValue("Volume_MSX", Current_MSX);
            }
            try
            {
                if (attacking && currentClip.stopOnAttack)
                {
                    var identity = NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().gameObject.GetComponent<NetworkIdentity>();

                    if (!NetworkServer.active)
                    {
                        new SyncAnimationToServer(identity.netId, "none").Send(R2API.Networking.NetworkDestination.Server);
                    }
                    else
                    {
                        new SyncAnimationToClients(identity.netId, "none").Send(R2API.Networking.NetworkDestination.Clients);
                        GameObject bodyObject = Util.FindNetworkObject(identity.netId);
                        bodyObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>().PlayAnim("none");
                    }
                }
                if (moving && currentClip.stopOnMove)
                {
                    var identity = NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().gameObject.GetComponent<NetworkIdentity>();

                    if (!NetworkServer.active)
                    {
                        new SyncAnimationToServer(identity.netId, "none").Send(R2API.Networking.NetworkDestination.Server);
                    }
                    else
                    {
                        new SyncAnimationToClients(identity.netId, "none").Send(R2API.Networking.NetworkDestination.Clients);
                        GameObject bodyObject = Util.FindNetworkObject(identity.netId);
                        bodyObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>().PlayAnim("none");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        foreach (var item in animClips)
        {
            if (item.Value.syncPlayerCount != 0)
            {
                item.Value.syncTimer += Time.deltaTime;
                item.Value.syncTimer = item.Value.syncTimer % item.Value.clip.length;
                //DebugClass.Log($"----------adding to {item.Key}   [{item.Value.syncTimer}]");
            }
        }
        if (a2.GetCurrentAnimatorStateInfo(0).IsName("none"))
        {
            if (!twopart)
            {
                twopart = true;
            }
            else
            {
                if (a2.enabled)
                {
                    AkSoundEngine.PostEvent("StopEmotes", gameObject);
                    AkSoundEngine.PostEvent("StopCaramell", gameObject);
                    for (int i = 0; i < smr2.bones.Length; i++)
                    {
                        try
                        {
                            if (smr2.bones[i].gameObject.GetComponent<ParentConstraint>())
                            {
                                smr2.bones[i].gameObject.GetComponent<ParentConstraint>().constraintActive = false;
                            }
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                }
                a1.enabled = true;
                a2.enabled = false;
            }
        }
        else
        {
            //a1.enabled = false;
            twopart = false;
        }
        if (h.health <= 0)
        {
            AkSoundEngine.PostEvent("StopEmotes", gameObject);
            AkSoundEngine.PostEvent("StopCaramell", gameObject);
            for (int i = 0; i < smr2.bones.Length; i++)
            {
                try
                {
                    if (smr2.bones[i].gameObject.GetComponent<ParentConstraint>())
                    {
                        smr2.bones[i].gameObject.GetComponent<ParentConstraint>().constraintActive = false;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
            GameObject.Destroy(gameObject);
        }
    }
}
internal class BonePair
{
    public Transform original, newiginal;
    public BonePair(Transform n, Transform o)
    {
        newiginal = n;
        original = o;
    }

    public void test()
    {

    }
}

internal static class Pain
{
    internal static Transform FindBone(SkinnedMeshRenderer mr, string name)
    {
        foreach (var item in mr.bones)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        DebugClass.Log($"couldnt find bone [{name}]");
        return mr.bones[0];
    }

    internal static Transform FindBone(List<Transform> bones, string name)
    {
        foreach (var item in bones)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        DebugClass.Log($"couldnt find bone [{name}]");
        return bones[0];
    }
}