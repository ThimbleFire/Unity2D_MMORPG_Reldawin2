using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class DoodadEditor : EditorWindow
{
    [XmlRoot( "ItemYield" )]
    public class Yield
    {
        // yield id is in binary because the yield is an item
        public int id { get; set; }
        public int rate { get; set; }
    }

    [XmlRoot( "Doodad" )]
    public class DEDoodad
    {
        public string name { get; set; }
        public int id { get; set; }

        [XmlArray( "ItemYields" )]
        public Yield[] _doodadSpawnsAndProbabilities;
    }

    [XmlRoot( "Doodads" )]
    public class DEDoodadList
    {
        public List<DEDoodad> list = new List<DEDoodad>();
    }

    enum WindowState { Main, Create }

    //select tile to load
    private WindowState windowState = WindowState.Main;
    private const string ServerDir = "C:/Users/Retri/Documents/GitHub/ReldawinUnity2D/ReldawinServerMaster/ReldawinServerMaster/bin/Debug/";
    private static DEDoodadList activeList;
    private static ItemEditor.IEItemList itemList;
    private bool loaded = false;
    private int loadIndex = 0;
    private int y = 0;
    private string[] options;

    //doodad properties
    private string _doodadName = string.Empty;
    private int _ID = 0;
    private List<Yield> yieldRates = new List<Yield>();
    private int tempProbabilityOptionIndex = 0;
    private int tempProbabilitySpawnRate = 0;

    [MenuItem( "Window/Doodad Editor" )]

    public static void ShowWindow()
    {
        GetWindow( typeof( DoodadEditor ) );
    }

    [System.Obsolete]
    private void OnGUI()
    {
        focusedWindow.minSize = new Vector2( 400, 100 );

        switch ( windowState )
        {
            case WindowState.Main:
                MainWindow();
                break;
            case WindowState.Create:
                CreationWindow();
                break;
        }
    }

    private void MainWindow()
    {
        if ( !loaded )
        {
            Load();
            LoadItems();
        }
        PaintLoadList();

        if ( GUILayout.Button( "New" ) )
        {
            windowState = WindowState.Create;

            return;
        }
        if ( GUILayout.Button( "Load Selected" ) )
        {
            windowState = WindowState.Create;

            _doodadName = activeList.list[loadIndex].name;
            _ID = activeList.list[loadIndex].id;
            yieldRates = new List<Yield>( activeList.list[loadIndex]._doodadSpawnsAndProbabilities );

            return;
        }
    }

    private void CreationWindow()
    {
        y = 4;

        PaintTileName();
        PaintID();
        PaintHorizontalLine();
        ResourceYieldProbability();
        PaintHorizontalLine();
        PaintProbabilities();

        PaintSaveButton();
        PaintBackButton();
    }

    private void ResourceYieldProbability()
    {
        tempProbabilityOptionIndex = EditorGUI.Popup(
            new Rect( 4, y, position.width - 12, 20 ),
            "Item",
            tempProbabilityOptionIndex,
            itemList.GetNames ); y += 22;

        tempProbabilitySpawnRate = EditorGUI.IntSlider( new Rect( 4, y, position.width - 12, 20 ), "Yield Probability", tempProbabilitySpawnRate, 0, 100 ); y += 22;

        EditorGUI.BeginDisabledGroup( tempProbabilitySpawnRate == 0 );
        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ), "Dicks" ); y += 22;
        if ( GUILayout.Button( "Add probability" ) )
        {
            yieldRates.Add( new Yield() { id = itemList.list[tempProbabilityOptionIndex].id, rate = tempProbabilitySpawnRate } );
            tempProbabilityOptionIndex = 0;
            tempProbabilitySpawnRate = 0;
        }
        GUILayout.EndArea();
        EditorGUI.EndDisabledGroup();
    }

    private void PaintHorizontalLine()
    {
        Handles.color = Color.gray;
        Handles.DrawLine( new Vector3( 4, y + 11 ), new Vector3( position.width - 8, y + 11 ) ); y += 22;
    }

    private void PaintTileName()
    {
        _doodadName = EditorGUI.TextField( new Rect( 4, y, position.width - 12, 20 ), "Doodad Name", _doodadName ); y += 22;
    }

    private void PaintID()
    {
        _ID = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), "Base ID", _ID ); y += 22;
    }

    private void PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 70, position.width - 70, 20 ) );

        if ( GUILayout.Button( string.Format( "Save [ {0} ]", _doodadName ) ) )
        {
            DEDoodad newDoodad = new DEDoodad();

            newDoodad.id = _ID;
            newDoodad.name = _doodadName;
            newDoodad._doodadSpawnsAndProbabilities = yieldRates.ToArray();

            if ( activeList != null )
            {
                if ( activeList.list != null )
                {
                    DEDoodad doodadInList = activeList.list.Find( x => x.id == newDoodad.id );

                    if ( doodadInList == null )
                        activeList.list.Add( newDoodad );
                    else
                    {
                        activeList.list.Remove( doodadInList );
                        activeList.list.Add( newDoodad );
                    }

                }
            }

            Save();

            yieldRates.Clear();
        }
        GUILayout.EndArea();
    }

    private void PaintProbabilities()
    {
        foreach ( Yield yields in yieldRates )
        {
            EditorGUI.LabelField( new Rect( 4, y, position.width - 12, 20 ), yields.id.ToString() );
            EditorGUI.LabelField( new Rect( position.width - 40, y, position.width / 2, 20 ), ( yields.rate ).ToString() + " %" );
            y += 22;
        }
    }

    private void PaintBackButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 40, position.width - 70, 20 ) );
        if ( GUILayout.Button( "Back" ) )
        {
            windowState = WindowState.Main;
            loaded = false;
        }
        GUILayout.EndArea();
    }

    private void PaintLoadList()
    {
        string[] options = new string[activeList.list.Count];

        for ( int i = 0; i < options.Length; i++ )
        {
            options[i] = string.Format("{0} ({1})", activeList.list[i].name, activeList.list[i].id);
        }

        loadIndex = EditorGUI.Popup( new Rect( 4, 44, position.width - 8, 20 ), loadIndex, options );
    }

    private void Load()
    {
        activeList = new DEDoodadList();

        try
        {
            loaded = true;
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
            activeList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();
        }
        catch ( Exception e )
        {
            Debug.LogWarning( e.Message );
        }

        if ( activeList == null )
            activeList = new DEDoodadList();
    }

    private void LoadItems()
    {
        loaded = true;
        XmlSerializer serializer = new XmlSerializer( typeof( ItemEditor.IEItemList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
        itemList = serializer.Deserialize( stream ) as ItemEditor.IEItemList;
        stream.Close();
    }

    private void Save()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Create );
            serializer.Serialize( stream, activeList );
            stream.Close();

            //save to server
            stream = new FileStream( ServerDir + "doodads.xml", FileMode.Create );
            serializer.Serialize( stream, activeList );
            stream.Close();
        }
        catch ( Exception e )
        {
            Debug.LogError( e.Message );
        }
    }
}
