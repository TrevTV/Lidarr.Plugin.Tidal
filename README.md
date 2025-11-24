# Tidal for Lidarr
This plugin provides a Tidal indexer and downloader client for Lidarr.

## Installation
This requires your Lidarr setup to be using the `plugins` branch. My docker-compose is setup like the following.
```yml
  lidarr:
    image: ghcr.io/hotio/lidarr:pr-plugins
    container_name: lidarr
    environment:
      - PUID:1000
      - PGID:1001
      - TZ:Etc/UTC
    volumes:
      - /path/to/config/:/config
      - /path/to/downloads/:/downloads
      - /path/to/music:/music
    ports:
      - 8686:8686
    restart: unless-stopped
```

To install FFMPEG with the Docker container for the conversion settings, use the following.
```yml
  lidarr:
    build:
      context: /path/to/directory/containing/dockerfile
      dockerfile: Dockerfile
    container_name: lidarr
    environment:
      - PUID:1000
      - PGID:1001
      - TZ:Etc/UTC
    volumes:
      - /path/to/config/:/config
      - /path/to/downloads/:/data/downloads
      - /path/to/tidal/config/:/data/tidal-config
      - /path/to/music:/data/music
    ports:
      - 8686:8686
    restart: unless-stopped
```

```Dockerfile
FROM ghcr.io/hotio/lidarr:pr-plugins

RUN apk add --no-cache ffmpeg
```

1. In Lidarr, go to `System -> Plugins`, paste `https://github.com/TrevTV/Lidarr.Plugin.Tidal` into the GitHub URL box, and press Install.
2. Go into the Indexer settings and press Add. In the modal, choose `Tidal` (under Other at the bottom).
3. Enter a path to use to store user data, press Test, it will error, press Cancel.
4. Refresh the page, then re-open the Add screen and choose Tidal again.
5. There should now be a `Tidal URL` setting with a URL in it. Open that URL in a new tab.
6. In the new tab, log in to Tidal, then press `Yes, continue`. It will then bring you to a page labeled "Oops." Copy the new URL for that tab (something like `https://tidal.com/android/login/auth?code=[VERY LONG CODE]`).
   - Do NOT share this URL with people as it grants people access to your account.
   - Redirect URLs are NOT reusable. If you need to sign in again, make sure to use a new Tidal URL from the settings, they are regenerated semi-often.
7. Enter a path to use to store user data and paste the copied Tidal URL into the `Redirect Url` option. Then press Save.
8.  Go into the Download Client settings and press Add. In the modal, choose `Tidal` (under Other at the bottom).
9.  Put the path you want to download tracks to and fill out the other settings to your choosing.
   - If you want `.lrc` files to be saved, go into the Media Management settings and enable Import Extra Files and add `lrc` to the list.
   - Make sure to only enable the FFMPEG settings if you are sure that FFMPEG is available to Lidarr, it may cause issues otherwise.
10. Go into the Profile settings and find the Delay Profiles. On each (by default there is only one), click the wrench on the right and toggle Tidal on.
11. Optional: To prevent Lidarr from downloading all track files into the base artist folder rather than into their own separate album folder, go into the Media Management settings and enable Rename Tracks. You can change the formats to your liking, but it helps to let each album have their own folder.

## Known Issues
- Search results provide an estimated file size instead of an actual one
- User access tokens are stored in a separate folder even though (I think) Lidarr has a system to store it available to plugins

## Licensing
All of these libraries have been merged into the final plugin assembly due to (what I believe is) a bug in Lidarr's plugin system.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) is licensed under the MIT license. See [LICENSE](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md) for the full license.
- [TagLibSharp](https://github.com/mono/taglib-sharp) is licensed under the LGPL-2.1 license. See [COPYING](https://github.com/mono/taglib-sharp/blob/main/COPYING) for the full license.
- [TidalSharp](https://github.com/TrevTV/TidalSharp) is licensed under the GPL-3.0 license. See [LICENSE](https://github.com/TrevTV/TidalSharp/blob/main/LICENSE) for the full license.
