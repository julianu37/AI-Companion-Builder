using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BepInEx.Configuration;
using UnityEngine;

namespace CompanionBuilder;

/// <summary>
/// The builder's own DevBridge: a TCP loopback server, <b>separate from AI Companions'</b> (port 47111 against the
/// companion mod's 47110). Each mod owns its bridge, its port and its commands - a client connects to one or the
/// other, never to a shared surface, so a command can never reach the wrong mod.
///
/// <para><b>Off by default.</b> <c>[Advanced] DevBridge</c> turns it on; without it nothing listens.</para>
/// </summary>
internal sealed class BuilderBridge
{
	private TcpListener _listener;

	private Thread _thread;

	private volatile bool _running;

	internal static bool Enabled
	{
		get
		{
			return BuilderConfig.DevBridge != null && BuilderConfig.DevBridge.Value;
		}
	}

	internal static int Port
	{
		get
		{
			return BuilderConfig.DevBridgePort != null ? BuilderConfig.DevBridgePort.Value : 47111;
		}
	}

	internal void Start()
	{
		if (!Enabled || _running)
		{
			return;
		}
		try
		{
			_listener = new TcpListener(IPAddress.Loopback, Port);
			_listener.Start();
			_running = true;
			_thread = new Thread(Loop);
			_thread.IsBackground = true;
			_thread.Name = "CompanionBuilder.DevBridge";
			_thread.Start();
			BuilderPlugin.Log?.LogInfo("[DevBridge] builder bridge listening on 127.0.0.1:" + Port);
		}
		catch (Exception ex)
		{
			BuilderPlugin.Log?.LogError("[DevBridge] could not start: " + ex.Message);
			_running = false;
		}
	}

	internal void Stop()
	{
		_running = false;
		try
		{
			_listener?.Stop();
		}
		catch
		{
			// nothing to do on shutdown
		}
		_listener = null;
	}

	private void Loop()
	{
		while (_running && _listener != null)
		{
			try
			{
				using (TcpClient client = _listener.AcceptTcpClient())
				using (NetworkStream stream = client.GetStream())
				{
					string line = ReadLine(stream);
					if (line != null)
					{
						// the command runs on the main thread, like the game's own calls
						string answer = DispatchOnMainThread(line);
						byte[] bytes = Encoding.UTF8.GetBytes(answer + "\n");
						stream.Write(bytes, 0, bytes.Length);
					}
				}
			}
			catch (Exception ex)
			{
				if (_running)
				{
					BuilderPlugin.Log?.LogWarning("[DevBridge] connection: " + ex.Message);
				}
			}
		}
	}

	/// <summary>Reads one line (the protocol is one request per connection, like the companion mod's bridge).</summary>
	private static string ReadLine(NetworkStream stream)
	{
		List<byte> buffer = new List<byte>();
		int value;
		while ((value = stream.ReadByte()) >= 0)
		{
			if (value == '\n')
			{
				break;
			}
			buffer.Add((byte)value);
		}
		return buffer.Count > 0 ? Encoding.UTF8.GetString(buffer.ToArray()) : null;
	}

	/// <summary>
	/// Runs one command. The builder carries no game state yet, so the surface is honest: <c>ping</c> answers, and every
	/// other command says it does not exist yet instead of pretending.
	/// </summary>
	private static string DispatchOnMainThread(string request)
	{
		try
		{
			string cmd = request.Trim();
			if (cmd.Length == 0)
			{
				return "{\"ok\":false,\"error\":\"empty request\"}";
			}
			if (cmd.StartsWith("ping"))
			{
				return "{\"ok\":true,\"data\":{\"bridge\":\"CompanionBuilder.DevBridge\",\"version\":\""
					+ BuilderPlugin.PluginVersion + "\",\"inGame\":" + (ZNetScene.instance != null ? "true" : "false") + "}}";
			}
			return "{\"ok\":false,\"error\":\"unknown command: " + cmd.Replace("\"", "'") + "\"}";
		}
		catch (Exception ex)
		{
			return "{\"ok\":false,\"error\":\"" + ex.Message.Replace("\"", "'") + "\"}";
		}
	}
}
