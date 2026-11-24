using System;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CompanionBuilder;

/// <summary>
/// The AI Companion Builder: the build half of the AI Companions mod, as a mod of its own.
///
/// <para><b>Own identity.</b> A different GUID, a different DLL and a different config file from AI Companions, so the
/// two install side by side and either can be removed without touching the other. The <c>HC_*</c> ZDO keys and the
/// companion prefabs are NOT reused: this mod brings its own builder, because sharing a network identity across two
/// DLLs is what makes "two copies loaded" bugs.</para>
///
/// <para><b>Status.</b> The plugin boots, loads its config and exposes <see cref="Log"/>; the build pipeline
/// (blueprint parsing, planning, placement) arrives file by file, each one with its harness case in
/// <c>tools/builder-check</c> before it is wired to anything.</para>
/// </summary>
[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class BuilderPlugin : BaseUnityPlugin
{
	/// <summary>Deliberately NOT the companion mod's GUID: two mods, two identities.</summary>
	public const string PluginGUID = "julianu37.AICompanionBuilder";

	public const string PluginName = "AI Companion Builder";

	public const string PluginVersion = "0.1.0";

	private const string ConfigFileName = "AiCompanionBuilder.cfg";

	internal static ManualLogSource Log;

	private static Harmony _harmony;

	private readonly BuilderBridge _bridge = new BuilderBridge();

	private void Awake()
	{
		Log = Logger;
		BuilderConfig.Init(ConfigFilePath());
		_harmony = new Harmony(PluginGUID);
		try
		{
			_harmony.PatchAll(typeof(BuilderPlugin).Assembly);
			Log.LogInfo(PluginName + " v" + PluginVersion + " loaded successfully! (" + _harmony.GetPatchedMethods().Count() + " methods patched)");
			_bridge.Start();
		}
		catch (Exception ex)
		{
			Log.LogError(PluginName + ": Harmony PatchAll failed: " + ex);
		}
	}

	private void OnDestroy()
	{
		_bridge.Stop();
		if (_harmony != null)
		{
			_harmony.UnpatchSelf();
		}
	}

	/// <summary>The settings file, named after this mod (not after the GUID), like the companion mod does since 1.8.3.</summary>
	private static string ConfigFilePath()
	{
		return System.IO.Path.Combine(Paths.ConfigPath, ConfigFileName);
	}
}
