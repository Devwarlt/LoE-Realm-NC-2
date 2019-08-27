# How to setup game to local access (LAN), via Hamachi network (VPN) or via public network (DNS or fixed IP from Workspace, VPS or even Dedicated Server hosting)?
You must follow few steps along this guide to properly connect to the game using client or even allow other people to connect into your game server (if using VPN, VPS, Dedicated Server or Workspace hosting). Therefore, let's begin the guide!

**Note:** current project source code need some adjusts, because it does contains same parameters used on last production mode of running version before deprecation of support along further builds by our team.

## Client
Navigate to `client/src/com/company/assembleegameclient/parameters/Parameters.as`. There you'll find whole project configuration of client side such as current version, client name, ports et cetera. Therefore, let's understand which lines you can change for setup:

> [IS_DEVELOPER_MODE](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L16): this parameter is used to toggle setup of client for local or production mode along building.

> [DISCORD_PERMANENTLY_INVITE](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L17): this parameter is used to setup permanently Discord invitation link for your server. You can see a Discord icon once player connects to Nexus below star's icon.

> [CONNECTION_SECURITY_PROTOCOL](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L18): this parameter is used only if you are using SSL certifcate, not even required if you are hosting your game client at GitHub pages, because it's not needed.

> [CLIENT_NAME](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L19): this parameter is used to display the name of your game client and will shown at left corner of main screen when game is initialized and when player connects in game at begining on console log.

> [ENVIRONMENT_DNS](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L22): this parameter is used for quick setup of current environment DNS for your server, by default you'll see following configuration: "public static const ENVIRONMENT_DNS:String = !IS_DEVELOPER_MODE ? "loe-nc.portmap.io" : "localhost";", where "loe-nc.portmap.io" shall be replaced by your DNS / fixed IP / Hamachi IPv4 Address and second parameter "localhost" will only be needed to be updated if your local setup is not validating it properly, so you can change it to "127.0.0.1" barely, so must keep it by default as "localhost".

> [ENVIRONMENT_PORT](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L23): this parameter is the port used to connect to your appengine application (aka webserver), you'll find it set in other PServer sources with port "8080". However, you can change it to whatever port you want, but as **recommendation set it to value "8080"**.

> [BUILD_VERSION](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L24): this parameter represents main version of game.

> [MINOR_VERSION](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L25): this parameter represents minor and build versions of game, make sure to validate game version on client at server side along its configuration otherwise players cannot connect to the game.

> [FULL_BUILD](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L26): this parameter represents format of "BUILD_VERSION" and "MINOR_VERSION" parameters.

> [PORT](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L28): this parameter is the port used to connect to your game server application (aka wServer), you'll find it set in other PServer sources with port "2050". However, you can change it to whatever port you want, but as **recommendation set it to value "2050"**.

> [POLICY](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/client/src/com/company/assembleegameclient/parameters/Parameters.as#L29): this parameter is the port used to connect to your policy server at game server application, you'll find it set in other PServer sources with port "843" which is a default port accordlying AS3 API reference. However, you can change it to whatever port you want, but as **recommendation set it to value "843"**.

**Note:** you can change other parameters at this file, but make sure to know what are you doing to avoid further mistakes along building and deployment of your game client.

## Server
On server side, you need to do more more steps.

Navigate to file `server/common/config/Settings.cs`:

> [EVENT_RATE](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L14): this parameter is used to set as global factor, e.g. used to multiply all loot drop rate to its value (this parameter effect EXP, loot drop chance and other features, must check global usages for more details at solution).

> [EVENT_OVER](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L15): this parameter is used to automatically turns off running event affected by "EVENT_RATE". If declaration is valid and doesn't match with current UTC then all players (when connect to Nexus) will receive a notification about running event, e.g. "public static readonly DateTime EVENT_OVER = new DateTime(2019, 2, 4, 23, 59, 59);" means the event will over at year 2019, month 2, day 4, hour 23, minute 23 and second 59 based on UTC.

> [SERVER_MODE](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L21): this parameter is used for toggle setup between two possible modes of game ("Local" or "Production"), so if you want game to being accessed on local environment must setup with "Local" or if you are looking for public access set to "Production".

> [RESTART_DELAY_MINUTES](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L23): this parameter represents delay in minutes until next restart of game server application (only works on Windows). This feature is only enabled in "Production" mode.

> [RESTART_APPENGINE_DELAY_MINUTES](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L24): this parameter represents delay in minutes until next restart of appengine application (only works on Windows). This feature is only enabled in "Production" mode.

> [STARTUP](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L29-L39): this parameter represents startup parameters for "GOLD", "FAME", "TOTAL_FAME", "TOKENS" (aka Fortune Tokens, but disabled), "EMPIRES_COIN" (a currency used to purchase items at Game Store), "MAX_CHAR_SLOTS", "IS_AGE_VERIFIED" and "VERIFIED".

> [GAME_VERSION](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/Settings.cs#L41-L71): this parameter  represents a list of supported or not supported game versions, e.g. if you want to grant support for a hypothetical version "1.2.3" then you need to add into that list following declaration "new GameVersion(Version: "1.2.3", Allowed: true)" and to revoke must remove this item from list or assign into parameter "Allowed" value "false".

Navigate to file `server/common/config/internal/AppEngine.cs`:

> [PRODUCTION_PORT](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/AppEngine.cs#L16): this parameter is the port used to listen for new connections on appengine application (shall be the same port of client set on parameter ENVIRONMENT_PORT).

> [SERVERS](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/AppEngine.cs#L18-L20): this parameter represents a list of available servers where player will be able to select at Server screen on client. Make sure to change definition of this declaration to your own setup, e.g. "("Chicago", "loe-nc.portmap.io")" where "Chicago" means the name of server and "loe-nc.portmap.io" means the DNS / fixed IP / Hamachi IPv4 Address used to connect via client.

> [SAFE_DOMAIN](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/AppEngine.cs#L22): this parameter represents what is the domain used to lock your game client and validate incoming connections. In other words, which URL your game client need to be hosted to let guests and players to play your game. I recommend to use GitHub pages for its setup, take a look at this version of template at https://devwarlt.github.io/ (source code open sourced at Github on following link: https://github.com/Devwarlt/devwarlt.github.io ).

Navigate to file `server/common/config/internal/GameServer.cs`:

> [GAME_PORT](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/GameServer.cs#L9): this parameter is the port used to listen for new connections on game server application (shall be the same port of client set on parameter PORT).

Navigate to file `server/common/config/internal/Networking.cs`:

> [MAX_CONNECTIONS](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/Networking.cs#L11): this parameter is the maximum amount of connections that will be supported on your game server.

Navigate to file `server/common/config/internal/PolicyServer.cs`:

> [PORT](https://github.com/Devwarlt/LoE-Realm-NC-2/blob/production/server/common/config/internal/PolicyServer.cs#L7): this parameter is the port used to listen for new connections on Policy server via game server application (shall be the same port of client set on parameter POLICY).

Navigate to file `server/_appengine.bat` and change value "5555" to the same value set on ENVIRONMENT_PORT and at PRODUCTION_PORT parameters.

Navigate to file `server/_gameserver.bat` and change value "2050" to the same value set on PORT and at GAME_PORT parameters.

Therefore once you compile your game client and release it on your own GitHub page, then restore Nuget Packages on your solution project and build your server. For quick run of your server, must open file `server/_storage_engine.bat` and then `server/_initialize_game.bat`. Enjoy your own PServer with your friends!

Any question you can contact me through Discord at following links:
http://bit.ly/loesoft-discord or adding me as friend with my ID `Devwarlt#8483`.
