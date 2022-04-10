﻿using BepInEx;
using R2API.Utils;
using RoR2;
using R2API;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using RoR2.UI;
using RiskOfOptions;
using UnityEngine.Rendering.PostProcessing;
using R2API.Networking;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using System.Globalization;
using BepInEx.Configuration;

namespace EmotesAPI
{
    [BepInDependency("com.gemumoddo.MoistureUpset", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin("com.weliveinasociety.CustomEmotesAPI", "Custom Emotes API", VERSION)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency("PrefabAPI", "ResourcesAPI", "NetworkingAPI")]
    public class CustomEmotesAPI : BaseUnityPlugin
    {
        public struct NameTokenWithSprite
        {
            public string nameToken;
            public Sprite sprite;
        }
        public static List<NameTokenWithSprite> nameTokenSpritePairs = new List<NameTokenWithSprite>();
        public static bool CreateNameTokenSpritePair(string nameToken, Sprite sprite)
        {
            NameTokenWithSprite temp = new NameTokenWithSprite();
            temp.nameToken = nameToken;
            temp.sprite = sprite;
            if (nameTokenSpritePairs.Contains(temp))
            {
                return false;
            }
            nameTokenSpritePairs.Add(temp);
            return true;
        }
        void CreateBaseNameTokenPairs()
        {
            CreateNameTokenSpritePair("CAPTAIN_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/captain.png"));
            CreateNameTokenSpritePair("COMMANDO_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/commando.png"));
            CreateNameTokenSpritePair("MERC_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/merc.png"));
            CreateNameTokenSpritePair("ENGI_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/engi.png"));
            CreateNameTokenSpritePair("HUNTRESS_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/huntress.png"));
            CreateNameTokenSpritePair("MAGE_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/artificer.png"));
            CreateNameTokenSpritePair("TOOLBOT_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/mult.png"));
            CreateNameTokenSpritePair("TREEBOT_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/rex.png"));
            CreateNameTokenSpritePair("LOADER_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/loader.png"));
            CreateNameTokenSpritePair("CROCO_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/acrid.png"));
            CreateNameTokenSpritePair("BANDIT2_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/bandit.png"));
            CreateNameTokenSpritePair("VOIDSURVIVOR_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/voidfiend.png"));
            CreateNameTokenSpritePair("RAILGUNNER_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/railgunner.png"));
            //CreateNameTokenSpritePair("HERETIC_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/heretic.png"));
        }
        internal static List<string> allClipNames = new List<string>();
        internal static void LoadResource(string resource)
        {
            Assets.AddBundle($"{resource}");
        }
        internal static bool GetKey(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKey(entry.Value.MainKey);
        }
        internal static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
        public const string VERSION = "1.1.0";
        internal static float Actual_MSX = 69;
        public static CustomEmotesAPI instance;
        public void Awake()
        {
            instance = this;
            DebugClass.SetLogger(base.Logger);
            CustomEmotesAPI.LoadResource("customemotespackage");
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.gemumoddo.MoistureUpset"))
            {
                CustomEmotesAPI.LoadResource("moisture_animationreplacements"); // I don't remember what's in here that makes importing emotes work, don't @ me
            }
            Settings.RunAll();
            Register.Init();
            AnimationReplacements.RunAll();
            float WhosSteveJobs = 69420;
            CreateBaseNameTokenPairs();
            if (Settings.DontTouchThis.Value < 101)
            {
                WhosSteveJobs = Settings.DontTouchThis.Value;
            }
            On.RoR2.SceneCatalog.OnActiveSceneChanged += (orig, self, scene) =>
            {
                orig(self, scene);
                if (allClipNames != null)
                {
                    ScrollManager.SetupButtons(allClipNames);
                }
                AkSoundEngine.SetRTPCValue("Volume_MSX", Actual_MSX);
                for (int i = 0; i < CustomAnimationClip.syncPlayerCount.Count; i++)
                {
                    CustomAnimationClip.syncTimer[i] = 0;
                    CustomAnimationClip.syncPlayerCount[i] = 0;
                }
                if (scene.name == "title" && WhosSteveJobs < 101)
                {
                    AkSoundEngine.SetRTPCValue("Volume_MSX", WhosSteveJobs);
                    Actual_MSX = WhosSteveJobs;
                    WhosSteveJobs = 69420;
                }
                BoneMapper.allMappers.Clear();
                localMapper = null;
            };
            On.RoR2.AudioManager.VolumeConVar.SetString += (orig, self, newValue) =>
            {
                orig(self, newValue);
                //Volume_MSX
                if (self.GetFieldValue<string>("rtpcName") == "Volume_MSX" && WhosSteveJobs > 100)
                {
                    Actual_MSX = float.Parse(newValue, CultureInfo.InvariantCulture);
                    BoneMapper.Current_MSX = Actual_MSX;
                    Settings.DontTouchThis.Value = float.Parse(newValue, CultureInfo.InvariantCulture);
                }
            };
            On.RoR2.PlayerCharacterMasterController.FixedUpdate += (orig, self) =>
            {
                if (self.hasEffectiveAuthority && self.GetFieldValue<InputBankTest>("bodyInputs"))
                {
                    bool newState = false;
                    bool newState2 = false;
                    bool newState3 = false;
                    bool newState4 = false;
                    bool newState5 = false;
                    bool newState6 = false;
                    bool newState7 = false;
                    bool newState8 = false;
                    bool newState9 = false;
                    LocalUser localUser;
                    Rewired.Player player;
                    CameraRigController cameraRigController;
                    bool doIt = false;
                    if (!self.networkUser)
                    {
                        localUser = null;
                        player = null;
                        cameraRigController = null;
                        doIt = false;
                    }
                    else
                    {
                        localUser = self.networkUser.localUser;
                        player = self.networkUser.inputPlayer;
                        cameraRigController = self.networkUser.cameraRigController;
                        doIt = localUser != null && player != null && cameraRigController && !localUser.isUIFocused && cameraRigController.isControlAllowed;
                    }
                    if (doIt)
                    {
                        bool flag = self.GetFieldValue<CharacterBody>("body").isSprinting;
                        if (self.GetFieldValue<bool>("sprintInputPressReceived"))
                        {
                            self.SetFieldValue("sprintInputPressReceived", false);
                            flag = !flag;
                        }
                        if (flag)
                        {
                            Vector3 aimDirection = self.GetFieldValue<InputBankTest>("bodyInputs").aimDirection;
                            aimDirection.y = 0f;
                            aimDirection.Normalize();
                            Vector3 moveVector = self.GetFieldValue<InputBankTest>("bodyInputs").moveVector;
                            moveVector.y = 0f;
                            moveVector.Normalize();
                            if ((self.GetFieldValue<CharacterBody>("body").bodyFlags & CharacterBody.BodyFlags.SprintAnyDirection) == CharacterBody.BodyFlags.None && Vector3.Dot(aimDirection, moveVector) < self.GetFieldValue<float>("sprintMinAimMoveDot"))
                            {
                                flag = false;
                            }
                        }
                        newState = player.GetButton(7) && !CustomEmotesAPI.GetKey(Settings.EmoteWheel); //left click
                        newState2 = player.GetButton(8) && !CustomEmotesAPI.GetKey(Settings.EmoteWheel); //right click
                        newState3 = player.GetButton(9);
                        newState4 = player.GetButton(10);
                        newState5 = player.GetButton(5);
                        newState6 = player.GetButton(4);
                        newState7 = flag;
                        newState8 = player.GetButton(6);
                        newState9 = player.GetButton(28);
                    }
                    BoneMapper.attacking = newState || newState2 || newState3 || newState4;
                    BoneMapper.moving = self.GetFieldValue<InputBankTest>("bodyInputs").moveVector != Vector3.zero || player.GetButton(4);
                    self.GetFieldValue<InputBankTest>("bodyInputs").skill1.PushState(newState);
                    self.GetFieldValue<InputBankTest>("bodyInputs").skill2.PushState(newState2);
                    self.GetFieldValue<InputBankTest>("bodyInputs").skill3.PushState(newState3);
                    self.GetFieldValue<InputBankTest>("bodyInputs").skill4.PushState(newState4);
                    self.GetFieldValue<InputBankTest>("bodyInputs").interact.PushState(newState5);
                    self.GetFieldValue<InputBankTest>("bodyInputs").jump.PushState(newState6);
                    self.GetFieldValue<InputBankTest>("bodyInputs").sprint.PushState(newState7);
                    self.GetFieldValue<InputBankTest>("bodyInputs").activateEquipment.PushState(newState8);
                    self.GetFieldValue<InputBankTest>("bodyInputs").ping.PushState(newState9);
                    self.InvokeMethod("CheckPinging");
                }
            };
        }
        public static void AddNonAnimatingEmote(string emoteName, bool visible = true)
        {
            if (visible)
                allClipNames.Add(emoteName);
            BoneMapper.animClips.Add(emoteName, null);
        }
        public static void AddCustomAnimation(AnimationClip animationClip, bool looping, string _wwiseEventName = "", string _wwiseStopEvent = "", HumanBodyBones[] rootBonesToIgnore = null, HumanBodyBones[] soloBonesToIgnore = null, AnimationClip secondaryAnimation = null, bool dimWhenClose = false, bool stopWhenMove = false, bool stopWhenAttack = false, bool visible = true, bool syncAnim = false, bool syncAudio = false)
        {
            if (BoneMapper.animClips.ContainsKey(animationClip.name))
            {
                Debug.Log($"EmotesError: [{animationClip.name}] is already defined as a custom emote but is trying to be added. Skipping");
                return;
            }
            if (!animationClip.isHumanMotion)
            {
                Debug.Log($"EmotesError: [{animationClip.name}] is not a humanoid animation!");
                return;
            }
            if (rootBonesToIgnore == null)
                rootBonesToIgnore = new HumanBodyBones[0];
            if (soloBonesToIgnore == null)
                soloBonesToIgnore = new HumanBodyBones[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClip, looping, _wwiseEventName, _wwiseStopEvent, rootBonesToIgnore, soloBonesToIgnore, secondaryAnimation, dimWhenClose, stopWhenMove, stopWhenAttack, visible, syncAnim, syncAudio);
            if (visible)
                allClipNames.Add(animationClip.name);
            BoneMapper.animClips.Add(animationClip.name, clip);
        }

        internal static void AddCustomAnimation(Animator animator, bool[] looping, string[] _wwiseEventName = null, string[] _wwiseStopEvent = null, bool[] visible = null, bool[] syncAnim = null, bool[] syncAudio = null)
        {
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                AddCustomAnimation(animator.runtimeAnimatorController.animationClips[i], looping[i], _wwiseEventName[i], _wwiseStopEvent[i], visible: visible[i], syncAnim: syncAnim[i], syncAudio: syncAudio[i]);
            }
        }

        public static void ImportArmature(GameObject bodyPrefab, GameObject rigToAnimate, int meshPos = 0, bool hideMeshes = true)
        {
            if (hideMeshes)
            {
                foreach (var item in rigToAnimate.GetComponentsInChildren<MeshFilter>())
                {
                    item.mesh = null;
                }
                foreach (var item in rigToAnimate.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    item.sharedMesh = null;
                }
            }
            rigToAnimate.GetComponent<Animator>().runtimeAnimatorController = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab")).GetComponent<Animator>().runtimeAnimatorController;
            AnimationReplacements.ApplyAnimationStuff(bodyPrefab, rigToAnimate, meshPos);
        }

        public static void PlayAnimation(string animationName)
        {
            var identity = NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().gameObject.GetComponent<NetworkIdentity>();

            if (!NetworkServer.active)
            {
                new SyncAnimationToServer(identity.netId, animationName).Send(R2API.Networking.NetworkDestination.Server);
            }
            else
            {
                new SyncAnimationToClients(identity.netId, animationName).Send(R2API.Networking.NetworkDestination.Clients);

                GameObject bodyObject = Util.FindNetworkObject(identity.netId);
                bodyObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>().PlayAnim(animationName);
            }
        }
        static BoneMapper localMapper = null;
        public static AnimationClip GetLocalBodyAnimationClip()
        {
            if (!localMapper)
            {
                localMapper = NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().gameObject.GetComponentInChildren<BoneMapper>();
            }
            return localMapper.currentClip.clip;
        }
        public static BoneMapper[] GetAllBoneMappers()
        {
            return BoneMapper.allMappers.ToArray();
        }
        public delegate void AnimationChanged(string newAnimation, BoneMapper mapper);
        public static event AnimationChanged animChanged;
        internal static void Changed(string newAnimation, BoneMapper mapper) //is a neat game made by a developer who endorses nsfw content while calling it a fine game for kids
        {
            if (animChanged != null)
            {
                animChanged(newAnimation, mapper);
            }
        }
    }
}