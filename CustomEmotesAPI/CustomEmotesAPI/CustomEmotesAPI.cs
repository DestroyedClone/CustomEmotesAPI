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
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin("com.weliveinasociety.CustomEmotesAPI", "Custom Emotes API", VERSION)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency("PrefabAPI", "ResourcesAPI", "NetworkingAPI")]
    public class CustomEmotesAPI : BaseUnityPlugin
    {
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
        public const string VERSION = "1.0.0";
        internal static float Actual_MSX = 69;
        public static CustomEmotesAPI instance;
        public void Awake()
        {
            instance = this;
            DebugClass.SetLogger(base.Logger);
            Settings.RunAll();
            Register.Init();
            AnimationReplacements.RunAll();
            float WhosSteveJobs = 69420;
            if (Settings.DontTouchThis.Value < 101)
            {
                WhosSteveJobs = Settings.DontTouchThis.Value;
            }
            On.RoR2.SceneCatalog.OnActiveSceneChanged += (orig, self, scene) =>
            {
                orig(self, scene);
                AkSoundEngine.SetRTPCValue("Volume_MSX", Actual_MSX);
                foreach (var item in BoneMapper.animClips)
                {
                    item.Value.syncTimer = 0f;
                    item.Value.syncPlayerCount = 0;
                }
                if (scene.name == "title" && WhosSteveJobs < 101)
                {
                    AkSoundEngine.SetRTPCValue("Volume_MSX", WhosSteveJobs);
                    Actual_MSX = WhosSteveJobs;
                    WhosSteveJobs = 69420;
                }
                BoneMapper.allMappers.Clear();
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
                    if (self.hasEffectiveAuthority && self.GetFieldValue<CharacterBody>("body") && self.GetFieldValue<InputBankTest>("bodyInputs") && self.GetFieldValue<InputBankTest>("bodyInputs").ping.justPressed)
                    {
                        self.GetFieldValue<PingerController>("pingerController").AttemptPing(new Ray(self.GetFieldValue<InputBankTest>("bodyInputs").aimOrigin, self.GetFieldValue<InputBankTest>("bodyInputs").aimDirection), self.GetFieldValue<CharacterBody>("body").gameObject);
                    }
                }
            };
        }
        public static void AddCustomAnimation(AnimationClip animationClip, bool looping, string _wwiseEventName = "", string _wwiseStopEvent = "", HumanBodyBones[] rootBonesToIgnore = null, HumanBodyBones[] soloBonesToIgnore = null, AnimationClip secondaryAnimation = null, bool dimWhenClose = false, bool stopWhenMove = false, bool stopWhenAttack = false, bool visible = true)
        {
            if (BoneMapper.animClips.ContainsKey(animationClip.name))
            {
                //DebugClass.Log($"Error #1: [{name}] is already defined as a custom emote but is trying to be added. Skipping");
                return;
            }
            if (rootBonesToIgnore == null)
                rootBonesToIgnore = new HumanBodyBones[0];
            if (soloBonesToIgnore == null)
                soloBonesToIgnore = new HumanBodyBones[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClip, looping, _wwiseEventName, _wwiseStopEvent, rootBonesToIgnore, soloBonesToIgnore, secondaryAnimation, dimWhenClose, stopWhenMove, stopWhenAttack, visible);
            allClipNames.Add(animationClip.name);
            BoneMapper.animClips.Add(animationClip.name, clip);
        }

        public static void AddCustomAnimation(Animator animator, bool[] looping, string[] _wwiseEventName = null, string[] _wwiseStopEvent = null, bool visible = true)
        {
            for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                AddCustomAnimation(animator.runtimeAnimatorController.animationClips[i], looping[i], _wwiseEventName[i], _wwiseStopEvent[i], visible: visible);
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
    }
}