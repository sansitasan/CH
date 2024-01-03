using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Script
{
    public List<string> talkname;
    public List<int> discription;
    public ScriptData data;
}

[Serializable]
public class ScriptData
{
    public string nameNstate;
    public string dialog;
}

[Serializable]
public class ScriptLoad
{
    public List<Script> scripts;
}