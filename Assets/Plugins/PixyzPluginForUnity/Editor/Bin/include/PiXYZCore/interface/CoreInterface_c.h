// !!! This is a generated file, do not edit !!!
#ifndef _PXZ_CORE_INTERFACE_COREINTERFACE_C_H_
#define _PXZ_CORE_INTERFACE_COREINTERFACE_C_H_

#include "CoreTypes_c.h"

PXZ_EXPORTED char * Core_getLastError();

// Clone an entity
PXZ_EXPORTED Core_Entity Core_cloneEntity(Core_Entity entity);
// Delete a set of entities
PXZ_EXPORTED void Core_deleteEntities(Core_EntityList entities);
// Clear all the current session (all unsaved work will be lost)
PXZ_EXPORTED void Core_resetSession();
// Returns true if the user has made changes to the project
PXZ_EXPORTED Core_Bool Core_unsavedUserChanges();
// Update the documentation of available functions and plugins in HTML format
PXZ_EXPORTED void Core_updateDocumentation();
// emitted when pixyz is closed
PXZ_EXPORTED Core_Ident Core_addAtExitCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void Core_removeAtExitCallback(Core_Ident id);

// emits the progress of the progress bar
   /*!
     \param progress Current progression
   */
PXZ_EXPORTED Core_Ident Core_addProgressChangedCallback(void(*fp)(void *, Core_Int), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void Core_removeProgressChangedCallback(Core_Ident id);

// emits the name of finished step
PXZ_EXPORTED Core_Ident Core_addProgressStepFinishedCallback(void(*fp)(void *), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void Core_removeProgressStepFinishedCallback(Core_Ident id);

// emits the name of starting step
   /*!
     \param stepName Name of the started step
   */
PXZ_EXPORTED Core_Ident Core_addProgressStepStartCallback(void(*fp)(void *, Core_String), void * userdata = nullptr);
   //! method to remove a callback event
PXZ_EXPORTED void Core_removeProgressStepStartCallback(Core_Ident id);


// ----------------------------------------------------
// Desc
// Desc functions
// ----------------------------------------------------

// get EventDesc of an event
PXZ_EXPORTED Core_EventDesc Core_getEvent(Core_String moduleName, Core_String eventName);
// get FunctionDesc of a function
PXZ_EXPORTED Core_FunctionDesc Core_getFunction(Core_String moduleName, Core_String functionName);
// get functions of a group
PXZ_EXPORTED Core_FunctionDescList Core_getFunctions(Core_String moduleName, Core_String groupName);
// get a group desc from a specific module
PXZ_EXPORTED Core_GroupDesc Core_getGroup(Core_String moduleName, Core_String groupName);
// get all group desc of a module
PXZ_EXPORTED Core_GroupDescList Core_getGroups(Core_String moduleName);
// get all modules desc
PXZ_EXPORTED Core_ModuleDescList Core_getModules();
// get all modules name
PXZ_EXPORTED Core_StringList Core_getModulesName();
// Ask the async EventManager to join the main thread, enableEventManagerAsync must be enable
PXZ_EXPORTED void Core_softStopAsyncEventManager();

// ----------------------------------------------------
// UI
// User Interface functions
// ----------------------------------------------------

// Invite the user to enter a string
PXZ_EXPORTED Core_String Core_askString(Core_String msg, Core_String defaultValue);
// Ask a question which need a Yes/No answer
PXZ_EXPORTED Core_Boolean Core_askYesNo(Core_String question, Core_Boolean defaultValue);
// Invite the user to choose one value between multiple choice
PXZ_EXPORTED Core_Int Core_choose(Core_String message, Core_StringList values, Core_Int defaultValue);
// Returns True if the script is in interactive mode, else returns false
PXZ_EXPORTED Core_Boolean Core_isInteractiveMode();
// Display a message (or a MessageBox in GUI)
PXZ_EXPORTED void Core_message(Core_String msg);
// Switch between interactive mode and non-interactive mode, UI functions will no ask user on non-interactive mode and will return default values
PXZ_EXPORTED void Core_setInteractiveMode(Core_Boolean interactive);

// ----------------------------------------------------
// database
// ----------------------------------------------------

// returns all the entities on the database
PXZ_EXPORTED Core_EntityList Core_getAllEntities();
// returns the type id of the entity
PXZ_EXPORTED Core_Int Core_getEntityType(Core_Entity entity);
// returns the type id of the entity
PXZ_EXPORTED Core_Int Core_getEntityTypeFromString(Core_String entityTypeString);
// returns the type name of the entity
PXZ_EXPORTED Core_String Core_getEntityTypeString(Core_Entity entity);
// Get the database stats
PXZ_EXPORTED Core_IntListList Core_getTypeStats();
// Load a new scene
PXZ_EXPORTED void Core_load(Core_FilePath fileName);
// Save the scene
PXZ_EXPORTED void Core_save(Core_OutputFilePath fileName);

// ----------------------------------------------------
// function presets
// ----------------------------------------------------

// Export all presets
PXZ_EXPORTED void Core_exportPresets(Core_OutputFilePath fileName);
// Import presets from file
PXZ_EXPORTED void Core_importPresets(Core_FilePath fileName);
// Remove all presets
PXZ_EXPORTED void Core_removeAllPresets();

// ----------------------------------------------------
// license
// license utils functions
// ----------------------------------------------------

// Add a license token to the list of wanted optional tokens
PXZ_EXPORTED void Core_addWantedToken(Core_String tokenName);
// check the current license
PXZ_EXPORTED Core_Boolean Core_checkLicense();
// Configure the license server to use to get floating licenses
PXZ_EXPORTED void Core_configureLicenseServer(Core_String address, Core_UShort port, Core_Boolean flexLM);
// Create an activation code to generate an offline license
PXZ_EXPORTED void Core_generateActivationCode(Core_OutputFilePath filePath);
// Create an deactivation code to release the license from this machine
PXZ_EXPORTED void Core_generateDeactivationCode(Core_OutputFilePath filePath);
// get informations on current installed license
PXZ_EXPORTED Core_LicenseInfos Core_getCurrentLicenseInfos();
// Get current license server
typedef struct {
   Core_String serverHost;
   Core_UShort serverPort;
   Core_Bool useFlexLM;
} Core_getLicenseServerReturn;
PXZ_EXPORTED Core_getLicenseServerReturn Core_getLicenseServer();
// install a new license
PXZ_EXPORTED void Core_installLicense(Core_FilePath licensePath);
// Tells if license is floating
PXZ_EXPORTED Core_Bool Core_isFloatingLicense();
// Get the list of actually owned license tokens
PXZ_EXPORTED Core_StringList Core_listOwnedTokens();
// Get the list of license tokens for this product
PXZ_EXPORTED Core_StringList Core_listTokens(Core_Bool onlyMandatory);
// Ensure that a license token is available, usefull to be sure to own floatting licence tokens
PXZ_EXPORTED void Core_needToken(Core_String tokenName);
// Release an optional license token
PXZ_EXPORTED void Core_releaseToken(Core_String tokenName);
// release License owned by user WEB account
PXZ_EXPORTED void Core_releaseWebLicense(Core_String login, Core_Password password, Core_Ident id);
// remove a license token from the list of wanted optional tokens
PXZ_EXPORTED void Core_removeWantedToken(Core_String tokenName);
// request License owned by user WEB account
PXZ_EXPORTED void Core_requestWebLicense(Core_String login, Core_Password password, Core_Ident id);
// Retrieves License owned by user WEB account
PXZ_EXPORTED Core_WebLicenseInfoList Core_retrieveWebLicenses(Core_String login, Core_Password password);
// Returns True if a token is owned by the product
PXZ_EXPORTED Core_Bool Core_tokenValid(Core_String tokenName);

// ----------------------------------------------------
// pipeline
// pipeline functions
// ----------------------------------------------------

// Return a complete output file path for Pixyz Pipeline, this function is usefull for online usage when you know where is the output directory
PXZ_EXPORTED Core_OutputFilePath Core_getOutputFilePath(Core_String fileName, Core_String data);

// ----------------------------------------------------
// plugins
// plugins utils functions
// ----------------------------------------------------

// Execute a command
PXZ_EXPORTED void Core_executeCommand(Core_String cmd);
// Install a new plugin
PXZ_EXPORTED void Core_installPlugin(Core_FilePath pluginFile, Core_Boolean installForAllUsers, Core_Boolean generateDocumentation);

// ----------------------------------------------------
// progress
// Progress bar tools
// ----------------------------------------------------

// Leave current progression level
PXZ_EXPORTED void Core_popProgression();
// Create a new progression level
PXZ_EXPORTED void Core_pushProgression(Core_Int stepCount, Core_String progressName);
// Add a step to current progression level
PXZ_EXPORTED void Core_stepProgression(Core_Int stepCount);

// ----------------------------------------------------
// properties
// properties related function
// ----------------------------------------------------

// Add a custom property to an entity that support custom properties
PXZ_EXPORTED void Core_addCustomProperty(Core_Entity entity, Core_String name, Core_String value);
// Returns the value of a module property
PXZ_EXPORTED Core_String Core_getModuleProperty(Core_String module, Core_String propertyName);
// Get the property value on entities (if the property is not set on an entity, defaultValue is returned)
PXZ_EXPORTED Core_StringList Core_getProperties(Core_EntityList entities, Core_String propertyName, Core_String defaultValue);
// Get a property value as String on an entity (error if the property does not exist on the entity)
PXZ_EXPORTED Core_String Core_getProperty(Core_Entity entity, Core_String propertyName);
// Return true if the property was found on the occurrence, will not throw any exception except if the entity does not exist.
PXZ_EXPORTED Core_Boolean Core_hasProperty(Core_Entity entity, Core_String propertyName);
// Returns all the properties in the given module
PXZ_EXPORTED Core_StringList Core_listModuleProperties(Core_String module);
// Returns the name of the properties available on an entity
PXZ_EXPORTED Core_StringList Core_listProperties(Core_Entity entity);
// Remove a custom property from an entity that support custom properties
PXZ_EXPORTED void Core_removeCustomProperty(Core_Entity entity, Core_String name);
// Restore the default value of a module property
PXZ_EXPORTED Core_String Core_restoreModulePropertyDefaultValue(Core_String module, Core_String propertyName);
// Set the value of a module property
PXZ_EXPORTED Core_String Core_setModuleProperty(Core_String module, Core_String propertyName, Core_String propertyValue);
// Set a property value on an entity
PXZ_EXPORTED Core_String Core_setProperty(Core_Entity entity, Core_String propertyName, Core_String propertyValue);
// Return true if an entity support custom properties
PXZ_EXPORTED Core_Boolean Core_supportCustomProperties(Core_Entity entity);

// ----------------------------------------------------
// system
// System utility functions
// ----------------------------------------------------

// returns available memory
typedef struct {
   Core_Long availVirt;
   Core_Long totalVirt;
   Core_Long availPhys;
   Core_Long totalPhys;
} Core_availableMemoryReturn;
PXZ_EXPORTED Core_availableMemoryReturn Core_availableMemory();
// check for software update
typedef struct {
   Core_Bool newVersionAvailable;
   Core_String newVersion;
   Core_String newVersionLink;
} Core_checkForUpdatesReturn;
PXZ_EXPORTED Core_checkForUpdatesReturn Core_checkForUpdates();
// remove all other session temporary directories (warning: make sure that no other instance of pixyz is running
PXZ_EXPORTED void Core_clearOtherTemporaryDirectories();
// get the Pixyz custom version tag
PXZ_EXPORTED Core_String Core_getCustomVersionTag();
// get the Pixyz installation directory
PXZ_EXPORTED Core_String Core_getInstallationDirectory();
// Returns the memory usage peak of the current process in MB ( only available on windows yet )
PXZ_EXPORTED Core_Long Core_getMemoryUsagePeak();
// get the Pixyz website URL
PXZ_EXPORTED Core_String Core_getPixyzWebsiteURL();
// get the product documentation URL
PXZ_EXPORTED Core_String Core_getProductDocumentationURL();
// get the Pixyz temp directory
PXZ_EXPORTED Core_String Core_getTempDirectory();
// get the Pixyz product version
PXZ_EXPORTED Core_String Core_getVersion();
// push custom analytic event (Only for authorized products)
PXZ_EXPORTED void Core_pushAnalytic(Core_String name, Core_String data);
// set thread
PXZ_EXPORTED void Core_setCurrentThreadAsProcessThread();

// ----------------------------------------------------
// undo/redo
// Undo/redo functions
// ----------------------------------------------------

// Clear undo/redo history
PXZ_EXPORTED void Core_clearUndoRedo();
// End current undo/redo step
PXZ_EXPORTED void Core_endUndoRedoStep();
// redo some steps
PXZ_EXPORTED void Core_redo(Core_UInt count);
// Start a new undo/redo step
PXZ_EXPORTED void Core_startUndoRedoStep(Core_String stepName);
// Toggle undo/redo
PXZ_EXPORTED void Core_toggleUndoRedo();
// undo some steps
PXZ_EXPORTED void Core_undo(Core_UInt count);

// ----------------------------------------------------
// utils
// Utils functions
// ----------------------------------------------------

// Returns a unique color associated with an index
PXZ_EXPORTED Core_Color Core_getColorFromIndex(Core_Int index);

// ----------------------------------------------------
// verbose
// verbose functions
// ----------------------------------------------------

// add a console verbose level
PXZ_EXPORTED void Core_addConsoleVerbose(Core_Verbose level);
// add a log file verbose level
PXZ_EXPORTED void Core_addLogFileVerbose(Core_Verbose level);
// add a session log file (lastSession.log) verbose level
PXZ_EXPORTED void Core_addSessionLogFileVerbose(Core_Verbose level);
// Set new configuration for the Interface Logger
PXZ_EXPORTED void Core_configureInterfaceLogger(Core_Boolean enableFunction, Core_Boolean enableParameters, Core_Boolean enableExecutionTime);
// Get the current Interface Logger configuration
typedef struct {
   Core_Boolean functionEnabled;
   Core_Boolean parametersEnabled;
   Core_Boolean executionTimeEnabled;
} Core_getInterfaceLoggerConfigurationReturn;
PXZ_EXPORTED Core_getInterfaceLoggerConfigurationReturn Core_getInterfaceLoggerConfiguration();
// remove a console verbose level
PXZ_EXPORTED void Core_removeConsoleVerbose(Core_Verbose level);
// remove a log file verbose level
PXZ_EXPORTED void Core_removeLogFileVerbose(Core_Verbose level);
// remove a session log file (lastSession.log) verbose level
PXZ_EXPORTED void Core_removeSessionLogFileVerbose(Core_Verbose level);
// set the path of the log file
PXZ_EXPORTED void Core_setLogFile(Core_OutputFilePath path);



#endif
