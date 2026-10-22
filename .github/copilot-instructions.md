# GitHub Copilot Instructions for ImpassableFurniture (Continued) Mod

## Mod Overview and Purpose

**Mod Name:** ImpassableFurniture (Continued)  
**Author:** erdelf  
**PackageId:** Mlie.ImpassableFurniture  

ImpassableFurniture is a mod designed for RimWorld that allows players to configure furniture items as impassable. It builds upon the original mod by erdelf, adding new functionalities and maintaining compatibility with most other mods. The default setting is for no furniture to be impassable, but players can adjust settings as desired. Transforming items like beds and chairs into impassable objects can significantly alter game strategies, affecting the player's colony and their enemies.

## Key Features and Systems

- **Dynamic Passability:** The mod searches the entire database of furniture and provides an option to toggle their passability.
- **Mod Settings Buttons:** Includes user-friendly settings interface with "Add all", "Remove all", and "Reset to default" buttons to facilitate mass changes to furniture passability.
- **Broad Compatibility:** Designed to work seamlessly with most existing mods.
- **Strategic Gameplay Changes:** Impassable furniture can block paths for both colonists and enemies, impacting base layout planning.

## Coding Patterns and Conventions

- **C# Code Structure:** The mod consists of three main C# files: `ImpassableSettings.cs`, `ImpassableFurniture.cs`, and `ImpassableFurnitureMod.cs`.
- **Type and Member Definitions:**
  - **ImpassableSettings.cs:** Manages mod settings with the `ExposeData` function.
  - **ImpassableFurniture.cs:** Core implementation handling functions like `AddPassability`, `RemovePassability`, and `regenerate`.
  - **ImpassableFurnitureMod.cs:** Deals with rendering the mod's settings window and its integration into RimWorld's mod menu with functions such as `DoSettingsWindowContents` and `SettingsCategory`.

## XML Integration

Currently, the mod does not explicitly rely on XML files for its configuration or definitions, focusing instead on robust C# implementation to manage its settings and behaviors.

## Harmony Patching

- **Purpose of Harmony Patching:** Provides seamless integration and modification of existing methods without altering the original game files directly.
- **Implementation Recommendations:** When employing Harmony patches, ensure they are efficient and avoid conflicts by targeting specific methods related to furniture passability.

## Suggestions for Copilot

1. **Automated Code Suggestions:** Use Copilot to generate additional settings menu options or enhance logging functionality.
2. **Performance Optimization:** Leverage Copilot to suggest code improvements for scanning the furniture database efficiently.
3. **Compatibility Checks:** Implement helper functions suggested by Copilot to test compatibility with new RimWorld updates or other popular mods.
4. **Enhanced Debugging:** Utilize Copilot to create better error handling and logging mechanisms to assist in troubleshooting and mod support.

By following these guidelines and utilizing GitHub Copilot effectively, developers can maintain and expand the ImpassableFurniture mod, ensuring it remains a valuable asset for the RimWorld modding community.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

