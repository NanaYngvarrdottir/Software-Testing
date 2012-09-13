<<<<<<< HEAD
Virtual Realtiy Software:0.1.1.9 Dev:0.5.1
*The Virtual Reality Development Team is proud to present the release of Virtual Reality Software:0.1.1.3 Dev:0.5.1 virtual world server.
=======
#                                        Virtual Realtiy Software:0.1.1.3 Dev:0.5.1

*The Virtual Reality Development Team is proud to present the release of Virtual Reality Software:0.1.1.3 Dev:0.5.1 virtual world server.

*The Virtual Reality Software is an Aurora-Sim derived project with heavy emphasis on supporting all users, 
increased technology focus, heavy emphasis on working with other developers, whether it be viewer based developers or server based developers, 
and a set of features around stability and simplified usability for users.

*We aren’t just releasing new features, but a new outlook on Virtual Reality Virtual World Technology Development for the average human.


*Virtual Reality is an experimental and innovative framework containing advanced tools and options for creating virtual world applications. 

Virtual Reality is not a virtual world, nor a stand-alone application, it is a scalable and customizable platform containing some basic modules,
based on some fundamental innovative pillars (peer-to-peer architecture, secure communication infrastructure, legal framework, 
powerful scripting language); additional modules, extensions and expansion packs can be built on top of it on demand.

*The core of Virtual Reality is the innovative Virtual Reality Engine based on a hybrid peer-to-peer infrastructure that allows the sharing of
computational load in experiencing the virtual environment obtaining infrastructural resource optimization and bandwidth reduction. 

It enhances the platform in terms of robustness, availability, scalability, load balancing.

From an higher level point of view, Virtual Reality is provided with a Legal Framework that allows to conclude deals and to carry out transactions
directly in-world. To obtain this aim, Virtual Reality is integrated with a secure communication infrastructure based on a strong identity system 
that bounds the avatar to the real identity of the person behind, that is responsible of his actions as in real life. In case of infringement of 
the contract terms, an in-world dispute resolution is implemented within the platform The design of such system carefully mimics the Model Agreement 
provided by The International Mediation Institute, www.IMImediation.org.



##                                        The main Science & Technology objectives of the project:



###                                       To create a scalable, reliable p2p architecture for a 3d environment: 

*Two innovative custom libraries have been developed, namely vrnet and vrengine that allow 
the creation of a coherent world on top of a overlay network leveraging on a newly introduced authority mechanism;



####                                       To create a secure and trusted infrastructure and a certified authentication system: 

*It allows end-to-end security in communications between either users or participating virtual nodes; this layer is built upon PKI and stresses the role 
of mutually entrusted communication streams as well as digital signature of documents and proof of identity;


#####                                      To implement a virtual law system: 

*Leveraging the platform built-in strong identity, users are allowed to carry out transactions directly in-world (contracts and online dispute resolution system) 
and to participate to its ruling via customizable Constitution and Law System;


######                                     To create a powerful scripting engine: 

*Allowing the users to interact with the virtual world. Scripting has been targeted toward programmers, privileging power over ease-of-use;



*This innovative approach has received some encouraging feedbacks from private universities and training companies, so that a specific applicative scenario has been customized for educational purposes (Virtual Campus). 

The Virtual Campus scenario takes also advantage from the interactivity enabled by the Virtual Reality scripting engine. From the legal side, some Contracts template specifically targeted to the relationships existing among private universities, teachers and students, cover the main use cases.

Moreover, after a careful examination of potential business applications, thanks to the powerful scripting engine has been possible to obtain a specific applicative
scenario (industrial scenario) aimed, for example, to provide a testing platform for streetcars/railway designers/manufacturers. The project has devoted great 
attention also on dissemination and exploitation activities; more than thirty publications have been written and more than one hundred fifty potential stakeholders 
have been contacted in order to build a business relationship.

Platform Validation has demonstrated that Virtual Reality offers the architectural support for adoption of the platform, whether in autonomous or assisted usage, in educational and industrial application domains (i.e. Virtual Campus and Street-cars).

The validation highlighted some improvements area, in particular with respect to the usability and 3D interaction mode. Notwithstanding this finding, the users in both 
the domains were able to perform the planned scenarios and the validation exercises.

Virtual Reality demonstrates to be fully compliant with the requirements provided by the pool of involved users. As for Virtual Campus the platform proved to enhance teachers and students to carry out their duties. In particular the possibility to visualize complex objects, as well as their exploration, and the persistence of the 
virtual world, were assessed as valuable features to support the educational scenarios.



