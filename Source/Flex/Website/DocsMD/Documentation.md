#### Loading a log file

Sherlock log viewer can load many log formats, for the most part built in auto detection feature will parse the log file however in some cases some extra steps are required to get the logs properly formatter. More is explained in below sections

##### Basic usage (Automatic Loading)

This workflow is sufficient for most of structured logs. Logs which are delimited, Json, XML, Windows Events(evtx) should just work out of the box. Below are the primary steps to open any log file

1. Press the "Start" from the top menu, navigate to "Load Log File...", this should open up file explorer.
2. Select and required file for investigation. Confirm the Auto detection popup by pressing "Yes", this should load the logs into the table after it's parsed.

##### Intermediate Usage (Semi-Automatic Loading)

This workflow is very useful if you have logs which are partially structured, i.e few columns separated by "Space" and other by "," and so on. To start with just load the log file using Start menu, this should load the logs partially into the table, once this is done follow the below steps to sanitize the logs.

1. Navigate to "Session Preference" from top menu and then press on "Parser Config". This brings up a dialog with the auto-detected columns. "Syntax List" is where all the columns are defined.
2. Edit the syntax list by pressing "Edit(![Alt text](image.png))" icon, this will bring "Edit Syntax". Optionally you can prefer to rename the parser name.
3. Identify the excess columns and use the "Delete(![Alt text](image-1.png))" icon to remove them. Optionally you can prefer to rename the columns. Once satisfied "Confirm" on "Edit Syntax" and "Parser Config"
4. Reload the log file using the "Start" from top menu again, this time the logs loaded to table should be properly formatted.

##### Advanced Usage (Manual Loading)
This workflow is rarely used, but is necessary when analysing complex log files. Here the user has to create a brand new parser from "Session Preference" ->  "Parser Config" catering to the log file which will be loaded. Once creating this will be saved into the workbox so it can be re-used later. And the steps to load the files from this stays same as [this](#auto-loading-from-config)


###### *Remark : Once logs are sanitized for particular format the about steps are not required again in the future, logs can directly loaded onto the saved config thus avoiding re-work on sanitization, see [this](#auto-loading-from-config) section*

###### *Remark : Currently Edit Syntax only supports removing the columns in place, any new column added will go to the end*

##### Auto Loading From Config

This workflow is used to load the logfiles where in the parser config is already created and saved in user's workbox or as a .yaml file. Below are the steps to load the log file from a config

1. If you have the parser config saved as .yaml on the computers's disk **skip to step 3**
2. Select the "User workbox" and then press on "View Workbox", this will bring up a screen with Default and Custom parsers, select the appropriate parser and **skip to step 4**
3. Select the "Start" and then press on "Load Parser Config", this will open up the file browser, select the appropriate parser config file and load it.
4. Load the log file from "Start" menu using "Load Log File..." option, the selected file will be loaded as per the config loaded from above steps.

###### *Remark : Parser config seen in User Workbox are per by user/session which is determined by the entry given by the user during application start up screen*
