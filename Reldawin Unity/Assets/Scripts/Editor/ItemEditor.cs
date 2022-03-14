using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

public class ItemEditor : EditorBase
{
    //item properties
    private string _itemName = string.Empty;
    private string _itemSpriteFileName16x16 = string.Empty;
    private string _itemSpriteFileName32x32 = string.Empty;
    private string _itemSpriteOnFloorFileName = string.Empty;
    private Sprite _sprite16;
    private Sprite _sprite32;
    private Sprite _spriteOnFloor;
    private string _flavourText;
    private int _ID;

    [MenuItem( "Window/Item Editor" )]

    public static void ShowWindow()
    {
        activeList = new IEItemList();
        GetWindow( typeof( ItemEditor ) );
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
                Create();
                break;
        }
    }

    private void MainWindow()
    {
        if ( !loaded )
            Load();

        PaintLoadList();

        if ( GUILayout.Button( "New" ) )
        {
            windowState = WindowState.Create;

            _itemName = string.Empty;
            _itemSpriteFileName16x16 = string.Empty;
            _itemSpriteFileName32x32 = string.Empty;
            _itemSpriteOnFloorFileName = string.Empty;
            _sprite16 = null;
            _sprite32 = null;
            _spriteOnFloor = null;
            _flavourText = string.Empty;
            _ID = 0;

            return;
        }
        if ( GUILayout.Button( "Load Selected" ) )
        {
            windowState = WindowState.Create;

            _itemName = activeList.list[loadIndex].name;
            _itemSpriteFileName16x16 = activeList.list[loadIndex].itemSpriteFileName16x16;
            _itemSpriteFileName32x32 = activeList.list[loadIndex].itemSpriteFileName32x32;
            _itemSpriteOnFloorFileName = activeList.list[loadIndex].itemSpriteOnFloorFileName;
            _flavourText = activeList.list[loadIndex].flavourText;
            _ID = activeList.list[loadIndex].id;

            Sprite[] sprites16 = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/Items_16x16" );
            Sprite[] sprites32 = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/Items_32x32" );

            foreach ( Sprite sprite in sprites16 )
            {
                if ( sprite.name == _itemSpriteFileName16x16 )
                {
                    _sprite16 = sprite;
                    break;
                }
            }

            foreach ( Sprite sprite in sprites32 )
            {
                if ( sprite.name == _itemSpriteFileName32x32 )
                {
                    _sprite32 = sprite;
                    _spriteOnFloor = sprite; //temporary. Eventually we want a unique sprite for items on floor
                    break;
                }
            }

        }
    }

    private void PaintLoadList()
    {
        string[] options = new string[activeList.list.Count];

        for ( int i = 0; i < options.Length; i++ )
        {
            options[i] = string.Format( "{0} ({1})", activeList.list[i].name, activeList.list[i].id );
        }

        loadIndex = EditorGUI.Popup( new Rect( 4, 42, position.width - 8, 20 ), loadIndex, options );
    }

    private void Create()
    {
        y = 4;

        PaintItemName();

        _sprite16 = PaintSpriteField( _sprite16, ref _itemSpriteFileName16x16 );
        _sprite32 = PaintSpriteField( _sprite32, ref _itemSpriteFileName32x32 );
        _spriteOnFloor = PaintSpriteField( _spriteOnFloor, ref _itemSpriteOnFloorFileName);

        PaintID();
        PaintFlavourText();
        PaintSaveButton();
        PaintBackButton();
    }

    private void PaintItemName()
    {
        EditorGUI.LabelField( new Rect( 74, y, position.width, 20 ), "Item Name" ); y += 20;
        _itemName = EditorGUI.TextField( new Rect( 74, y, position.width - 86, y ), _itemName ); y += 30;
    }

    private Sprite PaintSpriteField(Sprite sprite, ref string fileName)
    {
        sprite = (Sprite)EditorGUI.ObjectField( new Rect( 4, y, 64, 64 ), sprite, typeof( Sprite ), false );

        if ( sprite != null )
        {
            fileName = sprite.name;
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), "16x16 sprite name" ); y += 20;
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), fileName ); y += 60;
        }
        else
            y += 80;

        return sprite;
    }

    private void PaintFlavourText()
    {
        _flavourText = EditorGUI.TextField( new Rect( 4, y, position.width - 12, 20 ), "Flavour Text", _flavourText ); y += 20;

    }

    private void PaintID()
    {
        _ID = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), "Base ID", _ID ); y += 22;
    }

    private void PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 70, position.width - 70, 20 ) );
        if ( GUILayout.Button( string.Format( "Save [ {0} ]", _itemName ) ) )
        {
            IEItem newItem = new IEItem
            {
                id = _ID,
                flavourText = _flavourText,
                name = _itemName,
                itemSpriteFileName16x16 = _itemSpriteFileName16x16,
                itemSpriteFileName32x32 = _itemSpriteFileName32x32,
                itemSpriteOnFloorFileName = _itemSpriteOnFloorFileName,
            };

            if ( activeList != null )
            {
                if ( activeList.list != null )
                {
                    IEItem itemInList = activeList.list.Find( x => x.id == newItem.id );

                    if ( itemInList == null )
                        activeList.list.Add( newItem );
                    else
                    {
                        activeList.list.Remove( itemInList );
                        activeList.list.Add( newItem );
                    }
                }
            }

            Save();
        }
        GUILayout.EndArea();
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

    public void Save()
    {
        //save to client streaming assets
        XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();

        //save to server
        stream = new FileStream( ServerDir + "items.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();
    }

    public void Load()
    {
        try
        {
            loaded = true;
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
            activeList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open items.xml" );
        }

        if ( activeList == null )
            activeList = new IEItemList();
    }
}
