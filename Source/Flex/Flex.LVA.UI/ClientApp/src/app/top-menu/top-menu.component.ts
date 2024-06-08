import { OnDestroy, OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api/menuitem';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ThemeService } from '../../services/theme-service.ts';
import { UiSessionService } from '../../services/uisession.service';
import { UserWorkboxService } from '../../services/userworkbox.service';
import { FileUtils } from '../../utils/fileutils';
import { TestDataLoader } from '../../utils/testdataloader';
import { AboutComponent } from '../about/about.component';
import { InvestigationEvents } from '../investigation-view/InvestigationEvents';
import { ParserSetupViewComponent } from '../parser-setup-view/parser-setup-view.component';
import { UserSettingsComponent } from '../user-settings/user-settings.component';
import { UserSettingsEvents } from '../user-settings/UserSettingsEvents';
import { MenuEvents } from './TopMenuEvents';
import { UpdateVersionChecker } from '../../services/updateversion.service';

@Component({
  selector: 'app-top-menu',
  templateUrl: './top-menu.component.html',
  styleUrls: ['./top-menu.component.css']
})
export class TopMenuComponent implements OnInit, OnDestroy {
  public menuItems: MenuItem[];
  public currentUser: string = "";
  public darkModeOn: boolean = false;
  public newVersionNo: string = "";
  public currentVersionNo: string = "";
  public packageJsonObject = require('package.json');
  public isUpdateAvailable: boolean = false;
  constructor(
    private myUpdateService: UpdateVersionChecker,
    private myThemeService :ThemeService,
    private myUserWorkBox: UserWorkboxService,
    private myUserSettingsEvents: UserSettingsEvents,
    private myInvEvents: InvestigationEvents,
    private myDialogService: DialogService,
    private myTopMenuEvents: MenuEvents,
    private myTestDataLoader: TestDataLoader,
    private mySessionService: UiSessionService) {
    this.menuItems = []
    this.myUserWorkBox.UserBoxLoaded.subscribe(() => {
      this.currentUser = this.myUserWorkBox.CurrentUserId;
    });
  }

  onThemeChange() {
    this.myThemeService.switchTheme(this.darkModeOn);
  }
  ngOnDestroy(): void {
    this.myUserWorkBox.UserBoxLoaded.unsubscribe();
  }

  ngOnInit(): void {
    this.menuItems = [
      {
        label: 'Start',
        icon: 'pi pi-fw pi-play',
        items: [
          {
            label: 'Load Log File...',
            icon: 'pi pi-fw pi-plus',
            command: () => this.onLoadClicked(),
          },
          {
            label: 'Reset Session',
            icon: 'pi pi-fw pi-refresh',
            command: () => this.mySessionService.ResetSession()
          },
          {
            label: 'Load Parser Config...',
            icon: 'pi pi-fw pi-sliders-h',
            command: () => this.onParserConifigClicked(),
          }
        ]
      },
      {
        label: 'Filter',
        icon: 'pi pi-fw pi-filter',
        items: [
          {
            label: 'Find...',
            icon: 'pi pi-fw pi-search',
            command: () => this.onSearchClicked(),
          },
          {
            label: 'Filter...',
            icon: 'pi pi-fw pi-filter-fill',
            command: () => this.onFilterClicked(),
          }
        ]
      },
      {
        label: 'Investigation',
        icon: 'pi pi-fw pi-book',
        items: [
          {
            label: 'From Slection...',
            icon: 'pi pi-fw pi-check-circle',
            command: () => this.myInvEvents.noteFromSelection.next(),
          },
          {
            label: 'From Bookmarks...',
            icon: 'pi pi-fw pi-bookmark',
            command: () => this.myInvEvents.noteFromBookmarks.next(),
          },
          {
            label: 'From Find Results...',
            icon: 'pi pi-fw pi-search',
            command: () => this.myInvEvents.noteFromFindResults.next(),
          },
        ]
      },
      {
        label: 'User Workbox',
        icon: 'pi pi-fw pi-user',
        items: [
          {
            label: 'View Workbox...',
            icon: 'pi pi-fw pi-eye',
            command: () => this.myUserSettingsEvents.userSettingsClicked.next()
          },
          {
            label: 'Export Workbox...',
            icon: 'pi pi-fw pi-upload',
            command: () => this.myUserSettingsEvents.exportUserSettings.next()
          },
          {
            label: 'Import Workbox',
            icon: 'pi pi-fw pi-download',
            command: () => this.myUserSettingsEvents.importUserSettings.next()
          },
        ]
      },
      {
        label: 'Session Preferences',
        icon: 'pi pi-fw pi-cog',
        items: [
          {
            label: 'Column Options...',
            icon: 'pi pi-fw pi-bars',
            command: () => this.onColumnOptionsClicked(),
          },
          {
            label: 'Parser Config...',
            icon: 'pi pi-fw pi-sliders-h',
            command: () => this.onParserConfigClicked(),
          }
        ]
      },
      {
        label: 'About',
        icon: 'pi pi-fw pi-cog',
        command: () => this.onAboutClicked(),
        //items: [
        //  //{
        //   // label: 'Documentation',
        //    //icon: 'pi pi-fw pi-info-circle',
        //   // command: () => this.onHelp(),
        //  //},
        //  {
        //    label: 'About',
        //    icon: 'pi pi-fw pi-briefcase',
        //    command: () => this.onAboutClicked(),
        //  }
        //]
      }
      
    ];

    if (process?.env?.NODE_ENV !== 'production') {
      this.menuItems.push({
        label: 'Load Test Data',
        icon: 'pi pi-fw pi-database',
        command: () => this.myTestDataLoader.Load(),
      });
    }
    //this.myUpdateService.IsUpdateAvailable();
    //this.myTopMenuEvents.isUpdateAvailable.subscribe((versionNumber: string) => {
    //  this.newVersionNo = versionNumber;
    //  this.currentVersionNo = this.packageJsonObject.version;
    //  if (this.currentVersionNo === this.newVersionNo) {
    //    console.log("App version is latest")
    //  }
    //  else {
    //    this.isUpdateAvailable = true;
    //  }
    //});
  }

  //private onHelp(): void {
   // // TODO
  //  window.open("https://github.com/lokanathshankar/sherlocklogviewer/blob/main/README.md", '_blank');
  //}

  private onParserConfigClicked(): void {
    this.myDialogService.open(ParserSetupViewComponent, { header: 'Parser Configuration', closable: true, closeOnEscape: true });
  }

  private onFilterClicked(): void {
    this.myTopMenuEvents.filterClicked.next();
  }

  private onColumnOptionsClicked(): void {
    this.myTopMenuEvents.columnOptionsClicked.next();
  }

  private onSearchClicked(): void {
    this.myTopMenuEvents.findClicked.next();
  }
  private onAboutClicked(): void {

    this.myDialogService.open(AboutComponent, { header: 'About Sherlock Log Viewer', closable: true, closeOnEscape: true });
  }
  
  private onLoadClicked(): void {
    FileUtils.pickFile("*.*", (file: File) => {
      this.myTopMenuEvents.logFileSelected.next(file)
    })
  }

  private onParserConifigClicked(): void {
    FileUtils.pickFile(".yaml", (file: File) => {
      this.myTopMenuEvents.parserConfigClicked.next(file)
    })
  }

  //onUpdateAvailable(): void {
  //  this.myUpdateService.OpenLocalBrowser();
  //}
}
