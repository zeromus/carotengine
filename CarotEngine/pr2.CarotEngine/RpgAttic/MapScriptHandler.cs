using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using pr2.CarotEngine.Maps;



namespace pr2.CarotEngine
{
	public class ScriptActivationArgs
	{
		public String scriptName;
		public Entity activator;
		public Entity activatedEntity;
		public TileEngine.Zone activatedZone;
	}
	public class MapScriptHandler
	{
		String basename;
		MapScript mapScript;
		Type mapScriptType;
		
		public MapScriptHandler(String mapFilename, Map map, RpgController rpgController)
		{
			basename = FindBaseName(mapFilename);
			mapScript = null;
			mapScriptType = null;

			// Attempt to instanciate class
			try
			{
				Type t = Type.GetType(String.Format("Cabedge.Cabedge+Map_{0},Cabedge", basename), false, true);
				if (t != null)
				{
					mapScriptType = t;
					if (t.IsSubclassOf(typeof(MapScript)))
					{
						mapScript = (MapScript)Activator.CreateInstance(t);
						mapScript.init(map, this, rpgController);
					}
					else
					{
						Console.WriteLine("WARNING: Found class 'Map_{0}' but it does not extend MapScript!!", basename);
					}
				}
				else
				{
					Console.WriteLine("WARNING: Did not find Map_{0}.", basename);
				}
			}
			catch (TypeLoadException ex)
			{
				Console.WriteLine("WARNING: Did not find Map_{0}.", basename, ex);
			}
			catch (AmbiguousMatchException ex)
			{
				Console.WriteLine("WARNING: Found multiple matching classes.", ex);
			}
		}

		public bool InvokeOnload(String arg)
		{
			if (mapScript != null)
			{
				try
				{
					MethodInfo mi = mapScriptType.GetMethod(
						"onLoad",
						BindingFlags.IgnoreCase |
						BindingFlags.Instance |
						BindingFlags.Public |
						BindingFlags.FlattenHierarchy
					);
					if (mi != null)
					{
						ScriptActivationArgs saa = new ScriptActivationArgs();
						saa.activator = null;
						saa.activatedEntity = null;
						saa.activatedZone = null;
						saa.scriptName = "onLoad";

						mapScript.setArgs(saa);

						mi.Invoke(mapScript, new object[] { (arg==null) ? "" : arg });
						return true;
					}
					else
					{
						Console.WriteLine("WARNING: Failed to find matching method for event '{0}'", "onLoad");
					}
				}
				catch (AmbiguousMatchException ex)
				{
					Console.WriteLine("WARNING: Found multiple matching methods.", ex);
				}
			}
			return false;
		}
		public bool InvokeDirect(String script)
		{
			if (mapScript != null)
			{
				try
				{
					MethodInfo mi = mapScriptType.GetMethod(
						script,
						BindingFlags.IgnoreCase |
						BindingFlags.Instance |
						BindingFlags.Public |
						BindingFlags.FlattenHierarchy
					);
					if (mi != null)
					{
						ScriptActivationArgs saa = new ScriptActivationArgs();
						saa.activator = null;
						saa.activatedEntity = null;
						saa.activatedZone = null;
						saa.scriptName = script;

						mapScript.setArgs(saa);
						
						mi.Invoke(mapScript, new object[] { });
						return true;
					}
					else
					{
						Console.WriteLine("WARNING: Failed to find matching method for event '{0}'", script);
					}
				}
				catch (AmbiguousMatchException ex)
				{
					Console.WriteLine("WARNING: Found multiple matching methods.", ex);
				}
			}
			return false;
		}

		public bool EntityInvoke(Entity activator, Entity activatedEntity)
		{
			if (mapScript != null && activatedEntity.activationScript != null)
			{
				try
				{
					MethodInfo mi = mapScriptType.GetMethod(
						activatedEntity.activationScript,
						BindingFlags.IgnoreCase | 
						BindingFlags.Instance | 
						BindingFlags.Public |
						BindingFlags.FlattenHierarchy
					);
					if (mi != null)
					{
						ScriptActivationArgs saa = new ScriptActivationArgs();
						saa.activator = activator;
						saa.activatedEntity = activatedEntity;
						saa.scriptName = activatedEntity.activationScript;
						mapScript.setArgs(saa);
						mi.Invoke(mapScript, new object[] { });
						return true;
					}
					else
					{
						Console.WriteLine("WARNING: Failed to find matching method for event '{0}'", activatedEntity.activationScript);
					}					
				}
				catch (AmbiguousMatchException ex)
				{
					Console.WriteLine("WARNING: Found multiple matching methods.", ex);
				}
			}
			return false;
		}

		public bool ZoneInvoke(Entity activator, TileEngine.Zone zone)
		{
			if (mapScript != null && zone.script != null)
			{
				try
				{
					MethodInfo mi = mapScriptType.GetMethod(
						zone.script,
						BindingFlags.IgnoreCase |
						BindingFlags.Instance |
						BindingFlags.Public |
						BindingFlags.FlattenHierarchy
					);

					if (mi != null)
					{
						ScriptActivationArgs saa = new ScriptActivationArgs();
						saa.activator = activator;
						saa.activatedZone = zone;
						saa.scriptName = zone.script;

						mapScript.setArgs(saa);
						
						mi.Invoke(mapScript, new object[] { });
						return true;
					}
					else
					{
						Console.WriteLine("WARNING: Failed to find matching method for event '{0}'", zone.script);
					}
				}
				catch (AmbiguousMatchException ex)
				{
					Console.WriteLine("WARNING: Found multiple matching methods.", ex);
				}
			}
			return false;			
		}

		protected String FindBaseName(String fname)
		{
			int extloc = fname.LastIndexOf(".map", StringComparison.OrdinalIgnoreCase);
			if (extloc > -1)
				fname = fname.Substring(0, extloc);
			int startloc = fname.LastIndexOf('/');
			if (startloc > -1)
				fname = fname.Substring(startloc + 1, fname.Length - startloc - 1);
			Console.WriteLine("Map name: {0}", fname);
			return fname.ToLower();
		}
	}
}
