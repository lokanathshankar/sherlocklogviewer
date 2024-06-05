import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";


export enum FindWith {
  Equals = "Same",
  Like = "Like",
  Regex = "Regex",
}


export class FindWithWhat {
  public With: FindWith = FindWith.Like;
  public What: string = "";

  Equals(theInput: FindWithWhat): boolean {
    return theInput.With === this.With && theInput.What === this.What;
  }
}

@Injectable({
  providedIn: 'root'
})
export class FindEvents {
  public findNext: Subject<FindWithWhat> = new Subject();
  public findPrevious: Subject<FindWithWhat> = new Subject();
  public findAll: Subject<FindWithWhat> = new Subject();
  public clearAll: Subject<void> = new Subject();
}