The experimental results demonstrated that P2P framework effectively balances richness, responsiveness, robustness to provide an enhanced user experience to what is 
possible on traditional client/server architectures on regular Internet connections. In addition, our P2P framework reduces the requirements of bandwidth and CPU 
power on the service provider of a Virtual Reality-based virtual world service, and hence reduces the total cost of ownership.




#######                                         Compiling Virtual Reality

*To compile Virtual Reality, look at BUILDING.txt for more information.*

## Virtuail Reality Configuration
*Configuration in Virtual Reality is a bit different than in other distributions of Aurora-Sim.*

In Virtual Reality, all configuration files (except for AuroraServer.ini and Aurora.ini) have been moved to the AuroraServerConfiguration and Configuration folders (respectively) and further subdivided from there into categories of .ini files.

### Virtual Reality Configuration rundown

*Note - All file paths are from the Configuration directory*


#### Configuration folder

*	Main.ini -  Contains the settings to switch between standalone, grid mode, and Simian.


#### Data folder

*	Data.ini -  Settings to switch between database modules
*	MySQL.ini -  Settings for the MySQL Database.
*	SQLite.ini -  Settings for the SQLite Database.


#### Grid folder

*	AuroraGridCommon.ini -  Contains files that are used in grid mode.


#### Modules folder

*	Advanced.ini -  Contains settings including the prioritizer, LLUDP server settings, and packet pool.
*	AssetCache.ini - Contains the asset cache settings.
*	AuroraModules.ini -  Contains all the settings for the Aurora Modules, including the Aurora Profile and Search plugins, Map Module, World Terrain settings, Chat, Messaging, Display Names, and more.
*	Concierge.ini -  Contains the concierge chat plugin settings.
*	Economy.ini -  Contains settings that have to do with the economy module in Virtual Reality.
*	Groups.ini -  Contains all group module configuration.
*	InstanceSettings.ini -  Contains settings that have to deal with the OpenRegionSettings module.
*	IRC.ini -  Contains settings that deal with the IRC chat module connector.
*	Nature.ini -  Contains cloud, wind, sun, and tree settings.
*	Permissions.ini -  This sets up the permission modules and who can create scripts.
*	Protection.ini -  This deals with banning, restarting Aurora automatically, auto OAR backup, and more.
*	RemoteAdmin.ini -  This contains all the Remote Admin settings.
*	Search.ini -  This configures the DataSnapshot module which is used for alternative search plugins.
*   StarDust.ini - Contains Virtual Reality Economy Settings.
*   StarDustCollections.ini -Contains Virtual Reality Economy Collections Settings.
*	SMTPEmail.ini -  This sets up the Email module.
*	Startup.ini -  This contains settings including Default object name, checking for updates, error reporting, MegaRegions, Persistence, and Animations.
*	Stats.ini -  Sets up the optional stats module.
*	VoiceModules.ini -  Contains the voice modules configurations.
*   Vivox.ini - Contains the Virtual Reality vivox voice configurations.


#### Physics folder

*	Physics.ini -  Contains settings to enable/disable physics plugins.
*	Meshing.ini -  Contains meshing settings.

#### Scripting folder
*	Scripting.ini -  Contains settings to enable/disable scripting plugins.
*	MRM.ini -  Contains MRM settings.
*	AuroraDotNetEngine.ini -  Contains ADNE (the script engine) settings.


#### Standalone folder

*	StandaloneCommon.ini -  Contains files that are used in Standalone mode.

### Standalone Configuration

To run Virtual Reality, you will need to edit a config file for it to run. This file is:
>>>>>>> VirtualReality/SoftwareTesting

*The Virtual Reality Software is an Aurora-Sim derived project with heavy emphasis on supporting all users, increased technology focus, heavy emphasis on working with other developers, whether it be viewer based developers or server based developers, and a set of features around stability and simplified usability for users.

*We aren’t just releasing new features, but a new outlook on Virtual Reality Virtual World Technology Development for the average human.

*Virtual Reality is an experimental and innovative framework containing advanced tools and options for creating virtual world applications. 

Virtual Reality is not a virtual world, nor a stand-alone application, it is a scalable and customizable platform containing some basic modules, based on some fundamental innovative pillars (peer-to-peer architecture, secure communication infrastructure, legal framework, powerful scripting language); additional modules, extensions and expansion packs can be built on top of it on demand.

