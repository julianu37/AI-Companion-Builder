using BepInEx.Configuration;

namespace CompanionBuilder;

/// <summary>
/// Every setting of the builder, in one place - the counterpart of the companion mod's <c>ModConfig</c>, and the same
/// rule applies: a new option needs its <c>hc_cfg_builder_&lt;section&gt;_&lt;key&gt;</c> label in the four JSONs in
/// the same cycle, or the translation gate fails.
///
/// <para><b>Why so few.</b> The first cycle only has what the plugin needs to boot and be found. Options arrive with
/// the feature that reads them - a setting nobody reads is a promise with no code behind it.</para>
/// </summary>
internal static class BuilderConfig
{
	internal static ConfigFile File;

	internal static ConfigEntry<bool> Enabled;

	internal static ConfigEntry<bool> DebugLog;

	internal static ConfigEntry<bool> DevBridge;

	internal static ConfigEntry<int> DevBridgePort;

	internal static void Init(string path)
	{
		File = new ConfigFile(path, saveOnInit: true);
		// the binds write the whole file once each; with the flag off the boot does one write at the end instead
		bool saveOnConfigSet = File.SaveOnConfigSet;
		File.SaveOnConfigSet = false;
		try
		{
			BindEntries();
		}
		finally
		{
			File.Save();
			File.SaveOnConfigSet = saveOnConfigSet;
		}
	}

	private static void BindEntries()
	{
		Enabled = Bind(File, "Builder", "Enabled", true, "The builder works at all. Off = it loads, reads nothing and does nothing (the pieces and the panel stay silent).");
		DebugLog = Bind(File, "Builder", "DebugLog", false, "Write the detailed build log (every decision, every retry) instead of only the summary.");
		// 19/11/2026: the builder has its OWN bridge (port 47111) - the companion mod's is 47110. Each mod owns its
		// port and its commands; a client connects to one or the other, never to a shared surface.
		DevBridge = Bind(File, "Advanced", "DevBridge", false, "Development tool (MCP): runs a TCP loopback server for the builder. Off by default.");
		DevBridgePort = Bind(File, "Advanced", "DevBridgePort", 47111, "Development tool (MCP): TCP port of the builder's DevBridge (loopback only). The companion mod uses 47110.");
	}

	private static ConfigEntry<T> Bind<T>(ConfigFile file, string section, string key, T value, string description)
	{
		return file.Bind(section, key, value, description);
	}
}
