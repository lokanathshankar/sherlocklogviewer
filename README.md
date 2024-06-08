# Sherlock Log Viewer

Hello there! Welcome to Sherlock Log Viewer, your go-to solution for analyzing logs. It effortlessly detects and parses your logs automatically, saving you time and effort. With Sherlock Log Viewer, you can seamlessly apply advanced filtering and access sophisticated reporting features with ease. Best of all, it's cross-platform, available for Windows, Linux, and Mac.

## Setting up Sherlock Log Viewer

Getting up and running with Sherlock Log Viewer is quick and easy. It's a small download, so you can install it in a matter of minutes and start analyzing logs right away.

### Cross-Platform

Sherlock Log Viewer runs on macOS, Linux, and Windows operating systems. Follow the steps below to install it on your respective OS:

#### Sherlock Log Viewer on Windows

**Installation Steps:**
1. Download the Sherlock Log Viewer installer for Windows.
2. Run the installer. This will only take a minute.
3. Start analyzing your logs!

#### Sherlock Log Viewer on Linux

**Installation Steps:**
1. Download the Sherlock Log Viewer installer for Linux.
2. Extract the `SherlockLogViewer.zip` and double-click the Sherlock Log Viewer executable inside the extracted directory.
3. Start analyzing your logs!

## Loading a Log File

Sherlock Log Viewer can load many log formats. The built-in auto-detection feature will parse most log files automatically. However, in some cases, additional steps are required to properly format the logs. More details are provided below.

### Automatic Loading

This workflow is sufficient for most structured logs (e.g., delimited, JSON, XML, Windows Events (EVTX)). Follow these primary steps to open any log file:

1. Press "Start" from the top menu, then navigate to "Load Log File..." to open the file explorer.
2. Select the required file for investigation. Confirm the Auto Detection popup by pressing "Yes." This will load the logs into the table after parsing.

### Semi-Automatic Loading

This workflow is useful for partially structured logs (e.g., logs with columns separated by spaces, commas, etc.). Start by loading the log file using the "Start" menu. This should partially load the logs into the table. Then follow these steps to sanitize the logs:

1. Navigate to "Session Preference" from the top menu and press "Parser Config." This brings up a dialog with the auto-detected columns. The "Syntax List" is where all the columns are defined.
2. Edit the syntax list by pressing the "Edit" icon. This will bring up "Edit Syntax." Optionally, you can rename the parser.
3. Identify and remove excess columns using the "Delete" icon. Optionally, rename the columns. Once satisfied, confirm the changes in "Edit Syntax" and "Parser Config."
4. Reload the log file using the "Start" menu again. This time, the logs should be properly formatted.

### Manual Loading

This workflow is rarely used but necessary for analyzing complex log files. The user must create a new parser in "Session Preference" -> "Parser Config" to cater to the log file. Once created, this will be saved into the workbox for reuse. The steps to load the files remain the same.

**Remark:** Once logs are sanitized for a particular format, the above steps are not required again in the future. Logs can be directly loaded onto the saved config, avoiding re-work on sanitization.

### Auto Loading From Config

This workflow is used to load log files where the parser config is already created and saved in the user's workbox or as a `.yaml` file. Follow these steps to load the log file from a config:

1. If you have the parser config saved as a `.yaml` on your computer, skip to step 3.
2. Select the "User Workbox" and then press "View Workbox." This will bring up a screen with Default and Custom parsers. Select the appropriate parser and skip to step 4.
3. Select "Start" and then press "Load Parser Config." This will open the file browser. Select the appropriate parser config file and load it.
4. Load the log file from the "Start" menu using the "Load Log File..." option. The selected file will be loaded as per the config from the above steps.

**Remark:** Parser config seen in the User Workbox are user/session-specific, determined by the entry given by the user during the application start-up screen.

## Change Logs

| Version | Changes                                                                                  | Release Date |
|---------|------------------------------------------------------------------------------------------|--------------|
| 1.0.0   | Initial Release for Sherlock Log Viewer                                                  | 7/10/2023    |
| 1.1.0   | Improvement for CSV file reading and minor improvements for log parsing                  | 17/10/2023   |
| 1.1.1   | Bug Fixes                                                                                | 22/10/2023   |
| 1.2.0   | Added Import and Export functionality to filters and Sherlock Log Viewer Auto Update app checker | 04/12/2023   |