*The core of Virtual Reality is the innovative Virtual Reality Engine based on a hybrid peer-to-peer infrastructure that allows the sharing of computational load in experiencing the virtual environment obtaining infrastructural resource optimization and bandwidth reduction. 

It enhances the platform in terms of robustness, availability, scalability, load balancing.

From an higher level point of view, Virtual Reality is provided with a Legal Framework that allows to conclude deals and to carry out transactions directly in-world. To obtain this aim, Virtual Reality is integrated with a secure communication infrastructure based on a strong identity system that bounds the avatar to the real identity of the person behind, that is responsible of his actions as in real life. In case of infringement of the contract terms, an in-world dispute resolution is implemented within the platform The design of such system carefully mimics the Model Agreement provided by The International Mediation Institute, www.IMImediation.org.

The main Science & Technology objectives of the project:
To create a scalable, reliable p2p architecture for a 3d environment:
*Two innovative custom libraries have been developed, namely vrnet and vrengine that allow the creation of a coherent world on top of a overlay network leveraging on a newly introduced authority mechanism;

To create a secure and trusted infrastructure and a certified authentication system:
*It allows end-to-end security in communications between either users or participating virtual nodes; this layer is built upon PKI and stresses the role of mutually entrusted communication streams as well as digital signature of documents and proof of identity;

To implement a virtual law system:
*Leveraging the platform built-in strong identity, users are allowed to carry out transactions directly in-world (contracts and online dispute resolution system) and to participate to its ruling via customizable Constitution and Law System;

To create a powerful scripting engine:
*Allowing the users to interact with the virtual world. Scripting has been targeted toward programmers, privileging power over ease-of-use;

<<<<<<< HEAD
*This innovative approach has received some encouraging feedbacks from private universities and training companies, so that a specific applicative scenario has been customized for educational purposes (Virtual Campus). 
=======
*	You will need to modify the [Network] section, specifically the HostName parameter.
*	By default, the line is "HostName = http://127.0.0.1" and if you want to allow other users besides yourself to connect to your instance, you will need to 
change this to your external IP, which can be found  at http://www.whatsmyip.org/.
>>>>>>> VirtualReality/SoftwareTesting

The Virtual Campus scenario takes also advantage from the interactivity enabled by the Virtual Reality scripting engine. From the legal side, some Contracts template specifically targeted to the relationships existing among private universities, teachers and students, cover the main use cases.

<<<<<<< HEAD
Moreover, after a careful examination of potential business applications, thanks to the powerful scripting engine has been possible to obtain a specific applicative scenario (industrial scenario) aimed, for example, to provide a testing platform for streetcars/railway designers/manufacturers. The project has devoted great attention also on dissemination and exploitation activities; more than thirty publications have been written and more than one hundred fifty potential stakeholders have been contacted in order to build a business relationship.

Platform Validation has demonstrated that Virtual Reality offers the architectural support for adoption of the platform, whether in autonomous or assisted usage, in educational and industrial application domains (i.e. Virtual Campus and Street-cars).

The validation highlighted some improvements area, in particular with respect to the usability and 3D interaction mode. Notwithstanding this finding, the users in both the domains were able to perform the planned scenarios and the validation exercises.

Virtual Reality demonstrates to be fully compliant with the requirements provided by the pool of involved users. As for Virtual Campus the platform proved to enhance teachers and students to carry out their duties. In particular the possibility to visualize complex objects, as well as their exploration, and the persistence of the virtual world, were assessed as valuable features to support the educational scenarios.

The experimental results demonstrated that P2P framework effectively balances richness, responsiveness, robustness to provide an enhanced user experience to what is possible on traditional client/server architectures on regular Internet connections. In addition, our P2P framework reduces the requirements of bandwidth and CPU power on the service provider of a Virtual Reality-based virtual world service, and hence reduces the total cost of ownership.

# Compiling Virtual Reality
To compile Virtual Reality, look at BUILDING.txt for more information.

Virtuail Reality Configuration
Configuration in Virtual Reality is a bit different than in other distributions of Aurora-Sim.

In Virtual Reality, all configuration files (except for AuroraServer.ini and Aurora.ini) have been moved to the AuroraServerConfiguration and Configuration folders (respectively) and further subdivided from there into categories of .ini files.

Virtual Reality Configuration rundown
Note - All file paths are from the Configuration directory

