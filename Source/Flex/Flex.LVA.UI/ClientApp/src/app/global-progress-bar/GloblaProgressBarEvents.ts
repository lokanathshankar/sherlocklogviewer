import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";

@Injectable({
  providedIn: 'root'
})
export class GlobalProgressBarEvents {
  public showProgress: Subject<string> = new Subject();
  public updateProgressPercent: Subject<number> = new Subject();
  public updateProgressMessage: Subject<string> = new Subject();
  public hideProgress: Subject<void> = new Subject();
}

