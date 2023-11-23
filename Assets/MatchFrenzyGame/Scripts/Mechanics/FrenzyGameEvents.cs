using System.Collections;
using System.Collections.Generic;
using Modules.DesignPatterns.EventManager;
using UnityEngine;

public class FrenzyGameEvents
{
    public class GetFrezyItem : IEventParameterBase
    {
        public string id;
    }
}