Configuration folder
•Main.ini - Contains the settings to switch between standalone, grid mode, and Simian.
Data folder
•Data.ini - Settings to switch between database modules
•MySQL.ini - Settings for the MySQL Database.
•SQLite.ini - Settings for the SQLite Database.
Grid folder
•AuroraGridCommon.ini - Contains files that are used in grid mode.
Modules folder
•Advanced.ini - Contains settings including the prioritizer, LLUDP server settings, and packet pool.
•AssetCache.ini - Contains the asset cache settings.
•AuroraModules.ini - Contains all the settings for the Aurora Modules, including the Aurora Profile and Search plugins, Map Module, World Terrain settings, Chat, Messaging, Display Names, and more.
•Concierge.ini - Contains the concierge chat plugin settings.
•Economy.ini - Contains settings that have to do with the economy module in Virtual Reality.
•Groups.ini - Contains all group module configuration.
•InstanceSettings.ini - Contains settings that have to deal with the OpenRegionSettings module.
•IRC.ini - Contains settings that deal with the IRC chat module connector.
•Nature.ini - Contains cloud, wind, sun, and tree settings.
•Permissions.ini - This sets up the permission modules and who can create scripts.
•Protection.ini - This deals with banning, restarting Aurora automatically, auto OAR backup, and more.
•RemoteAdmin.ini - This contains all the Remote Admin settings.
•Search.ini - This configures the DataSnapshot module which is used for alternative search plugins.
•StarDust.ini - Contains Virtual Reality Economy Settings.
•StarDustCollections.ini -Contains Virtual Reality Economy Collections Settings.
•SMTPEmail.ini - This sets up the Email module.
•Startup.ini - This contains settings including Default object name, checking for updates, error reporting, MegaRegions, Persistence, and Animations.
•Stats.ini - Sets up the optional stats module.
•VoiceModules.ini - Contains the voice modules configurations.
•Vivox.ini - Contains the Virtual Reality vivox voice configurations.
Physics folder
•Physics.ini - Contains settings to enable/disable physics plugins.
•Meshing.ini - Contains meshing settings.
Scripting folder
•Scripting.ini - Contains settings to enable/disable scripting plugins.
•MRM.ini - Contains MRM settings.
•AuroraDotNetEngine.ini - Contains ADNE (the script engine) settings.
Standalone folder
•StandaloneCommon.ini - Contains files that are used in Standalone mode.
Standalone Configuration
To run Virtual Reality, you will need to edit a config file for it to run. This file is:

bin/Aurora.ini

•You will need to modify the [Network] section, specifically the HostName parameter.
•By default, the line is "HostName = http://127.0.0.1" and if you want to allow other users besides yourself to connect to your instance, you will need to change this to your external IP, which can be found at http://www.whatsmyip.org/.
Starting Virtual Reality
To Start the virtual world part of Virtual Reality, just double click on Aurora.exe. It will run for a bit until you come to a screen that will help you interactively configure your new region.

Connecting to Virtual Reality
To connect to the simulator with Angstrom, you must add a new grid to the Grid Manager.

1.Click on the Grid Manager button, and in the new popup, click "Add".
2.In the loginURI space, put "http://<IP>:9000/" where "<IP>" is your IP.
3.If you arn't sure what your IP is, you can check at http://www.whatsmyip.org/. 
4.After this, set a name for your grid in the Grid Name area.
5.Then press apply and close the box. Select the grid you just created and then login with your username and password and enjoy!
Router issues
=======
## Starting Virtual Reality

To Start the virtual world part of Virtual Reality, just double click on Aurora.exe. It will run for a bit until you come to a screen that will help you interactively configure your new region.

## Connecting to Virtual Reality

To connect to the simulator with Angstrom, you must add a new grid to the Grid Manager.

1.	Click on the Grid Manager button, and in the new popup, click "Add".
2.	In the loginURI space, put "http://<IP>:9000/" where "<IP>" is your IP.
3.	If you arn't sure what your IP is, you can check at http://www.whatsmyip.org/. 
4.	After this, set a name for your grid in the Grid Name area.
5.	Then press apply and close the box. Select the grid you just created and then login with your username and password and enjoy!

## Router issues

>>>>>>> VirtualReality/SoftwareTesting
If you are having issues logging into your simulator, take a look at http://forums.osgrid.org/viewtopic.php?f=14&t=2082 in the Router Configuration section for more information on ways to resolve this issue.
