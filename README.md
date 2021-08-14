# BinarySerializer.Klonoa
BinarySerializer.Klonoa is an extension library to [BinarySerializer](https://github.com/RayCarrot/BinarySerializer) for serializing the game data in the console Klonoa games. This can be used for reverse engineering and to read any of its files, export the data (such as the graphics, models, sounds etc.) and more.

*Note*: This library is not complete with some data not being parsed.

![Map Example](img/map_example.png)
Example image is from this library being used in [Ray1Map](https://github.com/Adsolution/Ray1Map) to display the maps

# Supported Games and Versions
## Currently Supported Versions
* Klonoa: Door to Phantomile (Prototype 1997/07/17)
* Klonoa: Door to Phantomile (NTSC)

## Planned Versions
* Klonoa: Door to Phantomile (Prototype 1997/12/18)
* Klonoa: Door to Phantomile (NTSC-J)
* Klonoa: Door to Phantomile (PAL)
* Klonoa: Door to Phantomile (Demo)

# Get Started
To use this library in your own project you must first reference [BinarySerializer](https://github.com/RayCarrot/BinarySerializer) and [BinarySerializer.PS1](https://github.com/RayCarrot/BinarySerializer.PS1) as it relies on these libraries. After that you can quickly get started using the `Loader` class.

```cs
// First create a context for the data serialization
using Context context = new Context(basePath);

// Add the IDX and BIN to the context. The BIN gets added as a linear file while the IDX has to be memory mapped. If the level data will be parsed then the exe needs to be added too.
context.AddFile(new LinearFile(context, Loader.FilePath_BIN));
context.AddFile(new MemoryMappedFile(context, Loader.FilePath_IDX, 0x80010000));

// Create a configuration
LoaderConfiguration_DTP_US config = new LoaderConfiguration_DTP_US();

// Load the IDX and pass in the configuration to it
IDX idx = FileFactory.Read<IDX>(Loader.FilePath_IDX, context, (s, idxObj) => idxObj.Pre_LoaderConfig = config);

// Create the loader, passing in the context, IDX and configuration
Loader loader = Loader.Create(context, idx, config);

// Switch to the BIN block you want to load. Block 3 is Vision 1-1 for example, while block 0 is the fixed block.
loader.SwitchBlocks(3);

// Load and process the BIN block
loader.LoadAndProcessBINBlock();

// The data is now stored in the loader and can be accessed
LevelPack_ArchiveFile level = loader.LevelPack;
Sprites_ArchiveFile[] spriteSets = loader.SpriteSets;

// For example the level model can be accessed in the level pack
PS1_TMD levelModel = level.Sectors[0].LevelModel;

// The textures and palettes are stored in the virtual VRAM in the loader
PS1_VRAM vram = loader.VRAM;

```

# Documentation (Door to Phantomile)
All of the data for the game is stored in the `FILE.BIN` file with the `FILE.IDX` telling us how to parse it. The BIN is split into 25 blocks, each representing a level (except the first 3).

Why is everything stored in a single file? Well, primarily it's for performance when loading. Now when the game wants to load a level all of its data gets read from the same place, thus is doesn't need to seek around to different sectors on the disc! This inevitably means data gets duplicated across levels, but that's not an issue since discs can store a lot of data (a lot more than the PS1 can store in memory).

## IDX
The IDX file starts with an 8-byte header followed by a pointer array. Each of these pointers leads to the load commands (which are stored further down in the file) used to load the block we want to load. As mentioned earlier each block represents a level except the first 3. Here are the first couple of blocks as an example:

- 0: Fixed data (stays in memory throughout the game)
- 1: Menu (the main menu)
- 2: Code (multiple code files)
- 3: Vision 1-1
- 4: Vision 1-2

etc.

### Load Commands
The load commands tells the game how to load the block. Each block consists of sub-blocks which we can refer to as files (so each level has multiple files that get loaded - there are however no file names or such). The first command is always a seek command which has the game seek to the start of the block on the disc. It does so using a supplied sector index, `LBA`, relative to where the BIN is on the disc. For example the third block has the LBA offset 573. To get the offset in the BIN file we do `573 * 2048 = 1173504` (each sector is 2048 bytes).

The following commands are responsible for loading the files in the block. Files have different formats, so how do we know how to parse them? Well the game does it using a function pointer which is included with each file. This points to a function in the exe which then parses the file. We can use this function pointer as a way to identify the file type since it will always stay the same for the same type of file.

## BIN
As explained in the previous section each block in the BIN consists of multiple files. There are many different types of files, but primarily we can split them into two categories; `normal files` and `archives`. Archives contain an array of additional files (such as a folder).

Now let's check out some of the file types in the BIN and what they are for.

### TIM
These files are archives which contain `.TIM` (graphics) files. This is used to load textures and palettes into the VRAM.

### OA05
Contains multiple `.VAB` and `.SEQ` files. Most likely the sound effects used for the level.

### Code
Blocks of compiled code. This doesn't get parsed and instead just loaded into memory. Mosty likely used for level specific functionality, such as bosses and cutscenes.

### SEQ
Just a `.SEQ` file. Most likely the level music.

### Backgrounds
An archive with the backgrounds for the level. Contains `.TIM` (the tilesets), `.CEL` (the tiles) and `.BGD` (the maps) files as well as some unknown files (most likely for animations).

### Sprites
The sprites for the 2D animations used in the level.

### Level
The primary level data. Contains the object models, sector data (such as the level models and movement paths) and more.

As an example, here are all the files in Vision 1-1:
- 1: OA05 (sound bank)
- 2: TIM (sprite sheets)
- 3: TIM (textures)
- 4: Backgrounds
- 6: Code
- 7: Code
- 8: Sprites
- 9: Level
- 10: SEQ

## Compression
The game uses multiple compression types.

### ULZ
A variant of LZSS. It has 4 types, with only 2 of them being used in the game.

### Raw texture block
A compression used on certain raw texture data blocks.

### Level sector
An unknown compression type used for the level sectors.