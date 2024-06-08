import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { ColumnComponent, ColumnDefinition, RowComponent, TabulatorFull } from "tabulator-tables";

export class ColumnSelection {
  public visible: boolean = true;
  public pinned: boolean | any = false;
  public value: string = "";
}



@Injectable({
  providedIn: 'root'
})
export class ColumnEvents {
  public hideColumn: Subject<string> = new Subject();
  public showColumn: Subject<string> = new Subject();
  public pinColumn: Subject<string> = new Subject();
  public unpinColumn: Subject<string> = new Subject();
}
