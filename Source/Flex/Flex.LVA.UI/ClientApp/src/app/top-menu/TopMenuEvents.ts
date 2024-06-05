import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { RowComponent } from "tabulator-tables";

@Injectable({
  providedIn: 'root'
})
export class MenuEvents {
  public parserConfigClicked: Subject<File> = new Subject();
  public logFileSelected: Subject<File> = new Subject();
  public columnOptionsClicked: Subject<void> = new Subject();
  public filterClicked: Subject<void> = new Subject();
  public findClicked: Subject<void> = new Subject();
  public isUpdateAvailable: Subject<string> = new Subject();
}
