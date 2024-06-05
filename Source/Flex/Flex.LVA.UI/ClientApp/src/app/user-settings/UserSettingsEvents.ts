import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";
@Injectable({
  providedIn: 'root'
})
export class UserSettingsEvents {
  public userSettingsClicked: Subject<void> = new Subject();
  public exportUserSettings: Subject<void> = new Subject();
  public importUserSettings: Subject<void> = new Subject();
}

