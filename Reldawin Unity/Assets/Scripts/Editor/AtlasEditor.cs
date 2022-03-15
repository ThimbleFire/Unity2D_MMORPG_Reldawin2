using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class AtlasEditor : EditorBase
{
    private Sprite[] sprites;

    //tile properties
    private AEAtlasList activeList = new AEAtlasList();

    [MenuItem( "Window/Atlas Editor" )]
    private static void ShowWindow()
    {
        GetWindow( typeof( AtlasEditor ) );
    }

    protected override void OnClick_SaveButton()
    {
        Save( activeList, "/atlas.xml" );
    }

    protected override void CreationWindow()
    {
        for ( int i = 0; i < activeList.list.Count; i++ )
            PaintSpriteAtlasKey( sprites[i], ref activeList.list[i].state );
    }

    protected override void Load()
    {
        sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/TileTemplate" );

        if ( File.Exists( Application.streamingAssetsPath + "/atlas.xml" ) )
        {
            activeList = Load<AEAtlasList>( "/atlas.xml" );
            LoadOptions = activeList.GetNames;
            IncludeLoadList = false;
        }
        else
        {
            for ( int i = 0; i < sprites.Length; i++ )
            {
                activeList.list.Add( new AEAtlas() );
            }
        }

        base.Load();

        WindowState = WindowStates.Create;
    }
}
