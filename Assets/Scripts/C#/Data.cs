using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Script
{
    public readonly List<string> talkname;
    public readonly int speed;
    public readonly Characters character;
    public readonly Emotions emotion;
    public readonly string dialog;

    public Script(JsonScript script)
    {
        talkname = script.talkname;
        speed = script.discription[0];
        emotion = (Emotions)script.data.state;
        character = (Characters)script.discription[1];
        dialog = script.data.dialog;
    }
}

[Serializable]
public class JsonScript
{
    public List<string> talkname;
    public List<int> discription;
    public ScriptData data;
}

[Serializable]
public class ScriptData
{
    public int state;
    public string dialog;
}

[Serializable]
public class ScriptLoad
{
    public List<JsonScript> scripts;

    public List<Script> GetScript()
    {
        List<Script> script = new List<Script>(scripts.Count);

        for (int i = 0; i < scripts.Count; ++i)
        {
            script.Add(new Script(scripts[i]));
        }

        return script;
    }
}