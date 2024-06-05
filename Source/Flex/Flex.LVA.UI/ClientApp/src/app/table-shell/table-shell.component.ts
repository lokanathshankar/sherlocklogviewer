import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { SplitComponent } from 'angular-split';
import { DialogService } from 'primeng/dynamicdialog';
import { Tabulator } from 'tabulator-tables';
import { MessageHelper } from '../../services/messagehelper';
import { UserWorkboxService } from '../../services/userworkbox.service';
import { FileUtils } from '../../utils/fileutils';
import { InfoAreaComponent } from '../info-area/info-area.component';
import { SessionLoginComponent } from '../session-login/session-login.component';
import { TableViewComponent } from '../table-view/table-view.component';
import { UserSettingsComponent } from '../user-settings/user-settings.component';
import { UserSettingsEvents } from '../user-settings/UserSettingsEvents';
import { TableShellEvents } from './TableShellEvents';

@Component({
  selector: 'app-table-shell',
  templateUrl: './table-shell.component.html',
  styleUrls: ['./table-shell.component.css']
})
export class TableShellComponent implements AfterViewInit {
  //@ts-ignore
  @ViewChild('getHeight') myHeight: any
  public infoAreaHeight: string = "200px";
  constructor(
    private myMessenger: MessageHelper,
    private myDialogService: DialogService,
    private myShellEvents: TableShellEvents,
    private myUserBoxEvents: UserSettingsEvents,
    private myUserWorkBox: UserWorkboxService) {
  //@ts-ignore
    this.myUserBoxEvents.userSettingsClicked.subscribe(() => {
      this.myDialogService.open(UserSettingsComponent, { header: 'User Workbox', maximizable: true, draggable: true, contentStyle: { overflow: 'auto' } });
    });

    this.myUserBoxEvents.exportUserSettings.subscribe(async () => {
      const aData = myUserWorkBox.GetWorkBoxData()
      if (!aData) {
        myMessenger.PostError("Workbox data missing, is login active?")
        return;
      }

      FileUtils.download(`${myUserWorkBox.CurrentUserId}.fubx`, aData);
    });

    this.myUserBoxEvents.importUserSettings.subscribe(() => {
      FileUtils.pickFile("*.fubx", (theFile: File) => {
        const aFileReader = new FileReader();
        aFileReader.onloadend = (_) => {
          //@ts-ignore
          if (!this.myUserWorkBox.ImportWorkBoxData(JSON.parse(aFileReader.result))) {
            myMessenger.PostError("Workbox data import failed..")
          } else {
            myMessenger.PostSuccess("Workbox data import done...")
          }
        };

        aFileReader.readAsText(theFile);
      });
    });
  }

  SetHeight(value: any) {
    this.myShellEvents.infoAreaHeightChanged.next(value)
  }

  ngAfterViewInit(): void {
    this.myDialogService.open(SessionLoginComponent, { data: {}, header: 'Simple Login', closable: false, closeOnEscape: false }).
      onClose.subscribe(() => {
          this.SetHeight(this.myHeight.nativeElement.offsetHeight);
      });
  }
}
