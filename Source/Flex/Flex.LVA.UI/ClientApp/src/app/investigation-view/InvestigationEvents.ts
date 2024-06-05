import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";

@Injectable({
  providedIn: 'root'
})
export class InvestigationEvents {
  public noteFromSelection: Subject<void> = new Subject();
  public noteFromBookmarks: Subject<void> = new Subject();
  public noteFromFindResults: Subject<void> = new Subject();
}
