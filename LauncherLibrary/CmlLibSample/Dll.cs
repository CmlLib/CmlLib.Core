using System;
using System.Collections.Generic;
using System.Reflection;

public class Dll
{
    static Dll()
    {
        AppDomain.CurrentDomain.AssemblyResolve += Assembly_Resolve;
    }

    static Dictionary<string, Assembly> dict = new Dictionary<string, Assembly>();
    public static bool LoadDll(string path)
    {
        Assembly curAssm = Assembly.GetExecutingAssembly();
        string appName = curAssm.GetName().Name.Replace(" ", "_");
        Assembly dllAssm = null;
        byte[] dllData;
        using (System.IO.Stream s = curAssm.GetManifestResourceStream($"{appName}.{path}"))
        {
            if (s != null)
            {
                dllData = new byte[s.Length];
                s.Read(dllData, 0, (int)s.Length);
                dllAssm = Assembly.Load(dllData);
            }
            else
            {
                return false;
            }
        }
        dict.Add(dllAssm.FullName, dllAssm);
        return true;
    }
    static Assembly Assembly_Resolve(object sender, ResolveEventArgs e)
    {
        if (dict.ContainsKey(e.Name))
        {
            Assembly assm = dict[e.Name];
            dict.Remove(e.Name);
            return assm;
        }
        return null;
    }
}
