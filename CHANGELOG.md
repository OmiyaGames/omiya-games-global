# Change Log:

## 1.4.0

- **New Feature:** added static helper class, [`Manager`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/Manager.cs): handles starting and stopping coroutines for any non-`MonoBehaviour` scripts.  Also has events for `Update`, `LateUpdate`, and `FixedUpdate`.

## 1.3.0

 - **New Enhancement:** allowing one to edit [`SettingsEditor`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Editor/SettingsEditor.cs) by overriding the `UxmlPath` property.
 
## 1.2.0

- **New Feature:** added abstract class, [`SettingsEditor`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Editor/SettingsEditor.cs).  This editor simply displays one button, prompting the user to open the Project Settings window instead.
- **New Feature:** added abstract class, [`SettingsPropertyDrawer`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Editor/SettingsPropertyDrawer.cs)  This `PropertyDrawer` automatically calls `Reset(SerializedProperty)` as soon as the user sees the serialized variable in the inspector.  Provides an opportunity for the developer to replace the variable with an existing asset in the project.
- **Bug Fix:** fixed [`ComponentSingleton`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/ComponentSingleton.cs) to actually work on runtime.

## 1.1.0

- **New Feature:** added [`ComponentSingleton`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/ComponentSingleton.cs), which creates a singleton instance of a component!

## 1.0.0

- Marking this package as stable, given it's relatively small footprint and unlikeliness to cause problems.

## 0.1.2-preview.2

- **New Enhancement:** upgrading the assembly definitions and package files.

## 0.1.2-preview.1

- **New Enhancement:** added [`SettingsHelpers`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Editor/SettingsHelpers.cs)

## 0.1.1-preview.1

- Updating version of each dependency libraries.
- Updating [`Singleton`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/Singleton.cs) to compile with latest dependency changes.

## 0.1.0-preview.1

- Initial release
- New Feature: added [`Singleton`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/Singleton.cs)
- New Feature: added [`ISingletonScript`](https://github.com/OmiyaGames/omiya-games-global/blob/main/Runtime/ISingletonScript.cs)
