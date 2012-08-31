# PartyCraft

PartyCraft is a 12w34b Minecraft server based on [Craft.Net](https://github.com/SirCmpwn/Craft.Net).

## Features

PartyCraft is incomplete. However, it aims to eventually reproduce the functionality of the vanilla
Minecraft server. In addition, PartyCraft should use significantly less CPU and memory on the host
machine when operating a server. For more information on progress, see the latest
[Milestone](https://github.com/SirCmpwn/PartyCraft/issues/milestones).

If there are any additional features you would like, or any problems you encounter while using
PartyCraft, please do not hesitate to [create an issue](https://github.com/SirCmpwn/Craft.Net/issues).

PartyCraft is a work in progress, and many features may be lacking. However, the current feature list
includes:

* Support for 12w34b Minecraft clients
* Support for vanilla Anvil worlds

## Usage

On Linux and Mac, you will need to install [Mono](https://github.com/mono/mono) first.

Simply run PartyCraft.exe from the command line (Linux/Mac: "mono PartyCraft.exe") to start the server.
If a world is not found in the same directory (in the "world" subdirectory by default), it will be
created. The default world generator is flatland. If an Anvil world does exist in the specified directory,
PartyCraft will use the existing world.

If a vanilla-style server.properties file is provided, PartyCraft will use it. Otherwise, a config.xml
file will be produced and populated with default values. You may use a plugin to change the settings format,
such as JSON, YML, or SQL.

After starting the server at the command line, you should be free to connect 12w34b clients to it.

## Contributing

If you wish to contribute your own code to PartyCraft, please create a fork. You are encouraged to follow the
code standards currently in use, and pull requests that do not will be rejected. You are also encouraged to
make small, focused pull requests, rather than large, sweeping changes. For such changes, it would be better
to create an issue instead.

## Getting Help

You can get help by making an [issue on GitHub](https://github.com/SirCmpwn/PartyCraft/issues), or joining
\#craft.net on irc.freenode.net. If you are already knowledgable about using PartyCraft, consider contributing
to the [wiki](https://github.com/SirCmpwn/PartyCraft/wiki) for the sake of others.

## Licensing

PartyCraft uses the permissive [MIT license](http://www.opensource.org/licenses/mit-license.php/).

In a nutshell:

* You are not restricted on usage of PartyCraft; commercial, private, etc, all fine.
* The developers are not liable for what you do with it.
* PartyCraft is provided "as is" with no warranty.

[Minecraft](http://minecraft.net/) is not officially affiliated with PartyCraft.