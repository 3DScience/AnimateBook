// <copyright file="RemoteConfigDeps.cs" company="Google Inc.">
// Copyright (C) 2016 Google Inc. All Rights Reserved.
// </copyright>

using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// This file is used to define dependencies, and pass them along to a system
/// which can resolve dependencies.
/// </summary>
[InitializeOnLoad]
public class FirebaseRemoteConfigDeps : AssetPostprocessor
{
    /// <summary>
    /// This is the entry point for "InitializeOnLoad". It will register the
    /// dependencies with the dependency tracking system.
    /// </summary>
    static FirebaseRemoteConfigDeps()
    {
        SetupDeps();
    }

    static void SetupDeps()
    {
#if UNITY_ANDROID
        // Setup the resolver using reflection as the module may not be
        // available at compile time.
        Type playServicesSupport = Google.VersionHandler.FindClass(
            "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
        if (playServicesSupport == null) {
            return;
        }
        object svcSupport = Google.VersionHandler.InvokeStaticMethod(
            playServicesSupport, "CreateInstance",
            new object[] {
                "FirebaseRemoteConfig",
                EditorPrefs.GetString("AndroidSdkRoot"),
                "ProjectSettings"
            });

        Google.VersionHandler.InvokeInstanceMethod(svcSupport, "DependOn", new object[] { "com.google.firebase", "firebase-config", "10+" }, namedArgs: new Dictionary<string, object>() { { "packageIds", new string[] { "extra-google-m2repository", "extra-android-m2repository" } }, { "repositories", null } });
        Google.VersionHandler.InvokeInstanceMethod(svcSupport, "DependOn", new object[] { "com.google.firebase", "firebase-config-unity", "1.1.2" }, namedArgs: new Dictionary<string, object>() { { "packageIds", null }, { "repositories", new string[] { "Assets/Firebase/m2repository" } } });
#elif UNITY_IOS
        Type iosResolver = Google.VersionHandler.FindClass(
            "Google.IOSResolver", "Google.IOSResolver");
        if (iosResolver == null) {
            return;
        }
        Google.VersionHandler.InvokeStaticMethod(iosResolver, "AddPod", new object[] { "Firebase/RemoteConfig" }, new Dictionary<string, object>() { { "version", "3.10+" }, { "minTargetSdk", "7.0" } });
#endif
    }

    // Handle delayed loading of the dependency resolvers.
    private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromPath) {
        foreach (string asset in importedAssets) {
            if (asset.Contains("IOSResolver") ||
                asset.Contains("JarResolver")) {
                SetupDeps();
                break;
            }
        }
    }
}

