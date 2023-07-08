# ChankoPot #

ChankoPot is a Wpf application for configuring Windows settings.

## Operating Environment

* Windows10
* .NET 6.0.15 or later
* VisualStudio2022

## Usage

### Creating Plugin Files (.dll)

Build the "Plugin.Difinition.Basic" project within the solution. This will create the "Plugin.Difinition.Basic.dll" inside the binary folder of the "Plugin.Difinition.Basic" project. After building, "Plugin.Difinition.Basic.dll" will be automatically copied to the "plugin" folder located at the same level as the exe file of the "ChankoPot" project.

### Execution

Launch the exe file with administrator privileges. If there is no exe file, build the "ChankoPot" project.

## UI Configuration

The UI consists of two sections, each containing the following elements.

#### First Section (from left to right):
* Apply Button
* Save Button

#### Second Section (from left to right):
* SettingValueDefinition Area
* SettingValueObject Area

#### SettingValueDefinition Area
This area displays the SettingValueDefinitions included in the loaded plugins when the application starts.
#### SettingValueDefinition
It contains the names and descriptions of values. By dragging and dropping them into the SettingValueObject area, they become SettingValueObjects, allowing you to retrieve and modify the current values.

#### SettingValueObject Area
This area displays the SettingValueObjects.
#### SettingValueObject
It contains the names, descriptions, and current values of the objects. It also includes a dropdown UI for selecting the desired value. By dragging and dropping them back into the SettingValueDefinition area, they return to SettingValueDefinition.

#### Apply Button
This button applies the values to Windows. Since applying the values requires a system restart, clicking this button displays a message stating the need for a restart, along with Apply and Cancel buttons. Clicking the Apply button applies the values to Windows and performs the restart. The values applied are the ones selected in the dropdown of the SettingValueObject area.

#### Save Button
This button saves the configuration information of the SettingValueObjects in the SettingValueObject area. The saved data is written in YAML format.

## Creating New Setting Values

### SettingDefinition Class

This is a class that defines the values to be configured in this tool. By inheriting from this class, you can implement the values that can be configured in this tool. It is included in the "Plugin.Core" project.

### If using the Plugin.Difinition.Basic project

Create a new class that inherits from the SettingDefinition class within the "Plugin.Difinition.Basic" project and perform the necessary implementations. If you want to manipulate the registry, inherit from the RegistrySettingDefinition class.

### If creating a new project

Create a new project and reference the "Plugin.Core" project. (Right-click on the created project, select "Build Dependencies / Project Dependencies," and check "Plugin.Core" in the list of projects.) Create a new class that inherits from the SettingDefinition class within the new project and perform the necessary implementations.