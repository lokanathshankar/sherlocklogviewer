import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { ColumnComponent, ColumnDefinition, RowComponent, TabulatorFull } from "tabulator-tables";

@Injectable({
  providedIn: 'root'
})
export class TableShellEvents {
  public infoAreaHeightChanged: Subject<number> = new Subject();
}
