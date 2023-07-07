using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private bool isLockingPlayer;
    
    private Narrative _narrativeStructure;
    private Dictionary<string, GameObject> _speakers;

    private void Awake()
    {
        _narrativeStructure = narrativeLoader.LoadNarrativeFromData();
    }
    
    // controlling everything from narrative

}
