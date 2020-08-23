using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using R2API;

namespace be.vanderlei.arne.Prefaboutput
{
    // Token: 0x02000002 RID: 2
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("be.vanderlei.arne.Prefaboutput", "PrefabOutput", "1.0")]
    public class ExamplePlugin : BaseUnityPlugin
    {
        // Token: 0x06000001 RID: 1 RVA: 0x0000205C File Offset: 0x0000025C
        public void Awake()
        {
            R2API.AssetAPI.AssetLoaderReady += delegate (object s, EventArgs e)
            {
                string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Prefabs/";
                string text2 = "";
                base.Logger.LogInfo(text);
                text2 = text2 + text + "\n";
                ExamplePlugin.types = new List<Type>();
                try
                {
                    text2 += "create Directory\n";
                    Directory.CreateDirectory(text);
                    text2 += "for each\n";
                    string text3;
                    foreach (GameObject gameObject in Resources.LoadAll<GameObject>("Prefabs/"))
                    {
                        Component[] components = gameObject.GetComponents<Component>();
                        text3 = "";
                        text3 = text3 + gameObject.name + "\n";
                        text3 += ExamplePlugin.OutputComponents(components, ">");
                        text3 += ExamplePlugin.GetChildren(gameObject.transform, ">");
                        File.WriteAllText(text + gameObject.name + ".txt", text3);
                    }
                    text2 += "types\n";
                    text3 = "";
                    text3 += "\n\ntypes\n\n";
                    for (int j = 0; j < ExamplePlugin.types.Count; j++)
                    {
                        MemberInfo[] members = ExamplePlugin.types[j].GetMembers();
                        text3 = text3 + ExamplePlugin.types[j].Name + "\n";
                        for (int k = 0; k < members.Length; k++)
                        {
                            if (!(members[k].DeclaringType.Name == "MonoBehaviour") && !(members[k].DeclaringType.Name == "Component") && !(members[k].DeclaringType.Name == "Behaviour") && !(members[k].DeclaringType.Name == "Object"))
                            {
                                text3 = string.Concat(new string[]
                                {
                                text3,
                                "> ",
                                members[k].MemberType.ToString(),
                                ": ",
                                members[k].Name,
                                "\n"
                                });
                            }
                        }
                    }
                    File.WriteAllText(text + "types.txt", text3);
                }
                catch (Exception ex)
                {
                    text2 += ex.ToString();
                }
                File.WriteAllText(text + "log.txt", text2);
            };
            
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
        public void Update()
        {
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000022F0 File Offset: 0x000004F0
        public static string OutputComponents(Component[] components, string delimi)
        {
            string text = "";
            for (int i = 0; i < components.Length; i++)
            {
                string text2;
                if (!(components[i].GetType().FullName == "UnityEngine.Transform"))
                {
                    Type type = components[i].GetType();
                    text2 = type.FullName + "\n";
                    foreach (FieldInfo fieldInfo in type.GetFields())
                    {
                        text2 = string.Concat(new object[]
                        {
                            text2,
                            delimi,
                            "v ",
                            fieldInfo.Name,
                            " = ",
                            fieldInfo.GetValue(components[i]),
                            "\n"
                        });
                    }
                }
                else
                {
                    Transform transform = (Transform)components[i];
                    text2 = string.Concat(new string[]
                    {
                        "transform = p:",
                        transform.localPosition.ToString(),
                        " r:",
                        transform.eulerAngles.ToString(),
                        " s:",
                        transform.localScale.ToString(),
                        "\n"
                    });
                }
                text = string.Concat(new string[]
                {
                    text,
                    "\n",
                    delimi,
                    " ",
                    text2
                });
                if (!ExamplePlugin.types.Contains(components[i].GetType()))
                {
                    ExamplePlugin.types.Add(components[i].GetType());
                }
            }
            return text;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002484 File Offset: 0x00000684
        public static string GetChildren(Transform transform, string delimi)
        {
            string text = "";
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject gameObject = transform.GetChild(i).gameObject;
                text = string.Concat(new string[]
                {
                    text,
                    delimi,
                    "c ",
                    gameObject.name,
                    "\n"
                });
                Component[] components = gameObject.GetComponents<Component>();
                text += ExamplePlugin.OutputComponents(components, delimi + ">");
                text += ExamplePlugin.GetChildren(transform.GetChild(i), delimi + ">");
            }
            return text;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002052 File Offset: 0x00000252
        public ExamplePlugin()
        {
        }

        // Token: 0x04000001 RID: 1
        public static List<Type> types;
    }
}
