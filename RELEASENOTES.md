# 3.8.0

## Features:
- Logic to automatically load Steam WADs on startup
- Doom64 support and default thumbnails
- Improved text file parsing
- Auto-load Sigil II from Doom 1 + II
- Hexen/Heretic TITLEPIC support
- Nyan Doom statistics
- Load STARTUP image when no TITLEPIC/TITLE image is present
- Read author, description and IWAD from GAMECONF
- Read info from WADFILE lumps
- Streamlined tag menu, with inline toggles
- Syncing updates known IWAD titles

## Bug Fixes:
- Fix file select form resizing / color theme
- Fix metadata search for unmanaged files not working
- Fix bug where maps that were lower case in MAPINFO lumps not being read
- Show error messages when a bad response is sent from idgames API instead of showing 'No Results Found'
- Fix relative paths when selecting individual files for relative path unmanaged files
