// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CORE_INTERFACE_COREINTERFACE_H_
#define _PXZ_CORE_INTERFACE_COREINTERFACE_H_

#include "CoreTypes.h"

PXZ_MODULE_BEGIN(CoreI)

class PXZ_EXPORTED CoreInterface
{
public:
   //! Clone an entity
   /*!
     \param entity The entity to clone
     \return clonedEntity The cloned entity
   */
   static Entity cloneEntity(const Entity & entity);

   //! Delete a set of entities
   /*!
     \param entities List of entity to delete
   */
   static void deleteEntities(const EntityList & entities);

   //! Clear all the current session (all unsaved work will be lost)
   static void resetSession();

   //! Returns true if the user has made changes to the project
   /*!
     \return hasChanged Boolean checking if the project has been modified
   */
   static Bool unsavedUserChanges();

   //! Update the documentation of available functions and plugins in HTML format
   static void updateDocumentation();

   //! emitted when pixyz is closed
   static CoreI::Ident addAtExitCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeAtExitCallback(CoreI::Ident id); 

   //! emits the progress of the progress bar
   /*!
     \param progress Current progression
   */
   static CoreI::Ident addProgressChangedCallback(void(*fp)(void *, const Int &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeProgressChangedCallback(CoreI::Ident id); 

   //! emits the name of finished step
   static CoreI::Ident addProgressStepFinishedCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeProgressStepFinishedCallback(CoreI::Ident id); 

   //! emits the name of starting step
   /*!
     \param stepName Name of the started step
   */
   static CoreI::Ident addProgressStepStartCallback(void(*fp)(void *, const String &), void * userdata = nullptr);
   //! method to remove a callback event
   static void removeProgressStepStartCallback(CoreI::Ident id); 


   /**
    * \defgroup Desc Desc functions
    * @{
    */
   //! get EventDesc of an event
   /*!
     \param moduleName Target module name
     \param eventName 
     \return event 
   */
   static EventDesc getEvent(const String & moduleName, const String & eventName);

   //! get FunctionDesc of a function
   /*!
     \param moduleName Target module name
     \param functionName Target function name
     \return functionDesc 
   */
   static FunctionDesc getFunction(const String & moduleName, const String & functionName);

   //! get functions of a group
   /*!
     \param moduleName Target module name
     \param groupName Target group name
     \return functions 
   */
   static FunctionDescList getFunctions(const String & moduleName, const String & groupName);

   //! get a group desc from a specific module
   /*!
     \param moduleName Target module name
     \param groupName Target group name
     \return group 
   */
   static GroupDesc getGroup(const String & moduleName, const String & groupName);

   //! get all group desc of a module
   /*!
     \param moduleName Target module name
     \return groups 
   */
   static GroupDescList getGroups(const String & moduleName);

   //! get all modules desc
   /*!
     \return modules 
   */
   static ModuleDescList getModules();

   //! get all modules name
   /*!
     \return modulesName 
   */
   static StringList getModulesName();

   //! Ask the async EventManager to join the main thread, enableEventManagerAsync must be enable
   static void softStopAsyncEventManager();

   /**@}*/

   /**
    * \defgroup UI User Interface functions
    * @{
    */
   //! Invite the user to enter a string
   /*!
     \param msg Message to display
     \param defaultValue Message to display
     \return text The string entered by the user
   */
   static String askString(const String & msg, const String & defaultValue = "");

   //! Ask a question which need a Yes/No answer
   /*!
     \param question Question to display
     \param defaultValue Default value (if interfactive mode is disabled)
     \return answer True if the user say Yes, else False
   */
   static Boolean askYesNo(const String & question, const Boolean & defaultValue = false);

   //! Invite the user to choose one value between multiple choice
   /*!
     \param message Message to display
     \param values Possible values to choose
     \param defaultValue Default value index
     \return choice Index of the choosen value in the values list
   */
   static Int choose(const String & message, const StringList & values, const Int & defaultValue = 0);

   //! Returns True if the script is in interactive mode, else returns false
   /*!
     \return interactive True if interactive, else false
   */
   static Boolean isInteractiveMode();

   //! Display a message (or a MessageBox in GUI)
   /*!
     \param msg Message to display
   */
   static void message(const String & msg);

   //! Switch between interactive mode and non-interactive mode, UI functions will no ask user on non-interactive mode and will return default values
   /*!
     \param interactive True if you want to enter interactive mode, else False
   */
   static void setInteractiveMode(const Boolean & interactive = true);

   /**@}*/

   /**
    * \defgroup database 
    * @{
    */
   //! returns all the entities on the database
   /*!
     \return entities All entities
   */
   static EntityList getAllEntities();

   //! returns the type id of the entity
   /*!
     \param entity The wanted entity
     \return type Type id of the entity
   */
   static Int getEntityType(const Entity & entity);

   //! returns the type id of the entity
   /*!
     \param entityTypeString The wanted entity type
     \return type Type id of the entity
   */
   static Int getEntityTypeFromString(const String & entityTypeString);

   //! returns the type name of the entity
   /*!
     \param entity The wanted entity
     \return type Name of the entity type
   */
   static String getEntityTypeString(const Entity & entity);

   //! Get the database stats
   /*!
     \return stats 
   */
   static IntListList getTypeStats();

   //! Load a new scene
   /*!
     \param fileName Path to load the file
   */
   static void load(const FilePath & fileName);

   //! Save the scene
   /*!
     \param fileName Path to save the file
   */
   static void save(const OutputFilePath & fileName);

   /**@}*/

   /**
    * \defgroup function presets 
    * @{
    */
   //! Export all presets
   /*!
     \param fileName Path to save the preset file
   */
   static void exportPresets(const OutputFilePath & fileName);

   //! Import presets from file
   /*!
     \param fileName Path to the preset file to load
   */
   static void importPresets(const FilePath & fileName);

   //! Remove all presets
   static void removeAllPresets();

   /**@}*/

   /**
    * \defgroup license license utils functions
    * @{
    */
   //! Add a license token to the list of wanted optional tokens
   /*!
     \param tokenName Wanted token
   */
   static void addWantedToken(const String & tokenName);

   //! check the current license
   /*!
     \return valid 
   */
   static Boolean checkLicense();

   //! Configure the license server to use to get floating licenses
   /*!
     \param address Server address
     \param port Server port
     \param flexLM Enable FlexLM license server
   */
   static void configureLicenseServer(const String & address, const UShort & port, const Boolean & flexLM = true);

   //! Create an activation code to generate an offline license
   /*!
     \param filePath Path to write the activation code
   */
   static void generateActivationCode(const OutputFilePath & filePath);

   //! Create an deactivation code to release the license from this machine
   /*!
     \param filePath Path to write the deactivation code
   */
   static void generateDeactivationCode(const OutputFilePath & filePath);

   //! get informations on current installed license
   /*!
     \return licenseInfos 
   */
   static LicenseInfos getCurrentLicenseInfos();

   //! Get current license server
   /*!
     \return serverHost Server host name or IP
     \return serverPort Server port
     \return useFlexLM Set to true if flexLM server
   */
   static getLicenseServerReturn getLicenseServer();

   //! install a new license
   /*!
     \param licensePath Path of the license file
   */
   static void installLicense(const FilePath & licensePath);

   //! Tells if license is floating
   /*!
     \return floating 
   */
   static Bool isFloatingLicense();

   //! Get the list of actually owned license tokens
   /*!
     \return ownedTokens Owned token list
   */
   static StringList listOwnedTokens();

   //! Get the list of license tokens for this product
   /*!
     \param onlyMandatory If True, optional tokens will not be returned
     \return tokens Token list
   */
   static StringList listTokens(const Bool & onlyMandatory = false);

   //! Ensure that a license token is available, usefull to be sure to own floatting licence tokens
   /*!
     \param tokenName Token name
   */
   static void needToken(const String & tokenName);

   //! Release an optional license token
   /*!
     \param tokenName Token name
   */
   static void releaseToken(const String & tokenName);

   //! release License owned by user WEB account
   /*!
     \param login WEB account login
     \param password WEB account password
     \param id WEB license id
   */
   static void releaseWebLicense(const String & login, const Password & password, const Ident & id);

   //! remove a license token from the list of wanted optional tokens
   /*!
     \param tokenName Unwanted token
   */
   static void removeWantedToken(const String & tokenName);

   //! request License owned by user WEB account
   /*!
     \param login WEB account login
     \param password WEB account password
     \param id WEB license id
   */
   static void requestWebLicense(const String & login, const Password & password, const Ident & id);

   //! Retrieves License owned by user WEB account
   /*!
     \param login WEB account login
     \param password WEB account password
     \return licenses 
   */
   static WebLicenseInfoList retrieveWebLicenses(const String & login, const Password & password);

   //! Returns True if a token is owned by the product
   /*!
     \param tokenName Token name
     \return valid 
   */
   static Bool tokenValid(const String & tokenName);

   /**@}*/

   /**
    * \defgroup pipeline pipeline functions
    * @{
    */
   //! Return a complete output file path for Pixyz Pipeline, this function is usefull for online usage when you know where is the output directory
   /*!
     \param fileName The desired file name (suffix of the path)
     \param data Optional data assiocated with file
     \return filePath The complete generated file path
   */
   static OutputFilePath getOutputFilePath(const String & fileName, const String & data = "none");

   /**@}*/

   /**
    * \defgroup plugins plugins utils functions
    * @{
    */
   //! Execute a command
   /*!
     \param cmd Command to execute
   */
   static void executeCommand(const String & cmd);

   //! Install a new plugin
   /*!
     \param pluginFile Path to the plugin to be installed
     \param installForAllUsers If false only the current user will see the plugin installed
     \param generateDocumentation If false the documentation of the plugin is not generated
   */
   static void installPlugin(const FilePath & pluginFile, const Boolean & installForAllUsers = true, const Boolean & generateDocumentation = true);

   /**@}*/

   /**
    * \defgroup progress Progress bar tools
    * @{
    */
   //! Leave current progression level
   static void popProgression();

   //! Create a new progression level
   /*!
     \param stepCount Step count
     \param progressName Name of the progression step
   */
   static void pushProgression(const Int & stepCount, const String & progressName = "");

   //! Add a step to current progression level
   /*!
     \param stepCount Step count
   */
   static void stepProgression(const Int & stepCount = 1);

   /**@}*/

   /**
    * \defgroup properties properties related function
    * @{
    */
   //! Add a custom property to an entity that support custom properties
   /*!
     \param entity An entity that support custom properties
     \param name Name of the custom property
     \param value Value of the custom property
   */
   static void addCustomProperty(const Entity & entity, const String & name, const String & value = "");

   //! Returns the value of a module property
   /*!
     \param module Name of the module
     \param propertyName The property name
     \return propertyValue The property value
   */
   static String getModuleProperty(const String & module, const String & propertyName);

   //! Get the property value on entities (if the property is not set on an entity, defaultValue is returned)
   /*!
     \param entities List of entities
     \param propertyName The property name
     \param defaultValue Default value to return if the property does not exist on an entity
     \return values Property value on each entity
   */
   static StringList getProperties(const EntityList & entities, const String & propertyName, const String & defaultValue = "");

   //! Get a property value as String on an entity (error if the property does not exist on the entity)
   /*!
     \param entity The entity
     \param propertyName The property name
     \return value The property value as String
   */
   static String getProperty(const Entity & entity, const String & propertyName);

   //! Return true if the property was found on the occurrence, will not throw any exception except if the entity does not exist.
   /*!
     \param entity An entity that support properties
     \param propertyName Name of the property
     \return propertyFound True if the entity has the property asked, else False
   */
   static Boolean hasProperty(const Entity & entity, const String & propertyName);

   //! Returns all the properties in the given module
   /*!
     \param module Name of the module
     \return properties Properties names of the module
   */
   static StringList listModuleProperties(const String & module);

   //! Returns the name of the properties available on an entity
   /*!
     \param entity Entity to list
     \return properties Names of available properties
   */
   static StringList listProperties(const Entity & entity);

   //! Remove a custom property from an entity that support custom properties
   /*!
     \param entity An entity that support custom properties
     \param name Name of the custom property
   */
   static void removeCustomProperty(const Entity & entity, const String & name);

   //! Restore the default value of a module property
   /*!
     \param module Name of the module
     \param propertyName The property name
     \return value The property value as String
   */
   static String restoreModulePropertyDefaultValue(const String & module, const String & propertyName);

   //! Set the value of a module property
   /*!
     \param module Name of the module
     \param propertyName The property name
     \param propertyValue The property value
     \return value The property value as String
   */
   static String setModuleProperty(const String & module, const String & propertyName, const String & propertyValue);

   //! Set a property value on an entity
   /*!
     \param entity The entity
     \param propertyName The property name
     \param propertyValue The property value
     \return value The property value as String
   */
   static String setProperty(const Entity & entity, const String & propertyName, const String & propertyValue);

   //! Return true if an entity support custom properties
   /*!
     \param entity An entity
     \return support True if the entity support custom properties, else False
   */
   static Boolean supportCustomProperties(const Entity & entity);

   /**@}*/

   /**
    * \defgroup system System utility functions
    * @{
    */
   //! returns available memory
   /*!
     \return availVirt Available virtual memory in bytes
     \return totalVirt Total virtual memory in bytes
     \return availPhys Available physical memory in bytes
     \return totalPhys Total physical memory in bytes
   */
   static availableMemoryReturn availableMemory();

   //! check for software update
   /*!
     \return newVersionAvailable True if there is a new version available of this product
     \return newVersion New version value
     \return newVersionLink Link to download the new version
   */
   static checkForUpdatesReturn checkForUpdates();

   //! remove all other session temporary directories (warning: make sure that no other instance of pixyz is running
   static void clearOtherTemporaryDirectories();

   //! get the Pixyz custom version tag
   /*!
     \return customVersionTag 
   */
   static String getCustomVersionTag();

   //! get the Pixyz installation directory
   /*!
     \return installDir 
   */
   static String getInstallationDirectory();

   //! Returns the memory usage peak of the current process in MB ( only available on windows yet )
   /*!
     \return peakWorkingSet Maximum physical memory used by the process until now (in MB)
   */
   static Long getMemoryUsagePeak();

   //! get the Pixyz website URL
   /*!
     \return url 
   */
   static String getPixyzWebsiteURL();

   //! get the product documentation URL
   /*!
     \return url 
   */
   static String getProductDocumentationURL();

   //! get the Pixyz temp directory
   /*!
     \return tmpDir 
   */
   static String getTempDirectory();

   //! get the Pixyz product version
   /*!
     \return version 
   */
   static String getVersion();

   //! push custom analytic event (Only for authorized products)
   /*!
     \param name Analytic event name
     \param data Analytic event data
   */
   static void pushAnalytic(const String & name, const String & data = "");

   //! set thread
   static void setCurrentThreadAsProcessThread();

   /**@}*/

   /**
    * \defgroup undo/redo Undo/redo functions
    * @{
    */
   //! Clear undo/redo history
   static void clearUndoRedo();

   //! End current undo/redo step
   static void endUndoRedoStep();

   //! redo some steps
   /*!
     \param count 
   */
   static void redo(const UInt & count = 1);

   //! Start a new undo/redo step
   /*!
     \param stepName 
   */
   static void startUndoRedoStep(const String & stepName);

   //! Toggle undo/redo
   static void toggleUndoRedo();

   //! undo some steps
   /*!
     \param count 
   */
   static void undo(const UInt & count = 1);

   /**@}*/

   /**
    * \defgroup utils Utils functions
    * @{
    */
   //! Returns a unique color associated with an index
   /*!
     \param index Index of the color (index must be less than 2^24)
     \return color The unique color associated to the given index
   */
   static Color getColorFromIndex(const Int & index);

   /**@}*/

   /**
    * \defgroup verbose verbose functions
    * @{
    */
   //! add a console verbose level
   /*!
     \param level Verbose level
   */
   static void addConsoleVerbose(const Verbose & level);

   //! add a log file verbose level
   /*!
     \param level Verbose level
   */
   static void addLogFileVerbose(const Verbose & level);

   //! add a session log file (lastSession.log) verbose level
   /*!
     \param level Verbose level
   */
   static void addSessionLogFileVerbose(const Verbose & level);

   //! Set new configuration for the Interface Logger
   /*!
     \param enableFunction If true, the called function names will be print
     \param enableParameters If true, the called function parameters will be print (only if enableFunction=true too)
     \param enableExecutionTime If true, the called functions execution times will be print
   */
   static void configureInterfaceLogger(const Boolean & enableFunction, const Boolean & enableParameters, const Boolean & enableExecutionTime);

   //! Get the current Interface Logger configuration
   /*!
     \return functionEnabled True if the called function names are printed
     \return parametersEnabled True if the called function parameters are printed
     \return executionTimeEnabled True if the called functions execution times are printed
   */
   static getInterfaceLoggerConfigurationReturn getInterfaceLoggerConfiguration();

   //! remove a console verbose level
   /*!
     \param level Verbose level
   */
   static void removeConsoleVerbose(const Verbose & level);

   //! remove a log file verbose level
   /*!
     \param level Verbose level
   */
   static void removeLogFileVerbose(const Verbose & level);

   //! remove a session log file (lastSession.log) verbose level
   /*!
     \param level Verbose level
   */
   static void removeSessionLogFileVerbose(const Verbose & level);

   //! set the path of the log file
   /*!
     \param path Path of the log file
   */
   static void setLogFile(const OutputFilePath & path);

   /**@}*/

};

PXZ_MODULE_END



#endif
