import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";


export enum FilterWith {
  Equals = "=",
  NotEquals = "!=",
  Like = "Like",
  Regex = "Regex",
}


export class FilterWithWhat {
  public with: string | undefined;
  public what: any = "";
  public column: string = "";
  public Or: any = false;
  public Visited: boolean = false;
  public Grouped: any = false;
  public rowNo: any;
  public Color: any = null
  public Guid: string | undefined;

  Equals(theInput: FilterWithWhat): boolean {
    return theInput.with === this.with && theInput.what === this.what && this.column === theInput.column && theInput.Or === this.Or;
  }
}

@Injectable({
  providedIn: 'root'
})
export class FilterEvents {
  public applyFilter: Subject<FilterWithWhat[]> = new Subject();
  public clearAllFilters: Subject<void> = new Subject();
}
