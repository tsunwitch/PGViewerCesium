using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using UnityEngine.Networking;

public class FileManager : MonoBehaviour
{
    public GameObject parser;

    public void OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filter = "IGC file (*.igc) | *.igc";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path => {
            parser.GetComponent<Parser>().loadIGC(path);
        });
    }
}
