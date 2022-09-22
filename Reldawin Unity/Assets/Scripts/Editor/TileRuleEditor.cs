using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TileRuleEditor : EditorBase
{
    private Sprite[] sprites;

    //tile properties
    private TRETileRuleList activeList = new TRETileRuleList();

    [MenuItem( "Window/Tile Rule Editor" )]
    private static void ShowWindow()
    {
        GetWindow( typeof( TileRuleEditor ) );
    }

    protected override void OnClick_SaveButton()
    {
        Save( activeList, "/atlas.xml" );
    }

    protected override void CreationWindow()
    {
        PaintLabel( "Tiles[x + 0, y + 1], //forward" );
        PaintLabel( "Tiles[x + 1, y + 0], //right" );
        PaintLabel( "Tiles[x + 0, y - 1], //down" );
        PaintLabel( "Tiles[x - 1, y + 0], //left" );
        PaintLabel( "Tiles[x + 1, y + 1], //forard-right" );
        PaintLabel( "Tiles[x + 1, y - 1], //back-right" );
        PaintLabel( "Tiles[x - 1, y - 1], //back-left" );
        PaintLabel( "Tiles[x - 1, y + 1]  //forward-left" );

        for ( int i = 0; i < activeList.list.Count; i++ )
            PaintSpriteAtlasKey( sprites[i], ref activeList.list[i].state );
    }

    protected override void Load()
    {
        sprites = Resources.LoadAll<Sprite>( "Sprites/Enviroment/Terrain/TileTemplate" );

        if ( File.Exists( Application.streamingAssetsPath + "/atlas.xml" ) )
        {
            activeList = Load<TRETileRuleList>( "/atlas.xml" );
            for ( int i = 0; i < sprites.Length; i++ )
            {
                activeList.list[i].name = sprites[i].name.Remove( 0, "TileTemplate".Length );
            }
            LoadOptions = activeList.GetNames;
        }
        else
        {
            for ( int i = 0; i < sprites.Length; i++ )
            {
                activeList.list.Add( new TRETileRule() );
                activeList.list[i].name = sprites[i].name.Remove( 0, "TileTemplate".Length );
            }
        }

        IncludeLoadList = false;
        IncludeSaveBtn = true;
        IncludeBackBtn = false;

        base.Load();

        WindowState = WindowStates.Create;
    }
}