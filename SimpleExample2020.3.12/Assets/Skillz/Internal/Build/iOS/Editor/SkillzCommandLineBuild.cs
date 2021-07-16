using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;



/// <summary>
/// Facilitates command-line builds of this project.
/// Note that Unity 4 (but not 5) requires Pro to build via command-line.
/// If using this, you should set up the auto-build constants in SkillzPostProcess.cs.
/// </summary>
public static class SkillzCommandLineBuild
{
    /// <summary>
    /// Makes an append build of this project for iOS.
    /// Recognizes the following command-line arguments:
    ///
    /// * "-buildPath 'my/path/to/build'": can be used to specify the build path for the project.
    ///   The default build path will be alongside the "Assets" folder, in "Build/iOS/".
    /// * "-appendBuild": makes an append build instead of a replace build.
    /// * "-devBuild": makes a development build.
    /// </summary>
    public static void Build()
    {
        string[] scenes = EditorBuildSettings.scenes.Select((bs, i) => bs.path).ToArray();

        //Parse command-line args.
        string buildPath = "Build/iOS/";
        bool append = false,
             dev = false;
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        for (int i = 0; i < commandLineArgs.Length; ++i)
        {
            if (commandLineArgs[i] == "-buildPath" && i < commandLineArgs.Length - 1)
            {
                buildPath = commandLineArgs[i + 1];
            }
            else if (commandLineArgs[i] == "-append")
            {
                append = true;
            }
            else if (commandLineArgs[i] == "-devBuild")
            {
                dev = true;
            }
        }

        //Get the build target enum.
        //Unity 4 uses 'iPhone'; Unity 5 uses 'iOS'.
        string enumValue = (Application.unityVersion[0] == '5' ? "iOS" : "iPhone");
        BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), enumValue);

        //Calculate build options.
        BuildOptions opts = BuildOptions.None;
        if (append)
        {
            opts |= BuildOptions.AcceptExternalModificationsToPlayer;
        }
        if (dev)
        {
            opts |= BuildOptions.Development;
        }


        BuildPipeline.BuildPlayer(scenes, buildPath, target, opts);
    }
}
