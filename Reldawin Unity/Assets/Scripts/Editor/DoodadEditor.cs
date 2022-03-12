using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;

public class DoodadEditor : EditorWindow
{
    private enum WindowState { Main, Create }
    private WindowState windowState = WindowState.Main;
    private const string ServerDir = "C:/Users/Retri/Documents/GitHub/Reldawin/ReldawinServerMaster/ReldawinServerMaster/bin/Debug/";
    private static DEDoodadList activeList;
    private static IEItemList itemList;
    private bool loaded = false;
    private int loadIndex = 0;
    private int y = 0;

    //doodad properties
    private string _doodadName = string.Empty;
    private int _ID = 0;
    private List<Droprate> yieldRates = new List<Droprate>();
    private int tempProbabilityOptionIndex = 0;
    private int tempProbabilitySpawnRate = 0;
    DEDoodad.Interact _interact = DEDoodad.Interact.NONE;

    [MenuItem( "Window/Doodad Editor" )]

    public static void ShowWindow()
    {
        GetWindow( typeof( DoodadEditor ) );
    }

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

            _doodadName = string.Empty;
            _ID = 0;
            yieldRates.Clear();
            _interact = DEDoodad.Interact.NONE;

            return;
        }
        if ( GUILayout.Button( "Load Selected" ) )
        {
            windowState = WindowState.Create;

            _doodadName = activeList.list[loadIndex].name;
            _ID = activeList.list[loadIndex].id;
            yieldRates = new List<Droprate>( activeList.list[loadIndex].droprates );
            _interact = activeList.list[loadIndex].interact;

            return;
        }
    }

    private void CreationWindow()
    {
        y = 4;

        PaintTileName();
        PaintID();
        PaintInteractType();
        PaintHorizontalLine();
        if(itemList.GetNames.Length > 0)
            ResourceYieldProbability();
        PaintHorizontalLine();
        PaintProbabilities();

        PaintSaveButton();
        PaintBackButton();
    }

    private void PaintInteractType()
    {
        _interact = (DEDoodad.Interact)EditorGUI.Popup( new Rect( 4, y, position.width - 8, 20 ), (int)_interact, Enum.GetNames( typeof( DEDoodad.Interact ) ) ); y += 22;
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
            yieldRates.Add( new Droprate() { id = itemList.list[tempProbabilityOptionIndex].id, percent = tempProbabilitySpawnRate } );
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
            DEDoodad newDoodad = new DEDoodad
            {
                id = _ID,
                name = _doodadName,
                droprates = yieldRates.ToArray(),
                interact = _interact
            };

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

            windowState = WindowState.Main;
            loaded = false;
        }
        GUILayout.EndArea();
    }

    private void PaintProbabilities()
    {
        foreach ( Droprate yields in yieldRates )
        {
            EditorGUI.LabelField( new Rect( 4, y, position.width - 12, 20 ), yields.id.ToString() );
            EditorGUI.LabelField( new Rect( position.width - 40, y, position.width / 2, 20 ), ( yields.percent ).ToString() + " %" );
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
        try
        {
            loaded = true;
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
            activeList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open doodads.xml" );
        }

        if ( activeList == null )
            activeList = new DEDoodadList();
    }

    private void LoadItems()
    {
        try
        {
            loaded = true;
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
            itemList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open items.xml" );
        }

        if ( itemList == null )
            itemList = new IEItemList();
    }

    private void Save()
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
}
